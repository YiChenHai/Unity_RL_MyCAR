// 模糊控制器 - 自动生成
// 生成时间: 2026-10-11 13:35:34
// 数据样本数: 300000
// 规则数: 70
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
        { -1.00000f, -1.00000f, -0.25744f },  // NB
        { -1.00000f, -0.25744f, -0.12872f },  // NM
        { -0.25744f, -0.12872f,  0.00000f },  // NS
        { -0.12872f,  0.00000f,  0.09401f },  // ZE
        {  0.00000f,  0.09401f,  0.18802f },  // PS
        {  0.09401f,  0.18802f,  0.28204f },  // PM
        {  0.18802f,  1.00000f,  1.00000f },  // PB
    };

    // rear_lr_diff: NB, NM, NS, ZE, PS, PM, PB
    private static readonly float[,] MF_RearLR = {
        { -1.00000f, -1.00000f, -0.26116f },  // NB
        { -1.00000f, -0.26116f, -0.13058f },  // NM
        { -0.26116f, -0.13058f,  0.00000f },  // NS
        { -0.13058f,  0.00000f,  0.09679f },  // ZE
        {  0.00000f,  0.09679f,  0.19359f },  // PS
        {  0.09679f,  0.19359f,  0.29038f },  // PM
        {  0.19359f,  1.00000f,  1.00000f },  // PB
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

    // ===== 规则库 (70 条规则) =====
    // 每行: { 前LR集, 后LR集, 前中集, 后中集, 输出vx, 输出omega, 权重 }
    private static readonly float[,] Rules = {
        {  3,  3,  4,  4,  0.017127f, -0.061600f,  0.99956f },  // Rule 1: ZE & ZE & VH & VH
        {  1,  4,  4,  4,  0.712781f, -0.840277f,  0.99757f },  // Rule 2: NM & PS & VH & VH
        {  3,  2,  4,  4,  0.126139f,  0.372242f,  0.99680f },  // Rule 3: ZE & NS & VH & VH
        {  2,  3,  4,  4,  0.424115f, -0.427319f,  0.99503f },  // Rule 4: NS & ZE & VH & VH
        {  4,  3,  4,  4, -0.204899f,  0.127756f,  0.99420f },  // Rule 5: PS & ZE & VH & VH
        {  3,  4,  4,  4, -0.201352f, -0.682046f,  0.99187f },  // Rule 6: ZE & PS & VH & VH
        {  5,  2,  4,  4, -0.729963f,  0.851565f,  0.99130f },  // Rule 7: PM & NS & VH & VH
        {  5,  1,  4,  4, -0.253528f,  0.787969f,  0.99114f },  // Rule 8: PM & NM & VH & VH
        {  4,  2,  4,  4, -0.243522f,  0.535490f,  0.98846f },  // Rule 9: PS & NS & VH & VH
        {  2,  1,  4,  4,  0.607308f,  0.666576f,  0.98539f },  // Rule 10: NS & NM & VH & VH
        {  5,  3,  4,  4, -0.642528f,  0.660889f,  0.98316f },  // Rule 11: PM & ZE & VH & VH
        {  1,  1,  4,  4,  0.553590f, -0.400334f,  0.97945f },  // Rule 12: NM & NM & VH & VH
        {  2,  5,  4,  4,  0.357999f, -0.930116f,  0.97836f },  // Rule 13: NS & PM & VH & VH
        {  3,  1,  4,  4,  0.565063f,  0.810480f,  0.97690f },  // Rule 14: ZE & NM & VH & VH
        {  2,  2,  4,  4,  0.448515f,  0.293169f,  0.97678f },  // Rule 15: NS & NS & VH & VH
        {  1,  5,  4,  4,  0.548317f, -0.686009f,  0.97498f },  // Rule 16: NM & PM & VH & VH
        {  4,  4,  4,  4, -0.396297f, -0.498478f,  0.96878f },  // Rule 17: PS & PS & VH & VH
        {  4,  1,  4,  4, -0.108521f,  0.585180f,  0.96789f },  // Rule 18: PS & NM & VH & VH
        {  1,  2,  4,  4,  0.468745f, -0.170129f,  0.96409f },  // Rule 19: NM & NS & VH & VH
        {  1,  3,  4,  4,  0.658823f, -0.576283f,  0.96000f },  // Rule 20: NM & ZE & VH & VH
        {  5,  5,  4,  4, -0.463607f, -0.372751f,  0.94613f },  // Rule 21: PM & PM & VH & VH
        {  2,  4,  4,  4,  0.331023f, -0.841391f,  0.94140f },  // Rule 22: NS & PS & VH & VH
        {  3,  5,  4,  4, -0.571080f, -0.850717f,  0.93568f },  // Rule 23: ZE & PM & VH & VH
        {  5,  4,  4,  4, -0.407148f, -0.206645f,  0.93352f },  // Rule 24: PM & PS & VH & VH
        {  4,  5,  4,  4, -0.660780f, -0.823793f,  0.93125f },  // Rule 25: PS & PM & VH & VH
        {  6,  0,  2,  2, -0.377511f,  0.578377f,  0.85401f },  // Rule 26: PB & NB & ME & ME
        {  6,  0,  3,  3, -0.518197f,  0.930521f,  0.83407f },  // Rule 27: PB & NB & HI & HI
        {  0,  6,  2,  2,  0.012136f, -0.587876f,  0.83291f },  // Rule 28: NB & PB & ME & ME
        {  0,  6,  2,  3,  0.184340f, -0.953426f,  0.80905f },  // Rule 29: NB & PB & ME & HI
        {  0,  6,  3,  3,  0.163564f, -0.757194f,  0.64606f },  // Rule 30: NB & PB & HI & HI
        {  2,  4,  3,  3, -0.060567f,  0.012914f,  0.62744f },  // Rule 31: NS & PS & HI & HI
        {  3,  3,  3,  3, -0.045672f,  0.021716f,  0.60833f },  // Rule 32: ZE & ZE & HI & HI
        {  4,  2,  3,  3, -0.076523f,  0.032146f,  0.52456f },  // Rule 33: PS & NS & HI & HI
        {  5,  2,  3,  3, -0.070585f,  0.037711f,  0.43212f },  // Rule 34: PM & NS & HI & HI
        {  6,  1,  4,  4, -0.601434f,  0.733716f,  0.42063f },  // Rule 35: PB & NM & VH & VH
        {  6,  0,  4,  3, -0.363365f,  1.000000f,  0.41934f },  // Rule 36: PB & NB & VH & HI
        {  6,  0,  2,  3, -0.511506f,  0.863629f,  0.41373f },  // Rule 37: PB & NB & ME & HI
        {  1,  6,  4,  4,  0.309881f, -0.791474f,  0.39254f },  // Rule 38: NM & PB & VH & VH
        {  6,  2,  4,  4, -0.704785f,  0.395188f,  0.38410f },  // Rule 39: PB & NS & VH & VH
        {  0,  6,  3,  4,  0.483671f, -0.954012f,  0.37668f },  // Rule 40: NB & PB & HI & VH
        {  6,  5,  4,  4, -0.264892f,  0.041477f,  0.37397f },  // Rule 41: PB & PM & VH & VH
        {  1,  5,  3,  3, -0.056114f, -0.013139f,  0.36731f },  // Rule 42: NM & PM & HI & HI
        {  6,  3,  4,  4, -0.604689f,  0.186951f,  0.36519f },  // Rule 43: PB & ZE & VH & VH
        {  6,  2,  3,  4, -1.000000f,  0.916451f,  0.36356f },  // Rule 44: PB & NS & HI & VH
        {  6,  4,  4,  4, -0.462690f,  0.122526f,  0.35777f },  // Rule 45: PB & PS & VH & VH
        {  2,  5,  3,  3, -0.082019f,  0.014977f,  0.35137f },  // Rule 46: NS & PM & HI & HI
        {  2,  6,  4,  4, -0.262576f, -0.961144f,  0.35078f },  // Rule 47: NS & PB & VH & VH
        {  5,  0,  4,  4,  0.102955f,  1.000000f,  0.34303f },  // Rule 48: PM & NB & VH & VH
        {  4,  0,  4,  4,  0.207479f,  1.000000f,  0.33415f },  // Rule 49: PS & NB & VH & VH
        {  0,  6,  1,  1, -0.021274f, -0.166980f,  0.33009f },  // Rule 50: NB & PB & LO & LO
        {  3,  6,  4,  4, -0.468157f, -0.921168f,  0.32270f },  // Rule 51: ZE & PB & VH & VH
        {  1,  5,  2,  2, -0.079546f, -0.011935f,  0.31055f },  // Rule 52: NM & PM & ME & ME
        {  4,  3,  3,  3, -0.055100f,  0.016502f,  0.29909f },  // Rule 53: PS & ZE & HI & HI
        {  0,  6,  1,  2, -0.026673f, -0.390710f,  0.28333f },  // Rule 54: NB & PB & LO & ME
        {  0,  3,  4,  4,  0.817581f, -0.874554f,  0.28214f },  // Rule 55: NB & ZE & VH & VH
        {  1,  5,  3,  4,  0.068263f, -0.429283f,  0.28061f },  // Rule 56: NM & PM & HI & VH
        {  4,  6,  4,  4, -0.265771f, -0.787367f,  0.27798f },  // Rule 57: PS & PB & VH & VH
        {  6,  0,  1,  1, -0.115876f,  0.154501f,  0.26191f },  // Rule 58: PB & NB & LO & LO
        {  6,  0,  4,  4, -0.427124f,  0.978443f,  0.25937f },  // Rule 59: PB & NB & VH & VH
        {  5,  6,  4,  4, -0.493471f, -0.674070f,  0.25579f },  // Rule 60: PM & PB & VH & VH
        {  0,  6,  4,  4,  0.559755f, -0.938654f,  0.25188f },  // Rule 61: NB & PB & VH & VH
        {  1,  6,  2,  2, -0.042442f, -0.052526f,  0.25082f },  // Rule 62: NM & PB & ME & ME
        {  6,  1,  2,  2, -0.091771f,  0.132335f,  0.25060f },  // Rule 63: PB & NM & ME & ME
        {  6,  3,  3,  4, -1.000000f,  0.849812f,  0.25018f },  // Rule 64: PB & ZE & HI & VH
        {  6,  1,  3,  3, -0.166321f,  0.208399f,  0.24739f },  // Rule 65: PB & NM & HI & HI
        {  1,  6,  3,  3, -0.092414f, -0.085786f,  0.22219f },  // Rule 66: NM & PB & HI & HI
        {  6,  0,  3,  4, -0.480693f,  0.975447f,  0.20685f },  // Rule 67: PB & NB & HI & VH
        {  1,  6,  3,  4, -0.029809f, -0.191017f,  0.09602f },  // Rule 68: NM & PB & HI & VH
        {  6,  6,  4,  4, -0.639672f,  0.137937f,  0.09364f },  // Rule 69: PB & PB & VH & VH
        {  6,  1,  3,  4, -0.180272f,  0.277842f,  0.07752f },  // Rule 70: PB & NM & HI & VH
    };

    private const int N_RULES = 70;

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
        return $"FuzzyController: {N_RULES} rules, generated 2026-10-11 13:35:34";
    }
}