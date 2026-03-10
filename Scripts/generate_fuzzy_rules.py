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

# 最小规则支持度（低于此值的规则被剪枝，即匹配该规则的数据点数量下限）
MIN_RULE_SUPPORT = 3

# 最小规则强度（低于此值的规则被剪枝，即规则触发强度下限）
MIN_RULE_STRENGTH = 0.01

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
        if x <= self.a or x >= self.c:
            return 0.0
        elif x <= self.b:
            return (x - self.a) / max(1e-10, self.b - self.a)
        else:
            return (self.c - x) / max(1e-10, self.c - self.b)
    
    def __repr__(self):
        return f"TriMF('{self.name}', a={self.a:.4f}, b={self.b:.4f}, c={self.c:.4f})"


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
            print(f"\n  ❌ 错误: CSV只有 {df.shape[1]} 列，但需要至少 {max_col+1} 列")
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
    
    def generate_mfs(self, method="equal", n_diff=5, n_center=3):
        """
        自动生成隶属函数
        
        参数:
          method: "equal" (等间距) 或 "data-driven" (数据驱动)
          n_diff: 偏差变量的模糊集数量
          n_center: 中心变量的模糊集数量
        """
        print(f"\n{'='*60}")
        print(f"[2/5] 生成隶属函数 (方法: {method})")
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
                fvar = self._generate_data_driven_mfs(var_name, var_range, data_col, n, set_names)
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
    
    def _generate_data_driven_mfs(self, var_name, var_range, data_col, n_sets, set_names):
        """
        数据驱动的隶属函数生成
        
        原理：
          1. 使用数据的百分位数确定MF中心（而非等间距）
          2. 数据密集区域的MF更窄（分辨率更高）
          3. 数据稀疏区域的MF更宽（覆盖更广）
        
        这样生成的MF更好地匹配实际数据分布，
        在数据密集区域（如车身居中时 front_lr_diff≈0）有更精细的模糊划分
        """
        fvar = FuzzyVariable(var_name, var_range)
        lo, hi = var_range
        
        # 使用等间距的百分位数作为MF中心
        percentiles = np.linspace(0, 100, n_sets + 2)[1:-1]  # 排除0%和100%
        centers = np.percentile(data_col, percentiles)
        
        # 确保中心点单调递增且在范围内
        centers = np.clip(centers, lo, hi)
        centers = np.sort(np.unique(np.round(centers, 6)))
        
        # 如果去重后数量不够，补充等间距点
        if len(centers) < n_sets:
            equal_centers = np.linspace(lo, hi, n_sets)
            centers = np.sort(np.unique(np.concatenate([centers, equal_centers])))[:n_sets]
        elif len(centers) > n_sets:
            # 从中均匀选取
            idx = np.linspace(0, len(centers) - 1, n_sets).astype(int)
            centers = centers[idx]
        
        # 确保首尾与范围边界对齐
        centers[0] = lo
        centers[-1] = hi
        
        # 生成三角形MF（相邻中心之间重叠）
        for i in range(n_sets):
            c = centers[i]
            if i == 0:
                a = lo
                b = lo
                r = centers[1] if n_sets > 1 else hi
            elif i == n_sets - 1:
                a = centers[i - 1]
                b = hi
                r = hi
            else:
                a = centers[i - 1]
                b = c
                r = centers[i + 1]
            
            fvar.add_mf(TriangularMF(set_names[i], a, b, r))
        
        return fvar
    
    # ===== Wang-Mendel 规则提取 =====
    
    def extract_rules(self, min_support=3, min_strength=0.01):
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
        
        return self.rules
    
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
            print(f"      R²   = {r2:.4f}")
            
            if r2 < 0.5:
                print(f"      ⚠️ R²偏低，建议增加模糊集数量或使用 data-driven 方法")
        
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
        
        生成的代码包含：
          - 所有隶属函数参数（三角形MF的a,b,c）
          - 完整规则库（前件索引 + 后件值 + 权重）
          - 模糊推理引擎（模糊化 → 规则评估 → 去模糊化）
          - 公开的Evaluate()接口
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
        lines.append(f"            // 无规则匹配时，输出零（保持当前状态）")
        lines.append(f"            outputVx = 0f;")
        lines.append(f"            outputOmega = 0f;")
        lines.append(f"        }}")
        lines.append(f"    }}")
        lines.append(f"")
        
        # ===== 三角形隶属函数 =====
        lines.append(f"    // ===== 三角形隶属函数 =====")
        lines.append(f"    private static float TriMF(float x, float a, float b, float c)")
        lines.append(f"    {{")
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
        
        print(f"  ✅ 已生成 {class_name}.cs")
        print(f"     文件路径: {output_path}")
        print(f"     规则数: {n_rules}")
        print(f"     代码行数: {len(lines)}")
    
    # ===== 可视化 =====
    
    def visualize(self, save_path=None):
        """可视化隶属函数和控制曲面"""
        try:
            import matplotlib.pyplot as plt
            import matplotlib
            matplotlib.rcParams['font.sans-serif'] = ['SimHei', 'Microsoft YaHei', 'DejaVu Sans']
            matplotlib.rcParams['axes.unicode_minus'] = False
        except ImportError:
            print("  ⚠️ matplotlib 未安装，跳过可视化")
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
  # 基本用法
  python generate_fuzzy_rules.py --data training_data.csv

  # 数据驱动MF + 可视化
  python generate_fuzzy_rules.py --data training_data.csv --mf-method data-driven --visualize

  # 调整模糊集数量
  python generate_fuzzy_rules.py --data training_data.csv --n-diff 7 --n-center 5

  # 指定输出文件
  python generate_fuzzy_rules.py --data training_data.csv --output FuzzyController.cs
        """)
    
    parser.add_argument('--data', type=str, default=DATA_FILE_PATH,
                       help='CSV数据文件路径')
    parser.add_argument('--output', type=str, default=OUTPUT_CS_PATH,
                       help='输出C#文件路径（留空使用默认）')
    parser.add_argument('--mf-method', type=str, default=MF_METHOD,
                       choices=['equal', 'data-driven'],
                       help='隶属函数生成方法')
    parser.add_argument('--n-diff', type=int, default=N_DIFF_SETS,
                       help='偏差变量模糊集数量（建议5或7）')
    parser.add_argument('--n-center', type=int, default=N_CENTER_SETS,
                       help='中心变量模糊集数量（建议3或5）')
    parser.add_argument('--min-support', type=int, default=MIN_RULE_SUPPORT,
                       help='最小规则支持度')
    parser.add_argument('--min-strength', type=float, default=MIN_RULE_STRENGTH,
                       help='最小规则强度')
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
    print(f"  偏差变量模糊集数: {args.n_diff}")
    print(f"  中心变量模糊集数: {args.n_center}")
    print(f"  可能的最大规则数: {args.n_diff**2 * args.n_center**2}")
    
    # 创建生成器
    generator = FuzzyRuleGenerator()
    
    # Step 1: 加载数据
    generator.load_data(args.data, max_samples=args.max_samples)
    
    # Step 2: 生成隶属函数
    generator.generate_mfs(
        method=args.mf_method,
        n_diff=args.n_diff,
        n_center=args.n_center
    )
    
    # Step 3: 提取规则
    rules = generator.extract_rules(
        min_support=args.min_support,
        min_strength=args.min_strength
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
    print(f"✅ 完成！")
    print(f"{'='*60}")
    print(f"\n下一步:")
    print(f"  1. 将 {os.path.basename(args.output)} 复制到 Unity 项目的 Scripts 文件夹")
    print(f"  2. 在测试脚本中调用:")
    print(f"     FuzzyController.Evaluate(frontLRDiff, rearLRDiff, frontCenter, rearCenter,")
    print(f"                              out float vx, out float omega);")
    print(f"  3. 将 vx * maxLateralSpeed, omega * maxOmegaRad 作为控制量输出")


if __name__ == '__main__':
    main()
