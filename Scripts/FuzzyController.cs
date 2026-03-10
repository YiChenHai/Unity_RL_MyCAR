// 模糊控制器 - 自动生成
// 生成时间: 2026-03-10 13:29:31
// 数据样本数: 300000
// 规则数: 289
// 输入变量: front_lr_diff, rear_lr_diff, front_center, rear_center, accumulated_vx, accumulated_omega
// 输出变量: output_vx_norm, output_omega_norm
//
// 隶属函数配置:
//   front_lr_diff: 7 集 (NB, NM, NS, ZE, PS, PM, PB)
//   rear_lr_diff: 7 集 (NB, NM, NS, ZE, PS, PM, PB)
//   front_center: 5 集 (VL, LO, ME, HI, VH)
//   rear_center: 5 集 (VL, LO, ME, HI, VH)
//   accumulated_vx: 5 集 (NB, NS, ZE, PS, PB)
//   accumulated_omega: 5 集 (NB, NS, ZE, PS, PB)

using System;
using UnityEngine;

public class FuzzyController
{
    // ===== 三角形隶属函数参数 =====
    // 每行: { left, center, right }

    // front_lr_diff: NB, NM, NS, ZE, PS, PM, PB
    private static readonly float[,] MF_FrontLR = {
        { -1.00000f, -1.00000f, -0.45582f },  // NB
        { -1.00000f, -0.45582f, -0.22002f },  // NM
        { -0.45582f, -0.22002f,  0.01497f },  // NS
        { -0.22002f,  0.01497f,  0.24980f },  // ZE
        {  0.01497f,  0.24980f,  0.48479f },  // PS
        {  0.24980f,  0.48479f,  1.00000f },  // PM
        {  0.48479f,  1.00000f,  1.00000f },  // PB
    };

    // rear_lr_diff: NB, NM, NS, ZE, PS, PM, PB
    private static readonly float[,] MF_RearLR = {
        { -1.00000f, -1.00000f, -0.48119f },  // NB
        { -1.00000f, -0.48119f, -0.24564f },  // NM
        { -0.48119f, -0.24564f, -0.01031f },  // NS
        { -0.24564f, -0.01031f,  0.22526f },  // ZE
        { -0.01031f,  0.22526f,  0.46200f },  // PS
        {  0.22526f,  0.46200f,  1.00000f },  // PM
        {  0.46200f,  1.00000f,  1.00000f },  // PB
    };

    // front_center: VL, LO, ME, HI, VH
    private static readonly float[,] MF_FrontCenter = {
        {  0.00000f,  0.00000f,  0.42954f },  // VL
        {  0.00000f,  0.42954f,  0.60567f },  // LO
        {  0.42954f,  0.60567f,  0.78128f },  // ME
        {  0.60567f,  0.78128f,  1.00000f },  // HI
        {  0.78128f,  1.00000f,  1.00000f },  // VH
    };

    // rear_center: VL, LO, ME, HI, VH
    private static readonly float[,] MF_RearCenter = {
        {  0.00000f,  0.00000f,  0.42972f },  // VL
        {  0.00000f,  0.42972f,  0.60541f },  // LO
        {  0.42972f,  0.60541f,  0.78137f },  // ME
        {  0.60541f,  0.78137f,  1.00000f },  // HI
        {  0.78137f,  1.00000f,  1.00000f },  // VH
    };

    // accumulated_vx: NB, NS, ZE, PS, PB
    private static readonly float[,] MF_AccVx = {
        { -1.00000f, -1.00000f, -0.36209f },  // NB
        { -1.00000f, -0.36209f, -0.00166f },  // NS
        { -0.36209f, -0.00166f,  0.35873f },  // ZE
        { -0.00166f,  0.35873f,  1.00000f },  // PS
        {  0.35873f,  1.00000f,  1.00000f },  // PB
    };

    // accumulated_omega: NB, NS, ZE, PS, PB
    private static readonly float[,] MF_AccOmega = {
        { -1.00000f, -1.00000f, -0.36382f },  // NB
        { -1.00000f, -0.36382f, -0.00121f },  // NS
        { -0.36382f, -0.00121f,  0.36066f },  // ZE
        { -0.00121f,  0.36066f,  1.00000f },  // PS
        {  0.36066f,  1.00000f,  1.00000f },  // PB
    };

    // ===== 规则库 (289 条规则) =====
    // 每行: { MF_FrontLR集, MF_RearLR集, MF_FrontCenter集, MF_RearCenter集, MF_AccVx集, MF_AccOmega集, 输出vx, 输出omega, 权重 }
    private static readonly float[,] Rules = {
        {  1,  5,  1,  1,  2,  2,  0.117435f, -0.222017f,  0.90814f },  // Rule 1: NM & PM & LO & LO & ZE & ZE
        {  3,  3,  2,  2,  2,  2,  0.013435f, -0.029487f,  0.87771f },  // Rule 2: ZE & ZE & ME & ME & ZE & ZE
        {  5,  1,  1,  1,  2,  2, -0.104897f,  0.227258f,  0.87191f },  // Rule 3: PM & NM & LO & LO & ZE & ZE
        {  3,  3,  3,  3,  2,  2,  0.001905f,  0.000950f,  0.80834f },  // Rule 4: ZE & ZE & HI & HI & ZE & ZE
        {  3,  3,  3,  4,  2,  2, -0.037118f, -0.016612f,  0.73292f },  // Rule 5: ZE & ZE & HI & VH & ZE & ZE
        {  4,  2,  3,  3,  2,  4, -0.027654f,  0.900635f,  0.70517f },  // Rule 6: PS & NS & HI & HI & ZE & PB
        {  2,  4,  3,  3,  1,  1, -0.315634f, -0.561463f,  0.69958f },  // Rule 7: NS & PS & HI & HI & NS & NS
        {  2,  4,  3,  3,  2,  0, -0.015634f, -0.903036f,  0.63007f },  // Rule 8: NS & PS & HI & HI & ZE & NB
        {  2,  4,  2,  2,  2,  2,  0.081727f, -0.214235f,  0.61054f },  // Rule 9: NS & PS & ME & ME & ZE & ZE
        {  3,  3,  3,  3,  2,  3,  0.017389f,  0.320608f,  0.57592f },  // Rule 10: ZE & ZE & HI & HI & ZE & PS
        {  3,  2,  3,  3,  3,  4,  0.422388f,  0.715277f,  0.57574f },  // Rule 11: ZE & NS & HI & HI & PS & PB
        {  3,  3,  3,  4,  2,  1, -0.070530f, -0.222091f,  0.57258f },  // Rule 12: ZE & ZE & HI & VH & ZE & NS
        {  3,  3,  3,  3,  2,  1, -0.010896f, -0.342605f,  0.56152f },  // Rule 13: ZE & ZE & HI & HI & ZE & NS
        {  2,  4,  3,  3,  2,  2,  0.046723f, -0.200960f,  0.55548f },  // Rule 14: NS & PS & HI & HI & ZE & ZE
        {  4,  2,  3,  3,  2,  1,  0.032572f, -0.154949f,  0.55208f },  // Rule 15: PS & NS & HI & HI & ZE & NS
        {  3,  3,  3,  4,  3,  3,  0.274392f,  0.192660f,  0.54041f },  // Rule 16: ZE & ZE & HI & VH & PS & PS
        {  2,  4,  2,  2,  2,  1,  0.153124f, -0.536963f,  0.53672f },  // Rule 17: NS & PS & ME & ME & ZE & NS
        {  3,  3,  3,  4,  2,  3,  0.075418f,  0.170034f,  0.51962f },  // Rule 18: ZE & ZE & HI & VH & ZE & PS
        {  4,  2,  3,  4,  2,  4,  0.007177f,  0.822782f,  0.50791f },  // Rule 19: PS & NS & HI & VH & ZE & PB
        {  4,  2,  2,  2,  2,  2, -0.074985f,  0.181824f,  0.50627f },  // Rule 20: PS & NS & ME & ME & ZE & ZE
        {  4,  2,  3,  3,  3,  3,  0.274147f,  0.535315f,  0.49510f },  // Rule 21: PS & NS & HI & HI & PS & PS
        {  3,  3,  2,  3,  2,  1,  0.085809f, -0.330770f,  0.49415f },  // Rule 22: ZE & ZE & ME & HI & ZE & NS
        {  3,  2,  3,  3,  3,  3,  0.377251f,  0.483374f,  0.48815f },  // Rule 23: ZE & NS & HI & HI & PS & PS
        {  4,  3,  3,  3,  1,  4, -0.385199f,  0.859456f,  0.48346f },  // Rule 24: PS & ZE & HI & HI & NS & PB
        {  5,  1,  1,  1,  2,  3, -0.212223f,  0.541605f,  0.47929f },  // Rule 25: PM & NM & LO & LO & ZE & PS
        {  2,  4,  3,  3,  2,  1,  0.016319f, -0.562318f,  0.47900f },  // Rule 26: NS & PS & HI & HI & ZE & NS
        {  3,  3,  4,  3,  2,  2, -0.029688f, -0.007799f,  0.47759f },  // Rule 27: ZE & ZE & VH & HI & ZE & ZE
        {  3,  4,  3,  3,  1,  1, -0.344713f, -0.390108f,  0.47739f },  // Rule 28: ZE & PS & HI & HI & NS & NS
        {  2,  4,  3,  3,  1,  3, -0.333543f,  0.177357f,  0.47659f },  // Rule 29: NS & PS & HI & HI & NS & PS
        {  3,  3,  3,  3,  3,  3,  0.312465f,  0.321858f,  0.47558f },  // Rule 30: ZE & ZE & HI & HI & PS & PS
        {  2,  4,  3,  3,  1,  2, -0.324054f, -0.161858f,  0.47403f },  // Rule 31: NS & PS & HI & HI & NS & ZE
        {  4,  2,  3,  3,  3,  2,  0.339424f,  0.178972f,  0.47320f },  // Rule 32: PS & NS & HI & HI & PS & ZE
        {  4,  2,  3,  3,  2,  2,  0.047817f,  0.185593f,  0.47197f },  // Rule 33: PS & NS & HI & HI & ZE & ZE
        {  3,  3,  3,  3,  1,  1, -0.277653f, -0.314144f,  0.47099f },  // Rule 34: ZE & ZE & HI & HI & NS & NS
        {  4,  2,  2,  2,  2,  3, -0.142380f,  0.456311f,  0.47096f },  // Rule 35: PS & NS & ME & ME & ZE & PS
        {  3,  3,  3,  3,  3,  1,  0.292086f, -0.342215f,  0.46680f },  // Rule 36: ZE & ZE & HI & HI & PS & NS
        {  2,  3,  3,  3,  3,  0,  0.420597f, -0.855555f,  0.46658f },  // Rule 37: NS & ZE & HI & HI & PS & NB
        {  4,  2,  3,  3,  1,  3, -0.271605f,  0.602823f,  0.46105f },  // Rule 38: PS & NS & HI & HI & NS & PS
        {  1,  4,  3,  3,  1,  2, -0.293043f, -0.194606f,  0.45936f },  // Rule 39: NM & PS & HI & HI & NS & ZE
        {  4,  2,  3,  3,  1,  4, -0.305430f,  0.962230f,  0.45796f },  // Rule 40: PS & NS & HI & HI & NS & PB
        {  1,  4,  3,  3,  3,  1,  0.401263f, -0.625210f,  0.45545f },  // Rule 41: NM & PS & HI & HI & PS & NS
        {  2,  3,  4,  3,  3,  0,  0.308514f, -0.815137f,  0.45482f },  // Rule 42: NS & ZE & VH & HI & PS & NB
        {  2,  5,  3,  4,  2,  1, -0.151399f, -0.653819f,  0.44816f },  // Rule 43: NS & PM & HI & VH & ZE & NS
        {  3,  2,  3,  4,  3,  1,  0.339699f, -0.261539f,  0.44639f },  // Rule 44: ZE & NS & HI & VH & PS & NS
        {  4,  2,  3,  3,  2,  3,  0.007001f,  0.570665f,  0.44543f },  // Rule 45: PS & NS & HI & HI & ZE & PS
        {  1,  5,  1,  2,  3,  0,  0.282705f, -0.953079f,  0.44249f },  // Rule 46: NM & PM & LO & ME & PS & NB
        {  3,  3,  3,  4,  1,  4, -0.347406f,  0.665174f,  0.43734f },  // Rule 47: ZE & ZE & HI & VH & NS & PB
        {  4,  3,  3,  3,  0,  4, -0.948014f,  0.956890f,  0.43635f },  // Rule 48: PS & ZE & HI & HI & NB & PB
        {  3,  2,  3,  3,  3,  1,  0.419623f, -0.262849f,  0.43189f },  // Rule 49: ZE & NS & HI & HI & PS & NS
        {  1,  4,  3,  3,  2,  0,  0.097262f, -0.976428f,  0.42505f },  // Rule 50: NM & PS & HI & HI & ZE & NB
        {  1,  4,  3,  3,  1,  1, -0.192926f, -0.576183f,  0.42477f },  // Rule 51: NM & PS & HI & HI & NS & NS
        {  3,  3,  4,  3,  2,  1,  0.029984f, -0.275172f,  0.42411f },  // Rule 52: ZE & ZE & VH & HI & ZE & NS
        {  3,  3,  3,  3,  3,  2,  0.288658f, -0.059740f,  0.42353f },  // Rule 53: ZE & ZE & HI & HI & PS & ZE
        {  3,  3,  3,  4,  1,  2, -0.224675f, -0.099882f,  0.42207f },  // Rule 54: ZE & ZE & HI & VH & NS & ZE
        {  4,  2,  3,  3,  3,  4,  0.251599f,  0.926708f,  0.41808f },  // Rule 55: PS & NS & HI & HI & PS & PB
        {  5,  1,  1,  2,  1,  4, -0.344617f,  0.972980f,  0.41466f },  // Rule 56: PM & NM & LO & ME & NS & PB
        {  3,  3,  3,  3,  1,  3, -0.314826f,  0.367093f,  0.40835f },  // Rule 57: ZE & ZE & HI & HI & NS & PS
        {  4,  3,  3,  3,  2,  4, -0.091269f,  0.813484f,  0.40722f },  // Rule 58: PS & ZE & HI & HI & ZE & PB
        {  1,  4,  3,  3,  3,  0,  0.333440f, -0.985082f,  0.40464f },  // Rule 59: NM & PS & HI & HI & PS & NB
        {  3,  3,  3,  4,  3,  2,  0.217999f,  0.114186f,  0.40416f },  // Rule 60: ZE & ZE & HI & VH & PS & ZE
        {  4,  3,  3,  3,  0,  3, -0.975966f,  0.625537f,  0.40415f },  // Rule 61: PS & ZE & HI & HI & NB & PS
        {  3,  2,  4,  3,  3,  1,  0.368467f, -0.183101f,  0.40253f },  // Rule 62: ZE & NS & VH & HI & PS & NS
        {  2,  3,  3,  3,  1,  3, -0.296697f,  0.267404f,  0.40140f },  // Rule 63: NS & ZE & HI & HI & NS & PS
        {  5,  2,  3,  3,  3,  4,  0.249659f,  0.963048f,  0.40112f },  // Rule 64: PM & NS & HI & HI & PS & PB
        {  4,  3,  4,  3,  2,  3, -0.086612f,  0.383477f,  0.40029f },  // Rule 65: PS & ZE & VH & HI & ZE & PS
        {  2,  3,  3,  3,  3,  2,  0.414893f, -0.157319f,  0.39742f },  // Rule 66: NS & ZE & HI & HI & PS & ZE
        {  3,  2,  3,  3,  4,  2,  0.843939f,  0.153050f,  0.39248f },  // Rule 67: ZE & NS & HI & HI & PB & ZE
        {  3,  2,  3,  3,  4,  3,  0.825139f,  0.370651f,  0.38986f },  // Rule 68: ZE & NS & HI & HI & PB & PS
        {  2,  4,  3,  4,  2,  0, -0.086228f, -0.802907f,  0.38950f },  // Rule 69: NS & PS & HI & VH & ZE & NB
        {  3,  3,  2,  3,  2,  2,  0.023655f, -0.037534f,  0.38911f },  // Rule 70: ZE & ZE & ME & HI & ZE & ZE
        {  5,  1,  3,  4,  3,  3,  0.126703f,  0.709255f,  0.38841f },  // Rule 71: PM & NM & HI & VH & PS & PS
        {  2,  3,  3,  4,  1,  3, -0.302900f,  0.275736f,  0.38669f },  // Rule 72: NS & ZE & HI & VH & NS & PS
        {  3,  2,  3,  3,  2,  4,  0.054502f,  0.773640f,  0.38126f },  // Rule 73: ZE & NS & HI & HI & ZE & PB
        {  2,  4,  3,  3,  1,  0, -0.322268f, -0.947359f,  0.38029f },  // Rule 74: NS & PS & HI & HI & NS & NB
        {  1,  5,  1,  1,  2,  1,  0.228845f, -0.533333f,  0.37828f },  // Rule 75: NM & PM & LO & LO & ZE & NS
        {  3,  2,  4,  3,  3,  4,  0.337211f,  0.794838f,  0.37798f },  // Rule 76: ZE & NS & VH & HI & PS & PB
        {  3,  4,  3,  3,  0,  4, -0.839073f,  0.673278f,  0.37792f },  // Rule 77: ZE & PS & HI & HI & NB & PB
        {  4,  1,  2,  3,  1,  4, -0.261188f,  0.992998f,  0.37746f },  // Rule 78: PS & NM & ME & HI & NS & PB
        {  3,  2,  3,  3,  4,  1,  0.772466f, -0.301069f,  0.37581f },  // Rule 79: ZE & NS & HI & HI & PB & NS
        {  5,  2,  3,  3,  2,  4, -0.068276f,  0.960064f,  0.37258f },  // Rule 80: PM & NS & HI & HI & ZE & PB
        {  3,  3,  3,  4,  1,  1, -0.242024f, -0.235786f,  0.36979f },  // Rule 81: ZE & ZE & HI & VH & NS & NS
        {  3,  3,  3,  3,  1,  2, -0.261984f,  0.024192f,  0.36922f },  // Rule 82: ZE & ZE & HI & HI & NS & ZE
        {  4,  2,  3,  3,  3,  1,  0.335831f, -0.152659f,  0.36848f },  // Rule 83: PS & NS & HI & HI & PS & NS
        {  5,  1,  1,  1,  1,  3, -0.286851f,  0.675942f,  0.36826f },  // Rule 84: PM & NM & LO & LO & NS & PS
        {  3,  3,  4,  3,  1,  1, -0.271922f, -0.246067f,  0.36619f },  // Rule 85: ZE & ZE & VH & HI & NS & NS
        {  1,  5,  1,  1,  3,  1,  0.278791f, -0.609258f,  0.36201f },  // Rule 86: NM & PM & LO & LO & PS & NS
        {  4,  2,  2,  3,  2,  3, -0.139794f,  0.487515f,  0.36162f },  // Rule 87: PS & NS & ME & HI & ZE & PS
        {  4,  2,  4,  3,  1,  3, -0.350364f,  0.585945f,  0.35849f },  // Rule 88: PS & NS & VH & HI & NS & PS
        {  1,  4,  3,  3,  2,  2,  0.103455f, -0.232354f,  0.35636f },  // Rule 89: NM & PS & HI & HI & ZE & ZE
        {  2,  3,  3,  3,  3,  1,  0.365866f, -0.508815f,  0.35623f },  // Rule 90: NS & ZE & HI & HI & PS & NS
        {  2,  4,  2,  3,  2,  0,  0.136549f, -0.932831f,  0.35621f },  // Rule 91: NS & PS & ME & HI & ZE & NB
        {  3,  3,  3,  3,  1,  4, -0.330954f,  0.699872f,  0.35582f },  // Rule 92: ZE & ZE & HI & HI & NS & PB
        {  5,  1,  3,  4,  2,  4,  0.058309f,  0.986017f,  0.35331f },  // Rule 93: PM & NM & HI & VH & ZE & PB
        {  2,  4,  2,  3,  3,  0,  0.246034f, -0.932532f,  0.35326f },  // Rule 94: NS & PS & ME & HI & PS & NB
        {  1,  4,  2,  3,  2,  1,  0.098356f, -0.577252f,  0.35073f },  // Rule 95: NM & PS & ME & HI & ZE & NS
        {  1,  4,  3,  3,  2,  1,  0.085459f, -0.653976f,  0.34606f },  // Rule 96: NM & PS & HI & HI & ZE & NS
        {  2,  4,  2,  4,  3,  0,  0.237770f, -0.909757f,  0.34444f },  // Rule 97: NS & PS & ME & VH & PS & NB
        {  1,  4,  2,  3,  2,  0,  0.152638f, -0.969194f,  0.34371f },  // Rule 98: NM & PS & ME & HI & ZE & NB
        {  2,  4,  3,  3,  0,  3, -0.667665f,  0.260465f,  0.34328f },  // Rule 99: NS & PS & HI & HI & NB & PS
        {  3,  4,  4,  3,  0,  0, -0.824007f, -0.725399f,  0.34281f },  // Rule 100: ZE & PS & VH & HI & NB & NB
        {  3,  2,  3,  3,  3,  2,  0.388543f,  0.060352f,  0.34096f },  // Rule 101: ZE & NS & HI & HI & PS & ZE
        {  3,  3,  4,  3,  3,  2,  0.261468f, -0.119363f,  0.34094f },  // Rule 102: ZE & ZE & VH & HI & PS & ZE
        {  2,  3,  3,  4,  3,  0,  0.430985f, -0.875970f,  0.34089f },  // Rule 103: NS & ZE & HI & VH & PS & NB
        {  4,  2,  2,  2,  1,  3, -0.247996f,  0.583480f,  0.33766f },  // Rule 104: PS & NS & ME & ME & NS & PS
        {  4,  2,  2,  3,  1,  4, -0.279506f,  0.950870f,  0.33217f },  // Rule 105: PS & NS & ME & HI & NS & PB
        {  3,  4,  3,  3,  1,  3, -0.437776f,  0.278886f,  0.33043f },  // Rule 106: ZE & PS & HI & HI & NS & PS
        {  2,  4,  3,  4,  2,  1, -0.029487f, -0.623836f,  0.32789f },  // Rule 107: NS & PS & HI & VH & ZE & NS
        {  3,  3,  3,  3,  2,  4, -0.036721f,  0.696857f,  0.32232f },  // Rule 108: ZE & ZE & HI & HI & ZE & PB
        {  2,  4,  3,  3,  2,  3, -0.045211f,  0.104865f,  0.32144f },  // Rule 109: NS & PS & HI & HI & ZE & PS
        {  3,  4,  4,  3,  1,  2, -0.423332f, -0.144020f,  0.31859f },  // Rule 110: ZE & PS & VH & HI & NS & ZE
        {  4,  1,  3,  4,  3,  3,  0.182448f,  0.600834f,  0.31413f },  // Rule 111: PS & NM & HI & VH & PS & PS
        {  4,  2,  3,  3,  1,  2, -0.310763f,  0.145559f,  0.31106f },  // Rule 112: PS & NS & HI & HI & NS & ZE
        {  2,  2,  3,  3,  4,  0,  0.844852f, -0.681801f,  0.30897f },  // Rule 113: NS & NS & HI & HI & PB & NB
        {  4,  2,  3,  4,  3,  4,  0.297892f,  0.836639f,  0.30525f },  // Rule 114: PS & NS & HI & VH & PS & PB
        {  3,  2,  4,  3,  3,  2,  0.394856f, -0.003805f,  0.30391f },  // Rule 115: ZE & NS & VH & HI & PS & ZE
        {  5,  1,  3,  4,  2,  3,  0.059412f,  0.726667f,  0.30346f },  // Rule 116: PM & NM & HI & VH & ZE & PS
        {  5,  1,  2,  2,  1,  4, -0.335157f,  0.952631f,  0.30236f },  // Rule 117: PM & NM & ME & ME & NS & PB
        {  3,  2,  3,  4,  3,  3,  0.285037f,  0.442557f,  0.30205f },  // Rule 118: ZE & NS & HI & VH & PS & PS
        {  4,  2,  3,  3,  4,  2,  0.710539f,  0.147315f,  0.30152f },  // Rule 119: PS & NS & HI & HI & PB & ZE
        {  2,  3,  3,  3,  2,  1,  0.083615f, -0.481163f,  0.30135f },  // Rule 120: NS & ZE & HI & HI & ZE & NS
        {  4,  3,  3,  3,  2,  3, -0.101654f,  0.475781f,  0.30120f },  // Rule 121: PS & ZE & HI & HI & ZE & PS
        {  3,  3,  3,  4,  3,  1,  0.264270f, -0.319325f,  0.30035f },  // Rule 122: ZE & ZE & HI & VH & PS & NS
        {  3,  2,  3,  4,  3,  2,  0.282111f,  0.066126f,  0.29469f },  // Rule 123: ZE & NS & HI & VH & PS & ZE
        {  3,  4,  3,  3,  1,  2, -0.349818f, -0.146214f,  0.29456f },  // Rule 124: ZE & PS & HI & HI & NS & ZE
        {  3,  3,  2,  3,  2,  3, -0.052682f,  0.246617f,  0.29418f },  // Rule 125: ZE & ZE & ME & HI & ZE & PS
        {  2,  4,  3,  4,  2,  2,  0.017663f, -0.154279f,  0.29247f },  // Rule 126: NS & PS & HI & VH & ZE & ZE
        {  3,  2,  3,  4,  2,  2,  0.031322f,  0.135651f,  0.28900f },  // Rule 127: ZE & NS & HI & VH & ZE & ZE
        {  2,  5,  3,  4,  1,  0, -0.256233f, -0.836905f,  0.28455f },  // Rule 128: NS & PM & HI & VH & NS & NB
        {  3,  3,  3,  3,  2,  0,  0.016210f, -0.660886f,  0.28429f },  // Rule 129: ZE & ZE & HI & HI & ZE & NB
        {  4,  2,  3,  4,  2,  3, -0.029085f,  0.534696f,  0.28358f },  // Rule 130: PS & NS & HI & VH & ZE & PS
        {  4,  4,  3,  3,  1,  4, -0.510156f,  0.694377f,  0.28118f },  // Rule 131: PS & PS & HI & HI & NS & PB
        {  4,  3,  4,  3,  1,  4, -0.444545f,  0.797147f,  0.27967f },  // Rule 132: PS & ZE & VH & HI & NS & PB
        {  4,  2,  3,  3,  4,  1,  0.788251f, -0.083283f,  0.27910f },  // Rule 133: PS & NS & HI & HI & PB & NS
        {  4,  1,  3,  4,  3,  4,  0.164408f,  0.995341f,  0.27893f },  // Rule 134: PS & NM & HI & VH & PS & PB
        {  2,  5,  3,  4,  1,  1, -0.214767f, -0.731446f,  0.27830f },  // Rule 135: NS & PM & HI & VH & NS & NS
        {  3,  2,  3,  3,  3,  0,  0.444182f, -0.638230f,  0.27828f },  // Rule 136: ZE & NS & HI & HI & PS & NB
        {  4,  3,  3,  4,  2,  3, -0.092822f,  0.345166f,  0.27641f },  // Rule 137: PS & ZE & HI & VH & ZE & PS
        {  3,  2,  4,  3,  2,  2,  0.040745f,  0.125730f,  0.27641f },  // Rule 138: ZE & NS & VH & HI & ZE & ZE
        {  1,  4,  3,  4,  2,  1,  0.042866f, -0.592989f,  0.27558f },  // Rule 139: NM & PS & HI & VH & ZE & NS
        {  2,  4,  2,  2,  3,  1,  0.219032f, -0.644221f,  0.27005f },  // Rule 140: NS & PS & ME & ME & PS & NS
        {  2,  3,  4,  3,  3,  2,  0.447025f, -0.185079f,  0.26910f },  // Rule 141: NS & ZE & VH & HI & PS & ZE
        {  3,  3,  3,  3,  1,  0, -0.187042f, -0.735277f,  0.26850f },  // Rule 142: ZE & ZE & HI & HI & NS & NB
        {  4,  3,  3,  3,  2,  2, -0.111338f,  0.094719f,  0.26847f },  // Rule 143: PS & ZE & HI & HI & ZE & ZE
        {  3,  2,  3,  3,  2,  3,  0.064131f,  0.470692f,  0.26828f },  // Rule 144: ZE & NS & HI & HI & ZE & PS
        {  5,  2,  3,  3,  3,  3,  0.290826f,  0.684176f,  0.26820f },  // Rule 145: PM & NS & HI & HI & PS & PS
        {  5,  1,  3,  3,  3,  3,  0.233911f,  0.644844f,  0.26712f },  // Rule 146: PM & NM & HI & HI & PS & PS
        {  2,  3,  3,  3,  4,  0,  0.853319f, -0.761487f,  0.26539f },  // Rule 147: NS & ZE & HI & HI & PB & NB
        {  2,  3,  3,  3,  2,  0,  0.042775f, -0.792890f,  0.26454f },  // Rule 148: NS & ZE & HI & HI & ZE & NB
        {  3,  3,  4,  4,  2,  2, -0.055178f, -0.019924f,  0.26426f },  // Rule 149: ZE & ZE & VH & VH & ZE & ZE
        {  4,  4,  3,  3,  1,  2, -0.426163f,  0.052794f,  0.25914f },  // Rule 150: PS & PS & HI & HI & NS & ZE
        {  3,  3,  4,  3,  1,  3, -0.290690f,  0.278134f,  0.25892f },  // Rule 151: ZE & ZE & VH & HI & NS & PS
        {  4,  1,  3,  3,  3,  3,  0.427096f,  0.665570f,  0.25691f },  // Rule 152: PS & NM & HI & HI & PS & PS
        {  3,  3,  3,  3,  0,  4, -0.732798f,  0.767923f,  0.25646f },  // Rule 153: ZE & ZE & HI & HI & NB & PB
        {  2,  3,  4,  3,  2,  0,  0.080761f, -0.749940f,  0.25368f },  // Rule 154: NS & ZE & VH & HI & ZE & NB
        {  2,  4,  3,  3,  3,  0,  0.310860f, -0.970723f,  0.25351f },  // Rule 155: NS & PS & HI & HI & PS & NB
        {  4,  3,  3,  4,  1,  4, -0.427277f,  0.787564f,  0.25199f },  // Rule 156: PS & ZE & HI & VH & NS & PB
        {  2,  2,  3,  3,  4,  1,  0.936981f, -0.454539f,  0.25107f },  // Rule 157: NS & NS & HI & HI & PB & NS
        {  4,  3,  3,  3,  1,  1, -0.390985f, -0.191282f,  0.25099f },  // Rule 158: PS & ZE & HI & HI & NS & NS
        {  3,  4,  3,  3,  0,  3, -0.785235f,  0.371066f,  0.25098f },  // Rule 159: ZE & PS & HI & HI & NB & PS
        {  2,  4,  4,  3,  2,  1, -0.029803f, -0.621375f,  0.24995f },  // Rule 160: NS & PS & VH & HI & ZE & NS
        {  2,  4,  4,  3,  2,  0, -0.039980f, -0.791278f,  0.24839f },  // Rule 161: NS & PS & VH & HI & ZE & NB
        {  4,  1,  3,  4,  2,  4,  0.094056f,  0.984631f,  0.24723f },  // Rule 162: PS & NM & HI & VH & ZE & PB
        {  4,  3,  3,  4,  1,  3, -0.342009f,  0.479471f,  0.24641f },  // Rule 163: PS & ZE & HI & VH & NS & PS
        {  3,  2,  3,  4,  2,  4,  0.087881f,  0.758828f,  0.24379f },  // Rule 164: ZE & NS & HI & VH & ZE & PB
        {  2,  4,  3,  3,  0,  4, -0.772848f,  0.599423f,  0.24338f },  // Rule 165: NS & PS & HI & HI & NB & PB
        {  2,  2,  4,  3,  3,  1,  0.504585f, -0.399305f,  0.24141f },  // Rule 166: NS & NS & VH & HI & PS & NS
        {  3,  4,  4,  3,  1,  1, -0.388415f, -0.430975f,  0.24116f },  // Rule 167: ZE & PS & VH & HI & NS & NS
        {  1,  5,  2,  2,  3,  0,  0.258582f, -0.949574f,  0.24007f },  // Rule 168: NM & PM & ME & ME & PS & NB
        {  3,  3,  4,  3,  3,  1,  0.237468f, -0.250459f,  0.23927f },  // Rule 169: ZE & ZE & VH & HI & PS & NS
        {  3,  2,  3,  3,  2,  2,  0.087848f,  0.128943f,  0.23892f },  // Rule 170: ZE & NS & HI & HI & ZE & ZE
        {  3,  4,  3,  3,  2,  2, -0.090063f, -0.142663f,  0.23878f },  // Rule 171: ZE & PS & HI & HI & ZE & ZE
        {  3,  2,  4,  4,  2,  3,  0.000430f,  0.240709f,  0.23752f },  // Rule 172: ZE & NS & VH & VH & ZE & PS
        {  4,  2,  4,  3,  1,  4, -0.357812f,  0.918477f,  0.23695f },  // Rule 173: PS & NS & VH & HI & NS & PB
        {  3,  2,  3,  3,  2,  1,  0.077210f, -0.159106f,  0.23599f },  // Rule 174: ZE & NS & HI & HI & ZE & NS
        {  3,  3,  3,  4,  1,  3, -0.351514f,  0.420570f,  0.23432f },  // Rule 175: ZE & ZE & HI & VH & NS & PS
        {  2,  3,  3,  4,  3,  1,  0.266144f, -0.493920f,  0.23324f },  // Rule 176: NS & ZE & HI & VH & PS & NS
        {  3,  3,  2,  4,  2,  1,  0.092020f, -0.261343f,  0.23261f },  // Rule 177: ZE & ZE & ME & VH & ZE & NS
        {  4,  4,  3,  4,  1,  4, -0.602140f,  0.818328f,  0.23244f },  // Rule 178: PS & PS & HI & VH & NS & PB
        {  2,  5,  3,  3,  1,  0, -0.346932f, -0.969511f,  0.23221f },  // Rule 179: NS & PM & HI & HI & NS & NB
        {  5,  2,  3,  3,  1,  4, -0.302780f,  0.998727f,  0.23161f },  // Rule 180: PM & NS & HI & HI & NS & PB
        {  1,  5,  1,  2,  3,  1,  0.273138f, -0.676753f,  0.23138f },  // Rule 181: NM & PM & LO & ME & PS & NS
        {  4,  3,  3,  3,  1,  3, -0.329675f,  0.511412f,  0.23006f },  // Rule 182: PS & ZE & HI & HI & NS & PS
        {  3,  2,  4,  3,  3,  3,  0.400007f,  0.309004f,  0.22925f },  // Rule 183: ZE & NS & VH & HI & PS & PS
        {  3,  3,  4,  3,  1,  2, -0.229742f,  0.100203f,  0.22822f },  // Rule 184: ZE & ZE & VH & HI & NS & ZE
        {  3,  2,  3,  4,  3,  4,  0.289215f,  0.680338f,  0.22763f },  // Rule 185: ZE & NS & HI & VH & PS & PB
        {  3,  2,  3,  4,  2,  3,  0.044445f,  0.319045f,  0.22728f },  // Rule 186: ZE & NS & HI & VH & ZE & PS
        {  2,  4,  2,  3,  2,  1,  0.140544f, -0.595021f,  0.22721f },  // Rule 187: NS & PS & ME & HI & ZE & NS
        {  4,  3,  4,  3,  1,  3, -0.336395f,  0.405122f,  0.22683f },  // Rule 188: PS & ZE & VH & HI & NS & PS
        {  3,  4,  4,  3,  0,  1, -0.922355f, -0.462665f,  0.22655f },  // Rule 189: ZE & PS & VH & HI & NB & NS
        {  3,  3,  4,  3,  2,  3, -0.143456f,  0.187349f,  0.22583f },  // Rule 190: ZE & ZE & VH & HI & ZE & PS
        {  3,  4,  3,  3,  1,  4, -0.477414f,  0.659778f,  0.22504f },  // Rule 191: ZE & PS & HI & HI & NS & PB
        {  3,  4,  3,  3,  3,  2,  0.264982f, -0.042569f,  0.22361f },  // Rule 192: ZE & PS & HI & HI & PS & ZE
        {  5,  2,  3,  3,  2,  3, -0.039122f,  0.686503f,  0.22159f },  // Rule 193: PM & NS & HI & HI & ZE & PS
        {  3,  4,  3,  3,  1,  0, -0.295769f, -0.737541f,  0.21974f },  // Rule 194: ZE & PS & HI & HI & NS & NB
        {  1,  4,  3,  4,  1,  2, -0.263031f, -0.227974f,  0.21853f },  // Rule 195: NM & PS & HI & VH & NS & ZE
        {  3,  5,  4,  3,  0,  0, -0.923183f, -0.781650f,  0.21828f },  // Rule 196: ZE & PM & VH & HI & NB & NB
        {  5,  1,  3,  3,  3,  4,  0.225703f,  0.996515f,  0.21782f },  // Rule 197: PM & NM & HI & HI & PS & PB
        {  2,  4,  3,  3,  3,  1,  0.352989f, -0.589390f,  0.21738f },  // Rule 198: NS & PS & HI & HI & PS & NS
        {  2,  3,  4,  3,  3,  1,  0.304249f, -0.427318f,  0.21523f },  // Rule 199: NS & ZE & VH & HI & PS & NS
        {  2,  2,  3,  3,  3,  0,  0.609647f, -0.846819f,  0.21466f },  // Rule 200: NS & NS & HI & HI & PS & NB
        {  4,  2,  4,  3,  2,  3, -0.039901f,  0.633552f,  0.21403f },  // Rule 201: PS & NS & VH & HI & ZE & PS
        {  4,  2,  3,  4,  2,  2,  0.052241f,  0.195565f,  0.21056f },  // Rule 202: PS & NS & HI & VH & ZE & ZE
        {  4,  2,  2,  3,  1,  3, -0.243206f,  0.644683f,  0.20891f },  // Rule 203: PS & NS & ME & HI & NS & PS
        {  4,  3,  4,  3,  1,  2, -0.424165f,  0.139942f,  0.20861f },  // Rule 204: PS & ZE & VH & HI & NS & ZE
        {  2,  4,  4,  3,  1,  0, -0.371828f, -0.873794f,  0.20670f },  // Rule 205: NS & PS & VH & HI & NS & NB
        {  3,  3,  3,  3,  3,  4,  0.337872f,  0.614657f,  0.20570f },  // Rule 206: ZE & ZE & HI & HI & PS & PB
        {  3,  5,  4,  3,  1,  0, -0.545497f, -0.805148f,  0.20468f },  // Rule 207: ZE & PM & VH & HI & NS & NB
        {  3,  2,  2,  2,  2,  2, -0.071343f,  0.142251f,  0.20314f },  // Rule 208: ZE & NS & ME & ME & ZE & ZE
        {  3,  4,  4,  4,  1,  1, -0.336671f, -0.432024f,  0.20289f },  // Rule 209: ZE & PS & VH & VH & NS & NS
        {  5,  1,  3,  3,  2,  4, -0.016059f,  0.979370f,  0.20192f },  // Rule 210: PM & NM & HI & HI & ZE & PB
        {  4,  3,  4,  3,  2,  4, -0.179512f,  0.818121f,  0.20128f },  // Rule 211: PS & ZE & VH & HI & ZE & PB
        {  3,  4,  4,  3,  1,  0, -0.592940f, -0.818612f,  0.20077f },  // Rule 212: ZE & PS & VH & HI & NS & NB
        {  3,  3,  3,  4,  3,  0,  0.306558f, -0.688936f,  0.20076f },  // Rule 213: ZE & ZE & HI & VH & PS & NB
        {  2,  5,  3,  4,  2,  0, -0.147953f, -0.914107f,  0.20033f },  // Rule 214: NS & PM & HI & VH & ZE & NB
        {  5,  1,  1,  1,  1,  4, -0.332347f,  0.888889f,  0.19915f },  // Rule 215: PM & NM & LO & LO & NS & PB
        {  3,  4,  3,  4,  1,  1, -0.286612f, -0.383892f,  0.19823f },  // Rule 216: ZE & PS & HI & VH & NS & NS
        {  2,  3,  4,  3,  2,  1,  0.060106f, -0.507124f,  0.19731f },  // Rule 217: NS & ZE & VH & HI & ZE & NS
        {  5,  1,  1,  2,  1,  3, -0.309019f,  0.695573f,  0.19635f },  // Rule 218: PM & NM & LO & ME & NS & PS
        {  4,  1,  3,  4,  2,  3,  0.126217f,  0.724144f,  0.19530f },  // Rule 219: PS & NM & HI & VH & ZE & PS
        {  3,  3,  3,  3,  3,  0,  0.280530f, -0.698022f,  0.19495f },  // Rule 220: ZE & ZE & HI & HI & PS & NB
        {  2,  3,  3,  3,  2,  2,  0.099968f, -0.203637f,  0.19360f },  // Rule 221: NS & ZE & HI & HI & ZE & ZE
        {  2,  4,  3,  4,  1,  0, -0.240247f, -0.782861f,  0.19286f },  // Rule 222: NS & PS & HI & VH & NS & NB
        {  3,  2,  3,  4,  3,  0,  0.417948f, -0.643997f,  0.19271f },  // Rule 223: ZE & NS & HI & VH & PS & NB
        {  3,  2,  3,  3,  4,  0,  0.814023f, -0.614791f,  0.18897f },  // Rule 224: ZE & NS & HI & HI & PB & NB
        {  4,  1,  2,  2,  1,  4, -0.326386f,  0.957004f,  0.18853f },  // Rule 225: PS & NM & ME & ME & NS & PB
        {  2,  3,  3,  3,  4,  1,  0.671091f, -0.406336f,  0.18840f },  // Rule 226: NS & ZE & HI & HI & PB & NS
        {  2,  5,  4,  3,  0,  0, -0.741033f, -0.962574f,  0.18472f },  // Rule 227: NS & PM & VH & HI & NB & NB
        {  1,  5,  1,  2,  2,  1,  0.231818f, -0.587899f,  0.18150f },  // Rule 228: NM & PM & LO & ME & ZE & NS
        {  3,  2,  3,  3,  1,  3, -0.226063f,  0.556669f,  0.18039f },  // Rule 229: ZE & NS & HI & HI & NS & PS
        {  2,  4,  2,  3,  3,  1,  0.217362f, -0.656488f,  0.17993f },  // Rule 230: NS & PS & ME & HI & PS & NS
        {  4,  2,  3,  4,  1,  3, -0.152831f,  0.560314f,  0.17825f },  // Rule 231: PS & NS & HI & VH & NS & PS
        {  4,  3,  3,  3,  1,  2, -0.334774f,  0.094177f,  0.17825f },  // Rule 232: PS & ZE & HI & HI & NS & ZE
        {  3,  2,  2,  4,  2,  3, -0.075036f,  0.476873f,  0.17823f },  // Rule 233: ZE & NS & ME & VH & ZE & PS
        {  4,  2,  1,  2,  2,  2, -0.085224f,  0.177778f,  0.17716f },  // Rule 234: PS & NS & LO & ME & ZE & ZE
        {  2,  3,  3,  3,  2,  3,  0.029378f,  0.189110f,  0.17607f },  // Rule 235: NS & ZE & HI & HI & ZE & PS
        {  4,  2,  3,  4,  3,  3,  0.197019f,  0.535326f,  0.17592f },  // Rule 236: PS & NS & HI & VH & PS & PS
        {  2,  3,  3,  3,  3,  3,  0.422656f,  0.107833f,  0.17469f },  // Rule 237: NS & ZE & HI & HI & PS & PS
        {  3,  3,  4,  4,  2,  3, -0.043713f,  0.206164f,  0.17166f },  // Rule 238: ZE & ZE & VH & VH & ZE & PS
        {  4,  4,  4,  3,  1,  3, -0.629725f,  0.369738f,  0.17140f },  // Rule 239: PS & PS & VH & HI & NS & PS
        {  4,  3,  4,  3,  3,  3,  0.206213f,  0.602243f,  0.17073f },  // Rule 240: PS & ZE & VH & HI & PS & PS
        {  2,  3,  3,  4,  2,  1,  0.071243f, -0.467235f,  0.17063f },  // Rule 241: NS & ZE & HI & VH & ZE & NS
        {  2,  4,  3,  4,  2,  3, -0.046883f,  0.152698f,  0.17053f },  // Rule 242: NS & PS & HI & VH & ZE & PS
        {  4,  1,  3,  3,  3,  4,  0.440279f,  0.909195f,  0.17028f },  // Rule 243: PS & NM & HI & HI & PS & PB
        {  3,  4,  3,  4,  2,  2, -0.077387f, -0.144306f,  0.16401f },  // Rule 244: ZE & PS & HI & VH & ZE & ZE
        {  4,  3,  3,  4,  0,  4, -0.896096f,  0.766822f,  0.16328f },  // Rule 245: PS & ZE & HI & VH & NB & PB
        {  3,  3,  2,  2,  2,  1,  0.087201f, -0.238321f,  0.16311f },  // Rule 246: ZE & ZE & ME & ME & ZE & NS
        {  3,  4,  3,  4,  2,  1, -0.110974f, -0.400818f,  0.16173f },  // Rule 247: ZE & PS & HI & VH & ZE & NS
        {  3,  4,  3,  3,  2,  3, -0.089060f,  0.122121f,  0.16126f },  // Rule 248: ZE & PS & HI & HI & ZE & PS
        {  3,  4,  3,  3,  2,  1, -0.074848f, -0.427688f,  0.16116f },  // Rule 249: ZE & PS & HI & HI & ZE & NS
        {  3,  3,  4,  4,  1,  3, -0.245229f,  0.236181f,  0.16008f },  // Rule 250: ZE & ZE & VH & VH & NS & PS
        {  1,  5,  2,  3,  3,  0,  0.284770f, -1.000000f,  0.15886f },  // Rule 251: NM & PM & ME & HI & PS & NB
        {  3,  2,  2,  3,  2,  2, -0.117345f,  0.155378f,  0.15415f },  // Rule 252: ZE & NS & ME & HI & ZE & ZE
        {  3,  2,  4,  3,  2,  3,  0.102988f,  0.269712f,  0.15402f },  // Rule 253: ZE & NS & VH & HI & ZE & PS
        {  3,  4,  3,  4,  0,  3, -0.743354f,  0.383259f,  0.15378f },  // Rule 254: ZE & PS & HI & VH & NB & PS
        {  4,  2,  3,  4,  1,  4, -0.186905f,  0.822907f,  0.15306f },  // Rule 255: PS & NS & HI & VH & NS & PB
        {  3,  3,  4,  4,  1,  2, -0.214013f,  0.084507f,  0.15285f },  // Rule 256: ZE & ZE & VH & VH & NS & ZE
        {  1,  5,  2,  2,  3,  1,  0.245603f, -0.689780f,  0.15143f },  // Rule 257: NM & PM & ME & ME & PS & NS
        {  2,  5,  4,  3,  1,  0, -0.441646f, -0.864615f,  0.14815f },  // Rule 258: NS & PM & VH & HI & NS & NB
        {  3,  3,  4,  4,  2,  1, -0.095520f, -0.202977f,  0.14693f },  // Rule 259: ZE & ZE & VH & VH & ZE & NS
        {  3,  2,  3,  4,  2,  1,  0.037492f, -0.149888f,  0.14688f },  // Rule 260: ZE & NS & HI & VH & ZE & NS
        {  3,  4,  3,  4,  1,  2, -0.342720f, -0.143674f,  0.14630f },  // Rule 261: ZE & PS & HI & VH & NS & ZE
        {  3,  5,  3,  3,  1,  2, -0.674470f, -0.148722f,  0.14608f },  // Rule 262: ZE & PM & HI & HI & NS & ZE
        {  3,  2,  4,  3,  2,  1,  0.078008f, -0.153830f,  0.14527f },  // Rule 263: ZE & NS & VH & HI & ZE & NS
        {  3,  2,  2,  3,  2,  3, -0.114604f,  0.385964f,  0.14317f },  // Rule 264: ZE & NS & ME & HI & ZE & PS
        {  4,  2,  4,  4,  2,  3,  0.015778f,  0.488327f,  0.14191f },  // Rule 265: PS & NS & VH & VH & ZE & PS
        {  2,  3,  3,  4,  3,  2,  0.397010f, -0.252166f,  0.14185f },  // Rule 266: NS & ZE & HI & VH & PS & ZE
        {  4,  2,  4,  3,  3,  3,  0.289413f,  0.565091f,  0.13729f },  // Rule 267: PS & NS & VH & HI & PS & PS
        {  5,  1,  1,  2,  2,  3, -0.215427f,  0.589725f,  0.13597f },  // Rule 268: PM & NM & LO & ME & ZE & PS
        {  2,  4,  3,  4,  3,  0,  0.204940f, -0.763127f,  0.13567f },  // Rule 269: NS & PS & HI & VH & PS & NB
        {  3,  4,  3,  3,  2,  0, -0.101424f, -0.776375f,  0.13312f },  // Rule 270: ZE & PS & HI & HI & ZE & NB
        {  4,  2,  2,  4,  2,  4, -0.063819f,  0.843940f,  0.13156f },  // Rule 271: PS & NS & ME & VH & ZE & PB
        {  3,  2,  4,  4,  2,  2,  0.108581f,  0.129554f,  0.12485f },  // Rule 272: ZE & NS & VH & VH & ZE & ZE
        {  3,  3,  4,  3,  2,  0,  0.031617f, -0.652410f,  0.12245f },  // Rule 273: ZE & ZE & VH & HI & ZE & NB
        {  3,  4,  4,  3,  2,  1, -0.157415f, -0.623995f,  0.11963f },  // Rule 274: ZE & PS & VH & HI & ZE & NS
        {  3,  3,  3,  4,  2,  0,  0.031392f, -0.626820f,  0.11721f },  // Rule 275: ZE & ZE & HI & VH & ZE & NB
        {  4,  1,  2,  2,  2,  3, -0.150811f,  0.648078f,  0.11617f },  // Rule 276: PS & NM & ME & ME & ZE & PS
        {  2,  5,  2,  2,  3,  0,  0.281730f, -0.940208f,  0.11437f },  // Rule 277: NS & PM & ME & ME & PS & NB
        {  3,  1,  3,  3,  4,  3,  0.764003f,  0.635979f,  0.11384f },  // Rule 278: ZE & NM & HI & HI & PB & PS
        {  1,  5,  2,  2,  2,  1,  0.188599f, -0.656648f,  0.11300f },  // Rule 279: NM & PM & ME & ME & ZE & NS
        {  3,  3,  4,  4,  1,  1, -0.269052f, -0.149320f,  0.10769f },  // Rule 280: ZE & ZE & VH & VH & NS & NS
        {  4,  1,  2,  2,  1,  3, -0.241336f,  0.670129f,  0.10369f },  // Rule 281: PS & NM & ME & ME & NS & PS
        {  3,  4,  3,  4,  1,  0, -0.276689f, -0.705569f,  0.10193f },  // Rule 282: ZE & PS & HI & VH & NS & NB
        {  4,  1,  2,  2,  2,  4, -0.136971f,  0.902955f,  0.10166f },  // Rule 283: PS & NM & ME & ME & ZE & PB
        {  2,  4,  1,  2,  2,  2,  0.104879f, -0.205755f,  0.09926f },  // Rule 284: NS & PS & LO & ME & ZE & ZE
        {  3,  2,  3,  3,  1,  4, -0.189736f,  0.856097f,  0.09645f },  // Rule 285: ZE & NS & HI & HI & NS & PB
        {  4,  3,  3,  4,  2,  4, -0.079983f,  0.610953f,  0.09465f },  // Rule 286: PS & ZE & HI & VH & ZE & PB
        {  2,  4,  2,  2,  3,  0,  0.219135f, -0.874549f,  0.07634f },  // Rule 287: NS & PS & ME & ME & PS & NB
        {  3,  4,  2,  3,  2,  1,  0.133730f, -0.545908f,  0.07577f },  // Rule 288: ZE & PS & ME & HI & ZE & NS
        {  1,  5,  1,  2,  2,  2,  0.178143f, -0.355556f,  0.07180f },  // Rule 289: NM & PM & LO & ME & ZE & ZE
    };

    private const int N_RULES = 289;
    private const int N_INPUTS = 6;

    // ===== 模糊推理接口 =====
    /// <summary>
    /// 模糊推理：输入特征 → 输出控制量
    /// </summary>
    /// <param name="frontLRDiff">front_lr_diff（[-1.0,1.0]）</param>
    /// <param name="rearLRDiff">rear_lr_diff（[-1.0,1.0]）</param>
    /// <param name="frontCenter">front_center（[0.0,1.0]）</param>
    /// <param name="rearCenter">rear_center（[0.0,1.0]）</param>
    /// <param name="accVx">accumulated_vx（[-1.0,1.0]）</param>
    /// <param name="accOmega">accumulated_omega（[-1.0,1.0]）</param>
    public static void Evaluate(
        float frontLRDiff,
        float rearLRDiff,
        float frontCenter,
        float rearCenter,
        float accVx,
        float accOmega,
        out float outputVx, out float outputOmega)
    {
        float sumWeightedVx = 0f;
        float sumWeightedOmega = 0f;
        float sumWeights = 0f;

        for (int r = 0; r < N_RULES; r++)
        {
            int mf0 = (int)Rules[r, 0];
            int mf1 = (int)Rules[r, 1];
            int mf2 = (int)Rules[r, 2];
            int mf3 = (int)Rules[r, 3];
            int mf4 = (int)Rules[r, 4];
            int mf5 = (int)Rules[r, 5];

            // 模糊化：计算各输入的隶属度
            float mu0 = TriMF(frontLRDiff, MF_FrontLR[mf0, 0], MF_FrontLR[mf0, 1], MF_FrontLR[mf0, 2]);
            float mu1 = TriMF(rearLRDiff, MF_RearLR[mf1, 0], MF_RearLR[mf1, 1], MF_RearLR[mf1, 2]);
            float mu2 = TriMF(frontCenter, MF_FrontCenter[mf2, 0], MF_FrontCenter[mf2, 1], MF_FrontCenter[mf2, 2]);
            float mu3 = TriMF(rearCenter, MF_RearCenter[mf3, 0], MF_RearCenter[mf3, 1], MF_RearCenter[mf3, 2]);
            float mu4 = TriMF(accVx, MF_AccVx[mf4, 0], MF_AccVx[mf4, 1], MF_AccVx[mf4, 2]);
            float mu5 = TriMF(accOmega, MF_AccOmega[mf5, 0], MF_AccOmega[mf5, 1], MF_AccOmega[mf5, 2]);

            // AND运算（乘积法）
            float firing = mu0 * mu1 * mu2 * mu3 * mu4 * mu5;

            if (firing > 1e-6f)
            {
                float outVx = Rules[r, 6];
                float outOmega = Rules[r, 7];
                float ruleWeight = Rules[r, 8];
                float w = firing * ruleWeight;

                sumWeightedVx += w * outVx;
                sumWeightedOmega += w * outOmega;
                sumWeights += w;
            }
        }

        if (sumWeights > 1e-6f)
        {
            outputVx = Mathf.Clamp(sumWeightedVx / sumWeights, -1f, 1f);
            outputOmega = Mathf.Clamp(sumWeightedOmega / sumWeights, -1f, 1f);
        }
        else
        {
            outputVx = 0f;
            outputOmega = 0f;
        }
    }

    // ===== 三角形隶属函数 =====
    private static float TriMF(float x, float a, float b, float c)
    {
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
        float accVx, float accOmega,
        out float outputVx, out float outputOmega)
    {
        float invMax = 1f / Mathf.Max(1e-6f, maxField);
        float frontLRDiff = Mathf.Clamp((sensorFL - sensorFR) * invMax, -1f, 1f);
        float rearLRDiff = Mathf.Clamp((sensorRL - sensorRR) * invMax, -1f, 1f);
        float frontCenter = Mathf.Clamp01(sensorFC * invMax);
        float rearCenter = Mathf.Clamp01(sensorRC * invMax);
        Evaluate(frontLRDiff, rearLRDiff, frontCenter, rearCenter, accVx, accOmega, out outputVx, out outputOmega);
    }

    /// <summary>
    /// 获取规则库信息（用于调试）
    /// </summary>
    public static string GetInfo()
    {
        return $"FuzzyController: {N_RULES} rules, generated 2026-03-10 13:29:31";
    }
}