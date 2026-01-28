#!/usr/bin/env python3
"""
从CSV数据生成Unity可用的C# if-else规则库（决策树蒸馏）

示例：
  python generate_rules.py --data "D:/Data/episode_data.csv" --out "DecisionTreeRules.cs"
  python generate_rules.py --data data.csv --depth 18 --min-leaf 3 --min-split 6 --optimize-w
  python generate_rules.py --data data.csv --auto-optimize --target-r2 0.92 --max-code-lines 40000
"""

import argparse
import os
from datetime import datetime
import pandas as pd
import numpy as np
from sklearn.tree import DecisionTreeRegressor
from sklearn.metrics import r2_score, mean_absolute_error, mean_squared_error

# ============================================================================
# 全局配置参数（可直接在此修改，无需命令行参数）
# ============================================================================

# ========== 数据文件配置 ==========
# CSV数据文件路径（必需，如果未通过命令行参数指定）
DATA_FILE_PATH = "D:/XiaoYiFei/Project/Unity/XYF_Car_Test/Data_Record/training_data_20260124_193252.csv"  # 例如: "D:/XiaoYiFei/Project/Unity/XYF_Car_Test/Data_Record/training_data.csv"

# ========== 输出文件配置 ==========
# 输出C#文件路径或目录（如果未通过命令行参数指定）
# 如果指定为目录，会在该目录下创建带日期的文件
# 如果指定为文件路径，会在文件名中插入日期
OUTPUT_FILE_PATH = "D:/XiaoYiFei/Project/Unity/XYF_Car_Test/OutputRules"  # 例如: "DecisionTreeRules.cs" 或 "D:/Output/"

# C#类名
CLASS_NAME = "DecisionTreeRules"  # 例如: "DecisionTreeRules"

# 是否在文件名中自动添加日期时间戳（True=添加，False=不添加）
# 如果为True，文件名格式：DecisionTreeRules_20260125.cs
# 如果为False，使用原始文件名（可能覆盖旧文件）
AUTO_ADD_DATE_TO_FILENAME = True

# ========== 自动优化配置（推荐使用） ==========
# 是否启用自动参数优化（True=自动寻找最优参数，False=使用手动参数）
USE_AUTO_OPTIMIZE = True

# 目标R²分数（自动优化模式下，会尽量达到此R²值）
# 建议范围：0.90-0.95（越高精度越好，但代码量可能越大）
TARGET_R2 = 0.80

# 最大代码行数限制（超过此值会警告，建议20000-500000）
# 注意：代码行数过多可能导致Unity编译或运行问题
MAX_CODE_LINES = 300000

# 自动优化最大迭代次数（建议50-200，越大搜索越充分但耗时越长）
MAX_ITERATIONS = 50

# 是否自动扩大搜索范围（True=如果未找到满足条件的参数，自动扩大搜索范围）
AUTO_EXPAND = True

# 自动优化进度打印间隔（迭代数，0=不打印进度）
PROGRESS_INTERVAL = 20

# ========== 手动参数配置（仅在 USE_AUTO_OPTIMIZE=False 时生效） ==========
# 决策树最大深度（建议12-25，越大越复杂，代码量越大）
MANUAL_DEPTH = 12

# 叶子节点最小样本数（建议3-5，越大代码越小）
MANUAL_MIN_LEAF = 5

# 分裂节点最小样本数（建议6-10，越大代码越小）
MANUAL_MIN_SPLIT = 10

# 是否针对action_w进行优化（True=增加action_w树的深度，改善弯道跟踪）
OPTIMIZE_W = False

# action_w树的深度增量（仅在OPTIMIZE_W=True时生效，建议2-4）
W_DEPTH_BOOST = 2

# ============================================================================
# 配置说明：
# 1. 直接修改上面的全局变量即可，无需使用命令行参数
# 2. 命令行参数仍然可用，会覆盖全局变量设置
# 3. 推荐配置：
#    - USE_AUTO_OPTIMIZE = True（自动优化）
#    - TARGET_R2 = 0.95（目标精度）
#    - MAX_CODE_LINES = 50000（代码行数限制）
#    - DATA_FILE_PATH = "你的CSV文件路径"
# ============================================================================

FEATURE_NAMES = [
    "sensor0", "sensor1", "sensor2", "sensor3", "sensor4", "sensor5",
    "smoothed_vx", "smoothed_omega",
    "raw_action_x", "raw_action_w",
    "actual_vz", "actual_vx", "actual_omega",
]
TARGET_NAMES = ["action_x", "action_w"]


def _add_date_to_filename(file_path):
    """
    在文件名中插入日期时间戳（年月日格式：yyyyMMdd）
    例如: "DecisionTreeRules.cs" -> "DecisionTreeRules_20260125.cs"
          "D:/Output/file.cs" -> "D:/Output/file_20260125.cs"
          "D:/Output/" -> "D:/Output/DecisionTreeRules_20260125.cs"
    """
    date_str = datetime.now().strftime("%Y%m%d")
    
    # 如果路径是目录，在目录下创建默认文件名
    if os.path.isdir(file_path):
        base_name = "DecisionTreeRules"
        ext = ".cs"
        return os.path.join(file_path, f"{base_name}_{date_str}{ext}")
    
    # 分离目录、文件名和扩展名
    dir_path = os.path.dirname(file_path)
    filename = os.path.basename(file_path)
    
    # 分离文件名和扩展名
    if '.' in filename:
        name_part, ext = os.path.splitext(filename)
        # 如果文件名已经包含日期格式（8位数字），替换它；否则添加日期
        import re
        if re.search(r'_\d{8}$', name_part):
            # 替换现有的日期
            name_part = re.sub(r'_\d{8}$', '', name_part)
        new_filename = f"{name_part}_{date_str}{ext}"
    else:
        # 没有扩展名，直接添加日期和.cs扩展名
        new_filename = f"{filename}_{date_str}.cs"
    
    return os.path.join(dir_path, new_filename) if dir_path else new_filename


def _is_number_string(text):
    try:
        float(text)
        return True
    except Exception:
        return False


def _load_csv(csv_path):
    df = pd.read_csv(csv_path)
    has_expected_header = all(col in df.columns for col in FEATURE_NAMES + TARGET_NAMES)
    if not has_expected_header:
        # 可能是无表头的CSV，或表头不是预期格式
        if all(_is_number_string(str(c)) for c in df.columns):
            df = pd.read_csv(csv_path, header=None)
    return df


def _prepare_data(df):
    if all(col in df.columns for col in FEATURE_NAMES + TARGET_NAMES):
        data = df[FEATURE_NAMES + TARGET_NAMES]
    else:
        if df.shape[1] < 15:
            raise ValueError(f"CSV列数不足15列，当前为 {df.shape[1]} 列")
        data = df.iloc[:, :15]
        data.columns = FEATURE_NAMES + TARGET_NAMES

    data = data.apply(pd.to_numeric, errors="coerce")
    before = len(data)
    data = data.dropna()
    dropped = before - len(data)
    if dropped > 0:
        print(f"⚠️  发现 {dropped} 行非数值或缺失数据，已自动丢弃")

    X = data[FEATURE_NAMES].values.astype(np.float32)
    y_x = data["action_x"].values.astype(np.float32)
    y_w = data["action_w"].values.astype(np.float32)
    return X, y_x, y_w, len(data)


def _tree_to_csharp(tree, feature_names, base_indent=2):
    def recurse(node, indent_level):
        indent = "    " * (base_indent + indent_level)
        if tree.feature[node] == -2:
            value = float(tree.value[node][0][0])
            return f"{indent}return {value:.6f}f;"

        feature_idx = int(tree.feature[node])
        feature_name = feature_names[feature_idx]
        threshold = float(tree.threshold[node])

        left_code = recurse(tree.children_left[node], indent_level + 1)
        right_code = recurse(tree.children_right[node], indent_level + 1)

        return (
            f"{indent}if ({feature_name} <= {threshold:.6f}f)\n"
            f"{left_code}\n"
            f"{indent}else\n"
            f"{right_code}"
        )

    return recurse(0, 0)


def _estimate_code_lines(tree):
    return tree.tree_.node_count * 4


def _search_best_params(
    X,
    y,
    target_r2,
    max_code_lines,
    max_iterations,
    name,
    auto_expand,
    progress_interval,
):
    best_hit_params = None
    best_hit_code = float("inf")
    best_hit_r2 = -1.0
    best_any_params = None
    best_any_r2 = -1.0

    def try_update(depth, min_leaf, min_split):
        nonlocal best_hit_params, best_hit_code, best_hit_r2, best_any_params, best_any_r2
        tree = DecisionTreeRegressor(
            max_depth=depth,
            min_samples_leaf=min_leaf,
            min_samples_split=min_split,
            random_state=42,
        )
        tree.fit(X, y)
        r2 = tree.score(X, y)
        code_lines = _estimate_code_lines(tree)

        # 优先满足R²目标，其次尽量减少代码行数
        if r2 >= target_r2:
            if code_lines < best_hit_code:
                best_hit_r2 = r2
                best_hit_code = code_lines
                best_hit_params = {
                    "max_depth": depth,
                    "min_samples_leaf": min_leaf,
                    "min_samples_split": min_split,
                }
        else:
            if r2 > best_any_r2:
                best_any_r2 = r2
                best_any_params = {
                    "max_depth": depth,
                    "min_samples_leaf": min_leaf,
                    "min_samples_split": min_split,
                }
        return r2 >= target_r2

    print(f"\n[{name}] 自动优化参数中 (目标R²>={target_r2:.2f}，代码行数仅作为次要参考)")
    iterations = 0
    depth_base = [8, 12, 16, 20, 24, 28, 32]
    min_leaf_base = [5, 4, 3, 2]
    min_split_base = [10, 8, 6, 4]

    expand_round = 0
    while iterations < max_iterations:
        print(f"[{name}] 扩展轮次 {expand_round}，当前搜索范围扩展中...")
        depth_range = [d + expand_round * 6 for d in depth_base]
        min_leaf_range = [max(2, v - expand_round) for v in min_leaf_base]
        min_split_range = [max(2, v - expand_round * 2) for v in min_split_base]

        for depth in depth_range:
            for min_leaf in min_leaf_range:
                for min_split in min_split_range:
                    if iterations >= max_iterations:
                        break
                    try_update(depth, min_leaf, min_split)
                    iterations += 1
                    if progress_interval > 0 and iterations % progress_interval == 0:
                        hit_r2 = f"{best_hit_r2:.4f}" if best_hit_params else "N/A"
                        hit_code = f"{best_hit_code}" if best_hit_params else "N/A"
                        any_r2 = f"{best_any_r2:.4f}" if best_any_params else "N/A"
                        print(
                            f"[{name}] 进度 {iterations}/{max_iterations} | "
                            f"best_hit_r2={hit_r2}, best_hit_lines={hit_code}, best_any_r2={any_r2}"
                        )
                if iterations >= max_iterations:
                    break
            if iterations >= max_iterations:
                break

        if not auto_expand:
            break
        expand_round += 1

    if best_hit_params:
        if best_hit_code > max_code_lines:
            print(f"⚠️  {name} 已达到目标R²，但代码行数约 {best_hit_code} 超过限制 {max_code_lines}")
        return best_hit_params, best_hit_r2

    if best_any_params:
        print(f"⚠️  {name} 在当前搜索范围内未达到目标R²，已返回最优结果")
        return best_any_params, best_any_r2

    return None, -1.0


def generate_rules(
    data_path,
    output_path,
    class_name="DecisionTreeRules",
    max_depth=12,
    min_leaf=5,
    min_split=10,
    optimize_w=False,
    w_depth_boost=2,
    max_code_lines=50000,
    auto_optimize=False,
    target_r2=0.95,
    max_iterations=50,
    auto_expand=True,
    progress_interval=20,
):
    df = _load_csv(data_path)
    X, y_x, y_w, sample_count = _prepare_data(df)

    if auto_optimize:
        per_tree_limit = max_code_lines // 2
        params_x, r2_x = _search_best_params(
            X, y_x, target_r2=target_r2, max_code_lines=per_tree_limit, max_iterations=max_iterations,
            name="action_x", auto_expand=auto_expand, progress_interval=progress_interval
        )
        params_w, r2_w = _search_best_params(
            X, y_w, target_r2=target_r2, max_code_lines=per_tree_limit, max_iterations=max_iterations,
            name="action_w", auto_expand=auto_expand, progress_interval=progress_interval
        )

        if not params_x or not params_w:
            raise RuntimeError("自动优化失败：未找到可用参数组合")

        depth_x = params_x["max_depth"]
        min_leaf = params_x["min_samples_leaf"]
        min_split = params_x["min_samples_split"]

        depth_w = params_w["max_depth"]
        min_leaf_w = params_w["min_samples_leaf"]
        min_split_w = params_w["min_samples_split"]
    else:
        depth_x = max_depth
        depth_w = max_depth + (w_depth_boost if optimize_w else 0)
        min_leaf_w = max(min_leaf - 1, 2) if optimize_w else min_leaf
        min_split_w = max(min_split - 2, 2) if optimize_w else min_split

    tree_x = DecisionTreeRegressor(
        max_depth=depth_x,
        min_samples_leaf=min_leaf,
        min_samples_split=min_split,
        random_state=42,
    )
    tree_w = DecisionTreeRegressor(
        max_depth=depth_w,
        min_samples_leaf=min_leaf_w,
        min_samples_split=min_split_w,
        random_state=42,
    )

    tree_x.fit(X, y_x)
    tree_w.fit(X, y_w)

    pred_x = tree_x.predict(X)
    pred_w = tree_w.predict(X)
    r2_x = r2_score(y_x, pred_x)
    r2_w = r2_score(y_w, pred_w)
    mae_x = mean_absolute_error(y_x, pred_x)
    mae_w = mean_absolute_error(y_w, pred_w)
    mse_x = mean_squared_error(y_x, pred_x)
    mse_w = mean_squared_error(y_w, pred_w)

    estimated_lines = _estimate_code_lines(tree_x) + _estimate_code_lines(tree_w) + 50
    if estimated_lines > max_code_lines:
        print(
            f"⚠️  预计生成代码约 {estimated_lines} 行，超过限制 {max_code_lines} 行，"
            "可能导致Unity编译或运行问题"
        )

    code_x = _tree_to_csharp(tree_x.tree_, FEATURE_NAMES, base_indent=2)
    code_w = _tree_to_csharp(tree_w.tree_, FEATURE_NAMES, base_indent=2)

    code = f"""// 决策树规则库 - 自动生成
// 数据样本数: {sample_count}
// R² Score - action_x: {r2_x:.4f}, action_w: {r2_w:.4f}
// MAE - action_x: {mae_x:.6f}, action_w: {mae_w:.6f}
// MSE - action_x: {mse_x:.6f}, action_w: {mse_w:.6f}
// 树深度: action_x={depth_x}, action_w={depth_w}
// 特征顺序: {", ".join(FEATURE_NAMES)}

using System;

public class {class_name}
{{
    // 特征变量（与MyCar_Agent.CollectObservations顺序一致）
    private float sensor0, sensor1, sensor2, sensor3, sensor4, sensor5;
    private float smoothed_vx, smoothed_omega;
    private float raw_action_x, raw_action_w;
    private float actual_vz, actual_vx, actual_omega;

    public void SetObservations(float[] obs)
    {{
        if (obs == null || obs.Length < 13)
        {{
            throw new ArgumentException("观测数组长度不足13");
        }}
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

    public float PredictActionX()
    {{
{code_x}
    }}

    public float PredictActionW()
    {{
{code_w}
    }}

    public (float action_x, float action_w) Predict()
    {{
        return (PredictActionX(), PredictActionW());
    }}
    }}
"""

    # 确保输出目录存在
    output_dir = os.path.dirname(output_path)
    if output_dir and not os.path.exists(output_dir):
        try:
            os.makedirs(output_dir, exist_ok=True)
        except OSError as e:
            raise RuntimeError(f"无法创建输出目录: {output_dir}\n错误: {e}")

    # 检查文件是否已存在且可能被占用
    if os.path.exists(output_path):
        # 检查文件是否为只读
        if not os.access(output_path, os.W_OK):
            raise RuntimeError(
                f"文件为只读或没有写入权限: {output_path}\n"
                f"解决方案:\n"
                f"  1. 检查文件是否被其他程序打开（如Unity编辑器、IDE等），请先关闭\n"
                f"  2. 检查文件属性，取消只读属性\n"
                f"  3. 以管理员权限运行脚本"
            )
    
    try:
        with open(output_path, "w", encoding="utf-8") as f:
            f.write(code)
    except PermissionError as e:
        raise RuntimeError(
            f"权限被拒绝，无法写入文件: {output_path}\n"
            f"错误详情: {e}\n"
            f"\n可能的解决方案:\n"
            f"  1. 文件可能正在被其他程序使用（Unity编辑器、IDE、文件浏览器等）\n"
            f"     → 请关闭所有可能占用该文件的程序\n"
            f"  2. 文件可能被设置为只读\n"
            f"     → 右键文件 → 属性 → 取消'只读'选项\n"
            f"  3. 没有足够的权限\n"
            f"     → 以管理员权限运行命令行/Python\n"
            f"  4. 尝试使用不同的输出路径\n"
            f"     → 使用 --out 参数指定其他路径"
        )
    except IOError as e:
        raise RuntimeError(f"无法写入文件: {output_path}\n错误: {e}")

    print(f"✓ 规则库已生成: {output_path}")
    print(f"  - action_x R²: {r2_x:.4f}, action_w R²: {r2_w:.4f}")
    print(f"  - 估算代码行数: ~{estimated_lines}")


def main():
    parser = argparse.ArgumentParser(description="CSV -> C# if-else规则库 生成器")
    parser.add_argument("--data", type=str, default=None, help="CSV数据文件路径（会覆盖全局变量）")
    parser.add_argument("--out", type=str, default=None, help="输出C#文件路径（会覆盖全局变量）")
    parser.add_argument("--class-name", type=str, default=None, help="C#类名（会覆盖全局变量）")
    parser.add_argument("--depth", type=int, default=None, help="决策树最大深度（会覆盖全局变量）")
    parser.add_argument("--min-leaf", type=int, default=None, help="叶子节点最小样本数（会覆盖全局变量）")
    parser.add_argument("--min-split", type=int, default=None, help="分裂节点最小样本数（会覆盖全局变量）")
    parser.add_argument("--optimize-w", action="store_true", help="针对action_w进行优化（会覆盖全局变量）")
    parser.add_argument("--w-depth-boost", type=int, default=None, help="action_w深度增加量（会覆盖全局变量）")
    parser.add_argument("--max-code-lines", type=int, default=None, help="最大代码行数警戒值（会覆盖全局变量）")
    parser.add_argument("--auto-optimize", action="store_true", help="自动优化参数以达到目标R²（会覆盖全局变量）")
    parser.add_argument("--target-r2", type=float, default=None, help="自动优化目标R²（会覆盖全局变量）")
    parser.add_argument("--max-iterations", type=int, default=None, help="自动优化最大迭代次数（会覆盖全局变量）")
    parser.add_argument("--no-auto-expand", action="store_true", help="关闭自动扩大搜索范围（会覆盖全局变量）")
    parser.add_argument("--progress-interval", type=int, default=None, help="自动优化进度打印间隔(迭代数)（会覆盖全局变量）")

    args = parser.parse_args()

    # ========== 使用全局变量，命令行参数作为覆盖 ==========
    # 数据文件路径
    data_path = args.data if args.data is not None else DATA_FILE_PATH
    if not data_path:
        parser.error("必须指定数据文件路径（通过 --data 参数或设置 DATA_FILE_PATH 全局变量）")
    
    # 输出文件路径（自动添加日期时间戳）
    output_path = args.out if args.out is not None else OUTPUT_FILE_PATH
    if not os.path.isabs(output_path):
        output_path = os.path.abspath(output_path)
    
    # 如果启用了自动添加日期，在文件名中插入日期
    if AUTO_ADD_DATE_TO_FILENAME:
        output_path = _add_date_to_filename(output_path)
    
    # C#类名
    class_name = args.class_name if args.class_name is not None else CLASS_NAME
    
    # 自动优化参数
    auto_optimize = args.auto_optimize if args.auto_optimize else USE_AUTO_OPTIMIZE
    target_r2 = args.target_r2 if args.target_r2 is not None else TARGET_R2
    max_code_lines = args.max_code_lines if args.max_code_lines is not None else MAX_CODE_LINES
    max_iterations = args.max_iterations if args.max_iterations is not None else MAX_ITERATIONS
    auto_expand = not args.no_auto_expand if args.no_auto_expand else AUTO_EXPAND
    progress_interval = args.progress_interval if args.progress_interval is not None else PROGRESS_INTERVAL
    
    # 手动参数（仅在非自动优化模式下使用）
    depth = args.depth if args.depth is not None else MANUAL_DEPTH
    min_leaf = args.min_leaf if args.min_leaf is not None else MANUAL_MIN_LEAF
    min_split = args.min_split if args.min_split is not None else MANUAL_MIN_SPLIT
    optimize_w = args.optimize_w if args.optimize_w else OPTIMIZE_W
    w_depth_boost = args.w_depth_boost if args.w_depth_boost is not None else W_DEPTH_BOOST

    generate_rules(
        data_path=data_path,
        output_path=output_path,
        class_name=class_name,
        max_depth=depth,
        min_leaf=min_leaf,
        min_split=min_split,
        optimize_w=optimize_w,
        w_depth_boost=w_depth_boost,
        max_code_lines=max_code_lines,
        auto_optimize=auto_optimize,
        target_r2=target_r2,
        max_iterations=max_iterations,
        auto_expand=auto_expand,
        progress_interval=progress_interval,
    )


if __name__ == "__main__":
    main()
