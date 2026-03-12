// 模糊控制器 - 自动生成
// 生成时间: 2026-03-12 13:32:57
// 数据样本数: 300000
// 规则数: 257
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
        { -1.00000f, -1.00000f,  0.02101f },  // NB
        { -1.00000f,  0.02101f,  0.02723f },  // NM
        {  0.02101f,  0.02723f,  0.03076f },  // NS
        {  0.02723f,  0.03076f,  0.03416f },  // ZE
        {  0.03076f,  0.03416f,  0.03914f },  // PS
        {  0.03416f,  0.03914f,  1.00000f },  // PM
        {  0.03914f,  1.00000f,  1.00000f },  // PB
    };

    // rear_lr_diff: NB, NM, NS, ZE, PS, PM, PB
    private static readonly float[,] MF_RearLR = {
        { -1.00000f, -1.00000f, -0.01224f },  // NB
        { -1.00000f, -0.01224f, -0.00687f },  // NM
        { -0.01224f, -0.00687f, -0.00295f },  // NS
        { -0.00687f, -0.00295f,  0.00100f },  // ZE
        { -0.00295f,  0.00100f,  0.00672f },  // PS
        {  0.00100f,  0.00672f,  1.00000f },  // PM
        {  0.00672f,  1.00000f,  1.00000f },  // PB
    };

    // front_center: VL, LO, ME, HI, VH
    private static readonly float[,] MF_FrontCenter = {
        {  0.00000f,  0.00000f,  0.84959f },  // VL
        {  0.00000f,  0.84959f,  0.85233f },  // LO
        {  0.84959f,  0.85233f,  0.85460f },  // ME
        {  0.85233f,  0.85460f,  1.00000f },  // HI
        {  0.85460f,  1.00000f,  1.00000f },  // VH
    };

    // rear_center: VL, LO, ME, HI, VH
    private static readonly float[,] MF_RearCenter = {
        {  0.00000f,  0.00000f,  0.84986f },  // VL
        {  0.00000f,  0.84986f,  0.85193f },  // LO
        {  0.84986f,  0.85193f,  0.85477f },  // ME
        {  0.85193f,  0.85477f,  1.00000f },  // HI
        {  0.85477f,  1.00000f,  1.00000f },  // VH
    };

    // ===== 规则库 (257 条规则) =====
    // 每行: { 前LR集, 后LR集, 前中集, 后中集, 输出vx, 输出omega, 权重 }
    private static readonly float[,] Rules = {
        {  5,  5,  3,  1, -0.236292f,  0.288344f,  0.99878f },  // Rule 1: PM & PM & HI & LO
        {  1,  1,  3,  1,  0.239000f, -0.327691f,  0.99827f },  // Rule 2: NM & NM & HI & LO
        {  2,  1,  3,  1,  0.013885f,  0.006706f,  0.99794f },  // Rule 3: NS & NM & HI & LO
        {  5,  1,  3,  1, -0.015166f,  0.403540f,  0.99790f },  // Rule 4: PM & NM & HI & LO
        {  5,  5,  1,  3,  0.005576f,  0.061225f,  0.99787f },  // Rule 5: PM & PM & LO & HI
        {  3,  5,  3,  1, -0.019678f, -0.009210f,  0.99760f },  // Rule 6: ZE & PM & HI & LO
        {  1,  1,  1,  3,  0.060871f, -0.109492f,  0.99759f },  // Rule 7: NM & NM & LO & HI
        {  4,  1,  3,  1, -0.003385f,  0.046850f,  0.99750f },  // Rule 8: PS & NM & HI & LO
        {  5,  3,  3,  1, -0.029304f,  0.061298f,  0.99742f },  // Rule 9: PM & ZE & HI & LO
        {  3,  5,  1,  3, -0.018913f, -0.008447f,  0.99706f },  // Rule 10: ZE & PM & LO & HI
        {  5,  1,  1,  3,  0.187977f,  0.426610f,  0.99648f },  // Rule 11: PM & NM & LO & HI
        {  4,  5,  3,  1, -0.025819f,  0.004086f,  0.99645f },  // Rule 12: PS & PM & HI & LO
        {  1,  3,  1,  3,  0.030145f, -0.101506f,  0.99645f },  // Rule 13: NM & ZE & LO & HI
        {  1,  5,  1,  3, -0.183634f, -0.349249f,  0.99644f },  // Rule 14: NM & PM & LO & HI
        {  1,  5,  3,  1, -0.020622f, -0.417038f,  0.99622f },  // Rule 15: NM & PM & HI & LO
        {  2,  5,  3,  1, -0.016641f, -0.019176f,  0.99617f },  // Rule 16: NS & PM & HI & LO
        {  2,  1,  1,  3,  0.008467f,  0.005887f,  0.99551f },  // Rule 17: NS & NM & LO & HI
        {  4,  1,  1,  3,  0.001701f,  0.041919f,  0.99514f },  // Rule 18: PS & NM & LO & HI
        {  5,  5,  3,  3, -0.080888f,  0.115303f,  0.99484f },  // Rule 19: PM & PM & HI & HI
        {  3,  1,  1,  3,  0.004726f,  0.020634f,  0.99462f },  // Rule 20: ZE & NM & LO & HI
        {  1,  5,  1,  4,  0.019908f, -0.001472f,  0.99410f },  // Rule 21: NM & PM & LO & VH
        {  1,  2,  1,  3,  0.033524f, -0.083107f,  0.99404f },  // Rule 22: NM & NS & LO & HI
        {  5,  4,  1,  3, -0.004040f,  0.058193f,  0.99390f },  // Rule 23: PM & PS & LO & HI
        {  4,  4,  1,  3, -0.011398f,  0.008482f,  0.99364f },  // Rule 24: PS & PS & LO & HI
        {  3,  1,  3,  1,  0.010630f,  0.017850f,  0.99360f },  // Rule 25: ZE & NM & HI & LO
        {  5,  2,  1,  3, -0.006644f,  0.127394f,  0.99348f },  // Rule 26: PM & NS & LO & HI
        {  4,  3,  1,  3, -0.006756f,  0.011459f,  0.99343f },  // Rule 27: PS & ZE & LO & HI
        {  5,  4,  3,  1, -0.034320f,  0.053943f,  0.99340f },  // Rule 28: PM & PS & HI & LO
        {  1,  2,  3,  1,  0.037635f, -0.070708f,  0.99307f },  // Rule 29: NM & NS & HI & LO
        {  1,  1,  3,  3,  0.090275f, -0.119359f,  0.99262f },  // Rule 30: NM & NM & HI & HI
        {  5,  3,  1,  3, -0.005884f,  0.079867f,  0.99242f },  // Rule 31: PM & ZE & LO & HI
        {  3,  3,  1,  3, -0.006002f,  0.003700f,  0.99204f },  // Rule 32: ZE & ZE & LO & HI
        {  4,  5,  1,  3, -0.017090f,  0.003719f,  0.99190f },  // Rule 33: PS & PM & LO & HI
        {  2,  5,  1,  3, -0.022467f, -0.024474f,  0.99164f },  // Rule 34: NS & PM & LO & HI
        {  1,  4,  1,  3,  0.035377f, -0.146076f,  0.99073f },  // Rule 35: NM & PS & LO & HI
        {  5,  5,  3,  4, -0.068299f, -0.020192f,  0.98963f },  // Rule 36: PM & PM & HI & VH
        {  1,  1,  3,  2,  0.146988f, -0.209412f,  0.98957f },  // Rule 37: NM & NM & HI & ME
        {  1,  4,  3,  1,  0.046469f, -0.136814f,  0.98918f },  // Rule 38: NM & PS & HI & LO
        {  5,  2,  3,  1, -0.045422f,  0.113949f,  0.98841f },  // Rule 39: PM & NS & HI & LO
        {  1,  1,  1,  2,  0.037998f, -0.057591f,  0.98838f },  // Rule 40: NM & NM & LO & ME
        {  2,  5,  1,  4, -0.007707f,  0.004587f,  0.98811f },  // Rule 41: NS & PM & LO & VH
        {  3,  4,  1,  3, -0.011377f, -0.001956f,  0.98775f },  // Rule 42: ZE & PS & LO & HI
        {  1,  1,  2,  1,  0.244300f, -0.361805f,  0.98762f },  // Rule 43: NM & NM & ME & LO
        {  3,  3,  3,  1, -0.000807f, -0.001495f,  0.98757f },  // Rule 44: ZE & ZE & HI & LO
        {  2,  3,  1,  3, -0.005875f, -0.007233f,  0.98750f },  // Rule 45: NS & ZE & LO & HI
        {  1,  3,  3,  1,  0.035300f, -0.088281f,  0.98635f },  // Rule 46: NM & ZE & HI & LO
        {  1,  1,  1,  1,  0.011815f, -0.048770f,  0.98628f },  // Rule 47: NM & NM & LO & LO
        {  5,  5,  1,  4, -0.024873f,  0.035041f,  0.98566f },  // Rule 48: PM & PM & LO & VH
        {  2,  2,  1,  3, -0.000264f, -0.003482f,  0.98566f },  // Rule 49: NS & NS & LO & HI
        {  1,  5,  3,  3, -0.008040f, -0.042386f,  0.98509f },  // Rule 50: NM & PM & HI & HI
        {  3,  5,  3,  4, -0.075442f, -0.079822f,  0.98474f },  // Rule 51: ZE & PM & HI & VH
        {  2,  2,  3,  1,  0.007277f, -0.006417f,  0.98460f },  // Rule 52: NS & NS & HI & LO
        {  5,  5,  3,  2, -0.139236f,  0.196063f,  0.98405f },  // Rule 53: PM & PM & HI & ME
        {  3,  2,  3,  1,  0.003767f,  0.005104f,  0.98308f },  // Rule 54: ZE & NS & HI & LO
        {  5,  5,  2,  3, -0.026295f,  0.016635f,  0.98287f },  // Rule 55: PM & PM & ME & HI
        {  4,  3,  3,  1, -0.004331f,  0.008866f,  0.98279f },  // Rule 56: PS & ZE & HI & LO
        {  1,  1,  1,  4,  0.226143f,  0.039127f,  0.98271f },  // Rule 57: NM & NM & LO & VH
        {  4,  2,  3,  1,  0.001454f,  0.015249f,  0.98169f },  // Rule 58: PS & NS & HI & LO
        {  4,  2,  1,  3, -0.001437f,  0.017290f,  0.98163f },  // Rule 59: PS & NS & LO & HI
        {  5,  2,  3,  3, -0.015991f,  0.047870f,  0.98102f },  // Rule 60: PM & NS & HI & HI
        {  4,  4,  3,  1, -0.010573f,  0.004976f,  0.98055f },  // Rule 61: PS & PS & HI & LO
        {  5,  5,  2,  1, -0.314269f,  0.443218f,  0.97970f },  // Rule 62: PM & PM & ME & LO
        {  1,  5,  1,  1, -0.061587f, -0.174675f,  0.97965f },  // Rule 63: NM & PM & LO & LO
        {  5,  1,  3,  3, -0.012736f,  0.050478f,  0.97928f },  // Rule 64: PM & NM & HI & HI
        {  2,  3,  3,  1,  0.001252f, -0.011204f,  0.97911f },  // Rule 65: NS & ZE & HI & LO
        {  2,  3,  2,  3, -0.000015f, -0.005046f,  0.97901f },  // Rule 66: NS & ZE & ME & HI
        {  4,  5,  3,  3, -0.021166f, -0.001305f,  0.97886f },  // Rule 67: PS & PM & HI & HI
        {  5,  1,  1,  1,  0.130074f,  0.266321f,  0.97825f },  // Rule 68: PM & NM & LO & LO
        {  3,  2,  1,  3, -0.001289f,  0.007690f,  0.97822f },  // Rule 69: ZE & NS & LO & HI
        {  1,  5,  3,  4, -0.056332f, -0.075812f,  0.97785f },  // Rule 70: NM & PM & HI & VH
        {  5,  5,  1,  1, -0.027789f,  0.002365f,  0.97752f },  // Rule 71: PM & PM & LO & LO
        {  1,  2,  3,  3,  0.047545f, -0.070010f,  0.97724f },  // Rule 72: NM & NS & HI & HI
        {  5,  3,  1,  1, -0.021247f,  0.019015f,  0.97453f },  // Rule 73: PM & ZE & LO & LO
        {  2,  4,  3,  1, -0.005022f, -0.016443f,  0.97351f },  // Rule 74: NS & PS & HI & LO
        {  3,  4,  3,  1, -0.006613f, -0.006745f,  0.97222f },  // Rule 75: ZE & PS & HI & LO
        {  2,  5,  3,  3, -0.009116f, -0.017980f,  0.97200f },  // Rule 76: NS & PM & HI & HI
        {  4,  1,  1,  1,  0.005652f,  0.005357f,  0.97154f },  // Rule 77: PS & NM & LO & LO
        {  4,  5,  1,  4, -0.022523f,  0.001971f,  0.97152f },  // Rule 78: PS & PM & LO & VH
        {  1,  2,  1,  4,  0.190601f,  0.017287f,  0.97127f },  // Rule 79: NM & NS & LO & VH
        {  5,  1,  1,  2,  0.126509f,  0.460530f,  0.97069f },  // Rule 80: PM & NM & LO & ME
        {  3,  5,  1,  4, -0.015290f, -0.004518f,  0.97032f },  // Rule 81: ZE & PM & LO & VH
        {  2,  1,  1,  1,  0.004711f, -0.007003f,  0.97017f },  // Rule 82: NS & NM & LO & LO
        {  3,  1,  2,  1, -0.003482f,  0.047549f,  0.97005f },  // Rule 83: ZE & NM & ME & LO
        {  1,  4,  3,  3,  0.015210f, -0.043131f,  0.96920f },  // Rule 84: NM & PS & HI & HI
        {  1,  1,  2,  3,  0.073000f, -0.124487f,  0.96845f },  // Rule 85: NM & NM & ME & HI
        {  2,  4,  1,  3, -0.010939f, -0.011404f,  0.96665f },  // Rule 86: NS & PS & LO & HI
        {  1,  3,  3,  3,  0.033176f, -0.057988f,  0.96658f },  // Rule 87: NM & ZE & HI & HI
        {  5,  2,  1,  1, -0.018000f,  0.013223f,  0.96542f },  // Rule 88: PM & NS & LO & LO
        {  3,  5,  3,  3, -0.013396f, -0.008992f,  0.95945f },  // Rule 89: ZE & PM & HI & HI
        {  1,  5,  2,  3, -0.094821f, -0.270638f,  0.95931f },  // Rule 90: NM & PM & ME & HI
        {  2,  5,  1,  1, -0.018188f, -0.018803f,  0.95892f },  // Rule 91: NS & PM & LO & LO
        {  5,  5,  2,  4, -0.095229f, -0.034161f,  0.95782f },  // Rule 92: PM & PM & ME & VH
        {  5,  5,  2,  2, -0.122528f,  0.163514f,  0.95725f },  // Rule 93: PM & PM & ME & ME
        {  3,  1,  1,  1, -0.000579f, -0.005280f,  0.95667f },  // Rule 94: ZE & NM & LO & LO
        {  1,  3,  1,  1, -0.008148f, -0.079999f,  0.95511f },  // Rule 95: NM & ZE & LO & LO
        {  2,  2,  3,  3,  0.011011f, -0.012103f,  0.95507f },  // Rule 96: NS & NS & HI & HI
        {  5,  4,  3,  3, -0.038129f,  0.061627f,  0.95451f },  // Rule 97: PM & PS & HI & HI
        {  4,  5,  3,  2, -0.021724f,  0.000862f,  0.95439f },  // Rule 98: PS & PM & HI & ME
        {  1,  5,  2,  4, -0.069045f, -0.093332f,  0.95436f },  // Rule 99: NM & PM & ME & VH
        {  2,  1,  2,  3,  0.012000f,  0.004773f,  0.95423f },  // Rule 100: NS & NM & ME & HI
        {  5,  1,  1,  4,  0.180058f,  0.263170f,  0.95384f },  // Rule 101: PM & NM & LO & VH
        {  1,  5,  1,  2, -0.208363f, -0.458336f,  0.95315f },  // Rule 102: NM & PM & LO & ME
        {  1,  1,  2,  2,  0.151064f, -0.251760f,  0.95200f },  // Rule 103: NM & NM & ME & ME
        {  3,  5,  3,  2, -0.024421f, -0.013484f,  0.95100f },  // Rule 104: ZE & PM & HI & ME
        {  4,  1,  1,  2,  0.000601f,  0.029433f,  0.95051f },  // Rule 105: PS & NM & LO & ME
        {  2,  1,  1,  4,  0.195248f,  0.143741f,  0.94945f },  // Rule 106: NS & NM & LO & VH
        {  3,  3,  3,  3, -0.002306f,  0.004226f,  0.94935f },  // Rule 107: ZE & ZE & HI & HI
        {  5,  1,  2,  2, -0.104559f,  0.397303f,  0.94574f },  // Rule 108: PM & NM & ME & ME
        {  5,  4,  2,  2, -0.072040f,  0.114058f,  0.94515f },  // Rule 109: PM & PS & ME & ME
        {  3,  2,  3,  2,  0.004259f,  0.007363f,  0.94394f },  // Rule 110: ZE & NS & HI & ME
        {  4,  1,  2,  3,  0.002437f,  0.034169f,  0.94284f },  // Rule 111: PS & NM & ME & HI
        {  1,  2,  2,  3,  0.041518f, -0.090831f,  0.94192f },  // Rule 112: NM & NS & ME & HI
        {  5,  3,  3,  3, -0.024871f,  0.057845f,  0.94086f },  // Rule 113: PM & ZE & HI & HI
        {  2,  1,  1,  2,  0.010940f,  0.003504f,  0.94051f },  // Rule 114: NS & NM & LO & ME
        {  2,  2,  2,  2,  0.000596f, -0.000615f,  0.93969f },  // Rule 115: NS & NS & ME & ME
        {  1,  5,  2,  2,  0.005594f, -0.408014f,  0.93969f },  // Rule 116: NM & PM & ME & ME
        {  4,  5,  3,  4, -0.057572f, -0.060849f,  0.93885f },  // Rule 117: PS & PM & HI & VH
        {  1,  5,  3,  2,  0.044896f, -0.176930f,  0.93840f },  // Rule 118: NM & PM & HI & ME
        {  1,  3,  2,  2,  0.028544f, -0.075355f,  0.93781f },  // Rule 119: NM & ZE & ME & ME
        {  2,  1,  2,  2,  0.005923f,  0.013343f,  0.93763f },  // Rule 120: NS & NM & ME & ME
        {  5,  1,  2,  1,  0.060601f,  0.446273f,  0.93640f },  // Rule 121: PM & NM & ME & LO
        {  1,  3,  2,  3,  0.026300f, -0.077368f,  0.93514f },  // Rule 122: NM & ZE & ME & HI
        {  2,  1,  3,  3,  0.018222f,  0.000763f,  0.93248f },  // Rule 123: NS & NM & HI & HI
        {  5,  5,  1,  2, -0.116368f,  0.161413f,  0.93242f },  // Rule 124: PM & PM & LO & ME
        {  5,  4,  1,  4,  0.168851f,  0.217698f,  0.93081f },  // Rule 125: PM & PS & LO & VH
        {  1,  4,  1,  4,  0.112231f,  0.012501f,  0.92931f },  // Rule 126: NM & PS & LO & VH
        {  2,  1,  2,  1,  0.015407f, -0.001117f,  0.92889f },  // Rule 127: NS & NM & ME & LO
        {  4,  2,  3,  3, -0.005156f,  0.018439f,  0.92811f },  // Rule 128: PS & NS & HI & HI
        {  1,  2,  1,  1, -0.002434f, -0.096641f,  0.92791f },  // Rule 129: NM & NS & LO & LO
        {  4,  5,  2,  2, -0.025209f,  0.006677f,  0.92496f },  // Rule 130: PS & PM & ME & ME
        {  2,  5,  3,  4, -0.075237f, -0.081038f,  0.92439f },  // Rule 131: NS & PM & HI & VH
        {  5,  4,  1,  1, -0.025481f,  0.004543f,  0.92429f },  // Rule 132: PM & PS & LO & LO
        {  4,  1,  3,  3,  0.004423f,  0.024842f,  0.92401f },  // Rule 133: PS & NM & HI & HI
        {  4,  4,  2,  2, -0.017955f,  0.012930f,  0.92328f },  // Rule 134: PS & PS & ME & ME
        {  1,  4,  1,  1, -0.015592f, -0.086274f,  0.92232f },  // Rule 135: NM & PS & LO & LO
        {  1,  3,  1,  2,  0.000878f, -0.015870f,  0.92080f },  // Rule 136: NM & ZE & LO & ME
        {  3,  3,  2,  2, -0.006452f,  0.000717f,  0.92020f },  // Rule 137: ZE & ZE & ME & ME
        {  3,  5,  1,  1, -0.012306f, -0.016486f,  0.91962f },  // Rule 138: ZE & PM & LO & LO
        {  5,  3,  2,  2, -0.068201f,  0.118303f,  0.91745f },  // Rule 139: PM & ZE & ME & ME
        {  3,  4,  2,  2, -0.011380f,  0.004123f,  0.91733f },  // Rule 140: ZE & PS & ME & ME
        {  3,  5,  2,  3, -0.018218f, -0.008607f,  0.91557f },  // Rule 141: ZE & PM & ME & HI
        {  3,  1,  3,  2,  0.013134f,  0.027815f,  0.91530f },  // Rule 142: ZE & NM & HI & ME
        {  3,  4,  1,  1, -0.020271f, -0.010505f,  0.91287f },  // Rule 143: ZE & PS & LO & LO
        {  2,  3,  3,  3,  0.003669f, -0.009695f,  0.91242f },  // Rule 144: NS & ZE & HI & HI
        {  2,  5,  1,  2, -0.014138f, -0.012064f,  0.91160f },  // Rule 145: NS & PM & LO & ME
        {  2,  3,  3,  2, -0.002834f, -0.012197f,  0.91157f },  // Rule 146: NS & ZE & HI & ME
        {  5,  4,  3,  2, -0.032432f,  0.048288f,  0.90931f },  // Rule 147: PM & PS & HI & ME
        {  3,  1,  1,  2,  0.005641f,  0.020964f,  0.90869f },  // Rule 148: ZE & NM & LO & ME
        {  1,  4,  2,  2,  0.036273f, -0.115582f,  0.90824f },  // Rule 149: NM & PS & ME & ME
        {  3,  3,  2,  3, -0.007958f,  0.006127f,  0.90374f },  // Rule 150: ZE & ZE & ME & HI
        {  4,  1,  1,  4,  0.206005f,  0.177127f,  0.90306f },  // Rule 151: PS & NM & LO & VH
        {  5,  4,  2,  1, -0.148364f,  0.268035f,  0.90243f },  // Rule 152: PM & PS & ME & LO
        {  4,  4,  3,  3, -0.012035f,  0.009373f,  0.90212f },  // Rule 153: PS & PS & HI & HI
        {  3,  4,  3,  2, -0.010962f, -0.000433f,  0.89851f },  // Rule 154: ZE & PS & HI & ME
        {  4,  1,  2,  1, -0.021806f,  0.107733f,  0.89667f },  // Rule 155: PS & NM & ME & LO
        {  4,  5,  1,  1, -0.030557f, -0.013732f,  0.89585f },  // Rule 156: PS & PM & LO & LO
        {  5,  3,  1,  4,  0.170585f,  0.243459f,  0.89555f },  // Rule 157: PM & ZE & LO & VH
        {  3,  4,  3,  3, -0.005978f, -0.001212f,  0.89457f },  // Rule 158: ZE & PS & HI & HI
        {  3,  1,  3,  3,  0.008558f,  0.012925f,  0.89290f },  // Rule 159: ZE & NM & HI & HI
        {  1,  3,  1,  4,  0.149541f, -0.021839f,  0.89218f },  // Rule 160: NM & ZE & LO & VH
        {  3,  1,  2,  2, -0.001822f,  0.034387f,  0.89195f },  // Rule 161: ZE & NM & ME & ME
        {  4,  3,  3,  3, -0.008795f,  0.010746f,  0.89156f },  // Rule 162: PS & ZE & HI & HI
        {  4,  2,  2,  2, -0.008840f,  0.024726f,  0.89055f },  // Rule 163: PS & NS & ME & ME
        {  1,  2,  1,  2,  0.004826f, -0.018824f,  0.88960f },  // Rule 164: NM & NS & LO & ME
        {  2,  5,  3,  2, -0.009848f, -0.016057f,  0.88950f },  // Rule 165: NS & PM & HI & ME
        {  1,  2,  3,  2,  0.054924f, -0.091827f,  0.88883f },  // Rule 166: NM & NS & HI & ME
        {  2,  3,  2,  2, -0.005792f, -0.003356f,  0.88873f },  // Rule 167: NS & ZE & ME & ME
        {  2,  2,  1,  1, -0.016521f, -0.010559f,  0.88584f },  // Rule 168: NS & NS & LO & LO
        {  3,  1,  1,  4,  0.218707f,  0.188938f,  0.88431f },  // Rule 169: ZE & NM & LO & VH
        {  5,  3,  2,  3, -0.013084f,  0.037786f,  0.88347f },  // Rule 170: PM & ZE & ME & HI
        {  1,  5,  2,  1, -0.179392f, -0.443344f,  0.88227f },  // Rule 171: NM & PM & ME & LO
        {  3,  5,  2,  1, -0.017284f, -0.020173f,  0.87541f },  // Rule 172: ZE & PM & ME & LO
        {  2,  4,  3,  3, -0.000818f, -0.008672f,  0.87532f },  // Rule 173: NS & PS & HI & HI
        {  5,  4,  1,  2, -0.053655f,  0.095001f,  0.87361f },  // Rule 174: PM & PS & LO & ME
        {  3,  3,  1,  1, -0.014765f, -0.018083f,  0.87299f },  // Rule 175: ZE & ZE & LO & LO
        {  3,  2,  2,  2, -0.004917f,  0.008969f,  0.87244f },  // Rule 176: ZE & NS & ME & ME
        {  3,  1,  2,  3,  0.008384f,  0.025477f,  0.87194f },  // Rule 177: ZE & NM & ME & HI
        {  5,  1,  3,  2, -0.004753f,  0.052753f,  0.86921f },  // Rule 178: PM & NM & HI & ME
        {  1,  2,  2,  2,  0.037633f, -0.080287f,  0.86836f },  // Rule 179: NM & NS & ME & ME
        {  3,  2,  1,  1, -0.006976f, -0.007400f,  0.86831f },  // Rule 180: ZE & NS & LO & LO
        {  4,  2,  1,  1, -0.007553f,  0.003626f,  0.86621f },  // Rule 181: PS & NS & LO & LO
        {  2,  2,  1,  4,  0.133915f,  0.112810f,  0.86594f },  // Rule 182: NS & NS & LO & VH
        {  5,  4,  2,  3, -0.011835f,  0.029238f,  0.86511f },  // Rule 183: PM & PS & ME & HI
        {  5,  1,  2,  3,  0.102677f,  0.161760f,  0.86391f },  // Rule 184: PM & NM & ME & HI
        {  3,  5,  2,  4, -0.090161f, -0.091349f,  0.86154f },  // Rule 185: ZE & PM & ME & VH
        {  3,  4,  2,  3, -0.012237f, -0.006939f,  0.85971f },  // Rule 186: ZE & PS & ME & HI
        {  5,  2,  2,  2, -0.083545f,  0.172948f,  0.85478f },  // Rule 187: PM & NS & ME & ME
        {  4,  5,  2,  3, -0.020635f,  0.000627f,  0.85385f },  // Rule 188: PS & PM & ME & HI
        {  4,  4,  2,  1,  0.000919f, -0.005419f,  0.85296f },  // Rule 189: PS & PS & ME & LO
        {  3,  2,  3,  3,  0.003214f,  0.008633f,  0.85269f },  // Rule 190: ZE & NS & HI & HI
        {  2,  4,  1,  1, -0.019878f, -0.023449f,  0.85038f },  // Rule 191: NS & PS & LO & LO
        {  2,  4,  2,  3, -0.008253f, -0.010438f,  0.84984f },  // Rule 192: NS & PS & ME & HI
        {  3,  5,  1,  2, -0.018853f, -0.007731f,  0.84769f },  // Rule 193: ZE & PM & LO & ME
        {  5,  3,  2,  1, -0.123324f,  0.283576f,  0.84721f },  // Rule 194: PM & ZE & ME & LO
        {  4,  1,  2,  2, -0.019106f,  0.071011f,  0.84646f },  // Rule 195: PS & NM & ME & ME
        {  5,  2,  2,  3, -0.006013f,  0.026484f,  0.84633f },  // Rule 196: PM & NS & ME & HI
        {  1,  4,  1,  2, -0.004007f, -0.022787f,  0.84255f },  // Rule 197: NM & PS & LO & ME
        {  2,  5,  2,  2, -0.015793f, -0.010327f,  0.84243f },  // Rule 198: NS & PM & ME & ME
        {  5,  2,  1,  4,  0.169015f,  0.251613f,  0.84232f },  // Rule 199: PM & NS & LO & VH
        {  2,  1,  3,  2,  0.009215f,  0.016726f,  0.84079f },  // Rule 200: NS & NM & HI & ME
        {  4,  4,  1,  4,  0.207010f,  0.170561f,  0.84067f },  // Rule 201: PS & PS & LO & VH
        {  2,  2,  2,  3,  0.002964f,  0.000132f,  0.84063f },  // Rule 202: NS & NS & ME & HI
        {  5,  3,  3,  2, -0.022462f,  0.054001f,  0.84045f },  // Rule 203: PM & ZE & HI & ME
        {  4,  3,  1,  1, -0.015034f, -0.002675f,  0.83968f },  // Rule 204: PS & ZE & LO & LO
        {  4,  3,  2,  1, -0.013931f,  0.012359f,  0.83788f },  // Rule 205: PS & ZE & ME & LO
        {  3,  4,  2,  1, -0.009792f,  0.000855f,  0.83620f },  // Rule 206: ZE & PS & ME & LO
        {  2,  5,  2,  3, -0.016934f, -0.019360f,  0.83325f },  // Rule 207: NS & PM & ME & HI
        {  5,  3,  1,  2, -0.048362f,  0.101188f,  0.83147f },  // Rule 208: PM & ZE & LO & ME
        {  1,  4,  2,  3,  0.031679f, -0.110713f,  0.83110f },  // Rule 209: NM & PS & ME & HI
        {  4,  3,  2,  2, -0.010158f,  0.014717f,  0.82518f },  // Rule 210: PS & ZE & ME & ME
        {  2,  2,  2,  1,  0.007799f, -0.006930f,  0.82385f },  // Rule 211: NS & NS & ME & LO
        {  4,  5,  1,  2, -0.015782f,  0.000319f,  0.82218f },  // Rule 212: PS & PM & LO & ME
        {  5,  2,  3,  2, -0.018665f,  0.053981f,  0.81845f },  // Rule 213: PM & NS & HI & ME
        {  4,  2,  2,  3,  0.000325f,  0.019462f,  0.81745f },  // Rule 214: PS & NS & ME & HI
        {  4,  4,  3,  2, -0.018086f,  0.007662f,  0.81228f },  // Rule 215: PS & PS & HI & ME
        {  3,  3,  1,  2, -0.001106f,  0.002088f,  0.81107f },  // Rule 216: ZE & ZE & LO & ME
        {  2,  4,  2,  2, -0.008004f, -0.003713f,  0.81028f },  // Rule 217: NS & PS & ME & ME
        {  5,  2,  1,  2, -0.050816f,  0.137795f,  0.81018f },  // Rule 218: PM & NS & LO & ME
        {  4,  4,  2,  3, -0.012244f,  0.004443f,  0.80785f },  // Rule 219: PS & PS & ME & HI
        {  4,  3,  3,  2, -0.001140f,  0.011096f,  0.80520f },  // Rule 220: PS & ZE & HI & ME
        {  4,  1,  3,  2, -0.007513f,  0.055475f,  0.79723f },  // Rule 221: PS & NM & HI & ME
        {  4,  4,  1,  2, -0.008618f,  0.007273f,  0.78706f },  // Rule 222: PS & PS & LO & ME
        {  3,  5,  2,  2, -0.020502f, -0.004791f,  0.78620f },  // Rule 223: ZE & PM & ME & ME
        {  2,  3,  2,  1,  0.001078f, -0.004625f,  0.78156f },  // Rule 224: NS & ZE & ME & LO
        {  1,  3,  3,  2,  0.048381f, -0.100378f,  0.77972f },  // Rule 225: NM & ZE & HI & ME
        {  2,  3,  1,  2,  0.001283f, -0.007120f,  0.77564f },  // Rule 226: NS & ZE & LO & ME
        {  1,  2,  2,  1,  0.010799f, -0.019737f,  0.77118f },  // Rule 227: NM & NS & ME & LO
        {  3,  2,  2,  3,  0.000297f,  0.003929f,  0.76529f },  // Rule 228: ZE & NS & ME & HI
        {  2,  5,  2,  1, -0.000019f, -0.042422f,  0.76451f },  // Rule 229: NS & PM & ME & LO
        {  4,  4,  1,  1, -0.019525f,  0.000149f,  0.76246f },  // Rule 230: PS & PS & LO & LO
        {  2,  2,  1,  2,  0.003024f, -0.005060f,  0.75740f },  // Rule 231: NS & NS & LO & ME
        {  2,  2,  3,  2,  0.005731f, -0.003167f,  0.75607f },  // Rule 232: NS & NS & HI & ME
        {  3,  4,  1,  2, -0.018784f,  0.001732f,  0.75334f },  // Rule 233: ZE & PS & LO & ME
        {  4,  2,  1,  2, -0.004743f,  0.010470f,  0.75171f },  // Rule 234: PS & NS & LO & ME
        {  4,  5,  2,  4, -0.061673f, -0.048539f,  0.74674f },  // Rule 235: PS & PM & ME & VH
        {  3,  2,  1,  2, -0.000333f, -0.002244f,  0.74635f },  // Rule 236: ZE & NS & LO & ME
        {  2,  4,  2,  1, -0.014713f, -0.019981f,  0.73956f },  // Rule 237: NS & PS & ME & LO
        {  2,  4,  1,  4,  0.112373f,  0.072020f,  0.73885f },  // Rule 238: NS & PS & LO & VH
        {  1,  4,  3,  2,  0.048977f, -0.109699f,  0.73379f },  // Rule 239: NM & PS & HI & ME
        {  5,  2,  2,  1, -0.165974f,  0.387939f,  0.73318f },  // Rule 240: PM & NS & ME & LO
        {  3,  3,  3,  2, -0.003964f,  0.009685f,  0.73063f },  // Rule 241: ZE & ZE & HI & ME
        {  4,  3,  2,  3, -0.008331f,  0.012195f,  0.72687f },  // Rule 242: PS & ZE & ME & HI
        {  4,  2,  3,  2, -0.004595f,  0.019142f,  0.72606f },  // Rule 243: PS & NS & HI & ME
        {  2,  4,  1,  2,  0.000434f, -0.007080f,  0.72116f },  // Rule 244: NS & PS & LO & ME
        {  2,  3,  1,  1, -0.014435f, -0.009025f,  0.71635f },  // Rule 245: NS & ZE & LO & LO
        {  1,  4,  2,  1,  0.011189f, -0.014910f,  0.71586f },  // Rule 246: NM & PS & ME & LO
        {  4,  2,  2,  1, -0.008056f,  0.010685f,  0.71207f },  // Rule 247: PS & NS & ME & LO
        {  4,  3,  1,  2, -0.002271f,  0.010421f,  0.71182f },  // Rule 248: PS & ZE & LO & ME
        {  2,  5,  2,  4, -0.062681f, -0.074994f,  0.70628f },  // Rule 249: NS & PM & ME & VH
        {  3,  3,  2,  1, -0.000785f, -0.014204f,  0.69118f },  // Rule 250: ZE & ZE & ME & LO
        {  2,  4,  3,  2, -0.011624f, -0.012535f,  0.68359f },  // Rule 251: NS & PS & HI & ME
        {  2,  3,  1,  4,  0.168718f,  0.114265f,  0.68243f },  // Rule 252: NS & ZE & LO & VH
        {  3,  2,  2,  1, -0.002307f,  0.000254f,  0.66600f },  // Rule 253: ZE & NS & ME & LO
        {  4,  5,  2,  1, -0.017198f,  0.002844f,  0.65246f },  // Rule 254: PS & PM & ME & LO
        {  1,  3,  2,  1, -0.009106f, -0.011515f,  0.64244f },  // Rule 255: NM & ZE & ME & LO
        {  4,  2,  1,  4,  0.174383f,  0.152235f,  0.61056f },  // Rule 256: PS & NS & LO & VH
        {  3,  3,  1,  4,  0.207105f,  0.157284f,  0.59581f },  // Rule 257: ZE & ZE & LO & VH
    };

    private const int N_RULES = 257;

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
            // 无规则匹配时，输出零（保持当前状态）
            outputVx = 0f;
            outputOmega = 0f;
        }
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
        return $"FuzzyController: {N_RULES} rules, generated 2026-03-12 13:32:57";
    }
}