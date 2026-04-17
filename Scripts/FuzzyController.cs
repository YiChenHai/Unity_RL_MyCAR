// 模糊控制器 - 自动生成
// 生成时间: 2026-04-08 16:35:02
// 数据样本数: 300000
// 规则数: 86
// 输入变量: front_lr_diff, rear_lr_diff, front_center, rear_center
// 输出变量: output_vx_norm, output_omega_norm
//
// 隶属函数配置:
//   front_lr_diff: 7 集 (NB, NM, NS, ZE, PS, PM, PB)
//   rear_lr_diff: 7 集 (NB, NM, NS, ZE, PS, PM, PB)
//   front_center: 5 集 (VL, LO, ME, HI, VH)
//   rear_center: 5 集 (VL, LO, ME, HI, VH)

using System;
using UnityEngine;

public class FuzzyController
{
    // ===== 三角形隶属函数参数 =====
    // 每行: { left, center, right }

    // front_lr_diff: NB, NM, NS, ZE, PS, PM, PB
    private static readonly float[,] MF_FrontLR = {
        { -1.00000f, -1.00000f, -0.25967f },  // NB
        { -1.00000f, -0.25967f, -0.12983f },  // NM
        { -0.25967f, -0.12983f,  0.00000f },  // NS
        { -0.12983f,  0.00000f,  0.09549f },  // ZE
        {  0.00000f,  0.09549f,  0.19099f },  // PS
        {  0.09549f,  0.19099f,  0.28648f },  // PM
        {  0.19099f,  1.00000f,  1.00000f },  // PB
    };

    // rear_lr_diff: NB, NM, NS, ZE, PS, PM, PB
    private static readonly float[,] MF_RearLR = {
        { -1.00000f, -1.00000f, -0.26287f },  // NB
        { -1.00000f, -0.26287f, -0.13144f },  // NM
        { -0.26287f, -0.13144f,  0.00000f },  // NS
        { -0.13144f,  0.00000f,  0.09660f },  // ZE
        {  0.00000f,  0.09660f,  0.19321f },  // PS
        {  0.09660f,  0.19321f,  0.28981f },  // PM
        {  0.19321f,  1.00000f,  1.00000f },  // PB
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

    // ===== 规则库 (86 条规则) =====
    // 每行: { 前LR集, 后LR集, 前中集, 后中集, 输出vx, 输出omega, 权重 }
    private static readonly float[,] Rules = {
        {  2,  3,  4,  4,  0.390301f, -0.349072f,  0.99906f },  // Rule 1: NS & ZE & VH & VH
        {  3,  3,  4,  4,  0.017121f, -0.083445f,  0.99837f },  // Rule 2: ZE & ZE & VH & VH
        {  3,  4,  4,  4, -0.184102f, -0.561280f,  0.99827f },  // Rule 3: ZE & PS & VH & VH
        {  3,  2,  4,  4,  0.195290f,  0.467853f,  0.99812f },  // Rule 4: ZE & NS & VH & VH
        {  2,  4,  4,  4,  0.189213f, -0.263172f,  0.99724f },  // Rule 5: NS & PS & VH & VH
        {  4,  3,  4,  4, -0.233588f,  0.218258f,  0.99683f },  // Rule 6: PS & ZE & VH & VH
        {  1,  4,  4,  4,  0.636919f, -0.692591f,  0.99544f },  // Rule 7: NM & PS & VH & VH
        {  3,  5,  4,  4, -0.381233f, -0.709599f,  0.99428f },  // Rule 8: ZE & PM & VH & VH
        {  5,  3,  4,  4, -0.481617f,  0.554563f,  0.99291f },  // Rule 9: PM & ZE & VH & VH
        {  4,  2,  4,  4, -0.077797f,  0.465700f,  0.99107f },  // Rule 10: PS & NS & VH & VH
        {  4,  1,  4,  4,  0.200117f,  0.657810f,  0.98989f },  // Rule 11: PS & NM & VH & VH
        {  4,  4,  4,  4, -0.386126f, -0.251864f,  0.98889f },  // Rule 12: PS & PS & VH & VH
        {  3,  1,  4,  4,  0.604194f,  0.783210f,  0.98879f },  // Rule 13: ZE & NM & VH & VH
        {  5,  1,  4,  4, -0.085571f,  0.880831f,  0.98621f },  // Rule 14: PM & NM & VH & VH
        {  2,  2,  4,  4,  0.533053f,  0.486217f,  0.98564f },  // Rule 15: NS & NS & VH & VH
        {  1,  3,  4,  4,  0.836323f, -0.873960f,  0.98416f },  // Rule 16: NM & ZE & VH & VH
        {  4,  5,  4,  4, -0.478023f, -0.745770f,  0.97940f },  // Rule 17: PS & PM & VH & VH
        {  5,  2,  4,  4, -0.286488f,  0.599252f,  0.97818f },  // Rule 18: PM & NS & VH & VH
        {  2,  1,  4,  4,  0.701868f,  0.692367f,  0.97674f },  // Rule 19: NS & NM & VH & VH
        {  2,  5,  4,  4,  0.132051f, -0.663249f,  0.96876f },  // Rule 20: NS & PM & VH & VH
        {  1,  5,  4,  4,  0.500813f, -0.624457f,  0.96594f },  // Rule 21: NM & PM & VH & VH
        {  5,  4,  4,  4, -0.645572f,  0.578262f,  0.96041f },  // Rule 22: PM & PS & VH & VH
        {  5,  5,  4,  4, -0.387416f, -0.383318f,  0.94283f },  // Rule 23: PM & PM & VH & VH
        {  1,  1,  4,  4,  0.478215f, -0.021978f,  0.93205f },  // Rule 24: NM & NM & VH & VH
        {  6,  0,  3,  2, -0.153006f,  0.856840f,  0.90645f },  // Rule 25: PB & NB & HI & ME
        {  1,  2,  4,  4,  0.576008f, -0.503042f,  0.90098f },  // Rule 26: NM & NS & VH & VH
        {  3,  3,  3,  3, -0.016848f, -0.011157f,  0.77503f },  // Rule 27: ZE & ZE & HI & HI
        {  2,  4,  3,  3, -0.005743f, -0.018305f,  0.64674f },  // Rule 28: NS & PS & HI & HI
        {  6,  4,  3,  4, -0.866762f,  0.948131f,  0.61709f },  // Rule 29: PB & PS & HI & VH
        {  6,  0,  3,  3, -0.000424f,  0.543051f,  0.57304f },  // Rule 30: PB & NB & HI & HI
        {  4,  2,  3,  3, -0.008706f, -0.013587f,  0.54820f },  // Rule 31: PS & NS & HI & HI
        {  0,  5,  3,  4,  0.782094f, -0.753490f,  0.54294f },  // Rule 32: NB & PM & HI & VH
        {  6,  3,  3,  4, -0.430186f,  0.516879f,  0.54119f },  // Rule 33: PB & ZE & HI & VH
        {  5,  0,  4,  3,  0.970752f,  1.000000f,  0.51331f },  // Rule 34: PM & NB & VH & HI
        {  6,  3,  4,  4, -0.806332f,  0.781282f,  0.50982f },  // Rule 35: PB & ZE & VH & VH
        {  0,  6,  3,  3, -0.221007f, -0.501914f,  0.49505f },  // Rule 36: NB & PB & HI & HI
        {  0,  3,  3,  4,  0.716859f, -0.938817f,  0.49232f },  // Rule 37: NB & ZE & HI & VH
        {  0,  6,  3,  4,  0.065410f, -0.877705f,  0.46622f },  // Rule 38: NB & PB & HI & VH
        {  6,  2,  4,  4, -0.666426f,  0.689544f,  0.45801f },  // Rule 39: PB & NS & VH & VH
        {  6,  0,  4,  2,  0.129559f,  1.000000f,  0.45684f },  // Rule 40: PB & NB & VH & ME
        {  1,  0,  4,  3,  0.790552f,  0.174589f,  0.45142f },  // Rule 41: NM & NB & VH & HI
        {  0,  3,  4,  4,  0.528130f, -0.704234f,  0.44943f },  // Rule 42: NB & ZE & VH & VH
        {  6,  1,  4,  4, -0.383829f,  0.869162f,  0.44701f },  // Rule 43: PB & NM & VH & VH
        {  5,  0,  4,  4,  0.483968f,  0.916688f,  0.43309f },  // Rule 44: PM & NB & VH & VH
        {  0,  1,  4,  4,  0.751474f, -0.936973f,  0.42802f },  // Rule 45: NB & NM & VH & VH
        {  0,  4,  4,  4,  0.422944f, -0.553224f,  0.42619f },  // Rule 46: NB & PS & VH & VH
        {  0,  1,  3,  4,  0.437123f, -0.945284f,  0.41913f },  // Rule 47: NB & NM & HI & VH
        {  5,  2,  3,  3,  0.017260f, -0.006734f,  0.40578f },  // Rule 48: PM & NS & HI & HI
        {  6,  4,  4,  4, -0.497779f,  0.404559f,  0.40011f },  // Rule 49: PB & PS & VH & VH
        {  6,  0,  4,  3,  0.326130f,  0.958881f,  0.39154f },  // Rule 50: PB & NB & VH & HI
        {  1,  6,  4,  4,  0.270413f, -0.957239f,  0.38933f },  // Rule 51: NM & PB & VH & VH
        {  5,  1,  3,  3,  0.017982f, -0.018183f,  0.38370f },  // Rule 52: PM & NM & HI & HI
        {  1,  6,  4,  3, -0.756824f, -0.764468f,  0.37842f },  // Rule 53: NM & PB & VH & HI
        {  6,  0,  3,  4, -0.151236f,  0.886874f,  0.36995f },  // Rule 54: PB & NB & HI & VH
        {  6,  5,  4,  4, -0.428192f,  0.510865f,  0.36811f },  // Rule 55: PB & PM & VH & VH
        {  4,  6,  4,  3, -0.616533f, -0.467013f,  0.36166f },  // Rule 56: PS & PB & VH & HI
        {  2,  5,  3,  3,  0.003106f, -0.024976f,  0.36155f },  // Rule 57: NS & PM & HI & HI
        {  6,  0,  2,  2,  0.025455f,  0.235897f,  0.36154f },  // Rule 58: PB & NB & ME & ME
        {  0,  4,  3,  4,  0.551148f, -0.652542f,  0.35975f },  // Rule 59: NB & PS & HI & VH
        {  5,  6,  4,  4, -0.703471f, -0.348462f,  0.35695f },  // Rule 60: PM & PB & VH & VH
        {  2,  0,  4,  3,  1.000000f,  0.871336f,  0.35410f },  // Rule 61: NS & NB & VH & HI
        {  0,  2,  4,  4,  0.450616f, -0.814822f,  0.34956f },  // Rule 62: NB & NS & VH & VH
        {  6,  2,  3,  4, -0.896587f,  0.816589f,  0.33914f },  // Rule 63: PB & NS & HI & VH
        {  2,  6,  4,  4, -0.168526f, -0.836431f,  0.33522f },  // Rule 64: NS & PB & VH & VH
        {  0,  6,  2,  3, -0.238868f, -0.401631f,  0.32100f },  // Rule 65: NB & PB & ME & HI
        {  0,  5,  4,  4,  0.240649f, -0.125959f,  0.32093f },  // Rule 66: NB & PM & VH & VH
        {  3,  6,  4,  4, -0.609467f, -0.886758f,  0.31545f },  // Rule 67: ZE & PB & VH & VH
        {  1,  0,  4,  4,  0.970605f,  0.366209f,  0.31461f },  // Rule 68: NM & NB & VH & VH
        {  3,  4,  3,  3, -0.000311f, -0.053675f,  0.29834f },  // Rule 69: ZE & PS & HI & HI
        {  0,  2,  3,  4,  0.928217f, -1.000000f,  0.29781f },  // Rule 70: NB & NS & HI & VH
        {  1,  5,  3,  3, -0.028818f, -0.087075f,  0.29314f },  // Rule 71: NM & PM & HI & HI
        {  6,  1,  2,  2,  0.026886f,  0.094888f,  0.28015f },  // Rule 72: PB & NM & ME & ME
        {  6,  1,  3,  3,  0.033838f,  0.150016f,  0.25958f },  // Rule 73: PB & NM & HI & HI
        {  1,  6,  2,  2, -0.037709f, -0.065257f,  0.25949f },  // Rule 74: NM & PB & ME & ME
        {  4,  6,  4,  4, -0.488872f, -0.600025f,  0.25908f },  // Rule 75: PS & PB & VH & VH
        {  0,  6,  4,  4,  0.323037f, -0.863166f,  0.25592f },  // Rule 76: NB & PB & VH & VH
        {  6,  0,  2,  3,  0.115920f,  0.339694f,  0.24221f },  // Rule 77: PB & NB & ME & HI
        {  6,  0,  4,  4, -0.194265f,  0.940470f,  0.24011f },  // Rule 78: PB & NB & VH & VH
        {  0,  6,  2,  2, -0.110807f, -0.215045f,  0.23710f },  // Rule 79: NB & PB & ME & ME
        {  1,  6,  3,  3, -0.056926f, -0.116332f,  0.23326f },  // Rule 80: NM & PB & HI & HI
        {  5,  6,  4,  3, -0.977542f, -0.892522f,  0.21272f },  // Rule 81: PM & PB & VH & HI
        {  6,  1,  3,  4, -0.122531f,  0.602123f,  0.20986f },  // Rule 82: PB & NM & HI & VH
        {  6,  6,  4,  4, -0.702632f,  0.360468f,  0.16308f },  // Rule 83: PB & PB & VH & VH
        {  1,  6,  3,  4, -0.044623f, -0.300720f,  0.12946f },  // Rule 84: NM & PB & HI & VH
        {  6,  6,  3,  4, -0.733636f,  0.781932f,  0.12452f },  // Rule 85: PB & PB & HI & VH
        {  1,  6,  2,  3, -0.112584f, -0.214407f,  0.11474f },  // Rule 86: NM & PB & ME & HI
    };

    private const int N_RULES = 86;

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
        return $"FuzzyController: {N_RULES} rules, generated 2026-04-08 16:35:02";
    }
}