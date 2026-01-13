#!/usr/bin/env python3
"""
高级蒸馏工具：特征分析、可视化、多模型对比

使用方法：
1. 生成if规则库（推荐）：
   python analysis.py --data <CSV文件路径> --rules [输出文件路径] --depth 8

2. 对比不同深度的性能：
   python analysis.py --data <CSV文件路径> --compare

3. 分析特征重要性：
   python analysis.py --data <CSV文件路径> --analysis

4. 完整流程（推荐）：
   python analysis.py --data <CSV文件路径> --compare --rules DecisionTreeRules.cs --depth 10

示例：
   python analysis.py --data training_data.csv --rules if_rules.cs --depth 8
   python analysis.py --data D:/Data/episode_data.csv --compare --rules
"""

import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
from sklearn.tree import DecisionTreeRegressor
from sklearn.ensemble import RandomForestRegressor
from sklearn.metrics import mean_squared_error, mean_absolute_error, r2_score
import json
import seaborn as sns

class AdvancedDistillation:
    def __init__(self, csv_path):
        self.data = pd.read_csv(csv_path)
        self.feature_names = [
            "sensor0", "sensor1", "sensor2", "sensor3", "sensor4", "sensor5",
            "smoothed_vx", "smoothed_omega",
            "raw_action_x", "raw_action_w",
            "actual_vz", "actual_vx", "actual_omega"
        ]
        self.X = self.data.iloc[:, :13].values
        self.y_x = self.data.iloc[:, 13].values
        self.y_w = self.data.iloc[:, 14].values
        
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
        
    def compare_models(self):
        """对比不同深度的决策树性能"""
        depths = [3, 5, 7, 8, 10, 12, 15]
        results = {"depth": [], "r2_x": [], "r2_w": [], "mse_x": [], "mse_w": []}
        
        print("\n=== 不同树深度的性能对比 ===")
        print(f"{'Depth':<8} {'R2_x':<10} {'R2_w':<10} {'MSE_x':<12} {'MSE_w':<12} {'Code Size Est.'}")
        print("-" * 65)
        
        for depth in depths:
            tree_x = DecisionTreeRegressor(max_depth=depth, min_samples_leaf=5, random_state=42)
            tree_w = DecisionTreeRegressor(max_depth=depth, min_samples_leaf=5, random_state=42)
            
            tree_x.fit(self.X, self.y_x)
            tree_w.fit(self.X, self.y_w)
            
            r2_x = tree_x.score(self.X, self.y_x)
            r2_w = tree_w.score(self.X, self.y_w)
            
            pred_x = tree_x.predict(self.X)
            pred_w = tree_w.predict(self.X)
            
            mse_x = mean_squared_error(self.y_x, pred_x)
            mse_w = mean_squared_error(self.y_w, pred_w)
            
            # 估算代码行数（大约每个节点3-4行）
            n_nodes_x = tree_x.tree_.node_count
            n_nodes_w = tree_w.tree_.node_count
            code_size = (n_nodes_x + n_nodes_w) * 4
            
            results["depth"].append(depth)
            results["r2_x"].append(r2_x)
            results["r2_w"].append(r2_w)
            results["mse_x"].append(mse_x)
            results["mse_w"].append(mse_w)
            
            print(f"{depth:<8} {r2_x:<10.4f} {r2_w:<10.4f} {mse_x:<12.6f} {mse_w:<12.6f} ~{code_size} lines")
        
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
    
    def generate_if_rules(self, output_path="if_rules.cs", max_depth=8, language="csharp"):
        """
        生成if-else规则代码（可直接用于Unity C#）
        
        Args:
            output_path: 输出文件路径
            max_depth: 决策树最大深度
            language: 输出语言 ("csharp" 或 "python")
        """
        tree_x = DecisionTreeRegressor(max_depth=max_depth, min_samples_leaf=5, random_state=42)
        tree_w = DecisionTreeRegressor(max_depth=max_depth, min_samples_leaf=5, random_state=42)
        
        tree_x.fit(self.X, self.y_x)
        tree_w.fit(self.X, self.y_w)
        
        r2_x = tree_x.score(self.X, self.y_x)
        r2_w = tree_w.score(self.X, self.y_w)
        
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
        
        print(f"✓ IF规则代码已保存: {output_path}")
        print(f"  - action_x R²: {r2_x:.4f}")
        print(f"  - action_w R²: {r2_w:.4f}")
        print(f"  - 树节点数: action_x={tree_x.tree_.node_count}, action_w={tree_w.tree_.node_count}")


def main():
    import argparse
    parser = argparse.ArgumentParser(description='决策树蒸馏工具：将训练数据转换为if规则库')
    parser.add_argument('--data', type=str, required=True, help='CSV数据文件路径')
    parser.add_argument('--analysis', action='store_true', help='分析特征重要性')
    parser.add_argument('--compare', action='store_true', help='对比模型性能')
    parser.add_argument('--boundary', action='store_true', help='可视化决策边界')
    parser.add_argument('--json', type=str, nargs='?', const='model.json', help='导出JSON模型（可选：指定输出路径）')
    parser.add_argument('--rules', type=str, nargs='?', const='if_rules.cs', help='生成if规则代码（可选：指定输出路径，默认if_rules.cs）')
    parser.add_argument('--depth', type=int, default=8, help='决策树最大深度（默认8）')
    parser.add_argument('--lang', type=str, choices=['csharp', 'python'], default='csharp', help='规则代码语言（默认csharp）')
    
    args = parser.parse_args()
    
    # 验证数据文件
    try:
        dist = AdvancedDistillation(args.data)
        print(f"✓ 成功加载数据: {len(dist.data)} 条样本")
    except Exception as e:
        print(f"✗ 加载数据失败: {e}")
        return
    
    if args.analysis:
        dist.analyze_feature_importance()
    if args.compare:
        dist.compare_models()
    if args.boundary:
        dist.analyze_decision_boundaries()
    if args.json:
        output_path = args.json if isinstance(args.json, str) else 'model.json'
        dist.generate_json_model(output_path, max_depth=args.depth)
    if args.rules:
        output_path = args.rules if isinstance(args.rules, str) else 'if_rules.cs'
        dist.generate_if_rules(output_path, max_depth=args.depth, language=args.lang)
    
    if not any([args.analysis, args.compare, args.boundary, args.json, args.rules]):
        print("请指定至少一个选项:")
        print("  --analysis    : 分析特征重要性")
        print("  --compare     : 对比模型性能")
        print("  --boundary    : 可视化决策边界")
        print("  --json [path] : 导出JSON模型")
        print("  --rules [path]: 生成if规则代码（推荐用于Unity）")
        print("\n示例:")
        print("  python analysis.py --data data.csv --rules --depth 10")
        print("  python analysis.py --data data.csv --compare --rules if_rules.cs")


if __name__ == '__main__':
    main()
