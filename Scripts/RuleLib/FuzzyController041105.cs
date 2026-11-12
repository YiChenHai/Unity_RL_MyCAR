// 模糊控制器 - 自动生成
// 生成时间: 2026-11-04 18:56:58
// 数据样本数: 300000
// 规则数: 135 (数据提取 123, 补全 12)
// 输入变量: front_lr_diff, rear_lr_diff, front_center, rear_center
// 输出变量: output_vx_norm, output_omega_norm
//
// 隶属函数配置:
//   front_lr_diff: 11 集 (NVB, NB, NM, NS, NZ, ZE, PZ, PS, PM, PB, PVB)
//   rear_lr_diff: 11 集 (NVB, NB, NM, NS, NZ, ZE, PZ, PS, PM, PB, PVB)
//   front_center: 5 集 (VL, LO, ME, HI, VH)
//   rear_center: 5 集 (VL, LO, ME, HI, VH)

using System;
using UnityEngine;

public class FuzzyController
{
    // ===== 三角形隶属函数参数 =====
    // 每行: { left, center, right }

    // front_lr_diff: NVB, NB, NM, NS, NZ, ZE, PZ, PS, PM, PB, PVB
    private static readonly float[,] MF_FrontLR = {
        { -1.00000f, -1.00000f, -0.40000f },  // NVB
        { -1.00000f, -0.40000f, -0.30000f },  // NB
        { -0.40000f, -0.30000f, -0.20000f },  // NM
        { -0.30000f, -0.20000f, -0.10000f },  // NS
        { -0.20000f, -0.10000f,  0.00000f },  // NZ
        { -0.10000f,  0.00000f,  0.10000f },  // ZE
        {  0.00000f,  0.10000f,  0.20000f },  // PZ
        {  0.10000f,  0.20000f,  0.30000f },  // PS
        {  0.20000f,  0.30000f,  0.40000f },  // PM
        {  0.30000f,  0.40000f,  1.00000f },  // PB
        {  0.40000f,  1.00000f,  1.00000f },  // PVB
    };

    // rear_lr_diff: NVB, NB, NM, NS, NZ, ZE, PZ, PS, PM, PB, PVB
    private static readonly float[,] MF_RearLR = {
        { -1.00000f, -1.00000f, -0.40000f },  // NVB
        { -1.00000f, -0.40000f, -0.30000f },  // NB
        { -0.40000f, -0.30000f, -0.20000f },  // NM
        { -0.30000f, -0.20000f, -0.10000f },  // NS
        { -0.20000f, -0.10000f,  0.00000f },  // NZ
        { -0.10000f,  0.00000f,  0.10000f },  // ZE
        {  0.00000f,  0.10000f,  0.20000f },  // PZ
        {  0.10000f,  0.20000f,  0.30000f },  // PS
        {  0.20000f,  0.30000f,  0.40000f },  // PM
        {  0.30000f,  0.40000f,  1.00000f },  // PB
        {  0.40000f,  1.00000f,  1.00000f },  // PVB
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

    // ===== 规则库 (135 条规则) =====
    // 每行: { 前LR集, 后LR集, 前中集, 后中集, 输出vx, 输出omega, 权重 }
    private static readonly float[,] Rules = {
        {  6,  6,  4,  4, -0.264217f, -0.269043f,  0.99824f },  // Rule 1: PZ & PZ & VH & VH
        {  3,  3,  4,  4,  0.690404f, -0.020011f,  0.99758f },  // Rule 2: NS & NS & VH & VH
        {  1,  1,  4,  4,  1.000000f, -0.004387f,  0.99581f },  // Rule 3: NB & NB & VH & VH
        {  5,  4,  4,  4,  0.203158f,  0.315625f,  0.99557f },  // Rule 4: ZE & NZ & VH & VH
        {  4,  3,  4,  4,  0.551031f,  0.425806f,  0.99551f },  // Rule 5: NZ & NS & VH & VH
        {  9,  7,  4,  4, -0.710654f,  0.798586f,  0.99526f },  // Rule 6: PB & PS & VH & VH
        {  7,  5,  4,  4, -0.250775f,  0.538235f,  0.99521f },  // Rule 7: PS & ZE & VH & VH
        {  6,  5,  4,  4, -0.102554f,  0.256911f,  0.99512f },  // Rule 8: PZ & ZE & VH & VH
        {  5,  5,  4,  4,  0.006120f, -0.058466f,  0.99366f },  // Rule 9: ZE & ZE & VH & VH
        {  4,  5,  4,  4,  0.166269f, -0.501506f,  0.99366f },  // Rule 10: NZ & ZE & VH & VH
        {  8,  6,  4,  4, -0.487850f,  0.658978f,  0.99281f },  // Rule 11: PM & PZ & VH & VH
        {  3,  4,  4,  4,  0.482823f, -0.603035f,  0.99113f },  // Rule 12: NS & NZ & VH & VH
        {  6,  7,  4,  4, -0.415238f, -0.643064f,  0.99105f },  // Rule 13: PZ & PS & VH & VH
        {  7,  9,  4,  4, -0.594280f, -0.843198f,  0.99069f },  // Rule 14: PS & PB & VH & VH
        {  9,  3,  4,  4, -0.389994f,  1.000000f,  0.98979f },  // Rule 15: PB & NS & VH & VH
        {  5,  6,  4,  4, -0.169152f, -0.540031f,  0.98905f },  // Rule 16: ZE & PZ & VH & VH
        {  8,  9,  4,  4, -0.636133f, -0.625382f,  0.98901f },  // Rule 17: PM & PB & VH & VH
        {  9,  1,  4,  4, -0.161402f,  1.000000f,  0.98718f },  // Rule 18: PB & NB & VH & VH
        {  2,  4,  4,  4,  0.655546f, -0.874823f,  0.98630f },  // Rule 19: NM & NZ & VH & VH
        {  7,  6,  4,  4, -0.372777f,  0.367458f,  0.98560f },  // Rule 20: PS & PZ & VH & VH
        {  1,  4,  4,  4,  0.761605f, -0.991088f,  0.98399f },  // Rule 21: NB & NZ & VH & VH
        {  1,  7,  4,  4,  0.226256f, -1.000000f,  0.98360f },  // Rule 22: NB & PS & VH & VH
        {  2,  1,  4,  4,  0.998034f,  0.363574f,  0.98142f },  // Rule 23: NM & NB & VH & VH
        {  4,  2,  4,  4,  0.676298f,  0.644993f,  0.98107f },  // Rule 24: NZ & NM & VH & VH
        {  6,  4,  4,  4,  0.071599f,  0.600808f,  0.98062f },  // Rule 25: PZ & NZ & VH & VH
        {  1,  8,  4,  4,  0.033749f, -1.000000f,  0.97896f },  // Rule 26: NB & PM & VH & VH
        {  9,  9,  4,  4, -0.711906f, -0.164152f,  0.97852f },  // Rule 27: PB & PB & VH & VH
        {  4,  4,  4,  4,  0.328945f, -0.282459f,  0.97828f },  // Rule 28: NZ & NZ & VH & VH
        {  5,  9,  4,  4, -0.488606f, -1.000000f,  0.97741f },  // Rule 29: ZE & PB & VH & VH
        {  1,  3,  4,  4,  0.932073f, -0.840801f,  0.97640f },  // Rule 30: NB & NS & VH & VH
        {  8,  7,  4,  4, -0.590545f,  0.064255f,  0.97535f },  // Rule 31: PM & PS & VH & VH
        {  3,  5,  4,  4,  0.336469f, -0.816758f,  0.97462f },  // Rule 32: NS & ZE & VH & VH
        {  2,  3,  4,  4,  0.793762f, -0.421413f,  0.97257f },  // Rule 33: NM & NS & VH & VH
        {  5,  7,  4,  4, -0.334869f, -0.819482f,  0.97163f },  // Rule 34: ZE & PS & VH & VH
        {  3,  7,  4,  4, -0.123411f, -1.000000f,  0.97133f },  // Rule 35: NS & PS & VH & VH
        {  1,  5,  4,  4,  0.625380f, -1.000000f,  0.97093f },  // Rule 36: NB & ZE & VH & VH
        {  5,  3,  4,  4,  0.403940f,  0.669693f,  0.96757f },  // Rule 37: ZE & NS & VH & VH
        {  7,  8,  4,  4, -0.542031f, -0.609871f,  0.96655f },  // Rule 38: PS & PM & VH & VH
        {  2,  6,  4,  4,  0.188894f, -1.000000f,  0.96640f },  // Rule 39: NM & PZ & VH & VH
        {  8,  1,  4,  4,  0.279459f,  1.000000f,  0.96510f },  // Rule 40: PM & NB & VH & VH
        {  1,  2,  4,  4,  0.998561f, -0.423374f,  0.96360f },  // Rule 41: NB & NM & VH & VH
        {  9,  2,  4,  4, -0.219780f,  1.000000f,  0.96283f },  // Rule 42: PB & NM & VH & VH
        {  5,  1,  4,  4,  0.702078f,  0.909233f,  0.96186f },  // Rule 43: ZE & NB & VH & VH
        {  9,  8,  4,  4, -0.757401f,  0.389905f,  0.95859f },  // Rule 44: PB & PM & VH & VH
        {  9,  6,  4,  4, -0.635936f,  0.891969f,  0.95771f },  // Rule 45: PB & PZ & VH & VH
        {  2,  2,  4,  4,  0.923879f,  0.045009f,  0.95745f },  // Rule 46: NM & NM & VH & VH
        {  7,  4,  4,  4, -0.052666f,  0.974978f,  0.95605f },  // Rule 47: PS & NZ & VH & VH
        {  8,  2,  4,  4,  0.077618f,  1.000000f,  0.95601f },  // Rule 48: PM & NM & VH & VH
        {  8,  3,  4,  4, -0.031519f,  1.000000f,  0.95559f },  // Rule 49: PM & NS & VH & VH
        {  7,  3,  4,  4,  0.108070f,  1.000000f,  0.95515f },  // Rule 50: PS & NS & VH & VH
        {  6,  2,  4,  4,  0.375467f,  1.000000f,  0.95466f },  // Rule 51: PZ & NM & VH & VH
        {  8,  4,  4,  4, -0.205122f,  1.000000f,  0.95192f },  // Rule 52: PM & NZ & VH & VH
        {  4,  6,  4,  4, -0.042485f, -0.842115f,  0.95177f },  // Rule 53: NZ & PZ & VH & VH
        {  9,  5,  4,  4, -0.632000f,  1.000000f,  0.94912f },  // Rule 54: PB & ZE & VH & VH
        {  7,  7,  4,  4, -0.505245f, -0.309174f,  0.94862f },  // Rule 55: PS & PS & VH & VH
        {  4,  1,  4,  4,  0.749989f,  0.867089f,  0.94770f },  // Rule 56: NZ & NB & VH & VH
        {  2,  7,  4,  4, -0.015197f, -1.000000f,  0.94753f },  // Rule 57: NM & PS & VH & VH
        {  6,  9,  4,  4, -0.544772f, -0.983034f,  0.94703f },  // Rule 58: PZ & PB & VH & VH
        {  9,  4,  4,  4, -0.529429f,  1.000000f,  0.94574f },  // Rule 59: PB & NZ & VH & VH
        {  8,  5,  4,  4, -0.368689f,  0.867048f,  0.94273f },  // Rule 60: PM & ZE & VH & VH
        {  5,  2,  4,  4,  0.584152f,  0.838710f,  0.93737f },  // Rule 61: ZE & NM & VH & VH
        {  3,  2,  4,  4,  0.825553f,  0.359710f,  0.93682f },  // Rule 62: NS & NM & VH & VH
        {  3,  6,  4,  4,  0.056309f, -1.000000f,  0.93634f },  // Rule 63: NS & PZ & VH & VH
        {  4,  7,  4,  4, -0.249590f, -1.000000f,  0.93483f },  // Rule 64: NZ & PS & VH & VH
        {  8,  8,  4,  4, -0.622958f, -0.304722f,  0.93284f },  // Rule 65: PM & PM & VH & VH
        {  6,  8,  4,  4, -0.503876f, -0.790686f,  0.93158f },  // Rule 66: PZ & PM & VH & VH
        {  1,  6,  4,  4,  0.447564f, -1.000000f,  0.92964f },  // Rule 67: NB & PZ & VH & VH
        {  6,  3,  4,  4,  0.267897f,  0.964100f,  0.92495f },  // Rule 68: PZ & NS & VH & VH
        {  2,  5,  4,  4,  0.434112f, -0.999833f,  0.92197f },  // Rule 69: NM & ZE & VH & VH
        {  4,  9,  4,  4, -0.472713f, -1.000000f,  0.90982f },  // Rule 70: NZ & PB & VH & VH
        {  3,  1,  4,  4,  0.919458f,  0.591629f,  0.90076f },  // Rule 71: NS & NB & VH & VH
        {  2,  8,  4,  4, -0.133766f, -1.000000f,  0.89349f },  // Rule 72: NM & PM & VH & VH
        {  1,  9,  4,  4, -0.069327f, -1.000000f,  0.87909f },  // Rule 73: NB & PB & VH & VH
        {  4,  8,  4,  4, -0.344372f, -1.000000f,  0.86929f },  // Rule 74: NZ & PM & VH & VH
        {  7,  2,  4,  4,  0.230225f,  1.000000f,  0.86693f },  // Rule 75: PS & NM & VH & VH
        {  6,  1,  4,  4,  0.531462f,  1.000000f,  0.85204f },  // Rule 76: PZ & NB & VH & VH
        {  5,  8,  4,  4, -0.425588f, -0.997892f,  0.82933f },  // Rule 77: ZE & PM & VH & VH
        {  3,  9,  4,  4, -0.398611f, -1.000000f,  0.79881f },  // Rule 78: NS & PB & VH & VH
        {  7,  1,  4,  4,  0.402441f,  1.000000f,  0.79810f },  // Rule 79: PS & NB & VH & VH
        { 10,  6,  3,  4, -0.805671f,  1.000000f,  0.78643f },  // Rule 80: PVB & PZ & HI & VH
        { 10,  7,  3,  4, -0.779161f,  1.000000f,  0.75919f },  // Rule 81: PVB & PS & HI & VH
        {  3,  8,  4,  4, -0.238186f, -1.000000f,  0.75418f },  // Rule 82: NS & PM & VH & VH
        {  0,  9,  3,  4,  0.166371f, -1.000000f,  0.75406f },  // Rule 83: NVB & PB & HI & VH
        { 10,  3,  3,  4, -0.835212f,  1.000000f,  0.75271f },  // Rule 84: PVB & NS & HI & VH
        {  0,  3,  3,  4,  0.977759f, -1.000000f,  0.74855f },  // Rule 85: NVB & NS & HI & VH
        { 10,  8,  3,  4, -0.763448f,  0.993547f,  0.73568f },  // Rule 86: PVB & PM & HI & VH
        { 10,  4,  3,  4, -0.842428f,  1.000000f,  0.73405f },  // Rule 87: PVB & NZ & HI & VH
        {  0,  2,  3,  4,  1.000000f, -0.998599f,  0.73163f },  // Rule 88: NVB & NM & HI & VH
        {  0,  8,  3,  4,  0.347463f, -1.000000f,  0.72558f },  // Rule 89: NVB & PM & HI & VH
        { 10,  5,  3,  4, -0.826541f,  1.000000f,  0.71620f },  // Rule 90: PVB & ZE & HI & VH
        {  2,  9,  4,  4, -0.375966f, -1.000000f,  0.68037f },  // Rule 91: NM & PB & VH & VH
        { 10,  1,  3,  4, -0.740362f,  1.000000f,  0.67841f },  // Rule 92: PVB & NB & HI & VH
        {  0,  7,  3,  4,  0.496128f, -1.000000f,  0.65847f },  // Rule 93: NVB & PS & HI & VH
        {  0,  6,  3,  4,  0.643650f, -1.000000f,  0.63312f },  // Rule 94: NVB & PZ & HI & VH
        { 10,  9,  3,  4, -0.741703f,  0.789029f,  0.58785f },  // Rule 95: PVB & PB & HI & VH
        {  0,  1,  3,  4,  0.741703f, -0.789029f,  0.58785f },  // Rule 96: NVB & NB & HI & VH  [补全]
        { 10,  2,  3,  4, -0.795587f,  1.000000f,  0.55538f },  // Rule 97: PVB & NM & HI & VH
        { 10,  0,  3,  3, -0.525872f,  1.000000f,  0.54623f },  // Rule 98: PVB & NVB & HI & HI
        {  0, 10,  3,  3,  0.029574f, -1.000000f,  0.53681f },  // Rule 99: NVB & PVB & HI & HI
        {  0,  4,  3,  4,  0.880355f, -1.000000f,  0.52338f },  // Rule 100: NVB & NZ & HI & VH
        { 10,  1,  4,  4, -0.564805f,  1.000000f,  0.45981f },  // Rule 101: PVB & NB & VH & VH
        { 10,  9,  2,  4, -0.620865f,  0.852225f,  0.45149f },  // Rule 102: PVB & PB & ME & VH
        {  0,  1,  2,  4,  0.620865f, -0.852225f,  0.45149f },  // Rule 103: NVB & NB & ME & VH  [补全]
        {  0,  5,  3,  4,  0.762660f, -1.000000f,  0.36360f },  // Rule 104: NVB & ZE & HI & VH
        {  9, 10,  4,  4, -0.572880f, -0.796388f,  0.30279f },  // Rule 105: PB & PVB & VH & VH
        {  1,  0,  4,  4,  0.572880f,  0.796388f,  0.30279f },  // Rule 106: NB & NVB & VH & VH  [补全]
        { 10, 10,  3,  3, -0.582176f,  0.030647f,  0.29468f },  // Rule 107: PVB & PVB & HI & HI
        {  0,  0,  3,  3,  0.582176f, -0.030647f,  0.29468f },  // Rule 108: NVB & NVB & HI & HI  [补全]
        { 10,  9,  4,  4, -0.825975f,  0.559209f,  0.26488f },  // Rule 109: PVB & PB & VH & VH
        {  0,  1,  4,  4,  0.825975f, -0.559209f,  0.26488f },  // Rule 110: NVB & NB & VH & VH  [补全]
        { 10,  3,  4,  4, -0.776343f,  1.000000f,  0.26188f },  // Rule 111: PVB & NS & VH & VH
        {  0,  9,  4,  4,  0.079656f, -1.000000f,  0.26188f },  // Rule 112: NVB & PB & VH & VH
        { 10,  5,  4,  4, -0.831291f,  1.000000f,  0.26182f },  // Rule 113: PVB & ZE & VH & VH
        {  0,  5,  4,  4,  0.831291f, -1.000000f,  0.26182f },  // Rule 114: NVB & ZE & VH & VH  [补全]
        { 10,  6,  4,  4, -0.828428f,  1.000000f,  0.25935f },  // Rule 115: PVB & PZ & VH & VH
        {  0,  4,  4,  4,  0.828428f, -1.000000f,  0.25935f },  // Rule 116: NVB & NZ & VH & VH  [补全]
        { 10,  4,  4,  4, -0.814149f,  1.000000f,  0.25590f },  // Rule 117: PVB & NZ & VH & VH
        {  0,  8,  4,  4,  0.265534f, -1.000000f,  0.24894f },  // Rule 118: NVB & PM & VH & VH
        { 10,  2,  4,  4, -0.709578f,  1.000000f,  0.24650f },  // Rule 119: PVB & NM & VH & VH
        {  0,  7,  4,  4,  0.415891f, -1.000000f,  0.24412f },  // Rule 120: NVB & PS & VH & VH
        {  9,  6,  3,  4, -0.815405f,  1.000000f,  0.23791f },  // Rule 121: PB & PZ & HI & VH
        {  1,  4,  3,  4,  0.815405f, -1.000000f,  0.23791f },  // Rule 122: NB & NZ & HI & VH  [补全]
        {  0,  6,  4,  4,  0.579283f, -1.000000f,  0.22879f },  // Rule 123: NVB & PZ & VH & VH
        {  9,  3,  3,  4, -0.758524f,  1.000000f,  0.21664f },  // Rule 124: PB & NS & HI & VH
        {  1,  7,  3,  4,  0.758524f, -1.000000f,  0.21664f },  // Rule 125: NB & PS & HI & VH  [补全]
        { 10,  0,  3,  4, -0.624288f,  1.000000f,  0.21527f },  // Rule 126: PVB & NVB & HI & VH
        { 10,  8,  4,  4, -0.831438f,  0.937576f,  0.21482f },  // Rule 127: PVB & PM & VH & VH
        {  0,  2,  4,  4,  0.831438f, -0.937576f,  0.21482f },  // Rule 128: NVB & NM & VH & VH  [补全]
        {  1,  9,  3,  4, -0.138815f, -1.000000f,  0.17714f },  // Rule 129: NB & PB & HI & VH
        {  9, 10,  4,  3, -0.587492f, -0.247268f,  0.14775f },  // Rule 130: PB & PVB & VH & HI
        {  1,  0,  4,  3,  0.587492f,  0.247268f,  0.14775f },  // Rule 131: NB & NVB & VH & HI  [补全]
        {  0, 10,  3,  4, -0.071594f, -1.000000f,  0.14093f },  // Rule 132: NVB & PVB & HI & VH
        {  9,  0,  4,  4, -0.365520f,  1.000000f,  0.09684f },  // Rule 133: PB & NVB & VH & VH
        {  1, 10,  4,  4,  0.365520f, -1.000000f,  0.09684f },  // Rule 134: NB & PVB & VH & VH  [补全]
        {  9,  1,  3,  4, -0.459857f,  1.000000f,  0.09357f },  // Rule 135: PB & NB & HI & VH
    };

    private const int N_RULES = 135;

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
        return $"FuzzyController: {N_RULES} rules, generated 2026-11-04 18:56:58";
    }
}