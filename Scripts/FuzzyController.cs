// 模糊控制器 - 自动生成
// 生成时间: 2026-10-13 14:07:55
// 数据样本数: 300000
// 规则数: 49
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
        { -1.00000f, -1.00000f, -0.25456f },  // NB
        { -1.00000f, -0.25456f, -0.12728f },  // NM
        { -0.25456f, -0.12728f,  0.00000f },  // NS
        { -0.12728f,  0.00000f,  0.09291f },  // ZE
        {  0.00000f,  0.09291f,  0.18581f },  // PS
        {  0.09291f,  0.18581f,  0.27872f },  // PM
        {  0.18581f,  1.00000f,  1.00000f },  // PB
    };

    // rear_lr_diff: NB, NM, NS, ZE, PS, PM, PB
    private static readonly float[,] MF_RearLR = {
        { -1.00000f, -1.00000f, -0.25888f },  // NB
        { -1.00000f, -0.25888f, -0.12944f },  // NM
        { -0.25888f, -0.12944f,  0.00000f },  // NS
        { -0.12944f,  0.00000f,  0.09257f },  // ZE
        {  0.00000f,  0.09257f,  0.18513f },  // PS
        {  0.09257f,  0.18513f,  0.27770f },  // PM
        {  0.18513f,  1.00000f,  1.00000f },  // PB
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

    // ===== 规则库 (49 条规则) =====
    // 每行: { 前LR集, 后LR集, 前中集, 后中集, 输出vx, 输出omega, 权重 }
    private static readonly float[,] Rules = {
        {  3,  3,  4,  4,  0.017929f, -0.026124f,  0.99865f },  // Rule 1: ZE & ZE & VH & VH
        {  3,  4,  4,  4, -0.088993f, -0.342042f,  0.99451f },  // Rule 2: ZE & PS & VH & VH
        {  4,  3,  4,  4, -0.144304f,  0.307307f,  0.96951f },  // Rule 3: PS & ZE & VH & VH
        {  4,  2,  4,  4,  0.058192f,  0.444600f,  0.96580f },  // Rule 4: PS & NS & VH & VH
        {  5,  2,  4,  4, -0.021993f,  0.370220f,  0.96097f },  // Rule 5: PM & NS & VH & VH
        {  5,  3,  4,  4, -0.356366f,  0.618800f,  0.95991f },  // Rule 6: PM & ZE & VH & VH
        {  5,  1,  4,  4,  0.050959f,  0.557656f,  0.95272f },  // Rule 7: PM & NM & VH & VH
        {  1,  4,  4,  4,  0.236731f, -0.597427f,  0.93986f },  // Rule 8: NM & PS & VH & VH
        {  2,  4,  4,  4,  0.066662f, -0.300772f,  0.93555f },  // Rule 9: NS & PS & VH & VH
        {  2,  3,  4,  4,  0.349927f, -0.625566f,  0.92799f },  // Rule 10: NS & ZE & VH & VH
        {  1,  5,  4,  4,  0.140939f, -0.582374f,  0.92291f },  // Rule 11: NM & PM & VH & VH
        {  6,  0,  2,  2, -0.128283f,  0.703740f,  0.88256f },  // Rule 12: PB & NB & ME & ME
        {  3,  5,  4,  4, -0.219586f, -0.458686f,  0.88241f },  // Rule 13: ZE & PM & VH & VH
        {  2,  5,  4,  4,  0.025798f, -0.305182f,  0.87324f },  // Rule 14: NS & PM & VH & VH
        {  3,  3,  3,  3, -0.076799f,  0.010761f,  0.84973f },  // Rule 15: ZE & ZE & HI & HI
        {  6,  0,  3,  3, -0.088507f,  0.855324f,  0.78422f },  // Rule 16: PB & NB & HI & HI
        {  3,  2,  4,  4,  0.152831f,  0.342153f,  0.78237f },  // Rule 17: ZE & NS & VH & VH
        {  0,  6,  2,  3,  0.072222f, -0.894506f,  0.77473f },  // Rule 18: NB & PB & ME & HI
        {  0,  6,  2,  2,  0.014799f, -0.564441f,  0.73179f },  // Rule 19: NB & PB & ME & ME
        {  4,  4,  4,  4, -0.192388f, -0.415531f,  0.73059f },  // Rule 20: PS & PS & VH & VH
        {  3,  1,  4,  4,  0.300668f,  0.530855f,  0.71282f },  // Rule 21: ZE & NM & VH & VH
        {  4,  5,  4,  4, -0.105877f, -0.731689f,  0.66468f },  // Rule 22: PS & PM & VH & VH
        {  0,  6,  3,  3,  0.074863f, -0.869963f,  0.65026f },  // Rule 23: NB & PB & HI & HI
        {  2,  4,  3,  3, -0.066827f, -0.021230f,  0.57822f },  // Rule 24: NS & PS & HI & HI
        {  5,  4,  4,  4, -0.218840f, -0.616051f,  0.49156f },  // Rule 25: PM & PS & VH & VH
        {  6,  0,  2,  3,  0.008173f,  0.973216f,  0.46291f },  // Rule 26: PB & NB & ME & HI
        {  4,  1,  4,  4,  0.170527f,  0.557821f,  0.44379f },  // Rule 27: PS & NM & VH & VH
        {  5,  2,  3,  3, -0.100056f,  0.083050f,  0.43436f },  // Rule 28: PM & NS & HI & HI
        {  0,  6,  3,  4,  0.283164f, -0.953393f,  0.40768f },  // Rule 29: NB & PB & HI & VH
        {  6,  1,  4,  4, -0.016478f,  0.770692f,  0.35155f },  // Rule 30: PB & NM & VH & VH
        {  0,  6,  1,  1,  0.016703f, -0.181389f,  0.33812f },  // Rule 31: NB & PB & LO & LO
        {  1,  5,  3,  3, -0.044858f, -0.044945f,  0.32511f },  // Rule 32: NM & PM & HI & HI
        {  6,  0,  3,  4, -0.151842f,  1.000000f,  0.31492f },  // Rule 33: PB & NB & HI & VH
        {  0,  6,  1,  2,  0.027301f, -0.366915f,  0.30374f },  // Rule 34: NB & PB & LO & ME
        {  6,  0,  1,  1, -0.140822f,  0.187762f,  0.30163f },  // Rule 35: PB & NB & LO & LO
        {  1,  6,  4,  4,  0.160362f, -0.756411f,  0.27471f },  // Rule 36: NM & PB & VH & VH
        {  6,  0,  4,  4,  0.013979f,  0.947795f,  0.26046f },  // Rule 37: PB & NB & VH & VH
        {  0,  6,  4,  4,  0.332850f, -0.934603f,  0.26022f },  // Rule 38: NB & PB & VH & VH
        {  6,  2,  4,  4, -0.359595f, -0.086010f,  0.24894f },  // Rule 39: PB & NS & VH & VH
        {  6,  1,  2,  2, -0.122628f,  0.172064f,  0.24834f },  // Rule 40: PB & NM & ME & ME
        {  6,  1,  3,  3, -0.193584f,  0.368138f,  0.24775f },  // Rule 41: PB & NM & HI & HI
        {  6,  0,  1,  2, -0.201478f,  0.304918f,  0.23417f },  // Rule 42: PB & NB & LO & ME
        {  5,  1,  3,  3, -0.120443f,  0.116520f,  0.20645f },  // Rule 43: PM & NM & HI & HI
        {  1,  6,  3,  3, -0.043322f, -0.159943f,  0.18818f },  // Rule 44: NM & PB & HI & HI
        {  2,  6,  4,  4,  0.044887f, -0.987314f,  0.18133f },  // Rule 45: NS & PB & VH & VH
        {  6,  1,  3,  4, -0.265648f,  0.756394f,  0.17511f },  // Rule 46: PB & NM & HI & VH
        {  1,  6,  2,  2, -0.015408f, -0.091305f,  0.17406f },  // Rule 47: NM & PB & ME & ME
        {  3,  6,  4,  4, -0.344460f, -0.580295f,  0.11846f },  // Rule 48: ZE & PB & VH & VH
        {  1,  6,  3,  4, -0.058226f, -0.196079f,  0.08739f },  // Rule 49: NM & PB & HI & VH
    };

    private const int N_RULES = 49;

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
        return $"FuzzyController: {N_RULES} rules, generated 2026-10-13 14:07:55";
    }
}