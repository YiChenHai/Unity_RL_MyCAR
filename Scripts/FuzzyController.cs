// 模糊控制器 - 自动生成
// 生成时间: 2026-03-10 20:30:01
// 数据样本数: 300000
// 规则数: 332
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
        { -1.00000f, -1.00000f,  0.03615f },  // NB
        { -1.00000f,  0.03615f,  0.04435f },  // NM
        {  0.03615f,  0.04435f,  0.04993f },  // NS
        {  0.04435f,  0.04993f,  0.05497f },  // ZE
        {  0.04993f,  0.05497f,  0.06052f },  // PS
        {  0.05497f,  0.06052f,  1.00000f },  // PM
        {  0.06052f,  1.00000f,  1.00000f },  // PB
    };

    // rear_lr_diff: NB, NM, NS, ZE, PS, PM, PB
    private static readonly float[,] MF_RearLR = {
        { -1.00000f, -1.00000f, -0.04813f },  // NB
        { -1.00000f, -0.04813f, -0.04083f },  // NM
        { -0.04813f, -0.04083f, -0.03420f },  // NS
        { -0.04083f, -0.03420f, -0.02680f },  // ZE
        { -0.03420f, -0.02680f, -0.01558f },  // PS
        { -0.02680f, -0.01558f,  1.00000f },  // PM
        { -0.01558f,  1.00000f,  1.00000f },  // PB
    };

    // front_center: VL, LO, ME, HI, VH
    private static readonly float[,] MF_FrontCenter = {
        {  0.00000f,  0.00000f,  0.84846f },  // VL
        {  0.00000f,  0.84846f,  0.85220f },  // LO
        {  0.84846f,  0.85220f,  0.85422f },  // ME
        {  0.85220f,  0.85422f,  1.00000f },  // HI
        {  0.85422f,  1.00000f,  1.00000f },  // VH
    };

    // rear_center: VL, LO, ME, HI, VH
    private static readonly float[,] MF_RearCenter = {
        {  0.00000f,  0.00000f,  0.84909f },  // VL
        {  0.00000f,  0.84909f,  0.85142f },  // LO
        {  0.84909f,  0.85142f,  0.85457f },  // ME
        {  0.85142f,  0.85457f,  1.00000f },  // HI
        {  0.85457f,  1.00000f,  1.00000f },  // VH
    };

    // ===== 规则库 (332 条规则) =====
    // 每行: { 前LR集, 后LR集, 前中集, 后中集, 输出vx, 输出omega, 权重 }
    private static readonly float[,] Rules = {
        {  1,  1,  3,  1,  0.206871f, -0.162529f,  0.99835f },  // Rule 1: NM & NM & HI & LO
        {  5,  3,  1,  3, -0.016187f,  0.040794f,  0.99782f },  // Rule 2: PM & ZE & LO & HI
        {  5,  1,  1,  3,  0.020076f,  0.176673f,  0.99745f },  // Rule 3: PM & NM & LO & HI
        {  5,  5,  1,  3, -0.127422f, -0.014942f,  0.99707f },  // Rule 4: PM & PM & LO & HI
        {  1,  1,  1,  3,  0.218852f,  0.125542f,  0.99620f },  // Rule 5: NM & NM & LO & HI
        {  5,  4,  1,  3, -0.027447f,  0.031340f,  0.99619f },  // Rule 6: PM & PS & LO & HI
        {  5,  2,  1,  3, -0.009562f,  0.051306f,  0.99576f },  // Rule 7: PM & NS & LO & HI
        {  5,  5,  3,  1, -0.258623f,  0.210397f,  0.99550f },  // Rule 8: PM & PM & HI & LO
        {  5,  1,  3,  1, -0.016047f,  0.238200f,  0.99539f },  // Rule 9: PM & NM & HI & LO
        {  2,  1,  1,  3,  0.083853f,  0.084799f,  0.99461f },  // Rule 10: NS & NM & LO & HI
        {  3,  1,  3,  1,  0.019615f,  0.033250f,  0.99359f },  // Rule 11: ZE & NM & HI & LO
        {  4,  1,  3,  1,  0.013662f,  0.050199f,  0.99358f },  // Rule 12: PS & NM & HI & LO
        {  5,  4,  3,  3, -0.090270f,  0.024820f,  0.99356f },  // Rule 13: PM & PS & HI & HI
        {  5,  1,  1,  1,  0.022241f,  0.321855f,  0.99342f },  // Rule 14: PM & NM & LO & LO
        {  3,  5,  1,  3, -0.089989f, -0.089208f,  0.99192f },  // Rule 15: ZE & PM & LO & HI
        {  5,  2,  3,  1, -0.031656f,  0.066154f,  0.99139f },  // Rule 16: PM & NS & HI & LO
        {  1,  5,  1,  3, -0.041459f, -0.210620f,  0.99133f },  // Rule 17: NM & PM & LO & HI
        {  1,  5,  3,  1, -0.089990f, -0.147057f,  0.99095f },  // Rule 18: NM & PM & HI & LO
        {  1,  4,  3,  1,  0.054161f, -0.141621f,  0.99082f },  // Rule 19: NM & PS & HI & LO
        {  4,  5,  1,  3, -0.091956f, -0.073420f,  0.99057f },  // Rule 20: PS & PM & LO & HI
        {  2,  5,  1,  1,  0.007162f, -0.010759f,  0.99051f },  // Rule 21: NS & PM & LO & LO
        {  5,  3,  3,  3, -0.072375f,  0.028210f,  0.98991f },  // Rule 22: PM & ZE & HI & HI
        {  2,  3,  3,  1, -0.002184f, -0.005240f,  0.98955f },  // Rule 23: NS & ZE & HI & LO
        {  1,  1,  1,  4,  0.161896f, -0.060076f,  0.98836f },  // Rule 24: NM & NM & LO & VH
        {  4,  4,  1,  3, -0.003728f,  0.001685f,  0.98822f },  // Rule 25: PS & PS & LO & HI
        {  1,  1,  1,  1,  0.195778f, -0.037490f,  0.98818f },  // Rule 26: NM & NM & LO & LO
        {  1,  5,  1,  1, -0.033950f, -0.225533f,  0.98803f },  // Rule 27: NM & PM & LO & LO
        {  1,  3,  3,  1,  0.071927f, -0.120590f,  0.98801f },  // Rule 28: NM & ZE & HI & LO
        {  5,  3,  3,  1, -0.041746f,  0.058543f,  0.98758f },  // Rule 29: PM & ZE & HI & LO
        {  3,  1,  1,  4,  0.070044f,  0.106001f,  0.98757f },  // Rule 30: ZE & NM & LO & VH
        {  2,  1,  3,  1,  0.030180f,  0.020688f,  0.98735f },  // Rule 31: NS & NM & HI & LO
        {  1,  2,  3,  1,  0.087967f, -0.115188f,  0.98726f },  // Rule 32: NM & NS & HI & LO
        {  4,  1,  1,  3,  0.044888f,  0.075280f,  0.98702f },  // Rule 33: PS & NM & LO & HI
        {  3,  4,  1,  3,  0.001247f, -0.007310f,  0.98648f },  // Rule 34: ZE & PS & LO & HI
        {  4,  2,  1,  3,  0.008229f,  0.020533f,  0.98568f },  // Rule 35: PS & NS & LO & HI
        {  5,  5,  1,  1, -0.153742f,  0.099977f,  0.98430f },  // Rule 36: PM & PM & LO & LO
        {  3,  5,  3,  1, -0.147974f,  0.024470f,  0.98373f },  // Rule 37: ZE & PM & HI & LO
        {  4,  3,  1,  3, -0.001312f,  0.006904f,  0.98314f },  // Rule 38: PS & ZE & LO & HI
        {  5,  4,  3,  1, -0.064457f,  0.059961f,  0.98312f },  // Rule 39: PM & PS & HI & LO
        {  4,  5,  3,  1, -0.162496f,  0.049065f,  0.98208f },  // Rule 40: PS & PM & HI & LO
        {  5,  1,  2,  1, -0.002760f,  0.175423f,  0.98171f },  // Rule 41: PM & NM & ME & LO
        {  3,  1,  1,  3,  0.058040f,  0.073316f,  0.98160f },  // Rule 42: ZE & NM & LO & HI
        {  1,  2,  1,  1,  0.069494f,  0.000288f,  0.98157f },  // Rule 43: NM & NS & LO & LO
        {  3,  3,  1,  3,  0.008734f,  0.003637f,  0.98069f },  // Rule 44: ZE & ZE & LO & HI
        {  2,  2,  1,  3,  0.027724f,  0.010401f,  0.98043f },  // Rule 45: NS & NS & LO & HI
        {  1,  2,  1,  3,  0.103682f,  0.024603f,  0.98027f },  // Rule 46: NM & NS & LO & HI
        {  1,  3,  1,  3,  0.105926f,  0.024780f,  0.97810f },  // Rule 47: NM & ZE & LO & HI
        {  1,  5,  3,  3, -0.018761f, -0.173285f,  0.97805f },  // Rule 48: NM & PM & HI & HI
        {  1,  1,  3,  4,  0.059563f, -0.040077f,  0.97700f },  // Rule 49: NM & NM & HI & VH
        {  3,  3,  3,  1, -0.008114f,  0.003866f,  0.97673f },  // Rule 50: ZE & ZE & HI & LO
        {  4,  5,  3,  3, -0.101843f, -0.042754f,  0.97658f },  // Rule 51: PS & PM & HI & HI
        {  3,  2,  1,  3,  0.019578f,  0.016193f,  0.97512f },  // Rule 52: ZE & NS & LO & HI
        {  3,  4,  3,  1, -0.016915f, -0.007888f,  0.97475f },  // Rule 53: ZE & PS & HI & LO
        {  1,  5,  3,  2, -0.005443f, -0.215108f,  0.97423f },  // Rule 54: NM & PM & HI & ME
        {  1,  4,  1,  3,  0.112553f,  0.010579f,  0.97415f },  // Rule 55: NM & PS & LO & HI
        {  5,  2,  1,  4, -0.084470f,  0.124702f,  0.97372f },  // Rule 56: PM & NS & LO & VH
        {  5,  1,  1,  4,  0.082026f,  0.416053f,  0.97352f },  // Rule 57: PM & NM & LO & VH
        {  1,  2,  3,  3,  0.062255f, -0.086095f,  0.97301f },  // Rule 58: NM & NS & HI & HI
        {  2,  5,  3,  1, -0.140526f, -0.010813f,  0.97289f },  // Rule 59: NS & PM & HI & LO
        {  5,  5,  3,  3, -0.151595f,  0.012782f,  0.97252f },  // Rule 60: PM & PM & HI & HI
        {  5,  5,  1,  2, -0.237533f, -0.051665f,  0.97231f },  // Rule 61: PM & PM & LO & ME
        {  1,  3,  3,  2,  0.123882f, -0.092554f,  0.97220f },  // Rule 62: NM & ZE & HI & ME
        {  1,  5,  1,  4,  0.009520f, -0.311461f,  0.96988f },  // Rule 63: NM & PM & LO & VH
        {  2,  4,  1,  3,  0.021776f, -0.001181f,  0.96958f },  // Rule 64: NS & PS & LO & HI
        {  1,  1,  3,  3,  0.144473f, -0.098831f,  0.96917f },  // Rule 65: NM & NM & HI & HI
        {  2,  3,  1,  3,  0.025130f,  0.005699f,  0.96668f },  // Rule 66: NS & ZE & LO & HI
        {  2,  4,  3,  1, -0.013905f, -0.015011f,  0.96648f },  // Rule 67: NS & PS & HI & LO
        {  5,  1,  3,  3, -0.039672f,  0.099641f,  0.96622f },  // Rule 68: PM & NM & HI & HI
        {  2,  5,  1,  3, -0.098068f, -0.115885f,  0.96418f },  // Rule 69: NS & PM & LO & HI
        {  4,  2,  3,  1, -0.008750f,  0.022186f,  0.96230f },  // Rule 70: PS & NS & HI & LO
        {  2,  1,  1,  1,  0.074571f,  0.058316f,  0.96151f },  // Rule 71: NS & NM & LO & LO
        {  1,  3,  1,  4,  0.057194f, -0.092717f,  0.96116f },  // Rule 72: NM & ZE & LO & VH
        {  5,  3,  1,  1,  0.026634f,  0.065732f,  0.96035f },  // Rule 73: PM & ZE & LO & LO
        {  1,  1,  2,  2,  0.192838f,  0.001876f,  0.95959f },  // Rule 74: NM & NM & ME & ME
        {  5,  5,  1,  4, -0.131110f,  0.059145f,  0.95842f },  // Rule 75: PM & PM & LO & VH
        {  1,  4,  1,  4,  0.045746f, -0.086868f,  0.95820f },  // Rule 76: NM & PS & LO & VH
        {  1,  1,  3,  2,  0.145274f, -0.107900f,  0.95558f },  // Rule 77: NM & NM & HI & ME
        {  1,  5,  2,  3,  0.055450f, -0.153054f,  0.95544f },  // Rule 78: NM & PM & ME & HI
        {  1,  3,  3,  3,  0.057813f, -0.093277f,  0.95508f },  // Rule 79: NM & ZE & HI & HI
        {  1,  1,  2,  1,  0.202201f,  0.006409f,  0.95505f },  // Rule 80: NM & NM & ME & LO
        {  1,  1,  2,  3,  0.233486f, -0.016347f,  0.95455f },  // Rule 81: NM & NM & ME & HI
        {  4,  4,  1,  1,  0.038165f,  0.025102f,  0.95330f },  // Rule 82: PS & PS & LO & LO
        {  5,  1,  3,  2, -0.031346f,  0.095965f,  0.95311f },  // Rule 83: PM & NM & HI & ME
        {  4,  1,  1,  1,  0.068127f,  0.071812f,  0.95258f },  // Rule 84: PS & NM & LO & LO
        {  1,  3,  1,  1,  0.064672f, -0.003649f,  0.95194f },  // Rule 85: NM & ZE & LO & LO
        {  1,  5,  3,  4, -0.087263f, -0.331311f,  0.95179f },  // Rule 86: NM & PM & HI & VH
        {  3,  5,  3,  3, -0.100033f, -0.053864f,  0.95145f },  // Rule 87: ZE & PM & HI & HI
        {  2,  2,  3,  1,  0.002633f, -0.001552f,  0.95081f },  // Rule 88: NS & NS & HI & LO
        {  2,  1,  2,  1,  0.079841f,  0.109835f,  0.95059f },  // Rule 89: NS & NM & ME & LO
        {  1,  3,  3,  4, -0.007694f, -0.035396f,  0.94931f },  // Rule 90: NM & ZE & HI & VH
        {  3,  2,  3,  1, -0.002248f,  0.009619f,  0.94797f },  // Rule 91: ZE & NS & HI & LO
        {  2,  1,  3,  3,  0.006316f, -0.014440f,  0.94647f },  // Rule 92: NS & NM & HI & HI
        {  1,  1,  1,  2,  0.221454f,  0.084674f,  0.94524f },  // Rule 93: NM & NM & LO & ME
        {  5,  1,  1,  2,  0.009397f,  0.275111f,  0.94434f },  // Rule 94: PM & NM & LO & ME
        {  4,  3,  3,  1, -0.014626f,  0.013112f,  0.94253f },  // Rule 95: PS & ZE & HI & LO
        {  1,  4,  3,  3,  0.048775f, -0.101196f,  0.94194f },  // Rule 96: NM & PS & HI & HI
        {  3,  1,  1,  1,  0.069976f,  0.061196f,  0.94135f },  // Rule 97: ZE & NM & LO & LO
        {  5,  3,  2,  3, -0.036028f,  0.028986f,  0.94105f },  // Rule 98: PM & ZE & ME & HI
        {  5,  5,  2,  1, -0.166227f,  0.006893f,  0.94033f },  // Rule 99: PM & PM & ME & LO
        {  5,  2,  3,  3, -0.060501f,  0.034517f,  0.93871f },  // Rule 100: PM & NS & HI & HI
        {  1,  4,  1,  1,  0.059934f, -0.010279f,  0.93697f },  // Rule 101: NM & PS & LO & LO
        {  5,  1,  2,  2,  0.025076f,  0.162068f,  0.93639f },  // Rule 102: PM & NM & ME & ME
        {  5,  2,  2,  1, -0.024675f,  0.045683f,  0.93609f },  // Rule 103: PM & NS & ME & LO
        {  5,  5,  2,  3, -0.203076f,  0.020420f,  0.93473f },  // Rule 104: PM & PM & ME & HI
        {  4,  5,  1,  1, -0.008910f,  0.006133f,  0.93448f },  // Rule 105: PS & PM & LO & LO
        {  2,  5,  2,  3, -0.122506f, -0.088570f,  0.93440f },  // Rule 106: NS & PM & ME & HI
        {  3,  5,  1,  1,  0.003130f, -0.000172f,  0.93313f },  // Rule 107: ZE & PM & LO & LO
        {  2,  5,  1,  4, -0.097314f, -0.045754f,  0.93225f },  // Rule 108: NS & PM & LO & VH
        {  4,  4,  3,  1, -0.023277f, -0.003995f,  0.93124f },  // Rule 109: PS & PS & HI & LO
        {  1,  5,  2,  2, -0.055940f, -0.269694f,  0.92988f },  // Rule 110: NM & PM & ME & ME
        {  1,  5,  2,  1, -0.095828f, -0.273532f,  0.92979f },  // Rule 111: NM & PM & ME & LO
        {  5,  2,  1,  1,  0.033670f,  0.060002f,  0.92935f },  // Rule 112: PM & NS & LO & LO
        {  1,  5,  1,  2, -0.093050f, -0.332681f,  0.92919f },  // Rule 113: NM & PM & LO & ME
        {  2,  4,  1,  4, -0.036784f,  0.025055f,  0.92479f },  // Rule 114: NS & PS & LO & VH
        {  5,  4,  2,  3, -0.044933f,  0.024401f,  0.92283f },  // Rule 115: PM & PS & ME & HI
        {  5,  5,  3,  2, -0.247300f,  0.178780f,  0.92260f },  // Rule 116: PM & PM & HI & ME
        {  5,  2,  2,  3, -0.029251f,  0.027193f,  0.92138f },  // Rule 117: PM & NS & ME & HI
        {  2,  1,  2,  3,  0.028032f,  0.004999f,  0.92038f },  // Rule 118: NS & NM & ME & HI
        {  5,  1,  3,  4,  0.003818f,  0.349222f,  0.91955f },  // Rule 119: PM & NM & HI & VH
        {  4,  1,  2,  2,  0.051836f,  0.110313f,  0.91936f },  // Rule 120: PS & NM & ME & ME
        {  2,  2,  1,  1,  0.050228f,  0.028707f,  0.91868f },  // Rule 121: NS & NS & LO & LO
        {  2,  5,  1,  2, -0.148317f, -0.175724f,  0.91803f },  // Rule 122: NS & PM & LO & ME
        {  4,  2,  1,  1,  0.045313f,  0.038555f,  0.91635f },  // Rule 123: PS & NS & LO & LO
        {  5,  4,  1,  1,  0.018432f,  0.067256f,  0.91368f },  // Rule 124: PM & PS & LO & LO
        {  2,  3,  1,  1,  0.045526f,  0.024737f,  0.91310f },  // Rule 125: NS & ZE & LO & LO
        {  1,  4,  2,  1,  0.146810f, -0.119767f,  0.91276f },  // Rule 126: NM & PS & ME & LO
        {  1,  1,  2,  4,  0.061108f, -0.019070f,  0.91187f },  // Rule 127: NM & NM & ME & VH
        {  2,  2,  3,  3, -0.021908f, -0.024373f,  0.91112f },  // Rule 128: NS & NS & HI & HI
        {  5,  3,  1,  4, -0.056088f,  0.209076f,  0.91080f },  // Rule 129: PM & ZE & LO & VH
        {  3,  5,  2,  1, -0.239914f, -0.187338f,  0.90990f },  // Rule 130: ZE & PM & ME & LO
        {  2,  5,  2,  2, -0.166427f, -0.177640f,  0.90832f },  // Rule 131: NS & PM & ME & ME
        {  3,  2,  2,  2,  0.023389f,  0.045107f,  0.90794f },  // Rule 132: ZE & NS & ME & ME
        {  5,  2,  3,  2, -0.037742f,  0.035839f,  0.90700f },  // Rule 133: PM & NS & HI & ME
        {  4,  5,  1,  4, -0.097245f, -0.020863f,  0.90700f },  // Rule 134: PS & PM & LO & VH
        {  3,  1,  2,  3,  0.015618f,  0.006655f,  0.90691f },  // Rule 135: ZE & NM & ME & HI
        {  5,  5,  2,  2, -0.182560f,  0.006422f,  0.90678f },  // Rule 136: PM & PM & ME & ME
        {  3,  2,  1,  1,  0.049413f,  0.038131f,  0.90456f },  // Rule 137: ZE & NS & LO & LO
        {  3,  4,  1,  1,  0.039095f,  0.030629f,  0.90334f },  // Rule 138: ZE & PS & LO & LO
        {  2,  1,  3,  2,  0.019250f, -0.005937f,  0.90120f },  // Rule 139: NS & NM & HI & ME
        {  1,  2,  2,  2,  0.134089f, -0.048515f,  0.90025f },  // Rule 140: NM & NS & ME & ME
        {  1,  4,  2,  3,  0.172751f,  0.014158f,  0.89863f },  // Rule 141: NM & PS & ME & HI
        {  2,  2,  3,  2,  0.004515f, -0.019188f,  0.89757f },  // Rule 142: NS & NS & HI & ME
        {  5,  4,  3,  2, -0.114364f,  0.089722f,  0.89721f },  // Rule 143: PM & PS & HI & ME
        {  5,  4,  1,  2, -0.048038f,  0.074308f,  0.89624f },  // Rule 144: PM & PS & LO & ME
        {  4,  3,  3,  3, -0.047976f, -0.010036f,  0.89577f },  // Rule 145: PS & ZE & HI & HI
        {  4,  3,  1,  1,  0.044131f,  0.041702f,  0.89480f },  // Rule 146: PS & ZE & LO & LO
        {  5,  4,  2,  1, -0.041839f,  0.026030f,  0.89409f },  // Rule 147: PM & PS & ME & LO
        {  3,  1,  2,  1,  0.062236f,  0.104449f,  0.89288f },  // Rule 148: ZE & NM & ME & LO
        {  2,  1,  3,  4,  0.007191f,  0.001626f,  0.89276f },  // Rule 149: NS & NM & HI & VH
        {  3,  1,  1,  2,  0.078346f,  0.138396f,  0.89138f },  // Rule 150: ZE & NM & LO & ME
        {  1,  4,  2,  2,  0.107565f, -0.116406f,  0.88885f },  // Rule 151: NM & PS & ME & ME
        {  4,  1,  2,  1,  0.034173f,  0.090584f,  0.88834f },  // Rule 152: PS & NM & ME & LO
        {  5,  4,  2,  2, -0.034391f,  0.056795f,  0.88790f },  // Rule 153: PM & PS & ME & ME
        {  1,  3,  2,  2,  0.134815f, -0.082512f,  0.88594f },  // Rule 154: NM & ZE & ME & ME
        {  2,  4,  1,  1,  0.044600f,  0.025385f,  0.88341f },  // Rule 155: NS & PS & LO & LO
        {  2,  1,  1,  2,  0.095519f,  0.122845f,  0.88309f },  // Rule 156: NS & NM & LO & ME
        {  5,  3,  2,  1, -0.032859f,  0.031769f,  0.88248f },  // Rule 157: PM & ZE & ME & LO
        {  2,  1,  2,  2,  0.082290f,  0.108291f,  0.87856f },  // Rule 158: NS & NM & ME & ME
        {  1,  5,  2,  4,  0.013691f, -0.232185f,  0.87844f },  // Rule 159: NM & PM & ME & VH
        {  3,  3,  1,  1,  0.043919f,  0.034411f,  0.87703f },  // Rule 160: ZE & ZE & LO & LO
        {  1,  2,  2,  1,  0.113144f, -0.042499f,  0.87527f },  // Rule 161: NM & NS & ME & LO
        {  5,  1,  2,  3, -0.028618f,  0.130143f,  0.87459f },  // Rule 162: PM & NM & ME & HI
        {  4,  1,  1,  2,  0.059785f,  0.115349f,  0.87176f },  // Rule 163: PS & NM & LO & ME
        {  3,  1,  3,  2,  0.004974f,  0.014617f,  0.87167f },  // Rule 164: ZE & NM & HI & ME
        {  5,  3,  2,  2, -0.025652f,  0.076394f,  0.86718f },  // Rule 165: PM & ZE & ME & ME
        {  4,  1,  3,  3, -0.014736f,  0.014094f,  0.86499f },  // Rule 166: PS & NM & HI & HI
        {  1,  2,  3,  4,  0.003986f, -0.067476f,  0.86068f },  // Rule 167: NM & NS & HI & VH
        {  3,  1,  3,  3, -0.007135f,  0.000561f,  0.86015f },  // Rule 168: ZE & NM & HI & HI
        {  1,  2,  1,  4,  0.031414f, -0.043387f,  0.85955f },  // Rule 169: NM & NS & LO & VH
        {  2,  3,  2,  3, -0.008222f, -0.022069f,  0.85871f },  // Rule 170: NS & ZE & ME & HI
        {  1,  3,  2,  1,  0.132394f, -0.150312f,  0.85832f },  // Rule 171: NM & ZE & ME & LO
        {  2,  5,  3,  3, -0.095478f, -0.064459f,  0.85757f },  // Rule 172: NS & PM & HI & HI
        {  1,  4,  3,  4, -0.026345f, -0.070272f,  0.85644f },  // Rule 173: NM & PS & HI & VH
        {  1,  3,  1,  2,  0.101720f, -0.125896f,  0.85497f },  // Rule 174: NM & ZE & LO & ME
        {  4,  1,  1,  4,  0.001583f,  0.069301f,  0.85429f },  // Rule 175: PS & NM & LO & VH
        {  2,  1,  1,  4,  0.061067f,  0.020919f,  0.85370f },  // Rule 176: NS & NM & LO & VH
        {  3,  5,  2,  2, -0.156804f, -0.147863f,  0.85295f },  // Rule 177: ZE & PM & ME & ME
        {  4,  5,  1,  2, -0.187390f, -0.200191f,  0.85190f },  // Rule 178: PS & PM & LO & ME
        {  1,  2,  1,  2,  0.115888f, -0.103263f,  0.85138f },  // Rule 179: NM & NS & LO & ME
        {  2,  5,  3,  2, -0.060868f, -0.073980f,  0.85016f },  // Rule 180: NS & PM & HI & ME
        {  5,  2,  2,  2, -0.008399f,  0.090912f,  0.84695f },  // Rule 181: PM & NS & ME & ME
        {  5,  5,  3,  4, -0.244005f,  0.099251f,  0.84612f },  // Rule 182: PM & PM & HI & VH
        {  3,  5,  1,  4, -0.090575f, -0.030472f,  0.84564f },  // Rule 183: ZE & PM & LO & VH
        {  5,  2,  3,  4, -0.149112f,  0.090773f,  0.84494f },  // Rule 184: PM & NS & HI & VH
        {  4,  1,  2,  3,  0.018234f,  0.020727f,  0.84481f },  // Rule 185: PS & NM & ME & HI
        {  3,  2,  3,  3, -0.028991f, -0.014930f,  0.84323f },  // Rule 186: ZE & NS & HI & HI
        {  1,  4,  1,  2,  0.116794f, -0.142481f,  0.84136f },  // Rule 187: NM & PS & LO & ME
        {  3,  1,  2,  2,  0.061986f,  0.101024f,  0.84014f },  // Rule 188: ZE & NM & ME & ME
        {  4,  4,  2,  3, -0.014643f, -0.005612f,  0.83733f },  // Rule 189: PS & PS & ME & HI
        {  2,  2,  2,  3,  0.014257f, -0.013478f,  0.83634f },  // Rule 190: NS & NS & ME & HI
        {  4,  2,  3,  3, -0.040447f, -0.006247f,  0.83555f },  // Rule 191: PS & NS & HI & HI
        {  2,  5,  2,  1, -0.226222f, -0.184810f,  0.83519f },  // Rule 192: NS & PM & ME & LO
        {  2,  3,  3,  3, -0.029597f, -0.028504f,  0.83352f },  // Rule 193: NS & ZE & HI & HI
        {  3,  4,  2,  3, -0.011286f, -0.013941f,  0.83203f },  // Rule 194: ZE & PS & ME & HI
        {  3,  3,  2,  3, -0.005452f, -0.004609f,  0.82897f },  // Rule 195: ZE & ZE & ME & HI
        {  1,  3,  2,  3,  0.172550f,  0.036208f,  0.82758f },  // Rule 196: NM & ZE & ME & HI
        {  3,  3,  3,  3, -0.039154f, -0.020898f,  0.82616f },  // Rule 197: ZE & ZE & HI & HI
        {  2,  3,  3,  2, -0.004734f, -0.043841f,  0.82107f },  // Rule 198: NS & ZE & HI & ME
        {  2,  4,  3,  3, -0.043923f, -0.041701f,  0.82056f },  // Rule 199: NS & PS & HI & HI
        {  5,  2,  1,  2, -0.030951f,  0.123027f,  0.81730f },  // Rule 200: PM & NS & LO & ME
        {  4,  2,  2,  3, -0.001225f,  0.002727f,  0.81663f },  // Rule 201: PS & NS & ME & HI
        {  3,  2,  2,  3,  0.011361f,  0.004690f,  0.81512f },  // Rule 202: ZE & NS & ME & HI
        {  1,  4,  3,  2,  0.111756f, -0.098536f,  0.81193f },  // Rule 203: NM & PS & HI & ME
        {  2,  4,  2,  2,  0.027586f,  0.033684f,  0.81095f },  // Rule 204: NS & PS & ME & ME
        {  4,  1,  3,  2,  0.001889f,  0.004449f,  0.81049f },  // Rule 205: PS & NM & HI & ME
        {  3,  3,  3,  2, -0.010412f, -0.016653f,  0.80775f },  // Rule 206: ZE & ZE & HI & ME
        {  4,  3,  2,  2, -0.000552f,  0.053998f,  0.80575f },  // Rule 207: PS & ZE & ME & ME
        {  3,  5,  1,  2, -0.187743f, -0.188501f,  0.80413f },  // Rule 208: ZE & PM & LO & ME
        {  4,  5,  2,  2, -0.153624f, -0.126130f,  0.80220f },  // Rule 209: PS & PM & ME & ME
        {  4,  2,  2,  1, -0.005400f,  0.019142f,  0.79887f },  // Rule 210: PS & NS & ME & LO
        {  5,  1,  4,  1, -0.087851f,  0.294772f,  0.79790f },  // Rule 211: PM & NM & VH & LO
        {  4,  5,  2,  3, -0.120696f, -0.065312f,  0.79586f },  // Rule 212: PS & PM & ME & HI
        {  1,  1,  4,  1,  0.173491f, -0.111157f,  0.79547f },  // Rule 213: NM & NM & VH & LO
        {  5,  4,  1,  4, -0.033938f,  0.078963f,  0.79466f },  // Rule 214: PM & PS & LO & VH
        {  3,  4,  2,  2,  0.003050f,  0.029083f,  0.79249f },  // Rule 215: ZE & PS & ME & ME
        {  3,  3,  2,  2,  0.002024f,  0.034359f,  0.79052f },  // Rule 216: ZE & ZE & ME & ME
        {  4,  5,  2,  1, -0.211117f, -0.156363f,  0.78996f },  // Rule 217: PS & PM & ME & LO
        {  4,  4,  2,  2, -0.004212f,  0.019331f,  0.78625f },  // Rule 218: PS & PS & ME & ME
        {  4,  3,  2,  3, -0.005329f,  0.002062f,  0.78621f },  // Rule 219: PS & ZE & ME & HI
        {  5,  3,  4,  1, -0.094648f,  0.125857f,  0.78517f },  // Rule 220: PM & ZE & VH & LO
        {  5,  3,  1,  2, -0.035210f,  0.080933f,  0.78044f },  // Rule 221: PM & ZE & LO & ME
        {  1,  2,  2,  3,  0.161808f,  0.051431f,  0.77907f },  // Rule 222: NM & NS & ME & HI
        {  2,  3,  2,  1,  0.018323f, -0.005721f,  0.77786f },  // Rule 223: NS & ZE & ME & LO
        {  1,  1,  4,  3,  0.040718f, -0.028361f,  0.77732f },  // Rule 224: NM & NM & VH & HI
        {  1,  5,  4,  1, -0.018564f, -0.487185f,  0.77473f },  // Rule 225: NM & PM & VH & LO
        {  4,  5,  3,  2, -0.121106f, -0.026272f,  0.77272f },  // Rule 226: PS & PM & HI & ME
        {  5,  2,  4,  1, -0.086881f,  0.110045f,  0.76686f },  // Rule 227: PM & NS & VH & LO
        {  2,  2,  1,  4, -0.005935f,  0.014202f,  0.76512f },  // Rule 228: NS & NS & LO & VH
        {  3,  2,  2,  1, -0.004736f,  0.010713f,  0.76159f },  // Rule 229: ZE & NS & ME & LO
        {  1,  2,  4,  1,  0.162719f, -0.160841f,  0.75849f },  // Rule 230: NM & NS & VH & LO
        {  2,  2,  2,  1,  0.033001f,  0.015639f,  0.75227f },  // Rule 231: NS & NS & ME & LO
        {  3,  5,  2,  3, -0.105629f, -0.074594f,  0.75125f },  // Rule 232: ZE & PM & ME & HI
        {  4,  4,  3,  3, -0.064200f, -0.024453f,  0.75118f },  // Rule 233: PS & PS & HI & HI
        {  5,  1,  4,  3, -0.072378f,  0.072927f,  0.75032f },  // Rule 234: PM & NM & VH & HI
        {  5,  2,  4,  3, -0.090943f,  0.073753f,  0.74821f },  // Rule 235: PM & NS & VH & HI
        {  2,  4,  2,  3,  0.000811f, -0.011454f,  0.74380f },  // Rule 236: NS & PS & ME & HI
        {  4,  2,  2,  2,  0.015236f,  0.051559f,  0.73742f },  // Rule 237: PS & NS & ME & ME
        {  5,  5,  4,  3, -0.138773f,  0.029524f,  0.73709f },  // Rule 238: PM & PM & VH & HI
        {  1,  2,  3,  2,  0.127977f, -0.122038f,  0.73553f },  // Rule 239: NM & NS & HI & ME
        {  5,  1,  4,  2, -0.045055f,  0.061466f,  0.73460f },  // Rule 240: PM & NM & VH & ME
        {  1,  3,  4,  1,  0.146400f, -0.177320f,  0.73169f },  // Rule 241: NM & ZE & VH & LO
        {  3,  5,  3,  2, -0.104558f, -0.087753f,  0.72969f },  // Rule 242: ZE & PM & HI & ME
        {  4,  4,  1,  4, -0.055554f,  0.065417f,  0.72051f },  // Rule 243: PS & PS & LO & VH
        {  2,  3,  2,  2,  0.029862f,  0.038365f,  0.71842f },  // Rule 244: NS & ZE & ME & ME
        {  5,  4,  4,  1, -0.107660f,  0.099989f,  0.71242f },  // Rule 245: PM & PS & VH & LO
        {  1,  1,  4,  2,  0.218918f, -0.157200f,  0.70231f },  // Rule 246: NM & NM & VH & ME
        {  3,  4,  3,  3, -0.055191f, -0.037040f,  0.70184f },  // Rule 247: ZE & PS & HI & HI
        {  1,  2,  4,  3,  0.022708f, -0.066349f,  0.69692f },  // Rule 248: NM & NS & VH & HI
        {  5,  3,  3,  4, -0.256893f,  0.254655f,  0.69673f },  // Rule 249: PM & ZE & HI & VH
        {  3,  3,  2,  1, -0.000234f,  0.017874f,  0.69240f },  // Rule 250: ZE & ZE & ME & LO
        {  1,  2,  4,  2,  0.236911f, -0.213708f,  0.68994f },  // Rule 251: NM & NS & VH & ME
        {  2,  2,  2,  2,  0.046023f,  0.044057f,  0.68644f },  // Rule 252: NS & NS & ME & ME
        {  3,  4,  1,  2,  0.015552f,  0.006957f,  0.68617f },  // Rule 253: ZE & PS & LO & ME
        {  5,  1,  2,  4,  0.006569f,  0.534862f,  0.68602f },  // Rule 254: PM & NM & ME & VH
        {  3,  2,  3,  2, -0.004866f, -0.002393f,  0.67933f },  // Rule 255: ZE & NS & HI & ME
        {  1,  3,  4,  3,  0.028938f, -0.083883f,  0.67640f },  // Rule 256: NM & ZE & VH & HI
        {  1,  4,  4,  1,  0.162357f, -0.193802f,  0.67630f },  // Rule 257: NM & PS & VH & LO
        {  5,  3,  3,  2, -0.082593f,  0.081026f,  0.67512f },  // Rule 258: PM & ZE & HI & ME
        {  5,  5,  4,  1, -0.397346f,  0.333492f,  0.67482f },  // Rule 259: PM & PM & VH & LO
        {  1,  5,  4,  2,  0.219348f, -0.254618f,  0.67146f },  // Rule 260: NM & PM & VH & ME
        {  4,  3,  1,  4, -0.068125f,  0.018822f,  0.66886f },  // Rule 261: PS & ZE & LO & VH
        {  5,  4,  4,  3, -0.134044f,  0.064385f,  0.66475f },  // Rule 262: PM & PS & VH & HI
        {  3,  3,  1,  4, -0.087394f,  0.054782f,  0.65960f },  // Rule 263: ZE & ZE & LO & VH
        {  2,  4,  1,  2,  0.013346f, -0.042669f,  0.65827f },  // Rule 264: NS & PS & LO & ME
        {  4,  1,  3,  4, -0.013690f,  0.117049f,  0.65380f },  // Rule 265: PS & NM & HI & VH
        {  3,  1,  3,  4,  0.024787f,  0.092362f,  0.65332f },  // Rule 266: ZE & NM & HI & VH
        {  3,  4,  3,  2, -0.016339f, -0.008850f,  0.65076f },  // Rule 267: ZE & PS & HI & ME
        {  4,  4,  1,  2, -0.001353f, -0.000575f,  0.64773f },  // Rule 268: PS & PS & LO & ME
        {  1,  3,  4,  2,  0.206851f, -0.208958f,  0.62879f },  // Rule 269: NM & ZE & VH & ME
        {  2,  4,  3,  2, -0.005918f, -0.028169f,  0.61930f },  // Rule 270: NS & PS & HI & ME
        {  4,  4,  3,  2, -0.027167f, -0.020284f,  0.61879f },  // Rule 271: PS & PS & HI & ME
        {  5,  3,  4,  3, -0.111146f,  0.051657f,  0.61359f },  // Rule 272: PM & ZE & VH & HI
        {  1,  4,  4,  2,  0.230103f, -0.198714f,  0.61229f },  // Rule 273: NM & PS & VH & ME
        {  3,  4,  1,  4, -0.078857f,  0.003628f,  0.60445f },  // Rule 274: ZE & PS & LO & VH
        {  5,  0,  1,  4,  0.264320f,  0.994273f,  0.60183f },  // Rule 275: PM & NB & LO & VH
        {  4,  3,  2,  1, -0.010876f,  0.010266f,  0.60002f },  // Rule 276: PS & ZE & ME & LO
        {  4,  3,  3,  2, -0.010159f, -0.014819f,  0.59816f },  // Rule 277: PS & ZE & HI & ME
        {  4,  2,  1,  2,  0.002053f,  0.020042f,  0.58704f },  // Rule 278: PS & NS & LO & ME
        {  2,  3,  1,  2,  0.006862f,  0.000803f,  0.58494f },  // Rule 279: NS & ZE & LO & ME
        {  3,  2,  1,  2,  0.012146f,  0.013405f,  0.58324f },  // Rule 280: ZE & NS & LO & ME
        {  0,  5,  1,  1,  0.708571f, -0.974648f,  0.58295f },  // Rule 281: NB & PM & LO & LO
        {  4,  2,  1,  4, -0.039332f,  0.004720f,  0.57799f },  // Rule 282: PS & NS & LO & VH
        {  2,  3,  1,  4, -0.041647f, -0.003506f,  0.57692f },  // Rule 283: NS & ZE & LO & VH
        {  5,  1,  4,  4, -0.073483f,  0.296430f,  0.56897f },  // Rule 284: PM & NM & VH & VH
        {  2,  1,  4,  1, -0.009033f,  0.214400f,  0.56874f },  // Rule 285: NS & NM & VH & LO
        {  0,  1,  1,  1,  1.000000f, -1.000000f,  0.56604f },  // Rule 286: NB & NM & LO & LO
        {  3,  3,  1,  2,  0.002028f,  0.000614f,  0.56464f },  // Rule 287: ZE & ZE & LO & ME
        {  1,  3,  2,  4, -0.023271f, -0.004287f,  0.56366f },  // Rule 288: NM & ZE & ME & VH
        {  4,  2,  3,  2, -0.024523f,  0.004716f,  0.55983f },  // Rule 289: PS & NS & HI & ME
        {  2,  4,  2,  1, -0.020892f,  0.003140f,  0.55722f },  // Rule 290: NS & PS & ME & LO
        {  4,  4,  2,  1, -0.023473f, -0.002333f,  0.55463f },  // Rule 291: PS & PS & ME & LO
        {  5,  4,  4,  2, -0.104562f,  0.091017f,  0.54672f },  // Rule 292: PM & PS & VH & ME
        {  1,  4,  2,  4, -0.067103f, -0.000292f,  0.53357f },  // Rule 293: NM & PS & ME & VH
        {  3,  2,  1,  4, -0.050386f, -0.033445f,  0.53085f },  // Rule 294: ZE & NS & LO & VH
        {  4,  3,  1,  2, -0.001986f,  0.018756f,  0.52870f },  // Rule 295: PS & ZE & LO & ME
        {  2,  1,  4,  3, -0.015173f, -0.026599f,  0.52779f },  // Rule 296: NS & NM & VH & HI
        {  0,  5,  1,  3,  0.903905f, -0.996846f,  0.52172f },  // Rule 297: NB & PM & LO & HI
        {  6,  1,  1,  1, -0.757801f,  1.000000f,  0.50961f },  // Rule 298: PB & NM & LO & LO
        {  4,  4,  4,  1, -0.134061f,  0.014591f,  0.50233f },  // Rule 299: PS & PS & VH & LO
        {  3,  4,  2,  1, -0.015318f,  0.001671f,  0.50103f },  // Rule 300: ZE & PS & ME & LO
        {  0,  5,  1,  2,  0.909552f, -1.000000f,  0.50073f },  // Rule 301: NB & PM & LO & ME
        {  2,  2,  1,  2,  0.012287f, -0.016031f,  0.49952f },  // Rule 302: NS & NS & LO & ME
        {  0,  1,  3,  1,  0.939589f, -1.000000f,  0.48136f },  // Rule 303: NB & NM & HI & LO
        {  6,  1,  3,  1, -0.875892f,  1.000000f,  0.45713f },  // Rule 304: PB & NM & HI & LO
        {  2,  5,  4,  1, -0.300927f,  0.105707f,  0.45563f },  // Rule 305: NS & PM & VH & LO
        {  0,  5,  3,  1,  0.462420f, -1.000000f,  0.44341f },  // Rule 306: NB & PM & HI & LO
        {  0,  5,  1,  4,  0.714057f, -1.000000f,  0.44338f },  // Rule 307: NB & PM & LO & VH
        {  6,  5,  1,  1, -1.000000f,  1.000000f,  0.44252f },  // Rule 308: PB & PM & LO & LO
        {  1,  4,  4,  3, -0.016561f, -0.103192f,  0.42635f },  // Rule 309: NM & PS & VH & HI
        {  0,  1,  1,  3,  1.000000f, -1.000000f,  0.40493f },  // Rule 310: NB & NM & LO & HI
        {  0,  4,  1,  3,  1.000000f, -1.000000f,  0.37605f },  // Rule 311: NB & PS & LO & HI
        {  6,  1,  1,  3, -0.487164f,  1.000000f,  0.37023f },  // Rule 312: PB & NM & LO & HI
        {  3,  1,  4,  1, -0.096387f, -0.021583f,  0.36793f },  // Rule 313: ZE & NM & VH & LO
        {  5,  0,  1,  3, -0.064638f,  1.000000f,  0.35437f },  // Rule 314: PM & NB & LO & HI
        {  0,  3,  1,  3,  1.000000f, -1.000000f,  0.33507f },  // Rule 315: NB & ZE & LO & HI
        {  5,  0,  1,  1, -0.263025f,  0.974993f,  0.31723f },  // Rule 316: PM & NB & LO & LO
        {  2,  2,  4,  3, -0.020430f, -0.003253f,  0.30728f },  // Rule 317: NS & NS & VH & HI
        {  1,  6,  1,  4,  0.084020f, -1.000000f,  0.27229f },  // Rule 318: NM & PB & LO & VH
        {  6,  0,  1,  4,  0.162314f,  1.000000f,  0.26261f },  // Rule 319: PB & NB & LO & VH
        {  0,  6,  1,  4,  0.301949f, -1.000000f,  0.23997f },  // Rule 320: NB & PB & LO & VH
        {  2,  5,  2,  4, -0.157267f, -0.155744f,  0.23542f },  // Rule 321: NS & PM & ME & VH
        {  6,  0,  1,  1, -0.215516f,  1.000000f,  0.23156f },  // Rule 322: PB & NB & LO & LO
        {  1,  6,  1,  1,  0.245246f, -0.967314f,  0.22883f },  // Rule 323: NM & PB & LO & LO
        {  0,  6,  1,  1,  0.268806f, -0.961092f,  0.21924f },  // Rule 324: NB & PB & LO & LO
        {  3,  2,  4,  3, -0.071547f, -0.012363f,  0.21138f },  // Rule 325: ZE & NS & VH & HI
        {  1,  6,  1,  3,  0.173160f, -1.000000f,  0.21039f },  // Rule 326: NM & PB & LO & HI
        {  0,  6,  1,  3,  0.207476f, -1.000000f,  0.19160f },  // Rule 327: NB & PB & LO & HI
        {  0,  6,  0,  1,  0.262006f, -0.633463f,  0.11363f },  // Rule 328: NB & PB & VL & LO
        {  5,  1,  0,  0, -0.084925f,  0.177778f,  0.08576f },  // Rule 329: PM & NM & VL & VL
        {  5,  1,  0,  1, -0.143002f,  0.318759f,  0.08062f },  // Rule 330: PM & NM & VL & LO
        {  0,  6,  0,  0,  0.160634f, -0.294570f,  0.07712f },  // Rule 331: NB & PB & VL & VL
        {  1,  5,  0,  0,  0.100000f, -0.177778f,  0.07374f },  // Rule 332: NM & PM & VL & VL
    };

    private const int N_RULES = 332;

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
        return $"FuzzyController: {N_RULES} rules, generated 2026-03-10 20:30:01";
    }
}