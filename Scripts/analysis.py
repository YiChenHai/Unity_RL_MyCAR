#!/usr/bin/env python3
"""
高级蒸馏工具：特征分析、可视化、多模型对比

使用方法：
1. 生成if规则库（自动优化，推荐，平衡R²和代码大小）：
   python analysis.py --data <CSV文件路径> --rules [输出文件路径] --auto-optimize --target-r2 0.95 --max-code-lines 50000

2. 生成if规则库（限制代码大小，适合Unity）：
   python analysis.py --data <CSV文件路径> --rules [输出文件路径] --auto-optimize --target-r2 0.90 --max-code-lines 20000

3. 生成if规则库（手动指定参数）：
   python analysis.py --data <CSV文件路径> --rules [输出文件路径] --depth 20 --min-leaf 3 --min-split 6

4. 对比不同深度的性能：
   python analysis.py --data <CSV文件路径> --compare

5. 分析特征重要性：
   python analysis.py --data <CSV文件路径> --analysis

6. 完整流程（推荐）：
   python analysis.py --data <CSV文件路径> --compare --rules DecisionTreeRules.cs --auto-optimize --target-r2 0.95 --max-code-lines 50000

参数说明：
  --auto-optimize: 自动优化参数以达到目标R²（强烈推荐！会自动寻找最优参数，同时限制代码大小）
  --target-r2: 自动优化模式下的目标R²分数（默认0.95，建议0.90-0.95）
  --max-code-lines: 最大代码行数限制（默认50000，建议20000-50000，超过可能导致Unity崩溃）
  --no-warn-large: 不警告大型代码文件（不推荐）
  --depth: 决策树最大深度（默认8，手动模式建议15-25）
  --min-leaf: 叶子节点最小样本数（默认5，越大代码越小，建议3-5）
  --min-split: 分裂节点最小样本数（默认10，越大代码越小，建议6-10）
  --optimize-w: 针对action_w进行优化（增加深度，改善弯道跟踪）
  --w-depth-boost: optimize-w模式下，action_w树的深度增加量（默认2）
  --max-features: 每次分裂考虑的最大特征数（sqrt/log2，默认全部）
  --ccp-alpha: 最小成本复杂度剪枝参数（默认0=不剪枝）

优化策略说明：
  - 自动优化会优先选择满足R²要求且代码最小的参数组合
  - 如果无法同时满足R²和代码大小要求，会优先满足R²，然后尽量减小代码
  - 建议：R²=0.90-0.95，代码行数<50000，这样既能保证性能又不会导致Unity崩溃

示例：
   # 自动优化，平衡R²和代码大小（推荐）
   python analysis.py --data training_data.csv --rules if_rules.cs --auto-optimize --target-r2 0.95 --max-code-lines 50000
   
   # 限制代码大小，适合Unity（如果Unity崩溃，使用这个）
   python analysis.py --data training_data.csv --rules if_rules.cs --auto-optimize --target-r2 0.90 --max-code-lines 20000
   
   # 手动指定参数（高级用户）
   python analysis.py --data training_data.csv --rules if_rules.cs --depth 20 --min-leaf 3 --min-split 6
   
   # 对比性能并自动优化
   python analysis.py --data D:/Data/episode_data.csv --compare --rules --auto-optimize --max-code-lines 30000
"""

import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
from sklearn.tree import DecisionTreeRegressor
from sklearn.ensemble import RandomForestRegressor
from sklearn.metrics import mean_squared_error, mean_absolute_error, r2_score
from sklearn.model_selection import GridSearchCV
import json
import seaborn as sns

# ============================================================================
# 全局配置参数（可直接在此修改，无需命令行参数）
# ============================================================================

# ========== 数据文件配置 ==========
# CSV数据文件路径（必需）
DATA_FILE_PATH = "D:/XiaoYiFei/Project/Unity/XYF_Car_Test/Data_Record/training_data_20260124_193252.csv"

# ========== 功能选项配置 ==========
# 是否分析特征重要性（生成特征重要性图表）
ENABLE_ANALYSIS = False

# 是否对比模型性能（生成性能对比图表）
ENABLE_COMPARE = False

# 是否可视化决策边界（生成决策边界图）
ENABLE_BOUNDARY = False

# 是否导出JSON模型（None=不导出，字符串=导出路径）
EXPORT_JSON = None  # 例如: "model.json" 或 None

# 是否生成if规则代码（None=不生成，字符串=输出文件路径）
GENERATE_RULES = "DecisionTreeRules2601252.cs"  # 例如: "DecisionTreeRules.cs" 或 None

# ========== 自动优化配置（推荐使用） ==========
# 是否启用自动参数优化（True=自动寻找最优参数，False=使用手动参数）
USE_AUTO_OPTIMIZE = True

# 目标R²分数（自动优化模式下，会尽量达到此R²值）
# 建议范围：0.90-0.95（越高精度越好，但代码量可能越大）
# 注意：如果设置为0.90，优化器可能在达到0.90后就停止搜索，导致R²值较低
# 建议设置为0.93-0.95以获得更高的精度
# 可以分别设置 action_x 和 action_w 的目标 R²
TARGET_R2_X = 0.8  # action_x 的目标 R²（横向速度）
TARGET_R2_W = 0.93  # action_w 的目标 R²（角速度）

# 最大代码行数限制（超过此值会警告，建议20000-500000）
# 注意：代码行数过多可能导致Unity编译或运行问题
MAX_CODE_LINES = 300000

# ========== 手动参数配置（仅在 USE_AUTO_OPTIMIZE=False 时生效） ==========
# 决策树最大深度（建议15-25，越大越复杂，代码量越大）
MANUAL_DEPTH = 25

# 叶子节点最小样本数（建议3-5，越大代码越小）
MANUAL_MIN_LEAF = 3

# 分裂节点最小样本数（建议6-10，越大代码越小）
MANUAL_MIN_SPLIT = 6

# 是否针对action_w进行优化（True=增加action_w树的深度，改善弯道跟踪）
OPTIMIZE_W = True

# action_w树的深度增量（仅在OPTIMIZE_W=True时生效，建议2-4）
W_DEPTH_BOOST = 2

# 每次分裂考虑的最大特征数（None=全部，'sqrt'=平方根，'log2'=对数）
# 用于减少过拟合，通常不需要修改
MAX_FEATURES = None  # 可选: 'sqrt', 'log2', None

# 最小成本复杂度剪枝参数（0=不剪枝，越大代码越小，但可能降低精度）
# 通常不需要修改，保持0即可
CCP_ALPHA = 0.0

# ========== 其他配置 ==========
# 规则代码语言（'csharp'=C#代码，'python'=Python代码）
CODE_LANGUAGE = 'csharp'

# 是否警告大型代码文件（True=警告，False=不警告）
WARN_ON_LARGE = True

# ============================================================================
# 配置说明：
# 1. 直接修改上面的全局变量即可，无需使用命令行参数
# 2. 命令行参数仍然可用，会覆盖全局变量设置
# 3. 推荐配置：
#    - USE_AUTO_OPTIMIZE = True（自动优化）
#    - TARGET_R2 = 0.93（目标精度）
#    - MAX_CODE_LINES = 300000（代码行数限制）
#    - GENERATE_RULES = "DecisionTreeRules.cs"（生成规则库）
# ============================================================================

class AdvancedDistillation:
    def __init__(self, csv_path):
        # 智能加载CSV：检查是否有表头，处理数据格式
        self.data = pd.read_csv(csv_path)
        self.feature_names = [
            "sensor0", "sensor1", "sensor2", "sensor3", "sensor4", "sensor5",
            "smoothed_vx", "smoothed_omega",
            "raw_action_x", "raw_action_w",
            "actual_vz", "actual_vx", "actual_omega"
        ]
        
        # 检查是否有预期的列名（有表头的CSV）
        has_expected_header = all(col in self.data.columns for col in self.feature_names + ["action_x", "action_w"])
        
        if has_expected_header:
            # 有表头：使用列名索引（更安全）
            X_data = self.data[self.feature_names]
            self.y_x = self.data["action_x"].values
            self.y_w = self.data["action_w"].values
        else:
            # 无表头或列名不匹配：使用位置索引
            # 检查第一行是否为数值（如果是，说明无表头）
            first_row_numeric = all(isinstance(val, (int, float)) or (isinstance(val, str) and self._is_number_string(val)) 
                                   for val in self.data.iloc[0, :15] if pd.notna(val))
            
            if first_row_numeric:
                # 无表头：直接使用位置索引
                X_data = self.data.iloc[:, :13]
                self.y_x = self.data.iloc[:, 13].values
                self.y_w = self.data.iloc[:, 14].values
            else:
                # 有表头但列名不匹配：跳过第一行（表头），使用位置索引
                print("⚠️  警告: CSV表头列名不匹配，跳过第一行（表头）")
                X_data = self.data.iloc[1:, :13]
                self.y_x = self.data.iloc[1:, 13].values
                self.y_w = self.data.iloc[1:, 14].values
        
        # 数据预处理：转换为数值类型，处理缺失值
        X_data = X_data.apply(pd.to_numeric, errors="coerce")
        before = len(X_data)
        X_data = X_data.dropna()
        dropped = before - len(X_data)
        if dropped > 0:
            print(f"⚠️  发现 {dropped} 行非数值或缺失数据，已自动丢弃")
        
        # 确保目标值也是数值类型（转换为Series以便后续处理）
        y_x_series = pd.Series(pd.to_numeric(self.y_x, errors="coerce"))
        y_w_series = pd.Series(pd.to_numeric(self.y_w, errors="coerce"))
        
        # 移除对应的目标值中的NaN
        valid_mask = pd.notna(y_x_series) & pd.notna(y_w_series)
        X_data = X_data[valid_mask]
        self.y_x = y_x_series[valid_mask].values
        self.y_w = y_w_series[valid_mask].values
        
        self.X = X_data.values.astype(np.float32)
        self.y_x = self.y_x.astype(np.float32)
        self.y_w = self.y_w.astype(np.float32)
        
        # 数据质量检查
        self.check_data_quality()
    
    def _is_number_string(self, text):
        """检查字符串是否为数字"""
        try:
            float(text)
            return True
        except Exception:
            return False
    
    def check_data_quality(self):
        """检查数据质量并输出统计信息"""
        print("\n=== 数据质量检查 ===")
        print(f"总样本数: {len(self.data)}")
        print(f"特征维度: {self.X.shape[1]}")
        
        # 检查缺失值
        missing = self.data.isnull().sum().sum()
        if missing > 0:
            print(f"⚠️  发现缺失值: {missing} 个")
        else:
            print("✓ 无缺失值")
        
        # 检查动作值范围
        print(f"\n动作值统计:")
        print(f"  action_x: min={self.y_x.min():.4f}, max={self.y_x.max():.4f}, mean={self.y_x.mean():.4f}, std={self.y_x.std():.4f}")
        print(f"  action_w: min={self.y_w.min():.4f}, max={self.y_w.max():.4f}, mean={self.y_w.mean():.4f}, std={self.y_w.std():.4f}")
        
        # 检查动作值分布（是否过于集中）
        x_range = self.y_x.max() - self.y_x.min()
        w_range = self.y_w.max() - self.y_w.min()
        
        if x_range < 0.1:
            print(f"⚠️  action_x值域过小 ({x_range:.4f})，可能缺乏多样性")
        if w_range < 0.1:
            print(f"⚠️  action_w值域过小 ({w_range:.4f})，可能缺乏多样性")
        
        # 检查角速度样本分布（弯道跟踪的关键）
        abs_w = np.abs(self.y_w)
        high_w_samples = np.sum(abs_w > 0.3)  # 角速度绝对值>0.3的样本
        high_w_ratio = high_w_samples / len(self.y_w)
        
        print(f"\n角速度分布分析（弯道跟踪关键指标）:")
        print(f"  |action_w| > 0.3 的样本: {high_w_samples} ({high_w_ratio*100:.1f}%)")
        if high_w_ratio < 0.15:
            print(f"  ⚠️  警告: 高角速度样本比例较低，可能导致弯道跟踪能力不足")
            print(f"     建议: 增加弯道场景的训练数据")
        
        # 检查传感器值分布
        sensor_cols = [f"sensor{i}" for i in range(6)]
        sensor_data = self.data[sensor_cols]
        print(f"\n传感器值统计:")
        for col in sensor_cols:
            if col in self.data.columns:
                print(f"  {col}: min={sensor_data[col].min():.4f}, max={sensor_data[col].max():.4f}, mean={sensor_data[col].mean():.4f}")
        
    def analyze_feature_importance(self, model_name="DecisionTree"):
        """分析特征重要性"""
        tree_x = DecisionTreeRegressor(max_depth=10, min_samples_leaf=5, random_state=42)
        tree_w = DecisionTreeRegressor(max_depth=10, min_samples_leaf=5, random_state=42)
        
        tree_x.fit(self.X, self.y_x)
        tree_w.fit(self.X, self.y_w)
        
        # 特征重要性排序
        importance_x = tree_x.feature_importances_
        importance_w = tree_w.feature_importances_
        
        print("\n=== 特征重要性分析 ===")
        print("\n[横向速度 action_x]")
        sorted_idx_x = np.argsort(importance_x)[::-1]
        for idx in sorted_idx_x[:5]:
            print(f"  {self.feature_names[idx]:20s}: {importance_x[idx]:.4f}")
        
        print("\n[角速度 action_w]")
        sorted_idx_w = np.argsort(importance_w)[::-1]
        for idx in sorted_idx_w[:5]:
            print(f"  {self.feature_names[idx]:20s}: {importance_w[idx]:.4f}")
        
        # 绘制特征重要性
        fig, axes = plt.subplots(1, 2, figsize=(14, 5))
        
        ax = axes[0]
        top_idx_x = sorted_idx_x[:10]
        ax.barh([self.feature_names[i] for i in top_idx_x], importance_x[top_idx_x])
        ax.set_xlabel("Importance")
        ax.set_title("Top 10 Features for action_x")
        ax.invert_yaxis()
        
        ax = axes[1]
        top_idx_w = sorted_idx_w[:10]
        ax.barh([self.feature_names[i] for i in top_idx_w], importance_w[top_idx_w])
        ax.set_xlabel("Importance")
        ax.set_title("Top 10 Features for action_w")
        ax.invert_yaxis()
        
        plt.tight_layout()
        plt.savefig("feature_importance.png", dpi=150)
        print("\n✓ 特征重要性图表已保存: feature_importance.png")
        
    def compare_models(self, optimize_w=False, w_depth_boost=2):
        """对比不同深度的决策树性能"""
        depths = [3, 5, 7, 8, 10, 12, 15]
        results = {"depth": [], "r2_x": [], "r2_w": [], "mse_x": [], "mse_w": [], "nodes_x": [], "nodes_w": []}
        
        print("\n=== 不同树深度的性能对比 ===")
        if optimize_w:
            print(f"⚡ action_w优化模式已启用（深度+{w_depth_boost}）")
        print(f"{'Depth':<8} {'R2_x':<10} {'R2_w':<10} {'MSE_x':<12} {'MSE_w':<12} {'Nodes':<12} {'Code Est.'}")
        print("-" * 80)
        
        for depth in depths:
            depth_x = depth
            depth_w = depth + (w_depth_boost if optimize_w else 0)
            
            min_leaf_w = 4 if optimize_w else 5
            min_split_w = 8 if optimize_w else 10
            
            tree_x = DecisionTreeRegressor(max_depth=depth_x, min_samples_leaf=5, min_samples_split=10, random_state=42)
            tree_w = DecisionTreeRegressor(max_depth=depth_w, min_samples_leaf=min_leaf_w, min_samples_split=min_split_w, random_state=42)
            
            tree_x.fit(self.X, self.y_x)
            tree_w.fit(self.X, self.y_w)
            
            r2_x = tree_x.score(self.X, self.y_x)
            r2_w = tree_w.score(self.X, self.y_w)
            
            pred_x = tree_x.predict(self.X)
            pred_w = tree_w.predict(self.X)
            
            mse_x = mean_squared_error(self.y_x, pred_x)
            mse_w = mean_squared_error(self.y_w, pred_w)
            
            # 节点数和代码行数估算
            n_nodes_x = tree_x.tree_.node_count
            n_nodes_w = tree_w.tree_.node_count
            code_size = (n_nodes_x + n_nodes_w) * 4
            
            results["depth"].append(depth)
            results["r2_x"].append(r2_x)
            results["r2_w"].append(r2_w)
            results["mse_x"].append(mse_x)
            results["mse_w"].append(mse_w)
            results["nodes_x"].append(n_nodes_x)
            results["nodes_w"].append(n_nodes_w)
            
            depth_str = f"{depth_x}/{depth_w}" if optimize_w and depth_w != depth_x else str(depth)
            print(f"{depth_str:<8} {r2_x:<10.4f} {r2_w:<10.4f} {mse_x:<12.6f} {mse_w:<12.6f} {n_nodes_x+n_nodes_w:<12} ~{code_size} lines")
        
        # 绘制性能曲线
        fig, axes = plt.subplots(1, 2, figsize=(14, 5))
        
        ax = axes[0]
        ax.plot(results["depth"], results["r2_x"], marker='o', label="action_x", linewidth=2)
        ax.plot(results["depth"], results["r2_w"], marker='s', label="action_w", linewidth=2)
        ax.axhline(y=0.90, color='r', linestyle='--', alpha=0.5, label="0.90 threshold")
        ax.set_xlabel("Tree Depth")
        ax.set_ylabel("R² Score")
        ax.set_title("R² vs Tree Depth")
        ax.legend()
        ax.grid(True, alpha=0.3)
        
        ax = axes[1]
        ax.plot(results["depth"], results["mse_x"], marker='o', label="action_x", linewidth=2)
        ax.plot(results["depth"], results["mse_w"], marker='s', label="action_w", linewidth=2)
        ax.set_xlabel("Tree Depth")
        ax.set_ylabel("MSE")
        ax.set_title("MSE vs Tree Depth")
        ax.legend()
        ax.grid(True, alpha=0.3)
        
        plt.tight_layout()
        plt.savefig("model_comparison.png", dpi=150)
        print("\n✓ 性能对比图表已保存: model_comparison.png")
        
        # 推荐深度
        print("\n[建议]")
        idx_best = np.argmax(np.array(results["r2_x"]) + np.array(results["r2_w"]))
        best_depth = results["depth"][idx_best]
        print(f"  推荐深度: {best_depth} (R² avg: {(results['r2_x'][idx_best] + results['r2_w'][idx_best])/2:.4f})")
        
    def analyze_decision_boundaries(self, feature_pair=(0, 6)):
        """分析两个特征之间的决策边界"""
        feat1, feat2 = feature_pair
        
        tree_x = DecisionTreeRegressor(max_depth=8, min_samples_leaf=5, random_state=42)
        tree_x.fit(self.X, self.y_x)
        
        # 创建网格
        x_min, x_max = self.X[:, feat1].min(), self.X[:, feat1].max()
        y_min, y_max = self.X[:, feat2].min(), self.X[:, feat2].max()
        
        xx, yy = np.meshgrid(
            np.linspace(x_min, x_max, 50),
            np.linspace(y_min, y_max, 50)
        )
        
        # 预测（保持其他特征为中位数）
        Z = np.zeros_like(xx)
        X_temp = np.tile(np.median(self.X, axis=0), (xx.size, 1))
        X_temp[:, feat1] = xx.ravel()
        X_temp[:, feat2] = yy.ravel()
        Z_pred = tree_x.predict(X_temp).reshape(xx.shape)
        
        # 绘制
        fig, ax = plt.subplots(figsize=(10, 8))
        
        # 热力图
        im = ax.contourf(xx, yy, Z_pred, levels=20, cmap='RdYlGn')
        plt.colorbar(im, ax=ax, label='action_x')
        
        # 实际数据点
        scatter = ax.scatter(self.X[:, feat1], self.X[:, feat2], 
                           c=self.y_x, cmap='RdYlGn', s=30, alpha=0.5, edgecolors='k')
        
        ax.set_xlabel(self.feature_names[feat1])
        ax.set_ylabel(self.feature_names[feat2])
        ax.set_title(f"Decision Boundary: {self.feature_names[feat1]} vs {self.feature_names[feat2]}")
        
        plt.tight_layout()
        plt.savefig(f"boundary_{self.feature_names[feat1]}_vs_{self.feature_names[feat2]}.png", dpi=150)
        print(f"\n✓ 决策边界已保存: boundary_*.png")
        
    def find_optimal_params(self, target_r2_x=0.95, target_r2_w=0.95, max_iterations=50, max_code_lines=50000):
        """
        自动寻找最优参数，使R²接近目标值，同时限制代码大小
        使用智能搜索策略：先粗搜索，再精细调整
        优先级：R²优先，代码行数次要（与generate_rules.py保持一致）
        确保 action_x 和 action_w 都达到各自的目标 R²
        
        Args:
            target_r2_x: action_x 的目标R²分数（默认0.95）
            target_r2_w: action_w 的目标R²分数（默认0.95）
            max_iterations: 最大迭代次数（默认50）
            max_code_lines: 最大代码行数限制（默认50000）
        
        Returns:
            dict: 包含最优参数的字典
        """
        print(f"\n=== 自动参数优化 ===")
        print(f"  目标R² - action_x: {target_r2_x:.2f}, action_w: {target_r2_w:.2f}")
        print(f"  最大代码行数: {max_code_lines}")
        
        def estimate_code_lines(tree):
            """估算决策树生成的代码行数"""
            return tree.tree_.node_count * 4  # 每个节点约4行代码
        
        def optimize_tree(y_data, name, target_r2, max_code_lines_per_tree):
            """优化单个决策树，平衡R²和代码大小"""
            best_params = None
            best_r2 = 0.0
            best_code_lines = float('inf')
            best_score = -float('inf')  # 综合评分：R²优先，代码大小次之
            
            # 第一阶段：粗搜索（快速找到大致范围）
            print(f"\n[{name} - 第一阶段：粗搜索]")
            # 从较小的深度开始，逐步增加，优先选择代码更小的方案
            # 扩大搜索范围，与generate_rules.py保持一致
            depth_range_coarse = [8, 12, 16, 20, 24, 28, 32, 36, 40, 44, 48]
            min_leaf_coarse = [2, 3, 4, 5]  # 从较大的值开始，减少代码量
            min_split_coarse = [4, 6, 8, 10]
            
            iteration = 0
            for depth in depth_range_coarse:
                for min_leaf in min_leaf_coarse:
                    for min_split in min_split_coarse:
                        if iteration >= max_iterations // 2:
                            break
                        
                        try:
                            tree = DecisionTreeRegressor(
                                max_depth=depth,
                                min_samples_leaf=min_leaf,
                                min_samples_split=min_split,
                                random_state=42
                            )
                            tree.fit(self.X, y_data)
                            r2 = tree.score(self.X, y_data)
                            code_lines = estimate_code_lines(tree)
                            
                            # 优先级：R²优先，代码行数次要（与generate_rules.py保持一致）
                            # 如果R²达到目标，在满足目标的方案中选择代码最小的
                            if r2 >= target_r2:
                                # 达到目标R²后，优先选择代码更小的（但不会为了代码更小而牺牲R²）
                                if best_r2 < target_r2 or code_lines < best_code_lines:
                                    # 第一次达到目标，或者代码更小（R²已经满足目标）
                                    best_r2 = r2
                                    best_code_lines = code_lines
                                    best_params = {
                                        'max_depth': depth,
                                        'min_samples_leaf': min_leaf,
                                        'min_samples_split': min_split
                                    }
                                    print(f"  迭代 {iteration+1}: R²={r2:.4f}, 代码行数≈{code_lines} (depth={depth}, leaf={min_leaf}, split={min_split})")
                            elif r2 > best_r2:
                                # 未达到目标时，优先选择R²更高的
                                best_r2 = r2
                                best_code_lines = code_lines
                                best_params = {
                                    'max_depth': depth,
                                    'min_samples_leaf': min_leaf,
                                    'min_samples_split': min_split
                                }
                                print(f"  迭代 {iteration+1}: R²={r2:.4f}, 代码行数≈{code_lines} (depth={depth}, leaf={min_leaf}, split={min_split})")
                        except:
                            pass
                        
                        iteration += 1
            
            # 第二阶段：精细调整（在最佳参数附近搜索，优先选择代码更小的）
            if best_params:
                print(f"\n[{name} - 第二阶段：精细调整]")
                base_depth = best_params['max_depth'] or 50
                base_leaf = best_params['min_samples_leaf']
                base_split = best_params['min_samples_split']
                
                # 在最佳参数附近搜索，优先尝试增加min_leaf和min_split（减少代码）
                depth_range_fine = [base_depth - 3, base_depth - 1, base_depth, base_depth + 1, base_depth + 3]
                min_leaf_fine = [max(2, base_leaf - 1), base_leaf, min(6, base_leaf + 1), min(8, base_leaf + 2)]
                min_split_fine = [max(4, base_split - 2), base_split, min(12, base_split + 2), min(15, base_split + 4)]
                
                iteration = max_iterations // 2
                for depth in depth_range_fine:
                    if depth < 1:
                        continue
                    for min_leaf in min_leaf_fine:
                        for min_split in min_split_fine:
                            if iteration >= max_iterations:
                                break
                            
                            try:
                                tree = DecisionTreeRegressor(
                                    max_depth=depth,
                                    min_samples_leaf=min_leaf,
                                    min_samples_split=min_split,
                                    random_state=42
                                )
                                tree.fit(self.X, y_data)
                                r2 = tree.score(self.X, y_data)
                                code_lines = estimate_code_lines(tree)
                                
                                # 优先级：R²优先，代码行数次要（与generate_rules.py保持一致）
                                if r2 >= target_r2:
                                    # 达到目标R²后，优先选择代码更小的（但不会为了代码更小而牺牲R²）
                                    if best_r2 < target_r2 or code_lines < best_code_lines:
                                        # 第一次达到目标，或者代码更小（R²已经满足目标）
                                        best_r2 = r2
                                        best_code_lines = code_lines
                                        best_params = {
                                            'max_depth': depth,
                                            'min_samples_leaf': min_leaf,
                                            'min_samples_split': min_split
                                        }
                                        print(f"  迭代 {iteration+1}: R²={r2:.4f}, 代码行数≈{code_lines} (depth={depth}, leaf={min_leaf}, split={min_split})")
                                elif r2 > best_r2:
                                    # 未达到目标时，优先选择R²更高的
                                    best_r2 = r2
                                    best_code_lines = code_lines
                                    best_params = {
                                        'max_depth': depth,
                                        'min_samples_leaf': min_leaf,
                                        'min_samples_split': min_split
                                    }
                                    print(f"  迭代 {iteration+1}: R²={r2:.4f}, 代码行数≈{code_lines} (depth={depth}, leaf={min_leaf}, split={min_split})")
                            except:
                                pass
                            
                            iteration += 1
            
            return best_params, best_r2
        
        # 优化action_x和action_w，每个树分配一半的代码行数限制
        # 确保两个目标都满足，优先满足R²要求
        max_code_lines_per_tree = max_code_lines // 2
        params_x, r2_x = optimize_tree(self.y_x, "action_x", target_r2_x, max_code_lines_per_tree)
        params_w, r2_w = optimize_tree(self.y_w, "action_w", target_r2_w, max_code_lines_per_tree)
        
        # 估算总代码行数
        tree_x_temp = DecisionTreeRegressor(
            max_depth=params_x['max_depth'] or 50,
            min_samples_leaf=params_x['min_samples_leaf'],
            min_samples_split=params_x['min_samples_split'],
            random_state=42
        )
        tree_w_temp = DecisionTreeRegressor(
            max_depth=params_w['max_depth'] or 50,
            min_samples_leaf=params_w['min_samples_leaf'],
            min_samples_split=params_w['min_samples_split'],
            random_state=42
        )
        tree_x_temp.fit(self.X, self.y_x)
        tree_w_temp.fit(self.X, self.y_w)
        total_code_lines = estimate_code_lines(tree_x_temp) + estimate_code_lines(tree_w_temp) + 50
        
        print(f"\n[优化结果]")
        print(f"  action_x: R²={r2_x:.4f}, 参数={params_x}, 代码行数≈{estimate_code_lines(tree_x_temp)}")
        print(f"  action_w: R²={r2_w:.4f}, 参数={params_w}, 代码行数≈{estimate_code_lines(tree_w_temp)}")
        print(f"  总代码行数估算: ≈{total_code_lines}")
        
        # 检查是否达到各自的目标R²
        if r2_x < target_r2_x:
            print(f"\n  ⚠️  action_x未达到目标R² ({r2_x:.4f} < {target_r2_x:.2f})")
            print(f"     建议: 增加训练数据量或检查数据质量")
            print(f"     或者: 降低 TARGET_R2_X 到 {r2_x:.2f} 或更低")
        else:
            print(f"  ✓ action_x已达到目标R² ({r2_x:.4f} >= {target_r2_x:.2f})")
            
        if r2_w < target_r2_w:
            print(f"\n  ⚠️  action_w未达到目标R² ({r2_w:.4f} < {target_r2_w:.2f})")
            print(f"     建议: 增加弯道场景的训练数据")
            print(f"     或者: 降低 TARGET_R2_W 到 {r2_w:.2f} 或更低")
        else:
            print(f"  ✓ action_w已达到目标R² ({r2_w:.4f} >= {target_r2_w:.2f})")
        
        if total_code_lines > max_code_lines:
            print(f"\n  ⚠️  警告: 总代码行数 ({total_code_lines}) 超过限制 ({max_code_lines})")
            print(f"     建议: 降低目标R²或增加 --max-code-lines 参数")
        
        return {
            'action_x': params_x,
            'action_w': params_w,
            'r2_x': r2_x,
            'r2_w': r2_w
        }
    
    def generate_json_model(self, output_path="model.json", max_depth=8):
        """导出为JSON格式（便于手工优化或跨平台部署）"""
        tree_x = DecisionTreeRegressor(max_depth=max_depth, min_samples_leaf=5, random_state=42)
        tree_w = DecisionTreeRegressor(max_depth=max_depth, min_samples_leaf=5, random_state=42)
        
        tree_x.fit(self.X, self.y_x)
        tree_w.fit(self.X, self.y_w)
        
        def tree_to_dict(tree, feature_names):
            """递归将树转换为字典"""
            def recurse(node):
                if tree.feature[node] == -2:  # 叶子节点
                    return {
                        "type": "leaf",
                        "value": float(tree.value[node][0])
                    }
                else:
                    return {
                        "type": "node",
                        "feature": int(tree.feature[node]),
                        "feature_name": feature_names[tree.feature[node]],
                        "threshold": float(tree.threshold[node]),
                        "left": recurse(tree.children_left[node]),
                        "right": recurse(tree.children_right[node])
                    }
            return recurse(0)
        
        model = {
            "metadata": {
                "samples": len(self.data),
                "features": len(self.feature_names),
                "feature_names": self.feature_names
            },
            "trees": {
                "action_x": {
                    "depth": max_depth,
                    "r2_score": float(tree_x.score(self.X, self.y_x)),
                    "tree": tree_to_dict(tree_x.tree_, self.feature_names)
                },
                "action_w": {
                    "depth": max_depth,
                    "r2_score": float(tree_w.score(self.X, self.y_w)),
                    "tree": tree_to_dict(tree_w.tree_, self.feature_names)
                }
            }
        }
        
        with open(output_path, 'w') as f:
            json.dump(model, f, indent=2)
        
        print(f"✓ JSON模型已保存: {output_path}")
    
    def generate_if_rules(self, output_path="if_rules.cs", max_depth=8, language="csharp",
                          min_samples_leaf=5, min_samples_split=10, 
                          max_features=None, ccp_alpha=0.0,
                          optimize_w=False, w_depth_boost=2,
                          auto_optimize=False, target_r2_x=0.95, target_r2_w=0.95,
                          max_code_lines=50000, warn_on_large=True):
        """
        生成if-else规则代码（可直接用于Unity C#）
        
        Args:
            output_path: 输出文件路径
            max_depth: 决策树最大深度
            language: 输出语言 ("csharp" 或 "python")
            min_samples_leaf: 叶子节点最小样本数（默认5）
            min_samples_split: 分裂节点最小样本数（默认10）
            max_features: 每次分裂考虑的最大特征数（None=全部，'sqrt'=sqrt(n_features)，'log2'=log2(n_features)）
            ccp_alpha: 最小成本复杂度剪枝参数（0=不剪枝，越大越简单）
            optimize_w: 是否针对action_w进行优化（增加深度或调整参数）
            w_depth_boost: 如果optimize_w=True，action_w树的深度增加量
            auto_optimize: 是否自动优化参数以达到目标R²（默认False）
            target_r2_x: action_x 的目标R²分数（默认0.95）
            target_r2_w: action_w 的目标R²分数（默认0.95）
        """
        # 自动优化模式：寻找最优参数
        if auto_optimize:
            print("\n" + "="*60)
            print("自动优化模式已启用，正在寻找最优参数...")
            print(f"目标: action_x R²>={target_r2_x:.2f}, action_w R²>={target_r2_w:.2f}")
            print(f"代码行数限制: <={max_code_lines}")
            print("="*60)
            optimal_params = self.find_optimal_params(
                target_r2_x=target_r2_x, 
                target_r2_w=target_r2_w, 
                max_code_lines=max_code_lines
            )
            
            # 使用找到的最优参数
            depth_x = optimal_params['action_x']['max_depth'] or 50  # None表示不限制，设为50作为上限
            depth_w = optimal_params['action_w']['max_depth'] or 50
            min_samples_leaf = optimal_params['action_x']['min_samples_leaf']
            min_samples_split = optimal_params['action_x']['min_samples_split']
            min_leaf_w = optimal_params['action_w']['min_samples_leaf']
            min_split_w = optimal_params['action_w']['min_samples_split']
            
            print(f"\n使用优化后的参数:")
            print(f"  action_x: depth={depth_x}, min_leaf={min_samples_leaf}, min_split={min_samples_split}, R²={optimal_params['r2_x']:.4f}")
            print(f"  action_w: depth={depth_w}, min_leaf={min_leaf_w}, min_split={min_split_w}, R²={optimal_params['r2_w']:.4f}")
        else:
            # 手动参数模式
            # 针对action_w优化：如果R²较低，使用更深的树
            depth_x = max_depth
            depth_w = max_depth + (w_depth_boost if optimize_w else 0)
            
            # 为action_w使用更精细的参数（如果optimize_w=True）
            min_leaf_w = min_samples_leaf - 1 if optimize_w and min_samples_leaf > 2 else min_samples_leaf
            min_split_w = min_samples_split - 2 if optimize_w and min_samples_split > 3 else min_samples_split
        
        tree_x = DecisionTreeRegressor(
            max_depth=depth_x, 
            min_samples_leaf=min_samples_leaf,
            min_samples_split=min_samples_split,
            max_features=max_features,
            ccp_alpha=ccp_alpha,
            random_state=42
        )
        tree_w = DecisionTreeRegressor(
            max_depth=depth_w,
            min_samples_leaf=min_leaf_w,
            min_samples_split=min_split_w,
            max_features=max_features,
            ccp_alpha=ccp_alpha,
            random_state=42
        )
        
        if not auto_optimize:
            print(f"\n[训练决策树]")
            print(f"  action_x: depth={depth_x}, min_leaf={min_samples_leaf}, min_split={min_samples_split}")
            print(f"  action_w: depth={depth_w}, min_leaf={min_leaf_w}, min_split={min_split_w}")
            if optimize_w:
                print(f"  ⚡ action_w优化模式已启用（深度+{w_depth_boost}）")
        else:
            print(f"\n[训练决策树（使用优化参数）]")
            print(f"  action_x: depth={depth_x}, min_leaf={min_samples_leaf}, min_split={min_samples_split}")
            print(f"  action_w: depth={depth_w}, min_leaf={min_leaf_w}, min_split={min_split_w}")
        
        tree_x.fit(self.X, self.y_x)
        tree_w.fit(self.X, self.y_w)
        
        r2_x = tree_x.score(self.X, self.y_x)
        r2_w = tree_w.score(self.X, self.y_w)
        
        # 估算代码行数（每个节点大约4-5行代码）
        estimated_lines_x = tree_x.tree_.node_count * 4
        estimated_lines_w = tree_w.tree_.node_count * 4
        estimated_total_lines = estimated_lines_x + estimated_lines_w + 50  # +50 for class structure
        
        print(f"\n[代码大小估算]")
        print(f"  action_x节点数: {tree_x.tree_.node_count}, 估算代码行数: ~{estimated_lines_x}")
        print(f"  action_w节点数: {tree_w.tree_.node_count}, 估算代码行数: ~{estimated_lines_w}")
        print(f"  总估算代码行数: ~{estimated_total_lines}")
        
        # 检查代码大小限制
        if estimated_total_lines > max_code_lines:
            print(f"\n  ⚠️  警告: 估算代码行数 ({estimated_total_lines}) 超过限制 ({max_code_lines})")
            print(f"     这可能导致Unity编译失败或运行崩溃！")
            print(f"     建议:")
            print(f"     1. 增加 --max-code-lines 参数（如 --max-code-lines 100000）")
            print(f"     2. 降低目标R²（如 --target-r2 0.90）")
            print(f"     3. 增加 min_samples_leaf（如 --min-leaf 3）")
            print(f"     4. 限制树深度（如 --depth 20）")
            if warn_on_large:
                response = input(f"\n是否继续生成代码？(y/n): ")
                if response.lower() != 'y':
                    print("已取消生成代码")
                    return
        
        # 计算更多评估指标
        from sklearn.metrics import mean_absolute_error, mean_squared_error
        pred_x = tree_x.predict(self.X)
        pred_w = tree_w.predict(self.X)
        mae_x = mean_absolute_error(self.y_x, pred_x)
        mae_w = mean_absolute_error(self.y_w, pred_w)
        mse_x = mean_squared_error(self.y_x, pred_x)
        mse_w = mean_squared_error(self.y_w, pred_w)
        
        print(f"\n[模型性能评估]")
        print(f"  action_x: R²={r2_x:.4f}, MAE={mae_x:.6f}, MSE={mse_x:.6f}")
        print(f"  action_w: R²={r2_w:.4f}, MAE={mae_w:.6f}, MSE={mse_w:.6f}")
        
        # 如果action_w的R²仍然较低，给出警告和建议
        if r2_w < 0.75:
            print(f"\n  ⚠️ 警告: action_w的R²分数较低 ({r2_w:.4f})")
            print(f"     建议:")
            print(f"     1. 增加训练数据量（特别是弯道场景）")
            print(f"     2. 使用 --depth {depth_w + 2} 增加树深度")
            print(f"     3. 检查数据质量（是否有足够的角速度变化样本）")
            print(f"     4. 考虑使用 --optimize-w 参数")
        
        def tree_to_code(tree, feature_names, base_indent=0, language="csharp"):
            """递归将树转换为if-else代码"""
            def recurse(node, indent_level=0):
                indent_str = "    " * (base_indent + indent_level)
                
                if tree.feature[node] == -2:  # 叶子节点
                    value = float(tree.value[node][0])
                    if language == "csharp":
                        return f"{indent_str}return {value:.6f}f;"
                    else:  # python
                        return f"{indent_str}return {value:.6f}"
                else:
                    feature_idx = int(tree.feature[node])
                    feature_name = feature_names[feature_idx]
                    threshold = float(tree.threshold[node])
                    
                    # 获取左右子树
                    left_code = recurse(tree.children_left[node], indent_level + 1)
                    right_code = recurse(tree.children_right[node], indent_level + 1)
                    
                    if language == "csharp":
                        condition = f"{indent_str}if ({feature_name} <= {threshold:.6f}f)"
                        else_str = f"{indent_str}else"
                    else:  # python
                        condition = f"{indent_str}if {feature_name} <= {threshold:.6f}:"
                        else_str = f"{indent_str}else:"
                    
                    code = f"{condition}\n{left_code}\n{else_str}\n{right_code}"
                    return code
            
            return recurse(0)
        
        # 生成代码
        if language == "csharp":
            code = f"""// 决策树规则库 - 自动生成
// 数据样本数: {len(self.data)}
// R² Score - action_x: {r2_x:.4f}, action_w: {r2_w:.4f}
// 树深度: {max_depth}

using System;

public class DecisionTreeRules
{{
    // 特征变量（需要从观测中获取）
    private float sensor0, sensor1, sensor2, sensor3, sensor4, sensor5;
    private float smoothed_vx, smoothed_omega;
    private float raw_action_x, raw_action_w;
    private float actual_vz, actual_vx, actual_omega;
    
    // 设置观测值
    public void SetObservations(float[] obs)
    {{
        if (obs.Length < 13) throw new ArgumentException("观测数组长度不足13");
        sensor0 = obs[0];
        sensor1 = obs[1];
        sensor2 = obs[2];
        sensor3 = obs[3];
        sensor4 = obs[4];
        sensor5 = obs[5];
        smoothed_vx = obs[6];
        smoothed_omega = obs[7];
        raw_action_x = obs[8];
        raw_action_w = obs[9];
        actual_vz = obs[10];
        actual_vx = obs[11];
        actual_omega = obs[12];
    }}
    
    // 预测横向速度 action_x
    public float PredictActionX()
    {{
{tree_to_code(tree_x.tree_, self.feature_names, base_indent=2, language="csharp")}
    }}
    
    // 预测角速度 action_w
    public float PredictActionW()
    {{
{tree_to_code(tree_w.tree_, self.feature_names, base_indent=2, language="csharp")}
    }}
    
    // 同时预测两个动作
    public (float action_x, float action_w) Predict()
    {{
        return (PredictActionX(), PredictActionW());
    }}
}}
"""
        else:  # python
            code = f"""# 决策树规则库 - 自动生成
# 数据样本数: {len(self.data)}
# R² Score - action_x: {r2_x:.4f}, action_w: {r2_w:.4f}
# 树深度: {max_depth}

def predict_action_x(sensor0, sensor1, sensor2, sensor3, sensor4, sensor5,
                     smoothed_vx, smoothed_omega,
                     raw_action_x, raw_action_w,
                     actual_vz, actual_vx, actual_omega):
{tree_to_code(tree_x.tree_, self.feature_names, base_indent=1, language="python")}

def predict_action_w(sensor0, sensor1, sensor2, sensor3, sensor4, sensor5,
                     smoothed_vx, smoothed_omega,
                     raw_action_x, raw_action_w,
                     actual_vz, actual_vx, actual_omega):
{tree_to_code(tree_w.tree_, self.feature_names, base_indent=1, language="python")}

def predict(sensor0, sensor1, sensor2, sensor3, sensor4, sensor5,
            smoothed_vx, smoothed_omega,
            raw_action_x, raw_action_w,
            actual_vz, actual_vx, actual_omega):
    return (predict_action_x(sensor0, sensor1, sensor2, sensor3, sensor4, sensor5,
                            smoothed_vx, smoothed_omega,
                            raw_action_x, raw_action_w,
                            actual_vz, actual_vx, actual_omega),
            predict_action_w(sensor0, sensor1, sensor2, sensor3, sensor4, sensor5,
                            smoothed_vx, smoothed_omega,
                            raw_action_x, raw_action_w,
                            actual_vz, actual_vx, actual_omega))
"""
        
        with open(output_path, 'w', encoding='utf-8') as f:
            f.write(code)
        
        # 检查实际代码行数
        actual_lines = len(code.split('\n'))
        
        print(f"\n✓ IF规则代码已保存: {output_path}")
        print(f"  - action_x R²: {r2_x:.4f}")
        print(f"  - action_w R²: {r2_w:.4f}")
        print(f"  - 树节点数: action_x={tree_x.tree_.node_count}, action_w={tree_w.tree_.node_count}")
        print(f"  - 实际代码行数: {actual_lines}")
        
        # 最终警告
        if actual_lines > max_code_lines:
            print(f"\n  ⚠️  严重警告: 实际代码行数 ({actual_lines}) 超过限制 ({max_code_lines})")
            print(f"     这个文件可能导致Unity编译失败或运行崩溃！")
            print(f"     强烈建议:")
            print(f"     1. 删除此文件")
            print(f"     2. 使用更保守的参数重新生成:")
            print(f"        python analysis.py --data <file> --rules --depth 15 --min-leaf 3 --target-r2 0.90")
        elif actual_lines > max_code_lines * 0.8:
            print(f"\n  ⚠️  注意: 代码文件较大 ({actual_lines} 行)，接近限制 ({max_code_lines})")
            print(f"     如果Unity编译失败，请降低参数重新生成")


def main():
    import argparse
    parser = argparse.ArgumentParser(description='决策树蒸馏工具：将训练数据转换为if规则库')
    parser.add_argument('--data', type=str, default=None, help='CSV数据文件路径（可选，会覆盖全局变量）')
    parser.add_argument('--analysis', action='store_true', help='分析特征重要性（会覆盖全局变量）')
    parser.add_argument('--compare', action='store_true', help='对比模型性能（会覆盖全局变量）')
    parser.add_argument('--boundary', action='store_true', help='可视化决策边界（会覆盖全局变量）')
    parser.add_argument('--json', type=str, nargs='?', const='model.json', help='导出JSON模型（可选：指定输出路径，会覆盖全局变量）')
    parser.add_argument('--rules', type=str, nargs='?', const='if_rules.cs', help='生成if规则代码（可选：指定输出路径，会覆盖全局变量）')
    parser.add_argument('--depth', type=int, default=None, help='决策树最大深度（会覆盖全局变量）')
    parser.add_argument('--lang', type=str, choices=['csharp', 'python'], default=None, help='规则代码语言（会覆盖全局变量）')
    parser.add_argument('--min-leaf', type=int, default=None, help='叶子节点最小样本数（会覆盖全局变量）')
    parser.add_argument('--min-split', type=int, default=None, help='分裂节点最小样本数（会覆盖全局变量）')
    parser.add_argument('--max-features', type=str, default=None, choices=['sqrt', 'log2'], help='每次分裂考虑的最大特征数（会覆盖全局变量）')
    parser.add_argument('--ccp-alpha', type=float, default=None, help='最小成本复杂度剪枝参数（会覆盖全局变量）')
    parser.add_argument('--optimize-w', action='store_true', help='针对action_w进行优化（会覆盖全局变量）')
    parser.add_argument('--w-depth-boost', type=int, default=None, help='optimize-w模式下，action_w树的深度增加量（会覆盖全局变量）')
    parser.add_argument('--auto-optimize', action='store_true', help='自动优化参数以达到目标R²（会覆盖全局变量）')
    parser.add_argument('--target-r2', type=float, default=None, help='自动优化模式下的目标R²分数（会覆盖全局变量，同时设置X和W）')
    parser.add_argument('--target-r2-x', type=float, default=None, help='action_x的目标R²分数（会覆盖全局变量）')
    parser.add_argument('--target-r2-w', type=float, default=None, help='action_w的目标R²分数（会覆盖全局变量）')
    parser.add_argument('--max-code-lines', '--max_code_lines', type=int, default=None, 
                       dest='max_code_lines', help='最大代码行数限制（会覆盖全局变量）')
    parser.add_argument('--no-warn-large', action='store_true', help='不警告大型代码文件（会覆盖全局变量）')
    
    args = parser.parse_args()
    
    # ========== 使用全局变量，命令行参数作为覆盖 ==========
    # 数据文件路径
    data_path = args.data if args.data is not None else DATA_FILE_PATH
    
    # 功能选项（命令行参数优先，否则使用全局变量）
    enable_analysis = args.analysis if args.analysis else ENABLE_ANALYSIS
    enable_compare = args.compare if args.compare else ENABLE_COMPARE
    enable_boundary = args.boundary if args.boundary else ENABLE_BOUNDARY
    export_json = args.json if args.json is not None else EXPORT_JSON
    generate_rules = args.rules if args.rules is not None else GENERATE_RULES
    
    # 自动优化参数
    use_auto_optimize = args.auto_optimize if args.auto_optimize else USE_AUTO_OPTIMIZE
    max_code_lines = args.max_code_lines if args.max_code_lines is not None else MAX_CODE_LINES
    
    # 处理目标R²：优先使用单独设置的X和W，否则使用统一的target_r2，最后使用全局变量
    if args.target_r2_x is not None:
        target_r2_x = args.target_r2_x
    elif args.target_r2 is not None:
        target_r2_x = args.target_r2
    elif TARGET_R2_X is not None:
        target_r2_x = TARGET_R2_X
    else:
        target_r2_x = 0.93  # 默认值
    
    if args.target_r2_w is not None:
        target_r2_w = args.target_r2_w
    elif args.target_r2 is not None:
        target_r2_w = args.target_r2
    elif TARGET_R2_W is not None:
        target_r2_w = TARGET_R2_W
    else:
        target_r2_w = 0.93  # 默认值
    
    # 手动参数
    depth = args.depth if args.depth is not None else MANUAL_DEPTH
    min_leaf = args.min_leaf if args.min_leaf is not None else MANUAL_MIN_LEAF
    min_split = args.min_split if args.min_split is not None else MANUAL_MIN_SPLIT
    optimize_w = args.optimize_w if args.optimize_w else OPTIMIZE_W
    w_depth_boost = args.w_depth_boost if args.w_depth_boost is not None else W_DEPTH_BOOST
    max_features = args.max_features if args.max_features is not None else MAX_FEATURES
    ccp_alpha = args.ccp_alpha if args.ccp_alpha is not None else CCP_ALPHA
    
    # 其他参数
    code_language = args.lang if args.lang is not None else CODE_LANGUAGE
    warn_on_large = not args.no_warn_large if args.no_warn_large else WARN_ON_LARGE
    
    # 验证数据文件
    try:
        dist = AdvancedDistillation(data_path)
        print(f"✓ 成功加载数据: {len(dist.data)} 条样本")
        print(f"  数据文件: {data_path}")
    except Exception as e:
        print(f"✗ 加载数据失败: {e}")
        print(f"  请检查数据文件路径是否正确: {data_path}")
        return
    
    # 执行功能
    if enable_analysis:
        print("\n=== 执行特征重要性分析 ===")
        dist.analyze_feature_importance()
    
    if enable_compare:
        print("\n=== 执行模型性能对比 ===")
        dist.compare_models(optimize_w=optimize_w, w_depth_boost=w_depth_boost)
    
    if enable_boundary:
        print("\n=== 执行决策边界可视化 ===")
        dist.analyze_decision_boundaries()
    
    if export_json:
        output_path = export_json if isinstance(export_json, str) else 'model.json'
        print(f"\n=== 导出JSON模型: {output_path} ===")
        dist.generate_json_model(output_path, max_depth=depth)
    
    if generate_rules:
        output_path = generate_rules if isinstance(generate_rules, str) else 'if_rules.cs'
        print(f"\n=== 生成if规则代码: {output_path} ===")
        print(f"  自动优化: {use_auto_optimize}")
        if use_auto_optimize:
            print(f"  目标R² - action_x: {target_r2_x:.2f}, action_w: {target_r2_w:.2f}")
            print(f"  最大代码行数: {max_code_lines}")
        else:
            print(f"  树深度: {depth}")
            print(f"  最小叶子节点: {min_leaf}")
            print(f"  最小分裂节点: {min_split}")
            print(f"  优化action_w: {optimize_w}")
        
        dist.generate_if_rules(
            output_path, 
            max_depth=depth, 
            language=code_language,
            min_samples_leaf=min_leaf,
            min_samples_split=min_split,
            max_features=max_features,
            ccp_alpha=ccp_alpha,
            optimize_w=optimize_w,
            w_depth_boost=w_depth_boost,
            auto_optimize=use_auto_optimize,
            target_r2_x=target_r2_x,
            target_r2_w=target_r2_w,
            max_code_lines=max_code_lines,
            warn_on_large=warn_on_large
        )
    
    # 如果没有指定任何功能，显示提示
    if not any([enable_analysis, enable_compare, enable_boundary, export_json, generate_rules]):
        print("\n⚠️  未指定任何功能选项！")
        print("\n请在代码中修改全局变量，或使用命令行参数:")
        print("  命令行示例: python analysis.py --data data.csv --rules DecisionTreeRules.cs --auto-optimize")
        print("\n或在代码中设置全局变量:")
        print("  GENERATE_RULES = 'DecisionTreeRules.cs'")
        print("  USE_AUTO_OPTIMIZE = True")
        print("  TARGET_R2 = 0.93")


if __name__ == '__main__':
    main()
