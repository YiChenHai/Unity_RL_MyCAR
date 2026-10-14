// 模糊控制器 - 自动生成
// 生成时间: 2026-10-14 19:32:00
// 数据样本数: 300000
// 规则数: 48
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
        { -1.00000f, -1.00000f, -0.25947f },  // NB
        { -1.00000f, -0.25947f, -0.12974f },  // NM
        { -0.25947f, -0.12974f,  0.00000f },  // NS
        { -0.12974f,  0.00000f,  0.09379f },  // ZE
        {  0.00000f,  0.09379f,  0.18757f },  // PS
        {  0.09379f,  0.18757f,  0.28136f },  // PM
        {  0.18757f,  1.00000f,  1.00000f },  // PB
    };

    // rear_lr_diff: NB, NM, NS, ZE, PS, PM, PB
    private static readonly float[,] MF_RearLR = {
        { -1.00000f, -1.00000f, -0.26040f },  // NB
        { -1.00000f, -0.26040f, -0.13020f },  // NM
        { -0.26040f, -0.13020f,  0.00000f },  // NS
        { -0.13020f,  0.00000f,  0.09646f },  // ZE
        {  0.00000f,  0.09646f,  0.19292f },  // PS
        {  0.09646f,  0.19292f,  0.28939f },  // PM
        {  0.19292f,  1.00000f,  1.00000f },  // PB
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

    // ===== 规则库 (48 条规则) =====
    // 每行: { 前LR集, 后LR集, 前中集, 后中集, 输出vx, 输出omega, 权重 }
    private static readonly float[,] Rules = {
        {  3,  1,  4,  4,  0.649784f,  0.838373f,  0.99964f },  // Rule 1: ZE & NM & VH & VH
        {  3,  3,  4,  4,  0.008069f, -0.008771f,  0.99871f },  // Rule 2: ZE & ZE & VH & VH
        {  3,  4,  4,  4, -0.129460f, -0.398143f,  0.99782f },  // Rule 3: ZE & PS & VH & VH
        {  4,  3,  4,  4, -0.237091f,  0.323978f,  0.99737f },  // Rule 4: PS & ZE & VH & VH
        {  3,  5,  4,  4, -0.415853f, -0.746240f,  0.99628f },  // Rule 5: ZE & PM & VH & VH
        {  4,  4,  4,  4, -0.346528f,  0.092318f,  0.99625f },  // Rule 6: PS & PS & VH & VH
        {  4,  2,  4,  4,  0.066367f,  0.599844f,  0.99537f },  // Rule 7: PS & NS & VH & VH
        {  2,  2,  4,  4,  0.392956f, -0.025978f,  0.99422f },  // Rule 8: NS & NS & VH & VH
        {  2,  1,  4,  4,  0.686092f,  0.403015f,  0.99310f },  // Rule 9: NS & NM & VH & VH
        {  3,  2,  4,  4,  0.216392f,  0.349840f,  0.98725f },  // Rule 10: ZE & NS & VH & VH
        {  5,  2,  4,  4, -0.153972f,  0.739360f,  0.98653f },  // Rule 11: PM & NS & VH & VH
        {  2,  4,  4,  4,  0.154224f, -0.609549f,  0.98528f },  // Rule 12: NS & PS & VH & VH
        {  5,  4,  4,  4, -0.649182f,  0.592245f,  0.98440f },  // Rule 13: PM & PS & VH & VH
        {  2,  3,  4,  4,  0.410784f, -0.504929f,  0.98337f },  // Rule 14: NS & ZE & VH & VH
        {  5,  1,  4,  4,  0.122530f,  0.826029f,  0.98136f },  // Rule 15: PM & NM & VH & VH
        {  4,  5,  4,  4, -0.469326f, -0.345809f,  0.97676f },  // Rule 16: PS & PM & VH & VH
        {  5,  3,  4,  4, -0.419998f,  0.651273f,  0.97674f },  // Rule 17: PM & ZE & VH & VH
        {  2,  5,  4,  4, -0.024186f, -0.744051f,  0.97650f },  // Rule 18: NS & PM & VH & VH
        {  1,  3,  4,  4,  0.623141f, -0.753062f,  0.97575f },  // Rule 19: NM & ZE & VH & VH
        {  1,  4,  4,  4,  0.403801f, -0.634149f,  0.97489f },  // Rule 20: NM & PS & VH & VH
        {  1,  5,  4,  4,  0.250121f, -0.591335f,  0.96799f },  // Rule 21: NM & PM & VH & VH
        {  1,  2,  4,  4,  0.746005f, -0.443509f,  0.96398f },  // Rule 22: NM & NS & VH & VH
        {  0,  6,  2,  2,  0.120222f, -0.409419f,  0.96234f },  // Rule 23: NB & PB & ME & ME
        {  4,  1,  4,  4,  0.465679f,  0.851511f,  0.96042f },  // Rule 24: PS & NM & VH & VH
        {  5,  5,  4,  4, -0.658706f,  0.007599f,  0.95391f },  // Rule 25: PM & PM & VH & VH
        {  6,  0,  2,  2, -0.284741f,  0.525904f,  0.95193f },  // Rule 26: PB & NB & ME & ME
        {  1,  1,  4,  4,  0.627445f, -0.213104f,  0.80909f },  // Rule 27: NM & NM & VH & VH
        {  6,  0,  3,  3, -0.378705f,  0.759245f,  0.65179f },  // Rule 28: PB & NB & HI & HI
        {  6,  0,  2,  3, -0.521359f,  0.905671f,  0.64609f },  // Rule 29: PB & NB & ME & HI
        {  0,  6,  3,  3,  0.158292f, -0.715612f,  0.63619f },  // Rule 30: NB & PB & HI & HI
        {  6,  0,  3,  2, -0.502858f,  0.965235f,  0.39749f },  // Rule 31: PB & NB & HI & ME
        {  0,  6,  2,  3,  0.167042f, -0.854072f,  0.39533f },  // Rule 32: NB & PB & ME & HI
        {  6,  0,  4,  3, -0.252662f,  1.000000f,  0.36659f },  // Rule 33: PB & NB & VH & HI
        {  2,  6,  4,  4, -0.376819f, -0.840047f,  0.32869f },  // Rule 34: NS & PB & VH & VH
        {  6,  1,  4,  4, -0.277196f,  0.703610f,  0.32592f },  // Rule 35: PB & NM & VH & VH
        {  1,  6,  4,  4,  0.059698f, -0.764454f,  0.31130f },  // Rule 36: NM & PB & VH & VH
        {  3,  6,  4,  4, -0.669318f, -0.877415f,  0.29911f },  // Rule 37: ZE & PB & VH & VH
        {  6,  0,  4,  4, -0.119338f,  0.830427f,  0.26571f },  // Rule 38: PB & NB & VH & VH
        {  5,  6,  4,  4, -0.904221f, -0.558907f,  0.25755f },  // Rule 39: PM & PB & VH & VH
        {  6,  2,  4,  4, -0.402372f,  0.496383f,  0.25600f },  // Rule 40: PB & NS & VH & VH
        {  4,  6,  4,  4, -0.747771f, -0.742268f,  0.23952f },  // Rule 41: PS & PB & VH & VH
        {  6,  3,  4,  4, -0.603435f,  0.833961f,  0.20173f },  // Rule 42: PB & ZE & VH & VH
        {  0,  6,  3,  4,  0.208764f, -0.963507f,  0.18853f },  // Rule 43: NB & PB & HI & VH
        {  6,  4,  4,  4, -0.834417f,  0.975879f,  0.16556f },  // Rule 44: PB & PS & VH & VH
        {  6,  5,  4,  4, -0.923949f,  0.806155f,  0.16172f },  // Rule 45: PB & PM & VH & VH
        {  0,  6,  4,  4,  0.196571f, -0.976827f,  0.12874f },  // Rule 46: NB & PB & VH & VH
        {  6,  0,  3,  4, -0.378634f,  0.985677f,  0.12462f },  // Rule 47: PB & NB & HI & VH
        {  6,  6,  4,  4, -0.998474f,  0.750214f,  0.05138f },  // Rule 48: PB & PB & VH & VH
    };

    private const int N_RULES = 48;

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
        return $"FuzzyController: {N_RULES} rules, generated 2026-10-14 19:32:00";
    }
}