// 决策树规则库 - 自动生成（离散动作分类）
// 数据样本数: 300000
// 准确率: 0.8253
// 树深度: 12
// 动作索引: 0=急左转, 1=大左转, 2=左转, 3=小左转, 4=微左转, 5=直行, 6=微右转, 7=小右转, 8=右转, 9=大右转, 10=急右转

using System;

public class DecisionTreeRules
{
    // 特征变量（需要从观测中获取）
    private float sensor0, sensor1, sensor2, sensor3, sensor4, sensor5;
    private float last_action_state;
    private float actual_vz, actual_vx, actual_omega;
    
    // 设置观测值（10维特征）
    public void SetObservations(float[] obs)
    {
        if (obs.Length < 10) throw new ArgumentException("观测数组长度不足10");
        sensor0 = obs[0];
        sensor1 = obs[1];
        sensor2 = obs[2];
        sensor3 = obs[3];
        sensor4 = obs[4];
        sensor5 = obs[5];
        last_action_state = obs[6];
        actual_vz = obs[7];
        actual_vx = obs[8];
        actual_omega = obs[9];
    }
    
    // 预测离散动作索引（0-10）
    public int PredictDiscreteAction()
    {
        if (sensor1 <= 0.798045f)
            if (actual_vz <= 1.082263f)
                if (actual_vz <= 0.947619f)
                    if (sensor2 <= 0.302478f)
                        if (sensor3 <= 0.773299f)
                            if (actual_vz <= 0.922181f)
                                if (sensor5 <= 0.573114f)
                                    if (sensor0 <= 0.279750f)
                                        if (actual_vx <= -0.004875f)
                                            if (last_action_state <= 0.650000f)
                                                if (sensor0 <= 0.225028f)
                                                    if (actual_vx <= -0.009990f)
                                                        return 9;
                                                    else
                                                        return 10;
                                                else
                                                    if (actual_vz <= 0.744155f)
                                                        return 7;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor2 <= 0.232354f)
                                                    if (actual_omega <= 0.060157f)
                                                        return 7;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor5 <= 0.192275f)
                                                        return 6;
                                                    else
                                                        return 7;
                                        else
                                            if (actual_vx <= 0.023794f)
                                                if (last_action_state <= 0.550000f)
                                                    if (sensor0 <= 0.216089f)
                                                        return 9;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= 0.001162f)
                                                        return 9;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor0 <= 0.235732f)
                                                    if (sensor4 <= 0.913978f)
                                                        return 1;
                                                    else
                                                        return 0;
                                                else
                                                    if (last_action_state <= 0.550000f)
                                                        return 4;
                                                    else
                                                        return 1;
                                    else
                                        if (last_action_state <= 0.150000f)
                                            if (actual_vx <= -0.004192f)
                                                if (sensor4 <= 0.593974f)
                                                    if (sensor0 <= 0.489564f)
                                                        return 10;
                                                    else
                                                        return 8;
                                                else
                                                    if (sensor0 <= 0.320987f)
                                                        return 9;
                                                    else
                                                        return 7;
                                            else
                                                if (sensor2 <= 0.177678f)
                                                    if (sensor4 <= 0.708198f)
                                                        return 3;
                                                    else
                                                        return 4;
                                                else
                                                    if (actual_vz <= 0.880457f)
                                                        return 5;
                                                    else
                                                        return 1;
                                        else
                                            if (actual_vx <= 0.019399f)
                                                if (actual_vx <= -0.008243f)
                                                    if (sensor4 <= 0.859567f)
                                                        return 5;
                                                    else
                                                        return 7;
                                                else
                                                    if (sensor2 <= 0.143751f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor0 <= 0.387463f)
                                                    if (sensor0 <= 0.319354f)
                                                        return 4;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor3 <= 0.337137f)
                                                        return 1;
                                                    else
                                                        return 4;
                                else
                                    if (sensor4 <= 0.744904f)
                                        if (actual_vx <= -0.012764f)
                                            if (actual_vz <= 0.592367f)
                                                if (actual_vz <= 0.305369f)
                                                    if (actual_vz <= 0.264122f)
                                                        return 5;
                                                    else
                                                        return 4;
                                                else
                                                    if (sensor3 <= 0.078880f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                return 3;
                                        else
                                            if (sensor3 <= 0.051427f)
                                                return 3;
                                            else
                                                if (sensor0 <= 0.658373f)
                                                    if (actual_omega <= -0.112613f)
                                                        return 1;
                                                    else
                                                        return 0;
                                                else
                                                    return 4;
                                    else
                                        if (actual_vx <= 0.001763f)
                                            if (actual_vz <= 0.785673f)
                                                if (actual_vx <= -0.002119f)
                                                    if (sensor4 <= 0.794362f)
                                                        return 0;
                                                    else
                                                        return 0;
                                                else
                                                    if (actual_omega <= 0.004810f)
                                                        return 0;
                                                    else
                                                        return 4;
                                            else
                                                if (actual_vz <= 0.886275f)
                                                    if (sensor5 <= 0.582754f)
                                                        return 5;
                                                    else
                                                        return 0;
                                                else
                                                    if (actual_vx <= -0.003349f)
                                                        return 1;
                                                    else
                                                        return 0;
                                        else
                                            if (sensor3 <= 0.203990f)
                                                if (sensor0 <= 0.491398f)
                                                    if (sensor5 <= 0.577632f)
                                                        return 4;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor1 <= 0.535587f)
                                                        return 3;
                                                    else
                                                        return 0;
                                            else
                                                if (sensor5 <= 0.698497f)
                                                    if (sensor2 <= 0.150624f)
                                                        return 4;
                                                    else
                                                        return 3;
                                                else
                                                    if (actual_vx <= 0.035612f)
                                                        return 3;
                                                    else
                                                        return 3;
                            else
                                if (sensor5 <= 0.431052f)
                                    if (sensor3 <= 0.663854f)
                                        if (actual_vz <= 0.935982f)
                                            if (sensor1 <= 0.796480f)
                                                if (sensor3 <= 0.236351f)
                                                    if (sensor1 <= 0.792680f)
                                                        return 5;
                                                    else
                                                        return 4;
                                                else
                                                    if (actual_vx <= 0.000838f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                return 1;
                                        else
                                            if (actual_vz <= 0.947520f)
                                                if (actual_vx <= 0.013921f)
                                                    if (sensor4 <= 0.841872f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor4 <= 0.781543f)
                                                        return 5;
                                                    else
                                                        return 4;
                                            else
                                                if (actual_omega <= -0.167938f)
                                                    return 5;
                                                else
                                                    if (actual_vz <= 0.947613f)
                                                        return 4;
                                                    else
                                                        return 5;
                                    else
                                        if (sensor1 <= 0.466982f)
                                            return 6;
                                        else
                                            if (sensor4 <= 0.803236f)
                                                if (actual_vx <= -0.001546f)
                                                    if (sensor0 <= 0.612240f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor1 <= 0.750171f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                return 6;
                                else
                                    if (sensor1 <= 0.721730f)
                                        return 0;
                                    else
                                        if (sensor5 <= 0.641363f)
                                            if (actual_vz <= 0.943770f)
                                                if (sensor1 <= 0.790113f)
                                                    if (actual_vz <= 0.929484f)
                                                        return 4;
                                                    else
                                                        return 0;
                                                else
                                                    return 5;
                                            else
                                                return 4;
                                        else
                                            if (sensor0 <= 0.546712f)
                                                return 3;
                                            else
                                                return 2;
                        else
                            if (sensor4 <= 0.405063f)
                                if (last_action_state <= 0.150000f)
                                    if (actual_vx <= 0.010834f)
                                        if (sensor2 <= 0.019358f)
                                            if (sensor3 <= 0.812668f)
                                                return 5;
                                            else
                                                if (actual_vx <= 0.006604f)
                                                    if (sensor1 <= 0.321873f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor1 <= 0.309914f)
                                                        return 5;
                                                    else
                                                        return 6;
                                        else
                                            if (sensor3 <= 0.875030f)
                                                if (actual_omega <= -0.200868f)
                                                    return 6;
                                                else
                                                    if (actual_omega <= -0.200815f)
                                                        return 5;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor4 <= 0.347389f)
                                                    return 5;
                                                else
                                                    return 6;
                                    else
                                        return 5;
                                else
                                    if (sensor0 <= 0.846574f)
                                        if (actual_omega <= -0.163089f)
                                            return 6;
                                        else
                                            if (actual_omega <= -0.161531f)
                                                return 5;
                                            else
                                                if (sensor0 <= 0.845600f)
                                                    if (sensor2 <= 0.031396f)
                                                        return 6;
                                                    else
                                                        return 6;
                                                else
                                                    return 5;
                                    else
                                        return 5;
                            else
                                if (sensor5 <= 0.046108f)
                                    if (actual_omega <= -0.199461f)
                                        return 6;
                                    else
                                        return 5;
                                else
                                    if (last_action_state <= 0.350000f)
                                        if (actual_omega <= -0.151714f)
                                            if (actual_vz <= 0.941839f)
                                                if (sensor5 <= 0.060841f)
                                                    return 5;
                                                else
                                                    return 6;
                                            else
                                                if (actual_vz <= 0.946884f)
                                                    return 6;
                                                else
                                                    if (sensor0 <= 0.739195f)
                                                        return 5;
                                                    else
                                                        return 6;
                                        else
                                            if (actual_vx <= 0.002470f)
                                                if (actual_omega <= -0.138469f)
                                                    if (sensor0 <= 0.727891f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    return 6;
                                            else
                                                if (sensor2 <= 0.058399f)
                                                    return 6;
                                                else
                                                    return 5;
                                    else
                                        return 5;
                    else
                        if (last_action_state <= 0.550000f)
                            if (last_action_state <= 0.450000f)
                                if (actual_omega <= 0.177778f)
                                    if (actual_vx <= -0.010608f)
                                        if (sensor5 <= 0.193542f)
                                            return 9;
                                        else
                                            if (sensor1 <= 0.741399f)
                                                if (actual_vz <= 0.925417f)
                                                    if (sensor5 <= 0.743731f)
                                                        return 10;
                                                    else
                                                        return 9;
                                                else
                                                    if (actual_vx <= -0.013793f)
                                                        return 10;
                                                    else
                                                        return 9;
                                            else
                                                if (last_action_state <= 0.250000f)
                                                    if (actual_omega <= 0.061466f)
                                                        return 10;
                                                    else
                                                        return 9;
                                                else
                                                    if (actual_omega <= 0.145076f)
                                                        return 6;
                                                    else
                                                        return 7;
                                    else
                                        if (actual_vx <= -0.003454f)
                                            if (last_action_state <= 0.350000f)
                                                if (sensor5 <= 0.886776f)
                                                    if (actual_vx <= -0.003522f)
                                                        return 10;
                                                    else
                                                        return 10;
                                                else
                                                    return 9;
                                            else
                                                if (sensor4 <= 0.338575f)
                                                    if (sensor0 <= 0.026129f)
                                                        return 9;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor5 <= 0.569712f)
                                                        return 9;
                                                    else
                                                        return 10;
                                        else
                                            if (last_action_state <= 0.150000f)
                                                if (sensor1 <= 0.621462f)
                                                    if (actual_vz <= 0.946732f)
                                                        return 10;
                                                    else
                                                        return 9;
                                                else
                                                    if (actual_vz <= 0.902245f)
                                                        return 9;
                                                    else
                                                        return 10;
                                            else
                                                if (actual_omega <= 0.103118f)
                                                    if (sensor1 <= 0.680789f)
                                                        return 10;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor4 <= 0.350465f)
                                                        return 10;
                                                    else
                                                        return 10;
                                else
                                    if (sensor1 <= 0.648503f)
                                        if (sensor2 <= 0.861037f)
                                            if (actual_omega <= 0.178342f)
                                                if (sensor5 <= 0.831005f)
                                                    if (sensor0 <= 0.042846f)
                                                        return 7;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_omega <= 0.178026f)
                                                        return 10;
                                                    else
                                                        return 7;
                                            else
                                                if (sensor4 <= 0.473508f)
                                                    if (sensor2 <= 0.837196f)
                                                        return 9;
                                                    else
                                                        return 10;
                                                else
                                                    if (actual_vz <= 0.906908f)
                                                        return 9;
                                                    else
                                                        return 10;
                                        else
                                            return 9;
                                    else
                                        if (sensor0 <= 0.214947f)
                                            if (sensor3 <= 0.114121f)
                                                if (actual_vz <= 0.942149f)
                                                    return 9;
                                                else
                                                    return 7;
                                            else
                                                if (actual_omega <= 0.203456f)
                                                    if (sensor1 <= 0.739765f)
                                                        return 10;
                                                    else
                                                        return 9;
                                                else
                                                    if (sensor3 <= 0.550767f)
                                                        return 9;
                                                    else
                                                        return 10;
                                        else
                                            if (sensor0 <= 0.226613f)
                                                return 7;
                                            else
                                                return 6;
                            else
                                if (actual_omega <= 0.168213f)
                                    if (actual_vx <= -0.001666f)
                                        if (sensor2 <= 0.829033f)
                                            if (actual_vx <= -0.009811f)
                                                if (sensor1 <= 0.625069f)
                                                    if (actual_vz <= 0.080025f)
                                                        return 10;
                                                    else
                                                        return 9;
                                                else
                                                    if (sensor1 <= 0.627640f)
                                                        return 10;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor4 <= 0.371782f)
                                                    if (sensor0 <= 0.030444f)
                                                        return 10;
                                                    else
                                                        return 7;
                                                else
                                                    if (sensor0 <= 0.200153f)
                                                        return 10;
                                                    else
                                                        return 6;
                                        else
                                            if (sensor4 <= 0.378229f)
                                                if (sensor2 <= 0.843362f)
                                                    if (sensor2 <= 0.829787f)
                                                        return 9;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor2 <= 0.846883f)
                                                        return 10;
                                                    else
                                                        return 10;
                                            else
                                                if (sensor0 <= 0.037514f)
                                                    if (sensor2 <= 0.843661f)
                                                        return 10;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor4 <= 0.426354f)
                                                        return 10;
                                                    else
                                                        return 10;
                                    else
                                        if (actual_omega <= 0.002070f)
                                            if (actual_vx <= 0.000653f)
                                                if (actual_omega <= 0.001554f)
                                                    return 9;
                                                else
                                                    if (sensor5 <= 0.063367f)
                                                        return 9;
                                                    else
                                                        return 10;
                                            else
                                                return 10;
                                        else
                                            if (sensor4 <= 0.357624f)
                                                if (sensor3 <= 0.024655f)
                                                    if (actual_vz <= 0.940626f)
                                                        return 10;
                                                    else
                                                        return 10;
                                                else
                                                    return 9;
                                            else
                                                if (sensor0 <= 0.041352f)
                                                    if (actual_vz <= 0.902561f)
                                                        return 9;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor5 <= 0.826842f)
                                                        return 10;
                                                    else
                                                        return 10;
                                else
                                    if (sensor2 <= 0.836329f)
                                        if (sensor4 <= 0.429945f)
                                            if (sensor0 <= 0.025950f)
                                                if (actual_vz <= 0.940147f)
                                                    if (actual_omega <= 0.168849f)
                                                        return 1;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor0 <= 0.015824f)
                                                        return 7;
                                                    else
                                                        return 10;
                                            else
                                                if (sensor2 <= 0.827290f)
                                                    if (sensor1 <= 0.517205f)
                                                        return 10;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor3 <= 0.035636f)
                                                        return 7;
                                                    else
                                                        return 10;
                                        else
                                            if (actual_omega <= 0.188377f)
                                                if (sensor0 <= 0.135289f)
                                                    if (sensor3 <= 0.027916f)
                                                        return 9;
                                                    else
                                                        return 10;
                                                else
                                                    if (actual_vz <= 0.932830f)
                                                        return 9;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor1 <= 0.680296f)
                                                    if (sensor3 <= 0.060600f)
                                                        return 9;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor1 <= 0.719959f)
                                                        return 9;
                                                    else
                                                        return 6;
                                    else
                                        if (actual_vx <= -0.002940f)
                                            if (sensor4 <= 0.370995f)
                                                if (actual_vx <= -0.007141f)
                                                    return 7;
                                                else
                                                    if (actual_vx <= -0.005425f)
                                                        return 9;
                                                    else
                                                        return 10;
                                            else
                                                if (actual_vz <= 0.932896f)
                                                    if (actual_vx <= -0.010782f)
                                                        return 6;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor4 <= 0.434132f)
                                                        return 10;
                                                    else
                                                        return 10;
                                        else
                                            if (sensor0 <= 0.024083f)
                                                if (sensor5 <= 0.800263f)
                                                    if (sensor0 <= 0.020359f)
                                                        return 6;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor2 <= 0.897676f)
                                                        return 10;
                                                    else
                                                        return 8;
                                            else
                                                if (sensor3 <= 0.018475f)
                                                    if (sensor1 <= 0.426503f)
                                                        return 3;
                                                    else
                                                        return 7;
                                                else
                                                    if (sensor5 <= 0.858830f)
                                                        return 10;
                                                    else
                                                        return 6;
                        else
                            if (sensor5 <= 0.754062f)
                                if (sensor5 <= 0.241255f)
                                    if (actual_vx <= -0.013590f)
                                        if (sensor1 <= 0.612619f)
                                            if (sensor2 <= 0.521089f)
                                                if (sensor3 <= 0.324043f)
                                                    if (sensor2 <= 0.303882f)
                                                        return 9;
                                                    else
                                                        return 7;
                                                else
                                                    if (sensor4 <= 0.460261f)
                                                        return 10;
                                                    else
                                                        return 9;
                                            else
                                                if (actual_vx <= -0.040680f)
                                                    if (sensor2 <= 0.523564f)
                                                        return 10;
                                                    else
                                                        return 9;
                                                else
                                                    if (actual_omega <= 0.202878f)
                                                        return 10;
                                                    else
                                                        return 6;
                                        else
                                            if (last_action_state <= 0.950000f)
                                                if (actual_vx <= -0.034936f)
                                                    if (last_action_state <= 0.850000f)
                                                        return 10;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor5 <= 0.163903f)
                                                        return 7;
                                                    else
                                                        return 9;
                                            else
                                                if (sensor3 <= 0.462072f)
                                                    return 7;
                                                else
                                                    if (sensor5 <= 0.221416f)
                                                        return 6;
                                                    else
                                                        return 9;
                                    else
                                        if (sensor0 <= 0.109207f)
                                            if (actual_vx <= 0.000107f)
                                                if (sensor5 <= 0.173226f)
                                                    if (actual_vz <= 0.713100f)
                                                        return 10;
                                                    else
                                                        return 9;
                                                else
                                                    if (sensor0 <= 0.086292f)
                                                        return 6;
                                                    else
                                                        return 10;
                                            else
                                                if (sensor2 <= 0.450781f)
                                                    if (actual_omega <= 0.195759f)
                                                        return 10;
                                                    else
                                                        return 2;
                                                else
                                                    if (sensor3 <= 0.825783f)
                                                        return 10;
                                                    else
                                                        return 9;
                                        else
                                            if (actual_vx <= 0.018098f)
                                                if (sensor3 <= 0.374059f)
                                                    if (sensor1 <= 0.605282f)
                                                        return 9;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_omega <= 0.109829f)
                                                        return 10;
                                                    else
                                                        return 9;
                                            else
                                                if (sensor1 <= 0.638844f)
                                                    if (sensor5 <= 0.141590f)
                                                        return 0;
                                                    else
                                                        return 3;
                                                else
                                                    if (sensor0 <= 0.148976f)
                                                        return 0;
                                                    else
                                                        return 3;
                                else
                                    if (last_action_state <= 0.750000f)
                                        if (last_action_state <= 0.650000f)
                                            if (sensor1 <= 0.716560f)
                                                if (actual_vx <= -0.010649f)
                                                    if (sensor5 <= 0.437561f)
                                                        return 9;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vx <= -0.006471f)
                                                        return 10;
                                                    else
                                                        return 10;
                                            else
                                                if (actual_vx <= -0.008846f)
                                                    if (actual_vx <= -0.023388f)
                                                        return 7;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor0 <= 0.229207f)
                                                        return 10;
                                                    else
                                                        return 6;
                                        else
                                            if (sensor3 <= 0.074153f)
                                                if (actual_omega <= 0.201107f)
                                                    if (actual_vx <= -0.006398f)
                                                        return 6;
                                                    else
                                                        return 10;
                                                else
                                                    if (actual_vz <= 0.938938f)
                                                        return 7;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor2 <= 0.395499f)
                                                    if (actual_vx <= 0.008319f)
                                                        return 7;
                                                    else
                                                        return 3;
                                                else
                                                    if (sensor0 <= 0.163625f)
                                                        return 6;
                                                    else
                                                        return 6;
                                    else
                                        if (sensor5 <= 0.672060f)
                                            if (actual_vz <= 0.843172f)
                                                if (actual_vx <= 0.004922f)
                                                    if (actual_vx <= -0.041080f)
                                                        return 6;
                                                    else
                                                        return 7;
                                                else
                                                    if (actual_omega <= 0.176631f)
                                                        return 5;
                                                    else
                                                        return 0;
                                            else
                                                if (sensor4 <= 0.773710f)
                                                    if (actual_omega <= 0.167419f)
                                                        return 6;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor3 <= 0.752100f)
                                                        return 6;
                                                    else
                                                        return 10;
                                        else
                                            if (last_action_state <= 0.950000f)
                                                if (sensor0 <= 0.109661f)
                                                    if (sensor0 <= 0.060180f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_omega <= 0.196095f)
                                                        return 6;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor2 <= 0.823445f)
                                                    if (sensor3 <= 0.197795f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor2 <= 0.825042f)
                                                        return 0;
                                                    else
                                                        return 5;
                            else
                                if (last_action_state <= 0.850000f)
                                    if (last_action_state <= 0.650000f)
                                        if (actual_vx <= -0.002129f)
                                            if (sensor3 <= 0.050852f)
                                                if (actual_omega <= 0.132114f)
                                                    if (sensor5 <= 0.840092f)
                                                        return 10;
                                                    else
                                                        return 7;
                                                else
                                                    if (actual_vz <= 0.946185f)
                                                        return 10;
                                                    else
                                                        return 7;
                                            else
                                                if (sensor0 <= 0.069683f)
                                                    if (actual_vz <= 0.921650f)
                                                        return 9;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor5 <= 0.805674f)
                                                        return 6;
                                                    else
                                                        return 7;
                                        else
                                            if (sensor3 <= 0.018689f)
                                                return 7;
                                            else
                                                if (actual_vz <= 0.929044f)
                                                    if (sensor2 <= 0.587033f)
                                                        return 6;
                                                    else
                                                        return 10;
                                                else
                                                    if (actual_vz <= 0.939243f)
                                                        return 10;
                                                    else
                                                        return 10;
                                    else
                                        if (last_action_state <= 0.750000f)
                                            if (actual_vx <= -0.004125f)
                                                if (sensor3 <= 0.150986f)
                                                    if (sensor5 <= 0.795357f)
                                                        return 7;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor5 <= 0.756378f)
                                                        return 6;
                                                    else
                                                        return 7;
                                            else
                                                if (sensor3 <= 0.023983f)
                                                    if (actual_vz <= 0.925769f)
                                                        return 3;
                                                    else
                                                        return 7;
                                                else
                                                    if (sensor2 <= 0.590335f)
                                                        return 6;
                                                    else
                                                        return 10;
                                        else
                                            if (sensor3 <= 0.039151f)
                                                if (actual_omega <= 0.150015f)
                                                    if (sensor3 <= 0.034292f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor2 <= 0.841861f)
                                                        return 3;
                                                    else
                                                        return 0;
                                            else
                                                if (sensor3 <= 0.162216f)
                                                    if (sensor4 <= 0.658027f)
                                                        return 7;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vx <= -0.000537f)
                                                        return 10;
                                                    else
                                                        return 0;
                                else
                                    if (last_action_state <= 0.950000f)
                                        if (sensor3 <= 0.146736f)
                                            if (sensor4 <= 0.525387f)
                                                if (sensor5 <= 0.834002f)
                                                    if (sensor3 <= 0.025152f)
                                                        return 3;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor5 <= 0.839094f)
                                                        return 0;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_omega <= 0.146109f)
                                                    if (sensor3 <= 0.124422f)
                                                        return 7;
                                                    else
                                                        return 0;
                                                else
                                                    if (actual_omega <= 0.209459f)
                                                        return 5;
                                                    else
                                                        return 2;
                                        else
                                            if (actual_omega <= 0.186039f)
                                                if (sensor5 <= 0.772262f)
                                                    if (actual_vz <= 0.930727f)
                                                        return 6;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor5 <= 0.780253f)
                                                        return 7;
                                                    else
                                                        return 3;
                                            else
                                                if (actual_vx <= -0.006739f)
                                                    return 5;
                                                else
                                                    if (actual_omega <= 0.205142f)
                                                        return 7;
                                                    else
                                                        return 9;
                                    else
                                        if (sensor0 <= 0.048027f)
                                            if (sensor0 <= 0.046429f)
                                                if (sensor0 <= 0.040355f)
                                                    if (sensor2 <= 0.805553f)
                                                        return 4;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor4 <= 0.526654f)
                                                        return 5;
                                                    else
                                                        return 3;
                                            else
                                                if (sensor1 <= 0.551109f)
                                                    return 4;
                                                else
                                                    if (actual_omega <= 0.161991f)
                                                        return 7;
                                                    else
                                                        return 5;
                                        else
                                            if (actual_vz <= 0.942934f)
                                                if (actual_vx <= -0.001600f)
                                                    if (actual_omega <= 0.200005f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= 0.167827f)
                                                        return 5;
                                                    else
                                                        return 3;
                                            else
                                                if (sensor5 <= 0.813399f)
                                                    if (sensor5 <= 0.758120f)
                                                        return 7;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vx <= -0.003594f)
                                                        return 0;
                                                    else
                                                        return 5;
                else
                    if (sensor3 <= 0.798018f)
                        if (actual_vx <= -0.008311f)
                            if (sensor0 <= 0.112355f)
                                if (last_action_state <= 0.750000f)
                                    if (last_action_state <= 0.550000f)
                                        if (last_action_state <= 0.350000f)
                                            if (actual_omega <= 0.233426f)
                                                if (actual_vz <= 1.069468f)
                                                    return 10;
                                                else
                                                    if (sensor2 <= 0.836322f)
                                                        return 10;
                                                    else
                                                        return 9;
                                            else
                                                return 9;
                                        else
                                            if (actual_omega <= 0.184868f)
                                                if (sensor1 <= 0.533857f)
                                                    if (sensor0 <= 0.050039f)
                                                        return 10;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor0 <= 0.105028f)
                                                        return 10;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor2 <= 0.821746f)
                                                    if (actual_vx <= -0.008526f)
                                                        return 7;
                                                    else
                                                        return 8;
                                                else
                                                    if (actual_omega <= 0.186713f)
                                                        return 3;
                                                    else
                                                        return 10;
                                    else
                                        if (sensor4 <= 0.466222f)
                                            if (actual_omega <= 0.222025f)
                                                if (actual_vx <= -0.008648f)
                                                    if (actual_vz <= 1.075966f)
                                                        return 3;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor5 <= 0.847537f)
                                                        return 5;
                                                    else
                                                        return 4;
                                            else
                                                return 7;
                                        else
                                            if (actual_omega <= 0.169702f)
                                                if (sensor0 <= 0.074787f)
                                                    if (sensor2 <= 0.841203f)
                                                        return 7;
                                                    else
                                                        return 9;
                                                else
                                                    if (sensor1 <= 0.538277f)
                                                        return 10;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor5 <= 0.779947f)
                                                    if (actual_vx <= -0.011654f)
                                                        return 6;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor2 <= 0.839786f)
                                                        return 7;
                                                    else
                                                        return 6;
                                else
                                    if (sensor4 <= 0.670381f)
                                        if (last_action_state <= 0.950000f)
                                            if (sensor4 <= 0.562659f)
                                                if (actual_vx <= -0.008346f)
                                                    if (sensor4 <= 0.402225f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    return 4;
                                            else
                                                if (last_action_state <= 0.850000f)
                                                    if (actual_omega <= 0.192393f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor2 <= 0.819685f)
                                                        return 5;
                                                    else
                                                        return 5;
                                        else
                                            if (actual_vx <= -0.010877f)
                                                if (sensor0 <= 0.016400f)
                                                    if (sensor2 <= 0.836527f)
                                                        return 0;
                                                    else
                                                        return 4;
                                                else
                                                    if (sensor3 <= 0.032191f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_omega <= 0.192511f)
                                                    if (actual_vx <= -0.010390f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor0 <= 0.068951f)
                                                        return 5;
                                                    else
                                                        return 5;
                                    else
                                        if (actual_vz <= 1.056814f)
                                            if (actual_omega <= 0.231565f)
                                                if (sensor2 <= 0.814107f)
                                                    return 6;
                                                else
                                                    if (sensor2 <= 0.817136f)
                                                        return 7;
                                                    else
                                                        return 6;
                                            else
                                                if (actual_vx <= -0.012864f)
                                                    if (sensor1 <= 0.605701f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vx <= -0.011961f)
                                                        return 7;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor5 <= 0.637739f)
                                                if (actual_omega <= 0.182181f)
                                                    if (actual_vx <= -0.009515f)
                                                        return 6;
                                                    else
                                                        return 3;
                                                else
                                                    if (actual_omega <= 0.195221f)
                                                        return 5;
                                                    else
                                                        return 6;
                                            else
                                                if (last_action_state <= 0.950000f)
                                                    if (actual_vx <= -0.010418f)
                                                        return 7;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor3 <= 0.093417f)
                                                        return 5;
                                                    else
                                                        return 5;
                            else
                                if (last_action_state <= 0.750000f)
                                    if (sensor1 <= 0.741024f)
                                        if (actual_omega <= 0.141309f)
                                            if (sensor5 <= 0.509614f)
                                                if (sensor1 <= 0.691587f)
                                                    if (sensor5 <= 0.493360f)
                                                        return 9;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor4 <= 0.773329f)
                                                        return 9;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor4 <= 0.784302f)
                                                    if (sensor3 <= 0.129982f)
                                                        return 10;
                                                    else
                                                        return 9;
                                                else
                                                    if (sensor1 <= 0.655431f)
                                                        return 10;
                                                    else
                                                        return 10;
                                        else
                                            if (sensor2 <= 0.805313f)
                                                if (actual_omega <= 0.198999f)
                                                    if (actual_vx <= -0.009561f)
                                                        return 6;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vx <= -0.011110f)
                                                        return 6;
                                                    else
                                                        return 6;
                                            else
                                                if (actual_vx <= -0.012992f)
                                                    if (sensor1 <= 0.605459f)
                                                        return 7;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor1 <= 0.611475f)
                                                        return 9;
                                                    else
                                                        return 6;
                                    else
                                        if (sensor0 <= 0.250399f)
                                            if (actual_vz <= 1.081562f)
                                                if (sensor3 <= 0.102812f)
                                                    return 9;
                                                else
                                                    if (actual_vz <= 1.069510f)
                                                        return 6;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor2 <= 0.718790f)
                                                    return 9;
                                                else
                                                    return 7;
                                        else
                                            if (sensor0 <= 0.335115f)
                                                if (sensor0 <= 0.261438f)
                                                    return 7;
                                                else
                                                    return 5;
                                            else
                                                if (actual_omega <= 0.031978f)
                                                    return 1;
                                                else
                                                    if (last_action_state <= 0.350000f)
                                                        return 0;
                                                    else
                                                        return 2;
                                else
                                    if (sensor5 <= 0.587902f)
                                        if (sensor1 <= 0.713340f)
                                            if (last_action_state <= 0.950000f)
                                                if (sensor2 <= 0.795543f)
                                                    if (sensor2 <= 0.760643f)
                                                        return 7;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor2 <= 0.797433f)
                                                        return 7;
                                                    else
                                                        return 6;
                                            else
                                                if (actual_omega <= 0.194840f)
                                                    if (sensor2 <= 0.808023f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vx <= -0.012231f)
                                                        return 6;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor3 <= 0.137212f)
                                                if (sensor0 <= 0.203277f)
                                                    if (actual_vx <= -0.012010f)
                                                        return 5;
                                                    else
                                                        return 7;
                                                else
                                                    return 6;
                                            else
                                                if (sensor1 <= 0.762656f)
                                                    if (sensor4 <= 0.751315f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vz <= 1.071922f)
                                                        return 6;
                                                    else
                                                        return 6;
                                    else
                                        if (last_action_state <= 0.950000f)
                                            if (sensor5 <= 0.692162f)
                                                if (actual_omega <= 0.283040f)
                                                    if (sensor3 <= 0.073730f)
                                                        return 7;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vz <= 1.010185f)
                                                        return 7;
                                                    else
                                                        return 6;
                                            else
                                                if (actual_vz <= 1.045516f)
                                                    if (sensor3 <= 0.070510f)
                                                        return 7;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_omega <= 0.214887f)
                                                        return 6;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor4 <= 0.642736f)
                                                if (actual_omega <= 0.232710f)
                                                    if (sensor0 <= 0.127265f)
                                                        return 5;
                                                    else
                                                        return 7;
                                                else
                                                    if (actual_vz <= 1.038033f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_omega <= 0.206025f)
                                                    if (sensor0 <= 0.132231f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_omega <= 0.273938f)
                                                        return 5;
                                                    else
                                                        return 6;
                        else
                            if (sensor3 <= 0.578823f)
                                if (last_action_state <= 0.750000f)
                                    if (sensor0 <= 0.294329f)
                                        if (last_action_state <= 0.550000f)
                                            if (last_action_state <= 0.450000f)
                                                if (actual_omega <= 0.170096f)
                                                    if (sensor5 <= 0.352115f)
                                                        return 4;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor1 <= 0.471193f)
                                                        return 10;
                                                    else
                                                        return 10;
                                            else
                                                if (actual_omega <= 0.172043f)
                                                    if (actual_vz <= 0.958223f)
                                                        return 10;
                                                    else
                                                        return 10;
                                                else
                                                    if (actual_vx <= -0.003977f)
                                                        return 7;
                                                    else
                                                        return 10;
                                        else
                                            if (sensor4 <= 0.572197f)
                                                if (sensor4 <= 0.447999f)
                                                    if (actual_omega <= 0.165289f)
                                                        return 3;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= 0.156394f)
                                                        return 7;
                                                    else
                                                        return 7;
                                            else
                                                if (sensor4 <= 0.743682f)
                                                    if (actual_omega <= 0.151500f)
                                                        return 10;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor1 <= 0.765735f)
                                                        return 6;
                                                    else
                                                        return 6;
                                    else
                                        if (actual_vz <= 1.042695f)
                                            if (sensor5 <= 0.375368f)
                                                if (actual_vz <= 0.997972f)
                                                    if (sensor0 <= 0.817694f)
                                                        return 5;
                                                    else
                                                        return 4;
                                                else
                                                    if (actual_vx <= 0.005588f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor0 <= 0.339364f)
                                                    if (actual_vz <= 0.981073f)
                                                        return 5;
                                                    else
                                                        return 4;
                                                else
                                                    if (sensor0 <= 0.573090f)
                                                        return 4;
                                                    else
                                                        return 0;
                                        else
                                            if (sensor0 <= 0.592042f)
                                                if (sensor2 <= 0.244130f)
                                                    if (sensor4 <= 0.847927f)
                                                        return 5;
                                                    else
                                                        return 4;
                                                else
                                                    if (actual_omega <= 0.037160f)
                                                        return 5;
                                                    else
                                                        return 1;
                                            else
                                                if (sensor2 <= 0.182157f)
                                                    if (actual_omega <= -0.209254f)
                                                        return 1;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_omega <= -0.184461f)
                                                        return 4;
                                                    else
                                                        return 1;
                                else
                                    if (sensor5 <= 0.600861f)
                                        if (actual_vx <= -0.004401f)
                                            if (actual_omega <= 0.185498f)
                                                if (sensor2 <= 0.775949f)
                                                    if (actual_omega <= 0.060558f)
                                                        return 9;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor2 <= 0.779038f)
                                                        return 10;
                                                    else
                                                        return 6;
                                            else
                                                if (actual_vx <= -0.008030f)
                                                    return 5;
                                                else
                                                    if (sensor5 <= 0.394430f)
                                                        return 10;
                                                    else
                                                        return 7;
                                        else
                                            if (sensor3 <= 0.325213f)
                                                if (actual_vz <= 1.051838f)
                                                    if (actual_vx <= 0.014963f)
                                                        return 5;
                                                    else
                                                        return 2;
                                                else
                                                    if (actual_vx <= -0.002395f)
                                                        return 9;
                                                    else
                                                        return 2;
                                            else
                                                if (sensor1 <= 0.780492f)
                                                    return 4;
                                                else
                                                    if (sensor1 <= 0.784716f)
                                                        return 5;
                                                    else
                                                        return 4;
                                    else
                                        if (actual_vx <= -0.004105f)
                                            if (last_action_state <= 0.950000f)
                                                if (sensor5 <= 0.768270f)
                                                    if (actual_vx <= -0.004286f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (last_action_state <= 0.850000f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor1 <= 0.651433f)
                                                    if (actual_omega <= 0.213816f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vz <= 0.973808f)
                                                        return 5;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor1 <= 0.518491f)
                                                if (last_action_state <= 0.950000f)
                                                    if (actual_omega <= 0.178456f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor3 <= 0.024275f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor0 <= 0.097304f)
                                                    if (sensor4 <= 0.634597f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= 0.161675f)
                                                        return 5;
                                                    else
                                                        return 5;
                            else
                                if (actual_vz <= 1.028890f)
                                    if (actual_vx <= 0.007245f)
                                        if (sensor3 <= 0.757462f)
                                            if (actual_omega <= -0.134129f)
                                                if (sensor4 <= 0.783135f)
                                                    if (sensor1 <= 0.723844f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= -0.173547f)
                                                        return 6;
                                                    else
                                                        return 5;
                                            else
                                                if (last_action_state <= 0.250000f)
                                                    if (sensor4 <= 0.778076f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor2 <= 0.158428f)
                                                        return 5;
                                                    else
                                                        return 4;
                                        else
                                            if (last_action_state <= 0.250000f)
                                                if (sensor4 <= 0.293871f)
                                                    if (actual_vx <= 0.003839f)
                                                        return 6;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_omega <= -0.119068f)
                                                        return 6;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vz <= 0.985976f)
                                                    if (actual_omega <= -0.133586f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    return 6;
                                    else
                                        if (sensor4 <= 0.476708f)
                                            if (sensor0 <= 0.832427f)
                                                if (sensor4 <= 0.446362f)
                                                    if (actual_omega <= -0.175648f)
                                                        return 6;
                                                    else
                                                        return 4;
                                                else
                                                    if (sensor5 <= 0.033775f)
                                                        return 5;
                                                    else
                                                        return 6;
                                            else
                                                if (actual_omega <= -0.208070f)
                                                    return 6;
                                                else
                                                    if (sensor0 <= 0.843347f)
                                                        return 5;
                                                    else
                                                        return 6;
                                        else
                                            if (sensor4 <= 0.586952f)
                                                if (last_action_state <= 0.050000f)
                                                    if (sensor0 <= 0.830576f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor0 <= 0.855516f)
                                                        return 5;
                                                    else
                                                        return 4;
                                            else
                                                if (last_action_state <= 0.350000f)
                                                    if (actual_vz <= 0.951846f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= -0.150900f)
                                                        return 4;
                                                    else
                                                        return 5;
                                else
                                    if (sensor0 <= 0.770953f)
                                        if (actual_omega <= -0.157188f)
                                            if (actual_vx <= 0.003288f)
                                                if (sensor2 <= 0.127115f)
                                                    return 5;
                                                else
                                                    if (sensor5 <= 0.253491f)
                                                        return 4;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vx <= 0.014900f)
                                                    if (actual_vx <= 0.010612f)
                                                        return 1;
                                                    else
                                                        return 3;
                                                else
                                                    if (sensor4 <= 0.737151f)
                                                        return 2;
                                                    else
                                                        return 5;
                                        else
                                            return 1;
                                    else
                                        if (actual_omega <= -0.233861f)
                                            if (sensor1 <= 0.556381f)
                                                if (sensor1 <= 0.503220f)
                                                    if (sensor0 <= 0.836800f)
                                                        return 1;
                                                    else
                                                        return 2;
                                                else
                                                    if (sensor3 <= 0.764641f)
                                                        return 2;
                                                    else
                                                        return 2;
                                            else
                                                if (sensor0 <= 0.817766f)
                                                    if (sensor2 <= 0.120555f)
                                                        return 2;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor0 <= 0.826882f)
                                                        return 1;
                                                    else
                                                        return 2;
                                        else
                                            if (sensor0 <= 0.849055f)
                                                if (last_action_state <= 0.300000f)
                                                    if (sensor3 <= 0.609130f)
                                                        return 5;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor2 <= 0.032222f)
                                                        return 0;
                                                    else
                                                        return 1;
                                            else
                                                return 2;
                    else
                        if (actual_vz <= 1.031266f)
                            if (actual_vx <= 0.007164f)
                                if (last_action_state <= 0.250000f)
                                    if (last_action_state <= 0.150000f)
                                        if (sensor1 <= 0.362792f)
                                            if (sensor4 <= 0.346842f)
                                                if (sensor2 <= 0.016352f)
                                                    if (sensor3 <= 0.830885f)
                                                        return 6;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor5 <= 0.014680f)
                                                        return 6;
                                                    else
                                                        return 6;
                                            else
                                                if (actual_omega <= -0.197817f)
                                                    return 6;
                                                else
                                                    if (sensor2 <= 0.022369f)
                                                        return 6;
                                                    else
                                                        return 5;
                                        else
                                            if (actual_vx <= 0.005692f)
                                                if (sensor1 <= 0.429098f)
                                                    if (sensor4 <= 0.367054f)
                                                        return 6;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor4 <= 0.388985f)
                                                        return 6;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor4 <= 0.382055f)
                                                    if (sensor3 <= 0.810882f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor0 <= 0.838494f)
                                                        return 6;
                                                    else
                                                        return 5;
                                    else
                                        if (actual_vx <= 0.004841f)
                                            if (sensor1 <= 0.369661f)
                                                if (actual_omega <= -0.178304f)
                                                    if (sensor4 <= 0.316900f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vz <= 0.951153f)
                                                        return 6;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor3 <= 0.861532f)
                                                    if (sensor2 <= 0.040000f)
                                                        return 6;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vx <= 0.004640f)
                                                        return 6;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor0 <= 0.843527f)
                                                if (actual_vz <= 0.955880f)
                                                    if (sensor1 <= 0.364960f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor4 <= 0.299360f)
                                                        return 6;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_omega <= -0.188137f)
                                                    return 6;
                                                else
                                                    if (actual_vx <= 0.007112f)
                                                        return 5;
                                                    else
                                                        return 6;
                                else
                                    if (last_action_state <= 0.350000f)
                                        if (sensor3 <= 0.877970f)
                                            if (sensor0 <= 0.650177f)
                                                if (sensor5 <= 0.127484f)
                                                    if (sensor1 <= 0.692397f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    return 5;
                                            else
                                                if (actual_vz <= 0.953867f)
                                                    if (actual_omega <= -0.115347f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor4 <= 0.245435f)
                                                        return 7;
                                                    else
                                                        return 5;
                                        else
                                            return 6;
                                    else
                                        if (sensor5 <= 0.159774f)
                                            if (sensor1 <= 0.468056f)
                                                if (sensor5 <= 0.015436f)
                                                    return 5;
                                                else
                                                    if (sensor4 <= 0.320206f)
                                                        return 2;
                                                    else
                                                        return 3;
                                            else
                                                if (sensor3 <= 0.798749f)
                                                    return 6;
                                                else
                                                    if (actual_omega <= -0.109570f)
                                                        return 5;
                                                    else
                                                        return 5;
                                        else
                                            if (actual_vx <= -0.000562f)
                                                return 4;
                                            else
                                                return 6;
                            else
                                if (sensor4 <= 0.431375f)
                                    if (last_action_state <= 0.150000f)
                                        if (sensor4 <= 0.403248f)
                                            if (sensor1 <= 0.441130f)
                                                if (sensor4 <= 0.315174f)
                                                    if (sensor1 <= 0.290323f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor1 <= 0.384489f)
                                                        return 5;
                                                    else
                                                        return 6;
                                            else
                                                if (actual_omega <= -0.181899f)
                                                    if (sensor4 <= 0.369541f)
                                                        return 6;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vz <= 0.961351f)
                                                        return 5;
                                                    else
                                                        return 6;
                                        else
                                            if (sensor1 <= 0.452679f)
                                                if (sensor2 <= 0.035640f)
                                                    if (sensor4 <= 0.404307f)
                                                        return 4;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vz <= 0.967860f)
                                                        return 6;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vx <= 0.007342f)
                                                    if (sensor1 <= 0.454796f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vx <= 0.010197f)
                                                        return 6;
                                                    else
                                                        return 6;
                                    else
                                        if (sensor0 <= 0.839390f)
                                            if (sensor5 <= 0.024725f)
                                                if (sensor5 <= 0.021373f)
                                                    if (actual_vx <= 0.008534f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    return 5;
                                            else
                                                if (actual_omega <= -0.186898f)
                                                    return 5;
                                                else
                                                    return 6;
                                        else
                                            if (sensor4 <= 0.356086f)
                                                if (actual_vz <= 0.964920f)
                                                    if (actual_vx <= 0.007602f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    return 4;
                                            else
                                                if (sensor1 <= 0.393665f)
                                                    if (sensor4 <= 0.379109f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    return 5;
                                else
                                    if (sensor0 <= 0.837005f)
                                        if (last_action_state <= 0.150000f)
                                            if (sensor0 <= 0.815876f)
                                                if (sensor3 <= 0.842539f)
                                                    if (actual_vz <= 0.961507f)
                                                        return 6;
                                                    else
                                                        return 6;
                                                else
                                                    return 5;
                                            else
                                                if (actual_omega <= -0.192343f)
                                                    if (sensor4 <= 0.476128f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vx <= 0.007533f)
                                                        return 6;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor3 <= 0.809934f)
                                                if (sensor5 <= 0.031353f)
                                                    return 6;
                                                else
                                                    return 5;
                                            else
                                                return 5;
                                    else
                                        if (sensor4 <= 0.455534f)
                                            if (actual_omega <= -0.189745f)
                                                if (actual_vx <= 0.009868f)
                                                    if (sensor3 <= 0.823807f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor0 <= 0.838445f)
                                                        return 6;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vz <= 0.954467f)
                                                    return 6;
                                                else
                                                    if (last_action_state <= 0.350000f)
                                                        return 5;
                                                    else
                                                        return 2;
                                        else
                                            if (last_action_state <= 0.050000f)
                                                if (actual_vz <= 0.986165f)
                                                    return 6;
                                                else
                                                    return 5;
                                            else
                                                if (sensor5 <= 0.032372f)
                                                    if (sensor1 <= 0.508004f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vx <= 0.007456f)
                                                        return 7;
                                                    else
                                                        return 5;
                        else
                            if (sensor0 <= 0.786662f)
                                if (sensor3 <= 0.830462f)
                                    if (sensor3 <= 0.820701f)
                                        if (sensor0 <= 0.669850f)
                                            return 4;
                                        else
                                            return 0;
                                    else
                                        if (actual_vx <= 0.004827f)
                                            return 5;
                                        else
                                            return 1;
                                else
                                    if (actual_vx <= 0.007122f)
                                        return 2;
                                    else
                                        if (sensor5 <= 0.052126f)
                                            if (sensor4 <= 0.415170f)
                                                return 2;
                                            else
                                                return 0;
                                        else
                                            return 1;
                            else
                                if (actual_vz <= 1.075370f)
                                    if (actual_vz <= 1.073600f)
                                        if (sensor4 <= 0.246045f)
                                            if (actual_vz <= 1.070816f)
                                                if (sensor0 <= 0.839863f)
                                                    return 2;
                                                else
                                                    if (actual_vz <= 1.069610f)
                                                        return 1;
                                                    else
                                                        return 2;
                                            else
                                                if (sensor2 <= 0.027107f)
                                                    return 0;
                                                else
                                                    if (actual_vz <= 1.071601f)
                                                        return 4;
                                                    else
                                                        return 1;
                                        else
                                            if (actual_vz <= 1.068227f)
                                                if (sensor2 <= 0.038861f)
                                                    if (actual_vz <= 1.067310f)
                                                        return 1;
                                                    else
                                                        return 0;
                                                else
                                                    return 2;
                                            else
                                                if (sensor0 <= 0.846630f)
                                                    if (sensor3 <= 0.840692f)
                                                        return 1;
                                                    else
                                                        return 0;
                                                else
                                                    return 2;
                                    else
                                        if (sensor5 <= 0.012095f)
                                            if (actual_vz <= 1.073781f)
                                                return 0;
                                            else
                                                if (sensor5 <= 0.011312f)
                                                    return 1;
                                                else
                                                    if (actual_omega <= -0.209548f)
                                                        return 2;
                                                    else
                                                        return 1;
                                        else
                                            if (sensor3 <= 0.843776f)
                                                if (sensor1 <= 0.379722f)
                                                    if (actual_omega <= -0.210806f)
                                                        return 0;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_vz <= 1.073768f)
                                                        return 0;
                                                    else
                                                        return 2;
                                            else
                                                return 1;
                                else
                                    if (sensor5 <= 0.013029f)
                                        if (sensor2 <= 0.038612f)
                                            if (actual_omega <= -0.217418f)
                                                return 2;
                                            else
                                                if (sensor5 <= 0.012574f)
                                                    if (sensor5 <= 0.008908f)
                                                        return 0;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_vz <= 1.076941f)
                                                        return 1;
                                                    else
                                                        return 0;
                                        else
                                            if (actual_vx <= 0.005338f)
                                                return 3;
                                            else
                                                return 0;
                                    else
                                        if (sensor1 <= 0.419698f)
                                            if (sensor0 <= 0.841323f)
                                                if (actual_vz <= 1.082108f)
                                                    if (actual_vx <= 0.009930f)
                                                        return 1;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_vx <= 0.006457f)
                                                        return 1;
                                                    else
                                                        return 2;
                                            else
                                                if (sensor0 <= 0.841501f)
                                                    if (actual_vz <= 1.078277f)
                                                        return 2;
                                                    else
                                                        return 0;
                                                else
                                                    if (actual_vx <= 0.004090f)
                                                        return 0;
                                                    else
                                                        return 1;
                                        else
                                            if (sensor1 <= 0.420481f)
                                                return 0;
                                            else
                                                if (sensor2 <= 0.025926f)
                                                    return 2;
                                                else
                                                    if (actual_vz <= 1.077147f)
                                                        return 1;
                                                    else
                                                        return 1;
            else
                if (sensor0 <= 0.562564f)
                    if (sensor0 <= 0.343628f)
                        if (last_action_state <= 0.750000f)
                            if (last_action_state <= 0.450000f)
                                if (sensor2 <= 0.280317f)
                                    if (actual_vx <= 0.005798f)
                                        if (actual_vx <= -0.005222f)
                                            return 1;
                                        else
                                            if (sensor1 <= 0.795610f)
                                                if (sensor4 <= 0.996639f)
                                                    if (actual_vz <= 1.119967f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    return 0;
                                            else
                                                return 1;
                                    else
                                        if (sensor2 <= 0.265581f)
                                            if (sensor4 <= 0.985572f)
                                                if (actual_omega <= -0.026699f)
                                                    if (actual_vz <= 1.099732f)
                                                        return 4;
                                                    else
                                                        return 3;
                                                else
                                                    if (sensor0 <= 0.310113f)
                                                        return 4;
                                                    else
                                                        return 1;
                                            else
                                                if (sensor0 <= 0.328670f)
                                                    return 3;
                                                else
                                                    return 8;
                                        else
                                            if (actual_vx <= 0.010126f)
                                                return 5;
                                            else
                                                return 8;
                                else
                                    if (sensor1 <= 0.293720f)
                                        if (sensor0 <= 0.015173f)
                                            return 9;
                                        else
                                            return 8;
                                    else
                                        if (actual_omega <= 0.179935f)
                                            if (sensor1 <= 0.444111f)
                                                if (sensor2 <= 0.818738f)
                                                    if (sensor3 <= 0.023548f)
                                                        return 3;
                                                    else
                                                        return 7;
                                                else
                                                    if (actual_omega <= 0.102587f)
                                                        return 8;
                                                    else
                                                        return 10;
                                            else
                                                if (actual_vz <= 1.092424f)
                                                    if (actual_omega <= 0.139864f)
                                                        return 10;
                                                    else
                                                        return 10;
                                                else
                                                    if (last_action_state <= 0.350000f)
                                                        return 9;
                                                    else
                                                        return 10;
                                        else
                                            if (last_action_state <= 0.350000f)
                                                if (sensor5 <= 0.849222f)
                                                    if (sensor2 <= 0.853539f)
                                                        return 10;
                                                    else
                                                        return 8;
                                                else
                                                    if (actual_vx <= -0.006368f)
                                                        return 7;
                                                    else
                                                        return 9;
                                            else
                                                if (sensor3 <= 0.029733f)
                                                    return 7;
                                                else
                                                    if (sensor4 <= 0.462959f)
                                                        return 9;
                                                    else
                                                        return 10;
                            else
                                if (sensor5 <= 0.346598f)
                                    if (actual_vx <= -0.010904f)
                                        if (sensor4 <= 0.826707f)
                                            return 1;
                                        else
                                            return 6;
                                    else
                                        if (sensor5 <= 0.336004f)
                                            if (sensor1 <= 0.772297f)
                                                return 1;
                                            else
                                                if (actual_vx <= 0.000682f)
                                                    if (sensor3 <= 0.269093f)
                                                        return 0;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vz <= 1.100182f)
                                                        return 5;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor0 <= 0.306862f)
                                                if (actual_omega <= -0.013312f)
                                                    if (sensor1 <= 0.787860f)
                                                        return 4;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor4 <= 0.788622f)
                                                        return 4;
                                                    else
                                                        return 5;
                                            else
                                                return 1;
                                else
                                    if (sensor4 <= 0.516695f)
                                        if (last_action_state <= 0.550000f)
                                            if (actual_vx <= -0.004972f)
                                                if (actual_vx <= -0.005226f)
                                                    if (sensor0 <= 0.043974f)
                                                        return 3;
                                                    else
                                                        return 7;
                                                else
                                                    if (sensor5 <= 0.833603f)
                                                        return 9;
                                                    else
                                                        return 7;
                                            else
                                                if (sensor3 <= 0.030827f)
                                                    if (sensor3 <= 0.028822f)
                                                        return 10;
                                                    else
                                                        return 8;
                                                else
                                                    if (sensor5 <= 0.819809f)
                                                        return 9;
                                                    else
                                                        return 10;
                                        else
                                            if (sensor4 <= 0.375458f)
                                                if (actual_omega <= 0.127504f)
                                                    if (sensor2 <= 0.837489f)
                                                        return 3;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vx <= -0.004231f)
                                                        return 5;
                                                    else
                                                        return 3;
                                            else
                                                if (sensor0 <= 0.021961f)
                                                    if (actual_vx <= -0.001430f)
                                                        return 4;
                                                    else
                                                        return 8;
                                                else
                                                    if (actual_omega <= 0.132030f)
                                                        return 7;
                                                    else
                                                        return 7;
                                    else
                                        if (actual_vx <= -0.001140f)
                                            if (sensor0 <= 0.153065f)
                                                if (actual_omega <= 0.121273f)
                                                    if (actual_vx <= -0.009100f)
                                                        return 7;
                                                    else
                                                        return 10;
                                                else
                                                    if (last_action_state <= 0.550000f)
                                                        return 10;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor2 <= 0.743861f)
                                                    if (sensor5 <= 0.718055f)
                                                        return 6;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vx <= -0.009759f)
                                                        return 6;
                                                    else
                                                        return 10;
                                        else
                                            if (sensor2 <= 0.272059f)
                                                if (sensor1 <= 0.775997f)
                                                    if (actual_omega <= -0.018304f)
                                                        return 8;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor4 <= 0.988676f)
                                                        return 4;
                                                    else
                                                        return 10;
                                            else
                                                if (actual_vz <= 1.082761f)
                                                    if (last_action_state <= 0.650000f)
                                                        return 0;
                                                    else
                                                        return 3;
                                                else
                                                    if (sensor2 <= 0.709192f)
                                                        return 10;
                                                    else
                                                        return 4;
                        else
                            if (sensor5 <= 0.627182f)
                                if (sensor5 <= 0.570228f)
                                    if (sensor2 <= 0.678179f)
                                        if (actual_vx <= 0.005381f)
                                            if (sensor5 <= 0.475526f)
                                                return 5;
                                            else
                                                return 6;
                                        else
                                            return 4;
                                    else
                                        if (actual_vz <= 1.095433f)
                                            if (sensor4 <= 0.795135f)
                                                return 6;
                                            else
                                                if (sensor3 <= 0.153544f)
                                                    return 7;
                                                else
                                                    if (sensor4 <= 0.834796f)
                                                        return 6;
                                                    else
                                                        return 6;
                                        else
                                            if (sensor1 <= 0.682891f)
                                                return 6;
                                            else
                                                if (sensor5 <= 0.517400f)
                                                    if (sensor4 <= 0.833250f)
                                                        return 7;
                                                    else
                                                        return 6;
                                                else
                                                    return 5;
                                else
                                    if (sensor5 <= 0.607580f)
                                        if (sensor2 <= 0.796697f)
                                            if (actual_vz <= 1.086231f)
                                                return 5;
                                            else
                                                return 7;
                                        else
                                            if (sensor2 <= 0.827321f)
                                                if (actual_vz <= 1.083875f)
                                                    return 4;
                                                else
                                                    return 5;
                                            else
                                                return 6;
                                    else
                                        if (sensor3 <= 0.103060f)
                                            if (sensor3 <= 0.098788f)
                                                return 6;
                                            else
                                                if (sensor4 <= 0.721681f)
                                                    return 7;
                                                else
                                                    return 4;
                                        else
                                            return 6;
                            else
                                if (actual_omega <= 0.147509f)
                                    if (sensor3 <= 0.032388f)
                                        if (sensor0 <= 0.027808f)
                                            if (sensor4 <= 0.372408f)
                                                if (actual_vx <= -0.000171f)
                                                    if (actual_vx <= -0.002993f)
                                                        return 5;
                                                    else
                                                        return 4;
                                                else
                                                    if (sensor0 <= 0.024541f)
                                                        return 10;
                                                    else
                                                        return 0;
                                            else
                                                if (sensor1 <= 0.283719f)
                                                    return 4;
                                                else
                                                    if (sensor2 <= 0.832846f)
                                                        return 5;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor5 <= 0.827247f)
                                                return 1;
                                            else
                                                if (sensor5 <= 0.849833f)
                                                    if (actual_vz <= 1.087083f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vz <= 1.091426f)
                                                        return 5;
                                                    else
                                                        return 1;
                                    else
                                        if (sensor2 <= 0.832711f)
                                            if (sensor0 <= 0.051002f)
                                                if (actual_omega <= 0.129110f)
                                                    if (sensor2 <= 0.765445f)
                                                        return 4;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= 0.138520f)
                                                        return 4;
                                                    else
                                                        return 3;
                                            else
                                                if (sensor5 <= 0.815086f)
                                                    if (actual_omega <= 0.131512f)
                                                        return 8;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor5 <= 0.827390f)
                                                        return 0;
                                                    else
                                                        return 1;
                                        else
                                            if (actual_vx <= -0.001025f)
                                                if (last_action_state <= 0.950000f)
                                                    if (sensor5 <= 0.678958f)
                                                        return 1;
                                                    else
                                                        return 7;
                                                else
                                                    if (sensor1 <= 0.373092f)
                                                        return 1;
                                                    else
                                                        return 3;
                                            else
                                                if (sensor0 <= 0.017747f)
                                                    if (sensor4 <= 0.464024f)
                                                        return 4;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor5 <= 0.847472f)
                                                        return 0;
                                                    else
                                                        return 3;
                                else
                                    if (sensor3 <= 0.068883f)
                                        if (sensor4 <= 0.626952f)
                                            if (sensor4 <= 0.480360f)
                                                if (sensor3 <= 0.020861f)
                                                    if (sensor4 <= 0.352924f)
                                                        return 5;
                                                    else
                                                        return 4;
                                                else
                                                    if (actual_vz <= 1.082622f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor4 <= 0.555059f)
                                                    if (sensor0 <= 0.020478f)
                                                        return 3;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor2 <= 0.854168f)
                                                        return 5;
                                                    else
                                                        return 3;
                                        else
                                            return 4;
                                    else
                                        if (sensor0 <= 0.057426f)
                                            if (sensor2 <= 0.765351f)
                                                if (sensor5 <= 0.834248f)
                                                    if (actual_vz <= 1.095433f)
                                                        return 0;
                                                    else
                                                        return 3;
                                                else
                                                    if (actual_vz <= 1.085185f)
                                                        return 1;
                                                    else
                                                        return 4;
                                            else
                                                if (sensor3 <= 0.087478f)
                                                    if (sensor0 <= 0.021361f)
                                                        return 3;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor5 <= 0.800661f)
                                                        return 5;
                                                    else
                                                        return 1;
                                        else
                                            if (sensor1 <= 0.496213f)
                                                return 6;
                                            else
                                                if (sensor2 <= 0.506417f)
                                                    if (sensor4 <= 0.807976f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor0 <= 0.138428f)
                                                        return 5;
                                                    else
                                                        return 6;
                    else
                        if (actual_omega <= -0.109020f)
                            if (actual_omega <= -0.158707f)
                                if (sensor5 <= 0.294028f)
                                    if (sensor2 <= 0.125728f)
                                        if (sensor4 <= 0.751105f)
                                            if (actual_vx <= 0.003407f)
                                                return 2;
                                            else
                                                if (actual_vx <= 0.004686f)
                                                    return 1;
                                                else
                                                    return 5;
                                        else
                                            if (sensor4 <= 0.789219f)
                                                if (actual_vx <= 0.004037f)
                                                    if (actual_omega <= -0.177377f)
                                                        return 2;
                                                    else
                                                        return 4;
                                                else
                                                    return 1;
                                            else
                                                if (actual_omega <= -0.164617f)
                                                    if (sensor1 <= 0.754643f)
                                                        return 5;
                                                    else
                                                        return 1;
                                                else
                                                    return 4;
                                    else
                                        if (sensor2 <= 0.138962f)
                                            if (sensor3 <= 0.663905f)
                                                if (actual_omega <= -0.182220f)
                                                    return 2;
                                                else
                                                    if (sensor0 <= 0.540826f)
                                                        return 1;
                                                    else
                                                        return 1;
                                            else
                                                if (sensor5 <= 0.246241f)
                                                    if (sensor1 <= 0.760791f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor5 <= 0.248317f)
                                                        return 1;
                                                    else
                                                        return 0;
                                        else
                                            if (sensor0 <= 0.527885f)
                                                if (actual_omega <= -0.168186f)
                                                    if (actual_vx <= 0.002666f)
                                                        return 5;
                                                    else
                                                        return 4;
                                                else
                                                    if (actual_vz <= 1.099791f)
                                                        return 5;
                                                    else
                                                        return 1;
                                            else
                                                return 4;
                                else
                                    if (sensor4 <= 0.949579f)
                                        return 4;
                                    else
                                        if (actual_omega <= -0.182577f)
                                            return 2;
                                        else
                                            return 5;
                            else
                                if (actual_vx <= 0.000900f)
                                    if (sensor0 <= 0.492852f)
                                        if (actual_vz <= 1.109035f)
                                            if (actual_vx <= 0.000319f)
                                                if (sensor5 <= 0.309553f)
                                                    if (actual_omega <= -0.156639f)
                                                        return 1;
                                                    else
                                                        return 5;
                                                else
                                                    return 1;
                                            else
                                                if (sensor4 <= 0.849790f)
                                                    return 4;
                                                else
                                                    if (sensor2 <= 0.160785f)
                                                        return 1;
                                                    else
                                                        return 2;
                                        else
                                            if (sensor0 <= 0.489429f)
                                                if (sensor5 <= 0.272069f)
                                                    return 1;
                                                else
                                                    return 4;
                                            else
                                                return 2;
                                    else
                                        if (sensor4 <= 0.850947f)
                                            if (actual_vz <= 1.094410f)
                                                if (actual_vz <= 1.086169f)
                                                    if (sensor2 <= 0.133521f)
                                                        return 1;
                                                    else
                                                        return 4;
                                                else
                                                    if (sensor5 <= 0.189677f)
                                                        return 1;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor1 <= 0.776031f)
                                                    if (sensor5 <= 0.233194f)
                                                        return 1;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_vx <= -0.000187f)
                                                        return 1;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor0 <= 0.521039f)
                                                if (sensor0 <= 0.496412f)
                                                    return 1;
                                                else
                                                    if (actual_omega <= -0.151840f)
                                                        return 4;
                                                    else
                                                        return 1;
                                            else
                                                if (sensor5 <= 0.236867f)
                                                    return 5;
                                                else
                                                    if (last_action_state <= 0.450000f)
                                                        return 2;
                                                    else
                                                        return 1;
                                else
                                    if (last_action_state <= 0.550000f)
                                        if (sensor4 <= 0.746192f)
                                            if (sensor4 <= 0.720970f)
                                                return 3;
                                            else
                                                return 5;
                                        else
                                            if (sensor3 <= 0.727863f)
                                                if (sensor3 <= 0.577653f)
                                                    if (sensor4 <= 0.870479f)
                                                        return 4;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor2 <= 0.156251f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor5 <= 0.199474f)
                                                    if (actual_vx <= 0.003384f)
                                                        return 1;
                                                    else
                                                        return 4;
                                                else
                                                    return 4;
                                    else
                                        if (actual_omega <= -0.133449f)
                                            if (sensor4 <= 0.828371f)
                                                if (sensor3 <= 0.669818f)
                                                    return 2;
                                                else
                                                    if (actual_vx <= 0.002889f)
                                                        return 5;
                                                    else
                                                        return 1;
                                            else
                                                return 1;
                                        else
                                            if (sensor2 <= 0.124381f)
                                                if (sensor0 <= 0.559121f)
                                                    return 3;
                                                else
                                                    return 2;
                                            else
                                                return 1;
                        else
                            if (actual_vx <= -0.000494f)
                                if (sensor4 <= 0.820237f)
                                    if (sensor0 <= 0.535854f)
                                        if (sensor1 <= 0.771208f)
                                            if (sensor0 <= 0.534353f)
                                                if (actual_vx <= -0.001331f)
                                                    return 5;
                                                else
                                                    if (actual_omega <= -0.086640f)
                                                        return 3;
                                                    else
                                                        return 1;
                                            else
                                                if (actual_vx <= -0.001710f)
                                                    return 4;
                                                else
                                                    return 2;
                                        else
                                            if (actual_vz <= 1.112215f)
                                                if (sensor4 <= 0.818999f)
                                                    if (sensor5 <= 0.243498f)
                                                        return 4;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor4 <= 0.819178f)
                                                        return 5;
                                                    else
                                                        return 0;
                                            else
                                                return 1;
                                    else
                                        if (last_action_state <= 0.450000f)
                                            if (actual_vz <= 1.105262f)
                                                return 5;
                                            else
                                                if (sensor3 <= 0.694518f)
                                                    return 0;
                                                else
                                                    return 1;
                                        else
                                            if (sensor1 <= 0.750084f)
                                                if (sensor0 <= 0.560168f)
                                                    return 2;
                                                else
                                                    return 5;
                                            else
                                                if (actual_vz <= 1.100427f)
                                                    if (actual_omega <= -0.066761f)
                                                        return 1;
                                                    else
                                                        return 2;
                                                else
                                                    if (sensor1 <= 0.768847f)
                                                        return 1;
                                                    else
                                                        return 1;
                                else
                                    if (sensor1 <= 0.781490f)
                                        if (sensor3 <= 0.704589f)
                                            if (sensor3 <= 0.675545f)
                                                if (actual_vz <= 1.106030f)
                                                    if (actual_vz <= 1.105913f)
                                                        return 1;
                                                    else
                                                        return 0;
                                                else
                                                    return 1;
                                            else
                                                if (sensor3 <= 0.691781f)
                                                    return 5;
                                                else
                                                    return 1;
                                        else
                                            return 2;
                                    else
                                        if (sensor1 <= 0.782299f)
                                            if (sensor5 <= 0.279836f)
                                                return 4;
                                            else
                                                if (sensor2 <= 0.145949f)
                                                    return 2;
                                                else
                                                    return 1;
                                        else
                                            if (sensor5 <= 0.288863f)
                                                if (sensor1 <= 0.795809f)
                                                    if (sensor0 <= 0.509212f)
                                                        return 1;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_vz <= 1.111054f)
                                                        return 1;
                                                    else
                                                        return 0;
                                            else
                                                if (actual_vz <= 1.094777f)
                                                    if (actual_vz <= 1.088489f)
                                                        return 1;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor1 <= 0.784920f)
                                                        return 0;
                                                    else
                                                        return 1;
                            else
                                if (sensor2 <= 0.142471f)
                                    if (actual_vx <= 0.001227f)
                                        if (actual_vx <= -0.000427f)
                                            if (sensor2 <= 0.132046f)
                                                return 2;
                                            else
                                                return 3;
                                        else
                                            if (sensor2 <= 0.127260f)
                                                if (actual_vx <= 0.000134f)
                                                    return 1;
                                                else
                                                    if (sensor3 <= 0.682262f)
                                                        return 0;
                                                    else
                                                        return 2;
                                            else
                                                if (actual_vz <= 1.120584f)
                                                    if (actual_vz <= 1.108931f)
                                                        return 1;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor2 <= 0.136705f)
                                                        return 4;
                                                    else
                                                        return 1;
                                    else
                                        if (sensor5 <= 0.260414f)
                                            if (actual_omega <= -0.099990f)
                                                if (sensor1 <= 0.769956f)
                                                    return 4;
                                                else
                                                    return 5;
                                            else
                                                if (actual_vz <= 1.115004f)
                                                    if (sensor1 <= 0.774113f)
                                                        return 5;
                                                    else
                                                        return 1;
                                                else
                                                    return 1;
                                        else
                                            return 2;
                                else
                                    if (sensor0 <= 0.355552f)
                                        if (actual_omega <= 0.009547f)
                                            if (sensor1 <= 0.797617f)
                                                return 5;
                                            else
                                                return 1;
                                        else
                                            return 0;
                                    else
                                        if (actual_vz <= 1.096047f)
                                            if (sensor2 <= 0.208677f)
                                                return 4;
                                            else
                                                return 5;
                                        else
                                            if (actual_omega <= -0.084450f)
                                                if (sensor3 <= 0.678568f)
                                                    if (actual_omega <= -0.088400f)
                                                        return 1;
                                                    else
                                                        return 4;
                                                else
                                                    return 4;
                                            else
                                                if (sensor0 <= 0.512294f)
                                                    if (actual_vx <= -0.000067f)
                                                        return 0;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor0 <= 0.518244f)
                                                        return 5;
                                                    else
                                                        return 1;
                else
                    if (sensor0 <= 0.704733f)
                        if (actual_omega <= -0.150311f)
                            if (sensor0 <= 0.663576f)
                                if (last_action_state <= 0.550000f)
                                    if (sensor2 <= 0.088837f)
                                        if (sensor3 <= 0.782081f)
                                            if (actual_omega <= -0.168597f)
                                                if (actual_vx <= 0.003290f)
                                                    if (actual_omega <= -0.180009f)
                                                        return 1;
                                                    else
                                                        return 2;
                                                else
                                                    return 4;
                                            else
                                                return 1;
                                        else
                                            if (actual_vz <= 1.104945f)
                                                if (actual_vx <= 0.006052f)
                                                    if (last_action_state <= 0.450000f)
                                                        return 5;
                                                    else
                                                        return 1;
                                                else
                                                    return 4;
                                            else
                                                if (sensor0 <= 0.641580f)
                                                    return 1;
                                                else
                                                    if (sensor2 <= 0.087213f)
                                                        return 2;
                                                    else
                                                        return 2;
                                    else
                                        if (sensor3 <= 0.789816f)
                                            if (actual_omega <= -0.187548f)
                                                return 4;
                                            else
                                                if (actual_vx <= 0.004425f)
                                                    if (sensor0 <= 0.598740f)
                                                        return 1;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_vx <= 0.004482f)
                                                        return 4;
                                                    else
                                                        return 5;
                                        else
                                            if (actual_vz <= 1.099312f)
                                                return 5;
                                            else
                                                if (actual_vz <= 1.101171f)
                                                    if (actual_vz <= 1.099650f)
                                                        return 4;
                                                    else
                                                        return 2;
                                                else
                                                    if (actual_vx <= 0.002303f)
                                                        return 2;
                                                    else
                                                        return 5;
                                else
                                    if (actual_vx <= 0.002973f)
                                        if (actual_omega <= -0.174604f)
                                            if (actual_omega <= -0.176252f)
                                                if (sensor3 <= 0.766073f)
                                                    return 2;
                                                else
                                                    if (actual_vx <= 0.002615f)
                                                        return 1;
                                                    else
                                                        return 1;
                                            else
                                                return 5;
                                        else
                                            if (actual_vz <= 1.109057f)
                                                if (sensor2 <= 0.118400f)
                                                    if (actual_vz <= 1.108094f)
                                                        return 1;
                                                    else
                                                        return 5;
                                                else
                                                    return 1;
                                            else
                                                if (actual_vx <= 0.002947f)
                                                    if (actual_omega <= -0.158617f)
                                                        return 1;
                                                    else
                                                        return 1;
                                                else
                                                    return 0;
                                    else
                                        if (sensor3 <= 0.801416f)
                                            if (sensor0 <= 0.606900f)
                                                if (sensor3 <= 0.748453f)
                                                    if (sensor0 <= 0.568237f)
                                                        return 0;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_vz <= 1.097878f)
                                                        return 5;
                                                    else
                                                        return 2;
                                            else
                                                if (actual_omega <= -0.177600f)
                                                    if (actual_vx <= 0.005680f)
                                                        return 2;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor3 <= 0.793866f)
                                                        return 1;
                                                    else
                                                        return 1;
                                        else
                                            if (sensor0 <= 0.637190f)
                                                if (sensor4 <= 0.620386f)
                                                    if (actual_vz <= 1.108894f)
                                                        return 5;
                                                    else
                                                        return 2;
                                                else
                                                    if (sensor3 <= 0.803281f)
                                                        return 5;
                                                    else
                                                        return 2;
                                            else
                                                if (sensor2 <= 0.080264f)
                                                    if (sensor1 <= 0.663153f)
                                                        return 5;
                                                    else
                                                        return 2;
                                                else
                                                    if (last_action_state <= 0.800000f)
                                                        return 1;
                                                    else
                                                        return 0;
                            else
                                if (sensor4 <= 0.561341f)
                                    if (actual_vz <= 1.100098f)
                                        if (sensor2 <= 0.076151f)
                                            if (sensor2 <= 0.066141f)
                                                return 2;
                                            else
                                                if (actual_omega <= -0.168218f)
                                                    if (actual_vz <= 1.098820f)
                                                        return 1;
                                                    else
                                                        return 2;
                                                else
                                                    if (actual_vz <= 1.095547f)
                                                        return 2;
                                                    else
                                                        return 1;
                                        else
                                            if (actual_vz <= 1.088383f)
                                                return 1;
                                            else
                                                return 5;
                                    else
                                        if (last_action_state <= 0.550000f)
                                            if (sensor4 <= 0.534850f)
                                                return 4;
                                            else
                                                if (actual_omega <= -0.179467f)
                                                    if (actual_omega <= -0.180601f)
                                                        return 0;
                                                    else
                                                        return 4;
                                                else
                                                    if (actual_vz <= 1.106487f)
                                                        return 2;
                                                    else
                                                        return 1;
                                        else
                                            if (sensor0 <= 0.693231f)
                                                if (sensor0 <= 0.691667f)
                                                    if (sensor1 <= 0.660450f)
                                                        return 1;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor3 <= 0.838390f)
                                                        return 2;
                                                    else
                                                        return 1;
                                            else
                                                if (actual_vx <= 0.005109f)
                                                    if (sensor5 <= 0.088503f)
                                                        return 1;
                                                    else
                                                        return 0;
                                                else
                                                    if (sensor0 <= 0.697372f)
                                                        return 1;
                                                    else
                                                        return 2;
                                else
                                    if (sensor1 <= 0.655946f)
                                        if (actual_vx <= 0.007094f)
                                            if (sensor4 <= 0.652909f)
                                                if (last_action_state <= 0.450000f)
                                                    if (sensor4 <= 0.599301f)
                                                        return 3;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor2 <= 0.076328f)
                                                        return 1;
                                                    else
                                                        return 0;
                                            else
                                                if (actual_omega <= -0.166619f)
                                                    return 2;
                                                else
                                                    return 1;
                                        else
                                            if (sensor4 <= 0.597957f)
                                                return 2;
                                            else
                                                if (sensor0 <= 0.693463f)
                                                    return 4;
                                                else
                                                    return 3;
                                    else
                                        if (sensor4 <= 0.593626f)
                                            if (actual_vx <= 0.005730f)
                                                if (actual_vz <= 1.098202f)
                                                    if (sensor5 <= 0.101914f)
                                                        return 5;
                                                    else
                                                        return 3;
                                                else
                                                    if (actual_vx <= 0.004555f)
                                                        return 1;
                                                    else
                                                        return 2;
                                            else
                                                return 0;
                                        else
                                            if (actual_vx <= 0.014091f)
                                                if (actual_vz <= 1.104564f)
                                                    if (actual_vz <= 1.099633f)
                                                        return 1;
                                                    else
                                                        return 1;
                                                else
                                                    return 1;
                                            else
                                                if (sensor4 <= 0.825393f)
                                                    if (sensor0 <= 0.687313f)
                                                        return 1;
                                                    else
                                                        return 4;
                                                else
                                                    return 3;
                        else
                            if (actual_vx <= -0.001050f)
                                if (last_action_state <= 0.450000f)
                                    if (actual_vz <= 1.108990f)
                                        if (actual_omega <= -0.139034f)
                                            return 5;
                                        else
                                            return 1;
                                    else
                                        if (actual_vz <= 1.112903f)
                                            return 5;
                                        else
                                            return 1;
                                else
                                    if (sensor0 <= 0.697818f)
                                        if (actual_vz <= 1.096022f)
                                            if (actual_vz <= 1.095472f)
                                                if (actual_vx <= -0.001300f)
                                                    if (sensor0 <= 0.569105f)
                                                        return 5;
                                                    else
                                                        return 1;
                                                else
                                                    return 2;
                                            else
                                                if (sensor1 <= 0.680972f)
                                                    return 2;
                                                else
                                                    return 0;
                                        else
                                            if (sensor2 <= 0.096752f)
                                                if (actual_vz <= 1.114865f)
                                                    if (actual_omega <= -0.058715f)
                                                        return 1;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor3 <= 0.817541f)
                                                        return 1;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor4 <= 0.669219f)
                                                    if (actual_omega <= -0.109785f)
                                                        return 5;
                                                    else
                                                        return 0;
                                                else
                                                    if (sensor2 <= 0.096880f)
                                                        return 0;
                                                    else
                                                        return 1;
                                    else
                                        if (last_action_state <= 0.550000f)
                                            if (actual_vz <= 1.112953f)
                                                return 2;
                                            else
                                                return 1;
                                        else
                                            return 4;
                            else
                                if (sensor3 <= 0.746225f)
                                    if (sensor0 <= 0.573198f)
                                        if (sensor4 <= 0.806542f)
                                            if (sensor0 <= 0.564982f)
                                                if (last_action_state <= 0.450000f)
                                                    return 4;
                                                else
                                                    if (sensor4 <= 0.791884f)
                                                        return 1;
                                                    else
                                                        return 1;
                                            else
                                                if (sensor4 <= 0.782542f)
                                                    if (sensor3 <= 0.702352f)
                                                        return 3;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_vx <= 0.001793f)
                                                        return 1;
                                                    else
                                                        return 4;
                                        else
                                            return 5;
                                    else
                                        if (actual_vx <= 0.011284f)
                                            if (sensor2 <= 0.214182f)
                                                if (sensor4 <= 0.735178f)
                                                    if (actual_vx <= -0.000568f)
                                                        return 4;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor2 <= 0.098914f)
                                                        return 4;
                                                    else
                                                        return 1;
                                            else
                                                if (sensor2 <= 0.217744f)
                                                    return 5;
                                                else
                                                    return 1;
                                        else
                                            if (sensor4 <= 0.836593f)
                                                if (sensor4 <= 0.807973f)
                                                    return 4;
                                                else
                                                    if (sensor1 <= 0.796883f)
                                                        return 1;
                                                    else
                                                        return 4;
                                            else
                                                return 4;
                                else
                                    if (sensor2 <= 0.095847f)
                                        if (last_action_state <= 0.450000f)
                                            if (actual_vz <= 1.107892f)
                                                if (sensor5 <= 0.136613f)
                                                    return 5;
                                                else
                                                    return 3;
                                            else
                                                if (sensor3 <= 0.782140f)
                                                    return 2;
                                                else
                                                    if (actual_vx <= 0.000529f)
                                                        return 5;
                                                    else
                                                        return 1;
                                        else
                                            if (actual_vx <= -0.001031f)
                                                return 4;
                                            else
                                                if (actual_vx <= 0.005354f)
                                                    if (sensor3 <= 0.805726f)
                                                        return 1;
                                                    else
                                                        return 1;
                                                else
                                                    return 4;
                                    else
                                        if (actual_omega <= -0.125805f)
                                            if (actual_vz <= 1.108332f)
                                                if (actual_omega <= -0.127260f)
                                                    if (actual_vx <= 0.001885f)
                                                        return 1;
                                                    else
                                                        return 4;
                                                else
                                                    if (actual_omega <= -0.126072f)
                                                        return 4;
                                                    else
                                                        return 2;
                                            else
                                                if (sensor3 <= 0.811665f)
                                                    if (actual_omega <= -0.128716f)
                                                        return 1;
                                                    else
                                                        return 5;
                                                else
                                                    return 2;
                                        else
                                            if (sensor3 <= 0.809856f)
                                                if (sensor2 <= 0.108856f)
                                                    if (actual_vz <= 1.098614f)
                                                        return 1;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor0 <= 0.577594f)
                                                        return 1;
                                                    else
                                                        return 1;
                                            else
                                                if (sensor5 <= 0.147472f)
                                                    if (sensor3 <= 0.811265f)
                                                        return 4;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vx <= 0.001458f)
                                                        return 2;
                                                    else
                                                        return 1;
                    else
                        if (sensor0 <= 0.780404f)
                            if (sensor4 <= 0.475204f)
                                if (sensor0 <= 0.732946f)
                                    if (actual_vz <= 1.110191f)
                                        if (actual_vx <= 0.003885f)
                                            if (sensor4 <= 0.421017f)
                                                if (actual_vz <= 1.105782f)
                                                    return 2;
                                                else
                                                    return 4;
                                            else
                                                if (sensor1 <= 0.604699f)
                                                    if (sensor4 <= 0.472675f)
                                                        return 1;
                                                    else
                                                        return 0;
                                                else
                                                    if (actual_omega <= -0.140176f)
                                                        return 2;
                                                    else
                                                        return 1;
                                        else
                                            if (sensor4 <= 0.462013f)
                                                if (sensor5 <= 0.042046f)
                                                    if (actual_omega <= -0.171976f)
                                                        return 4;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor5 <= 0.054600f)
                                                        return 2;
                                                    else
                                                        return 3;
                                            else
                                                if (sensor5 <= 0.059702f)
                                                    if (last_action_state <= 0.550000f)
                                                        return 4;
                                                    else
                                                        return 1;
                                                else
                                                    return 2;
                                    else
                                        return 1;
                                else
                                    if (sensor4 <= 0.408946f)
                                        if (sensor3 <= 0.854245f)
                                            if (actual_omega <= -0.192139f)
                                                if (sensor3 <= 0.837889f)
                                                    if (actual_vz <= 1.095897f)
                                                        return 0;
                                                    else
                                                        return 1;
                                                else
                                                    if (last_action_state <= 0.750000f)
                                                        return 2;
                                                    else
                                                        return 0;
                                            else
                                                if (sensor3 <= 0.835689f)
                                                    if (sensor3 <= 0.831807f)
                                                        return 1;
                                                    else
                                                        return 2;
                                                else
                                                    if (sensor1 <= 0.521033f)
                                                        return 2;
                                                    else
                                                        return 1;
                                        else
                                            if (sensor3 <= 0.876918f)
                                                return 2;
                                            else
                                                if (actual_vz <= 1.106816f)
                                                    if (actual_omega <= -0.172889f)
                                                        return 2;
                                                    else
                                                        return 1;
                                                else
                                                    return 3;
                                    else
                                        if (actual_vz <= 1.088104f)
                                            if (actual_omega <= -0.194974f)
                                                if (sensor2 <= 0.049875f)
                                                    return 0;
                                                else
                                                    return 3;
                                            else
                                                if (sensor1 <= 0.527597f)
                                                    return 1;
                                                else
                                                    if (actual_vx <= 0.004471f)
                                                        return 2;
                                                    else
                                                        return 2;
                                        else
                                            if (sensor1 <= 0.548896f)
                                                if (last_action_state <= 0.550000f)
                                                    if (sensor0 <= 0.775424f)
                                                        return 1;
                                                    else
                                                        return 2;
                                                else
                                                    if (actual_omega <= -0.205570f)
                                                        return 2;
                                                    else
                                                        return 1;
                                            else
                                                if (actual_omega <= -0.162997f)
                                                    if (actual_vz <= 1.091288f)
                                                        return 2;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_vz <= 1.101506f)
                                                        return 1;
                                                    else
                                                        return 1;
                            else
                                if (actual_omega <= -0.177081f)
                                    if (sensor3 <= 0.844776f)
                                        if (last_action_state <= 0.450000f)
                                            if (actual_omega <= -0.181614f)
                                                if (sensor0 <= 0.769350f)
                                                    if (actual_vx <= 0.016041f)
                                                        return 1;
                                                    else
                                                        return 2;
                                                else
                                                    if (actual_omega <= -0.186712f)
                                                        return 4;
                                                    else
                                                        return 2;
                                            else
                                                return 1;
                                        else
                                            if (sensor2 <= 0.189112f)
                                                if (actual_vx <= 0.017537f)
                                                    if (sensor0 <= 0.717074f)
                                                        return 1;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_vz <= 1.086498f)
                                                        return 1;
                                                    else
                                                        return 2;
                                            else
                                                if (actual_vx <= 0.014945f)
                                                    if (sensor3 <= 0.531954f)
                                                        return 5;
                                                    else
                                                        return 4;
                                                else
                                                    return 1;
                                    else
                                        return 2;
                                else
                                    if (sensor0 <= 0.735460f)
                                        if (actual_vx <= 0.012512f)
                                            if (sensor3 <= 0.831279f)
                                                if (actual_vz <= 1.088672f)
                                                    if (sensor3 <= 0.477114f)
                                                        return 1;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor1 <= 0.793476f)
                                                        return 1;
                                                    else
                                                        return 4;
                                            else
                                                if (actual_vx <= 0.005035f)
                                                    if (sensor3 <= 0.831856f)
                                                        return 1;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_omega <= -0.170635f)
                                                        return 2;
                                                    else
                                                        return 1;
                                        else
                                            if (actual_vx <= 0.012546f)
                                                return 5;
                                            else
                                                if (sensor3 <= 0.532126f)
                                                    if (actual_vz <= 1.106583f)
                                                        return 1;
                                                    else
                                                        return 5;
                                                else
                                                    return 5;
                                    else
                                        if (actual_vx <= 0.018030f)
                                            if (sensor2 <= 0.040524f)
                                                return 0;
                                            else
                                                if (last_action_state <= 0.450000f)
                                                    if (sensor2 <= 0.180257f)
                                                        return 1;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_omega <= -0.166466f)
                                                        return 1;
                                                    else
                                                        return 1;
                                        else
                                            return 5;
                        else
                            if (last_action_state <= 0.350000f)
                                if (sensor4 <= 0.568798f)
                                    if (actual_vz <= 1.097751f)
                                        if (sensor0 <= 0.819656f)
                                            if (sensor1 <= 0.466277f)
                                                return 3;
                                            else
                                                return 5;
                                        else
                                            if (actual_omega <= -0.241359f)
                                                if (actual_omega <= -0.249664f)
                                                    return 5;
                                                else
                                                    return 1;
                                            else
                                                if (sensor5 <= 0.028031f)
                                                    if (actual_vz <= 1.090105f)
                                                        return 2;
                                                    else
                                                        return 2;
                                                else
                                                    if (actual_vz <= 1.090725f)
                                                        return 2;
                                                    else
                                                        return 1;
                                    else
                                        if (sensor2 <= 0.026271f)
                                            return 1;
                                        else
                                            if (sensor1 <= 0.440960f)
                                                return 6;
                                            else
                                                return 2;
                                else
                                    if (actual_vx <= 0.015206f)
                                        if (actual_vz <= 1.086935f)
                                            if (sensor0 <= 0.789143f)
                                                return 1;
                                            else
                                                return 2;
                                        else
                                            return 1;
                                    else
                                        if (sensor4 <= 0.636399f)
                                            return 3;
                                        else
                                            return 4;
                            else
                                if (sensor4 <= 0.368245f)
                                    if (sensor1 <= 0.404986f)
                                        if (sensor4 <= 0.268112f)
                                            if (actual_omega <= -0.207576f)
                                                if (sensor3 <= 0.832952f)
                                                    if (sensor4 <= 0.263536f)
                                                        return 2;
                                                    else
                                                        return 1;
                                                else
                                                    return 2;
                                            else
                                                if (sensor2 <= 0.017465f)
                                                    if (sensor0 <= 0.840427f)
                                                        return 1;
                                                    else
                                                        return 0;
                                                else
                                                    if (actual_vz <= 1.083050f)
                                                        return 1;
                                                    else
                                                        return 1;
                                        else
                                            if (last_action_state <= 0.550000f)
                                                if (actual_omega <= -0.214690f)
                                                    if (sensor3 <= 0.837564f)
                                                        return 2;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor0 <= 0.831719f)
                                                        return 1;
                                                    else
                                                        return 1;
                                            else
                                                if (sensor4 <= 0.337135f)
                                                    if (sensor4 <= 0.337107f)
                                                        return 1;
                                                    else
                                                        return 0;
                                                else
                                                    if (actual_vz <= 1.124590f)
                                                        return 1;
                                                    else
                                                        return 0;
                                    else
                                        if (last_action_state <= 0.550000f)
                                            if (actual_vz <= 1.098389f)
                                                if (sensor4 <= 0.319913f)
                                                    if (sensor2 <= 0.030020f)
                                                        return 1;
                                                    else
                                                        return 2;
                                                else
                                                    if (actual_vx <= 0.003555f)
                                                        return 1;
                                                    else
                                                        return 1;
                                            else
                                                if (sensor4 <= 0.264507f)
                                                    return 0;
                                                else
                                                    if (sensor0 <= 0.800875f)
                                                        return 2;
                                                    else
                                                        return 1;
                                        else
                                            if (sensor4 <= 0.300651f)
                                                if (sensor0 <= 0.813980f)
                                                    if (sensor2 <= 0.032803f)
                                                        return 2;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor5 <= 0.009463f)
                                                        return 2;
                                                    else
                                                        return 1;
                                            else
                                                if (sensor2 <= 0.049306f)
                                                    if (sensor1 <= 0.447123f)
                                                        return 1;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor0 <= 0.829856f)
                                                        return 2;
                                                    else
                                                        return 0;
                                else
                                    if (actual_omega <= -0.197862f)
                                        if (sensor0 <= 0.831518f)
                                            if (sensor5 <= 0.033832f)
                                                if (actual_vz <= 1.105106f)
                                                    if (last_action_state <= 0.550000f)
                                                        return 1;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor1 <= 0.474436f)
                                                        return 0;
                                                    else
                                                        return 2;
                                            else
                                                if (sensor0 <= 0.821360f)
                                                    if (sensor3 <= 0.719317f)
                                                        return 1;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor2 <= 0.106050f)
                                                        return 1;
                                                    else
                                                        return 1;
                                        else
                                            if (actual_omega <= -0.197916f)
                                                if (last_action_state <= 0.450000f)
                                                    if (sensor1 <= 0.395539f)
                                                        return 2;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_omega <= -0.216091f)
                                                        return 1;
                                                    else
                                                        return 1;
                                            else
                                                if (actual_omega <= -0.197887f)
                                                    return 2;
                                                else
                                                    return 1;
                                    else
                                        if (sensor0 <= 0.798340f)
                                            if (actual_omega <= -0.193435f)
                                                if (sensor0 <= 0.791001f)
                                                    if (actual_vx <= 0.016303f)
                                                        return 1;
                                                    else
                                                        return 2;
                                                else
                                                    if (actual_omega <= -0.197092f)
                                                        return 2;
                                                    else
                                                        return 1;
                                            else
                                                if (sensor4 <= 0.418112f)
                                                    if (sensor1 <= 0.479803f)
                                                        return 0;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_omega <= -0.186786f)
                                                        return 1;
                                                    else
                                                        return 1;
                                        else
                                            if (sensor5 <= 0.018720f)
                                                return 2;
                                            else
                                                if (actual_vx <= 0.016674f)
                                                    if (sensor5 <= 0.066607f)
                                                        return 1;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor3 <= 0.690898f)
                                                        return 1;
                                                    else
                                                        return 0;
        else
            if (sensor2 <= 0.319522f)
                if (sensor2 <= 0.280087f)
                    if (actual_omega <= -0.020420f)
                        if (sensor2 <= 0.246995f)
                            if (actual_vz <= 0.988604f)
                                if (sensor5 <= 0.412598f)
                                    if (actual_vx <= 0.010656f)
                                        if (sensor1 <= 0.840241f)
                                            if (actual_omega <= -0.080651f)
                                                if (sensor1 <= 0.840166f)
                                                    if (sensor3 <= 0.350874f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    return 4;
                                            else
                                                if (sensor5 <= 0.344682f)
                                                    if (sensor3 <= 0.632295f)
                                                        return 5;
                                                    else
                                                        return 4;
                                                else
                                                    if (actual_vx <= -0.001555f)
                                                        return 5;
                                                    else
                                                        return 5;
                                        else
                                            if (last_action_state <= 0.150000f)
                                                if (sensor0 <= 0.369999f)
                                                    if (sensor3 <= 0.369576f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor0 <= 0.385765f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor0 <= 0.622277f)
                                                    if (sensor1 <= 0.880055f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor4 <= 0.846378f)
                                                        return 5;
                                                    else
                                                        return 1;
                                    else
                                        if (sensor0 <= 0.387983f)
                                            if (sensor3 <= 0.353585f)
                                                return 4;
                                            else
                                                return 5;
                                        else
                                            if (actual_vx <= 0.015394f)
                                                if (sensor3 <= 0.320172f)
                                                    if (sensor3 <= 0.307505f)
                                                        return 5;
                                                    else
                                                        return 4;
                                                else
                                                    if (sensor0 <= 0.678286f)
                                                        return 5;
                                                    else
                                                        return 4;
                                            else
                                                if (last_action_state <= 0.300000f)
                                                    return 5;
                                                else
                                                    if (sensor3 <= 0.472429f)
                                                        return 4;
                                                    else
                                                        return 5;
                                else
                                    if (actual_vz <= 0.836636f)
                                        if (last_action_state <= 0.200000f)
                                            if (sensor2 <= 0.202768f)
                                                if (sensor2 <= 0.182117f)
                                                    return 5;
                                                else
                                                    return 4;
                                            else
                                                if (sensor0 <= 0.386514f)
                                                    if (sensor2 <= 0.224540f)
                                                        return 6;
                                                    else
                                                        return 7;
                                                else
                                                    return 3;
                                        else
                                            if (actual_vx <= 0.017549f)
                                                if (sensor1 <= 0.815703f)
                                                    if (sensor0 <= 0.437720f)
                                                        return 4;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vx <= -0.020746f)
                                                        return 6;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor1 <= 0.817515f)
                                                    if (last_action_state <= 0.350000f)
                                                        return 4;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor3 <= 0.378917f)
                                                        return 3;
                                                    else
                                                        return 4;
                                    else
                                        if (actual_vx <= 0.004059f)
                                            if (sensor3 <= 0.343848f)
                                                if (sensor2 <= 0.233232f)
                                                    if (last_action_state <= 0.450000f)
                                                        return 0;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= -0.032059f)
                                                        return 1;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor0 <= 0.436518f)
                                                    if (actual_vz <= 0.846500f)
                                                        return 3;
                                                    else
                                                        return 5;
                                                else
                                                    return 4;
                                        else
                                            if (sensor5 <= 0.544457f)
                                                if (sensor1 <= 0.861589f)
                                                    if (actual_vz <= 0.954152f)
                                                        return 5;
                                                    else
                                                        return 4;
                                                else
                                                    if (sensor5 <= 0.539963f)
                                                        return 5;
                                                    else
                                                        return 4;
                                            else
                                                if (sensor2 <= 0.188100f)
                                                    if (actual_omega <= -0.150434f)
                                                        return 4;
                                                    else
                                                        return 3;
                                                else
                                                    if (sensor2 <= 0.242446f)
                                                        return 4;
                                                    else
                                                        return 1;
                            else
                                if (sensor0 <= 0.428750f)
                                    if (actual_omega <= -0.072176f)
                                        if (sensor5 <= 0.372580f)
                                            if (sensor1 <= 0.835517f)
                                                if (actual_omega <= -0.115340f)
                                                    if (sensor2 <= 0.237449f)
                                                        return 5;
                                                    else
                                                        return 4;
                                                else
                                                    if (actual_vx <= -0.001010f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor5 <= 0.315911f)
                                                    if (actual_vx <= -0.005206f)
                                                        return 1;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vz <= 1.097266f)
                                                        return 5;
                                                    else
                                                        return 5;
                                        else
                                            if (actual_vz <= 1.100317f)
                                                if (sensor5 <= 0.501435f)
                                                    if (actual_vx <= 0.028032f)
                                                        return 4;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vx <= 0.006751f)
                                                        return 0;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor2 <= 0.207311f)
                                                    return 1;
                                                else
                                                    if (sensor5 <= 0.493340f)
                                                        return 5;
                                                    else
                                                        return 2;
                                    else
                                        if (sensor0 <= 0.384223f)
                                            if (sensor5 <= 0.347610f)
                                                if (sensor2 <= 0.215114f)
                                                    return 4;
                                                else
                                                    if (actual_vx <= 0.010897f)
                                                        return 5;
                                                    else
                                                        return 4;
                                            else
                                                if (sensor4 <= 0.889821f)
                                                    return 1;
                                                else
                                                    if (sensor5 <= 0.481534f)
                                                        return 5;
                                                    else
                                                        return 1;
                                        else
                                            if (actual_vx <= -0.002829f)
                                                if (sensor5 <= 0.285006f)
                                                    if (sensor0 <= 0.385113f)
                                                        return 1;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vz <= 1.105971f)
                                                        return 1;
                                                    else
                                                        return 1;
                                            else
                                                if (sensor2 <= 0.215341f)
                                                    if (sensor5 <= 0.334649f)
                                                        return 5;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor0 <= 0.386659f)
                                                        return 1;
                                                    else
                                                        return 5;
                                else
                                    if (sensor0 <= 0.616031f)
                                        if (actual_vx <= -0.000719f)
                                            if (actual_omega <= -0.091816f)
                                                if (sensor0 <= 0.480980f)
                                                    if (actual_omega <= -0.119416f)
                                                        return 5;
                                                    else
                                                        return 1;
                                                else
                                                    if (last_action_state <= 0.450000f)
                                                        return 1;
                                                    else
                                                        return 1;
                                            else
                                                if (actual_omega <= -0.045710f)
                                                    if (sensor3 <= 0.445062f)
                                                        return 5;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_vz <= 1.107835f)
                                                        return 1;
                                                    else
                                                        return 1;
                                        else
                                            if (actual_omega <= -0.111894f)
                                                if (sensor1 <= 0.812864f)
                                                    if (actual_vx <= 0.000381f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor5 <= 0.304787f)
                                                        return 5;
                                                    else
                                                        return 4;
                                            else
                                                if (sensor2 <= 0.236177f)
                                                    if (sensor0 <= 0.548292f)
                                                        return 5;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor0 <= 0.435512f)
                                                        return 4;
                                                    else
                                                        return 5;
                                    else
                                        if (actual_omega <= -0.140673f)
                                            if (sensor0 <= 0.664406f)
                                                if (sensor2 <= 0.224415f)
                                                    if (sensor2 <= 0.212078f)
                                                        return 5;
                                                    else
                                                        return 4;
                                                else
                                                    if (sensor0 <= 0.633448f)
                                                        return 5;
                                                    else
                                                        return 1;
                                            else
                                                if (sensor4 <= 0.826203f)
                                                    if (sensor2 <= 0.209738f)
                                                        return 5;
                                                    else
                                                        return 4;
                                                else
                                                    if (actual_vx <= 0.010313f)
                                                        return 1;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor2 <= 0.223614f)
                                                if (sensor2 <= 0.202242f)
                                                    if (last_action_state <= 0.050000f)
                                                        return 2;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_vx <= 0.006533f)
                                                        return 1;
                                                    else
                                                        return 1;
                                            else
                                                if (sensor0 <= 0.630735f)
                                                    if (actual_omega <= -0.078786f)
                                                        return 5;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_omega <= -0.119576f)
                                                        return 5;
                                                    else
                                                        return 4;
                        else
                            if (sensor2 <= 0.273768f)
                                if (last_action_state <= 0.350000f)
                                    if (sensor2 <= 0.262581f)
                                        if (sensor5 <= 0.351718f)
                                            if (sensor0 <= 0.367642f)
                                                if (sensor5 <= 0.255650f)
                                                    if (sensor4 <= 0.866225f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor5 <= 0.269004f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor2 <= 0.261876f)
                                                    if (actual_vz <= 1.087306f)
                                                        return 5;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor1 <= 0.838443f)
                                                        return 5;
                                                    else
                                                        return 4;
                                        else
                                            if (last_action_state <= 0.150000f)
                                                if (sensor1 <= 0.866780f)
                                                    if (sensor3 <= 0.229828f)
                                                        return 1;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= -0.064436f)
                                                        return 4;
                                                    else
                                                        return 3;
                                            else
                                                if (actual_vx <= 0.017831f)
                                                    if (sensor0 <= 0.367517f)
                                                        return 4;
                                                    else
                                                        return 3;
                                                else
                                                    if (sensor3 <= 0.352524f)
                                                        return 6;
                                                    else
                                                        return 3;
                                    else
                                        if (sensor0 <= 0.331359f)
                                            if (sensor4 <= 0.883136f)
                                                if (sensor5 <= 0.281905f)
                                                    if (actual_vz <= 1.040276f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor1 <= 0.802206f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor1 <= 0.831980f)
                                                    if (sensor5 <= 0.447207f)
                                                        return 7;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor3 <= 0.356536f)
                                                        return 4;
                                                    else
                                                        return 10;
                                        else
                                            if (sensor4 <= 0.850091f)
                                                if (sensor2 <= 0.269594f)
                                                    if (sensor1 <= 0.855474f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor1 <= 0.864352f)
                                                        return 6;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vx <= 0.003462f)
                                                    if (actual_vx <= -0.002567f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor3 <= 0.316511f)
                                                        return 4;
                                                    else
                                                        return 5;
                                else
                                    if (actual_omega <= -0.086985f)
                                        if (sensor2 <= 0.261807f)
                                            if (actual_vx <= 0.010285f)
                                                if (sensor0 <= 0.355694f)
                                                    if (sensor1 <= 0.847006f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor5 <= 0.240763f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor4 <= 0.879018f)
                                                    if (sensor5 <= 0.273982f)
                                                        return 5;
                                                    else
                                                        return 4;
                                                else
                                                    if (actual_vz <= 0.995913f)
                                                        return 4;
                                                    else
                                                        return 5;
                                        else
                                            if (actual_vx <= 0.004821f)
                                                if (sensor4 <= 0.861693f)
                                                    if (sensor1 <= 0.805514f)
                                                        return 6;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_omega <= -0.116298f)
                                                        return 6;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor0 <= 0.323227f)
                                                    if (actual_vz <= 0.988799f)
                                                        return 4;
                                                    else
                                                        return 10;
                                                else
                                                    if (actual_vx <= 0.021082f)
                                                        return 5;
                                                    else
                                                        return 4;
                                    else
                                        if (sensor4 <= 0.824688f)
                                            if (sensor2 <= 0.268236f)
                                                if (sensor2 <= 0.263096f)
                                                    if (actual_vx <= 0.002111f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= -0.067262f)
                                                        return 6;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor5 <= 0.263268f)
                                                    if (actual_omega <= -0.034393f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= -0.042681f)
                                                        return 5;
                                                    else
                                                        return 5;
                                        else
                                            if (actual_vx <= 0.006466f)
                                                if (sensor2 <= 0.267457f)
                                                    if (sensor0 <= 0.322706f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor4 <= 0.873509f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vx <= 0.013689f)
                                                    if (sensor0 <= 0.467369f)
                                                        return 5;
                                                    else
                                                        return 4;
                                                else
                                                    if (sensor1 <= 0.831433f)
                                                        return 5;
                                                    else
                                                        return 4;
                            else
                                if (sensor4 <= 0.845007f)
                                    if (sensor2 <= 0.275831f)
                                        if (actual_omega <= -0.030954f)
                                            if (actual_vx <= 0.001026f)
                                                if (actual_vz <= 1.042192f)
                                                    if (sensor2 <= 0.274387f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vz <= 1.044124f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (last_action_state <= 0.550000f)
                                                    if (sensor1 <= 0.896690f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    return 5;
                                        else
                                            if (sensor4 <= 0.811859f)
                                                if (last_action_state <= 0.450000f)
                                                    if (sensor4 <= 0.799537f)
                                                        return 0;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vz <= 1.042176f)
                                                        return 5;
                                                    else
                                                        return 6;
                                            else
                                                if (actual_vx <= -0.001354f)
                                                    return 6;
                                                else
                                                    if (sensor5 <= 0.241769f)
                                                        return 6;
                                                    else
                                                        return 5;
                                    else
                                        if (actual_vz <= 1.066929f)
                                            if (sensor5 <= 0.265688f)
                                                if (actual_omega <= -0.027676f)
                                                    if (sensor0 <= 0.369836f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor2 <= 0.275983f)
                                                        return 5;
                                                    else
                                                        return 6;
                                            else
                                                if (actual_omega <= -0.021656f)
                                                    if (sensor4 <= 0.836176f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor4 <= 0.813565f)
                                                        return 6;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor4 <= 0.810448f)
                                                if (actual_vz <= 1.077873f)
                                                    return 4;
                                                else
                                                    if (last_action_state <= 0.500000f)
                                                        return 1;
                                                    else
                                                        return 6;
                                            else
                                                if (last_action_state <= 0.200000f)
                                                    return 8;
                                                else
                                                    if (actual_omega <= -0.073258f)
                                                        return 6;
                                                    else
                                                        return 5;
                                else
                                    if (actual_omega <= -0.069263f)
                                        if (actual_vx <= 0.006554f)
                                            if (actual_omega <= -0.088079f)
                                                if (sensor3 <= 0.299922f)
                                                    if (actual_omega <= -0.105247f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vz <= 1.097408f)
                                                        return 6;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vx <= 0.001237f)
                                                    if (sensor3 <= 0.268657f)
                                                        return 1;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_omega <= -0.071199f)
                                                        return 5;
                                                    else
                                                        return 6;
                                        else
                                            if (actual_vz <= 0.994219f)
                                                if (sensor3 <= 0.306595f)
                                                    if (last_action_state <= 0.450000f)
                                                        return 4;
                                                    else
                                                        return 3;
                                                else
                                                    if (sensor5 <= 0.454862f)
                                                        return 5;
                                                    else
                                                        return 9;
                                            else
                                                if (sensor3 <= 0.238853f)
                                                    return 5;
                                                else
                                                    if (actual_vx <= 0.019179f)
                                                        return 10;
                                                    else
                                                        return 8;
                                    else
                                        if (last_action_state <= 0.350000f)
                                            if (sensor4 <= 0.875264f)
                                                if (sensor3 <= 0.286699f)
                                                    if (sensor1 <= 0.844845f)
                                                        return 4;
                                                    else
                                                        return 2;
                                                else
                                                    if (actual_omega <= -0.039871f)
                                                        return 6;
                                                    else
                                                        return 6;
                                            else
                                                if (actual_vz <= 1.107437f)
                                                    if (sensor5 <= 0.297457f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    return 10;
                                        else
                                            if (sensor1 <= 0.897609f)
                                                if (actual_vx <= -0.001260f)
                                                    if (actual_vz <= 1.084511f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vx <= 0.006180f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor0 <= 0.344268f)
                                                    if (actual_omega <= -0.043097f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor4 <= 0.850332f)
                                                        return 6;
                                                    else
                                                        return 5;
                    else
                        if (sensor2 <= 0.243159f)
                            if (actual_vz <= 0.979104f)
                                if (sensor5 <= 0.453268f)
                                    if (sensor2 <= 0.189310f)
                                        return 1;
                                    else
                                        if (sensor4 <= 0.771836f)
                                            return 4;
                                        else
                                            if (sensor5 <= 0.218361f)
                                                if (sensor3 <= 0.414962f)
                                                    return 1;
                                                else
                                                    return 5;
                                            else
                                                if (sensor5 <= 0.330491f)
                                                    if (sensor0 <= 0.390082f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor5 <= 0.406099f)
                                                        return 5;
                                                    else
                                                        return 5;
                                else
                                    if (sensor4 <= 0.985023f)
                                        if (actual_omega <= -0.015397f)
                                            return 1;
                                        else
                                            return 0;
                                    else
                                        if (actual_vx <= 0.008025f)
                                            if (last_action_state <= 0.350000f)
                                                if (sensor3 <= 0.330923f)
                                                    if (actual_vx <= -0.000672f)
                                                        return 0;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_omega <= 0.026343f)
                                                        return 5;
                                                    else
                                                        return 4;
                                            else
                                                if (sensor0 <= 0.354121f)
                                                    if (actual_vx <= -0.002144f)
                                                        return 7;
                                                    else
                                                        return 4;
                                                else
                                                    if (sensor5 <= 0.610652f)
                                                        return 5;
                                                    else
                                                        return 0;
                                        else
                                            if (actual_vx <= 0.009576f)
                                                if (sensor3 <= 0.352935f)
                                                    if (sensor1 <= 0.836015f)
                                                        return 4;
                                                    else
                                                        return 3;
                                                else
                                                    return 1;
                                            else
                                                if (actual_omega <= 0.013995f)
                                                    if (sensor5 <= 0.524751f)
                                                        return 4;
                                                    else
                                                        return 3;
                                                else
                                                    if (actual_vx <= 0.009866f)
                                                        return 4;
                                                    else
                                                        return 5;
                            else
                                if (sensor2 <= 0.232559f)
                                    if (sensor2 <= 0.224058f)
                                        if (actual_vz <= 1.040847f)
                                            if (sensor1 <= 0.810784f)
                                                return 4;
                                            else
                                                if (sensor1 <= 0.814114f)
                                                    return 1;
                                                else
                                                    return 5;
                                        else
                                            if (actual_omega <= -0.019416f)
                                                if (sensor2 <= 0.218901f)
                                                    return 5;
                                                else
                                                    return 1;
                                            else
                                                if (sensor3 <= 0.372535f)
                                                    if (sensor5 <= 0.245571f)
                                                        return 5;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor5 <= 0.319043f)
                                                        return 1;
                                                    else
                                                        return 1;
                                    else
                                        if (sensor3 <= 0.282747f)
                                            if (sensor3 <= 0.238487f)
                                                if (sensor4 <= 0.884439f)
                                                    return 0;
                                                else
                                                    return 1;
                                            else
                                                return 0;
                                        else
                                            if (sensor4 <= 0.830958f)
                                                if (sensor2 <= 0.225339f)
                                                    if (sensor1 <= 0.851826f)
                                                        return 4;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_omega <= 0.035681f)
                                                        return 5;
                                                    else
                                                        return 4;
                                            else
                                                if (sensor3 <= 0.353309f)
                                                    if (actual_vz <= 1.030815f)
                                                        return 5;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_omega <= -0.000233f)
                                                        return 1;
                                                    else
                                                        return 5;
                                else
                                    if (sensor3 <= 0.381136f)
                                        if (last_action_state <= 0.350000f)
                                            if (sensor2 <= 0.242077f)
                                                if (sensor3 <= 0.223972f)
                                                    if (sensor5 <= 0.558230f)
                                                        return 1;
                                                    else
                                                        return 0;
                                                else
                                                    return 1;
                                            else
                                                if (sensor4 <= 0.950220f)
                                                    return 3;
                                                else
                                                    return 4;
                                        else
                                            if (sensor2 <= 0.236677f)
                                                if (actual_omega <= -0.002849f)
                                                    if (actual_vz <= 1.103622f)
                                                        return 1;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vx <= -0.000720f)
                                                        return 1;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vx <= -0.006410f)
                                                    if (actual_vz <= 1.099818f)
                                                        return 5;
                                                    else
                                                        return 0;
                                                else
                                                    if (actual_vx <= -0.005784f)
                                                        return 1;
                                                    else
                                                        return 5;
                                    else
                                        if (sensor1 <= 0.869871f)
                                            if (sensor4 <= 0.871513f)
                                                if (sensor5 <= 0.187223f)
                                                    return 4;
                                                else
                                                    if (actual_vz <= 1.074233f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor5 <= 0.269295f)
                                                    if (sensor1 <= 0.841093f)
                                                        return 4;
                                                    else
                                                        return 0;
                                                else
                                                    if (actual_vx <= -0.004309f)
                                                        return 5;
                                                    else
                                                        return 1;
                                        else
                                            return 1;
                        else
                            if (sensor5 <= 0.450675f)
                                if (sensor1 <= 0.823563f)
                                    if (sensor2 <= 0.261631f)
                                        if (sensor2 <= 0.253360f)
                                            if (sensor4 <= 0.778989f)
                                                return 6;
                                            else
                                                if (sensor3 <= 0.339564f)
                                                    if (sensor0 <= 0.369501f)
                                                        return 5;
                                                    else
                                                        return 4;
                                                else
                                                    return 5;
                                        else
                                            if (actual_vz <= 1.037917f)
                                                if (sensor4 <= 0.791332f)
                                                    if (actual_vz <= 0.961281f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor5 <= 0.398049f)
                                                        return 5;
                                                    else
                                                        return 0;
                                            else
                                                if (sensor3 <= 0.271810f)
                                                    if (actual_vx <= 0.002399f)
                                                        return 1;
                                                    else
                                                        return 4;
                                                else
                                                    if (actual_omega <= 0.000878f)
                                                        return 5;
                                                    else
                                                        return 5;
                                    else
                                        if (actual_vz <= 0.959822f)
                                            if (actual_vx <= 0.010186f)
                                                if (last_action_state <= 0.450000f)
                                                    if (sensor5 <= 0.265358f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor4 <= 0.817115f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor5 <= 0.321345f)
                                                    return 5;
                                                else
                                                    return 4;
                                        else
                                            if (sensor5 <= 0.325326f)
                                                if (sensor1 <= 0.802428f)
                                                    if (last_action_state <= 0.250000f)
                                                        return 4;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor3 <= 0.339117f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor5 <= 0.374465f)
                                                    if (sensor1 <= 0.799669f)
                                                        return 1;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor5 <= 0.393743f)
                                                        return 4;
                                                    else
                                                        return 5;
                                else
                                    if (sensor2 <= 0.273961f)
                                        if (actual_vx <= -0.003448f)
                                            if (sensor1 <= 0.853149f)
                                                if (sensor1 <= 0.847582f)
                                                    if (sensor5 <= 0.316693f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor4 <= 0.789218f)
                                                        return 6;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor2 <= 0.266808f)
                                                    if (actual_vx <= -0.007519f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor3 <= 0.338827f)
                                                        return 5;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor0 <= 0.423012f)
                                                if (actual_vx <= 0.005685f)
                                                    if (sensor2 <= 0.269840f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor5 <= 0.329341f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vz <= 1.068928f)
                                                    if (sensor1 <= 0.825899f)
                                                        return 4;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor2 <= 0.255463f)
                                                        return 5;
                                                    else
                                                        return 5;
                                    else
                                        if (sensor4 <= 0.848218f)
                                            if (sensor4 <= 0.802775f)
                                                if (actual_omega <= 0.001750f)
                                                    if (actual_omega <= -0.012159f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vx <= -0.001179f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vx <= -0.003733f)
                                                    if (actual_omega <= -0.003247f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= 0.003036f)
                                                        return 5;
                                                    else
                                                        return 5;
                                        else
                                            if (actual_vx <= 0.006957f)
                                                if (actual_vx <= -0.004071f)
                                                    if (actual_vz <= 0.966160f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= 0.002621f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor5 <= 0.428323f)
                                                    if (actual_vz <= 1.118935f)
                                                        return 5;
                                                    else
                                                        return 4;
                                                else
                                                    if (sensor5 <= 0.440721f)
                                                        return 4;
                                                    else
                                                        return 5;
                            else
                                if (last_action_state <= 0.350000f)
                                    if (actual_omega <= 0.019039f)
                                        if (sensor1 <= 0.811394f)
                                            if (actual_vz <= 0.904878f)
                                                return 5;
                                            else
                                                return 1;
                                        else
                                            if (sensor5 <= 0.515682f)
                                                if (sensor1 <= 0.844007f)
                                                    if (actual_omega <= 0.002362f)
                                                        return 4;
                                                    else
                                                        return 8;
                                                else
                                                    if (sensor3 <= 0.338700f)
                                                        return 5;
                                                    else
                                                        return 4;
                                            else
                                                if (actual_vz <= 1.068337f)
                                                    if (actual_vz <= 0.892035f)
                                                        return 3;
                                                    else
                                                        return 0;
                                                else
                                                    if (sensor5 <= 0.531817f)
                                                        return 2;
                                                    else
                                                        return 4;
                                    else
                                        if (actual_vz <= 0.818292f)
                                            if (last_action_state <= 0.050000f)
                                                return 10;
                                            else
                                                return 7;
                                        else
                                            if (actual_vz <= 0.993741f)
                                                if (sensor2 <= 0.272372f)
                                                    if (sensor3 <= 0.330771f)
                                                        return 0;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_vz <= 0.973846f)
                                                        return 3;
                                                    else
                                                        return 2;
                                            else
                                                if (actual_vx <= 0.004646f)
                                                    if (sensor1 <= 0.839016f)
                                                        return 1;
                                                    else
                                                        return 0;
                                                else
                                                    return 2;
                                else
                                    if (sensor1 <= 0.852446f)
                                        if (actual_vx <= 0.000456f)
                                            if (actual_vz <= 0.885417f)
                                                if (actual_vx <= -0.007674f)
                                                    if (actual_omega <= 0.102319f)
                                                        return 6;
                                                    else
                                                        return 7;
                                                else
                                                    if (actual_vx <= -0.002533f)
                                                        return 5;
                                                    else
                                                        return 0;
                                            else
                                                if (sensor3 <= 0.323817f)
                                                    if (sensor2 <= 0.270804f)
                                                        return 0;
                                                    else
                                                        return 5;
                                                else
                                                    return 5;
                                        else
                                            if (actual_vz <= 1.094392f)
                                                if (sensor3 <= 0.255996f)
                                                    if (actual_vz <= 0.945978f)
                                                        return 1;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor2 <= 0.279983f)
                                                        return 5;
                                                    else
                                                        return 4;
                                            else
                                                if (sensor0 <= 0.332953f)
                                                    if (sensor2 <= 0.278490f)
                                                        return 5;
                                                    else
                                                        return 4;
                                                else
                                                    if (sensor1 <= 0.847288f)
                                                        return 4;
                                                    else
                                                        return 1;
                                    else
                                        if (actual_vx <= -0.011776f)
                                            if (actual_vz <= 0.907734f)
                                                if (actual_vz <= 0.823776f)
                                                    if (actual_vz <= 0.775068f)
                                                        return 7;
                                                    else
                                                        return 6;
                                                else
                                                    return 7;
                                            else
                                                if (sensor1 <= 0.881736f)
                                                    return 5;
                                                else
                                                    return 0;
                                        else
                                            if (last_action_state <= 0.450000f)
                                                if (actual_vz <= 0.770233f)
                                                    return 3;
                                                else
                                                    if (sensor3 <= 0.370393f)
                                                        return 4;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor0 <= 0.388619f)
                                                    if (sensor5 <= 0.451071f)
                                                        return 4;
                                                    else
                                                        return 5;
                                                else
                                                    return 1;
                else
                    if (actual_omega <= -0.019948f)
                        if (actual_vx <= 0.006021f)
                            if (sensor5 <= 0.263781f)
                                if (sensor0 <= 0.341316f)
                                    if (sensor2 <= 0.288952f)
                                        if (actual_omega <= -0.069126f)
                                            if (actual_vx <= 0.004744f)
                                                if (actual_vz <= 1.028413f)
                                                    if (last_action_state <= 0.450000f)
                                                        return 6;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor0 <= 0.315498f)
                                                        return 6;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor3 <= 0.359422f)
                                                    return 10;
                                                else
                                                    if (sensor2 <= 0.286582f)
                                                        return 6;
                                                    else
                                                        return 9;
                                        else
                                            if (sensor1 <= 0.882857f)
                                                if (sensor4 <= 0.816593f)
                                                    if (sensor2 <= 0.286382f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= -0.056586f)
                                                        return 6;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor4 <= 0.870350f)
                                                    if (sensor0 <= 0.336781f)
                                                        return 6;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_omega <= -0.038152f)
                                                        return 6;
                                                    else
                                                        return 5;
                                    else
                                        if (sensor5 <= 0.255908f)
                                            if (sensor1 <= 0.862763f)
                                                if (sensor1 <= 0.861545f)
                                                    if (sensor4 <= 0.873426f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_omega <= -0.072747f)
                                                        return 10;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vz <= 1.076018f)
                                                    if (sensor5 <= 0.252743f)
                                                        return 6;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vz <= 1.089381f)
                                                        return 10;
                                                    else
                                                        return 6;
                                        else
                                            if (sensor1 <= 0.845823f)
                                                if (sensor4 <= 0.895996f)
                                                    if (actual_vz <= 0.930020f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    return 5;
                                            else
                                                if (sensor2 <= 0.292174f)
                                                    if (actual_vz <= 1.045242f)
                                                        return 6;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vz <= 0.932020f)
                                                        return 5;
                                                    else
                                                        return 6;
                                else
                                    if (sensor2 <= 0.297198f)
                                        return 5;
                                    else
                                        if (actual_vz <= 1.037510f)
                                            return 5;
                                        else
                                            return 6;
                            else
                                if (actual_omega <= -0.062483f)
                                    if (actual_vz <= 1.077131f)
                                        if (sensor1 <= 0.845025f)
                                            if (sensor0 <= 0.323482f)
                                                if (actual_omega <= -0.074565f)
                                                    if (actual_vx <= 0.001638f)
                                                        return 7;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vz <= 1.035861f)
                                                        return 6;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vz <= 1.039435f)
                                                    if (sensor4 <= 0.895618f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    return 5;
                                        else
                                            if (sensor4 <= 0.898879f)
                                                if (actual_vz <= 1.059544f)
                                                    if (actual_vx <= 0.003692f)
                                                        return 6;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor1 <= 0.872723f)
                                                        return 10;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor2 <= 0.280256f)
                                                    return 5;
                                                else
                                                    if (sensor1 <= 0.876662f)
                                                        return 6;
                                                    else
                                                        return 5;
                                    else
                                        if (actual_vx <= 0.002795f)
                                            if (actual_vx <= 0.002247f)
                                                if (actual_vz <= 1.105043f)
                                                    if (sensor2 <= 0.280603f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    return 5;
                                            else
                                                if (actual_omega <= -0.063589f)
                                                    return 5;
                                                else
                                                    return 6;
                                        else
                                            if (actual_vz <= 1.103128f)
                                                if (sensor1 <= 0.855038f)
                                                    if (sensor3 <= 0.280832f)
                                                        return 5;
                                                    else
                                                        return 9;
                                                else
                                                    return 6;
                                            else
                                                return 10;
                                else
                                    if (sensor1 <= 0.873556f)
                                        if (actual_vx <= 0.000513f)
                                            if (sensor1 <= 0.853078f)
                                                if (sensor2 <= 0.281081f)
                                                    if (sensor4 <= 0.821824f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor3 <= 0.338923f)
                                                        return 5;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor0 <= 0.331667f)
                                                    if (actual_omega <= -0.022098f)
                                                        return 6;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor0 <= 0.331907f)
                                                        return 4;
                                                    else
                                                        return 5;
                                        else
                                            if (actual_omega <= -0.041992f)
                                                if (sensor0 <= 0.325146f)
                                                    if (last_action_state <= 0.450000f)
                                                        return 6;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vz <= 0.957904f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor1 <= 0.848490f)
                                                    if (sensor0 <= 0.312012f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor2 <= 0.294241f)
                                                        return 5;
                                                    else
                                                        return 6;
                                    else
                                        if (sensor4 <= 0.884127f)
                                            if (sensor0 <= 0.331851f)
                                                if (actual_vx <= 0.001143f)
                                                    return 6;
                                                else
                                                    if (sensor2 <= 0.294235f)
                                                        return 6;
                                                    else
                                                        return 6;
                                            else
                                                if (actual_omega <= -0.023313f)
                                                    if (sensor4 <= 0.843643f)
                                                        return 6;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vx <= 0.000546f)
                                                        return 6;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor0 <= 0.329599f)
                                                if (actual_omega <= -0.025852f)
                                                    if (sensor2 <= 0.287497f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor0 <= 0.323693f)
                                                        return 6;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_omega <= -0.028780f)
                                                    if (actual_vz <= 1.036338f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vz <= 1.041744f)
                                                        return 5;
                                                    else
                                                        return 5;
                        else
                            if (actual_vz <= 0.982231f)
                                if (sensor5 <= 0.342816f)
                                    if (sensor4 <= 0.873911f)
                                        if (sensor5 <= 0.232704f)
                                            if (sensor3 <= 0.409485f)
                                                return 6;
                                            else
                                                return 5;
                                        else
                                            if (sensor3 <= 0.410617f)
                                                if (sensor1 <= 0.862067f)
                                                    if (sensor2 <= 0.294309f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    return 4;
                                            else
                                                if (actual_vx <= 0.010041f)
                                                    return 9;
                                                else
                                                    return 5;
                                    else
                                        if (sensor4 <= 0.881608f)
                                            if (actual_vx <= 0.011089f)
                                                if (sensor3 <= 0.359462f)
                                                    if (sensor2 <= 0.301472f)
                                                        return 5;
                                                    else
                                                        return 3;
                                                else
                                                    return 6;
                                            else
                                                if (sensor5 <= 0.320242f)
                                                    return 4;
                                                else
                                                    return 5;
                                        else
                                            return 5;
                                else
                                    if (sensor5 <= 0.462664f)
                                        if (sensor2 <= 0.309754f)
                                            if (sensor3 <= 0.392903f)
                                                if (actual_vx <= 0.012577f)
                                                    if (actual_vz <= 0.974933f)
                                                        return 5;
                                                    else
                                                        return 4;
                                                else
                                                    if (actual_vz <= 0.938117f)
                                                        return 4;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vz <= 0.927676f)
                                                    return 3;
                                                else
                                                    if (sensor3 <= 0.404084f)
                                                        return 6;
                                                    else
                                                        return 4;
                                        else
                                            if (last_action_state <= 0.450000f)
                                                if (sensor2 <= 0.312567f)
                                                    return 9;
                                                else
                                                    return 7;
                                            else
                                                return 6;
                                    else
                                        if (actual_omega <= -0.036283f)
                                            if (sensor0 <= 0.310069f)
                                                return 10;
                                            else
                                                if (sensor3 <= 0.333872f)
                                                    if (actual_vx <= 0.009126f)
                                                        return 4;
                                                    else
                                                        return 2;
                                                else
                                                    if (sensor5 <= 0.472792f)
                                                        return 10;
                                                    else
                                                        return 5;
                                        else
                                            if (actual_vx <= 0.011684f)
                                                if (sensor3 <= 0.316247f)
                                                    return 5;
                                                else
                                                    if (sensor4 <= 0.888144f)
                                                        return 8;
                                                    else
                                                        return 5;
                                            else
                                                return 7;
                            else
                                if (sensor0 <= 0.318678f)
                                    if (actual_vz <= 1.072591f)
                                        if (sensor5 <= 0.303579f)
                                            if (sensor0 <= 0.276236f)
                                                return 10;
                                            else
                                                return 6;
                                        else
                                            if (actual_omega <= -0.060554f)
                                                if (sensor5 <= 0.340317f)
                                                    return 5;
                                                else
                                                    if (last_action_state <= 0.250000f)
                                                        return 2;
                                                    else
                                                        return 10;
                                            else
                                                if (actual_vz <= 1.024574f)
                                                    if (sensor5 <= 0.368515f)
                                                        return 5;
                                                    else
                                                        return 4;
                                                else
                                                    if (sensor3 <= 0.298032f)
                                                        return 5;
                                                    else
                                                        return 10;
                                    else
                                        if (actual_omega <= -0.029991f)
                                            if (actual_vx <= 0.009797f)
                                                if (sensor3 <= 0.318132f)
                                                    if (last_action_state <= 0.350000f)
                                                        return 4;
                                                    else
                                                        return 9;
                                                else
                                                    if (sensor2 <= 0.287604f)
                                                        return 4;
                                                    else
                                                        return 10;
                                            else
                                                if (sensor1 <= 0.849256f)
                                                    if (actual_vx <= 0.012176f)
                                                        return 10;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor1 <= 0.849541f)
                                                        return 9;
                                                    else
                                                        return 10;
                                        else
                                            if (sensor4 <= 0.881741f)
                                                if (sensor3 <= 0.403966f)
                                                    if (actual_omega <= -0.021056f)
                                                        return 5;
                                                    else
                                                        return 9;
                                                else
                                                    if (sensor3 <= 0.427334f)
                                                        return 6;
                                                    else
                                                        return 8;
                                            else
                                                if (actual_omega <= -0.027421f)
                                                    return 9;
                                                else
                                                    if (actual_omega <= -0.025495f)
                                                        return 10;
                                                    else
                                                        return 10;
                                else
                                    if (sensor5 <= 0.388714f)
                                        if (sensor0 <= 0.414288f)
                                            if (sensor0 <= 0.323612f)
                                                if (sensor3 <= 0.387574f)
                                                    if (actual_vz <= 1.101687f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor0 <= 0.321547f)
                                                        return 10;
                                                    else
                                                        return 6;
                                            else
                                                if (actual_vx <= 0.006341f)
                                                    return 9;
                                                else
                                                    if (sensor2 <= 0.280457f)
                                                        return 5;
                                                    else
                                                        return 5;
                                        else
                                            return 4;
                                    else
                                        if (actual_omega <= -0.022332f)
                                            if (sensor2 <= 0.287777f)
                                                if (sensor3 <= 0.380515f)
                                                    if (sensor5 <= 0.437922f)
                                                        return 4;
                                                    else
                                                        return 9;
                                                else
                                                    return 10;
                                            else
                                                if (sensor0 <= 0.324824f)
                                                    return 5;
                                                else
                                                    return 6;
                                        else
                                            return 5;
                    else
                        if (sensor5 <= 0.439924f)
                            if (sensor2 <= 0.290352f)
                                if (sensor4 <= 0.840199f)
                                    if (last_action_state <= 0.550000f)
                                        if (sensor3 <= 0.285280f)
                                            if (last_action_state <= 0.150000f)
                                                if (sensor5 <= 0.379527f)
                                                    return 10;
                                                else
                                                    return 4;
                                            else
                                                if (sensor4 <= 0.819689f)
                                                    if (sensor2 <= 0.288953f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor2 <= 0.280259f)
                                                        return 4;
                                                    else
                                                        return 5;
                                        else
                                            if (actual_omega <= 0.026466f)
                                                if (sensor2 <= 0.283753f)
                                                    if (sensor4 <= 0.806401f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vx <= 0.001393f)
                                                        return 6;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor2 <= 0.288501f)
                                                    if (actual_vx <= 0.008494f)
                                                        return 5;
                                                    else
                                                        return 4;
                                                else
                                                    if (actual_vx <= -0.001235f)
                                                        return 6;
                                                    else
                                                        return 5;
                                    else
                                        if (sensor2 <= 0.284727f)
                                            if (sensor0 <= 0.295033f)
                                                return 4;
                                            else
                                                if (sensor2 <= 0.280089f)
                                                    return 6;
                                                else
                                                    if (sensor4 <= 0.803215f)
                                                        return 5;
                                                    else
                                                        return 5;
                                        else
                                            if (actual_vx <= -0.003551f)
                                                if (sensor5 <= 0.242468f)
                                                    return 6;
                                                else
                                                    return 5;
                                            else
                                                if (actual_vx <= 0.012951f)
                                                    if (actual_vx <= 0.000974f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    return 4;
                                else
                                    if (last_action_state <= 0.550000f)
                                        if (sensor5 <= 0.271839f)
                                            if (sensor2 <= 0.284123f)
                                                if (sensor4 <= 0.878032f)
                                                    if (actual_vx <= -0.001896f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= -0.006732f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor4 <= 0.887978f)
                                                    if (actual_vx <= 0.003196f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= -0.014423f)
                                                        return 5;
                                                    else
                                                        return 5;
                                        else
                                            if (actual_vx <= -0.002259f)
                                                if (actual_vz <= 1.003679f)
                                                    if (sensor0 <= 0.329288f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vz <= 1.108048f)
                                                        return 5;
                                                    else
                                                        return 6;
                                            else
                                                if (actual_vx <= 0.008211f)
                                                    if (sensor4 <= 0.874904f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor4 <= 0.882358f)
                                                        return 5;
                                                    else
                                                        return 5;
                                    else
                                        if (sensor5 <= 0.348660f)
                                            if (actual_omega <= 0.036350f)
                                                if (sensor2 <= 0.283186f)
                                                    if (actual_omega <= 0.036176f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor1 <= 0.893755f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor1 <= 0.822462f)
                                                    if (actual_omega <= 0.049127f)
                                                        return 5;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor3 <= 0.284931f)
                                                        return 4;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor3 <= 0.343505f)
                                                if (sensor1 <= 0.849048f)
                                                    if (sensor4 <= 0.879948f)
                                                        return 4;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor2 <= 0.290281f)
                                                        return 5;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor4 <= 0.890267f)
                                                    if (sensor0 <= 0.323066f)
                                                        return 4;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vz <= 0.969138f)
                                                        return 5;
                                                    else
                                                        return 4;
                            else
                                if (sensor1 <= 0.868044f)
                                    if (actual_vx <= 0.006746f)
                                        if (sensor2 <= 0.296660f)
                                            if (actual_omega <= 0.012878f)
                                                if (sensor5 <= 0.265604f)
                                                    if (sensor3 <= 0.346256f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor0 <= 0.332931f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_omega <= 0.083765f)
                                                    if (sensor0 <= 0.348798f)
                                                        return 5;
                                                    else
                                                        return 4;
                                                else
                                                    return 4;
                                        else
                                            if (sensor4 <= 0.849663f)
                                                if (sensor2 <= 0.303931f)
                                                    if (actual_omega <= 0.011885f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor4 <= 0.847250f)
                                                        return 6;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor3 <= 0.382621f)
                                                    if (actual_omega <= -0.006203f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= 0.104564f)
                                                        return 6;
                                                    else
                                                        return 7;
                                    else
                                        if (actual_vz <= 1.028790f)
                                            if (sensor5 <= 0.424686f)
                                                if (actual_vx <= 0.009649f)
                                                    if (sensor2 <= 0.290500f)
                                                        return 4;
                                                    else
                                                        return 5;
                                                else
                                                    if (last_action_state <= 0.750000f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vx <= 0.011531f)
                                                    if (sensor5 <= 0.425086f)
                                                        return 4;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor2 <= 0.297519f)
                                                        return 4;
                                                    else
                                                        return 3;
                                        else
                                            if (actual_omega <= 0.001758f)
                                                if (sensor0 <= 0.308495f)
                                                    if (actual_vz <= 1.078957f)
                                                        return 5;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor3 <= 0.336434f)
                                                        return 5;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor5 <= 0.241411f)
                                                    if (actual_omega <= 0.006376f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor2 <= 0.299375f)
                                                        return 5;
                                                    else
                                                        return 5;
                                else
                                    if (last_action_state <= 0.550000f)
                                        if (sensor0 <= 0.341450f)
                                            if (sensor5 <= 0.283946f)
                                                if (sensor4 <= 0.846639f)
                                                    if (actual_vz <= 1.078238f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor2 <= 0.293248f)
                                                        return 5;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor3 <= 0.325260f)
                                                    if (sensor3 <= 0.259198f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor4 <= 0.894437f)
                                                        return 5;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor0 <= 0.357048f)
                                                if (actual_vx <= -0.001969f)
                                                    if (sensor3 <= 0.301748f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor3 <= 0.349154f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vx <= 0.003442f)
                                                    return 5;
                                                else
                                                    return 4;
                                    else
                                        if (actual_omega <= -0.004399f)
                                            if (sensor4 <= 0.876009f)
                                                if (sensor3 <= 0.369659f)
                                                    return 6;
                                                else
                                                    if (sensor4 <= 0.859949f)
                                                        return 5;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor0 <= 0.319081f)
                                                    if (actual_vz <= 1.017007f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor4 <= 0.889931f)
                                                        return 5;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor5 <= 0.246379f)
                                                if (sensor0 <= 0.334916f)
                                                    if (actual_vx <= 0.001457f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    return 5;
                                            else
                                                if (actual_omega <= 0.030994f)
                                                    if (sensor4 <= 0.886427f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor0 <= 0.320938f)
                                                        return 5;
                                                    else
                                                        return 5;
                        else
                            if (actual_vx <= -0.006431f)
                                if (actual_vz <= 1.006844f)
                                    if (sensor0 <= 0.338638f)
                                        if (sensor3 <= 0.391379f)
                                            if (sensor2 <= 0.294272f)
                                                if (sensor0 <= 0.314975f)
                                                    return 7;
                                                else
                                                    if (sensor3 <= 0.302189f)
                                                        return 5;
                                                    else
                                                        return 6;
                                            else
                                                if (actual_vz <= 0.982570f)
                                                    if (actual_vx <= -0.027891f)
                                                        return 7;
                                                    else
                                                        return 6;
                                                else
                                                    return 7;
                                        else
                                            if (last_action_state <= 0.350000f)
                                                if (actual_vz <= 0.744759f)
                                                    return 10;
                                                else
                                                    return 9;
                                            else
                                                if (actual_omega <= 0.106603f)
                                                    if (actual_vz <= 0.908655f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor3 <= 0.423207f)
                                                        return 7;
                                                    else
                                                        return 6;
                                    else
                                        return 5;
                                else
                                    if (sensor3 <= 0.379556f)
                                        if (actual_omega <= 0.049597f)
                                            return 1;
                                        else
                                            if (sensor2 <= 0.317531f)
                                                if (sensor0 <= 0.317437f)
                                                    if (actual_vx <= -0.024587f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor2 <= 0.286054f)
                                                        return 0;
                                                    else
                                                        return 5;
                                            else
                                                return 7;
                                    else
                                        return 6;
                            else
                                if (last_action_state <= 0.350000f)
                                    if (sensor5 <= 0.483786f)
                                        if (actual_vz <= 0.930617f)
                                            if (actual_omega <= 0.054274f)
                                                return 6;
                                            else
                                                return 7;
                                        else
                                            if (sensor4 <= 0.913518f)
                                                if (sensor3 <= 0.267104f)
                                                    if (actual_vx <= 0.006046f)
                                                        return 3;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor1 <= 0.853022f)
                                                        return 5;
                                                    else
                                                        return 1;
                                            else
                                                return 10;
                                    else
                                        if (actual_vz <= 0.981122f)
                                            if (sensor0 <= 0.308511f)
                                                if (last_action_state <= 0.150000f)
                                                    if (actual_vz <= 0.945495f)
                                                        return 8;
                                                    else
                                                        return 7;
                                                else
                                                    if (sensor3 <= 0.255374f)
                                                        return 6;
                                                    else
                                                        return 7;
                                            else
                                                if (actual_vx <= 0.003911f)
                                                    if (sensor1 <= 0.854401f)
                                                        return 0;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor4 <= 0.873343f)
                                                        return 3;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor0 <= 0.306225f)
                                                if (sensor4 <= 0.861738f)
                                                    if (sensor1 <= 0.846615f)
                                                        return 2;
                                                    else
                                                        return 0;
                                                else
                                                    if (actual_vx <= -0.002050f)
                                                        return 5;
                                                    else
                                                        return 10;
                                            else
                                                if (actual_vx <= 0.002305f)
                                                    if (actual_vz <= 1.105212f)
                                                        return 0;
                                                    else
                                                        return 3;
                                                else
                                                    if (actual_vx <= 0.006956f)
                                                        return 1;
                                                    else
                                                        return 3;
                                else
                                    if (sensor5 <= 0.521777f)
                                        if (actual_vz <= 1.020873f)
                                            if (actual_vx <= 0.001519f)
                                                if (actual_omega <= 0.083812f)
                                                    if (actual_vz <= 0.949714f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor3 <= 0.437654f)
                                                        return 5;
                                                    else
                                                        return 7;
                                            else
                                                if (actual_vx <= 0.011197f)
                                                    if (sensor2 <= 0.311356f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= -0.008507f)
                                                        return 5;
                                                    else
                                                        return 4;
                                        else
                                            if (actual_vx <= 0.007896f)
                                                if (sensor2 <= 0.316833f)
                                                    if (sensor1 <= 0.850114f)
                                                        return 5;
                                                    else
                                                        return 4;
                                                else
                                                    if (sensor0 <= 0.284995f)
                                                        return 4;
                                                    else
                                                        return 10;
                                            else
                                                if (sensor0 <= 0.315157f)
                                                    if (sensor0 <= 0.297908f)
                                                        return 10;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor0 <= 0.320928f)
                                                        return 4;
                                                    else
                                                        return 4;
                                    else
                                        if (last_action_state <= 0.650000f)
                                            if (sensor2 <= 0.303209f)
                                                if (actual_vx <= 0.001943f)
                                                    if (sensor3 <= 0.407019f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor1 <= 0.845046f)
                                                        return 4;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vz <= 0.957972f)
                                                    if (sensor1 <= 0.849497f)
                                                        return 7;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= 0.067046f)
                                                        return 7;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor0 <= 0.284312f)
                                                if (actual_vz <= 1.093121f)
                                                    return 0;
                                                else
                                                    if (last_action_state <= 0.800000f)
                                                        return 10;
                                                    else
                                                        return 2;
                                            else
                                                if (sensor5 <= 0.523307f)
                                                    if (sensor5 <= 0.522602f)
                                                        return 2;
                                                    else
                                                        return 8;
                                                else
                                                    if (last_action_state <= 0.850000f)
                                                        return 5;
                                                    else
                                                        return 5;
            else
                if (sensor5 <= 0.514523f)
                    if (sensor2 <= 0.346825f)
                        if (actual_vz <= 0.942524f)
                            if (actual_vx <= 0.005288f)
                                if (actual_vz <= 0.796609f)
                                    if (actual_vx <= -0.011637f)
                                        if (actual_omega <= 0.097806f)
                                            if (actual_vz <= 0.762931f)
                                                if (sensor1 <= 0.810069f)
                                                    return 10;
                                                else
                                                    return 9;
                                            else
                                                return 7;
                                        else
                                            return 9;
                                    else
                                        if (actual_vx <= -0.007143f)
                                            return 6;
                                        else
                                            if (sensor1 <= 0.812855f)
                                                if (last_action_state <= 0.650000f)
                                                    return 9;
                                                else
                                                    return 5;
                                            else
                                                if (sensor5 <= 0.504610f)
                                                    return 7;
                                                else
                                                    return 6;
                                else
                                    if (sensor0 <= 0.335815f)
                                        if (actual_vz <= 0.930682f)
                                            if (actual_omega <= 0.020590f)
                                                if (actual_vz <= 0.912843f)
                                                    if (sensor5 <= 0.356340f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor4 <= 0.849524f)
                                                        return 6;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor2 <= 0.338675f)
                                                    if (sensor3 <= 0.309277f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vz <= 0.925988f)
                                                        return 6;
                                                    else
                                                        return 6;
                                        else
                                            if (sensor1 <= 0.851092f)
                                                if (sensor5 <= 0.512353f)
                                                    return 6;
                                                else
                                                    return 3;
                                            else
                                                if (actual_vx <= 0.001939f)
                                                    if (actual_omega <= 0.014598f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vx <= 0.002819f)
                                                        return 5;
                                                    else
                                                        return 2;
                                    else
                                        if (actual_vx <= 0.000218f)
                                            if (sensor0 <= 0.335879f)
                                                return 5;
                                            else
                                                if (actual_vz <= 0.916539f)
                                                    if (sensor2 <= 0.336498f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_omega <= 0.027113f)
                                                        return 6;
                                                    else
                                                        return 6;
                                        else
                                            if (sensor5 <= 0.277636f)
                                                if (sensor2 <= 0.327804f)
                                                    return 5;
                                                else
                                                    return 6;
                                            else
                                                return 5;
                            else
                                if (sensor4 <= 0.894221f)
                                    return 4;
                                else
                                    if (sensor4 <= 0.907309f)
                                        return 5;
                                    else
                                        if (sensor1 <= 0.848007f)
                                            if (sensor3 <= 0.495342f)
                                                if (actual_vx <= 0.006654f)
                                                    return 10;
                                                else
                                                    if (actual_vz <= 0.875445f)
                                                        return 5;
                                                    else
                                                        return 0;
                                            else
                                                if (sensor2 <= 0.329162f)
                                                    return 6;
                                                else
                                                    return 4;
                                        else
                                            return 4;
                        else
                            if (actual_omega <= 0.017936f)
                                if (actual_vx <= 0.007767f)
                                    if (sensor2 <= 0.326715f)
                                        if (sensor2 <= 0.325763f)
                                            if (sensor4 <= 0.882020f)
                                                if (sensor0 <= 0.336406f)
                                                    if (actual_omega <= 0.005889f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= -0.043503f)
                                                        return 6;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_omega <= 0.009728f)
                                                    return 6;
                                                else
                                                    return 7;
                                        else
                                            return 5;
                                    else
                                        if (actual_vz <= 0.988254f)
                                            return 7;
                                        else
                                            if (sensor3 <= 0.281512f)
                                                return 5;
                                            else
                                                if (actual_vx <= -0.000035f)
                                                    if (actual_vx <= -0.000103f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    return 6;
                                else
                                    if (sensor3 <= 0.310878f)
                                        if (sensor5 <= 0.357340f)
                                            return 9;
                                        else
                                            return 5;
                                    else
                                        return 10;
                            else
                                if (sensor2 <= 0.328441f)
                                    if (sensor3 <= 0.334668f)
                                        if (sensor5 <= 0.490712f)
                                            if (actual_omega <= 0.019263f)
                                                return 6;
                                            else
                                                if (sensor1 <= 0.838982f)
                                                    if (actual_vx <= 0.002047f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor4 <= 0.849902f)
                                                        return 6;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor0 <= 0.280446f)
                                                return 7;
                                            else
                                                if (sensor3 <= 0.285073f)
                                                    return 6;
                                                else
                                                    if (sensor3 <= 0.292287f)
                                                        return 5;
                                                    else
                                                        return 4;
                                    else
                                        if (sensor3 <= 0.338515f)
                                            if (actual_omega <= 0.027695f)
                                                if (actual_omega <= 0.021747f)
                                                    return 6;
                                                else
                                                    return 5;
                                            else
                                                return 6;
                                        else
                                            if (sensor5 <= 0.269425f)
                                                if (sensor1 <= 0.871870f)
                                                    if (sensor0 <= 0.313581f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor4 <= 0.850112f)
                                                        return 5;
                                                    else
                                                        return 6;
                                            else
                                                if (actual_vz <= 0.997614f)
                                                    if (sensor5 <= 0.470261f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    return 5;
                                else
                                    if (actual_vx <= -0.001577f)
                                        if (sensor0 <= 0.336664f)
                                            if (sensor3 <= 0.341604f)
                                                if (sensor4 <= 0.852390f)
                                                    return 5;
                                                else
                                                    if (sensor2 <= 0.335197f)
                                                        return 6;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor1 <= 0.875992f)
                                                    if (actual_vz <= 1.077392f)
                                                        return 6;
                                                    else
                                                        return 7;
                                                else
                                                    return 6;
                                        else
                                            if (sensor4 <= 0.853562f)
                                                if (sensor1 <= 0.877830f)
                                                    return 5;
                                                else
                                                    if (actual_omega <= 0.049071f)
                                                        return 6;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor2 <= 0.345439f)
                                                    return 5;
                                                else
                                                    if (sensor2 <= 0.345881f)
                                                        return 6;
                                                    else
                                                        return 5;
                                    else
                                        if (sensor2 <= 0.338670f)
                                            if (sensor3 <= 0.322446f)
                                                if (sensor4 <= 0.853608f)
                                                    if (sensor2 <= 0.334532f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor3 <= 0.272387f)
                                                        return 2;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor0 <= 0.361267f)
                                                    if (sensor4 <= 0.853596f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    return 5;
                                        else
                                            if (sensor2 <= 0.346670f)
                                                if (actual_omega <= 0.033115f)
                                                    if (actual_omega <= 0.019802f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor0 <= 0.344818f)
                                                        return 6;
                                                    else
                                                        return 6;
                                            else
                                                return 5;
                    else
                        if (sensor1 <= 0.851336f)
                            if (actual_vx <= 0.002531f)
                                if (actual_vz <= 0.930198f)
                                    if (sensor0 <= 0.267689f)
                                        if (actual_vx <= -0.004951f)
                                            if (actual_vx <= -0.006899f)
                                                if (actual_vz <= 0.819988f)
                                                    if (actual_omega <= 0.160804f)
                                                        return 9;
                                                    else
                                                        return 7;
                                                else
                                                    if (sensor3 <= 0.209413f)
                                                        return 6;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor2 <= 0.660959f)
                                                    if (sensor5 <= 0.345340f)
                                                        return 10;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_omega <= 0.094998f)
                                                        return 6;
                                                    else
                                                        return 6;
                                        else
                                            if (sensor1 <= 0.850451f)
                                                if (sensor0 <= 0.245453f)
                                                    if (actual_omega <= 0.063016f)
                                                        return 10;
                                                    else
                                                        return 10;
                                                else
                                                    if (actual_vz <= 0.919699f)
                                                        return 6;
                                                    else
                                                        return 10;
                                            else
                                                if (sensor3 <= 0.252158f)
                                                    return 9;
                                                else
                                                    return 7;
                                    else
                                        if (sensor0 <= 0.326600f)
                                            if (actual_vz <= 0.893759f)
                                                return 7;
                                            else
                                                if (sensor2 <= 0.638317f)
                                                    if (actual_vx <= 0.001915f)
                                                        return 6;
                                                    else
                                                        return 9;
                                                else
                                                    if (sensor3 <= 0.246485f)
                                                        return 6;
                                                    else
                                                        return 10;
                                        else
                                            if (actual_vz <= 0.903323f)
                                                return 6;
                                            else
                                                return 7;
                                else
                                    if (actual_vx <= -0.005199f)
                                        if (sensor4 <= 0.773932f)
                                            if (sensor3 <= 0.145076f)
                                                if (sensor1 <= 0.814969f)
                                                    return 5;
                                                else
                                                    return 7;
                                            else
                                                return 10;
                                        else
                                            if (sensor4 <= 0.815136f)
                                                if (actual_vz <= 0.934048f)
                                                    if (sensor1 <= 0.805571f)
                                                        return 9;
                                                    else
                                                        return 10;
                                                else
                                                    if (actual_vz <= 1.086630f)
                                                        return 6;
                                                    else
                                                        return 7;
                                            else
                                                if (last_action_state <= 0.850000f)
                                                    if (sensor2 <= 0.691631f)
                                                        return 6;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor0 <= 0.298420f)
                                                        return 6;
                                                    else
                                                        return 5;
                                    else
                                        if (last_action_state <= 0.750000f)
                                            if (sensor2 <= 0.620117f)
                                                if (sensor0 <= 0.306283f)
                                                    if (sensor3 <= 0.213504f)
                                                        return 6;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor4 <= 0.854625f)
                                                        return 6;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor2 <= 0.621738f)
                                                    return 10;
                                                else
                                                    if (sensor2 <= 0.628633f)
                                                        return 6;
                                                    else
                                                        return 6;
                                        else
                                            if (sensor2 <= 0.415410f)
                                                if (sensor1 <= 0.847522f)
                                                    return 5;
                                                else
                                                    return 6;
                                            else
                                                if (sensor5 <= 0.410445f)
                                                    if (actual_vz <= 0.935529f)
                                                        return 6;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor3 <= 0.194070f)
                                                        return 5;
                                                    else
                                                        return 7;
                            else
                                if (last_action_state <= 0.750000f)
                                    if (actual_omega <= 0.026756f)
                                        if (actual_vz <= 1.018452f)
                                            if (sensor3 <= 0.295050f)
                                                if (actual_vz <= 1.016024f)
                                                    if (sensor1 <= 0.830791f)
                                                        return 10;
                                                    else
                                                        return 6;
                                                else
                                                    return 8;
                                            else
                                                return 9;
                                        else
                                            return 10;
                                    else
                                        if (sensor0 <= 0.296912f)
                                            if (actual_vx <= 0.006799f)
                                                if (sensor4 <= 0.881822f)
                                                    if (sensor1 <= 0.839450f)
                                                        return 6;
                                                    else
                                                        return 7;
                                                else
                                                    return 7;
                                            else
                                                if (sensor0 <= 0.259751f)
                                                    return 6;
                                                else
                                                    return 9;
                                        else
                                            if (sensor2 <= 0.370978f)
                                                return 5;
                                            else
                                                if (last_action_state <= 0.550000f)
                                                    if (actual_omega <= 0.069277f)
                                                        return 10;
                                                    else
                                                        return 5;
                                                else
                                                    if (last_action_state <= 0.650000f)
                                                        return 4;
                                                    else
                                                        return 7;
                                else
                                    if (sensor4 <= 0.877085f)
                                        return 5;
                                    else
                                        if (sensor4 <= 0.895394f)
                                            if (sensor2 <= 0.361939f)
                                                return 4;
                                            else
                                                return 5;
                                        else
                                            return 5;
                        else
                            if (sensor2 <= 0.377152f)
                                if (actual_omega <= 0.035790f)
                                    if (actual_vx <= 0.001112f)
                                        if (sensor2 <= 0.376588f)
                                            if (actual_vz <= 1.076587f)
                                                if (last_action_state <= 0.550000f)
                                                    if (sensor0 <= 0.327367f)
                                                        return 6;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vx <= -0.002709f)
                                                        return 5;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor0 <= 0.337228f)
                                                    if (sensor0 <= 0.326842f)
                                                        return 6;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vx <= -0.001018f)
                                                        return 5;
                                                    else
                                                        return 6;
                                        else
                                            return 5;
                                    else
                                        return 5;
                                else
                                    if (sensor1 <= 0.889316f)
                                        if (sensor0 <= 0.330500f)
                                            if (actual_omega <= 0.183921f)
                                                if (sensor2 <= 0.351644f)
                                                    if (actual_omega <= 0.052785f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vx <= 0.000660f)
                                                        return 6;
                                                    else
                                                        return 7;
                                            else
                                                if (sensor5 <= 0.488885f)
                                                    return 7;
                                                else
                                                    if (actual_omega <= 0.260345f)
                                                        return 6;
                                                    else
                                                        return 7;
                                        else
                                            if (last_action_state <= 0.650000f)
                                                if (sensor3 <= 0.322772f)
                                                    if (sensor1 <= 0.883844f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor0 <= 0.336305f)
                                                        return 6;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor0 <= 0.336131f)
                                                    return 6;
                                                else
                                                    return 5;
                                    else
                                        if (sensor2 <= 0.377117f)
                                            if (sensor4 <= 0.803551f)
                                                return 5;
                                            else
                                                if (actual_omega <= 0.035822f)
                                                    return 5;
                                                else
                                                    if (actual_omega <= 0.053185f)
                                                        return 6;
                                                    else
                                                        return 6;
                                        else
                                            return 5;
                            else
                                if (sensor2 <= 0.588373f)
                                    if (sensor1 <= 0.891610f)
                                        if (sensor0 <= 0.334544f)
                                            if (actual_omega <= 0.270177f)
                                                if (sensor1 <= 0.891606f)
                                                    if (actual_vz <= 0.799641f)
                                                        return 9;
                                                    else
                                                        return 6;
                                                else
                                                    return 9;
                                            else
                                                return 7;
                                        else
                                            if (last_action_state <= 0.650000f)
                                                if (sensor5 <= 0.330685f)
                                                    if (actual_vz <= 1.081014f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor0 <= 0.338592f)
                                                        return 5;
                                                    else
                                                        return 6;
                                            else
                                                return 5;
                                    else
                                        if (sensor5 <= 0.420204f)
                                            if (last_action_state <= 0.950000f)
                                                if (actual_vz <= 1.080869f)
                                                    if (sensor5 <= 0.313491f)
                                                        return 6;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor2 <= 0.457969f)
                                                        return 6;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_omega <= 0.099908f)
                                                    return 6;
                                                else
                                                    if (actual_vz <= 1.078255f)
                                                        return 5;
                                                    else
                                                        return 6;
                                        else
                                            return 9;
                                else
                                    if (actual_vx <= -0.004979f)
                                        if (sensor4 <= 0.810136f)
                                            return 7;
                                        else
                                            if (sensor3 <= 0.327884f)
                                                if (sensor0 <= 0.223126f)
                                                    return 9;
                                                else
                                                    if (actual_omega <= 0.098178f)
                                                        return 6;
                                                    else
                                                        return 6;
                                            else
                                                return 9;
                                    else
                                        if (sensor2 <= 0.589021f)
                                            return 10;
                                        else
                                            if (sensor2 <= 0.674777f)
                                                if (actual_vx <= -0.004972f)
                                                    if (actual_omega <= 0.066128f)
                                                        return 7;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor0 <= 0.290861f)
                                                        return 6;
                                                    else
                                                        return 9;
                                            else
                                                if (actual_vz <= 0.921488f)
                                                    if (sensor0 <= 0.231328f)
                                                        return 6;
                                                    else
                                                        return 9;
                                                else
                                                    return 10;
                else
                    if (last_action_state <= 0.850000f)
                        if (sensor2 <= 0.356530f)
                            if (actual_vz <= 1.030618f)
                                if (last_action_state <= 0.350000f)
                                    if (sensor2 <= 0.329763f)
                                        if (sensor1 <= 0.853736f)
                                            if (sensor2 <= 0.324804f)
                                                if (actual_omega <= 0.082050f)
                                                    return 9;
                                                else
                                                    if (sensor3 <= 0.309370f)
                                                        return 6;
                                                    else
                                                        return 9;
                                            else
                                                if (sensor0 <= 0.281577f)
                                                    return 6;
                                                else
                                                    return 10;
                                        else
                                            if (sensor5 <= 0.633294f)
                                                return 7;
                                            else
                                                return 2;
                                    else
                                        if (sensor1 <= 0.847685f)
                                            if (actual_omega <= 0.061682f)
                                                return 8;
                                            else
                                                return 9;
                                        else
                                            if (actual_omega <= 0.129789f)
                                                if (actual_omega <= 0.107607f)
                                                    if (actual_vx <= -0.001596f)
                                                        return 9;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor5 <= 0.665740f)
                                                        return 7;
                                                    else
                                                        return 10;
                                            else
                                                return 8;
                                else
                                    if (sensor5 <= 0.685192f)
                                        if (actual_vx <= -0.003010f)
                                            if (actual_omega <= 0.205864f)
                                                if (sensor4 <= 0.851219f)
                                                    if (sensor3 <= 0.261301f)
                                                        return 6;
                                                    else
                                                        return 7;
                                                else
                                                    if (actual_vz <= 0.950111f)
                                                        return 6;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vx <= -0.018816f)
                                                    if (actual_vz <= 1.015808f)
                                                        return 6;
                                                    else
                                                        return 7;
                                                else
                                                    return 7;
                                        else
                                            if (sensor0 <= 0.278244f)
                                                if (sensor1 <= 0.850334f)
                                                    if (sensor0 <= 0.255473f)
                                                        return 7;
                                                    else
                                                        return 6;
                                                else
                                                    if (last_action_state <= 0.550000f)
                                                        return 6;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor5 <= 0.617379f)
                                                    if (actual_vx <= -0.001900f)
                                                        return 7;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor3 <= 0.241871f)
                                                        return 5;
                                                    else
                                                        return 7;
                                    else
                                        if (actual_vx <= -0.001837f)
                                            if (sensor5 <= 0.686451f)
                                                return 2;
                                            else
                                                if (sensor4 <= 0.811548f)
                                                    return 7;
                                                else
                                                    if (sensor5 <= 0.695478f)
                                                        return 9;
                                                    else
                                                        return 7;
                                        else
                                            if (sensor5 <= 0.690496f)
                                                return 9;
                                            else
                                                if (sensor2 <= 0.343510f)
                                                    if (actual_omega <= 0.119773f)
                                                        return 7;
                                                    else
                                                        return 9;
                                                else
                                                    if (sensor2 <= 0.354478f)
                                                        return 6;
                                                    else
                                                        return 7;
                            else
                                if (last_action_state <= 0.650000f)
                                    if (sensor2 <= 0.337808f)
                                        if (sensor4 <= 0.841587f)
                                            if (sensor2 <= 0.329135f)
                                                if (actual_vx <= -0.002845f)
                                                    if (sensor3 <= 0.221713f)
                                                        return 0;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor1 <= 0.845911f)
                                                        return 2;
                                                    else
                                                        return 9;
                                            else
                                                if (sensor5 <= 0.669482f)
                                                    return 7;
                                                else
                                                    if (actual_vx <= -0.003160f)
                                                        return 7;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor3 <= 0.245442f)
                                                if (last_action_state <= 0.550000f)
                                                    if (sensor4 <= 0.866926f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    return 5;
                                            else
                                                if (sensor1 <= 0.842828f)
                                                    if (actual_vz <= 1.092967f)
                                                        return 6;
                                                    else
                                                        return 7;
                                                else
                                                    if (actual_vx <= -0.003388f)
                                                        return 6;
                                                    else
                                                        return 5;
                                    else
                                        if (actual_vx <= 0.001860f)
                                            if (sensor1 <= 0.841704f)
                                                if (actual_vz <= 1.099415f)
                                                    if (actual_omega <= 0.100037f)
                                                        return 10;
                                                    else
                                                        return 5;
                                                else
                                                    return 6;
                                            else
                                                if (sensor3 <= 0.303228f)
                                                    if (sensor5 <= 0.695883f)
                                                        return 6;
                                                    else
                                                        return 0;
                                                else
                                                    return 7;
                                        else
                                            if (actual_omega <= 0.061983f)
                                                return 6;
                                            else
                                                return 10;
                                else
                                    if (actual_omega <= 0.099601f)
                                        if (sensor5 <= 0.619392f)
                                            if (sensor3 <= 0.291545f)
                                                return 7;
                                            else
                                                if (sensor4 <= 0.873828f)
                                                    return 9;
                                                else
                                                    if (sensor5 <= 0.543005f)
                                                        return 4;
                                                    else
                                                        return 6;
                                        else
                                            return 10;
                                    else
                                        if (sensor2 <= 0.348827f)
                                            if (actual_omega <= 0.117121f)
                                                if (sensor5 <= 0.601057f)
                                                    if (actual_vz <= 1.082984f)
                                                        return 0;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor2 <= 0.333588f)
                                                        return 6;
                                                    else
                                                        return 3;
                                            else
                                                if (actual_vx <= -0.005197f)
                                                    if (actual_vz <= 1.073016f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor5 <= 0.518641f)
                                                        return 6;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor3 <= 0.244713f)
                                                if (sensor2 <= 0.349140f)
                                                    return 6;
                                                else
                                                    if (sensor3 <= 0.240666f)
                                                        return 5;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor0 <= 0.259160f)
                                                    return 7;
                                                else
                                                    return 8;
                        else
                            if (last_action_state <= 0.550000f)
                                if (actual_vx <= -0.003118f)
                                    if (actual_omega <= 0.201071f)
                                        if (sensor1 <= 0.824690f)
                                            if (sensor3 <= 0.218429f)
                                                if (actual_vx <= -0.007538f)
                                                    return 6;
                                                else
                                                    if (sensor3 <= 0.193031f)
                                                        return 6;
                                                    else
                                                        return 10;
                                            else
                                                if (actual_omega <= 0.147140f)
                                                    if (actual_vz <= 0.941825f)
                                                        return 9;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vx <= -0.005338f)
                                                        return 10;
                                                    else
                                                        return 6;
                                        else
                                            if (sensor1 <= 0.847976f)
                                                if (sensor3 <= 0.213613f)
                                                    if (actual_vz <= 1.066388f)
                                                        return 6;
                                                    else
                                                        return 7;
                                                else
                                                    if (actual_omega <= 0.179269f)
                                                        return 6;
                                                    else
                                                        return 6;
                                            else
                                                if (actual_vz <= 0.947019f)
                                                    return 7;
                                                else
                                                    return 10;
                                    else
                                        if (sensor5 <= 0.752185f)
                                            if (sensor5 <= 0.746048f)
                                                if (sensor1 <= 0.811695f)
                                                    if (sensor1 <= 0.802786f)
                                                        return 9;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor1 <= 0.839674f)
                                                        return 7;
                                                    else
                                                        return 6;
                                            else
                                                return 6;
                                        else
                                            return 7;
                                else
                                    if (sensor1 <= 0.827832f)
                                        if (last_action_state <= 0.350000f)
                                            if (actual_omega <= 0.092641f)
                                                if (sensor2 <= 0.458616f)
                                                    return 10;
                                                else
                                                    return 9;
                                            else
                                                return 10;
                                        else
                                            if (actual_omega <= 0.115395f)
                                                if (actual_vx <= 0.001155f)
                                                    return 9;
                                                else
                                                    return 10;
                                            else
                                                if (sensor2 <= 0.491801f)
                                                    if (actual_vx <= -0.001471f)
                                                        return 10;
                                                    else
                                                        return 10;
                                                else
                                                    if (actual_vx <= -0.002260f)
                                                        return 6;
                                                    else
                                                        return 9;
                                    else
                                        if (last_action_state <= 0.250000f)
                                            if (sensor4 <= 0.847060f)
                                                if (sensor4 <= 0.737933f)
                                                    return 7;
                                                else
                                                    if (actual_vx <= -0.001811f)
                                                        return 9;
                                                    else
                                                        return 10;
                                            else
                                                return 9;
                                        else
                                            if (sensor2 <= 0.417955f)
                                                if (sensor1 <= 0.833418f)
                                                    if (actual_vz <= 0.928189f)
                                                        return 9;
                                                    else
                                                        return 7;
                                                else
                                                    if (actual_omega <= 0.123014f)
                                                        return 10;
                                                    else
                                                        return 6;
                                            else
                                                if (actual_vx <= -0.002998f)
                                                    return 9;
                                                else
                                                    if (actual_vz <= 0.937218f)
                                                        return 6;
                                                    else
                                                        return 6;
                            else
                                if (actual_vx <= -0.002183f)
                                    if (actual_vz <= 0.959688f)
                                        if (actual_vz <= 0.932330f)
                                            if (actual_omega <= 0.187235f)
                                                if (sensor0 <= 0.166708f)
                                                    if (sensor4 <= 0.731001f)
                                                        return 6;
                                                    else
                                                        return 10;
                                                else
                                                    if (actual_vz <= 0.923851f)
                                                        return 6;
                                                    else
                                                        return 6;
                                            else
                                                if (actual_omega <= 0.193239f)
                                                    if (actual_vz <= 0.928921f)
                                                        return 7;
                                                    else
                                                        return 8;
                                                else
                                                    if (actual_vx <= -0.006369f)
                                                        return 6;
                                                    else
                                                        return 7;
                                        else
                                            if (sensor2 <= 0.424485f)
                                                if (sensor2 <= 0.374280f)
                                                    if (sensor5 <= 0.711899f)
                                                        return 6;
                                                    else
                                                        return 7;
                                                else
                                                    if (sensor2 <= 0.375658f)
                                                        return 7;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor2 <= 0.495995f)
                                                    if (actual_omega <= 0.160430f)
                                                        return 6;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_omega <= 0.179303f)
                                                        return 9;
                                                    else
                                                        return 7;
                                    else
                                        if (last_action_state <= 0.650000f)
                                            if (sensor2 <= 0.423183f)
                                                if (sensor5 <= 0.683219f)
                                                    if (sensor1 <= 0.825949f)
                                                        return 7;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vz <= 1.071779f)
                                                        return 7;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor1 <= 0.813974f)
                                                    if (sensor1 <= 0.812885f)
                                                        return 6;
                                                    else
                                                        return 7;
                                                else
                                                    if (sensor5 <= 0.776598f)
                                                        return 6;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor3 <= 0.213427f)
                                                if (sensor1 <= 0.804484f)
                                                    if (sensor2 <= 0.479457f)
                                                        return 6;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor5 <= 0.763846f)
                                                        return 7;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor2 <= 0.393257f)
                                                    if (sensor3 <= 0.266103f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vx <= -0.002820f)
                                                        return 6;
                                                    else
                                                        return 7;
                                else
                                    if (sensor1 <= 0.838157f)
                                        if (sensor4 <= 0.755172f)
                                            if (actual_omega <= 0.145887f)
                                                if (sensor2 <= 0.404941f)
                                                    return 5;
                                                else
                                                    if (actual_vx <= 0.001405f)
                                                        return 10;
                                                    else
                                                        return 9;
                                            else
                                                if (sensor1 <= 0.823389f)
                                                    return 6;
                                                else
                                                    if (last_action_state <= 0.650000f)
                                                        return 2;
                                                    else
                                                        return 10;
                                        else
                                            if (actual_omega <= 0.142024f)
                                                if (sensor5 <= 0.711477f)
                                                    if (sensor5 <= 0.697621f)
                                                        return 6;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor5 <= 0.740693f)
                                                        return 9;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor3 <= 0.312800f)
                                                    if (sensor1 <= 0.837507f)
                                                        return 6;
                                                    else
                                                        return 7;
                                                else
                                                    return 9;
                                    else
                                        if (sensor0 <= 0.247350f)
                                            if (sensor2 <= 0.394717f)
                                                if (sensor2 <= 0.381624f)
                                                    if (sensor3 <= 0.258090f)
                                                        return 6;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vx <= -0.000521f)
                                                        return 6;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor2 <= 0.405016f)
                                                    return 7;
                                                else
                                                    return 6;
                                        else
                                            if (sensor4 <= 0.861342f)
                                                if (sensor3 <= 0.300311f)
                                                    if (sensor0 <= 0.248204f)
                                                        return 2;
                                                    else
                                                        return 7;
                                                else
                                                    return 5;
                                            else
                                                if (sensor0 <= 0.251134f)
                                                    if (actual_omega <= 0.102453f)
                                                        return 10;
                                                    else
                                                        return 7;
                                                else
                                                    if (actual_omega <= 0.122646f)
                                                        return 6;
                                                    else
                                                        return 5;
                    else
                        if (actual_omega <= 0.216773f)
                            if (sensor2 <= 0.367598f)
                                if (actual_vx <= 0.004129f)
                                    if (sensor4 <= 0.803135f)
                                        if (sensor3 <= 0.211181f)
                                            if (sensor1 <= 0.843112f)
                                                return 5;
                                            else
                                                return 7;
                                        else
                                            return 0;
                                    else
                                        if (actual_vx <= -0.001989f)
                                            if (sensor3 <= 0.183890f)
                                                return 1;
                                            else
                                                if (sensor4 <= 0.921717f)
                                                    if (sensor1 <= 0.849475f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor5 <= 0.603048f)
                                                        return 5;
                                                    else
                                                        return 0;
                                        else
                                            if (sensor4 <= 0.858640f)
                                                if (sensor0 <= 0.279224f)
                                                    if (sensor2 <= 0.356650f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor3 <= 0.254190f)
                                                        return 1;
                                                    else
                                                        return 4;
                                            else
                                                if (sensor2 <= 0.319877f)
                                                    return 8;
                                                else
                                                    if (sensor4 <= 0.951485f)
                                                        return 5;
                                                    else
                                                        return 4;
                                else
                                    if (sensor1 <= 0.849503f)
                                        return 6;
                                    else
                                        return 2;
                            else
                                if (sensor3 <= 0.251223f)
                                    if (actual_vz <= 0.964013f)
                                        if (sensor5 <= 0.735148f)
                                            if (sensor1 <= 0.821698f)
                                                if (last_action_state <= 0.950000f)
                                                    if (actual_vz <= 0.937434f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vx <= -0.004386f)
                                                        return 6;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vz <= 0.960382f)
                                                    if (last_action_state <= 0.950000f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    return 6;
                                        else
                                            if (sensor5 <= 0.777084f)
                                                if (sensor3 <= 0.170792f)
                                                    if (actual_vx <= -0.003902f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    return 5;
                                            else
                                                if (sensor3 <= 0.158160f)
                                                    return 4;
                                                else
                                                    if (sensor1 <= 0.804209f)
                                                        return 7;
                                                    else
                                                        return 3;
                                    else
                                        if (sensor0 <= 0.187014f)
                                            if (sensor2 <= 0.453012f)
                                                if (sensor3 <= 0.186793f)
                                                    return 0;
                                                else
                                                    if (last_action_state <= 0.950000f)
                                                        return 6;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_omega <= 0.216429f)
                                                    if (sensor0 <= 0.182571f)
                                                        return 5;
                                                    else
                                                        return 3;
                                                else
                                                    if (sensor4 <= 0.795603f)
                                                        return 0;
                                                    else
                                                        return 7;
                                        else
                                            if (sensor5 <= 0.603001f)
                                                if (actual_vz <= 1.074751f)
                                                    if (sensor5 <= 0.600149f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    return 6;
                                            else
                                                if (sensor3 <= 0.248264f)
                                                    if (actual_vz <= 1.068552f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor5 <= 0.633287f)
                                                        return 7;
                                                    else
                                                        return 5;
                                else
                                    if (last_action_state <= 0.950000f)
                                        if (actual_omega <= 0.180060f)
                                            if (sensor1 <= 0.829726f)
                                                if (sensor4 <= 0.838149f)
                                                    if (actual_vz <= 1.079589f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor5 <= 0.564375f)
                                                        return 6;
                                                    else
                                                        return 7;
                                            else
                                                if (sensor4 <= 0.852766f)
                                                    if (sensor3 <= 0.261510f)
                                                        return 2;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor3 <= 0.298646f)
                                                        return 6;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor0 <= 0.229530f)
                                                if (sensor4 <= 0.820240f)
                                                    if (sensor4 <= 0.807427f)
                                                        return 6;
                                                    else
                                                        return 3;
                                                else
                                                    return 6;
                                            else
                                                if (actual_vz <= 1.067855f)
                                                    return 5;
                                                else
                                                    return 6;
                                    else
                                        if (sensor4 <= 0.874216f)
                                            if (sensor3 <= 0.284665f)
                                                if (sensor5 <= 0.650584f)
                                                    if (actual_vz <= 1.102574f)
                                                        return 5;
                                                    else
                                                        return 7;
                                                else
                                                    if (actual_omega <= 0.197614f)
                                                        return 5;
                                                    else
                                                        return 0;
                                            else
                                                if (sensor0 <= 0.225832f)
                                                    if (sensor1 <= 0.832309f)
                                                        return 5;
                                                    else
                                                        return 7;
                                                else
                                                    if (sensor2 <= 0.374198f)
                                                        return 5;
                                                    else
                                                        return 6;
                                        else
                                            if (sensor1 <= 0.825675f)
                                                return 6;
                                            else
                                                if (actual_vx <= -0.001875f)
                                                    return 8;
                                                else
                                                    if (sensor5 <= 0.552949f)
                                                        return 9;
                                                    else
                                                        return 0;
                        else
                            if (actual_vz <= 0.946362f)
                                return 6;
                            else
                                if (actual_vx <= -0.004699f)
                                    if (sensor0 <= 0.161048f)
                                        if (last_action_state <= 0.950000f)
                                            return 5;
                                        else
                                            return 0;
                                    else
                                        if (sensor3 <= 0.209433f)
                                            if (sensor4 <= 0.840299f)
                                                return 5;
                                            else
                                                if (last_action_state <= 0.950000f)
                                                    return 7;
                                                else
                                                    return 5;
                                        else
                                            if (sensor3 <= 0.210758f)
                                                if (sensor2 <= 0.426578f)
                                                    return 5;
                                                else
                                                    return 6;
                                            else
                                                if (sensor1 <= 0.807114f)
                                                    if (actual_vx <= -0.006525f)
                                                        return 5;
                                                    else
                                                        return 7;
                                                else
                                                    if (actual_omega <= 0.281211f)
                                                        return 5;
                                                    else
                                                        return 5;
                                else
                                    if (sensor4 <= 0.704462f)
                                        return 4;
                                    else
                                        if (actual_vx <= -0.004319f)
                                            if (sensor5 <= 0.734825f)
                                                if (sensor3 <= 0.238560f)
                                                    return 5;
                                                else
                                                    return 7;
                                            else
                                                if (sensor3 <= 0.194794f)
                                                    return 6;
                                                else
                                                    if (last_action_state <= 0.950000f)
                                                        return 4;
                                                    else
                                                        return 2;
                                        else
                                            if (actual_vz <= 1.069230f)
                                                if (actual_vz <= 1.068029f)
                                                    if (actual_omega <= 0.218902f)
                                                        return 3;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= 0.237707f)
                                                        return 4;
                                                    else
                                                        return 5;
                                            else
                                                return 7;
    }
}
