// 模糊控制器 - 自动生成
// 生成时间: 2026-10-29 16:41:49
// 数据样本数: 300000
// 规则数: 95 (数据提取 95, 补全 0)
// 输入变量: front_lr_diff, rear_lr_diff, front_center, rear_center
// 输出变量: output_vx_norm, output_omega_norm
//
// 隶属函数配置:
//   front_lr_diff: 11 集 (S0, S1, S2, S3, S4, S5, S6, S7, S8, S9, S10)
//   rear_lr_diff: 11 集 (S0, S1, S2, S3, S4, S5, S6, S7, S8, S9, S10)
//   front_center: 5 集 (VL, LO, ME, HI, VH)
//   rear_center: 5 集 (VL, LO, ME, HI, VH)

using System;
using UnityEngine;

public class FuzzyController
{
    // ===== 三角形隶属函数参数 =====
    // 每行: { left, center, right }

    // front_lr_diff: S0, S1, S2, S3, S4, S5, S6, S7, S8, S9, S10
    private static readonly float[,] MF_FrontLR = {
        { -1.00000f, -1.00000f, -0.40000f },  // S0
        { -1.00000f, -0.40000f, -0.30000f },  // S1
        { -0.40000f, -0.30000f, -0.20000f },  // S2
        { -0.30000f, -0.20000f, -0.10000f },  // S3
        { -0.20000f, -0.10000f,  0.00000f },  // S4
        { -0.10000f,  0.00000f,  0.10000f },  // S5
        {  0.00000f,  0.10000f,  0.20000f },  // S6
        {  0.10000f,  0.20000f,  0.30000f },  // S7
        {  0.20000f,  0.30000f,  0.40000f },  // S8
        {  0.30000f,  0.40000f,  1.00000f },  // S9
        {  0.40000f,  1.00000f,  1.00000f },  // S10
    };

    // rear_lr_diff: S0, S1, S2, S3, S4, S5, S6, S7, S8, S9, S10
    private static readonly float[,] MF_RearLR = {
        { -1.00000f, -1.00000f, -0.40000f },  // S0
        { -1.00000f, -0.40000f, -0.30000f },  // S1
        { -0.40000f, -0.30000f, -0.20000f },  // S2
        { -0.30000f, -0.20000f, -0.10000f },  // S3
        { -0.20000f, -0.10000f,  0.00000f },  // S4
        { -0.10000f,  0.00000f,  0.10000f },  // S5
        {  0.00000f,  0.10000f,  0.20000f },  // S6
        {  0.10000f,  0.20000f,  0.30000f },  // S7
        {  0.20000f,  0.30000f,  0.40000f },  // S8
        {  0.30000f,  0.40000f,  1.00000f },  // S9
        {  0.40000f,  1.00000f,  1.00000f },  // S10
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

    // ===== 规则库 (95 条规则) =====
    // 每行: { 前LR集, 后LR集, 前中集, 后中集, 输出vx, 输出omega, 权重 }
    private static readonly float[,] Rules = {
        {  4,  5,  4,  4,  0.212845f, -0.295462f,  0.99924f },  // Rule 1: S4 & S5 & VH & VH
        {  6,  5,  4,  4, -0.112182f,  0.257826f,  0.99908f },  // Rule 2: S6 & S5 & VH & VH
        {  6,  3,  4,  4,  0.258882f,  0.677102f,  0.99850f },  // Rule 3: S6 & S3 & VH & VH
        {  5,  6,  4,  4, -0.108012f, -0.235585f,  0.99725f },  // Rule 4: S5 & S6 & VH & VH
        {  3,  6,  4,  4,  0.242785f, -0.693222f,  0.99626f },  // Rule 5: S3 & S6 & VH & VH
        {  5,  9,  4,  4, -0.636828f, -0.832326f,  0.99621f },  // Rule 6: S5 & S9 & VH & VH
        {  9,  5,  4,  4, -0.698526f,  0.940554f,  0.99610f },  // Rule 7: S9 & S5 & VH & VH
        {  5,  5,  4,  4,  0.008327f,  0.000030f,  0.99528f },  // Rule 8: S5 & S5 & VH & VH
        {  5,  8,  4,  4, -0.453113f, -0.633780f,  0.99482f },  // Rule 9: S5 & S8 & VH & VH
        {  4,  6,  4,  4,  0.054603f, -0.451477f,  0.99444f },  // Rule 10: S4 & S6 & VH & VH
        {  2,  5,  4,  4,  0.663187f, -0.890806f,  0.99260f },  // Rule 11: S2 & S5 & VH & VH
        {  6,  4,  4,  4,  0.071887f,  0.462077f,  0.99134f },  // Rule 12: S6 & S4 & VH & VH
        {  5,  7,  4,  4, -0.277069f, -0.432491f,  0.99098f },  // Rule 13: S5 & S7 & VH & VH
        {  5,  4,  4,  4,  0.189745f,  0.266399f,  0.99066f },  // Rule 14: S5 & S4 & VH & VH
        {  3,  5,  4,  4,  0.420889f, -0.564840f,  0.99030f },  // Rule 15: S3 & S5 & VH & VH
        {  3,  8,  4,  4, -0.090866f, -0.968401f,  0.98980f },  // Rule 16: S3 & S8 & VH & VH
        {  7,  5,  4,  4, -0.299366f,  0.474959f,  0.98929f },  // Rule 17: S7 & S5 & VH & VH
        {  3,  4,  4,  4,  0.590007f, -0.384681f,  0.98718f },  // Rule 18: S3 & S4 & VH & VH
        {  7,  4,  4,  4, -0.101195f,  0.678713f,  0.98567f },  // Rule 19: S7 & S4 & VH & VH
        {  6,  6,  4,  4, -0.294019f,  0.008398f,  0.98400f },  // Rule 20: S6 & S6 & VH & VH
        {  5,  3,  4,  4,  0.375332f,  0.494601f,  0.98400f },  // Rule 21: S5 & S3 & VH & VH
        {  6,  9,  4,  4, -0.837924f, -0.659691f,  0.97967f },  // Rule 22: S6 & S9 & VH & VH
        {  1,  7,  4,  4,  0.581715f, -1.000000f,  0.97956f },  // Rule 23: S1 & S7 & VH & VH
        {  4,  2,  4,  4,  0.711820f,  0.419896f,  0.97900f },  // Rule 24: S4 & S2 & VH & VH
        {  4,  7,  4,  4, -0.119408f, -0.628365f,  0.97731f },  // Rule 25: S4 & S7 & VH & VH
        {  1,  9,  4,  4,  0.175258f, -1.000000f,  0.97674f },  // Rule 26: S1 & S9 & VH & VH
        {  2,  7,  4,  4,  0.264825f, -0.998462f,  0.97535f },  // Rule 27: S2 & S7 & VH & VH
        {  9,  3,  4,  4, -0.265762f,  1.000000f,  0.97299f },  // Rule 28: S9 & S3 & VH & VH
        {  5,  1,  4,  4,  0.685187f,  0.867407f,  0.97203f },  // Rule 29: S5 & S1 & VH & VH
        {  4,  9,  4,  4, -0.496755f, -0.958227f,  0.97182f },  // Rule 30: S4 & S9 & VH & VH
        {  4,  8,  4,  4, -0.324329f, -0.742752f,  0.97094f },  // Rule 31: S4 & S8 & VH & VH
        {  8,  4,  4,  4, -0.264477f,  0.932967f,  0.96678f },  // Rule 32: S8 & S4 & VH & VH
        {  9,  2,  4,  4, -0.090577f,  1.000000f,  0.96673f },  // Rule 33: S9 & S2 & VH & VH
        {  6,  8,  4,  4, -0.631008f, -0.403263f,  0.96633f },  // Rule 34: S6 & S8 & VH & VH
        {  6,  7,  4,  4, -0.472185f, -0.174343f,  0.96588f },  // Rule 35: S6 & S7 & VH & VH
        {  7,  3,  4,  4,  0.074913f,  0.906145f,  0.96562f },  // Rule 36: S7 & S3 & VH & VH
        {  4,  3,  4,  4,  0.552026f,  0.121397f,  0.96538f },  // Rule 37: S4 & S3 & VH & VH
        {  4,  4,  4,  4,  0.394402f, -0.030366f,  0.96033f },  // Rule 38: S4 & S4 & VH & VH
        {  2,  8,  4,  4,  0.124558f, -1.000000f,  0.95980f },  // Rule 39: S2 & S8 & VH & VH
        {  1,  6,  4,  4,  0.696198f, -1.000000f,  0.95921f },  // Rule 40: S1 & S6 & VH & VH
        {  8,  5,  4,  4, -0.485105f,  0.714730f,  0.95736f },  // Rule 41: S8 & S5 & VH & VH
        {  8,  3,  4,  4, -0.070962f,  0.999751f,  0.95734f },  // Rule 42: S8 & S3 & VH & VH
        {  3,  9,  4,  4, -0.339754f, -0.999750f,  0.95578f },  // Rule 43: S3 & S9 & VH & VH
        {  2,  3,  4,  4,  0.911733f, -0.555907f,  0.95415f },  // Rule 44: S2 & S3 & VH & VH
        {  3,  3,  4,  4,  0.709951f, -0.241979f,  0.95207f },  // Rule 45: S3 & S3 & VH & VH
        {  8,  2,  4,  4,  0.098569f,  1.000000f,  0.95131f },  // Rule 46: S8 & S2 & VH & VH
        {  9,  4,  4,  4, -0.447802f,  1.000000f,  0.95000f },  // Rule 47: S9 & S4 & VH & VH
        {  6,  2,  4,  4,  0.426617f,  0.817804f,  0.94925f },  // Rule 48: S6 & S2 & VH & VH
        {  9,  1,  4,  4,  0.039203f,  1.000000f,  0.94483f },  // Rule 49: S9 & S1 & VH & VH
        {  7,  2,  4,  4,  0.279775f,  1.000000f,  0.94481f },  // Rule 50: S7 & S2 & VH & VH
        { 10,  0,  2,  2, -0.935975f,  1.000000f,  0.94140f },  // Rule 51: S10 & S0 & ME & ME
        {  8,  1,  4,  4,  0.289208f,  1.000000f,  0.94069f },  // Rule 52: S8 & S1 & VH & VH
        {  5,  2,  4,  4,  0.534296f,  0.721030f,  0.94046f },  // Rule 53: S5 & S2 & VH & VH
        {  1,  8,  4,  4,  0.446702f, -1.000000f,  0.93799f },  // Rule 54: S1 & S8 & VH & VH
        {  9,  6,  4,  4, -0.872681f,  0.867987f,  0.93778f },  // Rule 55: S9 & S6 & VH & VH
        {  2,  4,  4,  4,  0.769749f, -0.703068f,  0.93719f },  // Rule 56: S2 & S4 & VH & VH
        {  7,  8,  4,  4, -0.777558f, -0.235787f,  0.93627f },  // Rule 57: S7 & S8 & VH & VH
        {  3,  7,  4,  4,  0.066466f, -0.823659f,  0.93245f },  // Rule 58: S3 & S7 & VH & VH
        {  4,  1,  4,  4,  0.874518f,  0.646265f,  0.93079f },  // Rule 59: S4 & S1 & VH & VH
        {  8,  6,  4,  4, -0.643562f,  0.535653f,  0.92757f },  // Rule 60: S8 & S6 & VH & VH
        {  7,  6,  4,  4, -0.466930f,  0.290925f,  0.92734f },  // Rule 61: S7 & S6 & VH & VH
        {  7,  1,  4,  4,  0.432375f,  1.000000f,  0.92536f },  // Rule 62: S7 & S1 & VH & VH
        {  2,  9,  4,  4, -0.141307f, -1.000000f,  0.92208f },  // Rule 63: S2 & S9 & VH & VH
        {  2,  6,  4,  4,  0.443183f, -0.952489f,  0.89385f },  // Rule 64: S2 & S6 & VH & VH
        {  7,  7,  4,  4, -0.599999f,  0.025633f,  0.88618f },  // Rule 65: S7 & S7 & VH & VH
        {  1,  1,  4,  4,  1.000000f, -0.454809f,  0.88393f },  // Rule 66: S1 & S1 & VH & VH
        {  6,  1,  4,  4,  0.594977f,  1.000000f,  0.86122f },  // Rule 67: S6 & S1 & VH & VH
        {  3,  2,  4,  4,  0.903337f,  0.008638f,  0.85702f },  // Rule 68: S3 & S2 & VH & VH
        {  0, 10,  2,  2,  0.512031f, -1.000000f,  0.83379f },  // Rule 69: S0 & S10 & ME & ME
        {  8,  7,  4,  4, -0.785291f,  0.307974f,  0.82295f },  // Rule 70: S8 & S7 & VH & VH
        {  2,  1,  4,  4,  1.000000f,  0.065712f,  0.81674f },  // Rule 71: S2 & S1 & VH & VH
        {  7,  9,  4,  4, -0.932853f, -0.473815f,  0.80971f },  // Rule 72: S7 & S9 & VH & VH
        {  9,  7,  4,  4, -0.990485f,  0.797129f,  0.78561f },  // Rule 73: S9 & S7 & VH & VH
        {  3,  1,  4,  4,  0.993954f,  0.519555f,  0.76030f },  // Rule 74: S3 & S1 & VH & VH
        {  9,  0,  4,  3,  0.369558f,  1.000000f,  0.74540f },  // Rule 75: S9 & S0 & VH & HI
        {  6,  0,  4,  3,  0.774182f,  1.000000f,  0.72290f },  // Rule 76: S6 & S0 & VH & HI
        {  2,  2,  4,  4,  0.999834f, -0.487875f,  0.70511f },  // Rule 77: S2 & S2 & VH & VH
        {  7,  0,  4,  3,  0.707724f,  1.000000f,  0.70072f },  // Rule 78: S7 & S0 & VH & HI
        {  1,  4,  4,  4,  0.936343f, -0.995540f,  0.67694f },  // Rule 79: S1 & S4 & VH & VH
        {  8,  0,  4,  3,  0.648891f,  1.000000f,  0.66710f },  // Rule 80: S8 & S0 & VH & HI
        {  1,  5,  4,  4,  0.814386f, -1.000000f,  0.66385f },  // Rule 81: S1 & S5 & VH & VH
        { 10,  0,  3,  2, -0.820801f,  1.000000f,  0.59829f },  // Rule 82: S10 & S0 & HI & ME
        { 10,  0,  3,  3, -0.800141f,  1.000000f,  0.59695f },  // Rule 83: S10 & S0 & HI & HI
        {  0, 10,  3,  3,  0.435598f, -1.000000f,  0.57202f },  // Rule 84: S0 & S10 & HI & HI
        {  5,  0,  4,  3,  0.854557f,  1.000000f,  0.55742f },  // Rule 85: S5 & S0 & VH & HI
        {  1, 10,  4,  3,  0.140426f, -1.000000f,  0.44980f },  // Rule 86: S1 & S10 & VH & HI
        {  0, 10,  3,  2,  0.560707f, -1.000000f,  0.39809f },  // Rule 87: S0 & S10 & HI & ME
        {  9,  1,  4,  3,  0.292498f,  1.000000f,  0.37386f },  // Rule 88: S9 & S1 & VH & HI
        { 10,  0,  2,  3, -0.931549f,  1.000000f,  0.36826f },  // Rule 89: S10 & S0 & ME & HI
        {  1,  9,  4,  3,  0.228075f, -1.000000f,  0.36710f },  // Rule 90: S1 & S9 & VH & HI
        {  0, 10,  2,  3,  0.492873f, -1.000000f,  0.29040f },  // Rule 91: S0 & S10 & ME & HI
        {  0, 10,  4,  3,  0.372599f, -1.000000f,  0.27334f },  // Rule 92: S0 & S10 & VH & HI
        { 10,  0,  4,  3, -0.145227f,  1.000000f,  0.25827f },  // Rule 93: S10 & S0 & VH & HI
        {  9,  0,  4,  4,  0.077015f,  1.000000f,  0.14834f },  // Rule 94: S9 & S0 & VH & VH
        {  0,  9,  4,  4,  0.258538f, -1.000000f,  0.07815f },  // Rule 95: S0 & S9 & VH & VH
    };

    private const int N_RULES = 95;

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
        return $"FuzzyController: {N_RULES} rules, generated 2026-10-29 16:41:49";
    }
}