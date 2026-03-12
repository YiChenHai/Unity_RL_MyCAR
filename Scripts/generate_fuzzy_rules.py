#!/usr/bin/env python3
"""
模糊规则库自动生成工具
从PPO训练数据中自动提取模糊隶属函数和规则，生成C#代码用于Unity部署

原理：
  1. 从CSV数据中读取「传感器状态 → 控制输出」映射
  2. 对每个输入/输出变量，自动划分模糊集并确定隶属函数参数
  3. 使用 Wang-Mendel 方法从数据中逐条提取模糊规则
  4. 冲突消解（同一前件不同后件 → 保留置信度最高的）
  5. 剪枝优化（删除低支持度/低置信度的规则）
  6. 生成可直接用于Unity的C#模糊控制器代码

使用方法：
  # 基本用法（推荐，自动确定隶属函数参数）
  python generate_fuzzy_rules.py --data <CSV文件路径>

  # 指定模糊集数量（偏差变量5/7/9集，中心变量3/5集）
  python generate_fuzzy_rules.py --data <CSV文件路径> --n-diff 5 --n-center 3

  # 使用数据驱动的隶属函数（根据数据分布自动放置MF中心）
  python generate_fuzzy_rules.py --data <CSV文件路径> --mf-method data-driven

  # 生成可视化图表
  python generate_fuzzy_rules.py --data <CSV文件路径> --visualize

  # 完整流程（推荐）
  python generate_fuzzy_rules.py --data <CSV文件路径> --mf-method data-driven --visualize --output FuzzyController.cs

配置说明：
  也可以直接修改下方 "全局配置参数" 区域的变量，无需命令行参数。
"""

import numpy as np
import pandas as pd
from collections import defaultdict
import os
import sys
from datetime import datetime

# ============================================================================
# 全局配置参数（可直接在此修改，无需命令行参数）
# ============================================================================

# CSV数据文件路径
DATA_FILE_PATH = ""

# 输出C#文件路径（留空则使用默认路径）
OUTPUT_CS_PATH = ""

# 隶属函数生成方法: "equal" (等间距) 或 "data-driven" (数据驱动)
MF_METHOD = "data-driven"

# 偏差变量（front_lr_diff, rear_lr_diff）的模糊集数量（建议 5 或 7）
N_DIFF_SETS = 5

# 中心变量（front_center, rear_center）的模糊集数量（建议 3 或 5）
N_CENTER_SETS = 3

# 最小间距比例（data-driven 模式专用）
# 相邻MF中心之间的最小间距 = 等间距距离 × 此比例
# 0 = 无约束（纯数据驱动，可能导致MF极端聚集）
# 0.5 = 最小间距为等间距的50%（推荐，平衡分辨率与覆盖范围）
# 1.0 = 完全等间距（退化为 equal 模式）
MIN_SPACING_RATIO = 0.5

# 最小规则支持度（低于此值的规则被剪枝，即匹配该规则的数据点数量下限）
MIN_RULE_SUPPORT = 3

# 最小规则强度（低于此值的规则被剪枝，即规则触发强度下限）
MIN_RULE_STRENGTH = 0.01

# 是否启用规则覆盖补全（推荐开启）
# 用于修复“训练数据集中于直行，转弯规则缺失”的问题
ENABLE_RULE_COMPLETION = True

# 规则补全后规则数上限（避免规则爆炸）
RULE_COMPLETION_MAX_ADDED = 320

# 镜像规则强度衰减系数（0~1）
RULE_COMPLETION_MIRROR_STRENGTH_SCALE = 0.85

# 严重偏差外推强度衰减系数（0~1）
RULE_COMPLETION_EXTRAP_STRENGTH_SCALE = 0.70

# 严重偏差外推输出放大系数（每跨一级偏差，输出放大比例）
RULE_COMPLETION_EXTRAP_GAIN = 0.35

# 是否生成可视化图表
ENABLE_VISUALIZE = False

# ============================================================================
# CSV列映射（与 MyCar_Agent.cs 的 RecordSample 对应）
# ============================================================================

INPUT_COLUMNS = {
    'front_lr_diff': 0,     # 前排左右差 [-1, 1]
    'rear_lr_diff': 1,      # 后排左右差 [-1, 1]
    'front_center': 2,      # 前中传感器 [0, 1]
    'rear_center': 3,       # 后中传感器 [0, 1]
}

OUTPUT_COLUMNS = {
    'output_vx_norm': 11,     # 直接横向速度输出 [-1, 1]
    'output_omega_norm': 12,  # 直接角速度输出 [-1, 1]
}

VARIABLE_RANGES = {
    'front_lr_diff': (-1.0, 1.0),
    'rear_lr_diff': (-1.0, 1.0),
    'front_center': (0.0, 1.0),
    'rear_center': (0.0, 1.0),
    'output_vx_norm': (-1.0, 1.0),
    'output_omega_norm': (-1.0, 1.0),
}

# 模糊集命名模板
FUZZY_SET_NAMES = {
    3: ['N', 'ZE', 'P'],                          # 3集（对称）
    5: ['NB', 'NS', 'ZE', 'PS', 'PB'],            # 5集（对称）
    7: ['NB', 'NM', 'NS', 'ZE', 'PS', 'PM', 'PB'],  # 7集（对称）
    9: ['NB', 'NM', 'NS', 'NZ', 'ZE', 'PZ', 'PS', 'PM', 'PB'],
}
FUZZY_SET_NAMES_POSITIVE = {
    3: ['LO', 'ME', 'HI'],                        # 3集（正值）
    5: ['VL', 'LO', 'ME', 'HI', 'VH'],            # 5集（正值）
}


# ============================================================================
# 核心类定义
# ============================================================================

class TriangularMF:
    """三角形隶属函数 (a=左端, b=中心, c=右端)"""
    
    def __init__(self, name, a, b, c):
        self.name = name
        self.a = a
        self.b = b
        self.c = c
    
    def evaluate(self, x):
        """计算隶属度 μ(x)"""
        # 左肩MF (a==b): x<=b 时 μ=1（梯形左端）
        if self.a >= self.b - 1e-10:
            if x <= self.b:
                return 1.0
            elif x >= self.c:
                return 0.0
            else:
                return (self.c - x) / max(1e-10, self.c - self.b)
        # 右肩MF (b==c): x>=b 时 μ=1（梯形右端）
        if self.b >= self.c - 1e-10:
            if x >= self.b:
                return 1.0
            elif x <= self.a:
                return 0.0
            else:
                return (x - self.a) / max(1e-10, self.b - self.a)
        # 常规三角形MF
        if x <= self.a or x >= self.c:
            return 0.0
        elif x <= self.b:
            return (x - self.a) / max(1e-10, self.b - self.a)
        else:
            return (self.c - x) / max(1e-10, self.c - self.b)
    
    def __repr__(self):
        return f"TriMF('{self.name}', a={self.a:.4f}, b={self.b:.4f}, c={self.c:.4f})"


class GaussianMF:
    """高斯隶属函数 μ(x) = exp(-(x-c)²/(2σ²))
    
    相比三角形MF：
      - 无硬截断（在整个定义域上都有非零隶属度）
      - 更平滑的过渡
      - 与聚类算法天然配合（聚类产生的分布本身近似高斯）
    """
    
    def __init__(self, name, center, sigma):
        self.name = name
        self.center = center
        self.sigma = max(sigma, 1e-6)
    
    def evaluate(self, x):
        """计算隶属度 μ(x) = exp(-(x-c)²/(2σ²))"""
        return float(np.exp(-((x - self.center) ** 2) / (2.0 * self.sigma ** 2)))
    
    def __repr__(self):
        return f"GaussMF('{self.name}', c={self.center:.4f}, σ={self.sigma:.4f})"


class FuzzyVariable:
    """模糊变量（包含多个隶属函数）"""
    
    def __init__(self, name, var_range):
        self.name = name
        self.range = var_range
        self.mfs = []  # List[TriangularMF]
    
    def add_mf(self, mf):
        self.mfs.append(mf)
    
    def fuzzify(self, x):
        """模糊化：返回各隶属函数的隶属度"""
        return {mf.name: mf.evaluate(x) for mf in self.mfs}
    
    def get_dominant_mf(self, x):
        """返回隶属度最高的模糊集名称和隶属度"""
        memberships = self.fuzzify(x)
        best_name = max(memberships, key=memberships.get)
        return best_name, memberships[best_name]
    
    def get_mf_index(self, mf_name):
        """返回模糊集的索引"""
        for i, mf in enumerate(self.mfs):
            if mf.name == mf_name:
                return i
        return -1


class FuzzyRule:
    """模糊规则: IF ... AND ... THEN output_vx = val, output_omega = val"""
    
    def __init__(self, antecedent, consequent_values, strength=1.0, support_count=1):
        self.antecedent = antecedent          # {var_name: mf_name}
        self.consequent_values = consequent_values  # {output_name: float_value}
        self.strength = strength              # 规则最大触发强度
        self.support_count = support_count    # 支持该规则的数据点数量
    
    def get_key(self):
        """生成规则的唯一键（基于前件）"""
        return tuple(sorted(self.antecedent.items()))
    
    def __repr__(self):
        ant = " AND ".join(f"{k} IS {v}" for k, v in self.antecedent.items())
        con = ", ".join(f"{k}={v:.4f}" for k, v in self.consequent_values.items())
        return f"IF {ant} THEN {con}  [strength={self.strength:.4f}, support={self.support_count}]"


# ============================================================================
# 减法聚类算法 (Chiu, 1994)
# ============================================================================

def subtractive_clustering(data, ra=0.5, accept_ratio=0.5, reject_ratio=0.15,
                           max_clusters=500, verbose=True):
    """
    减法聚类算法 —— 自动确定聚类数量和中心位置
    
    算法原理:
      1. 对每个数据点，计算周围的数据密度（高斯核函数）
      2. 选取密度最高的点作为第一个聚类中心
      3. 抑制该中心附近的密度（减去高斯衰减量）
      4. 重复2-3步，直到剩余密度低于阈值
    
    参数:
      data: (N, D) 归一化到[0,1]的数据矩阵
      ra: 聚类半径 (0.2~0.8)，越小→越多聚类→越多规则
           ra=0.3 → 精细划分，ra=0.5 → 中等，ra=0.7 → 粗略
      accept_ratio: 密度接受比率（相对于第一个聚类中心的密度）
      reject_ratio: 密度拒绝比率（低于此比率则停止）
      max_clusters: 最大聚类数（安全上限）
    
    返回:
      centers: (K, D) 聚类中心坐标
      sigmas: (K, D) 各维度的高斯宽度
    
    参考文献:
      Chiu, S.L. (1994). "Fuzzy Model Identification Based on Cluster Estimation."
      Journal of Intelligent and Fuzzy Systems, 2(3), 267-278.
    """
    n, d = data.shape
    alpha = 4.0 / (ra ** 2)
    rb = 1.5 * ra
    beta = 4.0 / (rb ** 2)
    
    # ===== Step 1: 计算每个点的初始密度 =====
    # D_i = Σ_j exp(-α * ||x_i - x_j||²)
    # 向量化分块计算，防止大数据集内存溢出
    chunk_size = 2000
    densities = np.zeros(n)
    
    for i in range(0, n, chunk_size):
        end_i = min(i + chunk_size, n)
        chunk = data[i:end_i]  # (chunk_size, d)
        # ||a - b||² = ||a||² + ||b||² - 2<a,b>
        a_sq = np.sum(chunk ** 2, axis=1, keepdims=True)   # (chunk, 1)
        b_sq = np.sum(data ** 2, axis=1, keepdims=True).T  # (1, n)
        dist2 = a_sq + b_sq - 2.0 * chunk @ data.T         # (chunk, n)
        dist2 = np.maximum(dist2, 0.0)  # 数值稳定性
        densities[i:end_i] = np.sum(np.exp(-alpha * dist2), axis=1)
    
    if verbose:
        print(f"  初始密度范围: [{densities.min():.2f}, {densities.max():.2f}]")
    
    d1 = densities.max()  # 第一个聚类中心的密度（作为参考基准）
    
    if d1 < 1e-10:
        # 数据量太少或完全相同，返回均值作为唯一聚类
        return (np.array([np.mean(data, axis=0)]),
                np.array([np.std(data, axis=0) + 0.05]))
    
    # ===== Step 2-4: 迭代提取聚类中心 =====
    centers = []
    
    while len(centers) < max_clusters:
        idx = np.argmax(densities)
        dk = densities[idx]
        
        if dk <= 0:
            break
        
        ratio = dk / d1
        
        if len(centers) == 0 or ratio > accept_ratio:
            # 密度足够高，直接接受
            centers.append(data[idx].copy())
        elif ratio < reject_ratio:
            # 密度太低，停止
            break
        else:
            # 灰色地带：检查与已有中心的距离
            dists = [np.sqrt(np.sum((data[idx] - c) ** 2)) for c in centers]
            min_dist = min(dists)
            if min_dist / ra + ratio >= 1.0:
                # 距离足够远，接受（即使密度不那么高）
                centers.append(data[idx].copy())
            else:
                # 太近了，拒绝这个点，尝试下一个
                densities[idx] = 0
                continue
        
        # Step 3: 抑制该中心附近的密度
        dist2_to_center = np.sum((data - data[idx]) ** 2, axis=1)
        densities -= dk * np.exp(-beta * dist2_to_center)
        densities = np.maximum(densities, 0.0)
    
    centers = np.array(centers)
    k = len(centers)
    
    if verbose:
        print(f"  聚类完成: 发现 {k} 个聚类中心")
    
    # ===== Step 5: 计算各聚类的宽度(sigma) =====
    # 将数据点分配到最近的聚类，计算各维度标准差
    sigmas = np.zeros_like(centers)
    
    if k > 1:
        # 分配数据点到最近聚类
        assignments = np.zeros(n, dtype=int)
        for i in range(0, n, chunk_size):
            end_i = min(i + chunk_size, n)
            chunk = data[i:end_i]
            dists = np.zeros((end_i - i, k))
            for c_idx in range(k):
                dists[:, c_idx] = np.sum((chunk - centers[c_idx]) ** 2, axis=1)
            assignments[i:end_i] = np.argmin(dists, axis=1)
        
        for c_idx in range(k):
            mask = assignments == c_idx
            count = np.sum(mask)
            if count > 1:
                sigmas[c_idx] = np.std(data[mask], axis=0)
            else:
                sigmas[c_idx] = ra / np.sqrt(8)
        
        # 最小sigma保证（防止MF过窄）
        min_sigma = ra / (2.0 * np.sqrt(2))
        sigmas = np.maximum(sigmas, min_sigma)
    else:
        sigmas = np.std(data, axis=0, keepdims=True)
        sigmas = np.maximum(sigmas, 0.1)
    
    return centers, sigmas


# ============================================================================
# 模糊规则库生成器
# ============================================================================

class FuzzyRuleGenerator:
    """模糊规则库自动生成器"""
    
    def __init__(self):
        self.input_vars = {}     # {name: FuzzyVariable}
        self.output_names = []
        self.rules = []
        self.data = None
        self.input_data = None   # numpy array
        self.output_data = None  # numpy array
        self.input_col_names = []
        self.output_col_names = []
        self._clustering_mode = False  # 是否使用聚类模式
        self._cluster_centers = None   # 聚类中心 (K, n_inputs)
        self._cluster_sigmas = None    # 聚类宽度 (K, n_inputs)
        self._cluster_outputs = None   # 聚类输出 (K, n_outputs)
    
    # ===== 数据加载 =====
    
    def load_data(self, csv_path, max_samples=None):
        """加载CSV数据文件"""
        print(f"\n{'='*60}")
        print(f"[1/5] 加载数据: {csv_path}")
        print(f"{'='*60}")
        
        df = pd.read_csv(csv_path)
        print(f"  CSV总行数: {len(df)}, 总列数: {df.shape[1]}")
        print(f"  列名: {list(df.columns)}")
        
        # 提取输入列
        self.input_col_names = list(INPUT_COLUMNS.keys())
        self.output_col_names = list(OUTPUT_COLUMNS.keys())
        self.output_names = self.output_col_names
        
        input_indices = list(INPUT_COLUMNS.values())
        output_indices = list(OUTPUT_COLUMNS.values())
        
        # 检查列数是否足够
        max_col = max(max(input_indices), max(output_indices))
        if df.shape[1] <= max_col:
            print(f"\n  [ERROR] CSV only has {df.shape[1]} columns, need at least {max_col+1}")
            print(f"  请确认CSV是由修改后的 MyCar_Agent.cs 生成的（含 output_vx_norm 和 output_omega_norm 列）")
            print(f"  如果是旧格式CSV（11列），output_vx_norm 和 output_omega_norm 列不存在。")
            sys.exit(1)
        
        self.input_data = df.iloc[:, input_indices].values
        self.output_data = df.iloc[:, output_indices].values
        
        # 可选采样
        if max_samples and len(self.input_data) > max_samples:
            indices = np.random.choice(len(self.input_data), max_samples, replace=False)
            self.input_data = self.input_data[indices]
            self.output_data = self.output_data[indices]
            print(f"  随机采样至 {max_samples} 条")
        
        print(f"  使用数据量: {len(self.input_data)} 条")
        print(f"\n  输入变量统计:")
        for i, name in enumerate(self.input_col_names):
            col = self.input_data[:, i]
            print(f"    {name:20s}: min={col.min():.4f}, max={col.max():.4f}, "
                  f"mean={col.mean():.4f}, std={col.std():.4f}")
        print(f"\n  输出变量统计:")
        for i, name in enumerate(self.output_col_names):
            col = self.output_data[:, i]
            print(f"    {name:20s}: min={col.min():.4f}, max={col.max():.4f}, "
                  f"mean={col.mean():.4f}, std={col.std():.4f}")
    
    # ===== 隶属函数生成 =====
    
    def generate_mfs(self, method="equal", n_diff=5, n_center=3, min_spacing_ratio=0.5):
        """
        自动生成隶属函数
        
        参数:
          method: "equal" (等间距) 或 "data-driven" (数据驱动)
          n_diff: 偏差变量的模糊集数量
          n_center: 中心变量的模糊集数量
          min_spacing_ratio: data-driven模式的最小间距比例 (0~1)
                            相邻MF中心间距 ≥ 等间距 × ratio
                            0=无约束, 0.5=推荐, 1.0=完全等间距
        """
        print(f"\n{'='*60}")
        print(f"[2/5] 生成隶属函数 (方法: {method})")
        if method == "data-driven":
            print(f"      最小间距比例: {min_spacing_ratio}")
        print(f"{'='*60}")
        
        # 确定每个变量的模糊集数量和类型
        var_configs = {
            'front_lr_diff': {'n_sets': n_diff,   'symmetric': True},
            'rear_lr_diff':  {'n_sets': n_diff,   'symmetric': True},
            'front_center':  {'n_sets': n_center, 'symmetric': False},
            'rear_center':   {'n_sets': n_center, 'symmetric': False},
        }
        
        for var_name, config in var_configs.items():
            var_range = VARIABLE_RANGES[var_name]
            n = config['n_sets']
            is_sym = config['symmetric']
            
            # 选择模糊集名称
            if is_sym:
                set_names = FUZZY_SET_NAMES.get(n, [f"S{i}" for i in range(n)])
            else:
                set_names = FUZZY_SET_NAMES_POSITIVE.get(n, [f"S{i}" for i in range(n)])
            
            col_idx = list(INPUT_COLUMNS.keys()).index(var_name)
            data_col = self.input_data[:, col_idx]
            
            if method == "data-driven":
                fvar = self._generate_data_driven_mfs(
                    var_name, var_range, data_col, n, set_names,
                    is_symmetric=is_sym,
                    min_spacing_ratio=min_spacing_ratio
                )
            else:
                fvar = self._generate_equal_mfs(var_name, var_range, n, set_names)
            
            self.input_vars[var_name] = fvar
            print(f"\n  {var_name} ({n} 个模糊集):")
            for mf in fvar.mfs:
                print(f"    {mf}")
    
    def _generate_equal_mfs(self, var_name, var_range, n_sets, set_names):
        """
        等间距三角形隶属函数生成
        
        原理：将变量范围等分为 n_sets 个区域，相邻MF重叠50%
        示例（5集，范围[-1,1]）：
          NB: (-1.0, -1.0, -0.5)  ← 左端截断
          NS: (-1.0, -0.5,  0.0)
          ZE: (-0.5,  0.0,  0.5)
          PS: ( 0.0,  0.5,  1.0)
          PB: ( 0.5,  1.0,  1.0)  ← 右端截断
        """
        fvar = FuzzyVariable(var_name, var_range)
        lo, hi = var_range
        step = (hi - lo) / (n_sets - 1)
        
        for i in range(n_sets):
            center = lo + i * step
            left = center - step
            right = center + step
            # 边界截断
            left = max(lo, left) if i > 0 else lo
            right = min(hi, right) if i < n_sets - 1 else hi
            # 两端MF的中心与边界重合
            if i == 0:
                left = lo
                center = lo
            if i == n_sets - 1:
                center = hi
                right = hi
            
            fvar.add_mf(TriangularMF(set_names[i], left, center, right))
        
        return fvar
    
    def _generate_data_driven_mfs(self, var_name, var_range, data_col, n_sets, set_names,
                                  is_symmetric=False, min_spacing_ratio=0.5):
        """
        数据驱动的隶属函数生成（带最小间距约束）
        
        解决的核心问题：
          纯百分位数法在训练数据分布极端集中时（如RL训练数据95%+在对齐状态），
          会导致所有MF中心聚集在极小范围内（如 [-1,1] 范围中仅 0.02 宽度），
          使控制器在分布外完全丧失分辨能力，无法泛化。
        
        改进策略：
          1. 对称变量（lr_diff）：强制 ZE 集中心 = 物理零点(0)，
             负/正半轴分别用各自数据的百分位数放置中间MF
          2. 最小间距约束：相邻MF中心距离 ≥ 等间距 × min_spacing_ratio，
             防止MF过度聚集，保证全输入范围的基本覆盖
          3. 数据稀疏区域自动回退到等间距分布
        
        参数:
          is_symmetric: 变量是否关于零点对称（如 lr_diff ∈ [-1,1]）
          min_spacing_ratio: 最小间距 / 等间距 的比例 (0~1)
        """
        fvar = FuzzyVariable(var_name, var_range)
        lo, hi = var_range
        total_range = hi - lo
        
        if is_symmetric and n_sets >= 3 and n_sets % 2 == 1:
            # ===== 对称变量：分半处理，ZE 中心强制为物理零点 =====
            mid_idx = n_sets // 2
            zero_point = (lo + hi) / 2.0  # 对称中心（[-1,1] → 0）
            
            # --- 负半轴：[lo, ..., zero_point] ---
            n_neg_inner = mid_idx - 1  # 不含边界 lo 和中心 zero_point
            neg_half_range = zero_point - lo
            neg_equal_spacing = neg_half_range / mid_idx if mid_idx > 0 else neg_half_range
            neg_min_spacing = neg_equal_spacing * min_spacing_ratio
            
            if n_neg_inner > 0:
                neg_data = data_col[data_col < zero_point]
                if len(neg_data) > 100:
                    pcts = np.linspace(0, 100, n_neg_inner + 2)[1:-1]
                    neg_inner = np.sort(np.percentile(neg_data, pcts))
                else:
                    # 数据不足，回退到等间距
                    neg_inner = np.linspace(lo, zero_point, n_neg_inner + 2)[1:-1]
                neg_all = np.concatenate([[lo], neg_inner, [zero_point]])
            else:
                neg_all = np.array([lo, zero_point])
            neg_all = self._enforce_min_spacing(neg_all, neg_min_spacing, lo, zero_point)
            
            # --- 正半轴：[zero_point, ..., hi] ---
            n_pos_total = n_sets - mid_idx  # 含 hi，不含 zero_point
            n_pos_inner = n_pos_total - 1   # 不含边界 zero_point 和 hi
            pos_half_range = hi - zero_point
            pos_equal_spacing = pos_half_range / (n_pos_total) if n_pos_total > 0 else pos_half_range
            pos_min_spacing = pos_equal_spacing * min_spacing_ratio
            
            if n_pos_inner > 0:
                pos_data = data_col[data_col > zero_point]
                if len(pos_data) > 100:
                    pcts = np.linspace(0, 100, n_pos_inner + 2)[1:-1]
                    pos_inner = np.sort(np.percentile(pos_data, pcts))
                else:
                    pos_inner = np.linspace(zero_point, hi, n_pos_inner + 2)[1:-1]
                pos_all = np.concatenate([[zero_point], pos_inner, [hi]])
            else:
                pos_all = np.array([zero_point, hi])
            pos_all = self._enforce_min_spacing(pos_all, pos_min_spacing, zero_point, hi)
            
            # 合并（去掉重复的 zero_point）
            centers = np.concatenate([neg_all, pos_all[1:]])
            
            print(f"    [对称模式] ZE中心固定={zero_point:.3f}, "
                  f"负半轴{len(neg_all)}点, 正半轴{len(pos_all)}点")
        else:
            # ===== 非对称变量：全范围百分位数 + 最小间距 =====
            equal_spacing = total_range / (n_sets - 1) if n_sets > 1 else total_range
            min_spacing = equal_spacing * min_spacing_ratio
            
            percentiles = np.linspace(0, 100, n_sets + 2)[1:-1]
            centers = np.percentile(data_col, percentiles)
            centers = np.clip(centers, lo, hi)
            centers = np.sort(centers)
            
            # 去重后数量修正
            centers = np.unique(np.round(centers, 6))
            if len(centers) < n_sets:
                equal_c = np.linspace(lo, hi, n_sets)
                centers = np.sort(np.unique(np.concatenate([centers, equal_c])))
            if len(centers) > n_sets:
                idx = np.linspace(0, len(centers) - 1, n_sets).astype(int)
                centers = centers[idx]
            
            centers[0] = lo
            centers[-1] = hi
            centers = self._enforce_min_spacing(centers, min_spacing, lo, hi)
        
        # ===== 诊断信息：显示间距 =====
        gaps = np.diff(centers)
        equal_gap = total_range / (n_sets - 1) if n_sets > 1 else total_range
        min_gap_actual = gaps.min() if len(gaps) > 0 else 0
        print(f"    [间距] 最小={min_gap_actual:.4f}, 最大={gaps.max():.4f}, "
              f"等间距={equal_gap:.4f}, 比值={min_gap_actual/equal_gap:.2f}")
        
        # ===== 生成三角形MF =====
        for i in range(n_sets):
            c = centers[i]
            if i == 0:
                a, b, r = lo, lo, (centers[1] if n_sets > 1 else hi)
            elif i == n_sets - 1:
                a, b, r = centers[i - 1], hi, hi
            else:
                a, b, r = centers[i - 1], c, centers[i + 1]
            fvar.add_mf(TriangularMF(set_names[i], a, b, r))
        
        return fvar
    
    @staticmethod
    def _enforce_min_spacing(centers, min_spacing, lo, hi):
        """
        强制相邻MF中心之间至少有 min_spacing 的间距
        
        算法：
          1. 将小于 min_spacing 的间距扩大到 min_spacing
          2. 按比例缩放所有间距，使总和 = (hi - lo)
          3. 若范围不足以容纳所有 min_spacing，回退到等间距
        
        参数:
          centers: MF中心数组（已排序，首尾为边界）
          min_spacing: 最小允许间距
          lo, hi: 变量范围边界
        返回:
          调整后的中心数组
        """
        n = len(centers)
        if n < 2 or min_spacing <= 0:
            return centers
        
        centers = np.array(centers, dtype=float)
        total_range = hi - lo
        gaps = np.diff(centers)
        
        # 如果所有间距已满足，直接返回
        if np.all(gaps >= min_spacing - 1e-9):
            return centers
        
        # 将过小间距扩大到 min_spacing
        clamped_gaps = np.maximum(gaps, min_spacing)
        total_clamped = clamped_gaps.sum()
        
        if total_clamped < 1e-9:
            # 退化情况：回退到等间距
            return np.linspace(lo, hi, n)
        
        # 按比例缩放使总和 = total_range
        clamped_gaps = clamped_gaps * (total_range / total_clamped)
        
        # 从左到右重建中心位置
        new_centers = np.zeros(n)
        new_centers[0] = lo
        for i in range(1, n):
            new_centers[i] = new_centers[i - 1] + clamped_gaps[i - 1]
        new_centers[-1] = hi  # 精确设置末端
        
        return new_centers
    
    # ===== 聚类法模糊系统辨识 =====
    
    def generate_from_clustering(self, ra=None, max_cluster_samples=5000,
                                max_rules=50):
        """
        基于减法聚类的全自动模糊系统辨识
        
        核心思路（前件聚类法）：
          1. 仅在输入空间中进行减法聚类，找到不同的"操作区域"
          2. 每个聚类 = 一条模糊规则
          3. 聚类中心投影 -> 高斯MF的中心
          4. 聚类内数据散布 -> 高斯MF的宽度(sigma)
          5. 聚类内数据的加权平均输出 -> 规则后件值
        
        与 Wang-Mendel 方法的区别：
          - Wang-Mendel: 先独立定义MF -> 再组合提取规则（MF和规则分开）
          - 聚类法: 聚类同时确定MF和规则（一体化，全数据驱动）
        
        所有参数均由数据自动确定：
          * 规则数量 = 聚类数量（自动确定，不超过max_rules）
          * MF形状 = 高斯（由聚类产生的自然分布形状）
          * MF参数 = 聚类中心和宽度（100%来自数据）
          * 规则后件 = 聚类内数据的加权平均输出
        
        参数:
          ra: 聚类半径（在标准化空间中），None=自动搜索最佳ra
          max_cluster_samples: 聚类使用的最大数据量
          max_rules: 规则数上限（自动搜索ra使规则数不超过此值，但尽量多）
        """
        print(f"\n{'='*60}")
        print(f"[2-3/5] Clustering-based Fuzzy System Identification")
        print(f"{'='*60}")
        
        n_in = self.input_data.shape[1]
        n_out = self.output_data.shape[1]
        n_total = len(self.input_data)
        
        # ===== 分层子采样（确保输入空间全范围覆盖）=====
        # 普通随机采样会导致稀有状态（如大偏差）被淹没
        # 分层采样确保输入空间各区域有均匀代表
        if n_total > max_cluster_samples:
            input_sub, output_sub = self._stratified_subsample(
                self.input_data, self.output_data,
                n_samples=max_cluster_samples
            )
            print(f"  Stratified subsample: {n_total} -> {len(input_sub)} "
                  f"(balanced across input range)")
        else:
            input_sub = self.input_data.copy()
            output_sub = self.output_data.copy()
        
        # ===== Z-score标准化（仅对输入空间）=====
        input_mean = input_sub.mean(axis=0)
        input_std = input_sub.std(axis=0)
        input_std[input_std < 1e-10] = 1.0
        
        input_norm = (input_sub - input_mean) / input_std
        
        print(f"  Normalization: Z-score (mean=0, std=1)")
        print(f"  Input space dimension: {n_in}")
        
        # ===== 自动搜索最佳ra =====
        if ra is None or ra <= 0:
            ra = self._auto_tune_ra(input_norm, max_rules)
        
        print(f"  Effective ra: {ra:.4f}")
        
        # ===== 执行减法聚类（仅在输入空间中）=====
        print(f"  Running subtractive clustering in {n_in}D input space...")
        centers_norm, sigmas_norm = subtractive_clustering(
            input_norm, ra=ra, max_clusters=max_rules, verbose=True
        )
        n_clusters = len(centers_norm)
        
        if n_clusters == 0:
            print("  [WARN] No clusters found, using global mean")
            centers_norm = np.array([np.mean(input_norm, axis=0)])
            sigmas_norm = np.array([np.std(input_norm, axis=0)])
            n_clusters = 1
        
        # ===== 反标准化到原始输入空间 =====
        input_centers = centers_norm * input_std + input_mean   # (K, n_inputs)
        input_sigmas = sigmas_norm * input_std                   # (K, n_inputs)
        
        # ===== 强制最小sigma =====
        # 确保每个MF至少覆盖变量范围的一定比例
        # 防止过窄的MF导致推理时大量输入"漏掉"
        for i, var_name in enumerate(self.input_col_names):
            lo, hi = VARIABLE_RANGES[var_name]
            min_sigma = (hi - lo) / (n_clusters * 2.5)  # 至少覆盖范围的1/(2.5K)
            input_sigmas[:, i] = np.maximum(input_sigmas[:, i], min_sigma)
        
        # ===== 计算每条规则的后件值 =====
        # 将每个数据点按模糊隶属度分配到各聚类，加权平均得到输出
        print(f"\n  Computing consequent values for {n_clusters} clusters...")
        
        output_values = np.zeros((n_clusters, n_out))
        cluster_supports = np.zeros(n_clusters, dtype=int)
        
        # 计算每个数据点对每个聚类的隶属度
        # μ_k(x) = exp(-||x - c_k||² / (2 * ra²))  (在标准化空间中)
        membership = np.zeros((len(input_sub), n_clusters))
        for k in range(n_clusters):
            diff = input_norm - centers_norm[k]
            dist2 = np.sum(diff ** 2, axis=1)
            membership[:, k] = np.exp(-dist2 / (2.0 * (ra * 0.5) ** 2))
        
        # 硬分配用于计算支持度
        hard_assignment = np.argmax(membership, axis=1)
        
        for k in range(n_clusters):
            mask = hard_assignment == k
            cluster_supports[k] = int(np.sum(mask))
            
            # 加权平均输出（使用隶属度作为权重）
            w = membership[:, k]
            w_sum = w.sum()
            if w_sum > 1e-10:
                for j in range(n_out):
                    output_values[k, j] = np.average(output_sub[:, j], weights=w)
            else:
                output_values[k] = output_sub.mean(axis=0)
        
        print(f"\n  Clustering result: {n_clusters} clusters -> {n_clusters} rules")
        
        # ===== 存储聚类模式标记 =====
        self._clustering_mode = True
        self._cluster_centers = input_centers
        self._cluster_sigmas = input_sigmas
        self._cluster_outputs = output_values
        
        # ===== 创建高斯隶属函数 =====
        self.input_vars = {}
        
        for i, var_name in enumerate(self.input_col_names):
            var_range = VARIABLE_RANGES[var_name]
            fvar = FuzzyVariable(var_name, var_range)
            
            for c in range(n_clusters):
                mf = GaussianMF(
                    name=f"C{c+1}",
                    center=float(input_centers[c, i]),
                    sigma=float(input_sigmas[c, i])
                )
                fvar.add_mf(mf)
            
            self.input_vars[var_name] = fvar
            print(f"\n  {var_name} ({n_clusters} Gaussian MFs):")
            for mf in fvar.mfs:
                print(f"    {mf}")
        
        # ===== 创建规则（每个聚类 = 一条规则）=====
        self.rules = []
        
        for c in range(n_clusters):
            antecedent = {}
            for var_name in self.input_col_names:
                antecedent[var_name] = f"C{c+1}"
            
            consequent = {}
            for j, out_name in enumerate(self.output_col_names):
                consequent[out_name] = float(output_values[c, j])
            
            rule = FuzzyRule(antecedent, consequent, strength=1.0,
                           support_count=int(cluster_supports[c]))
            self.rules.append(rule)
        
        print(f"\n  Rule summary:")
        print(f"    Total rules: {n_clusters}")
        for i, rule in enumerate(self.rules):
            out_str = ", ".join(f"{k}={v:.4f}" for k, v in rule.consequent_values.items())
            print(f"    Rule {i+1}: support={rule.support_count}, output=[{out_str}]")
        
        return self.rules
    
    def _stratified_subsample(self, input_data, output_data, n_samples=5000,
                               n_bins=20):
        """
        分层子采样：确保输入空间各区域有均匀代表
        
        普通随机采样的问题：
          - 数据大部分集中在"正常工况"（车身居中，偏差≈0）
          - 采样后极端工况（大偏差）几乎消失
          - 聚类只能找到正常工况附近的微小差异
        
        分层采样的解决方案：
          - 将主要偏差维度（front_lr_diff）分成等宽bins
          - 从每个bin中等量采样
          - 极端工况（少数据bins）被过采样，正常工况（多数据bins）被欠采样
          - 结果：输入空间全范围均匀覆盖
        """
        n = len(input_data)
        
        # 使用 front_lr_diff (第0列) 和 rear_lr_diff (第1列) 做2D分层
        x1 = input_data[:, 0]  # front_lr_diff
        x2 = input_data[:, 1]  # rear_lr_diff
        
        # 创建2D网格bins
        edges1 = np.linspace(x1.min() - 1e-6, x1.max() + 1e-6, n_bins + 1)
        edges2 = np.linspace(x2.min() - 1e-6, x2.max() + 1e-6, n_bins + 1)
        
        bin1 = np.digitize(x1, edges1) - 1
        bin2 = np.digitize(x2, edges2) - 1
        bin1 = np.clip(bin1, 0, n_bins - 1)
        bin2 = np.clip(bin2, 0, n_bins - 1)
        
        # 组合bin索引
        combined_bin = bin1 * n_bins + bin2
        unique_bins = np.unique(combined_bin)
        
        # 从有数据的bins中等量采样
        samples_per_bin = max(1, n_samples // len(unique_bins))
        selected = []
        
        for b in unique_bins:
            mask_indices = np.where(combined_bin == b)[0]
            if len(mask_indices) >= samples_per_bin:
                sel = np.random.choice(mask_indices, samples_per_bin, replace=False)
            else:
                # 数据不够：有放回过采样
                sel = np.random.choice(mask_indices, samples_per_bin, replace=True)
            selected.extend(sel)
        
        selected = np.array(selected)
        np.random.shuffle(selected)
        
        # 截断到目标数量
        if len(selected) > n_samples:
            selected = selected[:n_samples]
        
        print(f"    Stratified bins: {len(unique_bins)} non-empty out of {n_bins*n_bins}")
        print(f"    Samples per bin: ~{samples_per_bin}")
        
        return input_data[selected], output_data[selected]
    
    def _auto_tune_ra(self, data_norm, max_rules=50, max_iter=20):
        """
        自动搜索最佳聚类半径ra
        
        策略：在规则数不超过 max_rules（上限）的前提下，尽量多生成规则
        （规则越多 → R² 越高）
        
        使用二分搜索：ra越小->聚类越多，ra越大->聚类越少
        目标：找到最小的 ra 使得 n_clusters <= max_rules
        """
        print(f"\n  Auto-tuning ra for max {max_rules} rules (maximize within limit)...")
        
        # 用一个较小的子集做快速搜索（避免每次都跑完整聚类）
        n = len(data_norm)
        if n > 2000:
            search_idx = np.random.choice(n, 2000, replace=False)
            search_data = data_norm[search_idx]
        else:
            search_data = data_norm
        
        ra_lo, ra_hi = 0.02, 2.0
        best_ra = 0.5       # 保底值（较大ra，规则较少）
        best_n_clusters = 1  # 对应的规则数
        
        for iteration in range(max_iter):
            ra_mid = (ra_lo + ra_hi) / 2.0
            centers, _ = subtractive_clustering(
                search_data, ra=ra_mid, max_clusters=max_rules, verbose=False
            )
            n_clusters = len(centers)
            
            print(f"    iter {iteration+1}: ra={ra_mid:.4f} -> {n_clusters} clusters "
                  f"(limit={max_rules})")
            
            if n_clusters <= max_rules:
                # 没超上限 → 记录为候选，并尝试更小的ra以获得更多规则
                if n_clusters > best_n_clusters:
                    best_n_clusters = n_clusters
                    best_ra = ra_mid
                ra_hi = ra_mid  # 缩小ra → 更多规则
            else:
                # 超过上限 → 需要增大ra
                ra_lo = ra_mid
            
            # 收敛条件
            if ra_hi - ra_lo < 0.003:
                break
        
        print(f"  Auto-tuned ra = {best_ra:.4f} (expected ~{best_n_clusters} rules, limit={max_rules})")
        return best_ra
    
    # ===== Wang-Mendel 规则提取 =====
    
    def extract_rules(self, min_support=3, min_strength=0.01,
                      enable_rule_completion=False,
                      completion_max_added=220,
                      mirror_strength_scale=0.85,
                      extrap_strength_scale=0.70,
                      extrap_gain=0.25):
        """
        使用 Wang-Mendel 方法从数据中提取模糊规则
        
        Wang-Mendel 算法步骤：
          1. 对每个数据点，计算所有输入变量的最大隶属度模糊集
          2. 该组合（前件）的后件 = 该数据点的输出值
          3. 规则强度 = 各输入变量最大隶属度的乘积
          4. 冲突消解：同一前件下，保留强度最高的规则
          5. 后件值 = 所有匹配数据点的加权平均输出值
        
        参数:
          min_support: 最小支持度（匹配该规则的数据点数量）
          min_strength: 最小规则强度
          enable_rule_completion: 是否启用规则覆盖补全
          completion_max_added: 补全规则上限
          mirror_strength_scale: 镜像规则强度衰减
          extrap_strength_scale: 外推规则强度衰减
          extrap_gain: 外推规则输出放大系数
        """
        print(f"\n{'='*60}")
        print(f"[3/5] 提取模糊规则 (Wang-Mendel 方法)")
        print(f"{'='*60}")
        
        n_samples = len(self.input_data)
        
        # 存储每条规则的信息: key → {strengths: [], outputs: [], count: int}
        rule_candidates = defaultdict(lambda: {
            'strengths': [],
            'outputs': [],
            'count': 0
        })
        
        print(f"  处理 {n_samples} 条数据...")
        
        for idx in range(n_samples):
            # 对每个输入变量，找到最大隶属度的模糊集
            antecedent = {}
            firing_strength = 1.0
            
            for i, var_name in enumerate(self.input_col_names):
                x = self.input_data[idx, i]
                fvar = self.input_vars[var_name]
                best_mf, best_mu = fvar.get_dominant_mf(x)
                antecedent[var_name] = best_mf
                firing_strength *= best_mu
            
            if firing_strength < 1e-10:
                continue
            
            # 规则键
            key = tuple(sorted(antecedent.items()))
            
            # 记录该规则的触发强度和对应输出
            rule_candidates[key]['strengths'].append(firing_strength)
            rule_candidates[key]['outputs'].append(self.output_data[idx])
            rule_candidates[key]['count'] += 1
        
        print(f"  发现 {len(rule_candidates)} 种不同的前件组合")
        
        # 生成规则：后件值 = 加权平均输出
        self.rules = []
        pruned_support = 0
        pruned_strength = 0
        
        for key, info in rule_candidates.items():
            count = info['count']
            strengths = np.array(info['strengths'])
            outputs = np.array(info['outputs'])
            
            # 剪枝：支持度
            if count < min_support:
                pruned_support += 1
                continue
            
            # 计算加权平均后件值
            weights = strengths / strengths.sum()
            weighted_outputs = {}
            for j, out_name in enumerate(self.output_col_names):
                weighted_outputs[out_name] = float(np.average(outputs[:, j], weights=weights))
            
            max_strength = float(strengths.max())
            
            # 剪枝：强度
            if max_strength < min_strength:
                pruned_strength += 1
                continue
            
            antecedent = dict(key)
            rule = FuzzyRule(antecedent, weighted_outputs, max_strength, count)
            self.rules.append(rule)
        
        # 按强度排序
        self.rules.sort(key=lambda r: r.strength, reverse=True)
        
        print(f"\n  规则提取结果:")
        print(f"    有效规则数: {len(self.rules)}")
        print(f"    因支持度不足剪枝: {pruned_support} 条 (min_support={min_support})")
        print(f"    因强度不足剪枝: {pruned_strength} 条 (min_strength={min_strength})")
        
        # 统计覆盖率
        total_covered = sum(r.support_count for r in self.rules)
        print(f"    数据覆盖率: {total_covered}/{n_samples} ({total_covered/n_samples*100:.1f}%)")

        # 规则覆盖补全：对称镜像 + 严重偏差外推
        if enable_rule_completion:
            self._complete_rule_coverage(
                max_added=completion_max_added,
                mirror_strength_scale=mirror_strength_scale,
                extrap_strength_scale=extrap_strength_scale,
                extrap_gain=extrap_gain
            )
        
        return self.rules

    def _get_symmetric_level_map(self, var_name):
        """返回对称变量的等级映射（如 NB=-3 ... ZE=0 ... PB=+3）"""
        fvar = self.input_vars.get(var_name, None)
        if fvar is None:
            return None, None

        mf_names = [mf.name for mf in fvar.mfs]
        if "ZE" not in mf_names:
            return None, None

        ze_idx = mf_names.index("ZE")
        level_by_name = {name: i - ze_idx for i, name in enumerate(mf_names)}
        mirror_by_name = {name: mf_names[2 * ze_idx - i] for i, name in enumerate(mf_names)}
        return level_by_name, mirror_by_name

    def _make_rule_key(self, antecedent):
        return tuple((name, antecedent[name]) for name in self.input_col_names)

    @staticmethod
    def _clip_outputs(consequent):
        return {k: float(np.clip(v, -1.0, 1.0)) for k, v in consequent.items()}

    def _complete_rule_coverage(self, max_added=220, mirror_strength_scale=0.85,
                                extrap_strength_scale=0.70, extrap_gain=0.25):
        """
        规则覆盖补全（根治转弯缺失）：
          1) 对称镜像：左转规则 -> 右转规则（或反向）
          2) 偏差外推：NS/PS -> NM/PM -> NB/PB，输出按偏差级别放大
        """
        if len(self.rules) == 0:
            return

        symmetric_vars = [v for v in self.input_col_names if v.endswith("_lr_diff")]
        if len(symmetric_vars) == 0:
            return

        # 规则索引（用于判重）
        existing = {self._make_rule_key(r.antecedent): r for r in self.rules}
        added_rules = []

        # 构建每个对称变量的等级和镜像映射
        var_maps = {}
        for v in symmetric_vars:
            level_map, mirror_map = self._get_symmetric_level_map(v)
            if level_map is not None:
                var_maps[v] = (level_map, mirror_map)

        if len(var_maps) == 0:
            return

        # --- A. 对称镜像补全 ---
        original_rules = list(self.rules)
        for rule in original_rules:
            if len(added_rules) >= max_added:
                break

            mirrored_ant = dict(rule.antecedent)
            changed = False
            for v, (_, mirror_map) in var_maps.items():
                old_name = mirrored_ant[v]
                new_name = mirror_map.get(old_name, old_name)
                mirrored_ant[v] = new_name
                changed = changed or (new_name != old_name)

            if not changed:
                continue

            key = self._make_rule_key(mirrored_ant)
            if key in existing:
                continue

            # 左右镜像下，横向速度与角速度符号反转
            new_con = dict(rule.consequent_values)
            if "output_vx_norm" in new_con:
                new_con["output_vx_norm"] = -new_con["output_vx_norm"]
            if "output_omega_norm" in new_con:
                new_con["output_omega_norm"] = -new_con["output_omega_norm"]
            new_con = self._clip_outputs(new_con)

            new_rule = FuzzyRule(
                mirrored_ant, new_con,
                strength=float(rule.strength) * mirror_strength_scale,
                support_count=0
            )
            existing[key] = new_rule
            added_rules.append(new_rule)

        # --- B. 偏差等级外推补全 ---
        source_rules = list(self.rules) + list(added_rules)
        for rule in source_rules:
            if len(added_rules) >= max_added:
                break

            for v, (level_map, _) in var_maps.items():
                cur_name = rule.antecedent[v]
                cur_level = level_map.get(cur_name, 0)
                if cur_level == 0:
                    continue

                # 逐级向更大偏差外推，直到边界
                sign = 1 if cur_level > 0 else -1
                max_abs = max(abs(x) for x in level_map.values())
                for next_abs in range(abs(cur_level) + 1, max_abs + 1):
                    if len(added_rules) >= max_added:
                        break
                    target_level = sign * next_abs
                    target_name = None
                    for name, lv in level_map.items():
                        if lv == target_level:
                            target_name = name
                            break
                    if target_name is None:
                        continue

                    new_ant = dict(rule.antecedent)
                    new_ant[v] = target_name
                    key = self._make_rule_key(new_ant)
                    if key in existing:
                        continue

                    # 偏差越大，纠偏输出幅度应更大
                    level_step = next_abs - abs(cur_level)
                    scale = 1.0 + extrap_gain * level_step
                    new_con = {
                        out_name: float(val) * scale
                        for out_name, val in rule.consequent_values.items()
                    }
                    new_con = self._clip_outputs(new_con)

                    new_rule = FuzzyRule(
                        new_ant, new_con,
                        strength=float(rule.strength) * (extrap_strength_scale ** level_step),
                        support_count=0
                    )
                    existing[key] = new_rule
                    added_rules.append(new_rule)

        if len(added_rules) > 0:
            self.rules.extend(added_rules)
            self.rules.sort(key=lambda r: r.strength, reverse=True)
            print(f"\n  规则覆盖补全:")
            print(f"    新增规则: {len(added_rules)} 条")
            print(f"    补全后总规则数: {len(self.rules)} 条")
            print(f"    补全策略: 对称镜像 + 偏差外推")
    
    # ===== 评估 =====
    
    def evaluate(self, inputs):
        """
        模糊推理：给定输入值，计算输出
        
        参数:
          inputs: dict {var_name: float_value} 或 list/array (按input_col_names顺序)
        
        返回:
          dict {output_name: float_value}
        """
        if isinstance(inputs, (list, np.ndarray)):
            inputs = {name: float(inputs[i]) for i, name in enumerate(self.input_col_names)}
        
        sum_weighted = {name: 0.0 for name in self.output_col_names}
        sum_weights = 0.0
        
        for rule in self.rules:
            # 计算规则触发强度（AND = 乘积）
            firing = 1.0
            for var_name, mf_name in rule.antecedent.items():
                fvar = self.input_vars[var_name]
                mf_idx = fvar.get_mf_index(mf_name)
                if mf_idx >= 0:
                    mu = fvar.mfs[mf_idx].evaluate(inputs[var_name])
                    firing *= mu
                else:
                    firing = 0.0
                    break
            
            if firing < 1e-10:
                continue
            
            # 加权累加
            for out_name in self.output_col_names:
                sum_weighted[out_name] += firing * rule.consequent_values[out_name]
            sum_weights += firing
        
        # 去模糊化：加权平均
        if sum_weights > 1e-10:
            return {name: val / sum_weights for name, val in sum_weighted.items()}
        else:
            return {name: 0.0 for name in self.output_col_names}
    
    def evaluate_dataset(self):
        """评估规则库在完整数据集上的表现"""
        print(f"\n{'='*60}")
        print(f"[4/5] 评估模糊规则库性能")
        print(f"{'='*60}")
        
        n = len(self.input_data)
        predictions = np.zeros((n, len(self.output_col_names)))
        
        for i in range(n):
            result = self.evaluate(self.input_data[i])
            for j, name in enumerate(self.output_col_names):
                predictions[i, j] = result[name]
            
            if (i + 1) % 50000 == 0:
                print(f"  进度: {i+1}/{n} ({(i+1)/n*100:.1f}%)")
        
        # 计算评估指标
        print(f"\n  性能指标:")
        for j, name in enumerate(self.output_col_names):
            y_true = self.output_data[:, j]
            y_pred = predictions[:, j]
            
            mse = np.mean((y_true - y_pred) ** 2)
            mae = np.mean(np.abs(y_true - y_pred))
            ss_res = np.sum((y_true - y_pred) ** 2)
            ss_tot = np.sum((y_true - np.mean(y_true)) ** 2)
            r2 = 1 - ss_res / max(1e-10, ss_tot)
            
            print(f"\n    {name}:")
            print(f"      MSE  = {mse:.6f}")
            print(f"      MAE  = {mae:.6f}")
            print(f"      R2   = {r2:.4f}")
            
            if r2 < 0.5:
                print(f"      Warning: R2 is low, consider more fuzzy sets or data-driven/clustering method")
        
        return predictions
    
    # ===== 打印规则 =====
    
    def print_rules(self, max_show=50):
        """打印规则列表"""
        print(f"\n  规则列表 (共 {len(self.rules)} 条, 显示前 {min(max_show, len(self.rules))} 条):")
        print(f"  {'─'*90}")
        for i, rule in enumerate(self.rules[:max_show]):
            print(f"  Rule {i+1:3d}: {rule}")
        if len(self.rules) > max_show:
            print(f"  ... 省略 {len(self.rules) - max_show} 条 ...")
    
    # ===== C# 代码导出 =====
    
    def export_csharp(self, output_path, class_name="FuzzyController"):
        """
        导出为C#模糊控制器代码
        
        根据模式自动选择导出方式:
          - Wang-Mendel模式: 三角形MF + 规则索引表
          - 聚类模式: 高斯MF + 聚类中心/宽度表
        """
        if self._clustering_mode:
            return self._export_csharp_clustering(output_path, class_name)
        return self._export_csharp_wangmendel(output_path, class_name)
    
    def _export_csharp_clustering(self, output_path, class_name="FuzzyController"):
        """
        聚类模式C#代码导出（高斯隶属函数）
        
        生成的代码结构更简洁：
          - Centers[K, N_INPUTS]: 每条规则在各输入维度的高斯中心
          - Sigmas[K, N_INPUTS]:  每条规则在各输入维度的高斯宽度
          - Outputs[K, N_OUTPUTS]: 每条规则的输出值
          - 推理 = Σ(firing_k * output_k) / Σ(firing_k)
        """
        print(f"\n{'='*60}")
        print(f"[5/5] 导出C#代码 (聚类模式/高斯MF): {output_path}")
        print(f"{'='*60}")
        
        n_rules = len(self.rules)
        n_inputs = len(self.input_col_names)
        n_outputs = len(self.output_col_names)
        timestamp = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
        
        lines = []
        lines.append(f"// 模糊控制器 - 自动生成 (聚类法 / 高斯隶属函数)")
        lines.append(f"// 生成时间: {timestamp}")
        lines.append(f"// 生成方法: 减法聚类 (Subtractive Clustering, Chiu 1994)")
        lines.append(f"// 数据样本数: {len(self.input_data)}")
        lines.append(f"// 聚类数(=规则数): {n_rules}")
        lines.append(f"// 输入变量: {', '.join(self.input_col_names)}")
        lines.append(f"// 输出变量: {', '.join(self.output_col_names)}")
        lines.append(f"// 隶属函数类型: 高斯 μ(x) = exp(-(x-c)²/(2σ²))")
        lines.append(f"//")
        lines.append(f"// 特点: 所有参数（MF数量、中心、宽度、规则数）均由数据自动确定")
        lines.append(f"")
        lines.append(f"using System;")
        lines.append(f"using UnityEngine;")
        lines.append(f"")
        lines.append(f"public class {class_name}")
        lines.append(f"{{")
        
        # ===== 常量 =====
        lines.append(f"    private const int N_RULES = {n_rules};")
        lines.append(f"    private const int N_INPUTS = {n_inputs};")
        lines.append(f"    private const int N_OUTPUTS = {n_outputs};")
        lines.append(f"")
        
        # ===== 聚类中心 =====
        lines.append(f"    // ===== 聚类中心 (高斯MF的中心 c) =====")
        lines.append(f"    // Centers[规则索引, 输入变量索引]")
        lines.append(f"    // 输入变量顺序: {', '.join(self.input_col_names)}")
        lines.append(f"    private static readonly float[,] Centers = {{")
        for r in range(n_rules):
            vals = ", ".join(f"{self._cluster_centers[r, i]:10.6f}f" for i in range(n_inputs))
            lines.append(f"        {{ {vals} }},  // Rule {r+1}")
        lines.append(f"    }};")
        lines.append(f"")
        
        # ===== 聚类宽度 =====
        lines.append(f"    // ===== 聚类宽度 (高斯MF的sigma σ) =====")
        lines.append(f"    // Sigmas[规则索引, 输入变量索引]")
        lines.append(f"    private static readonly float[,] Sigmas = {{")
        for r in range(n_rules):
            vals = ", ".join(f"{self._cluster_sigmas[r, i]:10.6f}f" for i in range(n_inputs))
            lines.append(f"        {{ {vals} }},  // Rule {r+1}")
        lines.append(f"    }};")
        lines.append(f"")
        
        # ===== 规则输出 =====
        lines.append(f"    // ===== 规则输出值 (Takagi-Sugeno零阶) =====")
        lines.append(f"    // Outputs[规则索引, 输出变量索引]")
        lines.append(f"    // 输出变量顺序: {', '.join(self.output_col_names)}")
        lines.append(f"    private static readonly float[,] Outputs = {{")
        for r in range(n_rules):
            vals = ", ".join(f"{self._cluster_outputs[r, j]:10.6f}f" for j in range(n_outputs))
            lines.append(f"        {{ {vals} }},  // Rule {r+1}")
        lines.append(f"    }};")
        lines.append(f"")
        
        # ===== 推理方法 =====
        lines.append(f"    // ===== 模糊推理接口 =====")
        lines.append(f"    /// <summary>")
        lines.append(f"    /// 基于聚类的模糊推理")
        lines.append(f"    /// 每条规则的触发强度 = 各输入维度高斯隶属度的乘积")
        lines.append(f"    /// 输出 = 加权平均 (Takagi-Sugeno零阶去模糊化)")
        lines.append(f"    /// </summary>")
        lines.append(f"    public static void Evaluate(")
        lines.append(f"        float frontLRDiff, float rearLRDiff,")
        lines.append(f"        float frontCenter, float rearCenter,")
        lines.append(f"        out float outputVx, out float outputOmega)")
        lines.append(f"    {{")
        lines.append(f"        float sumWVx = 0f, sumWOmega = 0f, sumW = 0f;")
        lines.append(f"")
        lines.append(f"        for (int r = 0; r < N_RULES; r++)")
        lines.append(f"        {{")
        lines.append(f"            // 各输入维度的高斯隶属度乘积 (AND运算)")
        lines.append(f"            float firing = GaussMF(frontLRDiff, Centers[r, 0], Sigmas[r, 0])")
        lines.append(f"                         * GaussMF(rearLRDiff,  Centers[r, 1], Sigmas[r, 1])")
        lines.append(f"                         * GaussMF(frontCenter, Centers[r, 2], Sigmas[r, 2])")
        lines.append(f"                         * GaussMF(rearCenter,  Centers[r, 3], Sigmas[r, 3]);")
        lines.append(f"")
        lines.append(f"            if (firing > 1e-6f)")
        lines.append(f"            {{")
        lines.append(f"                sumWVx    += firing * Outputs[r, 0];")
        lines.append(f"                sumWOmega += firing * Outputs[r, 1];")
        lines.append(f"                sumW      += firing;")
        lines.append(f"            }}")
        lines.append(f"        }}")
        lines.append(f"")
        lines.append(f"        // Takagi-Sugeno零阶去模糊化: 加权平均")
        lines.append(f"        if (sumW > 1e-6f)")
        lines.append(f"        {{")
        lines.append(f"            outputVx    = Mathf.Clamp(sumWVx / sumW, -1f, 1f);")
        lines.append(f"            outputOmega = Mathf.Clamp(sumWOmega / sumW, -1f, 1f);")
        lines.append(f"        }}")
        lines.append(f"        else")
        lines.append(f"        {{")
        lines.append(f"            outputVx = 0f;")
        lines.append(f"            outputOmega = 0f;")
        lines.append(f"        }}")
        lines.append(f"    }}")
        lines.append(f"")
        
        # ===== 高斯隶属函数 =====
        lines.append(f"    // ===== 高斯隶属函数 =====")
        lines.append(f"    // μ(x) = exp(-(x-c)²/(2σ²))")
        lines.append(f"    private static float GaussMF(float x, float center, float sigma)")
        lines.append(f"    {{")
        lines.append(f"        float d = x - center;")
        lines.append(f"        return Mathf.Exp(-d * d / (2f * sigma * sigma));")
        lines.append(f"    }}")
        lines.append(f"")
        
        # ===== 便捷方法 =====
        lines.append(f"    // ===== 便捷方法 =====")
        lines.append(f"    /// <summary>")
        lines.append(f"    /// 直接从6个传感器原始值计算控制输出")
        lines.append(f"    /// </summary>")
        lines.append(f"    public static void EvaluateFromSensors(")
        lines.append(f"        float sensorFL, float sensorFC, float sensorFR,")
        lines.append(f"        float sensorRL, float sensorRC, float sensorRR,")
        lines.append(f"        float maxField,")
        lines.append(f"        out float outputVx, out float outputOmega)")
        lines.append(f"    {{")
        lines.append(f"        float invMax = 1f / Mathf.Max(1e-6f, maxField);")
        lines.append(f"        float frontLRDiff = Mathf.Clamp((sensorFL - sensorFR) * invMax, -1f, 1f);")
        lines.append(f"        float rearLRDiff = Mathf.Clamp((sensorRL - sensorRR) * invMax, -1f, 1f);")
        lines.append(f"        float frontCenter = Mathf.Clamp01(sensorFC * invMax);")
        lines.append(f"        float rearCenter = Mathf.Clamp01(sensorRC * invMax);")
        lines.append(f"        Evaluate(frontLRDiff, rearLRDiff, frontCenter, rearCenter, out outputVx, out outputOmega);")
        lines.append(f"    }}")
        lines.append(f"")
        lines.append(f"    /// <summary>")
        lines.append(f"    /// 获取控制器信息（调试用）")
        lines.append(f"    /// </summary>")
        lines.append(f"    public static string GetInfo()")
        lines.append(f"    {{")
        lines.append(f"        return $\"FuzzyController(Clustering): {{N_RULES}} rules, {{N_INPUTS}} inputs, generated {timestamp}\";")
        lines.append(f"    }}")
        lines.append(f"}}")
        
        # 写入文件
        code = "\n".join(lines)
        out_dir = os.path.dirname(output_path)
        if out_dir and not os.path.exists(out_dir):
            os.makedirs(out_dir)
        
        with open(output_path, 'w', encoding='utf-8') as f:
            f.write(code)
        
        print(f"  [OK] Generated {class_name}.cs (Clustering / Gaussian MF)")
        print(f"     Path: {output_path}")
        print(f"     Rules: {n_rules}")
        print(f"     MF type: Gaussian")
        print(f"     Lines: {len(lines)}")
    
    def _export_csharp_wangmendel(self, output_path, class_name="FuzzyController"):
        """
        Wang-Mendel模式C#代码导出（三角形隶属函数）
        """
        print(f"\n{'='*60}")
        print(f"[5/5] 导出C#代码: {output_path}")
        print(f"{'='*60}")
        
        n_rules = len(self.rules)
        timestamp = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
        
        # 收集各变量的MF参数
        var_info = {}
        for var_name in self.input_col_names:
            fvar = self.input_vars[var_name]
            var_info[var_name] = {
                'mfs': [(mf.name, mf.a, mf.b, mf.c) for mf in fvar.mfs],
                'n_sets': len(fvar.mfs),
                'set_names': [mf.name for mf in fvar.mfs],
            }
        
        # 构建规则表
        rule_rows = []
        for rule in self.rules:
            row = []
            for var_name in self.input_col_names:
                mf_name = rule.antecedent[var_name]
                mf_idx = self.input_vars[var_name].get_mf_index(mf_name)
                row.append(mf_idx)
            for out_name in self.output_col_names:
                row.append(rule.consequent_values[out_name])
            row.append(rule.strength)
            rule_rows.append(row)
        
        # 生成C#代码
        lines = []
        lines.append(f"// 模糊控制器 - 自动生成")
        lines.append(f"// 生成时间: {timestamp}")
        lines.append(f"// 数据样本数: {len(self.input_data)}")
        lines.append(f"// 规则数: {n_rules}")
        lines.append(f"// 输入变量: {', '.join(self.input_col_names)}")
        lines.append(f"// 输出变量: {', '.join(self.output_col_names)}")
        lines.append(f"//")
        lines.append(f"// 隶属函数配置:")
        for var_name, info in var_info.items():
            lines.append(f"//   {var_name}: {info['n_sets']} 集 ({', '.join(info['set_names'])})")
        lines.append(f"")
        lines.append(f"using System;")
        lines.append(f"using UnityEngine;")
        lines.append(f"")
        lines.append(f"public class {class_name}")
        lines.append(f"{{")
        
        # ===== 隶属函数参数 =====
        lines.append(f"    // ===== 三角形隶属函数参数 =====")
        lines.append(f"    // 每行: {{ left, center, right }}")
        lines.append(f"")
        
        cs_var_names = {
            'front_lr_diff': 'MF_FrontLR',
            'rear_lr_diff': 'MF_RearLR',
            'front_center': 'MF_FrontCenter',
            'rear_center': 'MF_RearCenter',
        }
        
        for var_name in self.input_col_names:
            info = var_info[var_name]
            cs_name = cs_var_names[var_name]
            lines.append(f"    // {var_name}: {', '.join(info['set_names'])}")
            lines.append(f"    private static readonly float[,] {cs_name} = {{")
            for mf_name, a, b, c in info['mfs']:
                lines.append(f"        {{ {a:8.5f}f, {b:8.5f}f, {c:8.5f}f }},  // {mf_name}")
            lines.append(f"    }};")
            lines.append(f"")
        
        # ===== 规则库 =====
        n_inputs = len(self.input_col_names)
        n_outputs = len(self.output_col_names)
        n_cols = n_inputs + n_outputs + 1  # +1 for weight
        
        lines.append(f"    // ===== 规则库 ({n_rules} 条规则) =====")
        lines.append(f"    // 每行: {{ 前LR集, 后LR集, 前中集, 后中集, 输出vx, 输出omega, 权重 }}")
        lines.append(f"    private static readonly float[,] Rules = {{")
        
        for i, row in enumerate(rule_rows):
            # 格式化：索引用整数，值用浮点
            parts = []
            for j in range(n_inputs):
                parts.append(f"{int(row[j]):2d}")
            for j in range(n_inputs, n_inputs + n_outputs):
                parts.append(f"{row[j]:9.6f}f")
            parts.append(f"{row[-1]:8.5f}f")
            
            # 注释：显示规则的模糊集名称
            ant_names = []
            for k, var_name in enumerate(self.input_col_names):
                mf_idx = int(row[k])
                mf_name = var_info[var_name]['set_names'][mf_idx]
                ant_names.append(mf_name)
            comment = " & ".join(ant_names)
            
            lines.append(f"        {{ {', '.join(parts)} }},  // Rule {i+1}: {comment}")
        
        lines.append(f"    }};")
        lines.append(f"")
        
        # ===== 模糊集数量常量 =====
        lines.append(f"    private const int N_RULES = {n_rules};")
        lines.append(f"")
        
        # ===== 推理方法 =====
        lines.append(f"    // ===== 模糊推理接口 =====")
        lines.append(f"    /// <summary>")
        lines.append(f"    /// 模糊推理：输入传感器特征，输出控制量")
        lines.append(f"    /// </summary>")
        lines.append(f"    /// <param name=\"frontLRDiff\">前排左右差（归一化 [-1,1]）</param>")
        lines.append(f"    /// <param name=\"rearLRDiff\">后排左右差（归一化 [-1,1]）</param>")
        lines.append(f"    /// <param name=\"frontCenter\">前中传感器（归一化 [0,1]）</param>")
        lines.append(f"    /// <param name=\"rearCenter\">后中传感器（归一化 [0,1]）</param>")
        lines.append(f"    /// <param name=\"outputVx\">输出：横向速度（归一化 [-1,1]）</param>")
        lines.append(f"    /// <param name=\"outputOmega\">输出：角速度（归一化 [-1,1]）</param>")
        lines.append(f"    public static void Evaluate(")
        lines.append(f"        float frontLRDiff, float rearLRDiff,")
        lines.append(f"        float frontCenter, float rearCenter,")
        lines.append(f"        out float outputVx, out float outputOmega)")
        lines.append(f"    {{")
        lines.append(f"        float sumWeightedVx = 0f;")
        lines.append(f"        float sumWeightedOmega = 0f;")
        lines.append(f"        float sumWeights = 0f;")
        lines.append(f"        float bestWeight = -1f;")
        lines.append(f"        float bestVx = 0f;")
        lines.append(f"        float bestOmega = 0f;")
        lines.append(f"")
        lines.append(f"        for (int r = 0; r < N_RULES; r++)")
        lines.append(f"        {{")
        lines.append(f"            int mfFL = (int)Rules[r, 0];")
        lines.append(f"            int mfRL = (int)Rules[r, 1];")
        lines.append(f"            int mfFC = (int)Rules[r, 2];")
        lines.append(f"            int mfRC = (int)Rules[r, 3];")
        lines.append(f"")
        lines.append(f"            // 模糊化：计算各输入的隶属度")
        lines.append(f"            float mu1 = TriMF(frontLRDiff, MF_FrontLR[mfFL, 0], MF_FrontLR[mfFL, 1], MF_FrontLR[mfFL, 2]);")
        lines.append(f"            float mu2 = TriMF(rearLRDiff, MF_RearLR[mfRL, 0], MF_RearLR[mfRL, 1], MF_RearLR[mfRL, 2]);")
        lines.append(f"            float mu3 = TriMF(frontCenter, MF_FrontCenter[mfFC, 0], MF_FrontCenter[mfFC, 1], MF_FrontCenter[mfFC, 2]);")
        lines.append(f"            float mu4 = TriMF(rearCenter, MF_RearCenter[mfRC, 0], MF_RearCenter[mfRC, 1], MF_RearCenter[mfRC, 2]);")
        lines.append(f"")
        lines.append(f"            // AND运算（乘积法）")
        lines.append(f"            float firing = mu1 * mu2 * mu3 * mu4;")
        lines.append(f"")
        lines.append(f"            if (firing > 1e-6f)")
        lines.append(f"            {{")
        lines.append(f"                float outVx = Rules[r, 4];")
        lines.append(f"                float outOmega = Rules[r, 5];")
        lines.append(f"                float ruleWeight = Rules[r, 6];")
        lines.append(f"                float w = firing * ruleWeight;")
        lines.append(f"")
        lines.append(f"                sumWeightedVx += w * outVx;")
        lines.append(f"                sumWeightedOmega += w * outOmega;")
        lines.append(f"                sumWeights += w;")
        lines.append(f"                if (w > bestWeight)")
        lines.append(f"                {{")
        lines.append(f"                    bestWeight = w;")
        lines.append(f"                    bestVx = outVx;")
        lines.append(f"                    bestOmega = outOmega;")
        lines.append(f"                }}")
        lines.append(f"            }}")
        lines.append(f"        }}")
        lines.append(f"")
        lines.append(f"        // 去模糊化：加权平均")
        lines.append(f"        if (sumWeights > 1e-6f)")
        lines.append(f"        {{")
        lines.append(f"            outputVx = Mathf.Clamp(sumWeightedVx / sumWeights, -1f, 1f);")
        lines.append(f"            outputOmega = Mathf.Clamp(sumWeightedOmega / sumWeights, -1f, 1f);")
        lines.append(f"        }}")
        lines.append(f"        else")
        lines.append(f"        {{")
        lines.append(f"            // 兜底策略：无有效加权时使用触发最强单条规则")
        lines.append(f"            if (bestWeight > 0f)")
        lines.append(f"            {{")
        lines.append(f"                outputVx = Mathf.Clamp(bestVx, -1f, 1f);")
        lines.append(f"                outputOmega = Mathf.Clamp(bestOmega, -1f, 1f);")
        lines.append(f"            }}")
        lines.append(f"            else")
        lines.append(f"            {{")
        lines.append(f"                outputVx = 0f;")
        lines.append(f"                outputOmega = 0f;")
        lines.append(f"            }}")
        lines.append(f"        }}")
        lines.append(f"")
        lines.append(f"        // 低置信度恢复混合：偏离训练分布时，加入解析式恢复控制，降低脱轨风险")
        lines.append(f"        float trackConf = Mathf.Clamp01(0.5f * (frontCenter + rearCenter));")
        lines.append(f"        float lateralErr = 0.55f * frontLRDiff + 0.45f * rearLRDiff;")
        lines.append(f"        float headingErr = frontLRDiff - rearLRDiff;")
        lines.append(f"")
        lines.append(f"        float recoverVx = Mathf.Clamp(-0.85f * lateralErr, -1f, 1f);")
        lines.append(f"        float recoverOmega = Mathf.Clamp(-(1.15f * lateralErr + 0.75f * headingErr), -1f, 1f);")
        lines.append(f"")
        lines.append(f"        float confBlend = Mathf.Clamp01((0.65f - trackConf) / 0.35f);")
        lines.append(f"        float weightBlend = Mathf.Clamp01((0.05f - sumWeights) / 0.05f);")
        lines.append(f"        float blend = Mathf.Max(confBlend, weightBlend);")
        lines.append(f"")
        lines.append(f"        outputVx = Mathf.Lerp(outputVx, recoverVx, blend);")
        lines.append(f"        outputOmega = Mathf.Lerp(outputOmega, recoverOmega, blend);")
        lines.append(f"    }}")
        lines.append(f"")
        
        # ===== 三角形隶属函数 =====
        lines.append(f"    // ===== 三角形隶属函数（含边界肩型处理）=====")
        lines.append(f"    private static float TriMF(float x, float a, float b, float c)")
        lines.append(f"    {{")
        lines.append(f"        // 左肩MF (a≈b): x<=b 时 μ=1")
        lines.append(f"        if (a >= b - 1e-6f)")
        lines.append(f"        {{")
        lines.append(f"            if (x <= b) return 1f;")
        lines.append(f"            if (x >= c) return 0f;")
        lines.append(f"            return (c - x) / Mathf.Max(1e-6f, c - b);")
        lines.append(f"        }}")
        lines.append(f"        // 右肩MF (b≈c): x>=b 时 μ=1")
        lines.append(f"        if (b >= c - 1e-6f)")
        lines.append(f"        {{")
        lines.append(f"            if (x >= b) return 1f;")
        lines.append(f"            if (x <= a) return 0f;")
        lines.append(f"            return (x - a) / Mathf.Max(1e-6f, b - a);")
        lines.append(f"        }}")
        lines.append(f"        // 常规三角形MF")
        lines.append(f"        if (x <= a || x >= c) return 0f;")
        lines.append(f"        if (x <= b) return (x - a) / Mathf.Max(1e-6f, b - a);")
        lines.append(f"        return (c - x) / Mathf.Max(1e-6f, c - b);")
        lines.append(f"    }}")
        lines.append(f"")
        
        # ===== 便捷方法 =====
        lines.append(f"    // ===== 便捷方法 =====")
        lines.append(f"    /// <summary>")
        lines.append(f"    /// 直接从6个传感器原始值计算控制输出")
        lines.append(f"    /// </summary>")
        lines.append(f"    public static void EvaluateFromSensors(")
        lines.append(f"        float sensorFL, float sensorFC, float sensorFR,")
        lines.append(f"        float sensorRL, float sensorRC, float sensorRR,")
        lines.append(f"        float maxField,")
        lines.append(f"        out float outputVx, out float outputOmega)")
        lines.append(f"    {{")
        lines.append(f"        float invMax = 1f / Mathf.Max(1e-6f, maxField);")
        lines.append(f"        float frontLRDiff = Mathf.Clamp((sensorFL - sensorFR) * invMax, -1f, 1f);")
        lines.append(f"        float rearLRDiff = Mathf.Clamp((sensorRL - sensorRR) * invMax, -1f, 1f);")
        lines.append(f"        float frontCenter = Mathf.Clamp01(sensorFC * invMax);")
        lines.append(f"        float rearCenter = Mathf.Clamp01(sensorRC * invMax);")
        lines.append(f"        Evaluate(frontLRDiff, rearLRDiff, frontCenter, rearCenter, out outputVx, out outputOmega);")
        lines.append(f"    }}")
        lines.append(f"")
        lines.append(f"    /// <summary>")
        lines.append(f"    /// 获取规则库信息（用于调试）")
        lines.append(f"    /// </summary>")
        lines.append(f"    public static string GetInfo()")
        lines.append(f"    {{")
        lines.append(f"        return $\"FuzzyController: {{N_RULES}} rules, generated {timestamp}\";")
        lines.append(f"    }}")
        lines.append(f"}}")
        
        # 写入文件
        code = "\n".join(lines)
        
        # 确保目录存在
        out_dir = os.path.dirname(output_path)
        if out_dir and not os.path.exists(out_dir):
            os.makedirs(out_dir)
        
        with open(output_path, 'w', encoding='utf-8') as f:
            f.write(code)
        
        print(f"  [OK] Generated {class_name}.cs (Wang-Mendel / Triangular MF)")
        print(f"     Path: {output_path}")
        print(f"     Rules: {n_rules}")
        print(f"     Lines: {len(lines)}")
    
    # ===== 可视化 =====
    
    def visualize(self, save_path=None):
        """可视化隶属函数和控制曲面"""
        try:
            import matplotlib.pyplot as plt
            import matplotlib
            matplotlib.rcParams['font.sans-serif'] = ['SimHei', 'Microsoft YaHei', 'DejaVu Sans']
            matplotlib.rcParams['axes.unicode_minus'] = False
        except ImportError:
            print("  [WARN] matplotlib not installed, skip visualization")
            return
        
        n_vars = len(self.input_vars)
        fig, axes = plt.subplots(1, n_vars, figsize=(5 * n_vars, 4))
        if n_vars == 1:
            axes = [axes]
        
        for idx, (var_name, fvar) in enumerate(self.input_vars.items()):
            ax = axes[idx]
            lo, hi = fvar.range
            x = np.linspace(lo, hi, 500)
            
            for mf in fvar.mfs:
                y = [mf.evaluate(xi) for xi in x]
                ax.plot(x, y, label=mf.name, linewidth=2)
            
            ax.set_title(f'{var_name}', fontsize=12)
            ax.set_xlabel('值')
            ax.set_ylabel('隶属度 μ')
            ax.legend(fontsize=9)
            ax.set_ylim(-0.05, 1.1)
            ax.grid(True, alpha=0.3)
        
        plt.suptitle('输入变量隶属函数', fontsize=14, fontweight='bold')
        plt.tight_layout()
        
        if save_path:
            mf_path = save_path.replace('.png', '_mf.png') if '.png' in save_path else save_path + '_mf.png'
            plt.savefig(mf_path, dpi=150, bbox_inches='tight')
            print(f"  隶属函数图已保存: {mf_path}")
        
        plt.show()
        
        # ===== 控制曲面（front_lr_diff vs rear_lr_diff → output_omega）=====
        if len(self.rules) > 0:
            fig2, axes2 = plt.subplots(1, 2, figsize=(14, 5))
            
            n_grid = 50
            x1 = np.linspace(-1, 1, n_grid)
            x2 = np.linspace(-1, 1, n_grid)
            X1, X2 = np.meshgrid(x1, x2)
            
            # 固定 front_center=0.8, rear_center=0.8（在轨道上）
            fc_fixed = 0.8
            rc_fixed = 0.8
            
            Z_vx = np.zeros_like(X1)
            Z_omega = np.zeros_like(X1)
            
            for i in range(n_grid):
                for j in range(n_grid):
                    result = self.evaluate([X1[i, j], X2[i, j], fc_fixed, rc_fixed])
                    Z_vx[i, j] = result['output_vx_norm']
                    Z_omega[i, j] = result['output_omega_norm']
            
            for k, (Z, title) in enumerate([(Z_vx, 'output_vx'), (Z_omega, 'output_omega')]):
                ax = axes2[k]
                c = ax.contourf(X1, X2, Z, levels=20, cmap='RdBu_r')
                plt.colorbar(c, ax=ax)
                ax.set_xlabel('front_lr_diff')
                ax.set_ylabel('rear_lr_diff')
                ax.set_title(f'{title}\n(front_center={fc_fixed}, rear_center={rc_fixed})')
            
            plt.suptitle('模糊控制曲面', fontsize=14, fontweight='bold')
            plt.tight_layout()
            
            if save_path:
                surf_path = save_path.replace('.png', '_surface.png') if '.png' in save_path else save_path + '_surface.png'
                plt.savefig(surf_path, dpi=150, bbox_inches='tight')
                print(f"  控制曲面图已保存: {surf_path}")
            
            plt.show()


# ============================================================================
# 主函数
# ============================================================================

def main():
    import argparse
    
    parser = argparse.ArgumentParser(
        description='模糊规则库自动生成工具 - 从PPO训练数据中提取模糊规则',
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="""
示例:
  # 基本用法（Wang-Mendel + 数据驱动MF）
  python generate_fuzzy_rules.py --data training_data.csv --mf-method data-driven

  # ★ 全自动聚类法（推荐，规则数上限50，自动最大化R2）
  python generate_fuzzy_rules.py --data training_data.csv --mf-method clustering

  # 聚类法 + 限制规则数上限
  python generate_fuzzy_rules.py --data training_data.csv --mf-method clustering --max-rules 30 --visualize

  # 传统等间距MF + 调整模糊集数量
  python generate_fuzzy_rules.py --data training_data.csv --mf-method equal --n-diff 7 --n-center 5
        """)
    
    parser.add_argument('--data', type=str, default=DATA_FILE_PATH,
                       help='CSV数据文件路径')
    parser.add_argument('--output', type=str, default=OUTPUT_CS_PATH,
                       help='输出C#文件路径（留空使用默认）')
    parser.add_argument('--mf-method', type=str, default=MF_METHOD,
                       choices=['equal', 'data-driven', 'clustering'],
                       help='隶属函数生成方法: equal(等间距), data-driven(数据驱动), clustering(全自动聚类)')
    parser.add_argument('--ra', type=float, default=0.0,
                       help='Clustering radius (clustering mode only): smaller->more rules. 0=auto-tune (default)')
    parser.add_argument('--max-rules', type=int, default=50,
                       help='Maximum number of rules (clustering mode). Algorithm maximizes rules within this limit for best R2. Default 50')
    parser.add_argument('--n-diff', type=int, default=N_DIFF_SETS,
                       help='偏差变量模糊集数量（建议5或7，仅equal/data-driven模式）')
    parser.add_argument('--n-center', type=int, default=N_CENTER_SETS,
                       help='中心变量模糊集数量（建议3或5，仅equal/data-driven模式）')
    parser.add_argument('--min-spacing-ratio', type=float, default=MIN_SPACING_RATIO,
                       help='data-driven模式MF最小间距比例(0~1)。'
                            '0=无约束(纯数据驱动), 0.5=推荐, 1.0=等间距。默认0.5')
    parser.add_argument('--min-support', type=int, default=MIN_RULE_SUPPORT,
                       help='最小规则支持度（仅Wang-Mendel模式）')
    parser.add_argument('--min-strength', type=float, default=MIN_RULE_STRENGTH,
                       help='最小规则强度（仅Wang-Mendel模式）')
    parser.add_argument('--enable-rule-completion', action='store_true',
                       default=ENABLE_RULE_COMPLETION,
                       help='启用规则覆盖补全（对称镜像+偏差外推），推荐开启')
    parser.add_argument('--completion-max-added', type=int, default=RULE_COMPLETION_MAX_ADDED,
                       help='规则补全最多新增条数（默认220）')
    parser.add_argument('--completion-mirror-scale', type=float, default=RULE_COMPLETION_MIRROR_STRENGTH_SCALE,
                       help='镜像规则强度衰减系数（默认0.85）')
    parser.add_argument('--completion-extrap-scale', type=float, default=RULE_COMPLETION_EXTRAP_STRENGTH_SCALE,
                       help='偏差外推规则强度衰减系数（默认0.70）')
    parser.add_argument('--completion-extrap-gain', type=float, default=RULE_COMPLETION_EXTRAP_GAIN,
                       help='偏差外推输出放大系数（默认0.25）')
    parser.add_argument('--max-samples', type=int, default=None,
                       help='最大采样数（加速处理）')
    parser.add_argument('--visualize', action='store_true', default=ENABLE_VISUALIZE,
                       help='生成可视化图表')
    parser.add_argument('--no-eval', action='store_true', default=False,
                       help='跳过评估步骤（加速）')
    parser.add_argument('--print-rules', action='store_true', default=True,
                       help='打印规则列表')
    
    args = parser.parse_args()
    
    # 检查数据文件
    if not args.data:
        print("错误: 请指定CSV数据文件路径（--data 参数）")
        print("或修改脚本顶部的 DATA_FILE_PATH 变量")
        sys.exit(1)
    
    if not os.path.exists(args.data):
        print(f"错误: 文件不存在: {args.data}")
        sys.exit(1)
    
    # 确定输出路径
    if not args.output:
        data_dir = os.path.dirname(os.path.abspath(args.data))
        args.output = os.path.join(data_dir, "FuzzyController.cs")
    
    print(f"╔{'═'*58}╗")
    print(f"║  模糊规则库自动生成工具                                 ║")
    print(f"║  从PPO训练数据 → 模糊隶属函数 + 规则库 → C#代码        ║")
    print(f"╚{'═'*58}╝")
    print(f"\n配置:")
    print(f"  数据文件: {args.data}")
    print(f"  输出文件: {args.output}")
    print(f"  MF方法: {args.mf_method}")
    
    if args.mf_method == 'clustering':
        if args.ra > 0:
            print(f"  Clustering radius (ra): {args.ra}")
        else:
            print(f"  Clustering radius (ra): AUTO (max {args.max_rules} rules, maximize R2)")
        print(f"  [*] Full-auto mode: MF count/shape/params/rules all data-driven")
    else:
        print(f"  偏差变量模糊集数: {args.n_diff}")
        print(f"  中心变量模糊集数: {args.n_center}")
        if args.mf_method == 'data-driven':
            print(f"  MF最小间距比例: {args.min_spacing_ratio}")
        print(f"  规则覆盖补全: {args.enable_rule_completion}")
        if args.enable_rule_completion:
            print(f"  补全最大新增规则: {args.completion_max_added}")
            print(f"  补全镜像强度衰减: {args.completion_mirror_scale}")
            print(f"  补全外推强度衰减: {args.completion_extrap_scale}")
            print(f"  补全外推输出增益: {args.completion_extrap_gain}")
        print(f"  可能的最大规则数: {args.n_diff**2 * args.n_center**2}")
    
    # 创建生成器
    generator = FuzzyRuleGenerator()
    
    # Step 1: 加载数据
    generator.load_data(args.data, max_samples=args.max_samples)
    
    if args.mf_method == 'clustering':
        # ===== 聚类法流程: MF生成和规则提取一体化 =====
        rules = generator.generate_from_clustering(
            ra=args.ra if args.ra > 0 else None,
            max_rules=args.max_rules
        )
    else:
        # ===== Wang-Mendel流程: MF生成 → 规则提取 =====
        # Step 2: 生成隶属函数
        generator.generate_mfs(
            method=args.mf_method,
            n_diff=args.n_diff,
            n_center=args.n_center,
            min_spacing_ratio=args.min_spacing_ratio
        )
        
        # Step 3: 提取规则
        rules = generator.extract_rules(
            min_support=args.min_support,
            min_strength=args.min_strength,
            enable_rule_completion=args.enable_rule_completion,
            completion_max_added=args.completion_max_added,
            mirror_strength_scale=args.completion_mirror_scale,
            extrap_strength_scale=args.completion_extrap_scale,
            extrap_gain=args.completion_extrap_gain
        )
    
    if args.print_rules:
        generator.print_rules(max_show=60)
    
    # Step 4: 评估
    if not args.no_eval:
        generator.evaluate_dataset()
    
    # Step 5: 导出C#
    generator.export_csharp(args.output)
    
    # 可视化
    if args.visualize:
        vis_path = args.output.replace('.cs', '.png')
        generator.visualize(save_path=vis_path)
    
    print(f"\n{'='*60}")
    print(f"[DONE]")
    print(f"{'='*60}")
    print(f"\n下一步:")
    print(f"  1. 将 {os.path.basename(args.output)} 复制到 Unity 项目的 Scripts 文件夹")
    print(f"  2. 在测试脚本中调用:")
    print(f"     FuzzyController.Evaluate(frontLRDiff, rearLRDiff, frontCenter, rearCenter,")
    print(f"                              out float vx, out float omega);")
    print(f"  3. 将 vx * maxLateralSpeed, omega * maxOmegaRad 作为控制量输出")
    
    if args.mf_method == 'clustering':
        print(f"\n  [*] Clustering info:")
        print(f"     Generated {len(rules)} rules (limit={args.max_rules})")
        print(f"     To allow more rules (higher R2): increase --max-rules")
        print(f"     To reduce rules: decrease --max-rules")


if __name__ == '__main__':
    main()
