// 模糊控制器 - 自动生成
// 生成时间: 2026-11-02 17:34:43
// 数据样本数: 300000
// 规则数: 61 (数据提取 55, 补全 6)
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
        { -1.00000f, -1.00000f, -0.33333f },  // NB
        { -1.00000f, -0.33333f, -0.16667f },  // NM
        { -0.33333f, -0.16667f,  0.00000f },  // NS
        { -0.16667f,  0.00000f,  0.16667f },  // ZE
        {  0.00000f,  0.16667f,  0.33333f },  // PS
        {  0.16667f,  0.33333f,  1.00000f },  // PM
        {  0.33333f,  1.00000f,  1.00000f },  // PB
    };

    // rear_lr_diff: NB, NM, NS, ZE, PS, PM, PB
    private static readonly float[,] MF_RearLR = {
        { -1.00000f, -1.00000f, -0.33333f },  // NB
        { -1.00000f, -0.33333f, -0.16667f },  // NM
        { -0.33333f, -0.16667f,  0.00000f },  // NS
        { -0.16667f,  0.00000f,  0.16667f },  // ZE
        {  0.00000f,  0.16667f,  0.33333f },  // PS
        {  0.16667f,  0.33333f,  1.00000f },  // PM
        {  0.33333f,  1.00000f,  1.00000f },  // PB
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

    // ===== 规则库 (61 条规则) =====
    // 每行: { 前LR集, 后LR集, 前中集, 后中集, 输出vx, 输出omega, 权重 }
    private static readonly float[,] Rules = {
        {  3,  3,  4,  4,  0.009507f, -0.057225f,  0.99619f },  // Rule 1: ZE & ZE & VH & VH
        {  3,  2,  4,  4,  0.311317f,  0.521880f,  0.99569f },  // Rule 2: ZE & NS & VH & VH
        {  1,  1,  4,  4,  0.977404f, -0.039017f,  0.99557f },  // Rule 3: NM & NM & VH & VH
        {  4,  4,  4,  4, -0.425656f, -0.345967f,  0.99350f },  // Rule 4: PS & PS & VH & VH
        {  5,  5,  4,  4, -0.669367f, -0.309897f,  0.99269f },  // Rule 5: PM & PM & VH & VH
        {  4,  3,  4,  4, -0.183703f,  0.391602f,  0.99267f },  // Rule 6: PS & ZE & VH & VH
        {  1,  2,  4,  4,  0.722171f, -0.870592f,  0.99252f },  // Rule 7: NM & NS & VH & VH
        {  5,  4,  4,  4, -0.589268f,  0.740372f,  0.99128f },  // Rule 8: PM & PS & VH & VH
        {  1,  4,  4,  4,  0.204593f, -1.000000f,  0.99062f },  // Rule 9: NM & PS & VH & VH
        {  2,  2,  4,  4,  0.549710f, -0.184567f,  0.99017f },  // Rule 10: NS & NS & VH & VH
        {  4,  2,  4,  4,  0.098583f,  0.870082f,  0.98994f },  // Rule 11: PS & NS & VH & VH
        {  3,  4,  4,  4, -0.273064f, -0.611440f,  0.98664f },  // Rule 12: ZE & PS & VH & VH
        {  5,  1,  4,  4, -0.088261f,  1.000000f,  0.98530f },  // Rule 13: PM & NM & VH & VH
        {  2,  4,  4,  4, -0.108165f, -0.999862f,  0.98352f },  // Rule 14: NS & PS & VH & VH
        {  2,  1,  4,  4,  0.782383f,  0.537601f,  0.98163f },  // Rule 15: NS & NM & VH & VH
        {  2,  3,  4,  4,  0.288777f, -0.555139f,  0.97841f },  // Rule 16: NS & ZE & VH & VH
        {  5,  2,  4,  4, -0.309674f,  1.000000f,  0.96957f },  // Rule 17: PM & NS & VH & VH
        {  1,  3,  4,  4,  0.582956f, -0.990810f,  0.96547f },  // Rule 18: NM & ZE & VH & VH
        {  4,  5,  4,  4, -0.547263f, -0.768780f,  0.96014f },  // Rule 19: PS & PM & VH & VH
        {  5,  3,  4,  4, -0.467715f,  0.836134f,  0.95959f },  // Rule 20: PM & ZE & VH & VH
        {  1,  5,  4,  4, -0.034006f, -1.000000f,  0.95475f },  // Rule 21: NM & PM & VH & VH
        {  3,  1,  4,  4,  0.647175f,  0.848622f,  0.95375f },  // Rule 22: ZE & NM & VH & VH
        {  4,  1,  4,  4,  0.313321f,  1.000000f,  0.95021f },  // Rule 23: PS & NM & VH & VH
        {  3,  5,  4,  4, -0.450477f, -0.984921f,  0.93458f },  // Rule 24: ZE & PM & VH & VH
        {  2,  5,  4,  4, -0.318522f, -1.000000f,  0.92593f },  // Rule 25: NS & PM & VH & VH
        {  0,  5,  3,  4,  0.254257f, -1.000000f,  0.78509f },  // Rule 26: NB & PM & HI & VH
        {  6,  4,  3,  4, -0.791367f,  1.000000f,  0.76233f },  // Rule 27: PB & PS & HI & VH
        {  6,  3,  3,  4, -0.827421f,  1.000000f,  0.75609f },  // Rule 28: PB & ZE & HI & VH
        {  6,  2,  3,  4, -0.839736f,  1.000000f,  0.75064f },  // Rule 29: PB & NS & HI & VH
        {  0,  4,  3,  4,  0.534322f, -1.000000f,  0.74406f },  // Rule 30: NB & PS & HI & VH
        {  0,  2,  3,  4,  0.942776f, -1.000000f,  0.73533f },  // Rule 31: NB & NS & HI & VH
        {  6,  5,  3,  4, -0.754896f,  0.893280f,  0.71869f },  // Rule 32: PB & PM & HI & VH
        {  6,  1,  3,  4, -0.757614f,  1.000000f,  0.68687f },  // Rule 33: PB & NM & HI & VH
        {  0,  1,  3,  4,  1.000000f, -0.997793f,  0.63803f },  // Rule 34: NB & NM & HI & VH
        {  6,  0,  3,  3, -0.525958f,  1.000000f,  0.57767f },  // Rule 35: PB & NB & HI & HI
        {  0,  6,  3,  3,  0.028063f, -1.000000f,  0.56471f },  // Rule 36: NB & PB & HI & HI
        {  6,  1,  4,  4, -0.597575f,  1.000000f,  0.51955f },  // Rule 37: PB & NM & VH & VH
        {  0,  3,  3,  4,  0.747128f, -1.000000f,  0.41330f },  // Rule 38: NB & ZE & HI & VH
        {  6,  5,  2,  4, -0.620865f,  0.852214f,  0.40860f },  // Rule 39: PB & PM & ME & VH
        {  0,  1,  2,  4,  0.620865f, -0.852214f,  0.40860f },  // Rule 40: NB & NM & ME & VH  [补全]
        {  6,  5,  4,  4, -0.820041f,  0.704768f,  0.39503f },  // Rule 41: PB & PM & VH & VH
        {  0,  1,  4,  4,  1.000000f, -0.917890f,  0.34909f },  // Rule 42: NB & NM & VH & VH
        {  5,  6,  4,  4, -0.576881f, -0.595986f,  0.33986f },  // Rule 43: PM & PB & VH & VH
        {  1,  0,  4,  4,  0.576881f,  0.595986f,  0.33986f },  // Rule 44: NM & NB & VH & VH  [补全]
        {  6,  6,  3,  3, -0.582207f,  0.029163f,  0.32336f },  // Rule 45: PB & PB & HI & HI
        {  0,  0,  3,  3,  0.582207f, -0.029163f,  0.32336f },  // Rule 46: NB & NB & HI & HI  [补全]
        {  6,  2,  4,  4, -0.771406f,  1.000000f,  0.31673f },  // Rule 47: PB & NS & VH & VH
        {  6,  3,  4,  4, -0.817523f,  1.000000f,  0.31176f },  // Rule 48: PB & ZE & VH & VH
        {  0,  5,  4,  4,  0.129371f, -1.000000f,  0.29964f },  // Rule 49: NB & PM & VH & VH
        {  0,  4,  4,  4,  0.461113f, -1.000000f,  0.29855f },  // Rule 50: NB & PS & VH & VH
        {  6,  4,  4,  4, -0.813538f,  1.000000f,  0.28656f },  // Rule 51: PB & PS & VH & VH
        {  0,  3,  4,  4,  0.737361f, -1.000000f,  0.28568f },  // Rule 52: NB & ZE & VH & VH
        {  0,  2,  4,  4,  0.919244f, -1.000000f,  0.27995f },  // Rule 53: NB & NS & VH & VH
        {  6,  0,  3,  4, -0.606592f,  1.000000f,  0.26105f },  // Rule 54: PB & NB & HI & VH
        {  0,  6,  3,  4, -0.082615f, -1.000000f,  0.17086f },  // Rule 55: NB & PB & HI & VH
        {  5,  0,  4,  4, -0.282229f,  1.000000f,  0.15689f },  // Rule 56: PM & NB & VH & VH
        {  1,  6,  4,  4,  0.282229f, -1.000000f,  0.15689f },  // Rule 57: NM & PB & VH & VH  [补全]
        {  5,  6,  4,  3, -0.596479f, -0.248491f,  0.14268f },  // Rule 58: PM & PB & VH & HI
        {  1,  0,  4,  3,  0.596479f,  0.248491f,  0.14268f },  // Rule 59: NM & NB & VH & HI  [补全]
        {  6,  0,  4,  4, -0.423962f,  1.000000f,  0.08882f },  // Rule 60: PB & NB & VH & VH
        {  0,  6,  4,  4,  0.423962f, -1.000000f,  0.08882f },  // Rule 61: NB & PB & VH & VH  [补全]
    };

    private const int N_RULES = 61;

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
        return $"FuzzyController: {N_RULES} rules, generated 2026-11-02 17:34:43";
    }
}