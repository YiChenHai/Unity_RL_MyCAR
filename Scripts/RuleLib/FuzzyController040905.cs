// 模糊控制器 - 自动生成
// 生成时间: 2026-11-05 11:29:10
// 数据样本数: 300000
// 规则数: 93 (数据提取 85, 补全 8)
// 输入变量: front_lr_diff, rear_lr_diff, front_center, rear_center
// 输出变量: output_vx_norm, output_omega_norm
//
// 隶属函数配置:
//   front_lr_diff: 9 集 (NB, NM, NS, NZ, ZE, PZ, PS, PM, PB)
//   rear_lr_diff: 9 集 (NB, NM, NS, NZ, ZE, PZ, PS, PM, PB)
//   front_center: 5 集 (VL, LO, ME, HI, VH)
//   rear_center: 5 集 (VL, LO, ME, HI, VH)

using System;
using UnityEngine;

public class FuzzyController040905
{
    // ===== 三角形隶属函数参数 =====
    // 每行: { left, center, right }

    // front_lr_diff: NB, NM, NS, NZ, ZE, PZ, PS, PM, PB
    private static readonly float[,] MF_FrontLR = {
        { -1.00000f, -1.00000f, -0.37500f },  // NB
        { -1.00000f, -0.37500f, -0.25000f },  // NM
        { -0.37500f, -0.25000f, -0.12500f },  // NS
        { -0.25000f, -0.12500f,  0.00000f },  // NZ
        { -0.12500f,  0.00000f,  0.12500f },  // ZE
        {  0.00000f,  0.12500f,  0.25000f },  // PZ
        {  0.12500f,  0.25000f,  0.37500f },  // PS
        {  0.25000f,  0.37500f,  1.00000f },  // PM
        {  0.37500f,  1.00000f,  1.00000f },  // PB
    };

    // rear_lr_diff: NB, NM, NS, NZ, ZE, PZ, PS, PM, PB
    private static readonly float[,] MF_RearLR = {
        { -1.00000f, -1.00000f, -0.37500f },  // NB
        { -1.00000f, -0.37500f, -0.25000f },  // NM
        { -0.37500f, -0.25000f, -0.12500f },  // NS
        { -0.25000f, -0.12500f,  0.00000f },  // NZ
        { -0.12500f,  0.00000f,  0.12500f },  // ZE
        {  0.00000f,  0.12500f,  0.25000f },  // PZ
        {  0.12500f,  0.25000f,  0.37500f },  // PS
        {  0.25000f,  0.37500f,  1.00000f },  // PM
        {  0.37500f,  1.00000f,  1.00000f },  // PB
    };

    // front_center: VL, LO, ME, HI, VH
    private static readonly float[,] MF_FrontCenter = {
        {  0.00000f,  0.00000f,  0.25000f },  // VL
        {  0.00000f,  0.25000f,  0.50000f },  // LO
        {  0.25000f,  0.50000f,  0.75000f },  // ME
        {  0.50000f,  0.75000f,  1.00000f },  // HI
        {  0.75000f,  1.00000f,  1.00000f },  // VH
    };

    // rear_center: VL, LO, ME, HI, VH
    private static readonly float[,] MF_RearCenter = {
        {  0.00000f,  0.00000f,  0.25000f },  // VL
        {  0.00000f,  0.25000f,  0.50000f },  // LO
        {  0.25000f,  0.50000f,  0.75000f },  // ME
        {  0.50000f,  0.75000f,  1.00000f },  // HI
        {  0.75000f,  1.00000f,  1.00000f },  // VH
    };

    // ===== 规则库 (93 条规则) =====
    // 每行: { 前LR集, 后LR集, 前中集, 后中集, 输出vx, 输出omega, 权重 }
    private static readonly float[,] Rules = {
        {  4,  3,  4,  4,  0.241132f,  0.402909f,  0.99775f },  // Rule 1: ZE & NZ & VH & VH
        {  6,  6,  4,  4, -0.577679f, -0.226197f,  0.99607f },  // Rule 2: PS & PS & VH & VH
        {  5,  4,  4,  4, -0.133444f,  0.326474f,  0.99548f },  // Rule 3: PZ & ZE & VH & VH
        {  4,  4,  4,  4,  0.006779f, -0.061046f,  0.99492f },  // Rule 4: ZE & ZE & VH & VH
        {  4,  5,  4,  4, -0.209991f, -0.572921f,  0.99488f },  // Rule 5: ZE & PZ & VH & VH
        {  3,  2,  4,  4,  0.625741f,  0.456701f,  0.99442f },  // Rule 6: NZ & NS & VH & VH
        {  3,  3,  4,  4,  0.406351f, -0.284203f,  0.99321f },  // Rule 7: NZ & NZ & VH & VH
        {  7,  5,  4,  4, -0.623145f,  0.859315f,  0.99316f },  // Rule 8: PM & PZ & VH & VH
        {  1,  1,  4,  4,  1.000000f, -0.051003f,  0.99281f },  // Rule 9: NM & NM & VH & VH
        {  7,  2,  4,  4, -0.254470f,  1.000000f,  0.99202f },  // Rule 10: PM & NS & VH & VH
        {  1,  3,  4,  4,  0.739511f, -0.946495f,  0.99151f },  // Rule 11: NM & NZ & VH & VH
        {  1,  2,  4,  4,  0.970881f, -0.574306f,  0.99132f },  // Rule 12: NM & NS & VH & VH
        {  3,  4,  4,  4,  0.208335f, -0.525087f,  0.99068f },  // Rule 13: NZ & ZE & VH & VH
        {  2,  1,  4,  4,  0.955482f,  0.408210f,  0.99015f },  // Rule 14: NS & NM & VH & VH
        {  5,  5,  4,  4, -0.326173f, -0.297188f,  0.98978f },  // Rule 15: PZ & PZ & VH & VH
        {  4,  6,  4,  4, -0.385121f, -0.911888f,  0.98847f },  // Rule 16: ZE & PS & VH & VH
        {  2,  2,  4,  4,  0.804875f,  0.011183f,  0.98814f },  // Rule 17: NS & NS & VH & VH
        {  4,  1,  4,  4,  0.703181f,  0.882051f,  0.98654f },  // Rule 18: ZE & NM & VH & VH
        {  6,  7,  4,  4, -0.618300f, -0.716781f,  0.98488f },  // Rule 19: PS & PM & VH & VH
        {  5,  6,  4,  4, -0.482590f, -0.666154f,  0.98481f },  // Rule 20: PZ & PS & VH & VH
        {  6,  2,  4,  4,  0.090316f,  1.000000f,  0.98276f },  // Rule 21: PS & NS & VH & VH
        {  2,  6,  4,  4, -0.127951f, -1.000000f,  0.98215f },  // Rule 22: NS & PS & VH & VH
        {  3,  5,  4,  4, -0.073951f, -0.980169f,  0.98155f },  // Rule 23: NZ & PZ & VH & VH
        {  2,  3,  4,  4,  0.556929f, -0.688928f,  0.97977f },  // Rule 24: NS & NZ & VH & VH
        {  1,  4,  4,  4,  0.612829f, -1.000000f,  0.97802f },  // Rule 25: NM & ZE & VH & VH
        {  6,  4,  4,  4, -0.326688f,  0.639091f,  0.97721f },  // Rule 26: PS & ZE & VH & VH
        {  5,  1,  4,  4,  0.466885f,  1.000000f,  0.97583f },  // Rule 27: PZ & NM & VH & VH
        {  1,  6,  4,  4,  0.092042f, -1.000000f,  0.97558f },  // Rule 28: NM & PS & VH & VH
        {  6,  1,  4,  4,  0.271680f,  1.000000f,  0.97500f },  // Rule 29: PS & NM & VH & VH
        {  6,  5,  4,  4, -0.444803f,  0.484276f,  0.97191f },  // Rule 30: PS & PZ & VH & VH
        {  1,  5,  4,  4,  0.366148f, -1.000000f,  0.97075f },  // Rule 31: NM & PZ & VH & VH
        {  5,  3,  4,  4,  0.077045f,  0.672383f,  0.96923f },  // Rule 32: PZ & NZ & VH & VH
        {  7,  1,  4,  4, -0.132256f,  1.000000f,  0.96864f },  // Rule 33: PM & NM & VH & VH
        {  3,  1,  4,  4,  0.784465f,  0.731144f,  0.96559f },  // Rule 34: NZ & NM & VH & VH
        {  7,  7,  4,  4, -0.705238f, -0.196724f,  0.96415f },  // Rule 35: PM & PM & VH & VH
        {  5,  7,  4,  4, -0.555470f, -0.863902f,  0.96310f },  // Rule 36: PZ & PM & VH & VH
        {  7,  6,  4,  4, -0.726045f,  0.499293f,  0.96157f },  // Rule 37: PM & PS & VH & VH
        {  7,  3,  4,  4, -0.450268f,  1.000000f,  0.95670f },  // Rule 38: PM & NZ & VH & VH
        {  2,  5,  4,  4,  0.058101f, -1.000000f,  0.95635f },  // Rule 39: NS & PZ & VH & VH
        {  6,  3,  4,  4, -0.069828f,  0.999865f,  0.95628f },  // Rule 40: PS & NZ & VH & VH
        {  4,  2,  4,  4,  0.488701f,  0.737939f,  0.95609f },  // Rule 41: ZE & NS & VH & VH
        {  4,  7,  4,  4, -0.479093f, -1.000000f,  0.94487f },  // Rule 42: ZE & PM & VH & VH
        {  1,  7,  4,  4, -0.049467f, -1.000000f,  0.94270f },  // Rule 43: NM & PM & VH & VH
        {  5,  2,  4,  4,  0.273148f,  0.998085f,  0.94121f },  // Rule 44: PZ & NS & VH & VH
        {  7,  4,  4,  4, -0.583928f,  0.996790f,  0.93887f },  // Rule 45: PM & ZE & VH & VH
        {  2,  4,  4,  4,  0.411786f, -0.911212f,  0.93140f },  // Rule 46: NS & ZE & VH & VH
        {  3,  6,  4,  4, -0.266322f, -1.000000f,  0.93120f },  // Rule 47: NZ & PS & VH & VH
        {  2,  7,  4,  4, -0.360269f, -1.000000f,  0.92868f },  // Rule 48: NS & PM & VH & VH
        {  8,  3,  3,  4, -0.843061f,  1.000000f,  0.77816f },  // Rule 49: PB & NZ & HI & VH
        {  8,  6,  3,  4, -0.769881f,  0.999805f,  0.76393f },  // Rule 50: PB & PS & HI & VH
        {  0,  7,  3,  4,  0.199284f, -1.000000f,  0.75227f },  // Rule 51: NB & PM & HI & VH
        {  8,  5,  3,  4, -0.801096f,  1.000000f,  0.74985f },  // Rule 52: PB & PZ & HI & VH
        {  3,  7,  4,  4, -0.435099f, -1.000000f,  0.74892f },  // Rule 53: NZ & PM & VH & VH
        {  8,  4,  3,  4, -0.826088f,  1.000000f,  0.73385f },  // Rule 54: PB & ZE & HI & VH
        {  8,  2,  3,  4, -0.814651f,  1.000000f,  0.72343f },  // Rule 55: PB & NS & HI & VH
        {  8,  1,  3,  4, -0.746372f,  1.000000f,  0.68870f },  // Rule 56: PB & NM & HI & VH
        {  0,  5,  3,  4,  0.606248f, -1.000000f,  0.68402f },  // Rule 57: NB & PZ & HI & VH
        {  8,  7,  3,  4, -0.748589f,  0.840665f,  0.67889f },  // Rule 58: PB & PM & HI & VH
        {  0,  6,  3,  4,  0.414373f, -1.000000f,  0.66495f },  // Rule 59: NB & PS & HI & VH
        {  0,  2,  3,  4,  0.995909f, -1.000000f,  0.66036f },  // Rule 60: NB & NS & HI & VH
        {  0,  3,  3,  4,  0.903993f, -1.000000f,  0.63737f },  // Rule 61: NB & NZ & HI & VH
        {  8,  0,  3,  3, -0.525908f,  1.000000f,  0.55870f },  // Rule 62: PB & NB & HI & HI
        {  0,  8,  3,  3,  0.028956f, -1.000000f,  0.54788f },  // Rule 63: NB & PB & HI & HI
        {  8,  2,  4,  4, -0.727884f,  1.000000f,  0.49314f },  // Rule 64: PB & NS & VH & VH
        {  8,  1,  4,  4, -0.582866f,  1.000000f,  0.49079f },  // Rule 65: PB & NM & VH & VH
        {  8,  7,  2,  4, -0.620865f,  0.852220f,  0.43439f },  // Rule 66: PB & PM & ME & VH
        {  0,  1,  2,  4,  0.620865f, -0.852220f,  0.43439f },  // Rule 67: NB & NM & ME & VH  [补全]
        {  0,  4,  3,  4,  0.759302f, -1.000000f,  0.38390f },  // Rule 68: NB & ZE & HI & VH
        {  0,  1,  3,  4,  1.000000f, -0.996296f,  0.37268f },  // Rule 69: NB & NM & HI & VH
        {  7,  8,  4,  4, -0.575774f, -0.677412f,  0.34024f },  // Rule 70: PM & PB & VH & VH
        {  1,  0,  4,  4,  0.575774f,  0.677412f,  0.34024f },  // Rule 71: NM & NB & VH & VH  [补全]
        {  8,  7,  4,  4, -0.820878f,  0.612916f,  0.31544f },  // Rule 72: PB & PM & VH & VH
        {  0,  1,  4,  4,  0.820878f, -0.612916f,  0.31544f },  // Rule 73: NB & NM & VH & VH  [补全]
        {  8,  8,  3,  3, -0.582189f,  0.030043f,  0.30601f },  // Rule 74: PB & PB & HI & HI
        {  0,  0,  3,  3,  0.582189f, -0.030043f,  0.30601f },  // Rule 75: NB & NB & HI & HI  [补全]
        {  0,  6,  4,  4,  0.339937f, -1.000000f,  0.28281f },  // Rule 76: NB & PS & VH & VH
        {  8,  4,  4,  4, -0.825299f,  1.000000f,  0.27775f },  // Rule 77: PB & ZE & VH & VH
        {  8,  3,  4,  4, -0.799604f,  1.000000f,  0.27667f },  // Rule 78: PB & NZ & VH & VH
        {  0,  7,  4,  4,  0.082461f, -1.000000f,  0.27187f },  // Rule 79: NB & PM & VH & VH
        {  0,  5,  4,  4,  0.542260f, -1.000000f,  0.26961f },  // Rule 80: NB & PZ & VH & VH
        {  8,  5,  4,  4, -0.819624f,  1.000000f,  0.26008f },  // Rule 81: PB & PZ & VH & VH
        {  0,  3,  4,  4,  0.913158f, -1.000000f,  0.25012f },  // Rule 82: NB & NZ & VH & VH
        {  8,  6,  4,  4, -0.814796f,  0.989532f,  0.23892f },  // Rule 83: PB & PS & VH & VH
        {  0,  2,  4,  4,  0.814796f, -0.989532f,  0.23892f },  // Rule 84: NB & NS & VH & VH  [补全]
        {  0,  4,  4,  4,  0.751300f, -1.000000f,  0.22818f },  // Rule 85: NB & ZE & VH & VH
        {  8,  0,  3,  4, -0.605757f,  1.000000f,  0.22588f },  // Rule 86: PB & NB & HI & VH
        {  0,  8,  3,  4, -0.070444f, -1.000000f,  0.15624f },  // Rule 87: NB & PB & HI & VH
        {  7,  8,  4,  3, -0.587596f, -0.247272f,  0.14592f },  // Rule 88: PM & PB & VH & HI
        {  1,  0,  4,  3,  0.587596f,  0.247272f,  0.14592f },  // Rule 89: NM & NB & VH & HI  [补全]
        {  7,  0,  4,  4, -0.366371f,  1.000000f,  0.09635f },  // Rule 90: PM & NB & VH & VH
        {  1,  8,  4,  4,  0.366371f, -1.000000f,  0.09635f },  // Rule 91: NM & PB & VH & VH  [补全]
        {  8,  0,  4,  4, -0.430784f,  1.000000f,  0.07810f },  // Rule 92: PB & NB & VH & VH
        {  0,  8,  4,  4,  0.430784f, -1.000000f,  0.07810f },  // Rule 93: NB & PB & VH & VH  [补全]
    };

    private const int N_RULES = 93;

    // ===== 模糊推理接口 =====
    /// <summary>
    /// 模糊推理：输入传感器特征，输出控制量
    /// </summary>
    /// <param name="frontLRDiff">前排左右差（归一化 [-1,1]）</param>
    /// <param name="rearLRDiff">后排左右差（归一化 [-1,1]）</param>
    /// <param name="frontCenter">前中传感器（归一化 [0,1]）</param>
    /// <param name="rearCenter">后中传感器（归一化 [0,1]）</param>
    /// <param name="outputVx">输出：横向速度（归一化 [-1,1]）</param>
    /// <param name="outputOmega">输出：角速度（归一化 [-1,1]）</param>
    public static void Evaluate(
        float frontLRDiff, float rearLRDiff,
        float frontCenter, float rearCenter,
        out float outputVx, out float outputOmega)
    {
        float sumWeightedVx = 0f;
        float sumWeightedOmega = 0f;
        float sumWeights = 0f;
        float bestWeight = -1f;
        float bestVx = 0f;
        float bestOmega = 0f;

        for (int r = 0; r < N_RULES; r++)
        {
            int mfFL = (int)Rules[r, 0];
            int mfRL = (int)Rules[r, 1];
            int mfFC = (int)Rules[r, 2];
            int mfRC = (int)Rules[r, 3];

            // 模糊化：计算各输入的隶属度
            float mu1 = TriMF(frontLRDiff, MF_FrontLR[mfFL, 0], MF_FrontLR[mfFL, 1], MF_FrontLR[mfFL, 2]);
            float mu2 = TriMF(rearLRDiff, MF_RearLR[mfRL, 0], MF_RearLR[mfRL, 1], MF_RearLR[mfRL, 2]);
            float mu3 = TriMF(frontCenter, MF_FrontCenter[mfFC, 0], MF_FrontCenter[mfFC, 1], MF_FrontCenter[mfFC, 2]);
            float mu4 = TriMF(rearCenter, MF_RearCenter[mfRC, 0], MF_RearCenter[mfRC, 1], MF_RearCenter[mfRC, 2]);

            // AND运算（乘积法）
            float firing = mu1 * mu2 * mu3 * mu4;

            if (firing > 1e-6f)
            {
                float outVx = Rules[r, 4];
                float outOmega = Rules[r, 5];
                float ruleWeight = Rules[r, 6];
                float w = firing * ruleWeight;

                sumWeightedVx += w * outVx;
                sumWeightedOmega += w * outOmega;
                sumWeights += w;
                if (w > bestWeight)
                {
                    bestWeight = w;
                    bestVx = outVx;
                    bestOmega = outOmega;
                }
            }
        }

        // 去模糊化：加权平均
        if (sumWeights > 1e-6f)
        {
            outputVx = Mathf.Clamp(sumWeightedVx / sumWeights, -1f, 1f);
            outputOmega = Mathf.Clamp(sumWeightedOmega / sumWeights, -1f, 1f);
        }
        else
        {
            // 兜底策略：无有效加权时使用触发最强单条规则
            if (bestWeight > 0f)
            {
                outputVx = Mathf.Clamp(bestVx, -1f, 1f);
                outputOmega = Mathf.Clamp(bestOmega, -1f, 1f);
            }
            else
            {
                outputVx = 0f;
                outputOmega = 0f;
            }
        }

        // 低置信度恢复混合：偏离训练分布时，加入解析式恢复控制，降低脱轨风险
        float trackConf = Mathf.Clamp01(0.5f * (frontCenter + rearCenter));
        float lateralErr = 0.55f * frontLRDiff + 0.45f * rearLRDiff;
        float headingErr = frontLRDiff - rearLRDiff;

        float recoverVx = Mathf.Clamp(-0.85f * lateralErr, -1f, 1f);
        float recoverOmega = Mathf.Clamp(-(1.15f * lateralErr + 0.75f * headingErr), -1f, 1f);

        float confBlend = Mathf.Clamp01((0.65f - trackConf) / 0.35f);
        float weightBlend = Mathf.Clamp01((0.05f - sumWeights) / 0.05f);
        float blend = Mathf.Max(confBlend, weightBlend);

        outputVx = Mathf.Lerp(outputVx, recoverVx, blend);
        outputOmega = Mathf.Lerp(outputOmega, recoverOmega, blend);
    }

    // ===== 三角形隶属函数（含边界肩型处理）=====
    private static float TriMF(float x, float a, float b, float c)
    {
        // 左肩MF (a≈b): x<=b 时 μ=1
        if (a >= b - 1e-6f)
        {
            if (x <= b) return 1f;
            if (x >= c) return 0f;
            return (c - x) / Mathf.Max(1e-6f, c - b);
        }
        // 右肩MF (b≈c): x>=b 时 μ=1
        if (b >= c - 1e-6f)
        {
            if (x >= b) return 1f;
            if (x <= a) return 0f;
            return (x - a) / Mathf.Max(1e-6f, b - a);
        }
        // 常规三角形MF
        if (x <= a || x >= c) return 0f;
        if (x <= b) return (x - a) / Mathf.Max(1e-6f, b - a);
        return (c - x) / Mathf.Max(1e-6f, c - b);
    }

    // ===== 便捷方法 =====
    /// <summary>
    /// 直接从6个传感器原始值计算控制输出
    /// </summary>
    public static void EvaluateFromSensors(
        float sensorFL, float sensorFC, float sensorFR,
        float sensorRL, float sensorRC, float sensorRR,
        float maxField,
        out float outputVx, out float outputOmega)
    {
        float invMax = 1f / Mathf.Max(1e-6f, maxField);
        float frontLRDiff = Mathf.Clamp((sensorFL - sensorFR) * invMax, -1f, 1f);
        float rearLRDiff = Mathf.Clamp((sensorRL - sensorRR) * invMax, -1f, 1f);
        float frontCenter = Mathf.Clamp01(sensorFC * invMax);
        float rearCenter = Mathf.Clamp01(sensorRC * invMax);
        Evaluate(frontLRDiff, rearLRDiff, frontCenter, rearCenter, out outputVx, out outputOmega);
    }

    /// <summary>
    /// 获取规则库信息（用于调试）
    /// </summary>
    public static string GetInfo()
    {
        return $"FuzzyController040905: {N_RULES} rules, generated 2026-11-05 11:29:10";
    }
}