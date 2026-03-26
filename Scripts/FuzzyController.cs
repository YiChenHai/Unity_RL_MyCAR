// 模糊控制器 - 自动生成
// 生成时间: 2026-03-26 14:35:19
// 数据样本数: 300000
// 规则数: 54
// 输入变量: front_lr_diff, rear_lr_diff, front_center, rear_center
// 输出变量: output_vx_norm, output_omega_norm
//
// 隶属函数配置:
//   front_lr_diff: 5 集 (NB, NS, ZE, PS, PB)
//   rear_lr_diff: 5 集 (NB, NS, ZE, PS, PB)
//   front_center: 3 集 (LO, ME, HI)
//   rear_center: 3 集 (LO, ME, HI)

using System;
using UnityEngine;

public class FuzzyController
{
    // ===== 三角形隶属函数参数 =====
    // 每行: { left, center, right }

    // front_lr_diff: NB, NS, ZE, PS, PB
    private static readonly float[,] MF_FrontLR = {
        { -1.00000f, -1.00000f, -0.33977f },  // NB
        { -1.00000f, -0.33977f,  0.00000f },  // NS
        { -0.33977f,  0.00000f,  0.20877f },  // ZE
        {  0.00000f,  0.20877f,  0.41754f },  // PS
        {  0.20877f,  1.00000f,  1.00000f },  // PB
    };

    // rear_lr_diff: NB, NS, ZE, PS, PB
    private static readonly float[,] MF_RearLR = {
        { -1.00000f, -1.00000f, -0.34209f },  // NB
        { -1.00000f, -0.34209f,  0.00000f },  // NS
        { -0.34209f,  0.00000f,  0.20769f },  // ZE
        {  0.00000f,  0.20769f,  0.41538f },  // PS
        {  0.20769f,  1.00000f,  1.00000f },  // PB
    };

    // front_center: LO, ME, HI
    private static readonly float[,] MF_FrontCenter = {
        {  0.00000f,  0.00000f,  0.63037f },  // LO
        {  0.00000f,  0.63037f,  1.00000f },  // ME
        {  0.63037f,  1.00000f,  1.00000f },  // HI
    };

    // rear_center: LO, ME, HI
    private static readonly float[,] MF_RearCenter = {
        {  0.00000f,  0.00000f,  0.62981f },  // LO
        {  0.00000f,  0.62981f,  1.00000f },  // ME
        {  0.62981f,  1.00000f,  1.00000f },  // HI
    };

    // ===== 规则库 (54 条规则) =====
    // 每行: { 前LR集, 后LR集, 前中集, 后中集, 输出vx, 输出omega, 权重 }
    private static readonly float[,] Rules = {
        {  2,  2,  1,  1, -0.020243f, -0.076505f,  0.94544f },  // Rule 1: ZE & ZE & ME & ME
        {  1,  3,  1,  2, -0.063185f, -0.997129f,  0.84301f },  // Rule 2: NS & PS & ME & HI
        {  3,  1,  1,  1,  0.075254f,  0.498799f,  0.77835f },  // Rule 3: PS & NS & ME & ME
        {  3,  1,  1,  2,  0.445196f,  0.941018f,  0.64610f },  // Rule 4: PS & NS & ME & HI
        {  2,  2,  1,  2,  0.030428f, -0.040694f,  0.62445f },  // Rule 5: ZE & ZE & ME & HI
        {  3,  2,  2,  2, -0.053408f,  0.741272f,  0.61629f },  // Rule 6: PS & ZE & HI & HI
        {  1,  3,  1,  1, -0.055054f, -0.484652f,  0.61201f },  // Rule 7: NS & PS & ME & ME
        {  3,  2,  1,  2,  0.137575f,  0.953112f,  0.57706f },  // Rule 8: PS & ZE & ME & HI
        {  2,  1,  1,  2,  0.381806f, -0.004799f,  0.55570f },  // Rule 9: ZE & NS & ME & HI
        {  3,  3,  1,  2,  0.092568f,  0.548000f,  0.54301f },  // Rule 10: PS & PS & ME & HI
        {  2,  2,  2,  2,  0.015974f, -0.070651f,  0.53317f },  // Rule 11: ZE & ZE & HI & HI
        {  3,  1,  2,  2,  0.480547f,  0.906356f,  0.50101f },  // Rule 12: PS & NS & HI & HI
        {  2,  3,  2,  2, -0.428959f, -0.776506f,  0.48985f },  // Rule 13: ZE & PS & HI & HI
        {  3,  3,  2,  2, -0.605661f,  0.495997f,  0.48963f },  // Rule 14: PS & PS & HI & HI
        {  1,  2,  1,  2, -0.282356f, -0.953545f,  0.48820f },  // Rule 15: NS & ZE & ME & HI
        {  2,  2,  2,  1, -0.046972f, -0.055820f,  0.48737f },  // Rule 16: ZE & ZE & HI & ME
        {  2,  3,  1,  2, -0.198602f, -0.192516f,  0.47467f },  // Rule 17: ZE & PS & ME & HI
        {  2,  1,  2,  2,  0.372550f,  0.363988f,  0.47445f },  // Rule 18: ZE & NS & HI & HI
        {  2,  1,  2,  1,  0.747354f,  0.275386f,  0.45014f },  // Rule 19: ZE & NS & HI & ME
        {  2,  3,  2,  1, -0.247387f, -0.785676f,  0.44823f },  // Rule 20: ZE & PS & HI & ME
        {  3,  1,  2,  1, -0.008148f,  0.989893f,  0.44342f },  // Rule 21: PS & NS & HI & ME
        {  2,  3,  1,  1, -0.288364f, -0.374536f,  0.40141f },  // Rule 22: ZE & PS & ME & ME
        {  3,  3,  2,  1, -0.794541f,  0.347473f,  0.38740f },  // Rule 23: PS & PS & HI & ME
        {  3,  2,  1,  1,  0.013414f,  0.206736f,  0.37122f },  // Rule 24: PS & ZE & ME & ME
        {  3,  0,  1,  2,  0.696707f,  1.000000f,  0.35036f },  // Rule 25: PS & NB & ME & HI
        {  1,  1,  2,  1,  0.202867f, -0.171660f,  0.34829f },  // Rule 26: NS & NS & HI & ME
        {  3,  4,  2,  1, -0.737053f, -0.231370f,  0.31488f },  // Rule 27: PS & PB & HI & ME
        {  3,  2,  2,  1, -0.651191f,  0.949841f,  0.30575f },  // Rule 28: PS & ZE & HI & ME
        {  2,  4,  2,  1, -0.965750f, -0.941771f,  0.30300f },  // Rule 29: ZE & PB & HI & ME
        {  3,  4,  1,  1, -0.748033f, -0.706312f,  0.30153f },  // Rule 30: PS & PB & ME & ME
        {  1,  1,  1,  1,  0.736690f, -0.420802f,  0.29307f },  // Rule 31: NS & NS & ME & ME
        {  1,  4,  1,  2, -0.111149f, -0.977875f,  0.28677f },  // Rule 32: NS & PB & ME & HI
        {  1,  2,  2,  2,  0.815789f, -0.981041f,  0.27425f },  // Rule 33: NS & ZE & HI & HI
        {  1,  1,  1,  2,  0.282855f, -0.780241f,  0.26261f },  // Rule 34: NS & NS & ME & HI
        {  4,  2,  1,  2, -0.697712f,  0.993082f,  0.25641f },  // Rule 35: PB & ZE & ME & HI
        {  4,  3,  1,  2, -0.916156f,  0.999379f,  0.24246f },  // Rule 36: PB & PS & ME & HI
        {  3,  3,  1,  1, -0.685060f,  0.437625f,  0.24198f },  // Rule 37: PS & PS & ME & ME
        {  1,  3,  2,  1, -0.025344f, -0.993079f,  0.24098f },  // Rule 38: NS & PS & HI & ME
        {  1,  4,  1,  1, -0.088640f, -0.641917f,  0.23715f },  // Rule 39: NS & PB & ME & ME
        {  2,  1,  1,  1,  0.358866f,  0.272912f,  0.23341f },  // Rule 40: ZE & NS & ME & ME
        {  1,  1,  2,  2,  0.044847f, -0.355577f,  0.22306f },  // Rule 41: NS & NS & HI & HI
        {  4,  3,  1,  1, -0.987902f,  1.000000f,  0.21106f },  // Rule 42: PB & PS & ME & ME
        {  1,  3,  2,  2, -0.190632f, -0.999981f,  0.20674f },  // Rule 43: NS & PS & HI & HI
        {  1,  2,  2,  1,  0.962730f, -0.981536f,  0.20030f },  // Rule 44: NS & ZE & HI & ME
        {  4,  1,  1,  2,  0.200560f,  0.940902f,  0.18801f },  // Rule 45: PB & NS & ME & HI
        {  2,  4,  2,  2, -0.491610f, -0.374884f,  0.18607f },  // Rule 46: ZE & PB & HI & HI
        {  4,  1,  1,  1, -0.028371f,  0.757569f,  0.18096f },  // Rule 47: PB & NS & ME & ME
        {  4,  2,  2,  2, -0.568036f,  0.995961f,  0.17847f },  // Rule 48: PB & ZE & HI & HI
        {  2,  4,  1,  1, -0.770702f, -0.842556f,  0.17826f },  // Rule 49: ZE & PB & ME & ME
        {  4,  1,  2,  1,  0.374029f,  1.000000f,  0.13792f },  // Rule 50: PB & NS & HI & ME
        {  4,  4,  2,  1, -0.741182f,  0.566453f,  0.13785f },  // Rule 51: PB & PB & HI & ME
        {  4,  3,  2,  2, -0.533939f,  1.000000f,  0.13039f },  // Rule 52: PB & PS & HI & HI
        {  3,  4,  1,  2, -0.228877f, -0.126061f,  0.11160f },  // Rule 53: PS & PB & ME & HI
        {  1,  4,  2,  2, -0.408901f, -0.669861f,  0.10953f },  // Rule 54: NS & PB & HI & HI
    };

    private const int N_RULES = 54;

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
        return $"FuzzyController: {N_RULES} rules, generated 2026-03-26 14:35:19";
    }
}