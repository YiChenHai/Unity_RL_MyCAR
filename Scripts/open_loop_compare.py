# -*- coding: utf-8 -*-
"""
开环输出逼近：把 PPO 采集 CSV 的四维磁特征送入 FuzzyController，
与 CSV 中的 output_vx_norm、output_omega_norm 对比。

用法:
  python open_loop_compare.py --data a.csv b.csv c.csv
  python open_loop_compare.py --data <放有这些csv的文件夹>

CSV 需含列:
  front_lr_diff, rear_lr_diff, front_center, rear_center,
  output_vx_norm, output_omega_norm

主指标使用规则加权平均（不含低置信度恢复），对应“规则库逼近教师输出”。
同时给出含恢复混合的部署输出，便于和车上实际 FuzzyController.Evaluate 对照。
"""

import argparse
import os
import re
import sys

import numpy as np
import pandas as pd

INPUT_COLS = ["front_lr_diff", "rear_lr_diff", "front_center", "rear_center"]
TEACHER_COLS = ["output_vx_norm", "output_omega_norm"]


def _extract_block(text, name):
    match = re.search(rf"{name}\s*=\s*\{{(.*?)\}};", text, re.S)
    if not match:
        raise ValueError(f"在 FuzzyController.cs 中找不到 {name}")
    block = re.sub(r"//.*", "", match.group(1))
    block = block.replace("f", "")
    nums = [float(x) for x in re.findall(r"[-+]?(?:\d+\.\d+|\d+)", block)]
    return np.asarray(nums, dtype=np.float64)


def load_fuzzy(cs_path):
    with open(cs_path, "r", encoding="utf-8-sig") as f:
        text = f.read()
    mf = {
        "front": _extract_block(text, "MF_FrontLR").reshape(-1, 3),
        "rear": _extract_block(text, "MF_RearLR").reshape(-1, 3),
        "fc": _extract_block(text, "MF_FrontCenter").reshape(-1, 3),
        "rc": _extract_block(text, "MF_RearCenter").reshape(-1, 3),
    }
    rules = _extract_block(text, "Rules").reshape(-1, 7)
    return mf, rules


def tri_mf(x, a, b, c):
    """与 FuzzyController.TriMF 一致。x 为一维数组。"""
    x = np.asarray(x, dtype=np.float64)
    out = np.zeros_like(x)
    if a >= b - 1e-6:
        out = np.where(x <= b, 1.0, np.where(x >= c, 0.0, (c - x) / max(1e-6, c - b)))
        return out
    if b >= c - 1e-6:
        out = np.where(x >= b, 1.0, np.where(x <= a, 0.0, (x - a) / max(1e-6, b - a)))
        return out
    left = (x - a) / max(1e-6, b - a)
    right = (c - x) / max(1e-6, c - b)
    out = np.where(x <= b, left, right)
    out = np.where((x <= a) | (x >= c), 0.0, out)
    return out


def evaluate_batch(front, rear, fc, rc, mf, rules, with_recovery):
    """批量推理。返回 (vx, omega)，均为归一化 [-1, 1]。"""
    n = len(front)
    sum_vx = np.zeros(n)
    sum_w = np.zeros(n)
    sum_omega = np.zeros(n)
    best_w = np.full(n, -1.0)
    best_vx = np.zeros(n)
    best_omega = np.zeros(n)

    for rule in rules:
        i0, i1, i2, i3 = (int(rule[0]), int(rule[1]), int(rule[2]), int(rule[3]))
        mu1 = tri_mf(front, *mf["front"][i0])
        mu2 = tri_mf(rear, *mf["rear"][i1])
        mu3 = tri_mf(fc, *mf["fc"][i2])
        mu4 = tri_mf(rc, *mf["rc"][i3])
        firing = mu1 * mu2 * mu3 * mu4
        active = firing > 1e-6
        w = firing * rule[6]
        sum_vx += np.where(active, w * rule[4], 0.0)
        sum_omega += np.where(active, w * rule[5], 0.0)
        sum_w += np.where(active, w, 0.0)
        better = active & (w > best_w)
        best_w = np.where(better, w, best_w)
        best_vx = np.where(better, rule[4], best_vx)
        best_omega = np.where(better, rule[5], best_omega)

    has_weight = sum_w > 1e-6
    vx = np.where(has_weight, sum_vx / np.maximum(sum_w, 1e-12), 0.0)
    omega = np.where(has_weight, sum_omega / np.maximum(sum_w, 1e-12), 0.0)
    use_best = (~has_weight) & (best_w > 0.0)
    vx = np.where(use_best, best_vx, vx)
    omega = np.where(use_best, best_omega, omega)
    vx = np.clip(vx, -1.0, 1.0)
    omega = np.clip(omega, -1.0, 1.0)

    if with_recovery:
        track_conf = np.clip(0.5 * (fc + rc), 0.0, 1.0)
        lateral_err = 0.55 * front + 0.45 * rear
        heading_err = front - rear
        recover_vx = np.clip(-0.85 * lateral_err, -1.0, 1.0)
        recover_omega = np.clip(-(1.15 * lateral_err + 0.75 * heading_err), -1.0, 1.0)
        conf_blend = np.clip((0.65 - track_conf) / 0.35, 0.0, 1.0)
        weight_blend = np.clip((0.05 - sum_w) / 0.05, 0.0, 1.0)
        blend = np.maximum(conf_blend, weight_blend)
        vx = (1.0 - blend) * vx + blend * recover_vx
        omega = (1.0 - blend) * omega + blend * recover_omega
        vx = np.clip(vx, -1.0, 1.0)
        omega = np.clip(omega, -1.0, 1.0)
    return vx, omega


def metrics(teacher, pred):
    err = pred - teacher
    mae = float(np.mean(np.abs(err)))
    rmse = float(np.sqrt(np.mean(err ** 2)))
    max_abs = float(np.max(np.abs(err)))
    p95 = float(np.quantile(np.abs(err), 0.95))
    return mae, rmse, p95, max_abs


def collect_csvs(paths):
    files = []
    for path in paths:
        if os.path.isfile(path):
            files.append(path)
        elif os.path.isdir(path):
            for name in sorted(os.listdir(path)):
                if name.lower().endswith(".csv"):
                    files.append(os.path.join(path, name))
        else:
            print(f"找不到路径，已跳过: {path}")
    return files


def load_teacher_csv(path):
    df = pd.read_csv(path)
    missing = [c for c in INPUT_COLS + TEACHER_COLS if c not in df.columns]
    if missing:
        return None
    use = df[INPUT_COLS + TEACHER_COLS].apply(pd.to_numeric, errors="coerce").dropna()
    return use


def setup_font():
    import matplotlib
    matplotlib.rcParams["font.sans-serif"] = ["Microsoft YaHei", "SimHei", "DejaVu Sans"]
    matplotlib.rcParams["axes.unicode_minus"] = False


def plot_scatter(teacher_vx, fuzzy_vx, teacher_w, fuzzy_w, out_path, title):
    import matplotlib.pyplot as plt
    n = len(teacher_vx)
    step = max(1, n // 8000)
    idx = np.arange(0, n, step)
    fig, axes = plt.subplots(1, 2, figsize=(10, 4.6))
    pairs = [
        (axes[0], teacher_vx[idx], fuzzy_vx[idx], "横向速度 output_vx_norm"),
        (axes[1], teacher_w[idx], fuzzy_w[idx], "角速度 output_omega_norm"),
    ]
    for ax, x, y, name in pairs:
        ax.scatter(x, y, s=6, alpha=0.35, c="#1f77b4", linewidths=0)
        ax.plot([-1, 1], [-1, 1], "k--", linewidth=1)
        ax.set_xlim(-1.05, 1.05)
        ax.set_ylim(-1.05, 1.05)
        ax.set_aspect("equal", adjustable="box")
        ax.set_xlabel("PPO")
        ax.set_ylabel("模糊规则库")
        ax.set_title(name)
        ax.grid(True, alpha=0.3)
    fig.suptitle(title)
    fig.tight_layout()
    fig.savefig(out_path, dpi=160)
    plt.close(fig)


def plot_series(teacher_vx, fuzzy_vx, teacher_w, fuzzy_w, out_path, title, max_points=4000):
    import matplotlib.pyplot as plt
    n = len(teacher_vx)
    step = max(1, n // max_points)
    idx = np.arange(0, n, step)
    x = idx
    fig, axes = plt.subplots(2, 1, figsize=(10, 6.2), sharex=True)
    axes[0].plot(x, teacher_vx[idx], label="PPO", linewidth=1.0)
    axes[0].plot(x, fuzzy_vx[idx], label="模糊规则库", linewidth=1.0, alpha=0.85)
    axes[0].set_ylabel("vx_norm")
    axes[0].legend(loc="upper right")
    axes[0].grid(True, alpha=0.3)
    axes[1].plot(x, teacher_w[idx], label="PPO", linewidth=1.0)
    axes[1].plot(x, fuzzy_w[idx], label="模糊规则库", linewidth=1.0, alpha=0.85)
    axes[1].set_ylabel("omega_norm")
    axes[1].set_xlabel("样本序号")
    axes[1].legend(loc="upper right")
    axes[1].grid(True, alpha=0.3)
    fig.suptitle(title)
    fig.tight_layout()
    fig.savefig(out_path, dpi=160)
    plt.close(fig)


def plot_summary_bars(summary, out_path):
    import matplotlib.pyplot as plt
    names = summary["file"].tolist()
    x = np.arange(len(names))
    width = 0.18
    fig, ax = plt.subplots(figsize=(max(8, 1.3 * len(names) + 3), 4.8))
    series = [
        ("rmse_vx", "RMSE vx"),
        ("rmse_omega", "RMSE omega"),
        ("mae_vx", "MAE vx"),
        ("mae_omega", "MAE omega"),
    ]
    for i, (col, label) in enumerate(series):
        ax.bar(x + (i - 1.5) * width, summary[col].to_numpy(), width, label=label)
    ax.set_xticks(x)
    ax.set_xticklabels(names, rotation=25, ha="right")
    ax.set_ylabel("归一化输出误差")
    ax.set_title("开环输出逼近误差（规则加权平均）")
    ax.legend(loc="upper right")
    ax.grid(True, axis="y", alpha=0.3)
    fig.tight_layout()
    fig.savefig(out_path, dpi=160)
    plt.close(fig)


def main():
    here = os.path.dirname(os.path.abspath(__file__))
    parser = argparse.ArgumentParser(description="PPO 与模糊规则库开环输出对比")
    parser.add_argument("--data", nargs="+", required=True,
                        help="一个或多个 PPO 采集 csv，或这些 csv 所在的文件夹")
    parser.add_argument("--fuzzy", default=os.path.join(here, "FuzzyController.cs"),
                        help="FuzzyController.cs 路径")
    parser.add_argument("--out", default=None, help="结果输出目录，默认写到数据旁边的 open_loop_out")
    args = parser.parse_args()

    if not os.path.exists(args.fuzzy):
        sys.exit(f"找不到规则库文件: {args.fuzzy}")
    mf, rules = load_fuzzy(args.fuzzy)
    print(f"已加载规则库: {len(rules)} 条规则, {args.fuzzy}")

    csvs = collect_csvs(args.data)
    if not csvs:
        sys.exit(f"没有找到 csv: {args.data}")

    if args.out:
        out_dir = args.out
    elif len(args.data) == 1 and os.path.isdir(args.data[0]):
        out_dir = os.path.join(args.data[0], "open_loop_out")
    else:
        out_dir = os.path.join(os.path.dirname(os.path.abspath(csvs[0])), "open_loop_out")
    os.makedirs(out_dir, exist_ok=True)

    setup_font()
    rows = []
    all_tvx, all_fvx, all_tw, all_fw = [], [], [], []

    for path in csvs:
        table = load_teacher_csv(path)
        if table is None:
            print(f"跳过（不是蒸馏记录格式）: {os.path.basename(path)}")
            continue
        front = table["front_lr_diff"].to_numpy()
        rear = table["rear_lr_diff"].to_numpy()
        fc = table["front_center"].to_numpy()
        rc = table["rear_center"].to_numpy()
        tvx = table["output_vx_norm"].to_numpy()
        tw = table["output_omega_norm"].to_numpy()
        fvx, fw = evaluate_batch(front, rear, fc, rc, mf, rules, with_recovery=False)
        dvx, dw = evaluate_batch(front, rear, fc, rc, mf, rules, with_recovery=True)

        mae_vx, rmse_vx, p95_vx, max_vx = metrics(tvx, fvx)
        mae_w, rmse_w, p95_w, max_w = metrics(tw, fw)
        mae_vx_d, rmse_vx_d, _, _ = metrics(tvx, dvx)
        mae_w_d, rmse_w_d, _, _ = metrics(tw, dw)
        name = os.path.splitext(os.path.basename(path))[0]
        rows.append({
            "file": name,
            "n": len(table),
            "mae_vx": mae_vx,
            "rmse_vx": rmse_vx,
            "p95_vx": p95_vx,
            "max_vx": max_vx,
            "mae_omega": mae_w,
            "rmse_omega": rmse_w,
            "p95_omega": p95_w,
            "max_omega": max_w,
            "mae_vx_with_recovery": mae_vx_d,
            "rmse_vx_with_recovery": rmse_vx_d,
            "mae_omega_with_recovery": mae_w_d,
            "rmse_omega_with_recovery": rmse_w_d,
        })
        all_tvx.append(tvx)
        all_fvx.append(fvx)
        all_tw.append(tw)
        all_fw.append(fw)
        plot_series(tvx, fvx, tw, fw, os.path.join(out_dir, f"{name}_series.png"),
                    f"{name} 开环输出")
        print(f"{name}: N={len(table)}  RMSE(vx)={rmse_vx:.4f}  MAE(vx)={mae_vx:.4f}  "
              f"RMSE(w)={rmse_w:.4f}  MAE(w)={mae_w:.4f}")

    if not rows:
        sys.exit("没有可用的蒸馏 CSV。需要含 output_vx_norm 与四维磁特征列。")

    summary = pd.DataFrame(rows)
    tvx = np.concatenate(all_tvx)
    fvx = np.concatenate(all_fvx)
    tw = np.concatenate(all_tw)
    fw = np.concatenate(all_fw)
    mae_vx, rmse_vx, p95_vx, max_vx = metrics(tvx, fvx)
    mae_w, rmse_w, p95_w, max_w = metrics(tw, fw)
    overall = {
        "file": "ALL",
        "n": int(len(tvx)),
        "mae_vx": mae_vx,
        "rmse_vx": rmse_vx,
        "p95_vx": p95_vx,
        "max_vx": max_vx,
        "mae_omega": mae_w,
        "rmse_omega": rmse_w,
        "p95_omega": p95_w,
        "max_omega": max_w,
        "mae_vx_with_recovery": np.nan,
        "rmse_vx_with_recovery": np.nan,
        "mae_omega_with_recovery": np.nan,
        "rmse_omega_with_recovery": np.nan,
    }
    summary = pd.concat([summary, pd.DataFrame([overall])], ignore_index=True)
    csv_path = os.path.join(out_dir, "open_loop_summary.csv")
    summary.to_csv(csv_path, index=False, float_format="%.6f")

    plot_scatter(tvx, fvx, tw, fw, os.path.join(out_dir, "open_loop_scatter.png"),
                 "开环输出散点（规则加权平均）")
    plot_summary_bars(summary[summary["file"] != "ALL"], os.path.join(out_dir, "open_loop_bars.png"))

    print("\n开环逼近（规则加权平均，不含低置信度恢复）")
    print(f"{'File':<28} {'N':>8} {'RMSEvx':>8} {'MAEvx':>8} {'RMSEw':>8} {'MAEw':>8}")
    for _, r in summary.iterrows():
        print(f"{r['file']:<28} {int(r['n']):8d} {r['rmse_vx']:8.4f} {r['mae_vx']:8.4f} "
              f"{r['rmse_omega']:8.4f} {r['mae_omega']:8.4f}")
    print(f"\n汇总表: {csv_path}")
    print(f"散点图: {os.path.join(out_dir, 'open_loop_scatter.png')}")
    print(f"柱状图: {os.path.join(out_dir, 'open_loop_bars.png')}")
    print("各文件时序图: <文件名>_series.png")
    print("论文主指标用 RMSEvx / MAEvx / RMSEw / MAEw。with_recovery 列是部署时含恢复混合的误差。")


if __name__ == "__main__":
    main()
