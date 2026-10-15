// 模糊控制器 - 自动生成
// 生成时间: 2026-10-15 17:18:46
// 数据样本数: 300000
// 规则数: 53
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
        { -1.00000f, -1.00000f, -0.27092f },  // NB
        { -1.00000f, -0.27092f, -0.13546f },  // NM
        { -0.27092f, -0.13546f,  0.00000f },  // NS
        { -0.13546f,  0.00000f,  0.09757f },  // ZE
        {  0.00000f,  0.09757f,  0.19514f },  // PS
        {  0.09757f,  0.19514f,  0.29270f },  // PM
        {  0.19514f,  1.00000f,  1.00000f },  // PB
    };

    // rear_lr_diff: NB, NM, NS, ZE, PS, PM, PB
    private static readonly float[,] MF_RearLR = {
        { -1.00000f, -1.00000f, -0.27014f },  // NB
        { -1.00000f, -0.27014f, -0.13507f },  // NM
        { -0.27014f, -0.13507f,  0.00000f },  // NS
        { -0.13507f,  0.00000f,  0.09712f },  // ZE
        {  0.00000f,  0.09712f,  0.19424f },  // PS
        {  0.09712f,  0.19424f,  0.29137f },  // PM
        {  0.19424f,  1.00000f,  1.00000f },  // PB
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

    // ===== 规则库 (53 条规则) =====
    // 每行: { 前LR集, 后LR集, 前中集, 后中集, 输出vx, 输出omega, 权重 }
    private static readonly float[,] Rules = {
        {  1,  2,  4,  4,  0.766282f, -0.557519f,  0.99940f },  // Rule 1: NM & NS & VH & VH
        {  3,  4,  4,  4, -0.091858f, -0.245734f,  0.99937f },  // Rule 2: ZE & PS & VH & VH
        {  4,  3,  4,  4, -0.094512f,  0.267538f,  0.99903f },  // Rule 3: PS & ZE & VH & VH
        {  4,  2,  4,  4,  0.131933f,  0.524774f,  0.99798f },  // Rule 4: PS & NS & VH & VH
        {  2,  4,  4,  4,  0.116398f, -0.522368f,  0.99783f },  // Rule 5: NS & PS & VH & VH
        {  3,  3,  4,  4,  0.014820f, -0.000774f,  0.99651f },  // Rule 6: ZE & ZE & VH & VH
        {  5,  2,  4,  4, -0.053844f,  0.703532f,  0.99582f },  // Rule 7: PM & NS & VH & VH
        {  1,  3,  4,  4,  0.586217f, -0.786938f,  0.99574f },  // Rule 8: NM & ZE & VH & VH
        {  2,  3,  4,  4,  0.278177f, -0.369109f,  0.99546f },  // Rule 9: NS & ZE & VH & VH
        {  5,  3,  4,  4, -0.287262f,  0.490981f,  0.99519f },  // Rule 10: PM & ZE & VH & VH
        {  3,  5,  4,  4, -0.264586f, -0.446241f,  0.99346f },  // Rule 11: ZE & PM & VH & VH
        {  5,  4,  4,  4, -0.462223f,  0.299001f,  0.99324f },  // Rule 12: PM & PS & VH & VH
        {  4,  1,  4,  4,  0.370400f,  0.764169f,  0.99021f },  // Rule 13: PS & NM & VH & VH
        {  4,  4,  4,  4, -0.284378f,  0.008362f,  0.98994f },  // Rule 14: PS & PS & VH & VH
        {  2,  5,  4,  4, -0.078473f, -0.660261f,  0.98936f },  // Rule 15: NS & PM & VH & VH
        {  3,  1,  4,  4,  0.545821f,  0.709394f,  0.98868f },  // Rule 16: ZE & NM & VH & VH
        {  3,  2,  4,  4,  0.250433f,  0.326926f,  0.98628f },  // Rule 17: ZE & NS & VH & VH
        {  2,  2,  4,  4,  0.479743f, -0.061187f,  0.97771f },  // Rule 18: NS & NS & VH & VH
        {  5,  5,  4,  4, -0.597621f,  0.029812f,  0.97508f },  // Rule 19: PM & PM & VH & VH
        {  1,  4,  4,  4,  0.349926f, -0.786697f,  0.97488f },  // Rule 20: NM & PS & VH & VH
        {  5,  1,  4,  4,  0.252076f,  0.991607f,  0.97447f },  // Rule 21: PM & NM & VH & VH
        {  4,  5,  4,  4, -0.467905f, -0.180655f,  0.96710f },  // Rule 22: PS & PM & VH & VH
        {  1,  5,  4,  4,  0.260043f, -0.973787f,  0.95567f },  // Rule 23: NM & PM & VH & VH
        {  6,  0,  2,  2, -0.935993f,  1.000000f,  0.94140f },  // Rule 24: PB & NB & ME & ME
        {  1,  1,  4,  4,  0.960807f, -0.422011f,  0.93398f },  // Rule 25: NM & NM & VH & VH
        {  2,  1,  4,  4,  0.787388f,  0.439680f,  0.90888f },  // Rule 26: NS & NM & VH & VH
        {  0,  6,  2,  2,  0.512080f, -1.000000f,  0.83379f },  // Rule 27: NB & PB & ME & ME
        {  4,  0,  4,  3,  0.774195f,  1.000000f,  0.74810f },  // Rule 28: PS & NB & VH & HI
        {  5,  0,  4,  3,  0.690879f,  1.000000f,  0.72427f },  // Rule 29: PM & NB & VH & HI
        {  6,  0,  3,  3, -0.793793f,  1.000000f,  0.66674f },  // Rule 30: PB & NB & HI & HI
        {  0,  6,  3,  3,  0.432624f, -1.000000f,  0.64630f },  // Rule 31: NB & PB & HI & HI
        {  6,  0,  3,  2, -0.817700f,  1.000000f,  0.62151f },  // Rule 32: PB & NB & HI & ME
        {  3,  0,  4,  3,  0.854500f,  1.000000f,  0.59643f },  // Rule 33: ZE & NB & VH & HI
        {  1,  6,  4,  4, -0.029796f, -0.999997f,  0.46287f },  // Rule 34: NM & PB & VH & VH
        {  1,  6,  4,  3,  0.080694f, -1.000000f,  0.42643f },  // Rule 35: NM & PB & VH & HI
        {  0,  6,  3,  2,  0.560641f, -1.000000f,  0.41696f },  // Rule 36: NB & PB & HI & ME
        {  2,  6,  4,  4, -0.368792f, -0.894095f,  0.39547f },  // Rule 37: NS & PB & VH & VH
        {  3,  6,  4,  4, -0.562384f, -0.767080f,  0.38702f },  // Rule 38: ZE & PB & VH & VH
        {  4,  6,  4,  4, -0.765447f, -0.579486f,  0.38565f },  // Rule 39: PS & PB & VH & VH
        {  6,  0,  2,  3, -0.931524f,  1.000000f,  0.37228f },  // Rule 40: PB & NB & ME & HI
        {  0,  6,  4,  3,  0.362841f, -1.000000f,  0.36522f },  // Rule 41: NB & PB & VH & HI
        {  6,  0,  4,  3,  0.182677f,  1.000000f,  0.34709f },  // Rule 42: PB & NB & VH & HI
        {  6,  1,  4,  4, -0.035158f,  1.000000f,  0.34551f },  // Rule 43: PB & NM & VH & VH
        {  6,  4,  4,  4, -0.807866f,  0.789416f,  0.33894f },  // Rule 44: PB & PS & VH & VH
        {  6,  5,  4,  4, -0.955737f,  0.725822f,  0.33818f },  // Rule 45: PB & PM & VH & VH
        {  5,  0,  4,  4,  0.624910f,  1.000000f,  0.31242f },  // Rule 46: PM & NB & VH & VH
        {  0,  6,  2,  3,  0.492877f, -1.000000f,  0.29652f },  // Rule 47: NB & PB & ME & HI
        {  6,  2,  4,  4, -0.294131f,  0.992504f,  0.28339f },  // Rule 48: PB & NS & VH & VH
        {  6,  3,  4,  4, -0.613252f,  0.857098f,  0.27320f },  // Rule 49: PB & ZE & VH & VH
        {  3,  0,  4,  4,  0.851801f,  1.000000f,  0.23729f },  // Rule 50: ZE & NB & VH & VH
        {  5,  6,  4,  4, -0.846823f, -0.341063f,  0.22689f },  // Rule 51: PM & PB & VH & VH
        {  6,  0,  4,  4, -0.202771f,  1.000000f,  0.12625f },  // Rule 52: PB & NB & VH & VH
        {  0,  6,  4,  4,  0.219129f, -1.000000f,  0.12573f },  // Rule 53: NB & PB & VH & VH
    };

    private const int N_RULES = 53;

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
        return $"FuzzyController: {N_RULES} rules, generated 2026-10-15 17:18:46";
    }
}