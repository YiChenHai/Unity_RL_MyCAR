// 决策树规则库 - 自动生成（离散动作分类）
// 数据样本数: 300000
// 准确率: 0.7536
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
        if (sensor1 <= 0.797909f)
            if (sensor2 <= 0.414516f)
                if (last_action_state <= 0.350000f)
                    if (sensor2 <= 0.058594f)
                        if (actual_vx <= 0.009453f)
                            if (actual_vx <= 0.003627f)
                                if (sensor2 <= 0.030946f)
                                    if (actual_vx <= -0.002146f)
                                        if (sensor2 <= 0.018587f)
                                            if (sensor2 <= 0.018183f)
                                                if (actual_omega <= -0.180895f)
                                                    return 1;
                                                else
                                                    if (actual_vx <= -0.005689f)
                                                        return 1;
                                                    else
                                                        return 0;
                                            else
                                                return 1;
                                        else
                                            if (last_action_state <= 0.250000f)
                                                if (sensor3 <= 0.803836f)
                                                    return 0;
                                                else
                                                    if (sensor3 <= 0.806839f)
                                                        return 1;
                                                    else
                                                        return 0;
                                            else
                                                if (sensor5 <= 0.013097f)
                                                    return 5;
                                                else
                                                    return 1;
                                    else
                                        if (actual_vz <= 1.301612f)
                                            if (actual_omega <= -0.232146f)
                                                if (sensor5 <= 0.020506f)
                                                    if (actual_vz <= 1.172233f)
                                                        return 0;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_omega <= -0.239234f)
                                                        return 0;
                                                    else
                                                        return 1;
                                            else
                                                if (sensor5 <= 0.028508f)
                                                    if (sensor5 <= 0.021174f)
                                                        return 0;
                                                    else
                                                        return 0;
                                                else
                                                    if (sensor1 <= 0.359005f)
                                                        return 1;
                                                    else
                                                        return 0;
                                        else
                                            if (sensor0 <= 0.820008f)
                                                return 1;
                                            else
                                                return 5;
                                else
                                    if (actual_vx <= -0.010619f)
                                        if (sensor1 <= 0.399664f)
                                            if (sensor4 <= 0.422639f)
                                                return 0;
                                            else
                                                return 1;
                                        else
                                            if (actual_omega <= -0.108829f)
                                                if (sensor0 <= 0.526538f)
                                                    return 1;
                                                else
                                                    return 0;
                                            else
                                                return 0;
                                    else
                                        if (sensor0 <= 0.791888f)
                                            if (actual_vz <= 1.293496f)
                                                if (last_action_state <= 0.150000f)
                                                    if (sensor0 <= 0.789880f)
                                                        return 0;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_vx <= -0.001356f)
                                                        return 1;
                                                    else
                                                        return 0;
                                            else
                                                if (sensor1 <= 0.518714f)
                                                    if (sensor2 <= 0.037984f)
                                                        return 1;
                                                    else
                                                        return 0;
                                                else
                                                    return 1;
                                        else
                                            if (actual_vx <= -0.000021f)
                                                if (sensor4 <= 0.537760f)
                                                    if (sensor4 <= 0.376131f)
                                                        return 0;
                                                    else
                                                        return 0;
                                                else
                                                    return 1;
                                            else
                                                if (sensor5 <= 0.016177f)
                                                    if (actual_vx <= 0.003414f)
                                                        return 1;
                                                    else
                                                        return 0;
                                                else
                                                    if (sensor4 <= 0.360085f)
                                                        return 0;
                                                    else
                                                        return 0;
                            else
                                if (sensor4 <= 0.357194f)
                                    if (last_action_state <= 0.050000f)
                                        if (actual_omega <= -0.272715f)
                                            if (sensor1 <= 0.492259f)
                                                if (sensor4 <= 0.293170f)
                                                    if (sensor3 <= 0.824593f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= -0.277581f)
                                                        return 0;
                                                    else
                                                        return 1;
                                            else
                                                return 5;
                                        else
                                            if (sensor5 <= 0.014967f)
                                                if (sensor2 <= 0.034358f)
                                                    if (actual_vx <= 0.008161f)
                                                        return 0;
                                                    else
                                                        return 0;
                                                else
                                                    if (sensor5 <= 0.012970f)
                                                        return 0;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vz <= 1.266572f)
                                                    if (actual_vz <= 1.259551f)
                                                        return 0;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_omega <= -0.175253f)
                                                        return 0;
                                                    else
                                                        return 1;
                                    else
                                        if (sensor2 <= 0.036055f)
                                            if (actual_vz <= 1.114038f)
                                                if (sensor4 <= 0.313671f)
                                                    if (actual_omega <= -0.247649f)
                                                        return 5;
                                                    else
                                                        return 0;
                                                else
                                                    if (actual_vz <= 1.110085f)
                                                        return 0;
                                                    else
                                                        return 1;
                                            else
                                                if (actual_omega <= -0.263263f)
                                                    if (sensor1 <= 0.346726f)
                                                        return 0;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vx <= 0.009136f)
                                                        return 0;
                                                    else
                                                        return 0;
                                        else
                                            if (actual_vx <= 0.007063f)
                                                if (actual_vx <= 0.004637f)
                                                    if (actual_vx <= 0.003862f)
                                                        return 1;
                                                    else
                                                        return 0;
                                                else
                                                    if (actual_vx <= 0.006958f)
                                                        return 5;
                                                    else
                                                        return 0;
                                            else
                                                if (sensor2 <= 0.041177f)
                                                    if (actual_vx <= 0.007116f)
                                                        return 3;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor1 <= 0.528958f)
                                                        return 0;
                                                    else
                                                        return 5;
                                else
                                    if (sensor1 <= 0.526441f)
                                        if (last_action_state <= 0.050000f)
                                            if (sensor2 <= 0.018413f)
                                                if (actual_vx <= 0.009193f)
                                                    if (sensor4 <= 0.360916f)
                                                        return 0;
                                                    else
                                                        return 0;
                                                else
                                                    return 5;
                                            else
                                                if (sensor5 <= 0.023094f)
                                                    if (actual_omega <= -0.221032f)
                                                        return 1;
                                                    else
                                                        return 0;
                                                else
                                                    if (actual_vx <= 0.009389f)
                                                        return 1;
                                                    else
                                                        return 0;
                                        else
                                            if (sensor4 <= 0.400399f)
                                                if (sensor2 <= 0.044014f)
                                                    if (actual_omega <= -0.255240f)
                                                        return 1;
                                                    else
                                                        return 0;
                                                else
                                                    if (actual_vz <= 1.078957f)
                                                        return 5;
                                                    else
                                                        return 1;
                                            else
                                                if (sensor4 <= 0.404075f)
                                                    if (sensor2 <= 0.039142f)
                                                        return 0;
                                                    else
                                                        return 0;
                                                else
                                                    if (actual_vx <= 0.004431f)
                                                        return 0;
                                                    else
                                                        return 0;
                                    else
                                        if (sensor5 <= 0.052149f)
                                            if (last_action_state <= 0.150000f)
                                                if (sensor5 <= 0.033453f)
                                                    if (sensor3 <= 0.846460f)
                                                        return 5;
                                                    else
                                                        return 4;
                                                else
                                                    if (sensor1 <= 0.560163f)
                                                        return 1;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor2 <= 0.045910f)
                                                    if (sensor1 <= 0.538003f)
                                                        return 1;
                                                    else
                                                        return 0;
                                                else
                                                    if (actual_vz <= 1.105936f)
                                                        return 5;
                                                    else
                                                        return 5;
                                        else
                                            if (actual_vx <= 0.005576f)
                                                if (sensor2 <= 0.048248f)
                                                    if (actual_vx <= 0.004623f)
                                                        return 1;
                                                    else
                                                        return 0;
                                                else
                                                    if (sensor4 <= 0.512915f)
                                                        return 0;
                                                    else
                                                        return 1;
                                            else
                                                if (sensor4 <= 0.502837f)
                                                    if (sensor4 <= 0.501542f)
                                                        return 1;
                                                    else
                                                        return 0;
                                                else
                                                    if (actual_vz <= 1.246929f)
                                                        return 1;
                                                    else
                                                        return 1;
                        else
                            if (actual_omega <= -0.269622f)
                                if (last_action_state <= 0.050000f)
                                    if (actual_omega <= -0.301381f)
                                        if (sensor5 <= 0.015381f)
                                            if (sensor2 <= 0.019844f)
                                                if (sensor5 <= 0.013776f)
                                                    if (actual_vz <= 1.242546f)
                                                        return 0;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor0 <= 0.835443f)
                                                        return 2;
                                                    else
                                                        return 1;
                                            else
                                                if (actual_omega <= -0.303938f)
                                                    if (sensor1 <= 0.433742f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vx <= 0.013672f)
                                                        return 0;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor0 <= 0.811303f)
                                                if (actual_vx <= 0.010797f)
                                                    if (sensor3 <= 0.842880f)
                                                        return 0;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor4 <= 0.467298f)
                                                        return 5;
                                                    else
                                                        return 1;
                                            else
                                                if (actual_omega <= -0.327248f)
                                                    if (sensor3 <= 0.815256f)
                                                        return 1;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vz <= 1.175110f)
                                                        return 5;
                                                    else
                                                        return 1;
                                    else
                                        if (sensor2 <= 0.032078f)
                                            if (actual_vz <= 1.213192f)
                                                if (actual_vx <= 0.012745f)
                                                    if (actual_vx <= 0.010528f)
                                                        return 5;
                                                    else
                                                        return 0;
                                                else
                                                    if (sensor5 <= 0.017028f)
                                                        return 5;
                                                    else
                                                        return 1;
                                            else
                                                if (actual_vz <= 1.296875f)
                                                    if (sensor2 <= 0.017848f)
                                                        return 0;
                                                    else
                                                        return 1;
                                                else
                                                    return 0;
                                        else
                                            if (sensor4 <= 0.301096f)
                                                if (actual_omega <= -0.281301f)
                                                    if (sensor0 <= 0.848313f)
                                                        return 5;
                                                    else
                                                        return 4;
                                                else
                                                    if (sensor4 <= 0.296321f)
                                                        return 1;
                                                    else
                                                        return 0;
                                            else
                                                if (sensor5 <= 0.014452f)
                                                    if (actual_omega <= -0.271746f)
                                                        return 0;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor4 <= 0.353626f)
                                                        return 5;
                                                    else
                                                        return 1;
                                else
                                    if (sensor4 <= 0.456310f)
                                        if (actual_omega <= -0.319227f)
                                            if (sensor2 <= 0.023324f)
                                                if (sensor3 <= 0.851027f)
                                                    if (sensor5 <= 0.013963f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor1 <= 0.360031f)
                                                        return 4;
                                                    else
                                                        return 1;
                                            else
                                                if (sensor0 <= 0.856198f)
                                                    if (sensor3 <= 0.791770f)
                                                        return 0;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor5 <= 0.025170f)
                                                        return 1;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor1 <= 0.391011f)
                                                if (actual_vx <= 0.014448f)
                                                    if (sensor3 <= 0.869346f)
                                                        return 5;
                                                    else
                                                        return 0;
                                                else
                                                    if (sensor2 <= 0.025239f)
                                                        return 5;
                                                    else
                                                        return 1;
                                            else
                                                if (sensor5 <= 0.017873f)
                                                    if (actual_vx <= 0.013399f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor1 <= 0.534759f)
                                                        return 5;
                                                    else
                                                        return 5;
                                    else
                                        if (sensor0 <= 0.754390f)
                                            if (sensor4 <= 0.502757f)
                                                if (sensor3 <= 0.851222f)
                                                    return 5;
                                                else
                                                    if (actual_vx <= 0.009555f)
                                                        return 0;
                                                    else
                                                        return 1;
                                            else
                                                if (actual_omega <= -0.276189f)
                                                    if (actual_vz <= 1.224783f)
                                                        return 1;
                                                    else
                                                        return 5;
                                                else
                                                    return 5;
                                        else
                                            if (actual_vx <= 0.015448f)
                                                if (actual_vz <= 1.248782f)
                                                    if (sensor1 <= 0.419048f)
                                                        return 5;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor4 <= 0.470628f)
                                                        return 4;
                                                    else
                                                        return 1;
                                            else
                                                if (sensor1 <= 0.482971f)
                                                    if (actual_omega <= -0.290907f)
                                                        return 1;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor2 <= 0.057237f)
                                                        return 5;
                                                    else
                                                        return 0;
                            else
                                if (last_action_state <= 0.150000f)
                                    if (sensor4 <= 0.353629f)
                                        if (last_action_state <= 0.050000f)
                                            if (actual_vx <= 0.015710f)
                                                if (sensor0 <= 0.841875f)
                                                    if (sensor2 <= 0.044871f)
                                                        return 0;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vz <= 1.077186f)
                                                        return 0;
                                                    else
                                                        return 1;
                                            else
                                                if (sensor3 <= 0.824225f)
                                                    if (sensor2 <= 0.021715f)
                                                        return 4;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= -0.259770f)
                                                        return 5;
                                                    else
                                                        return 1;
                                        else
                                            if (sensor2 <= 0.026229f)
                                                if (actual_vx <= 0.014162f)
                                                    if (sensor5 <= 0.018912f)
                                                        return 0;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_vz <= 1.180949f)
                                                        return 5;
                                                    else
                                                        return 0;
                                            else
                                                if (actual_omega <= -0.245071f)
                                                    if (sensor4 <= 0.303865f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor1 <= 0.416067f)
                                                        return 0;
                                                    else
                                                        return 5;
                                    else
                                        if (actual_omega <= -0.225545f)
                                            if (last_action_state <= 0.050000f)
                                                if (actual_vx <= 0.012622f)
                                                    if (sensor3 <= 0.799042f)
                                                        return 0;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor4 <= 0.374365f)
                                                        return 0;
                                                    else
                                                        return 1;
                                            else
                                                if (sensor1 <= 0.530263f)
                                                    if (sensor5 <= 0.051299f)
                                                        return 1;
                                                    else
                                                        return 0;
                                                else
                                                    if (sensor5 <= 0.074804f)
                                                        return 5;
                                                    else
                                                        return 2;
                                        else
                                            if (sensor5 <= 0.709613f)
                                                if (actual_omega <= -0.160069f)
                                                    if (sensor2 <= 0.037045f)
                                                        return 1;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor5 <= 0.592648f)
                                                        return 0;
                                                    else
                                                        return 1;
                                            else
                                                if (sensor1 <= 0.540518f)
                                                    return 1;
                                                else
                                                    return 0;
                                else
                                    if (sensor4 <= 0.428237f)
                                        if (actual_vz <= 1.160553f)
                                            if (sensor1 <= 0.330855f)
                                                if (sensor3 <= 0.848572f)
                                                    return 1;
                                                else
                                                    if (actual_vz <= 1.131578f)
                                                        return 6;
                                                    else
                                                        return 0;
                                            else
                                                if (actual_omega <= -0.193883f)
                                                    if (sensor0 <= 0.825928f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= -0.188403f)
                                                        return 1;
                                                    else
                                                        return 5;
                                        else
                                            if (actual_omega <= -0.239991f)
                                                if (actual_omega <= -0.244017f)
                                                    if (sensor4 <= 0.319650f)
                                                        return 0;
                                                    else
                                                        return 5;
                                                else
                                                    return 5;
                                            else
                                                if (sensor1 <= 0.508103f)
                                                    if (sensor1 <= 0.438450f)
                                                        return 0;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_vz <= 1.190074f)
                                                        return 7;
                                                    else
                                                        return 5;
                                    else
                                        if (sensor5 <= 0.027523f)
                                            if (actual_vx <= 0.011883f)
                                                if (sensor1 <= 0.445060f)
                                                    return 3;
                                                else
                                                    return 5;
                                            else
                                                if (last_action_state <= 0.250000f)
                                                    return 1;
                                                else
                                                    return 0;
                                        else
                                            if (sensor3 <= 0.804719f)
                                                if (actual_vx <= 0.014268f)
                                                    if (actual_omega <= -0.158853f)
                                                        return 0;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor5 <= 0.035037f)
                                                        return 5;
                                                    else
                                                        return 1;
                                            else
                                                if (sensor4 <= 0.432599f)
                                                    return 0;
                                                else
                                                    if (actual_vz <= 1.205177f)
                                                        return 5;
                                                    else
                                                        return 2;
                    else
                        if (sensor0 <= 0.367214f)
                            if (actual_omega <= -0.019683f)
                                if (sensor2 <= 0.266973f)
                                    if (sensor3 <= 0.392502f)
                                        if (actual_vz <= 1.147614f)
                                            if (last_action_state <= 0.150000f)
                                                if (sensor1 <= 0.793558f)
                                                    if (sensor3 <= 0.389230f)
                                                        return 5;
                                                    else
                                                        return 0;
                                                else
                                                    return 1;
                                            else
                                                if (actual_vx <= -0.000374f)
                                                    return 5;
                                                else
                                                    return 1;
                                        else
                                            if (sensor4 <= 0.797076f)
                                                if (sensor3 <= 0.314824f)
                                                    return 1;
                                                else
                                                    return 0;
                                            else
                                                if (sensor5 <= 0.294924f)
                                                    if (actual_vz <= 1.291309f)
                                                        return 5;
                                                    else
                                                        return 0;
                                                else
                                                    if (actual_omega <= -0.038222f)
                                                        return 0;
                                                    else
                                                        return 1;
                                    else
                                        return 5;
                                else
                                    if (actual_vx <= 0.006032f)
                                        if (sensor5 <= 0.546625f)
                                            if (sensor0 <= 0.316348f)
                                                if (actual_vz <= 1.210063f)
                                                    if (last_action_state <= 0.250000f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vz <= 1.210916f)
                                                        return 10;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor3 <= 0.333665f)
                                                    if (sensor3 <= 0.310958f)
                                                        return 1;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor0 <= 0.319866f)
                                                        return 1;
                                                    else
                                                        return 5;
                                        else
                                            return 0;
                                    else
                                        return 6;
                            else
                                if (sensor5 <= 0.474486f)
                                    if (sensor2 <= 0.276924f)
                                        if (sensor4 <= 0.859728f)
                                            if (sensor4 <= 0.817874f)
                                                return 5;
                                            else
                                                return 2;
                                        else
                                            if (sensor1 <= 0.786108f)
                                                if (sensor0 <= 0.288363f)
                                                    return 0;
                                                else
                                                    return 1;
                                            else
                                                return 0;
                                    else
                                        if (actual_vz <= 1.260899f)
                                            if (actual_vx <= -0.016893f)
                                                return 0;
                                            else
                                                if (actual_omega <= -0.017259f)
                                                    return 0;
                                                else
                                                    if (sensor1 <= 0.796365f)
                                                        return 5;
                                                    else
                                                        return 5;
                                        else
                                            if (actual_omega <= 0.021493f)
                                                return 5;
                                            else
                                                return 0;
                                else
                                    if (sensor5 <= 0.554264f)
                                        if (sensor1 <= 0.796326f)
                                            return 0;
                                        else
                                            if (actual_vx <= -0.010316f)
                                                return 5;
                                            else
                                                return 0;
                                    else
                                        return 1;
                        else
                            if (actual_omega <= -0.229460f)
                                if (sensor1 <= 0.724999f)
                                    if (actual_omega <= -0.261310f)
                                        if (sensor3 <= 0.774425f)
                                            if (actual_vx <= 0.023806f)
                                                if (actual_vz <= 1.034191f)
                                                    if (actual_vx <= 0.007740f)
                                                        return 0;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_vz <= 1.130217f)
                                                        return 5;
                                                    else
                                                        return 1;
                                            else
                                                if (sensor2 <= 0.110080f)
                                                    if (sensor4 <= 0.547012f)
                                                        return 5;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_vz <= 1.194459f)
                                                        return 5;
                                                    else
                                                        return 5;
                                        else
                                            if (actual_vz <= 1.224631f)
                                                if (sensor1 <= 0.556337f)
                                                    if (actual_vz <= 1.072239f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vx <= 0.008795f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (last_action_state <= 0.050000f)
                                                    if (actual_vx <= 0.013280f)
                                                        return 1;
                                                    else
                                                        return 2;
                                                else
                                                    if (actual_omega <= -0.284707f)
                                                        return 1;
                                                    else
                                                        return 5;
                                    else
                                        if (actual_vz <= 1.171696f)
                                            if (sensor1 <= 0.631632f)
                                                if (sensor3 <= 0.707843f)
                                                    if (sensor2 <= 0.097953f)
                                                        return 1;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor3 <= 0.826266f)
                                                        return 1;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor5 <= 0.169089f)
                                                    if (sensor0 <= 0.769845f)
                                                        return 5;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_vx <= 0.027670f)
                                                        return 1;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor0 <= 0.747404f)
                                                if (actual_vx <= 0.006259f)
                                                    if (sensor5 <= 0.078206f)
                                                        return 1;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor4 <= 0.828697f)
                                                        return 1;
                                                    else
                                                        return 1;
                                            else
                                                if (actual_omega <= -0.241641f)
                                                    if (actual_vx <= 0.020858f)
                                                        return 1;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor5 <= 0.146936f)
                                                        return 1;
                                                    else
                                                        return 2;
                                else
                                    if (sensor1 <= 0.772989f)
                                        if (actual_vx <= 0.021748f)
                                            if (actual_omega <= -0.242605f)
                                                if (sensor0 <= 0.705182f)
                                                    if (sensor1 <= 0.742037f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor2 <= 0.196110f)
                                                        return 1;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vz <= 1.157833f)
                                                    if (sensor3 <= 0.747270f)
                                                        return 1;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= -0.232318f)
                                                        return 1;
                                                    else
                                                        return 2;
                                        else
                                            if (actual_omega <= -0.230120f)
                                                if (sensor4 <= 0.746610f)
                                                    return 4;
                                                else
                                                    if (sensor5 <= 0.724597f)
                                                        return 5;
                                                    else
                                                        return 1;
                                            else
                                                return 2;
                                    else
                                        if (sensor1 <= 0.797612f)
                                            if (actual_vx <= 0.002850f)
                                                if (actual_vz <= 1.244909f)
                                                    return 0;
                                                else
                                                    return 2;
                                            else
                                                if (sensor0 <= 0.672049f)
                                                    if (sensor2 <= 0.143962f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vz <= 1.186594f)
                                                        return 5;
                                                    else
                                                        return 1;
                                        else
                                            return 10;
                            else
                                if (actual_vx <= 0.012918f)
                                    if (actual_omega <= -0.180201f)
                                        if (last_action_state <= 0.150000f)
                                            if (sensor1 <= 0.776168f)
                                                if (actual_vz <= 1.164855f)
                                                    if (sensor3 <= 0.732373f)
                                                        return 1;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor4 <= 0.688479f)
                                                        return 1;
                                                    else
                                                        return 1;
                                            else
                                                if (sensor3 <= 0.697338f)
                                                    if (actual_vx <= 0.006452f)
                                                        return 1;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= -0.222213f)
                                                        return 2;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor0 <= 0.719306f)
                                                if (actual_omega <= -0.182654f)
                                                    if (sensor5 <= 0.229412f)
                                                        return 5;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_vz <= 1.225621f)
                                                        return 5;
                                                    else
                                                        return 1;
                                            else
                                                if (sensor4 <= 0.564094f)
                                                    if (actual_vz <= 1.156590f)
                                                        return 5;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_omega <= -0.186876f)
                                                        return 0;
                                                    else
                                                        return 2;
                                    else
                                        if (sensor1 <= 0.698070f)
                                            if (actual_vz <= 0.587393f)
                                                if (actual_vx <= -0.006590f)
                                                    if (actual_vx <= -0.016719f)
                                                        return 0;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor0 <= 0.413547f)
                                                        return 0;
                                                    else
                                                        return 0;
                                            else
                                                if (last_action_state <= 0.250000f)
                                                    if (actual_vz <= 1.130873f)
                                                        return 1;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_omega <= -0.123446f)
                                                        return 5;
                                                    else
                                                        return 0;
                                        else
                                            if (actual_vz <= 1.113195f)
                                                if (actual_omega <= -0.173009f)
                                                    if (actual_omega <= -0.179017f)
                                                        return 0;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor5 <= 0.557606f)
                                                        return 1;
                                                    else
                                                        return 1;
                                            else
                                                if (actual_vx <= 0.000726f)
                                                    if (sensor4 <= 0.688956f)
                                                        return 0;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor0 <= 0.634968f)
                                                        return 1;
                                                    else
                                                        return 1;
                                else
                                    if (sensor0 <= 0.688844f)
                                        if (actual_omega <= -0.183531f)
                                            if (sensor2 <= 0.089330f)
                                                if (actual_vx <= 0.013116f)
                                                    return 2;
                                                else
                                                    if (actual_omega <= -0.225359f)
                                                        return 1;
                                                    else
                                                        return 1;
                                            else
                                                if (actual_vz <= 1.177177f)
                                                    if (sensor5 <= 0.759234f)
                                                        return 5;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_omega <= -0.218570f)
                                                        return 5;
                                                    else
                                                        return 1;
                                        else
                                            if (actual_vx <= 0.020141f)
                                                if (sensor2 <= 0.222629f)
                                                    if (sensor3 <= 0.471771f)
                                                        return 1;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_vx <= 0.015838f)
                                                        return 1;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_omega <= -0.157717f)
                                                    if (sensor5 <= 0.197857f)
                                                        return 0;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor0 <= 0.523659f)
                                                        return 5;
                                                    else
                                                        return 1;
                                    else
                                        if (actual_vz <= 1.085344f)
                                            if (actual_vx <= 0.022177f)
                                                if (actual_vz <= 1.031023f)
                                                    if (actual_vx <= 0.020099f)
                                                        return 1;
                                                    else
                                                        return 0;
                                                else
                                                    if (sensor5 <= 0.049617f)
                                                        return 1;
                                                    else
                                                        return 1;
                                            else
                                                if (sensor1 <= 0.627818f)
                                                    if (actual_vx <= 0.024917f)
                                                        return 1;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= -0.199163f)
                                                        return 5;
                                                    else
                                                        return 1;
                                        else
                                            if (sensor5 <= 0.190361f)
                                                if (sensor2 <= 0.189458f)
                                                    if (last_action_state <= 0.250000f)
                                                        return 1;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_vz <= 1.154147f)
                                                        return 1;
                                                    else
                                                        return 1;
                                            else
                                                if (sensor4 <= 0.870246f)
                                                    if (sensor1 <= 0.780214f)
                                                        return 1;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor2 <= 0.179561f)
                                                        return 1;
                                                    else
                                                        return 0;
                else
                    if (actual_omega <= -0.175347f)
                        if (sensor4 <= 0.532105f)
                            if (actual_vz <= 1.129092f)
                                if (sensor4 <= 0.439140f)
                                    if (actual_vx <= 0.007283f)
                                        if (sensor0 <= 0.845677f)
                                            if (sensor3 <= 0.837707f)
                                                if (actual_omega <= -0.178535f)
                                                    if (actual_omega <= -0.239019f)
                                                        return 0;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor3 <= 0.831082f)
                                                        return 0;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_omega <= -0.192437f)
                                                    if (actual_vx <= 0.005382f)
                                                        return 5;
                                                    else
                                                        return 0;
                                                else
                                                    if (sensor4 <= 0.310085f)
                                                        return 5;
                                                    else
                                                        return 5;
                                        else
                                            if (actual_vx <= 0.003115f)
                                                return 0;
                                            else
                                                if (sensor0 <= 0.847596f)
                                                    if (actual_omega <= -0.178516f)
                                                        return 1;
                                                    else
                                                        return 0;
                                                else
                                                    if (actual_vz <= 1.078755f)
                                                        return 0;
                                                    else
                                                        return 5;
                                    else
                                        if (sensor1 <= 0.414435f)
                                            if (sensor4 <= 0.302154f)
                                                if (actual_omega <= -0.177048f)
                                                    if (sensor3 <= 0.785055f)
                                                        return 0;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vz <= 1.060597f)
                                                        return 6;
                                                    else
                                                        return 1;
                                            else
                                                if (sensor2 <= 0.020207f)
                                                    if (sensor1 <= 0.323195f)
                                                        return 5;
                                                    else
                                                        return 0;
                                                else
                                                    if (sensor2 <= 0.026495f)
                                                        return 5;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor2 <= 0.031993f)
                                                if (sensor5 <= 0.029714f)
                                                    if (sensor0 <= 0.810655f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vz <= 1.113281f)
                                                        return 5;
                                                    else
                                                        return 4;
                                            else
                                                if (actual_omega <= -0.251945f)
                                                    if (sensor3 <= 0.829627f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vx <= 0.026210f)
                                                        return 5;
                                                    else
                                                        return 10;
                                else
                                    if (sensor5 <= 0.035055f)
                                        if (sensor0 <= 0.833099f)
                                            if (actual_vx <= 0.020446f)
                                                if (sensor0 <= 0.832852f)
                                                    if (sensor2 <= 0.069879f)
                                                        return 1;
                                                    else
                                                        return 5;
                                                else
                                                    return 3;
                                            else
                                                if (sensor3 <= 0.803664f)
                                                    return 10;
                                                else
                                                    if (sensor2 <= 0.075229f)
                                                        return 5;
                                                    else
                                                        return 6;
                                        else
                                            if (last_action_state <= 0.450000f)
                                                if (actual_vx <= 0.012267f)
                                                    return 1;
                                                else
                                                    if (actual_vx <= 0.015094f)
                                                        return 0;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_omega <= -0.203773f)
                                                    if (actual_vz <= 1.120187f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor4 <= 0.470372f)
                                                        return 5;
                                                    else
                                                        return 5;
                                    else
                                        if (sensor2 <= 0.034766f)
                                            return 3;
                                        else
                                            if (last_action_state <= 0.450000f)
                                                if (sensor2 <= 0.048329f)
                                                    if (sensor3 <= 0.839087f)
                                                        return 2;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_vx <= 0.024637f)
                                                        return 5;
                                                    else
                                                        return 1;
                                            else
                                                if (actual_vz <= 1.128851f)
                                                    if (actual_omega <= -0.176869f)
                                                        return 5;
                                                    else
                                                        return 1;
                                                else
                                                    return 4;
                            else
                                if (actual_omega <= -0.226520f)
                                    if (last_action_state <= 0.850000f)
                                        if (sensor0 <= 0.870666f)
                                            if (sensor5 <= 0.010455f)
                                                if (sensor1 <= 0.329223f)
                                                    return 10;
                                                else
                                                    if (actual_omega <= -0.271143f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vz <= 1.170990f)
                                                    if (sensor5 <= 0.010745f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vx <= 0.021384f)
                                                        return 5;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor5 <= 0.028155f)
                                                return 1;
                                            else
                                                return 4;
                                    else
                                        if (sensor4 <= 0.517602f)
                                            if (actual_vx <= 0.015674f)
                                                if (sensor1 <= 0.334123f)
                                                    return 8;
                                                else
                                                    if (sensor2 <= 0.061823f)
                                                        return 5;
                                                    else
                                                        return 9;
                                            else
                                                if (sensor4 <= 0.298469f)
                                                    if (actual_omega <= -0.260818f)
                                                        return 6;
                                                    else
                                                        return 10;
                                                else
                                                    if (actual_vz <= 1.150370f)
                                                        return 10;
                                                    else
                                                        return 5;
                                        else
                                            if (actual_omega <= -0.232919f)
                                                return 8;
                                            else
                                                return 10;
                                else
                                    if (sensor1 <= 0.411709f)
                                        if (actual_vx <= 0.008941f)
                                            if (sensor2 <= 0.021339f)
                                                if (actual_vx <= 0.004961f)
                                                    if (actual_vx <= 0.004141f)
                                                        return 5;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor3 <= 0.872409f)
                                                        return 0;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor5 <= 0.017368f)
                                                    if (sensor1 <= 0.399133f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor3 <= 0.841956f)
                                                        return 5;
                                                    else
                                                        return 0;
                                        else
                                            if (actual_vz <= 1.163428f)
                                                if (actual_vz <= 1.130199f)
                                                    if (sensor2 <= 0.026011f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor2 <= 0.033473f)
                                                        return 5;
                                                    else
                                                        return 1;
                                            else
                                                if (sensor3 <= 0.795164f)
                                                    return 0;
                                                else
                                                    if (sensor0 <= 0.803842f)
                                                        return 0;
                                                    else
                                                        return 5;
                                    else
                                        if (sensor3 <= 0.794414f)
                                            if (sensor0 <= 0.837851f)
                                                if (sensor3 <= 0.792783f)
                                                    if (sensor0 <= 0.832362f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor1 <= 0.467779f)
                                                        return 1;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor5 <= 0.032556f)
                                                    if (sensor1 <= 0.471823f)
                                                        return 1;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor3 <= 0.789358f)
                                                        return 0;
                                                    else
                                                        return 0;
                                        else
                                            if (actual_omega <= -0.191220f)
                                                if (sensor4 <= 0.341164f)
                                                    if (sensor2 <= 0.028176f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor2 <= 0.030542f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor1 <= 0.550891f)
                                                    if (sensor5 <= 0.029480f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor3 <= 0.899123f)
                                                        return 5;
                                                    else
                                                        return 6;
                        else
                            if (last_action_state <= 0.750000f)
                                if (sensor0 <= 0.734019f)
                                    if (sensor5 <= 0.645272f)
                                        if (sensor0 <= 0.615029f)
                                            if (actual_vx <= 0.009184f)
                                                if (sensor3 <= 0.556076f)
                                                    if (sensor0 <= 0.457472f)
                                                        return 0;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_vz <= 1.193833f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_omega <= -0.191337f)
                                                    if (sensor3 <= 0.771911f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor2 <= 0.145673f)
                                                        return 5;
                                                    else
                                                        return 10;
                                        else
                                            if (actual_vz <= 1.185225f)
                                                if (actual_vx <= 0.023280f)
                                                    if (sensor5 <= 0.197682f)
                                                        return 5;
                                                    else
                                                        return 0;
                                                else
                                                    if (sensor5 <= 0.142139f)
                                                        return 8;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vx <= 0.003647f)
                                                    if (actual_omega <= -0.200805f)
                                                        return 2;
                                                    else
                                                        return 4;
                                                else
                                                    if (actual_omega <= -0.205045f)
                                                        return 5;
                                                    else
                                                        return 5;
                                    else
                                        if (actual_vx <= 0.017580f)
                                            if (sensor1 <= 0.730212f)
                                                if (sensor1 <= 0.692735f)
                                                    if (actual_vz <= 0.852551f)
                                                        return 1;
                                                    else
                                                        return 0;
                                                else
                                                    return 1;
                                            else
                                                return 0;
                                        else
                                            if (actual_vz <= 0.931430f)
                                                if (actual_vz <= 0.846239f)
                                                    return 4;
                                                else
                                                    if (sensor1 <= 0.701562f)
                                                        return 1;
                                                    else
                                                        return 0;
                                            else
                                                if (sensor3 <= 0.340124f)
                                                    return 5;
                                                else
                                                    return 10;
                                else
                                    if (actual_vz <= 1.081120f)
                                        if (actual_vx <= 0.021210f)
                                            if (actual_omega <= -0.187707f)
                                                if (sensor1 <= 0.519255f)
                                                    if (sensor1 <= 0.512657f)
                                                        return 6;
                                                    else
                                                        return 0;
                                                else
                                                    if (sensor4 <= 0.536191f)
                                                        return 4;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor4 <= 0.689491f)
                                                    if (actual_omega <= -0.179738f)
                                                        return 1;
                                                    else
                                                        return 5;
                                                else
                                                    return 5;
                                        else
                                            if (sensor1 <= 0.630370f)
                                                if (sensor4 <= 0.680905f)
                                                    if (sensor1 <= 0.628320f)
                                                        return 5;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor2 <= 0.115385f)
                                                        return 8;
                                                    else
                                                        return 0;
                                            else
                                                if (last_action_state <= 0.450000f)
                                                    if (actual_omega <= -0.191848f)
                                                        return 4;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor0 <= 0.735179f)
                                                        return 9;
                                                    else
                                                        return 5;
                                    else
                                        if (actual_omega <= -0.214318f)
                                            if (sensor2 <= 0.066411f)
                                                if (sensor1 <= 0.470805f)
                                                    if (sensor5 <= 0.062012f)
                                                        return 0;
                                                    else
                                                        return 2;
                                                else
                                                    if (sensor3 <= 0.751206f)
                                                        return 6;
                                                    else
                                                        return 1;
                                            else
                                                if (actual_vx <= 0.028185f)
                                                    if (sensor1 <= 0.604818f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor4 <= 0.739286f)
                                                        return 10;
                                                    else
                                                        return 8;
                                        else
                                            if (actual_vx <= 0.025123f)
                                                if (sensor5 <= 0.148899f)
                                                    if (sensor0 <= 0.778570f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor1 <= 0.707070f)
                                                        return 0;
                                                    else
                                                        return 1;
                                            else
                                                if (sensor4 <= 0.601166f)
                                                    if (actual_omega <= -0.193402f)
                                                        return 10;
                                                    else
                                                        return 4;
                                                else
                                                    if (actual_vz <= 1.281969f)
                                                        return 5;
                                                    else
                                                        return 4;
                            else
                                if (actual_omega <= -0.189000f)
                                    if (sensor0 <= 0.516246f)
                                        if (sensor1 <= 0.795635f)
                                            if (sensor1 <= 0.745965f)
                                                return 8;
                                            else
                                                if (sensor1 <= 0.775366f)
                                                    if (sensor1 <= 0.774146f)
                                                        return 10;
                                                    else
                                                        return 9;
                                                else
                                                    if (actual_omega <= -0.244853f)
                                                        return 5;
                                                    else
                                                        return 10;
                                        else
                                            if (sensor4 <= 0.908419f)
                                                return 8;
                                            else
                                                return 9;
                                    else
                                        if (actual_omega <= -0.277872f)
                                            return 10;
                                        else
                                            if (actual_vx <= 0.024875f)
                                                if (sensor4 <= 0.538287f)
                                                    if (actual_vx <= 0.020753f)
                                                        return 8;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor5 <= 0.192349f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor2 <= 0.084938f)
                                                    if (sensor1 <= 0.527635f)
                                                        return 8;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vz <= 1.160532f)
                                                        return 5;
                                                    else
                                                        return 10;
                                else
                                    if (sensor2 <= 0.069715f)
                                        return 6;
                                    else
                                        if (sensor2 <= 0.156425f)
                                            if (sensor3 <= 0.821808f)
                                                if (sensor5 <= 0.140682f)
                                                    if (sensor0 <= 0.797365f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= -0.182268f)
                                                        return 10;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor2 <= 0.088076f)
                                                    return 8;
                                                else
                                                    return 10;
                                        else
                                            if (actual_omega <= -0.187472f)
                                                return 9;
                                            else
                                                if (actual_omega <= -0.186786f)
                                                    return 5;
                                                else
                                                    if (sensor2 <= 0.163730f)
                                                        return 10;
                                                    else
                                                        return 6;
                    else
                        if (sensor0 <= 0.392651f)
                            if (sensor2 <= 0.315874f)
                                if (actual_vx <= 0.009561f)
                                    if (actual_vz <= 0.790408f)
                                        if (sensor2 <= 0.311537f)
                                            if (sensor0 <= 0.350541f)
                                                if (sensor5 <= 0.508669f)
                                                    if (actual_omega <= 0.147747f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    return 1;
                                            else
                                                if (actual_vx <= -0.004364f)
                                                    if (actual_vx <= -0.009036f)
                                                        return 0;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_vz <= 0.271199f)
                                                        return 5;
                                                    else
                                                        return 5;
                                        else
                                            if (actual_vx <= -0.000462f)
                                                return 5;
                                            else
                                                if (sensor5 <= 0.318638f)
                                                    return 10;
                                                else
                                                    return 5;
                                    else
                                        if (sensor2 <= 0.198698f)
                                            if (sensor3 <= 0.334816f)
                                                if (sensor5 <= 0.590198f)
                                                    return 4;
                                                else
                                                    return 2;
                                            else
                                                return 1;
                                        else
                                            if (sensor0 <= 0.307716f)
                                                if (sensor5 <= 0.471697f)
                                                    if (sensor0 <= 0.307586f)
                                                        return 5;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor5 <= 0.497509f)
                                                        return 0;
                                                    else
                                                        return 1;
                                            else
                                                if (sensor5 <= 0.287814f)
                                                    if (actual_vz <= 1.132772f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= -0.010254f)
                                                        return 5;
                                                    else
                                                        return 5;
                                else
                                    if (sensor2 <= 0.232023f)
                                        if (sensor5 <= 0.588097f)
                                            if (actual_omega <= 0.075584f)
                                                if (sensor1 <= 0.796376f)
                                                    if (actual_vx <= 0.022943f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    return 0;
                                            else
                                                return 1;
                                        else
                                            return 2;
                                    else
                                        if (actual_vz <= 1.106750f)
                                            if (actual_omega <= 0.122846f)
                                                if (sensor1 <= 0.786119f)
                                                    if (sensor3 <= 0.249102f)
                                                        return 5;
                                                    else
                                                        return 10;
                                                else
                                                    if (last_action_state <= 0.750000f)
                                                        return 5;
                                                    else
                                                        return 6;
                                            else
                                                return 5;
                                        else
                                            if (sensor0 <= 0.339059f)
                                                if (actual_omega <= -0.028475f)
                                                    if (actual_vx <= 0.025685f)
                                                        return 8;
                                                    else
                                                        return 10;
                                                else
                                                    return 10;
                                            else
                                                if (sensor3 <= 0.474938f)
                                                    return 5;
                                                else
                                                    if (sensor0 <= 0.344702f)
                                                        return 9;
                                                    else
                                                        return 7;
                            else
                                if (sensor5 <= 0.268354f)
                                    if (actual_vx <= 0.000037f)
                                        if (sensor3 <= 0.453535f)
                                            if (actual_vx <= -0.001067f)
                                                if (actual_vx <= -0.002371f)
                                                    if (actual_omega <= -0.061508f)
                                                        return 10;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor5 <= 0.209854f)
                                                        return 10;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_omega <= -0.000433f)
                                                    if (sensor5 <= 0.129166f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    return 5;
                                        else
                                            if (actual_vx <= -0.010678f)
                                                if (actual_omega <= 0.141775f)
                                                    if (sensor0 <= 0.121916f)
                                                        return 10;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_omega <= 0.150711f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor2 <= 0.337676f)
                                                    return 5;
                                                else
                                                    if (sensor2 <= 0.413455f)
                                                        return 10;
                                                    else
                                                        return 8;
                                    else
                                        if (actual_vz <= 0.153715f)
                                            if (sensor1 <= 0.548611f)
                                                if (sensor1 <= 0.545355f)
                                                    return 9;
                                                else
                                                    return 10;
                                            else
                                                if (actual_omega <= -0.012170f)
                                                    return 10;
                                                else
                                                    return 5;
                                        else
                                            if (sensor3 <= 0.515884f)
                                                if (sensor0 <= 0.176754f)
                                                    if (actual_vx <= 0.000252f)
                                                        return 6;
                                                    else
                                                        return 10;
                                                else
                                                    return 5;
                                            else
                                                if (sensor3 <= 0.535073f)
                                                    return 9;
                                                else
                                                    return 10;
                                else
                                    if (last_action_state <= 0.750000f)
                                        if (actual_vz <= 0.822219f)
                                            if (actual_omega <= -0.034997f)
                                                if (sensor0 <= 0.192035f)
                                                    return 10;
                                                else
                                                    return 5;
                                            else
                                                if (sensor0 <= 0.141388f)
                                                    return 10;
                                                else
                                                    if (sensor3 <= 0.553144f)
                                                        return 5;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor0 <= 0.234329f)
                                                if (actual_vx <= -0.002268f)
                                                    if (sensor2 <= 0.408541f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vz <= 1.099199f)
                                                        return 10;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor3 <= 0.278600f)
                                                    if (sensor3 <= 0.244922f)
                                                        return 5;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor1 <= 0.776998f)
                                                        return 10;
                                                    else
                                                        return 5;
                                    else
                                        if (actual_omega <= 0.142398f)
                                            if (sensor2 <= 0.350055f)
                                                if (sensor5 <= 0.375875f)
                                                    if (actual_vz <= 0.699006f)
                                                        return 5;
                                                    else
                                                        return 10;
                                                else
                                                    if (actual_omega <= 0.051014f)
                                                        return 6;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor0 <= 0.221352f)
                                                    if (sensor2 <= 0.357491f)
                                                        return 5;
                                                    else
                                                        return 8;
                                                else
                                                    if (sensor0 <= 0.228936f)
                                                        return 9;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor5 <= 0.356378f)
                                                if (actual_vz <= 0.855520f)
                                                    if (sensor2 <= 0.360334f)
                                                        return 5;
                                                    else
                                                        return 9;
                                                else
                                                    if (sensor4 <= 0.931816f)
                                                        return 5;
                                                    else
                                                        return 6;
                                            else
                                                if (actual_vz <= 1.276334f)
                                                    if (sensor2 <= 0.361824f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= 0.173051f)
                                                        return 10;
                                                    else
                                                        return 6;
                        else
                            if (sensor3 <= 0.768817f)
                                if (last_action_state <= 0.650000f)
                                    if (sensor0 <= 0.613803f)
                                        if (actual_omega <= -0.113591f)
                                            if (actual_vz <= 1.128914f)
                                                if (sensor5 <= 0.240533f)
                                                    if (sensor3 <= 0.767916f)
                                                        return 5;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor1 <= 0.790377f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor2 <= 0.124897f)
                                                    if (sensor5 <= 0.177325f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor3 <= 0.755389f)
                                                        return 5;
                                                    else
                                                        return 10;
                                        else
                                            if (actual_vz <= 0.018544f)
                                                return 5;
                                            else
                                                if (actual_omega <= -0.092441f)
                                                    if (actual_vz <= 1.224251f)
                                                        return 5;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_vx <= 0.012349f)
                                                        return 1;
                                                    else
                                                        return 5;
                                    else
                                        if (actual_vz <= 1.141907f)
                                            if (actual_omega <= -0.115809f)
                                                if (last_action_state <= 0.450000f)
                                                    if (actual_vz <= 1.021403f)
                                                        return 5;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor2 <= 0.187895f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor1 <= 0.774632f)
                                                    if (sensor2 <= 0.058037f)
                                                        return 2;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor4 <= 0.811755f)
                                                        return 5;
                                                    else
                                                        return 1;
                                        else
                                            if (actual_omega <= -0.130988f)
                                                if (sensor0 <= 0.745207f)
                                                    if (sensor1 <= 0.787607f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor4 <= 0.510783f)
                                                        return 5;
                                                    else
                                                        return 1;
                                            else
                                                if (actual_vx <= 0.013556f)
                                                    if (actual_vx <= 0.013314f)
                                                        return 1;
                                                    else
                                                        return 0;
                                                else
                                                    if (sensor0 <= 0.641852f)
                                                        return 2;
                                                    else
                                                        return 1;
                                else
                                    if (sensor4 <= 0.972317f)
                                        if (actual_omega <= -0.071181f)
                                            if (actual_vz <= 1.147883f)
                                                if (sensor2 <= 0.062118f)
                                                    if (sensor2 <= 0.060656f)
                                                        return 1;
                                                    else
                                                        return 0;
                                                else
                                                    if (actual_vx <= 0.024028f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor4 <= 0.690800f)
                                                    if (actual_vz <= 1.290812f)
                                                        return 5;
                                                    else
                                                        return 3;
                                                else
                                                    if (actual_omega <= -0.127228f)
                                                        return 5;
                                                    else
                                                        return 5;
                                        else
                                            if (actual_vz <= 1.366817f)
                                                if (actual_vx <= -0.000823f)
                                                    if (sensor2 <= 0.150854f)
                                                        return 5;
                                                    else
                                                        return 9;
                                                else
                                                    if (sensor4 <= 0.707808f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                return 4;
                                    else
                                        if (sensor1 <= 0.784192f)
                                            return 10;
                                        else
                                            return 8;
                            else
                                if (actual_vx <= 0.004708f)
                                    if (sensor1 <= 0.466954f)
                                        if (actual_vz <= 1.071954f)
                                            if (actual_vx <= -0.004973f)
                                                if (sensor4 <= 0.371209f)
                                                    if (actual_vz <= 1.035739f)
                                                        return 0;
                                                    else
                                                        return 0;
                                                else
                                                    if (sensor4 <= 0.383808f)
                                                        return 1;
                                                    else
                                                        return 0;
                                            else
                                                if (sensor4 <= 0.352990f)
                                                    if (sensor1 <= 0.401368f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor3 <= 0.813645f)
                                                        return 0;
                                                    else
                                                        return 5;
                                        else
                                            if (actual_omega <= -0.114706f)
                                                if (sensor0 <= 0.835610f)
                                                    if (sensor5 <= 0.015304f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= -0.131031f)
                                                        return 0;
                                                    else
                                                        return 0;
                                            else
                                                if (actual_omega <= -0.069052f)
                                                    if (actual_vx <= -0.002001f)
                                                        return 5;
                                                    else
                                                        return 0;
                                                else
                                                    if (sensor2 <= 0.020930f)
                                                        return 0;
                                                    else
                                                        return 0;
                                    else
                                        if (actual_vz <= 1.154177f)
                                            if (actual_omega <= -0.070980f)
                                                if (actual_vx <= -0.001655f)
                                                    if (actual_vz <= 1.115088f)
                                                        return 5;
                                                    else
                                                        return 0;
                                                else
                                                    if (sensor0 <= 0.857424f)
                                                        return 5;
                                                    else
                                                        return 0;
                                            else
                                                if (last_action_state <= 0.550000f)
                                                    if (sensor5 <= 0.020597f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vx <= 0.004273f)
                                                        return 5;
                                                    else
                                                        return 10;
                                        else
                                            if (last_action_state <= 0.550000f)
                                                if (sensor2 <= 0.075827f)
                                                    if (actual_vx <= -0.001867f)
                                                        return 0;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= -0.140964f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_omega <= -0.096736f)
                                                    return 5;
                                                else
                                                    if (last_action_state <= 0.950000f)
                                                        return 5;
                                                    else
                                                        return 5;
                                else
                                    if (actual_vz <= 1.141708f)
                                        if (sensor0 <= 0.817515f)
                                            if (sensor2 <= 0.110984f)
                                                if (actual_omega <= -0.101085f)
                                                    if (sensor4 <= 0.314231f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vx <= 0.008424f)
                                                        return 5;
                                                    else
                                                        return 0;
                                            else
                                                if (actual_vx <= 0.010118f)
                                                    if (actual_vz <= 1.109492f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor5 <= 0.149947f)
                                                        return 10;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor5 <= 0.018326f)
                                                if (sensor1 <= 0.416826f)
                                                    if (sensor2 <= 0.017800f)
                                                        return 0;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vz <= 1.007136f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vx <= 0.010937f)
                                                    if (actual_vx <= 0.006385f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor1 <= 0.475163f)
                                                        return 5;
                                                    else
                                                        return 5;
                                    else
                                        if (sensor0 <= 0.813179f)
                                            if (sensor4 <= 0.316453f)
                                                if (sensor0 <= 0.810785f)
                                                    if (sensor3 <= 0.844372f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor0 <= 0.811062f)
                                                        return 0;
                                                    else
                                                        return 5;
                                            else
                                                if (last_action_state <= 0.750000f)
                                                    if (sensor1 <= 0.451473f)
                                                        return 0;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor0 <= 0.558416f)
                                                        return 10;
                                                    else
                                                        return 5;
                                        else
                                            if (last_action_state <= 0.950000f)
                                                if (sensor2 <= 0.021910f)
                                                    if (actual_vx <= 0.005004f)
                                                        return 2;
                                                    else
                                                        return 0;
                                                else
                                                    if (actual_omega <= -0.134291f)
                                                        return 5;
                                                    else
                                                        return 0;
                                            else
                                                if (sensor3 <= 0.781415f)
                                                    if (actual_omega <= -0.168488f)
                                                        return 10;
                                                    else
                                                        return 8;
                                                else
                                                    if (actual_vz <= 1.276403f)
                                                        return 5;
                                                    else
                                                        return 5;
            else
                if (sensor4 <= 0.515319f)
                    if (last_action_state <= 0.650000f)
                        if (actual_omega <= 0.171937f)
                            if (sensor1 <= 0.537182f)
                                if (actual_vz <= 0.057238f)
                                    if (sensor0 <= 0.043559f)
                                        if (sensor4 <= 0.412105f)
                                            return 5;
                                        else
                                            return 10;
                                    else
                                        return 5;
                                else
                                    if (actual_omega <= 0.130889f)
                                        if (actual_omega <= 0.037084f)
                                            if (actual_omega <= -0.000161f)
                                                if (actual_omega <= -0.000276f)
                                                    if (sensor5 <= 0.038143f)
                                                        return 5;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor1 <= 0.490631f)
                                                        return 8;
                                                    else
                                                        return 5;
                                            else
                                                return 10;
                                        else
                                            if (actual_vx <= -0.002169f)
                                                if (sensor1 <= 0.503718f)
                                                    if (sensor4 <= 0.449279f)
                                                        return 10;
                                                    else
                                                        return 8;
                                                else
                                                    if (actual_vz <= 1.062974f)
                                                        return 7;
                                                    else
                                                        return 7;
                                            else
                                                if (sensor4 <= 0.453054f)
                                                    if (sensor2 <= 0.807111f)
                                                        return 7;
                                                    else
                                                        return 10;
                                                else
                                                    return 9;
                                    else
                                        if (last_action_state <= 0.450000f)
                                            if (sensor4 <= 0.296128f)
                                                if (actual_vx <= -0.009055f)
                                                    if (sensor1 <= 0.417108f)
                                                        return 5;
                                                    else
                                                        return 0;
                                                else
                                                    if (sensor3 <= 0.017215f)
                                                        return 5;
                                                    else
                                                        return 7;
                                            else
                                                if (actual_vx <= -0.006911f)
                                                    if (sensor1 <= 0.496377f)
                                                        return 6;
                                                    else
                                                        return 0;
                                                else
                                                    if (sensor3 <= 0.020654f)
                                                        return 7;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor1 <= 0.435577f)
                                                if (sensor5 <= 0.803831f)
                                                    if (sensor1 <= 0.419921f)
                                                        return 9;
                                                    else
                                                        return 9;
                                                else
                                                    if (sensor0 <= 0.020813f)
                                                        return 10;
                                                    else
                                                        return 7;
                                            else
                                                if (sensor4 <= 0.377752f)
                                                    if (actual_vz <= 1.027335f)
                                                        return 6;
                                                    else
                                                        return 7;
                                                else
                                                    if (actual_vx <= -0.009078f)
                                                        return 7;
                                                    else
                                                        return 9;
                            else
                                if (actual_omega <= 0.121738f)
                                    if (sensor2 <= 0.716485f)
                                        if (actual_vz <= 1.173638f)
                                            if (sensor0 <= 0.072871f)
                                                if (sensor0 <= 0.060209f)
                                                    if (sensor4 <= 0.487314f)
                                                        return 10;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vx <= 0.001677f)
                                                        return 6;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_omega <= 0.114489f)
                                                    if (sensor0 <= 0.073899f)
                                                        return 7;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor4 <= 0.494740f)
                                                        return 6;
                                                    else
                                                        return 10;
                                        else
                                            if (sensor4 <= 0.497018f)
                                                if (sensor2 <= 0.695060f)
                                                    return 10;
                                                else
                                                    if (sensor1 <= 0.613695f)
                                                        return 5;
                                                    else
                                                        return 8;
                                            else
                                                if (actual_vz <= 1.177897f)
                                                    return 8;
                                                else
                                                    if (sensor5 <= 0.824264f)
                                                        return 6;
                                                    else
                                                        return 7;
                                    else
                                        if (actual_vz <= 1.035470f)
                                            if (sensor2 <= 0.788684f)
                                                if (sensor4 <= 0.492169f)
                                                    if (actual_omega <= 0.120026f)
                                                        return 6;
                                                    else
                                                        return 8;
                                                else
                                                    if (sensor5 <= 0.826878f)
                                                        return 10;
                                                    else
                                                        return 9;
                                            else
                                                return 5;
                                        else
                                            if (actual_omega <= 0.118243f)
                                                if (sensor2 <= 0.721609f)
                                                    if (sensor0 <= 0.062424f)
                                                        return 6;
                                                    else
                                                        return 7;
                                                else
                                                    if (actual_vx <= -0.002028f)
                                                        return 6;
                                                    else
                                                        return 10;
                                            else
                                                if (sensor2 <= 0.770988f)
                                                    if (sensor0 <= 0.055571f)
                                                        return 7;
                                                    else
                                                        return 7;
                                                else
                                                    if (sensor0 <= 0.049422f)
                                                        return 8;
                                                    else
                                                        return 6;
                                else
                                    if (actual_vz <= 1.146262f)
                                        if (sensor4 <= 0.439833f)
                                            if (actual_vx <= -0.006694f)
                                                if (sensor1 <= 0.608333f)
                                                    if (sensor4 <= 0.328008f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor4 <= 0.404107f)
                                                        return 6;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor4 <= 0.437625f)
                                                    if (actual_vx <= -0.000541f)
                                                        return 6;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor1 <= 0.585983f)
                                                        return 7;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor2 <= 0.747512f)
                                                if (sensor2 <= 0.716135f)
                                                    if (actual_vz <= 1.069098f)
                                                        return 6;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor1 <= 0.560100f)
                                                        return 7;
                                                    else
                                                        return 6;
                                            else
                                                if (actual_omega <= 0.138086f)
                                                    if (actual_vx <= -0.003270f)
                                                        return 7;
                                                    else
                                                        return 10;
                                                else
                                                    if (actual_vx <= -0.004725f)
                                                        return 6;
                                                    else
                                                        return 5;
                                    else
                                        if (last_action_state <= 0.450000f)
                                            if (actual_vz <= 1.197810f)
                                                return 5;
                                            else
                                                if (actual_omega <= 0.153221f)
                                                    if (sensor5 <= 0.853680f)
                                                        return 5;
                                                    else
                                                        return 2;
                                                else
                                                    return 0;
                                        else
                                            if (actual_vz <= 1.152062f)
                                                if (sensor3 <= 0.031399f)
                                                    return 5;
                                                else
                                                    if (actual_omega <= 0.126766f)
                                                        return 8;
                                                    else
                                                        return 7;
                                            else
                                                if (last_action_state <= 0.550000f)
                                                    if (sensor0 <= 0.056555f)
                                                        return 7;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vz <= 1.188474f)
                                                        return 6;
                                                    else
                                                        return 7;
                        else
                            if (last_action_state <= 0.150000f)
                                if (actual_omega <= 0.187652f)
                                    if (sensor2 <= 0.784714f)
                                        if (actual_vx <= -0.015891f)
                                            return 5;
                                        else
                                            return 0;
                                    else
                                        if (actual_vx <= -0.012471f)
                                            if (sensor4 <= 0.281594f)
                                                if (sensor0 <= 0.029610f)
                                                    return 7;
                                                else
                                                    return 5;
                                            else
                                                return 0;
                                        else
                                            if (actual_omega <= 0.173769f)
                                                return 6;
                                            else
                                                return 5;
                                else
                                    if (actual_vz <= 1.187119f)
                                        if (sensor2 <= 0.745981f)
                                            return 5;
                                        else
                                            if (sensor4 <= 0.256292f)
                                                return 6;
                                            else
                                                if (actual_omega <= 0.239618f)
                                                    return 0;
                                                else
                                                    if (sensor2 <= 0.826930f)
                                                        return 0;
                                                    else
                                                        return 6;
                                    else
                                        if (sensor5 <= 0.853593f)
                                            if (actual_omega <= 0.209548f)
                                                if (actual_vz <= 1.251454f)
                                                    if (sensor1 <= 0.502639f)
                                                        return 0;
                                                    else
                                                        return 1;
                                                else
                                                    return 7;
                                            else
                                                if (sensor4 <= 0.280832f)
                                                    if (sensor3 <= 0.013459f)
                                                        return 0;
                                                    else
                                                        return 6;
                                                else
                                                    return 5;
                                        else
                                            if (actual_vz <= 1.208743f)
                                                return 6;
                                            else
                                                return 0;
                            else
                                if (sensor1 <= 0.461054f)
                                    if (sensor4 <= 0.421819f)
                                        if (actual_omega <= 0.227079f)
                                            if (actual_vx <= -0.010230f)
                                                if (sensor1 <= 0.390648f)
                                                    if (sensor1 <= 0.379597f)
                                                        return 7;
                                                    else
                                                        return 7;
                                                else
                                                    if (sensor2 <= 0.836457f)
                                                        return 7;
                                                    else
                                                        return 7;
                                            else
                                                if (actual_vx <= -0.009943f)
                                                    if (sensor1 <= 0.406257f)
                                                        return 8;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor3 <= 0.019024f)
                                                        return 7;
                                                    else
                                                        return 7;
                                        else
                                            if (last_action_state <= 0.550000f)
                                                if (sensor5 <= 0.827302f)
                                                    if (sensor3 <= 0.014932f)
                                                        return 5;
                                                    else
                                                        return 7;
                                                else
                                                    if (sensor0 <= 0.039717f)
                                                        return 6;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vz <= 1.083869f)
                                                    if (actual_omega <= 0.229261f)
                                                        return 5;
                                                    else
                                                        return 7;
                                                else
                                                    if (sensor2 <= 0.820726f)
                                                        return 6;
                                                    else
                                                        return 7;
                                    else
                                        if (actual_vx <= -0.013350f)
                                            if (sensor2 <= 0.833838f)
                                                return 8;
                                            else
                                                if (sensor4 <= 0.450884f)
                                                    if (sensor3 <= 0.027100f)
                                                        return 7;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor5 <= 0.799270f)
                                                        return 8;
                                                    else
                                                        return 7;
                                        else
                                            if (actual_omega <= 0.176505f)
                                                if (last_action_state <= 0.450000f)
                                                    return 7;
                                                else
                                                    return 8;
                                            else
                                                if (sensor0 <= 0.051185f)
                                                    if (actual_vx <= -0.006025f)
                                                        return 9;
                                                    else
                                                        return 8;
                                                else
                                                    return 7;
                                else
                                    if (sensor1 <= 0.540504f)
                                        if (sensor3 <= 0.022003f)
                                            if (actual_vx <= -0.019636f)
                                                if (sensor5 <= 0.824170f)
                                                    return 0;
                                                else
                                                    return 5;
                                            else
                                                if (actual_omega <= 0.172966f)
                                                    if (actual_vz <= 1.046191f)
                                                        return 6;
                                                    else
                                                        return 7;
                                                else
                                                    if (actual_omega <= 0.199884f)
                                                        return 6;
                                                    else
                                                        return 6;
                                        else
                                            if (actual_omega <= 0.244617f)
                                                if (sensor5 <= 0.850373f)
                                                    if (actual_vz <= 1.211648f)
                                                        return 6;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vz <= 1.238699f)
                                                        return 7;
                                                    else
                                                        return 8;
                                            else
                                                if (sensor4 <= 0.416021f)
                                                    if (actual_omega <= 0.264848f)
                                                        return 7;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor4 <= 0.436010f)
                                                        return 6;
                                                    else
                                                        return 6;
                                    else
                                        if (actual_omega <= 0.201916f)
                                            if (sensor3 <= 0.026756f)
                                                if (actual_omega <= 0.174167f)
                                                    return 6;
                                                else
                                                    if (sensor2 <= 0.751558f)
                                                        return 7;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vz <= 1.160514f)
                                                    if (actual_vz <= 1.159604f)
                                                        return 6;
                                                    else
                                                        return 7;
                                                else
                                                    if (actual_vx <= -0.010598f)
                                                        return 5;
                                                    else
                                                        return 6;
                                        else
                                            if (sensor4 <= 0.393062f)
                                                if (actual_vz <= 1.051539f)
                                                    return 5;
                                                else
                                                    if (actual_omega <= 0.220875f)
                                                        return 6;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_omega <= 0.250019f)
                                                    if (sensor5 <= 0.774523f)
                                                        return 7;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_omega <= 0.254596f)
                                                        return 7;
                                                    else
                                                        return 5;
                    else
                        if (actual_omega <= 0.228457f)
                            if (last_action_state <= 0.850000f)
                                if (sensor1 <= 0.448428f)
                                    if (actual_omega <= 0.192223f)
                                        if (sensor4 <= 0.289652f)
                                            if (sensor2 <= 0.827379f)
                                                if (actual_vx <= -0.004233f)
                                                    if (sensor1 <= 0.384071f)
                                                        return 8;
                                                    else
                                                        return 7;
                                                else
                                                    if (sensor0 <= 0.033441f)
                                                        return 10;
                                                    else
                                                        return 7;
                                            else
                                                if (sensor2 <= 0.827968f)
                                                    if (sensor3 <= 0.011673f)
                                                        return 10;
                                                    else
                                                        return 9;
                                                else
                                                    if (actual_vx <= -0.010141f)
                                                        return 9;
                                                    else
                                                        return 7;
                                        else
                                            if (actual_vz <= 1.138516f)
                                                if (sensor0 <= 0.028057f)
                                                    if (actual_vx <= -0.007412f)
                                                        return 9;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor3 <= 0.026308f)
                                                        return 7;
                                                    else
                                                        return 8;
                                            else
                                                if (actual_vx <= -0.004663f)
                                                    if (actual_omega <= 0.158744f)
                                                        return 9;
                                                    else
                                                        return 9;
                                                else
                                                    if (actual_vz <= 1.187971f)
                                                        return 8;
                                                    else
                                                        return 9;
                                    else
                                        if (last_action_state <= 0.750000f)
                                            if (actual_vz <= 1.179239f)
                                                if (sensor2 <= 0.816121f)
                                                    if (actual_vx <= -0.007873f)
                                                        return 7;
                                                    else
                                                        return 7;
                                                else
                                                    if (sensor2 <= 0.842581f)
                                                        return 7;
                                                    else
                                                        return 7;
                                            else
                                                if (sensor0 <= 0.025071f)
                                                    if (actual_vz <= 1.200297f)
                                                        return 8;
                                                    else
                                                        return 9;
                                                else
                                                    if (sensor1 <= 0.446259f)
                                                        return 6;
                                                    else
                                                        return 7;
                                        else
                                            if (sensor5 <= 0.815114f)
                                                if (sensor0 <= 0.043410f)
                                                    if (actual_vz <= 1.107037f)
                                                        return 6;
                                                    else
                                                        return 8;
                                                else
                                                    if (actual_omega <= 0.198472f)
                                                        return 9;
                                                    else
                                                        return 8;
                                            else
                                                if (actual_vz <= 1.040575f)
                                                    if (actual_omega <= 0.193708f)
                                                        return 9;
                                                    else
                                                        return 7;
                                                else
                                                    if (sensor4 <= 0.311115f)
                                                        return 10;
                                                    else
                                                        return 9;
                                else
                                    if (actual_omega <= 0.169308f)
                                        if (sensor1 <= 0.559170f)
                                            if (sensor4 <= 0.402399f)
                                                if (actual_vx <= -0.001530f)
                                                    if (actual_vz <= 1.071834f)
                                                        return 7;
                                                    else
                                                        return 7;
                                                else
                                                    if (sensor4 <= 0.335628f)
                                                        return 10;
                                                    else
                                                        return 10;
                                            else
                                                if (actual_omega <= 0.168574f)
                                                    if (actual_vx <= 0.003601f)
                                                        return 10;
                                                    else
                                                        return 8;
                                                else
                                                    if (sensor3 <= 0.024569f)
                                                        return 7;
                                                    else
                                                        return 8;
                                        else
                                            if (actual_vx <= -0.006953f)
                                                if (actual_vz <= 1.071568f)
                                                    if (actual_vz <= 1.045714f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_omega <= 0.167067f)
                                                        return 9;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor2 <= 0.700594f)
                                                    if (sensor1 <= 0.601931f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor4 <= 0.421712f)
                                                        return 7;
                                                    else
                                                        return 7;
                                    else
                                        if (sensor2 <= 0.824678f)
                                            if (sensor0 <= 0.052965f)
                                                if (sensor4 <= 0.331807f)
                                                    if (sensor5 <= 0.825978f)
                                                        return 6;
                                                    else
                                                        return 7;
                                                else
                                                    if (actual_vx <= -0.004554f)
                                                        return 7;
                                                    else
                                                        return 10;
                                            else
                                                if (sensor4 <= 0.498860f)
                                                    if (actual_omega <= 0.188668f)
                                                        return 6;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vx <= -0.001522f)
                                                        return 6;
                                                    else
                                                        return 7;
                                        else
                                            if (sensor4 <= 0.360422f)
                                                if (actual_omega <= 0.202338f)
                                                    if (sensor1 <= 0.466238f)
                                                        return 7;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vz <= 1.139089f)
                                                        return 6;
                                                    else
                                                        return 6;
                                            else
                                                if (actual_vx <= -0.011387f)
                                                    if (actual_omega <= 0.206782f)
                                                        return 7;
                                                    else
                                                        return 7;
                                                else
                                                    if (actual_vz <= 1.015025f)
                                                        return 7;
                                                    else
                                                        return 9;
                            else
                                if (sensor3 <= 0.268757f)
                                    if (sensor0 <= 0.036095f)
                                        if (actual_vx <= -0.004010f)
                                            if (sensor4 <= 0.290738f)
                                                if (sensor1 <= 0.459744f)
                                                    if (actual_omega <= 0.210571f)
                                                        return 9;
                                                    else
                                                        return 7;
                                                else
                                                    if (sensor4 <= 0.285809f)
                                                        return 6;
                                                    else
                                                        return 7;
                                            else
                                                if (actual_vz <= 1.227105f)
                                                    if (sensor1 <= 0.425658f)
                                                        return 9;
                                                    else
                                                        return 7;
                                                else
                                                    if (sensor0 <= 0.032496f)
                                                        return 7;
                                                    else
                                                        return 8;
                                        else
                                            if (actual_vz <= 1.162779f)
                                                if (sensor4 <= 0.311439f)
                                                    if (actual_omega <= 0.181682f)
                                                        return 10;
                                                    else
                                                        return 7;
                                                else
                                                    if (actual_omega <= 0.131637f)
                                                        return 6;
                                                    else
                                                        return 10;
                                            else
                                                if (sensor1 <= 0.466871f)
                                                    if (sensor0 <= 0.033193f)
                                                        return 9;
                                                    else
                                                        return 8;
                                                else
                                                    if (actual_omega <= 0.218016f)
                                                        return 9;
                                                    else
                                                        return 10;
                                    else
                                        if (sensor4 <= 0.333488f)
                                            if (sensor0 <= 0.046445f)
                                                if (actual_omega <= 0.220122f)
                                                    if (actual_vx <= -0.004290f)
                                                        return 7;
                                                    else
                                                        return 8;
                                                else
                                                    if (actual_vz <= 1.224005f)
                                                        return 7;
                                                    else
                                                        return 9;
                                            else
                                                if (sensor5 <= 0.826466f)
                                                    if (actual_vx <= -0.006851f)
                                                        return 0;
                                                    else
                                                        return 6;
                                                else
                                                    return 7;
                                        else
                                            if (sensor1 <= 0.589301f)
                                                if (sensor4 <= 0.434860f)
                                                    if (sensor2 <= 0.783265f)
                                                        return 7;
                                                    else
                                                        return 8;
                                                else
                                                    if (sensor0 <= 0.059183f)
                                                        return 8;
                                                    else
                                                        return 7;
                                            else
                                                if (actual_vz <= 1.184646f)
                                                    if (sensor4 <= 0.492041f)
                                                        return 6;
                                                    else
                                                        return 7;
                                                else
                                                    if (actual_omega <= 0.203234f)
                                                        return 9;
                                                    else
                                                        return 6;
                                else
                                    if (sensor4 <= 0.513272f)
                                        return 10;
                                    else
                                        return 9;
                        else
                            if (sensor1 <= 0.488029f)
                                if (actual_omega <= 0.274774f)
                                    if (last_action_state <= 0.850000f)
                                        if (sensor4 <= 0.336628f)
                                            if (actual_omega <= 0.272791f)
                                                if (sensor0 <= 0.043131f)
                                                    if (sensor4 <= 0.314502f)
                                                        return 7;
                                                    else
                                                        return 7;
                                                else
                                                    if (actual_vz <= 1.023564f)
                                                        return 10;
                                                    else
                                                        return 6;
                                            else
                                                return 7;
                                        else
                                            if (sensor5 <= 0.880140f)
                                                if (sensor0 <= 0.057734f)
                                                    if (sensor1 <= 0.437175f)
                                                        return 7;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vz <= 1.128604f)
                                                        return 7;
                                                    else
                                                        return 9;
                                            else
                                                if (sensor1 <= 0.394329f)
                                                    return 9;
                                                else
                                                    return 6;
                                    else
                                        if (sensor1 <= 0.391646f)
                                            if (actual_vz <= 1.144856f)
                                                if (sensor0 <= 0.022520f)
                                                    if (actual_vz <= 1.121827f)
                                                        return 9;
                                                    else
                                                        return 7;
                                                else
                                                    if (sensor0 <= 0.023816f)
                                                        return 6;
                                                    else
                                                        return 9;
                                            else
                                                if (sensor1 <= 0.385430f)
                                                    if (actual_omega <= 0.253932f)
                                                        return 9;
                                                    else
                                                        return 8;
                                                else
                                                    if (last_action_state <= 0.950000f)
                                                        return 10;
                                                    else
                                                        return 9;
                                        else
                                            if (sensor4 <= 0.280054f)
                                                if (sensor4 <= 0.260415f)
                                                    if (sensor5 <= 0.813423f)
                                                        return 7;
                                                    else
                                                        return 7;
                                                else
                                                    if (sensor5 <= 0.824753f)
                                                        return 6;
                                                    else
                                                        return 7;
                                            else
                                                if (actual_vx <= -0.009171f)
                                                    if (sensor4 <= 0.391324f)
                                                        return 7;
                                                    else
                                                        return 7;
                                                else
                                                    if (sensor4 <= 0.284179f)
                                                        return 8;
                                                    else
                                                        return 7;
                                else
                                    if (sensor3 <= 0.028354f)
                                        if (sensor1 <= 0.466641f)
                                            if (sensor3 <= 0.013494f)
                                                if (actual_vz <= 1.227797f)
                                                    if (actual_vz <= 1.160242f)
                                                        return 6;
                                                    else
                                                        return 7;
                                                else
                                                    if (sensor1 <= 0.395211f)
                                                        return 9;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor1 <= 0.401524f)
                                                    if (sensor2 <= 0.834549f)
                                                        return 6;
                                                    else
                                                        return 8;
                                                else
                                                    if (sensor1 <= 0.463938f)
                                                        return 7;
                                                    else
                                                        return 7;
                                        else
                                            if (sensor2 <= 0.841546f)
                                                if (actual_vz <= 1.170092f)
                                                    if (sensor1 <= 0.486713f)
                                                        return 6;
                                                    else
                                                        return 7;
                                                else
                                                    if (sensor5 <= 0.852034f)
                                                        return 7;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor0 <= 0.049605f)
                                                    return 7;
                                                else
                                                    if (sensor4 <= 0.420353f)
                                                        return 6;
                                                    else
                                                        return 7;
                                    else
                                        if (sensor2 <= 0.834695f)
                                            if (sensor2 <= 0.832197f)
                                                if (actual_vx <= -0.017802f)
                                                    if (actual_vz <= 1.178576f)
                                                        return 6;
                                                    else
                                                        return 8;
                                                else
                                                    if (sensor1 <= 0.482293f)
                                                        return 9;
                                                    else
                                                        return 7;
                                            else
                                                if (sensor3 <= 0.029038f)
                                                    if (sensor4 <= 0.454222f)
                                                        return 8;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor5 <= 0.814893f)
                                                        return 9;
                                                    else
                                                        return 6;
                                        else
                                            if (sensor4 <= 0.493509f)
                                                if (actual_omega <= 0.301961f)
                                                    if (actual_vx <= -0.017754f)
                                                        return 7;
                                                    else
                                                        return 9;
                                                else
                                                    if (actual_omega <= 0.322534f)
                                                        return 6;
                                                    else
                                                        return 7;
                                            else
                                                if (actual_vz <= 1.146030f)
                                                    if (sensor5 <= 0.775709f)
                                                        return 6;
                                                    else
                                                        return 8;
                                                else
                                                    return 9;
                            else
                                if (sensor1 <= 0.593458f)
                                    if (sensor4 <= 0.473624f)
                                        if (actual_omega <= 0.267620f)
                                            if (last_action_state <= 0.850000f)
                                                if (sensor4 <= 0.420990f)
                                                    if (actual_vz <= 1.182023f)
                                                        return 6;
                                                    else
                                                        return 7;
                                                else
                                                    if (sensor4 <= 0.471673f)
                                                        return 7;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor4 <= 0.350724f)
                                                    if (sensor1 <= 0.489571f)
                                                        return 6;
                                                    else
                                                        return 7;
                                                else
                                                    if (sensor1 <= 0.499306f)
                                                        return 6;
                                                    else
                                                        return 6;
                                        else
                                            if (actual_vz <= 1.207430f)
                                                if (sensor1 <= 0.496557f)
                                                    if (sensor0 <= 0.057776f)
                                                        return 6;
                                                    else
                                                        return 7;
                                                else
                                                    if (actual_omega <= 0.311339f)
                                                        return 6;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor1 <= 0.503451f)
                                                    if (sensor1 <= 0.496245f)
                                                        return 6;
                                                    else
                                                        return 7;
                                                else
                                                    if (sensor5 <= 0.844085f)
                                                        return 8;
                                                    else
                                                        return 6;
                                    else
                                        if (actual_vx <= -0.017328f)
                                            if (last_action_state <= 0.950000f)
                                                if (actual_omega <= 0.270358f)
                                                    if (sensor0 <= 0.071697f)
                                                        return 6;
                                                    else
                                                        return 7;
                                                else
                                                    if (sensor4 <= 0.506861f)
                                                        return 7;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor0 <= 0.060227f)
                                                    return 6;
                                                else
                                                    if (sensor1 <= 0.500776f)
                                                        return 8;
                                                    else
                                                        return 7;
                                        else
                                            if (sensor2 <= 0.841359f)
                                                if (actual_vz <= 1.114085f)
                                                    if (actual_vx <= -0.017090f)
                                                        return 10;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor3 <= 0.062317f)
                                                        return 8;
                                                    else
                                                        return 7;
                                            else
                                                if (last_action_state <= 0.850000f)
                                                    if (actual_omega <= 0.266154f)
                                                        return 7;
                                                    else
                                                        return 10;
                                                else
                                                    if (actual_omega <= 0.270901f)
                                                        return 8;
                                                    else
                                                        return 9;
                                else
                                    if (sensor4 <= 0.510720f)
                                        if (actual_vz <= 1.178239f)
                                            if (actual_omega <= 0.252784f)
                                                if (sensor0 <= 0.066725f)
                                                    if (sensor0 <= 0.063825f)
                                                        return 6;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor5 <= 0.846574f)
                                                        return 6;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor4 <= 0.475838f)
                                                    if (sensor5 <= 0.829645f)
                                                        return 8;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vz <= 1.169812f)
                                                        return 7;
                                                    else
                                                        return 6;
                                        else
                                            if (actual_vx <= -0.008526f)
                                                if (sensor3 <= 0.065313f)
                                                    if (sensor1 <= 0.596736f)
                                                        return 6;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor5 <= 0.829506f)
                                                        return 6;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor3 <= 0.056644f)
                                                    if (actual_vx <= -0.006840f)
                                                        return 7;
                                                    else
                                                        return 8;
                                                else
                                                    if (sensor4 <= 0.504313f)
                                                        return 6;
                                                    else
                                                        return 8;
                                    else
                                        if (sensor2 <= 0.704878f)
                                            if (sensor4 <= 0.511402f)
                                                return 9;
                                            else
                                                if (sensor2 <= 0.663301f)
                                                    return 6;
                                                else
                                                    if (sensor2 <= 0.670483f)
                                                        return 5;
                                                    else
                                                        return 5;
                                        else
                                            if (actual_vx <= -0.005726f)
                                                return 7;
                                            else
                                                return 6;
                else
                    if (sensor2 <= 0.692407f)
                        if (last_action_state <= 0.350000f)
                            if (actual_vx <= -0.003971f)
                                if (sensor4 <= 0.907947f)
                                    if (sensor4 <= 0.531731f)
                                        return 5;
                                    else
                                        if (actual_omega <= 0.165551f)
                                            if (actual_omega <= 0.159573f)
                                                if (sensor2 <= 0.480441f)
                                                    if (actual_vz <= 1.329271f)
                                                        return 0;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_omega <= 0.117059f)
                                                        return 0;
                                                    else
                                                        return 0;
                                            else
                                                return 5;
                                        else
                                            if (actual_omega <= 0.223545f)
                                                if (sensor5 <= 0.631646f)
                                                    return 5;
                                                else
                                                    if (actual_vx <= -0.006521f)
                                                        return 0;
                                                    else
                                                        return 0;
                                            else
                                                return 2;
                                else
                                    return 5;
                            else
                                if (sensor1 <= 0.745935f)
                                    if (actual_vz <= 1.213430f)
                                        if (sensor2 <= 0.674525f)
                                            if (sensor4 <= 0.722683f)
                                                if (sensor5 <= 0.809834f)
                                                    return 5;
                                                else
                                                    if (actual_omega <= 0.112129f)
                                                        return 0;
                                                    else
                                                        return 5;
                                            else
                                                return 0;
                                        else
                                            return 6;
                                    else
                                        if (actual_omega <= 0.043350f)
                                            if (actual_vz <= 1.358472f)
                                                if (actual_vz <= 1.293472f)
                                                    if (actual_vz <= 1.249380f)
                                                        return 7;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vz <= 1.334376f)
                                                        return 5;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor5 <= 0.783081f)
                                                    return 0;
                                                else
                                                    return 8;
                                        else
                                            if (sensor1 <= 0.709342f)
                                                if (sensor5 <= 0.801371f)
                                                    return 7;
                                                else
                                                    if (actual_omega <= 0.074357f)
                                                        return 1;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vx <= -0.000748f)
                                                    return 0;
                                                else
                                                    if (sensor3 <= 0.209684f)
                                                        return 5;
                                                    else
                                                        return 6;
                                else
                                    if (actual_vx <= -0.002255f)
                                        if (sensor3 <= 0.268095f)
                                            if (sensor2 <= 0.553214f)
                                                if (sensor4 <= 0.792788f)
                                                    return 0;
                                                else
                                                    if (sensor1 <= 0.769533f)
                                                        return 1;
                                                    else
                                                        return 0;
                                            else
                                                return 5;
                                        else
                                            if (actual_vx <= -0.002326f)
                                                if (actual_vx <= -0.003363f)
                                                    if (actual_vx <= -0.003878f)
                                                        return 5;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_vz <= 1.293419f)
                                                        return 5;
                                                    else
                                                        return 0;
                                            else
                                                return 0;
                                    else
                                        if (sensor5 <= 0.756579f)
                                            if (actual_vx <= 0.001978f)
                                                if (actual_omega <= 0.059063f)
                                                    return 5;
                                                else
                                                    if (sensor2 <= 0.497068f)
                                                        return 1;
                                                    else
                                                        return 5;
                                            else
                                                return 7;
                                        else
                                            return 1;
                        else
                            if (sensor5 <= 0.244974f)
                                if (actual_vx <= -0.022917f)
                                    if (sensor5 <= 0.158915f)
                                        if (sensor3 <= 0.673637f)
                                            return 5;
                                        else
                                            return 9;
                                    else
                                        if (actual_omega <= 0.164333f)
                                            return 10;
                                        else
                                            if (sensor5 <= 0.165643f)
                                                return 10;
                                            else
                                                if (actual_vx <= -0.040613f)
                                                    return 5;
                                                else
                                                    if (actual_vz <= 0.948480f)
                                                        return 6;
                                                    else
                                                        return 8;
                                else
                                    if (actual_omega <= 0.184380f)
                                        if (sensor0 <= 0.095840f)
                                            if (sensor4 <= 0.995950f)
                                                if (actual_vz <= 0.495310f)
                                                    if (actual_omega <= 0.073397f)
                                                        return 10;
                                                    else
                                                        return 9;
                                                else
                                                    if (sensor2 <= 0.564976f)
                                                        return 10;
                                                    else
                                                        return 10;
                                            else
                                                if (actual_vz <= 0.795681f)
                                                    return 10;
                                                else
                                                    return 8;
                                        else
                                            if (actual_vz <= 0.604089f)
                                                if (sensor2 <= 0.430220f)
                                                    if (sensor3 <= 0.509289f)
                                                        return 5;
                                                    else
                                                        return 10;
                                                else
                                                    return 5;
                                            else
                                                if (actual_vz <= 0.737810f)
                                                    if (actual_omega <= 0.156328f)
                                                        return 10;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vz <= 0.871430f)
                                                        return 10;
                                                    else
                                                        return 9;
                                    else
                                        if (actual_vx <= -0.007915f)
                                            if (sensor2 <= 0.457498f)
                                                if (actual_vx <= -0.014546f)
                                                    if (actual_vz <= 0.896986f)
                                                        return 7;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor5 <= 0.194458f)
                                                        return 5;
                                                    else
                                                        return 7;
                                            else
                                                if (sensor5 <= 0.140284f)
                                                    if (actual_vz <= 0.816359f)
                                                        return 8;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor1 <= 0.610543f)
                                                        return 8;
                                                    else
                                                        return 10;
                                        else
                                            if (sensor5 <= 0.115524f)
                                                if (actual_vx <= -0.006680f)
                                                    return 10;
                                                else
                                                    if (sensor5 <= 0.096565f)
                                                        return 5;
                                                    else
                                                        return 8;
                                            else
                                                if (actual_vz <= 0.847636f)
                                                    if (actual_omega <= 0.187772f)
                                                        return 8;
                                                    else
                                                        return 10;
                                                else
                                                    if (last_action_state <= 0.900000f)
                                                        return 10;
                                                    else
                                                        return 9;
                            else
                                if (actual_vx <= -0.001948f)
                                    if (last_action_state <= 0.650000f)
                                        if (sensor4 <= 0.944136f)
                                            if (actual_omega <= 0.123939f)
                                                if (actual_vz <= 1.078062f)
                                                    if (actual_omega <= 0.104263f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vz <= 1.250963f)
                                                        return 6;
                                                    else
                                                        return 6;
                                            else
                                                if (last_action_state <= 0.550000f)
                                                    if (actual_omega <= 0.171132f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vz <= 1.063607f)
                                                        return 6;
                                                    else
                                                        return 6;
                                        else
                                            if (sensor1 <= 0.697706f)
                                                if (actual_vx <= -0.010523f)
                                                    if (sensor5 <= 0.261506f)
                                                        return 5;
                                                    else
                                                        return 7;
                                                else
                                                    return 10;
                                            else
                                                if (sensor2 <= 0.416237f)
                                                    return 8;
                                                else
                                                    if (actual_vz <= 1.191375f)
                                                        return 5;
                                                    else
                                                        return 6;
                                    else
                                        if (sensor5 <= 0.426734f)
                                            if (actual_vx <= -0.020300f)
                                                if (sensor3 <= 0.705118f)
                                                    if (actual_vx <= -0.025733f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor1 <= 0.668599f)
                                                        return 8;
                                                    else
                                                        return 6;
                                            else
                                                if (actual_vx <= -0.009566f)
                                                    if (sensor0 <= 0.207431f)
                                                        return 8;
                                                    else
                                                        return 8;
                                                else
                                                    if (sensor2 <= 0.619597f)
                                                        return 10;
                                                    else
                                                        return 8;
                                        else
                                            if (actual_omega <= 0.210117f)
                                                if (last_action_state <= 0.850000f)
                                                    if (sensor1 <= 0.782879f)
                                                        return 6;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vz <= 1.106377f)
                                                        return 6;
                                                    else
                                                        return 8;
                                            else
                                                if (sensor1 <= 0.781025f)
                                                    if (actual_vx <= -0.004667f)
                                                        return 6;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor3 <= 0.308644f)
                                                        return 5;
                                                    else
                                                        return 9;
                                else
                                    if (last_action_state <= 0.850000f)
                                        if (actual_vx <= -0.000643f)
                                            if (last_action_state <= 0.750000f)
                                                if (sensor5 <= 0.587846f)
                                                    if (actual_omega <= 0.115267f)
                                                        return 9;
                                                    else
                                                        return 8;
                                                else
                                                    if (sensor1 <= 0.778596f)
                                                        return 6;
                                                    else
                                                        return 6;
                                            else
                                                if (actual_omega <= 0.121541f)
                                                    if (sensor1 <= 0.755012f)
                                                        return 7;
                                                    else
                                                        return 8;
                                                else
                                                    if (actual_vz <= 1.205550f)
                                                        return 6;
                                                    else
                                                        return 8;
                                        else
                                            if (actual_omega <= 0.106355f)
                                                if (actual_vz <= 1.046649f)
                                                    if (sensor5 <= 0.693868f)
                                                        return 10;
                                                    else
                                                        return 9;
                                                else
                                                    if (actual_vx <= 0.001913f)
                                                        return 8;
                                                    else
                                                        return 8;
                                            else
                                                if (actual_vz <= 1.210988f)
                                                    if (sensor4 <= 0.968515f)
                                                        return 6;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor1 <= 0.744368f)
                                                        return 8;
                                                    else
                                                        return 6;
                                    else
                                        if (sensor4 <= 0.622982f)
                                            if (sensor1 <= 0.600217f)
                                                return 9;
                                            else
                                                if (sensor1 <= 0.648750f)
                                                    if (sensor4 <= 0.553544f)
                                                        return 8;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor2 <= 0.630213f)
                                                        return 8;
                                                    else
                                                        return 6;
                                        else
                                            if (actual_vz <= 1.188807f)
                                                if (sensor1 <= 0.744155f)
                                                    if (actual_omega <= 0.154821f)
                                                        return 8;
                                                    else
                                                        return 8;
                                                else
                                                    if (sensor3 <= 0.224829f)
                                                        return 6;
                                                    else
                                                        return 8;
                                            else
                                                if (sensor5 <= 0.547431f)
                                                    if (actual_vz <= 1.214549f)
                                                        return 9;
                                                    else
                                                        return 8;
                                                else
                                                    if (actual_omega <= 0.214233f)
                                                        return 8;
                                                    else
                                                        return 6;
                    else
                        if (last_action_state <= 0.650000f)
                            if (actual_omega <= 0.172948f)
                                if (sensor4 <= 0.693099f)
                                    if (sensor1 <= 0.619052f)
                                        if (actual_vx <= -0.017402f)
                                            if (sensor5 <= 0.647412f)
                                                if (sensor1 <= 0.608772f)
                                                    if (actual_vz <= 1.016136f)
                                                        return 6;
                                                    else
                                                        return 9;
                                                else
                                                    if (sensor5 <= 0.626323f)
                                                        return 6;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vx <= -0.021570f)
                                                    return 6;
                                                else
                                                    if (sensor4 <= 0.646348f)
                                                        return 7;
                                                    else
                                                        return 8;
                                        else
                                            if (sensor3 <= 0.046785f)
                                                if (sensor5 <= 0.760664f)
                                                    if (actual_omega <= 0.149521f)
                                                        return 10;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor2 <= 0.849620f)
                                                        return 9;
                                                    else
                                                        return 8;
                                            else
                                                if (actual_omega <= 0.137030f)
                                                    if (sensor2 <= 0.731064f)
                                                        return 7;
                                                    else
                                                        return 9;
                                                else
                                                    if (sensor1 <= 0.543851f)
                                                        return 9;
                                                    else
                                                        return 7;
                                    else
                                        if (actual_omega <= 0.136611f)
                                            if (actual_vx <= -0.004722f)
                                                if (actual_vx <= -0.005353f)
                                                    if (sensor3 <= 0.080788f)
                                                        return 8;
                                                    else
                                                        return 5;
                                                else
                                                    return 8;
                                            else
                                                if (actual_vx <= -0.004196f)
                                                    return 7;
                                                else
                                                    if (sensor3 <= 0.094711f)
                                                        return 6;
                                                    else
                                                        return 7;
                                        else
                                            if (sensor3 <= 0.103672f)
                                                if (actual_omega <= 0.147918f)
                                                    if (actual_omega <= 0.146700f)
                                                        return 6;
                                                    else
                                                        return 7;
                                                else
                                                    if (actual_omega <= 0.169757f)
                                                        return 6;
                                                    else
                                                        return 7;
                                            else
                                                if (actual_vx <= -0.008268f)
                                                    return 10;
                                                else
                                                    return 7;
                                else
                                    if (actual_vx <= -0.018385f)
                                        if (actual_vx <= -0.018650f)
                                            if (sensor0 <= 0.180527f)
                                                if (sensor4 <= 0.812498f)
                                                    if (sensor2 <= 0.813367f)
                                                        return 8;
                                                    else
                                                        return 8;
                                                else
                                                    return 8;
                                            else
                                                if (sensor4 <= 0.828494f)
                                                    if (sensor3 <= 0.188543f)
                                                        return 5;
                                                    else
                                                        return 8;
                                                else
                                                    if (last_action_state <= 0.550000f)
                                                        return 6;
                                                    else
                                                        return 6;
                                        else
                                            if (actual_omega <= 0.155569f)
                                                if (sensor5 <= 0.480889f)
                                                    if (sensor2 <= 0.726399f)
                                                        return 7;
                                                    else
                                                        return 8;
                                                else
                                                    return 6;
                                            else
                                                if (actual_omega <= 0.167854f)
                                                    if (actual_vz <= 1.159972f)
                                                        return 7;
                                                    else
                                                        return 8;
                                                else
                                                    if (actual_omega <= 0.171971f)
                                                        return 6;
                                                    else
                                                        return 10;
                                    else
                                        if (sensor2 <= 0.728278f)
                                            if (actual_vz <= 1.179446f)
                                                if (actual_vx <= -0.012893f)
                                                    if (sensor1 <= 0.739947f)
                                                        return 7;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor5 <= 0.391204f)
                                                        return 10;
                                                    else
                                                        return 8;
                                            else
                                                if (sensor1 <= 0.742445f)
                                                    if (actual_vx <= -0.015853f)
                                                        return 6;
                                                    else
                                                        return 7;
                                                else
                                                    if (sensor2 <= 0.695334f)
                                                        return 6;
                                                    else
                                                        return 8;
                                        else
                                            if (sensor1 <= 0.619605f)
                                                if (actual_vx <= -0.018010f)
                                                    return 8;
                                                else
                                                    if (sensor0 <= 0.105873f)
                                                        return 7;
                                                    else
                                                        return 9;
                                            else
                                                if (sensor2 <= 0.730875f)
                                                    if (sensor0 <= 0.191397f)
                                                        return 9;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vz <= 1.061213f)
                                                        return 8;
                                                    else
                                                        return 8;
                            else
                                if (sensor0 <= 0.143501f)
                                    if (sensor4 <= 0.631778f)
                                        if (sensor1 <= 0.516080f)
                                            if (sensor2 <= 0.857904f)
                                                if (sensor2 <= 0.842254f)
                                                    if (actual_vx <= -0.016684f)
                                                        return 9;
                                                    else
                                                        return 8;
                                                else
                                                    if (actual_vz <= 1.116772f)
                                                        return 6;
                                                    else
                                                        return 8;
                                            else
                                                if (sensor2 <= 0.862145f)
                                                    return 7;
                                                else
                                                    if (sensor1 <= 0.489290f)
                                                        return 10;
                                                    else
                                                        return 7;
                                        else
                                            if (sensor5 <= 0.753963f)
                                                if (sensor0 <= 0.088225f)
                                                    if (sensor4 <= 0.576722f)
                                                        return 7;
                                                    else
                                                        return 8;
                                                else
                                                    if (actual_vz <= 1.155353f)
                                                        return 6;
                                                    else
                                                        return 9;
                                            else
                                                if (actual_omega <= 0.180627f)
                                                    if (sensor0 <= 0.073595f)
                                                        return 6;
                                                    else
                                                        return 7;
                                                else
                                                    if (sensor0 <= 0.084698f)
                                                        return 6;
                                                    else
                                                        return 6;
                                    else
                                        if (sensor0 <= 0.100285f)
                                            if (sensor2 <= 0.834188f)
                                                if (actual_vx <= -0.022123f)
                                                    if (actual_omega <= 0.206446f)
                                                        return 8;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vz <= 1.039690f)
                                                        return 6;
                                                    else
                                                        return 8;
                                            else
                                                if (sensor4 <= 0.649781f)
                                                    if (last_action_state <= 0.550000f)
                                                        return 7;
                                                    else
                                                        return 8;
                                                else
                                                    if (sensor4 <= 0.708825f)
                                                        return 9;
                                                    else
                                                        return 7;
                                        else
                                            if (actual_omega <= 0.223998f)
                                                if (sensor3 <= 0.094221f)
                                                    if (sensor0 <= 0.116598f)
                                                        return 7;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor1 <= 0.628785f)
                                                        return 8;
                                                    else
                                                        return 8;
                                            else
                                                if (sensor0 <= 0.138776f)
                                                    if (actual_vx <= -0.020353f)
                                                        return 6;
                                                    else
                                                        return 8;
                                                else
                                                    if (sensor3 <= 0.122085f)
                                                        return 7;
                                                    else
                                                        return 10;
                                else
                                    if (sensor5 <= 0.492780f)
                                        if (sensor0 <= 0.181701f)
                                            if (actual_vx <= -0.019017f)
                                                if (sensor4 <= 0.833159f)
                                                    if (sensor0 <= 0.156173f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor0 <= 0.178862f)
                                                        return 8;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor3 <= 0.205011f)
                                                    if (sensor4 <= 0.837425f)
                                                        return 8;
                                                    else
                                                        return 6;
                                                else
                                                    return 9;
                                        else
                                            if (sensor2 <= 0.733055f)
                                                if (sensor4 <= 0.844959f)
                                                    if (actual_vz <= 1.202089f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor0 <= 0.195476f)
                                                        return 8;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor1 <= 0.744489f)
                                                    return 6;
                                                else
                                                    if (sensor3 <= 0.176205f)
                                                        return 8;
                                                    else
                                                        return 6;
                                    else
                                        if (actual_vx <= -0.022036f)
                                            if (sensor1 <= 0.721667f)
                                                if (actual_omega <= 0.221876f)
                                                    if (sensor1 <= 0.645278f)
                                                        return 8;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_omega <= 0.240874f)
                                                        return 7;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor3 <= 0.142689f)
                                                    if (actual_vx <= -0.024936f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    return 7;
                                        else
                                            if (sensor2 <= 0.751619f)
                                                if (actual_omega <= 0.173959f)
                                                    return 7;
                                                else
                                                    if (actual_vx <= -0.021185f)
                                                        return 7;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor5 <= 0.531442f)
                                                    if (actual_omega <= 0.217395f)
                                                        return 8;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vz <= 1.081605f)
                                                        return 6;
                                                    else
                                                        return 6;
                        else
                            if (actual_vx <= -0.020520f)
                                if (actual_omega <= 0.244979f)
                                    if (actual_vz <= 1.052713f)
                                        if (sensor5 <= 0.697370f)
                                            if (sensor0 <= 0.128831f)
                                                if (actual_vz <= 1.041209f)
                                                    if (actual_vz <= 0.995863f)
                                                        return 6;
                                                    else
                                                        return 8;
                                                else
                                                    if (actual_vx <= -0.023390f)
                                                        return 9;
                                                    else
                                                        return 8;
                                            else
                                                if (actual_vz <= 1.029482f)
                                                    if (sensor0 <= 0.166789f)
                                                        return 9;
                                                    else
                                                        return 8;
                                                else
                                                    if (actual_omega <= 0.200121f)
                                                        return 9;
                                                    else
                                                        return 6;
                                        else
                                            if (sensor0 <= 0.080603f)
                                                if (actual_vz <= 1.004039f)
                                                    if (sensor2 <= 0.846624f)
                                                        return 5;
                                                    else
                                                        return 9;
                                                else
                                                    if (actual_vz <= 1.009389f)
                                                        return 8;
                                                    else
                                                        return 7;
                                            else
                                                if (sensor5 <= 0.702995f)
                                                    return 6;
                                                else
                                                    if (sensor5 <= 0.713656f)
                                                        return 7;
                                                    else
                                                        return 6;
                                    else
                                        if (sensor1 <= 0.684748f)
                                            if (actual_vz <= 1.199287f)
                                                if (last_action_state <= 0.750000f)
                                                    if (sensor5 <= 0.726095f)
                                                        return 6;
                                                    else
                                                        return 7;
                                                else
                                                    if (sensor1 <= 0.547264f)
                                                        return 9;
                                                    else
                                                        return 8;
                                            else
                                                if (actual_vx <= -0.022476f)
                                                    if (sensor1 <= 0.618113f)
                                                        return 8;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor5 <= 0.594200f)
                                                        return 8;
                                                    else
                                                        return 8;
                                        else
                                            if (actual_omega <= 0.219334f)
                                                if (sensor1 <= 0.696744f)
                                                    if (actual_vz <= 1.201558f)
                                                        return 6;
                                                    else
                                                        return 7;
                                                else
                                                    if (sensor2 <= 0.732459f)
                                                        return 9;
                                                    else
                                                        return 8;
                                            else
                                                if (sensor2 <= 0.748629f)
                                                    if (actual_vz <= 1.183430f)
                                                        return 6;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor0 <= 0.160927f)
                                                        return 6;
                                                    else
                                                        return 9;
                                else
                                    if (actual_vz <= 1.165065f)
                                        if (actual_vx <= -0.026794f)
                                            if (actual_omega <= 0.270752f)
                                                if (actual_vx <= -0.027328f)
                                                    if (actual_vx <= -0.029964f)
                                                        return 7;
                                                    else
                                                        return 8;
                                                else
                                                    if (actual_vx <= -0.027087f)
                                                        return 6;
                                                    else
                                                        return 9;
                                            else
                                                if (actual_vz <= 1.051108f)
                                                    if (sensor5 <= 0.668685f)
                                                        return 7;
                                                    else
                                                        return 8;
                                                else
                                                    if (sensor2 <= 0.776516f)
                                                        return 7;
                                                    else
                                                        return 6;
                                        else
                                            if (last_action_state <= 0.750000f)
                                                if (actual_vz <= 1.155866f)
                                                    if (actual_vz <= 1.093503f)
                                                        return 7;
                                                    else
                                                        return 6;
                                                else
                                                    return 5;
                                            else
                                                if (sensor2 <= 0.793214f)
                                                    if (actual_vx <= -0.024812f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor4 <= 0.781168f)
                                                        return 6;
                                                    else
                                                        return 8;
                                    else
                                        if (sensor2 <= 0.829236f)
                                            if (actual_omega <= 0.282643f)
                                                if (sensor2 <= 0.806578f)
                                                    if (sensor3 <= 0.111406f)
                                                        return 6;
                                                    else
                                                        return 8;
                                                else
                                                    if (sensor4 <= 0.618531f)
                                                        return 7;
                                                    else
                                                        return 8;
                                            else
                                                if (actual_omega <= 0.321811f)
                                                    if (sensor5 <= 0.635130f)
                                                        return 6;
                                                    else
                                                        return 6;
                                                else
                                                    return 7;
                                        else
                                            if (actual_omega <= 0.256393f)
                                                if (sensor3 <= 0.072949f)
                                                    return 6;
                                                else
                                                    if (actual_vz <= 1.204884f)
                                                        return 9;
                                                    else
                                                        return 7;
                                            else
                                                if (actual_vz <= 1.174814f)
                                                    if (sensor5 <= 0.714646f)
                                                        return 8;
                                                    else
                                                        return 6;
                                                else
                                                    return 8;
                            else
                                if (sensor5 <= 0.641392f)
                                    if (sensor2 <= 0.762534f)
                                        if (actual_vx <= -0.018165f)
                                            if (sensor0 <= 0.201324f)
                                                if (actual_omega <= 0.242253f)
                                                    if (actual_omega <= 0.183278f)
                                                        return 8;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor0 <= 0.184744f)
                                                        return 8;
                                                    else
                                                        return 8;
                                            else
                                                if (sensor1 <= 0.769696f)
                                                    if (sensor4 <= 0.851503f)
                                                        return 6;
                                                    else
                                                        return 9;
                                                else
                                                    if (sensor2 <= 0.695582f)
                                                        return 6;
                                                    else
                                                        return 5;
                                        else
                                            if (actual_vz <= 1.095325f)
                                                if (actual_omega <= 0.140247f)
                                                    if (sensor5 <= 0.506662f)
                                                        return 8;
                                                    else
                                                        return 8;
                                                else
                                                    if (actual_vz <= 1.043268f)
                                                        return 6;
                                                    else
                                                        return 8;
                                            else
                                                if (actual_omega <= 0.174957f)
                                                    if (actual_vz <= 1.251305f)
                                                        return 8;
                                                    else
                                                        return 8;
                                                else
                                                    if (sensor1 <= 0.769955f)
                                                        return 8;
                                                    else
                                                        return 6;
                                    else
                                        if (actual_vz <= 1.038018f)
                                            if (sensor0 <= 0.106140f)
                                                if (sensor4 <= 0.693599f)
                                                    if (sensor2 <= 0.847278f)
                                                        return 9;
                                                    else
                                                        return 8;
                                                else
                                                    if (sensor5 <= 0.636930f)
                                                        return 10;
                                                    else
                                                        return 9;
                                            else
                                                if (last_action_state <= 0.750000f)
                                                    if (actual_vx <= -0.018715f)
                                                        return 6;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor0 <= 0.112402f)
                                                        return 8;
                                                    else
                                                        return 8;
                                        else
                                            if (sensor1 <= 0.632527f)
                                                if (sensor2 <= 0.821983f)
                                                    if (last_action_state <= 0.850000f)
                                                        return 8;
                                                    else
                                                        return 8;
                                                else
                                                    if (sensor1 <= 0.539827f)
                                                        return 9;
                                                    else
                                                        return 8;
                                            else
                                                if (actual_omega <= 0.233235f)
                                                    if (actual_vz <= 1.187837f)
                                                        return 8;
                                                    else
                                                        return 8;
                                                else
                                                    if (last_action_state <= 0.950000f)
                                                        return 8;
                                                    else
                                                        return 6;
                                else
                                    if (actual_vz <= 1.148011f)
                                        if (actual_vx <= -0.016412f)
                                            if (sensor3 <= 0.065544f)
                                                if (actual_omega <= 0.306679f)
                                                    if (sensor0 <= 0.069240f)
                                                        return 9;
                                                    else
                                                        return 7;
                                                else
                                                    if (sensor5 <= 0.716162f)
                                                        return 5;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor1 <= 0.588439f)
                                                    if (sensor4 <= 0.701005f)
                                                        return 8;
                                                    else
                                                        return 9;
                                                else
                                                    if (sensor3 <= 0.080026f)
                                                        return 8;
                                                    else
                                                        return 9;
                                        else
                                            if (last_action_state <= 0.850000f)
                                                if (sensor1 <= 0.522123f)
                                                    if (sensor3 <= 0.058311f)
                                                        return 9;
                                                    else
                                                        return 9;
                                                else
                                                    if (sensor4 <= 0.646173f)
                                                        return 8;
                                                    else
                                                        return 9;
                                            else
                                                if (sensor3 <= 0.093237f)
                                                    if (actual_omega <= 0.257791f)
                                                        return 9;
                                                    else
                                                        return 8;
                                                else
                                                    if (actual_omega <= 0.221036f)
                                                        return 8;
                                                    else
                                                        return 5;
                                    else
                                        if (sensor1 <= 0.609746f)
                                            if (sensor0 <= 0.061661f)
                                                if (sensor0 <= 0.058797f)
                                                    if (sensor4 <= 0.576378f)
                                                        return 8;
                                                    else
                                                        return 8;
                                                else
                                                    if (actual_vz <= 1.170545f)
                                                        return 10;
                                                    else
                                                        return 8;
                                            else
                                                if (actual_vz <= 1.164672f)
                                                    if (actual_vx <= -0.013116f)
                                                        return 8;
                                                    else
                                                        return 9;
                                                else
                                                    if (sensor1 <= 0.595624f)
                                                        return 8;
                                                    else
                                                        return 9;
                                        else
                                            if (actual_vz <= 1.207187f)
                                                if (actual_vx <= -0.002001f)
                                                    if (sensor2 <= 0.693653f)
                                                        return 6;
                                                    else
                                                        return 8;
                                                else
                                                    if (sensor5 <= 0.815114f)
                                                        return 9;
                                                    else
                                                        return 10;
                                            else
                                                if (actual_vx <= -0.007484f)
                                                    if (sensor2 <= 0.709439f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= 0.225526f)
                                                        return 8;
                                                    else
                                                        return 10;
        else
            if (sensor0 <= 0.268450f)
                if (last_action_state <= 0.550000f)
                    if (sensor2 <= 0.461353f)
                        if (actual_vx <= 0.000670f)
                            if (last_action_state <= 0.250000f)
                                if (actual_omega <= 0.105051f)
                                    if (actual_vx <= -0.016173f)
                                        if (actual_vz <= 0.941659f)
                                            return 1;
                                        else
                                            return 0;
                                    else
                                        if (actual_omega <= 0.075413f)
                                            if (sensor5 <= 0.488201f)
                                                if (sensor0 <= 0.257443f)
                                                    if (actual_omega <= 0.056376f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vx <= -0.006092f)
                                                        return 1;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor0 <= 0.217219f)
                                                    if (sensor5 <= 0.570612f)
                                                        return 5;
                                                    else
                                                        return 0;
                                                else
                                                    if (sensor0 <= 0.258945f)
                                                        return 0;
                                                    else
                                                        return 1;
                                        else
                                            if (actual_vz <= 1.282974f)
                                                if (last_action_state <= 0.150000f)
                                                    if (sensor4 <= 0.858608f)
                                                        return 0;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor3 <= 0.350938f)
                                                        return 0;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor4 <= 0.909362f)
                                                    if (sensor4 <= 0.874587f)
                                                        return 5;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_omega <= 0.079251f)
                                                        return 1;
                                                    else
                                                        return 0;
                                else
                                    if (actual_vx <= -0.004140f)
                                        if (sensor4 <= 0.866870f)
                                            if (sensor0 <= 0.184552f)
                                                return 0;
                                            else
                                                if (sensor1 <= 0.818584f)
                                                    return 5;
                                                else
                                                    return 1;
                                        else
                                            if (sensor5 <= 0.406817f)
                                                if (actual_vz <= 1.280370f)
                                                    return 5;
                                                else
                                                    return 0;
                                            else
                                                if (actual_vz <= 1.068504f)
                                                    if (actual_vz <= 1.042220f)
                                                        return 0;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor5 <= 0.578692f)
                                                        return 0;
                                                    else
                                                        return 0;
                                    else
                                        if (actual_vx <= -0.003336f)
                                            if (actual_vz <= 1.297612f)
                                                if (last_action_state <= 0.050000f)
                                                    return 5;
                                                else
                                                    return 1;
                                            else
                                                return 0;
                                        else
                                            if (last_action_state <= 0.100000f)
                                                if (sensor2 <= 0.453590f)
                                                    return 0;
                                                else
                                                    return 1;
                                            else
                                                return 5;
                            else
                                if (sensor0 <= 0.223383f)
                                    if (actual_omega <= 0.119596f)
                                        if (actual_vz <= 1.015880f)
                                            if (sensor3 <= 0.588993f)
                                                if (sensor5 <= 0.318195f)
                                                    return 10;
                                                else
                                                    if (sensor3 <= 0.586903f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor2 <= 0.455452f)
                                                    return 6;
                                                else
                                                    return 5;
                                        else
                                            if (sensor1 <= 0.816387f)
                                                if (actual_omega <= 0.098306f)
                                                    if (sensor2 <= 0.382284f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vx <= 0.000024f)
                                                        return 5;
                                                    else
                                                        return 6;
                                            else
                                                if (actual_omega <= 0.080628f)
                                                    if (sensor5 <= 0.443827f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vz <= 1.134342f)
                                                        return 5;
                                                    else
                                                        return 5;
                                    else
                                        if (actual_omega <= 0.142802f)
                                            if (actual_vx <= -0.006323f)
                                                if (sensor3 <= 0.487535f)
                                                    if (sensor0 <= 0.184749f)
                                                        return 7;
                                                    else
                                                        return 0;
                                                else
                                                    if (actual_omega <= 0.139826f)
                                                        return 5;
                                                    else
                                                        return 0;
                                            else
                                                if (sensor4 <= 0.872288f)
                                                    if (sensor5 <= 0.562344f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor0 <= 0.221501f)
                                                        return 5;
                                                    else
                                                        return 0;
                                        else
                                            if (sensor3 <= 0.347242f)
                                                if (actual_omega <= 0.187263f)
                                                    if (sensor1 <= 0.818128f)
                                                        return 5;
                                                    else
                                                        return 0;
                                                else
                                                    return 0;
                                            else
                                                if (actual_omega <= 0.143273f)
                                                    return 0;
                                                else
                                                    if (sensor0 <= 0.222310f)
                                                        return 5;
                                                    else
                                                        return 0;
                                else
                                    if (actual_omega <= 0.135839f)
                                        if (sensor2 <= 0.356433f)
                                            if (sensor1 <= 0.803201f)
                                                if (sensor1 <= 0.802378f)
                                                    if (actual_omega <= 0.001598f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor5 <= 0.402038f)
                                                        return 10;
                                                    else
                                                        return 1;
                                            else
                                                if (sensor5 <= 0.502479f)
                                                    if (sensor5 <= 0.303448f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    return 4;
                                        else
                                            if (actual_vz <= 1.152216f)
                                                if (sensor1 <= 0.806565f)
                                                    if (sensor2 <= 0.375614f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor5 <= 0.309901f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_omega <= 0.000628f)
                                                    return 10;
                                                else
                                                    if (sensor2 <= 0.357765f)
                                                        return 6;
                                                    else
                                                        return 5;
                                    else
                                        if (sensor2 <= 0.369833f)
                                            if (sensor1 <= 0.839327f)
                                                if (sensor4 <= 0.904071f)
                                                    if (actual_vx <= -0.001584f)
                                                        return 5;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_vz <= 1.167059f)
                                                        return 1;
                                                    else
                                                        return 0;
                                            else
                                                return 0;
                                        else
                                            if (sensor3 <= 0.338977f)
                                                if (sensor4 <= 0.898265f)
                                                    return 2;
                                                else
                                                    return 0;
                                            else
                                                if (actual_vx <= -0.004957f)
                                                    if (actual_omega <= 0.149454f)
                                                        return 5;
                                                    else
                                                        return 0;
                                                else
                                                    return 5;
                        else
                            if (sensor2 <= 0.363172f)
                                if (actual_vx <= 0.004745f)
                                    if (sensor0 <= 0.263159f)
                                        if (actual_omega <= 0.059302f)
                                            if (sensor3 <= 0.279038f)
                                                if (actual_vz <= 1.216534f)
                                                    if (sensor1 <= 0.803768f)
                                                        return 10;
                                                    else
                                                        return 5;
                                                else
                                                    return 6;
                                            else
                                                if (actual_omega <= 0.024356f)
                                                    if (sensor5 <= 0.293444f)
                                                        return 10;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor1 <= 0.837849f)
                                                        return 5;
                                                    else
                                                        return 5;
                                        else
                                            if (actual_vx <= 0.000751f)
                                                if (actual_vz <= 1.159210f)
                                                    return 10;
                                                else
                                                    if (actual_vz <= 1.223759f)
                                                        return 7;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor3 <= 0.415322f)
                                                    if (actual_vx <= 0.004113f)
                                                        return 5;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor2 <= 0.352758f)
                                                        return 6;
                                                    else
                                                        return 7;
                                    else
                                        if (sensor1 <= 0.822403f)
                                            if (sensor5 <= 0.327172f)
                                                if (sensor0 <= 0.267676f)
                                                    if (sensor5 <= 0.324567f)
                                                        return 6;
                                                    else
                                                        return 4;
                                                else
                                                    return 5;
                                            else
                                                if (sensor4 <= 0.824272f)
                                                    return 6;
                                                else
                                                    return 5;
                                        else
                                            if (actual_omega <= 0.093885f)
                                                if (actual_omega <= 0.000851f)
                                                    if (sensor2 <= 0.331330f)
                                                        return 5;
                                                    else
                                                        return 10;
                                                else
                                                    if (actual_omega <= 0.060216f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor4 <= 0.869018f)
                                                    return 6;
                                                else
                                                    return 10;
                                else
                                    if (sensor5 <= 0.326014f)
                                        if (actual_vx <= 0.007473f)
                                            if (sensor3 <= 0.348926f)
                                                if (actual_omega <= 0.002225f)
                                                    if (actual_vz <= 1.057760f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vz <= 1.160127f)
                                                        return 10;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vz <= 1.057236f)
                                                    if (sensor1 <= 0.851692f)
                                                        return 5;
                                                    else
                                                        return 10;
                                                else
                                                    if (actual_vz <= 1.188049f)
                                                        return 10;
                                                    else
                                                        return 5;
                                        else
                                            if (actual_omega <= 0.008894f)
                                                if (sensor4 <= 0.869752f)
                                                    return 10;
                                                else
                                                    return 5;
                                            else
                                                if (sensor5 <= 0.302934f)
                                                    return 9;
                                                else
                                                    return 6;
                                    else
                                        if (sensor2 <= 0.358478f)
                                            if (actual_vz <= 1.274608f)
                                                if (sensor0 <= 0.268316f)
                                                    if (sensor3 <= 0.335098f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    return 4;
                                            else
                                                if (sensor4 <= 0.890612f)
                                                    return 10;
                                                else
                                                    return 9;
                                        else
                                            if (sensor3 <= 0.356605f)
                                                return 10;
                                            else
                                                if (actual_vz <= 1.027241f)
                                                    return 5;
                                                else
                                                    if (sensor1 <= 0.855966f)
                                                        return 6;
                                                    else
                                                        return 7;
                            else
                                if (actual_vx <= 0.003807f)
                                    if (sensor2 <= 0.412064f)
                                        if (sensor3 <= 0.370081f)
                                            if (sensor2 <= 0.363700f)
                                                if (actual_vx <= 0.003201f)
                                                    return 6;
                                                else
                                                    return 10;
                                            else
                                                if (sensor4 <= 0.867657f)
                                                    return 5;
                                                else
                                                    if (actual_omega <= 0.022578f)
                                                        return 6;
                                                    else
                                                        return 5;
                                        else
                                            if (actual_vz <= 1.046696f)
                                                if (actual_vx <= 0.000894f)
                                                    return 10;
                                                else
                                                    if (sensor3 <= 0.373261f)
                                                        return 8;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_omega <= 0.059513f)
                                                    if (sensor0 <= 0.241937f)
                                                        return 5;
                                                    else
                                                        return 8;
                                                else
                                                    if (sensor2 <= 0.369765f)
                                                        return 5;
                                                    else
                                                        return 5;
                                    else
                                        if (sensor3 <= 0.339937f)
                                            if (actual_omega <= 0.057130f)
                                                if (actual_vx <= 0.002080f)
                                                    if (actual_vz <= 1.025139f)
                                                        return 9;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor3 <= 0.329487f)
                                                        return 10;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor2 <= 0.451871f)
                                                    if (sensor2 <= 0.417016f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vx <= 0.000882f)
                                                        return 6;
                                                    else
                                                        return 5;
                                        else
                                            if (last_action_state <= 0.450000f)
                                                return 5;
                                            else
                                                if (actual_vz <= 1.182103f)
                                                    if (sensor3 <= 0.363882f)
                                                        return 6;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor4 <= 0.899017f)
                                                        return 8;
                                                    else
                                                        return 10;
                                else
                                    if (sensor2 <= 0.375622f)
                                        if (actual_vz <= 1.095657f)
                                            if (sensor1 <= 0.844416f)
                                                if (sensor5 <= 0.376026f)
                                                    return 5;
                                                else
                                                    if (sensor4 <= 0.946815f)
                                                        return 9;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_omega <= 0.027550f)
                                                    if (sensor3 <= 0.366922f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    return 6;
                                        else
                                            if (actual_vz <= 1.209152f)
                                                if (sensor1 <= 0.833641f)
                                                    return 5;
                                                else
                                                    if (sensor4 <= 0.889285f)
                                                        return 10;
                                                    else
                                                        return 10;
                                            else
                                                if (sensor0 <= 0.238571f)
                                                    if (actual_vx <= 0.004465f)
                                                        return 6;
                                                    else
                                                        return 8;
                                                else
                                                    if (sensor5 <= 0.302501f)
                                                        return 5;
                                                    else
                                                        return 5;
                                    else
                                        if (sensor3 <= 0.355174f)
                                            if (sensor1 <= 0.827371f)
                                                return 10;
                                            else
                                                if (sensor2 <= 0.387620f)
                                                    if (sensor0 <= 0.232124f)
                                                        return 6;
                                                    else
                                                        return 9;
                                                else
                                                    if (actual_omega <= 0.039664f)
                                                        return 5;
                                                    else
                                                        return 6;
                                        else
                                            if (actual_vz <= 1.181843f)
                                                if (actual_vx <= 0.004532f)
                                                    if (sensor0 <= 0.193045f)
                                                        return 9;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor2 <= 0.379794f)
                                                        return 6;
                                                    else
                                                        return 10;
                                            else
                                                if (sensor5 <= 0.317883f)
                                                    if (sensor2 <= 0.396109f)
                                                        return 10;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vz <= 1.210830f)
                                                        return 5;
                                                    else
                                                        return 9;
                    else
                        if (actual_omega <= 0.068508f)
                            if (sensor2 <= 0.534853f)
                                if (sensor3 <= 0.316113f)
                                    if (actual_omega <= 0.001920f)
                                        if (sensor1 <= 0.884872f)
                                            if (actual_vz <= 1.165677f)
                                                if (sensor5 <= 0.304739f)
                                                    if (sensor3 <= 0.313134f)
                                                        return 5;
                                                    else
                                                        return 9;
                                                else
                                                    if (sensor1 <= 0.883617f)
                                                        return 10;
                                                    else
                                                        return 8;
                                            else
                                                return 8;
                                        else
                                            if (sensor5 <= 0.316554f)
                                                if (sensor4 <= 0.845944f)
                                                    if (sensor4 <= 0.842070f)
                                                        return 5;
                                                    else
                                                        return 10;
                                                else
                                                    if (actual_vz <= 1.151516f)
                                                        return 5;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor2 <= 0.502342f)
                                                    if (sensor1 <= 0.892840f)
                                                        return 5;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor1 <= 0.889874f)
                                                        return 10;
                                                    else
                                                        return 5;
                                    else
                                        if (sensor2 <= 0.501507f)
                                            if (actual_vx <= -0.001658f)
                                                if (sensor1 <= 0.832291f)
                                                    if (sensor5 <= 0.464725f)
                                                        return 9;
                                                    else
                                                        return 0;
                                                else
                                                    if (sensor3 <= 0.307603f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_omega <= 0.052902f)
                                                    if (actual_vx <= -0.001079f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor3 <= 0.306186f)
                                                        return 5;
                                                    else
                                                        return 6;
                                        else
                                            if (sensor2 <= 0.531792f)
                                                if (actual_vz <= 1.080631f)
                                                    if (sensor3 <= 0.296384f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor5 <= 0.301182f)
                                                        return 6;
                                                    else
                                                        return 9;
                                            else
                                                if (sensor3 <= 0.287626f)
                                                    if (actual_vx <= -0.003233f)
                                                        return 5;
                                                    else
                                                        return 10;
                                                else
                                                    return 5;
                                else
                                    if (sensor5 <= 0.288318f)
                                        if (sensor2 <= 0.509166f)
                                            if (actual_vx <= -0.001615f)
                                                if (sensor4 <= 0.848829f)
                                                    if (sensor2 <= 0.503772f)
                                                        return 5;
                                                    else
                                                        return 8;
                                                else
                                                    if (actual_vz <= 1.179263f)
                                                        return 6;
                                                    else
                                                        return 10;
                                            else
                                                if (sensor2 <= 0.502615f)
                                                    return 10;
                                                else
                                                    return 5;
                                        else
                                            return 10;
                                    else
                                        if (actual_vx <= -0.006808f)
                                            if (sensor1 <= 0.842505f)
                                                return 5;
                                            else
                                                if (sensor5 <= 0.376834f)
                                                    return 5;
                                                else
                                                    return 6;
                                        else
                                            if (sensor4 <= 0.987325f)
                                                if (last_action_state <= 0.350000f)
                                                    return 5;
                                                else
                                                    if (sensor5 <= 0.290864f)
                                                        return 5;
                                                    else
                                                        return 10;
                                            else
                                                return 8;
                            else
                                if (actual_vx <= -0.001336f)
                                    if (sensor5 <= 0.391624f)
                                        if (sensor1 <= 0.875946f)
                                            if (actual_vx <= -0.001623f)
                                                if (sensor1 <= 0.873782f)
                                                    if (actual_vz <= 1.055773f)
                                                        return 5;
                                                    else
                                                        return 10;
                                                else
                                                    if (actual_omega <= 0.019178f)
                                                        return 8;
                                                    else
                                                        return 9;
                                            else
                                                return 8;
                                        else
                                            if (sensor0 <= 0.242271f)
                                                if (sensor2 <= 0.577949f)
                                                    return 6;
                                                else
                                                    if (sensor3 <= 0.284331f)
                                                        return 6;
                                                    else
                                                        return 10;
                                            else
                                                if (actual_omega <= 0.036764f)
                                                    if (sensor4 <= 0.849956f)
                                                        return 9;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor1 <= 0.897558f)
                                                        return 5;
                                                    else
                                                        return 5;
                                    else
                                        if (sensor0 <= 0.224952f)
                                            if (actual_vz <= 1.094314f)
                                                return 9;
                                            else
                                                return 8;
                                        else
                                            return 9;
                                else
                                    if (sensor4 <= 0.861111f)
                                        if (sensor2 <= 0.535605f)
                                            return 8;
                                        else
                                            if (sensor1 <= 0.888561f)
                                                if (actual_vz <= 1.174414f)
                                                    if (sensor1 <= 0.873475f)
                                                        return 10;
                                                    else
                                                        return 10;
                                                else
                                                    return 6;
                                            else
                                                if (actual_vz <= 1.104541f)
                                                    return 5;
                                                else
                                                    return 10;
                                    else
                                        if (actual_omega <= 0.001755f)
                                            return 6;
                                        else
                                            if (actual_omega <= 0.003756f)
                                                return 5;
                                            else
                                                return 9;
                        else
                            if (last_action_state <= 0.150000f)
                                if (actual_omega <= 0.120936f)
                                    if (actual_vx <= -0.004699f)
                                        if (actual_vz <= 1.157144f)
                                            return 1;
                                        else
                                            return 0;
                                    else
                                        if (sensor1 <= 0.808307f)
                                            if (actual_vz <= 1.186063f)
                                                if (actual_vx <= -0.003336f)
                                                    return 5;
                                                else
                                                    return 0;
                                            else
                                                return 5;
                                        else
                                            return 5;
                                else
                                    return 0;
                            else
                                if (actual_omega <= 0.124869f)
                                    if (actual_vz <= 1.161235f)
                                        if (actual_vx <= -0.010314f)
                                            if (sensor5 <= 0.372747f)
                                                if (sensor3 <= 0.269818f)
                                                    return 6;
                                                else
                                                    return 10;
                                            else
                                                if (actual_vx <= -0.011659f)
                                                    if (sensor5 <= 0.433325f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor3 <= 0.244342f)
                                                        return 10;
                                                    else
                                                        return 9;
                                        else
                                            if (actual_omega <= 0.105532f)
                                                if (actual_omega <= 0.100114f)
                                                    if (sensor3 <= 0.351693f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_omega <= 0.103211f)
                                                        return 6;
                                                    else
                                                        return 9;
                                            else
                                                if (actual_omega <= 0.122557f)
                                                    if (sensor2 <= 0.649355f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vz <= 1.156256f)
                                                        return 6;
                                                    else
                                                        return 7;
                                    else
                                        if (sensor0 <= 0.251011f)
                                            if (sensor1 <= 0.803283f)
                                                if (sensor5 <= 0.664887f)
                                                    if (sensor1 <= 0.799362f)
                                                        return 9;
                                                    else
                                                        return 5;
                                                else
                                                    return 7;
                                            else
                                                if (actual_omega <= 0.070594f)
                                                    if (actual_vz <= 1.180726f)
                                                        return 5;
                                                    else
                                                        return 8;
                                                else
                                                    if (actual_vz <= 1.243255f)
                                                        return 6;
                                                    else
                                                        return 9;
                                        else
                                            if (sensor2 <= 0.582142f)
                                                if (sensor5 <= 0.328714f)
                                                    if (sensor1 <= 0.852790f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vz <= 1.234157f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor1 <= 0.837651f)
                                                    return 7;
                                                else
                                                    return 8;
                                else
                                    if (sensor2 <= 0.469685f)
                                        if (sensor5 <= 0.573514f)
                                            if (sensor0 <= 0.175891f)
                                                return 0;
                                            else
                                                return 5;
                                        else
                                            if (actual_omega <= 0.166814f)
                                                if (actual_omega <= 0.141119f)
                                                    if (sensor0 <= 0.173319f)
                                                        return 6;
                                                    else
                                                        return 0;
                                                else
                                                    return 0;
                                            else
                                                return 2;
                                    else
                                        if (actual_vx <= -0.001528f)
                                            if (actual_omega <= 0.161927f)
                                                if (sensor2 <= 0.628422f)
                                                    if (actual_vz <= 0.984385f)
                                                        return 2;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor3 <= 0.237077f)
                                                        return 5;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor2 <= 0.505159f)
                                                    if (actual_vz <= 1.071635f)
                                                        return 5;
                                                    else
                                                        return 0;
                                                else
                                                    if (sensor5 <= 0.402035f)
                                                        return 6;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor1 <= 0.805683f)
                                                return 7;
                                            else
                                                if (actual_omega <= 0.134050f)
                                                    return 6;
                                                else
                                                    if (actual_omega <= 0.142020f)
                                                        return 5;
                                                    else
                                                        return 0;
                else
                    if (sensor2 <= 0.534641f)
                        if (sensor2 <= 0.380178f)
                            if (actual_vz <= 1.116392f)
                                if (sensor3 <= 0.345154f)
                                    if (actual_vx <= 0.007084f)
                                        if (actual_vz <= 0.985737f)
                                            return 10;
                                        else
                                            if (actual_omega <= 0.056853f)
                                                if (sensor4 <= 0.795042f)
                                                    return 0;
                                                else
                                                    return 5;
                                            else
                                                if (actual_omega <= 0.058199f)
                                                    if (sensor0 <= 0.251101f)
                                                        return 9;
                                                    else
                                                        return 4;
                                                else
                                                    if (sensor0 <= 0.265672f)
                                                        return 5;
                                                    else
                                                        return 6;
                                    else
                                        return 7;
                                else
                                    if (actual_vx <= 0.000957f)
                                        if (last_action_state <= 0.750000f)
                                            if (sensor3 <= 0.568837f)
                                                if (sensor2 <= 0.375236f)
                                                    if (sensor1 <= 0.798993f)
                                                        return 10;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor0 <= 0.234481f)
                                                        return 10;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_omega <= 0.036138f)
                                                    return 9;
                                                else
                                                    return 8;
                                        else
                                            if (sensor1 <= 0.813006f)
                                                if (actual_omega <= 0.089536f)
                                                    return 10;
                                                else
                                                    if (actual_vx <= -0.003150f)
                                                        return 5;
                                                    else
                                                        return 8;
                                            else
                                                if (actual_vz <= 0.969626f)
                                                    if (sensor2 <= 0.378027f)
                                                        return 5;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor3 <= 0.401126f)
                                                        return 5;
                                                    else
                                                        return 6;
                                    else
                                        if (actual_omega <= 0.038601f)
                                            if (sensor3 <= 0.373122f)
                                                return 9;
                                            else
                                                return 10;
                                        else
                                            if (actual_vx <= 0.001257f)
                                                if (sensor0 <= 0.251372f)
                                                    if (sensor2 <= 0.374187f)
                                                        return 10;
                                                    else
                                                        return 6;
                                                else
                                                    return 5;
                                            else
                                                if (actual_vx <= 0.001415f)
                                                    return 5;
                                                else
                                                    if (sensor3 <= 0.346084f)
                                                        return 10;
                                                    else
                                                        return 5;
                            else
                                if (actual_vx <= -0.000689f)
                                    if (actual_vx <= -0.002424f)
                                        if (sensor0 <= 0.265929f)
                                            if (sensor3 <= 0.404928f)
                                                if (actual_vx <= -0.004345f)
                                                    if (actual_vx <= -0.005982f)
                                                        return 5;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor4 <= 0.860151f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor0 <= 0.237583f)
                                                    if (sensor0 <= 0.231921f)
                                                        return 8;
                                                    else
                                                        return 6;
                                                else
                                                    return 5;
                                        else
                                            return 6;
                                    else
                                        if (sensor1 <= 0.830656f)
                                            if (sensor4 <= 0.919515f)
                                                if (sensor4 <= 0.908738f)
                                                    if (sensor1 <= 0.827458f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vx <= -0.002094f)
                                                        return 5;
                                                    else
                                                        return 6;
                                            else
                                                if (actual_vz <= 1.279698f)
                                                    if (sensor3 <= 0.338315f)
                                                        return 10;
                                                    else
                                                        return 5;
                                                else
                                                    return 6;
                                        else
                                            if (sensor5 <= 0.274271f)
                                                if (last_action_state <= 0.900000f)
                                                    return 8;
                                                else
                                                    return 10;
                                            else
                                                if (sensor0 <= 0.236651f)
                                                    if (sensor1 <= 0.832081f)
                                                        return 8;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vx <= -0.000699f)
                                                        return 5;
                                                    else
                                                        return 6;
                                else
                                    if (last_action_state <= 0.850000f)
                                        if (actual_omega <= 0.080511f)
                                            if (actual_omega <= 0.075227f)
                                                if (actual_vx <= 0.004448f)
                                                    if (sensor3 <= 0.374059f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor0 <= 0.257800f)
                                                        return 10;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor3 <= 0.372780f)
                                                    return 6;
                                                else
                                                    if (actual_vx <= 0.002286f)
                                                        return 6;
                                                    else
                                                        return 9;
                                        else
                                            if (actual_vx <= 0.003179f)
                                                if (sensor5 <= 0.432939f)
                                                    if (actual_omega <= 0.100085f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    return 9;
                                            else
                                                if (sensor4 <= 0.900200f)
                                                    if (sensor0 <= 0.253053f)
                                                        return 10;
                                                    else
                                                        return 5;
                                                else
                                                    return 5;
                                    else
                                        if (sensor3 <= 0.353906f)
                                            if (actual_vx <= 0.003380f)
                                                if (sensor0 <= 0.267895f)
                                                    if (actual_vx <= 0.002374f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor3 <= 0.341522f)
                                                        return 10;
                                                    else
                                                        return 9;
                                            else
                                                if (actual_vx <= 0.004944f)
                                                    if (actual_vx <= 0.003529f)
                                                        return 6;
                                                    else
                                                        return 8;
                                                else
                                                    if (sensor3 <= 0.333256f)
                                                        return 9;
                                                    else
                                                        return 10;
                                        else
                                            if (actual_vz <= 1.296849f)
                                                if (actual_omega <= 0.131963f)
                                                    if (actual_vz <= 1.156682f)
                                                        return 10;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vz <= 1.229140f)
                                                        return 5;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor0 <= 0.218240f)
                                                    return 7;
                                                else
                                                    if (sensor5 <= 0.387779f)
                                                        return 8;
                                                    else
                                                        return 9;
                        else
                            if (actual_vx <= -0.000069f)
                                if (last_action_state <= 0.950000f)
                                    if (sensor0 <= 0.183509f)
                                        if (sensor5 <= 0.558001f)
                                            if (actual_omega <= 0.145226f)
                                                if (sensor5 <= 0.547548f)
                                                    if (actual_vz <= 1.181357f)
                                                        return 5;
                                                    else
                                                        return 8;
                                                else
                                                    if (actual_vz <= 1.163275f)
                                                        return 8;
                                                    else
                                                        return 7;
                                            else
                                                if (actual_vz <= 1.161568f)
                                                    if (sensor0 <= 0.174589f)
                                                        return 10;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor1 <= 0.805492f)
                                                        return 6;
                                                    else
                                                        return 8;
                                        else
                                            if (sensor5 <= 0.564124f)
                                                if (sensor3 <= 0.333851f)
                                                    if (sensor1 <= 0.799539f)
                                                        return 7;
                                                    else
                                                        return 6;
                                                else
                                                    return 5;
                                            else
                                                if (actual_omega <= 0.207810f)
                                                    if (actual_omega <= 0.153874f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor1 <= 0.814502f)
                                                        return 5;
                                                    else
                                                        return 6;
                                    else
                                        if (actual_omega <= 0.102486f)
                                            if (actual_vz <= 1.025367f)
                                                if (sensor5 <= 0.285444f)
                                                    if (actual_vz <= 0.978427f)
                                                        return 10;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor2 <= 0.528907f)
                                                        return 5;
                                                    else
                                                        return 10;
                                            else
                                                if (sensor1 <= 0.833654f)
                                                    if (sensor0 <= 0.208177f)
                                                        return 10;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor2 <= 0.440245f)
                                                        return 5;
                                                    else
                                                        return 5;
                                        else
                                            if (actual_vz <= 1.100461f)
                                                if (sensor0 <= 0.211552f)
                                                    if (sensor0 <= 0.211072f)
                                                        return 5;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor3 <= 0.403809f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_omega <= 0.146489f)
                                                    if (last_action_state <= 0.650000f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor1 <= 0.813772f)
                                                        return 5;
                                                    else
                                                        return 5;
                                else
                                    if (actual_omega <= 0.076124f)
                                        if (actual_vz <= 1.184372f)
                                            if (sensor3 <= 0.272758f)
                                                return 6;
                                            else
                                                if (sensor5 <= 0.310319f)
                                                    if (sensor2 <= 0.512720f)
                                                        return 6;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor0 <= 0.264233f)
                                                        return 10;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor4 <= 0.854324f)
                                                if (sensor4 <= 0.852486f)
                                                    if (sensor0 <= 0.264354f)
                                                        return 10;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vx <= -0.001126f)
                                                        return 6;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vx <= -0.002758f)
                                                    if (actual_vz <= 1.236621f)
                                                        return 10;
                                                    else
                                                        return 8;
                                                else
                                                    if (sensor1 <= 0.859997f)
                                                        return 9;
                                                    else
                                                        return 8;
                                    else
                                        if (actual_vx <= -0.006696f)
                                            if (sensor4 <= 0.855291f)
                                                if (sensor2 <= 0.484014f)
                                                    return 6;
                                                else
                                                    if (actual_vx <= -0.007476f)
                                                        return 9;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vx <= -0.006920f)
                                                    if (actual_omega <= 0.091282f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor3 <= 0.296363f)
                                                        return 7;
                                                    else
                                                        return 8;
                                        else
                                            if (actual_omega <= 0.139573f)
                                                if (sensor0 <= 0.174426f)
                                                    return 8;
                                                else
                                                    if (sensor5 <= 0.559232f)
                                                        return 6;
                                                    else
                                                        return 9;
                                            else
                                                if (sensor1 <= 0.815878f)
                                                    if (sensor4 <= 0.869948f)
                                                        return 6;
                                                    else
                                                        return 8;
                                                else
                                                    if (sensor1 <= 0.823819f)
                                                        return 6;
                                                    else
                                                        return 5;
                            else
                                if (last_action_state <= 0.750000f)
                                    if (actual_vx <= 0.002493f)
                                        if (sensor1 <= 0.826461f)
                                            if (sensor5 <= 0.630446f)
                                                if (actual_omega <= 0.055842f)
                                                    if (actual_omega <= 0.050716f)
                                                        return 6;
                                                    else
                                                        return 10;
                                                else
                                                    if (actual_vx <= 0.000013f)
                                                        return 5;
                                                    else
                                                        return 8;
                                            else
                                                if (sensor1 <= 0.801048f)
                                                    return 9;
                                                else
                                                    if (actual_vz <= 1.128821f)
                                                        return 5;
                                                    else
                                                        return 7;
                                        else
                                            if (actual_omega <= 0.061806f)
                                                if (sensor0 <= 0.234549f)
                                                    if (actual_vz <= 1.069017f)
                                                        return 5;
                                                    else
                                                        return 10;
                                                else
                                                    return 6;
                                            else
                                                if (sensor0 <= 0.234837f)
                                                    if (actual_vx <= 0.002060f)
                                                        return 5;
                                                    else
                                                        return 9;
                                                else
                                                    return 5;
                                    else
                                        if (sensor4 <= 0.830059f)
                                            if (sensor5 <= 0.638742f)
                                                if (actual_vz <= 1.008111f)
                                                    return 10;
                                                else
                                                    return 6;
                                            else
                                                return 8;
                                        else
                                            if (sensor5 <= 0.318691f)
                                                if (sensor3 <= 0.417835f)
                                                    if (sensor5 <= 0.292220f)
                                                        return 5;
                                                    else
                                                        return 9;
                                                else
                                                    if (sensor0 <= 0.222484f)
                                                        return 10;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor1 <= 0.844116f)
                                                    if (sensor0 <= 0.168340f)
                                                        return 9;
                                                    else
                                                        return 10;
                                                else
                                                    if (actual_omega <= 0.063903f)
                                                        return 5;
                                                    else
                                                        return 10;
                                else
                                    if (actual_omega <= 0.099712f)
                                        if (sensor2 <= 0.382499f)
                                            if (sensor5 <= 0.305624f)
                                                if (actual_omega <= 0.071706f)
                                                    return 10;
                                                else
                                                    return 6;
                                            else
                                                return 10;
                                        else
                                            if (sensor1 <= 0.843726f)
                                                if (actual_vx <= 0.003822f)
                                                    if (actual_vz <= 1.144761f)
                                                        return 10;
                                                    else
                                                        return 8;
                                                else
                                                    if (sensor2 <= 0.385030f)
                                                        return 6;
                                                    else
                                                        return 9;
                                            else
                                                if (sensor5 <= 0.310138f)
                                                    if (sensor4 <= 0.843541f)
                                                        return 5;
                                                    else
                                                        return 8;
                                                else
                                                    if (actual_vz <= 1.251016f)
                                                        return 10;
                                                    else
                                                        return 6;
                                    else
                                        if (sensor1 <= 0.832036f)
                                            if (actual_vz <= 1.019095f)
                                                return 10;
                                            else
                                                if (actual_omega <= 0.165833f)
                                                    if (actual_vx <= -0.000017f)
                                                        return 10;
                                                    else
                                                        return 8;
                                                else
                                                    if (sensor4 <= 0.902207f)
                                                        return 6;
                                                    else
                                                        return 8;
                                        else
                                            if (sensor3 <= 0.370554f)
                                                if (sensor2 <= 0.389960f)
                                                    if (sensor1 <= 0.844189f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vz <= 1.231095f)
                                                        return 5;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor4 <= 0.875165f)
                                                    if (sensor3 <= 0.388920f)
                                                        return 10;
                                                    else
                                                        return 8;
                                                else
                                                    if (sensor4 <= 0.881693f)
                                                        return 6;
                                                    else
                                                        return 10;
                    else
                        if (actual_vz <= 1.108528f)
                            if (actual_omega <= 0.119416f)
                                if (sensor0 <= 0.245683f)
                                    if (sensor3 <= 0.280261f)
                                        if (sensor1 <= 0.844589f)
                                            if (actual_omega <= 0.110968f)
                                                if (sensor3 <= 0.224900f)
                                                    if (sensor2 <= 0.662744f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor3 <= 0.274417f)
                                                        return 10;
                                                    else
                                                        return 6;
                                            else
                                                if (actual_omega <= 0.113095f)
                                                    if (actual_vx <= -0.008055f)
                                                        return 8;
                                                    else
                                                        return 10;
                                                else
                                                    if (actual_omega <= 0.113396f)
                                                        return 6;
                                                    else
                                                        return 9;
                                        else
                                            if (sensor2 <= 0.546596f)
                                                if (sensor1 <= 0.864854f)
                                                    return 6;
                                                else
                                                    return 9;
                                            else
                                                if (sensor3 <= 0.279326f)
                                                    if (sensor5 <= 0.400202f)
                                                        return 10;
                                                    else
                                                        return 5;
                                                else
                                                    return 8;
                                    else
                                        if (sensor2 <= 0.603510f)
                                            if (actual_omega <= 0.046147f)
                                                if (sensor5 <= 0.303032f)
                                                    if (sensor3 <= 0.300416f)
                                                        return 10;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vx <= -0.001065f)
                                                        return 6;
                                                    else
                                                        return 10;
                                            else
                                                if (sensor5 <= 0.318346f)
                                                    if (actual_vx <= -0.003024f)
                                                        return 5;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor5 <= 0.328964f)
                                                        return 10;
                                                    else
                                                        return 8;
                                        else
                                            if (sensor2 <= 0.647734f)
                                                if (sensor5 <= 0.324657f)
                                                    if (sensor2 <= 0.643543f)
                                                        return 10;
                                                    else
                                                        return 9;
                                                else
                                                    if (sensor5 <= 0.326558f)
                                                        return 8;
                                                    else
                                                        return 10;
                                            else
                                                if (sensor1 <= 0.820958f)
                                                    return 10;
                                                else
                                                    return 6;
                                else
                                    if (sensor5 <= 0.320092f)
                                        if (sensor4 <= 0.846373f)
                                            if (actual_omega <= 0.039235f)
                                                if (sensor5 <= 0.295701f)
                                                    if (actual_vz <= 0.997050f)
                                                        return 5;
                                                    else
                                                        return 8;
                                                else
                                                    if (actual_vx <= -0.001322f)
                                                        return 9;
                                                    else
                                                        return 10;
                                            else
                                                if (sensor0 <= 0.255997f)
                                                    if (sensor0 <= 0.254510f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    return 6;
                                        else
                                            if (actual_omega <= 0.053732f)
                                                if (actual_vx <= -0.003691f)
                                                    if (sensor1 <= 0.866870f)
                                                        return 10;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vz <= 1.069925f)
                                                        return 10;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vx <= -0.002555f)
                                                    if (sensor0 <= 0.258256f)
                                                        return 10;
                                                    else
                                                        return 8;
                                                else
                                                    if (sensor3 <= 0.296650f)
                                                        return 5;
                                                    else
                                                        return 8;
                                    else
                                        if (sensor4 <= 0.840852f)
                                            return 10;
                                        else
                                            if (actual_vz <= 1.088013f)
                                                if (last_action_state <= 0.650000f)
                                                    if (actual_omega <= 0.042586f)
                                                        return 8;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor4 <= 0.846353f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_omega <= 0.102040f)
                                                    if (actual_omega <= 0.093588f)
                                                        return 8;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vx <= -0.005026f)
                                                        return 5;
                                                    else
                                                        return 8;
                            else
                                if (sensor0 <= 0.234123f)
                                    if (actual_vx <= -0.014081f)
                                        if (sensor0 <= 0.215637f)
                                            if (sensor4 <= 0.851635f)
                                                if (sensor2 <= 0.693327f)
                                                    if (sensor1 <= 0.803541f)
                                                        return 10;
                                                    else
                                                        return 6;
                                                else
                                                    return 6;
                                            else
                                                if (sensor2 <= 0.660877f)
                                                    return 5;
                                                else
                                                    return 8;
                                        else
                                            if (actual_vx <= -0.015024f)
                                                if (actual_vx <= -0.015871f)
                                                    return 5;
                                                else
                                                    if (sensor1 <= 0.809857f)
                                                        return 7;
                                                    else
                                                        return 8;
                                            else
                                                return 5;
                                    else
                                        if (actual_omega <= 0.152808f)
                                            if (sensor2 <= 0.663445f)
                                                if (sensor1 <= 0.831549f)
                                                    if (sensor4 <= 0.845134f)
                                                        return 10;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor2 <= 0.637411f)
                                                        return 8;
                                                    else
                                                        return 9;
                                            else
                                                if (sensor0 <= 0.207382f)
                                                    if (sensor3 <= 0.221118f)
                                                        return 10;
                                                    else
                                                        return 8;
                                                else
                                                    if (sensor5 <= 0.406259f)
                                                        return 8;
                                                    else
                                                        return 6;
                                        else
                                            if (actual_vz <= 1.097116f)
                                                if (sensor5 <= 0.436862f)
                                                    if (sensor0 <= 0.219719f)
                                                        return 9;
                                                    else
                                                        return 6;
                                                else
                                                    return 5;
                                            else
                                                if (sensor0 <= 0.229971f)
                                                    if (actual_omega <= 0.173708f)
                                                        return 5;
                                                    else
                                                        return 10;
                                                else
                                                    return 8;
                                else
                                    if (actual_vx <= -0.007628f)
                                        if (sensor4 <= 0.860339f)
                                            if (actual_vz <= 1.093174f)
                                                if (sensor3 <= 0.249232f)
                                                    if (sensor3 <= 0.242898f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor3 <= 0.270599f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor0 <= 0.238673f)
                                                    return 10;
                                                else
                                                    if (actual_omega <= 0.136197f)
                                                        return 6;
                                                    else
                                                        return 8;
                                        else
                                            if (actual_vz <= 1.084956f)
                                                if (sensor1 <= 0.837656f)
                                                    if (sensor3 <= 0.264393f)
                                                        return 6;
                                                    else
                                                        return 9;
                                                else
                                                    return 6;
                                            else
                                                if (sensor2 <= 0.581269f)
                                                    if (sensor4 <= 0.863865f)
                                                        return 8;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor5 <= 0.349021f)
                                                        return 9;
                                                    else
                                                        return 6;
                                    else
                                        if (sensor3 <= 0.297354f)
                                            if (sensor5 <= 0.370340f)
                                                if (actual_omega <= 0.132993f)
                                                    return 6;
                                                else
                                                    return 8;
                                            else
                                                return 10;
                                        else
                                            if (sensor3 <= 0.299989f)
                                                return 8;
                                            else
                                                return 10;
                        else
                            if (actual_omega <= 0.132272f)
                                if (sensor1 <= 0.852427f)
                                    if (last_action_state <= 0.750000f)
                                        if (sensor1 <= 0.846032f)
                                            if (actual_vx <= -0.006940f)
                                                if (sensor1 <= 0.822951f)
                                                    if (sensor4 <= 0.859697f)
                                                        return 6;
                                                    else
                                                        return 8;
                                                else
                                                    if (sensor4 <= 0.850814f)
                                                        return 10;
                                                    else
                                                        return 8;
                                            else
                                                if (actual_vz <= 1.189458f)
                                                    if (sensor2 <= 0.569653f)
                                                        return 10;
                                                    else
                                                        return 8;
                                                else
                                                    if (actual_vz <= 1.211006f)
                                                        return 10;
                                                    else
                                                        return 8;
                                        else
                                            if (actual_vx <= -0.010285f)
                                                if (actual_omega <= 0.113219f)
                                                    return 10;
                                                else
                                                    return 5;
                                            else
                                                if (sensor1 <= 0.846424f)
                                                    return 10;
                                                else
                                                    if (sensor1 <= 0.846557f)
                                                        return 5;
                                                    else
                                                        return 6;
                                    else
                                        if (sensor2 <= 0.662538f)
                                            if (sensor0 <= 0.220840f)
                                                if (sensor5 <= 0.344895f)
                                                    if (actual_vz <= 1.176168f)
                                                        return 10;
                                                    else
                                                        return 8;
                                                else
                                                    if (actual_vx <= -0.010249f)
                                                        return 8;
                                                    else
                                                        return 9;
                                            else
                                                if (sensor0 <= 0.257583f)
                                                    if (sensor1 <= 0.849766f)
                                                        return 8;
                                                    else
                                                        return 8;
                                                else
                                                    if (sensor1 <= 0.825649f)
                                                        return 8;
                                                    else
                                                        return 6;
                                        else
                                            if (sensor4 <= 0.866614f)
                                                if (sensor2 <= 0.706239f)
                                                    return 8;
                                                else
                                                    if (sensor0 <= 0.191599f)
                                                        return 8;
                                                    else
                                                        return 6;
                                            else
                                                return 10;
                                else
                                    if (actual_omega <= 0.079840f)
                                        if (sensor3 <= 0.272442f)
                                            if (last_action_state <= 0.750000f)
                                                if (sensor2 <= 0.616743f)
                                                    if (actual_vz <= 1.206118f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor4 <= 0.858329f)
                                                        return 10;
                                                    else
                                                        return 8;
                                            else
                                                if (actual_omega <= 0.079522f)
                                                    if (sensor5 <= 0.351262f)
                                                        return 8;
                                                    else
                                                        return 8;
                                                else
                                                    return 9;
                                        else
                                            if (sensor0 <= 0.256483f)
                                                if (actual_vz <= 1.116839f)
                                                    return 8;
                                                else
                                                    if (actual_vz <= 1.121539f)
                                                        return 6;
                                                    else
                                                        return 10;
                                            else
                                                if (last_action_state <= 0.950000f)
                                                    if (sensor3 <= 0.282434f)
                                                        return 9;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor0 <= 0.259095f)
                                                        return 9;
                                                    else
                                                        return 10;
                                    else
                                        if (actual_vz <= 1.115650f)
                                            if (last_action_state <= 0.850000f)
                                                return 7;
                                            else
                                                return 8;
                                        else
                                            if (sensor5 <= 0.304665f)
                                                if (sensor1 <= 0.861293f)
                                                    return 5;
                                                else
                                                    return 8;
                                            else
                                                if (actual_omega <= 0.126213f)
                                                    if (actual_vx <= -0.006302f)
                                                        return 6;
                                                    else
                                                        return 8;
                                                else
                                                    if (sensor4 <= 0.876131f)
                                                        return 8;
                                                    else
                                                        return 7;
                            else
                                if (last_action_state <= 0.850000f)
                                    if (sensor2 <= 0.675159f)
                                        if (actual_vx <= -0.013464f)
                                            if (sensor0 <= 0.236848f)
                                                if (actual_vx <= -0.016898f)
                                                    if (sensor0 <= 0.215467f)
                                                        return 7;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= 0.213261f)
                                                        return 6;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vz <= 1.178741f)
                                                    if (actual_omega <= 0.157904f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    return 8;
                                        else
                                            if (sensor1 <= 0.823600f)
                                                if (sensor5 <= 0.363688f)
                                                    if (actual_vz <= 1.235187f)
                                                        return 8;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor3 <= 0.221406f)
                                                        return 5;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor2 <= 0.665501f)
                                                    if (actual_vz <= 1.242764f)
                                                        return 6;
                                                    else
                                                        return 10;
                                                else
                                                    return 7;
                                    else
                                        if (sensor3 <= 0.216538f)
                                            return 8;
                                        else
                                            if (sensor3 <= 0.235513f)
                                                if (sensor1 <= 0.808609f)
                                                    return 6;
                                                else
                                                    if (actual_vz <= 1.177467f)
                                                        return 8;
                                                    else
                                                        return 7;
                                            else
                                                return 8;
                                else
                                    if (sensor0 <= 0.228910f)
                                        if (actual_omega <= 0.197127f)
                                            if (sensor3 <= 0.286150f)
                                                if (sensor1 <= 0.851822f)
                                                    if (sensor5 <= 0.357905f)
                                                        return 8;
                                                    else
                                                        return 8;
                                                else
                                                    if (sensor2 <= 0.635132f)
                                                        return 10;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor0 <= 0.228030f)
                                                    return 9;
                                                else
                                                    return 8;
                                        else
                                            if (last_action_state <= 0.950000f)
                                                if (sensor0 <= 0.212327f)
                                                    return 10;
                                                else
                                                    if (sensor4 <= 0.856375f)
                                                        return 6;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vz <= 1.166603f)
                                                    return 8;
                                                else
                                                    if (actual_vz <= 1.254258f)
                                                        return 6;
                                                    else
                                                        return 5;
                                    else
                                        if (sensor0 <= 0.244093f)
                                            if (sensor5 <= 0.337588f)
                                                if (actual_omega <= 0.144465f)
                                                    if (actual_omega <= 0.133686f)
                                                        return 10;
                                                    else
                                                        return 8;
                                                else
                                                    if (actual_vz <= 1.213934f)
                                                        return 8;
                                                    else
                                                        return 9;
                                            else
                                                if (sensor0 <= 0.234031f)
                                                    if (actual_omega <= 0.173532f)
                                                        return 8;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor2 <= 0.579462f)
                                                        return 6;
                                                    else
                                                        return 6;
                                        else
                                            if (actual_omega <= 0.180448f)
                                                if (sensor0 <= 0.246205f)
                                                    if (sensor2 <= 0.602747f)
                                                        return 8;
                                                    else
                                                        return 9;
                                                else
                                                    if (sensor0 <= 0.246654f)
                                                        return 6;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor1 <= 0.825210f)
                                                    return 6;
                                                else
                                                    if (sensor4 <= 0.845308f)
                                                        return 5;
                                                    else
                                                        return 5;
            else
                if (sensor0 <= 0.377135f)
                    if (actual_vz <= 1.047468f)
                        if (actual_vx <= 0.012720f)
                            if (sensor2 <= 0.425586f)
                                if (actual_vx <= 0.004179f)
                                    if (sensor1 <= 0.846078f)
                                        if (sensor5 <= 0.365185f)
                                            if (sensor0 <= 0.285664f)
                                                if (sensor0 <= 0.285598f)
                                                    if (sensor4 <= 0.782878f)
                                                        return 10;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor0 <= 0.285622f)
                                                        return 0;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor2 <= 0.221225f)
                                                    return 0;
                                                else
                                                    if (sensor1 <= 0.809952f)
                                                        return 5;
                                                    else
                                                        return 5;
                                        else
                                            if (actual_omega <= 0.037245f)
                                                if (sensor5 <= 0.367813f)
                                                    if (actual_vz <= 0.998875f)
                                                        return 1;
                                                    else
                                                        return 0;
                                                else
                                                    if (sensor1 <= 0.844060f)
                                                        return 5;
                                                    else
                                                        return 1;
                                            else
                                                if (actual_vx <= -0.000535f)
                                                    if (sensor1 <= 0.807922f)
                                                        return 0;
                                                    else
                                                        return 5;
                                                else
                                                    return 1;
                                    else
                                        if (sensor2 <= 0.401531f)
                                            if (sensor1 <= 0.881094f)
                                                if (actual_vz <= 0.988528f)
                                                    if (sensor2 <= 0.242165f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor5 <= 0.494343f)
                                                        return 5;
                                                    else
                                                        return 0;
                                            else
                                                if (sensor3 <= 0.228940f)
                                                    if (sensor4 <= 0.809491f)
                                                        return 10;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor2 <= 0.363109f)
                                                        return 5;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor2 <= 0.401601f)
                                                return 6;
                                            else
                                                if (actual_omega <= 0.000821f)
                                                    if (sensor2 <= 0.420362f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= 0.000831f)
                                                        return 7;
                                                    else
                                                        return 5;
                                else
                                    if (sensor2 <= 0.320337f)
                                        if (actual_vx <= 0.007380f)
                                            if (sensor1 <= 0.799567f)
                                                return 0;
                                            else
                                                if (sensor5 <= 0.580045f)
                                                    if (actual_vx <= 0.004186f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    return 1;
                                        else
                                            if (actual_omega <= 0.027142f)
                                                if (actual_vx <= 0.007393f)
                                                    if (sensor5 <= 0.561390f)
                                                        return 0;
                                                    else
                                                        return 2;
                                                else
                                                    if (sensor1 <= 0.878509f)
                                                        return 5;
                                                    else
                                                        return 10;
                                            else
                                                if (sensor5 <= 0.267142f)
                                                    return 3;
                                                else
                                                    return 10;
                                    else
                                        if (actual_omega <= 0.055902f)
                                            if (sensor3 <= 0.326299f)
                                                if (sensor1 <= 0.850654f)
                                                    if (sensor1 <= 0.846424f)
                                                        return 5;
                                                    else
                                                        return 10;
                                                else
                                                    if (actual_omega <= 0.000487f)
                                                        return 10;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vx <= 0.006721f)
                                                    if (actual_vz <= 1.011935f)
                                                        return 10;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vz <= 1.024040f)
                                                        return 5;
                                                    else
                                                        return 10;
                                        else
                                            return 6;
                            else
                                if (sensor2 <= 0.496371f)
                                    if (sensor0 <= 0.281950f)
                                        if (sensor2 <= 0.431103f)
                                            if (actual_vx <= -0.000731f)
                                                if (sensor5 <= 0.307133f)
                                                    return 10;
                                                else
                                                    return 5;
                                            else
                                                return 6;
                                        else
                                            if (sensor0 <= 0.281876f)
                                                if (sensor4 <= 0.859858f)
                                                    if (actual_vx <= -0.001341f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vx <= -0.000706f)
                                                        return 5;
                                                    else
                                                        return 6;
                                            else
                                                return 6;
                                    else
                                        if (sensor2 <= 0.426345f)
                                            if (last_action_state <= 0.650000f)
                                                return 7;
                                            else
                                                return 10;
                                        else
                                            if (sensor1 <= 0.968804f)
                                                if (sensor0 <= 0.318754f)
                                                    if (sensor2 <= 0.489900f)
                                                        return 5;
                                                    else
                                                        return 10;
                                                else
                                                    return 10;
                                            else
                                                return 6;
                                else
                                    if (actual_omega <= 0.082496f)
                                        if (actual_vz <= 0.977963f)
                                            if (actual_omega <= 0.001039f)
                                                if (sensor2 <= 0.515689f)
                                                    return 6;
                                                else
                                                    return 9;
                                            else
                                                if (sensor2 <= 0.499927f)
                                                    if (sensor3 <= 0.299663f)
                                                        return 5;
                                                    else
                                                        return 9;
                                                else
                                                    return 5;
                                        else
                                            if (sensor1 <= 0.859863f)
                                                return 8;
                                            else
                                                if (actual_vz <= 1.032909f)
                                                    if (actual_vz <= 1.025780f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor4 <= 0.860922f)
                                                        return 8;
                                                    else
                                                        return 7;
                                    else
                                        if (sensor5 <= 0.295793f)
                                            return 10;
                                        else
                                            return 6;
                        else
                            if (sensor3 <= 0.464938f)
                                if (actual_omega <= -0.110429f)
                                    return 5;
                                else
                                    return 9;
                            else
                                if (actual_vx <= 0.023245f)
                                    if (sensor3 <= 0.514734f)
                                        if (sensor2 <= 0.238576f)
                                            if (sensor5 <= 0.467235f)
                                                return 5;
                                            else
                                                return 10;
                                        else
                                            return 10;
                                    else
                                        if (actual_omega <= -0.128663f)
                                            return 6;
                                        else
                                            if (sensor2 <= 0.273228f)
                                                return 5;
                                            else
                                                return 10;
                                else
                                    return 9;
                    else
                        if (sensor2 <= 0.433787f)
                            if (sensor1 <= 0.844590f)
                                if (sensor0 <= 0.339123f)
                                    if (actual_omega <= 0.006304f)
                                        if (actual_vx <= -0.005240f)
                                            if (last_action_state <= 0.050000f)
                                                if (sensor5 <= 0.334057f)
                                                    if (sensor0 <= 0.329348f)
                                                        return 5;
                                                    else
                                                        return 0;
                                                else
                                                    if (actual_vx <= -0.006174f)
                                                        return 0;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor0 <= 0.299653f)
                                                    if (sensor4 <= 0.891979f)
                                                        return 5;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor5 <= 0.291952f)
                                                        return 5;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor1 <= 0.815953f)
                                                if (last_action_state <= 0.150000f)
                                                    if (actual_vz <= 1.256822f)
                                                        return 5;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_vx <= 0.006011f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vx <= 0.003783f)
                                                    if (sensor5 <= 0.424608f)
                                                        return 5;
                                                    else
                                                        return 0;
                                                else
                                                    if (sensor3 <= 0.372223f)
                                                        return 5;
                                                    else
                                                        return 5;
                                    else
                                        if (actual_vx <= 0.002421f)
                                            if (last_action_state <= 0.650000f)
                                                if (sensor1 <= 0.823970f)
                                                    if (sensor0 <= 0.329879f)
                                                        return 5;
                                                    else
                                                        return 1;
                                                else
                                                    if (last_action_state <= 0.150000f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor0 <= 0.299063f)
                                                    if (sensor3 <= 0.414392f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (actual_vx <= -0.000106f)
                                                        return 5;
                                                    else
                                                        return 5;
                                        else
                                            if (last_action_state <= 0.850000f)
                                                if (sensor3 <= 0.343342f)
                                                    if (actual_vz <= 1.331963f)
                                                        return 5;
                                                    else
                                                        return 0;
                                                else
                                                    if (sensor2 <= 0.309150f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vx <= 0.006343f)
                                                    if (sensor0 <= 0.303580f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor5 <= 0.253253f)
                                                        return 8;
                                                    else
                                                        return 10;
                                else
                                    if (actual_vx <= -0.004273f)
                                        if (sensor5 <= 0.294597f)
                                            if (sensor3 <= 0.345896f)
                                                if (sensor1 <= 0.839671f)
                                                    if (actual_vz <= 1.273587f)
                                                        return 1;
                                                    else
                                                        return 2;
                                                else
                                                    if (actual_omega <= -0.071162f)
                                                        return 1;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor2 <= 0.234809f)
                                                    if (actual_omega <= 0.002343f)
                                                        return 1;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor0 <= 0.339468f)
                                                        return 1;
                                                    else
                                                        return 5;
                                        else
                                            if (last_action_state <= 0.400000f)
                                                if (sensor5 <= 0.308972f)
                                                    if (sensor0 <= 0.347850f)
                                                        return 5;
                                                    else
                                                        return 0;
                                                else
                                                    if (actual_omega <= -0.106410f)
                                                        return 5;
                                                    else
                                                        return 1;
                                            else
                                                if (sensor5 <= 0.368753f)
                                                    if (sensor2 <= 0.243859f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor5 <= 0.370561f)
                                                        return 1;
                                                    else
                                                        return 0;
                                    else
                                        if (sensor5 <= 0.347654f)
                                            if (actual_vz <= 1.145636f)
                                                if (actual_omega <= -0.042573f)
                                                    if (sensor2 <= 0.272527f)
                                                        return 5;
                                                    else
                                                        return 0;
                                                else
                                                    if (actual_omega <= -0.037197f)
                                                        return 1;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vz <= 1.146458f)
                                                    if (sensor5 <= 0.286709f)
                                                        return 0;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_vx <= 0.005237f)
                                                        return 5;
                                                    else
                                                        return 5;
                                        else
                                            if (actual_vx <= 0.010533f)
                                                if (actual_vx <= -0.001325f)
                                                    if (last_action_state <= 0.150000f)
                                                        return 1;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= -0.116913f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor1 <= 0.799502f)
                                                    return 5;
                                                else
                                                    if (sensor3 <= 0.479649f)
                                                        return 5;
                                                    else
                                                        return 10;
                            else
                                if (actual_vx <= 0.004243f)
                                    if (actual_vx <= -0.005339f)
                                        if (sensor2 <= 0.287693f)
                                            if (sensor5 <= 0.302269f)
                                                if (last_action_state <= 0.150000f)
                                                    if (sensor3 <= 0.321803f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vx <= -0.011244f)
                                                        return 0;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vz <= 1.144915f)
                                                    if (sensor5 <= 0.372978f)
                                                        return 5;
                                                    else
                                                        return 0;
                                                else
                                                    if (sensor0 <= 0.350797f)
                                                        return 5;
                                                    else
                                                        return 0;
                                        else
                                            if (sensor2 <= 0.304365f)
                                                if (sensor3 <= 0.296515f)
                                                    if (sensor3 <= 0.268152f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor4 <= 0.880634f)
                                                        return 5;
                                                    else
                                                        return 0;
                                            else
                                                if (sensor5 <= 0.318245f)
                                                    if (sensor5 <= 0.251163f)
                                                        return 6;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= 0.065679f)
                                                        return 5;
                                                    else
                                                        return 0;
                                    else
                                        if (sensor2 <= 0.266506f)
                                            if (actual_vx <= -0.002565f)
                                                if (sensor5 <= 0.346301f)
                                                    if (sensor2 <= 0.239091f)
                                                        return 0;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vx <= -0.004056f)
                                                        return 0;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor3 <= 0.294477f)
                                                    if (actual_vx <= -0.001506f)
                                                        return 2;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor4 <= 0.985687f)
                                                        return 5;
                                                    else
                                                        return 1;
                                        else
                                            if (sensor1 <= 0.884918f)
                                                if (sensor0 <= 0.283405f)
                                                    if (sensor3 <= 0.337433f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor2 <= 0.281640f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor2 <= 0.393284f)
                                                    if (actual_omega <= 0.031792f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor2 <= 0.393707f)
                                                        return 6;
                                                    else
                                                        return 5;
                                else
                                    if (sensor0 <= 0.305242f)
                                        if (sensor5 <= 0.284861f)
                                            if (sensor0 <= 0.279037f)
                                                if (sensor0 <= 0.276576f)
                                                    if (sensor2 <= 0.350296f)
                                                        return 5;
                                                    else
                                                        return 10;
                                                else
                                                    if (actual_vz <= 1.195446f)
                                                        return 5;
                                                    else
                                                        return 10;
                                            else
                                                if (last_action_state <= 0.650000f)
                                                    if (actual_vx <= 0.006168f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor4 <= 0.837416f)
                                                        return 6;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor0 <= 0.279555f)
                                                if (actual_vz <= 1.226023f)
                                                    if (actual_vx <= 0.006763f)
                                                        return 5;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor5 <= 0.291921f)
                                                        return 10;
                                                    else
                                                        return 6;
                                            else
                                                if (actual_vx <= 0.007387f)
                                                    if (last_action_state <= 0.850000f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor0 <= 0.303106f)
                                                        return 5;
                                                    else
                                                        return 10;
                                    else
                                        if (actual_vx <= 0.010552f)
                                            if (last_action_state <= 0.650000f)
                                                if (sensor0 <= 0.326366f)
                                                    if (sensor0 <= 0.326319f)
                                                        return 5;
                                                    else
                                                        return 6;
                                                else
                                                    if (sensor4 <= 0.778539f)
                                                        return 6;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor0 <= 0.316988f)
                                                    if (sensor3 <= 0.315690f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor5 <= 0.346700f)
                                                        return 5;
                                                    else
                                                        return 8;
                                        else
                                            if (actual_vx <= 0.016542f)
                                                if (actual_vx <= 0.014268f)
                                                    if (actual_vz <= 1.126032f)
                                                        return 10;
                                                    else
                                                        return 6;
                                                else
                                                    return 9;
                                            else
                                                return 10;
                        else
                            if (sensor2 <= 0.478450f)
                                if (last_action_state <= 0.950000f)
                                    if (actual_vx <= -0.002965f)
                                        if (actual_omega <= 0.012461f)
                                            if (sensor1 <= 0.854900f)
                                                if (sensor2 <= 0.448934f)
                                                    if (sensor5 <= 0.281062f)
                                                        return 5;
                                                    else
                                                        return 8;
                                                else
                                                    return 6;
                                            else
                                                if (sensor3 <= 0.344053f)
                                                    if (actual_omega <= 0.000513f)
                                                        return 5;
                                                    else
                                                        return 9;
                                                else
                                                    return 5;
                                        else
                                            return 5;
                                    else
                                        if (sensor1 <= 0.890988f)
                                            if (sensor3 <= 0.315438f)
                                                if (sensor2 <= 0.452553f)
                                                    if (sensor5 <= 0.300279f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor2 <= 0.461803f)
                                                        return 10;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vz <= 1.128998f)
                                                    if (actual_vx <= -0.002590f)
                                                        return 9;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor1 <= 0.884159f)
                                                        return 6;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor2 <= 0.458949f)
                                                if (actual_vx <= 0.001484f)
                                                    if (sensor3 <= 0.291661f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vz <= 1.097466f)
                                                        return 10;
                                                    else
                                                        return 8;
                                            else
                                                if (actual_omega <= 0.003798f)
                                                    if (actual_vx <= 0.000631f)
                                                        return 10;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor1 <= 0.933630f)
                                                        return 5;
                                                    else
                                                        return 5;
                                else
                                    if (actual_omega <= 0.079377f)
                                        if (sensor4 <= 0.852288f)
                                            if (sensor4 <= 0.808288f)
                                                return 6;
                                            else
                                                if (actual_vz <= 1.092979f)
                                                    return 5;
                                                else
                                                    if (sensor5 <= 0.298302f)
                                                        return 8;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor3 <= 0.327433f)
                                                if (sensor5 <= 0.319955f)
                                                    if (actual_vx <= -0.002888f)
                                                        return 5;
                                                    else
                                                        return 10;
                                                else
                                                    if (actual_omega <= 0.079121f)
                                                        return 6;
                                                    else
                                                        return 10;
                                            else
                                                return 8;
                                    else
                                        if (sensor2 <= 0.438033f)
                                            if (sensor1 <= 0.882686f)
                                                return 5;
                                            else
                                                return 10;
                                        else
                                            if (actual_omega <= 0.106152f)
                                                return 6;
                                            else
                                                if (actual_omega <= 0.111753f)
                                                    return 5;
                                                else
                                                    return 6;
                            else
                                if (sensor3 <= 0.304731f)
                                    if (sensor3 <= 0.303607f)
                                        if (actual_vx <= -0.000647f)
                                            if (sensor2 <= 0.496506f)
                                                if (actual_vx <= -0.003339f)
                                                    if (sensor3 <= 0.300420f)
                                                        return 5;
                                                    else
                                                        return 8;
                                                else
                                                    if (sensor0 <= 0.276793f)
                                                        return 5;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor4 <= 0.845242f)
                                                    if (actual_vz <= 1.056381f)
                                                        return 10;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor2 <= 0.543163f)
                                                        return 6;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor4 <= 0.843905f)
                                                if (sensor0 <= 0.275093f)
                                                    if (actual_vx <= -0.000418f)
                                                        return 8;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor4 <= 0.836325f)
                                                        return 6;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_omega <= 0.000482f)
                                                    return 5;
                                                else
                                                    if (sensor5 <= 0.304180f)
                                                        return 7;
                                                    else
                                                        return 8;
                                    else
                                        return 5;
                                else
                                    if (actual_vz <= 1.188572f)
                                        if (sensor2 <= 0.506550f)
                                            if (sensor4 <= 0.854667f)
                                                if (sensor4 <= 0.852815f)
                                                    if (sensor4 <= 0.847126f)
                                                        return 10;
                                                    else
                                                        return 5;
                                                else
                                                    return 10;
                                            else
                                                if (sensor3 <= 0.309560f)
                                                    return 5;
                                                else
                                                    if (sensor3 <= 0.326721f)
                                                        return 6;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor3 <= 0.320411f)
                                                if (sensor3 <= 0.315251f)
                                                    if (sensor0 <= 0.272713f)
                                                        return 10;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor5 <= 0.292025f)
                                                        return 9;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor0 <= 0.289720f)
                                                    return 10;
                                                else
                                                    return 6;
                                    else
                                        if (actual_omega <= 0.065167f)
                                            if (sensor3 <= 0.321748f)
                                                return 9;
                                            else
                                                if (actual_omega <= 0.017921f)
                                                    return 5;
                                                else
                                                    if (sensor3 <= 0.327866f)
                                                        return 7;
                                                    else
                                                        return 6;
                                        else
                                            if (actual_omega <= 0.079711f)
                                                return 8;
                                            else
                                                if (actual_vx <= -0.004278f)
                                                    if (actual_vz <= 1.206078f)
                                                        return 10;
                                                    else
                                                        return 8;
                                                else
                                                    return 10;
                else
                    if (last_action_state <= 0.250000f)
                        if (actual_omega <= -0.156719f)
                            if (sensor0 <= 0.442120f)
                                if (sensor5 <= 0.339656f)
                                    if (sensor0 <= 0.377148f)
                                        return 0;
                                    else
                                        if (actual_omega <= -0.179117f)
                                            if (sensor5 <= 0.250940f)
                                                return 10;
                                            else
                                                if (actual_vx <= -0.000461f)
                                                    if (sensor5 <= 0.313112f)
                                                        return 1;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor4 <= 0.843928f)
                                                        return 5;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor3 <= 0.515334f)
                                                if (sensor4 <= 0.881940f)
                                                    if (sensor0 <= 0.399342f)
                                                        return 5;
                                                    else
                                                        return 0;
                                                else
                                                    if (actual_omega <= -0.178160f)
                                                        return 0;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor5 <= 0.291896f)
                                                    if (actual_omega <= -0.166215f)
                                                        return 2;
                                                    else
                                                        return 5;
                                                else
                                                    return 5;
                                else
                                    if (actual_vx <= 0.002141f)
                                        if (sensor1 <= 0.816047f)
                                            if (actual_vx <= -0.000753f)
                                                return 5;
                                            else
                                                if (sensor2 <= 0.210372f)
                                                    return 0;
                                                else
                                                    return 2;
                                        else
                                            if (sensor2 <= 0.209785f)
                                                if (actual_omega <= -0.162109f)
                                                    if (actual_vz <= 1.256312f)
                                                        return 1;
                                                    else
                                                        return 1;
                                                else
                                                    return 0;
                                            else
                                                if (sensor4 <= 0.890927f)
                                                    return 1;
                                                else
                                                    if (sensor1 <= 0.834109f)
                                                        return 5;
                                                    else
                                                        return 0;
                                    else
                                        if (sensor0 <= 0.428242f)
                                            if (sensor4 <= 0.882229f)
                                                return 9;
                                            else
                                                if (sensor5 <= 0.371409f)
                                                    if (actual_omega <= -0.230956f)
                                                        return 2;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor1 <= 0.830055f)
                                                        return 1;
                                                    else
                                                        return 5;
                                        else
                                            if (actual_vz <= 1.254094f)
                                                return 0;
                                            else
                                                if (sensor5 <= 0.346093f)
                                                    return 5;
                                                else
                                                    return 1;
                            else
                                if (actual_vz <= 1.145559f)
                                    if (sensor0 <= 0.637334f)
                                        if (sensor5 <= 0.299541f)
                                            if (actual_vx <= 0.016699f)
                                                if (actual_omega <= -0.156980f)
                                                    if (sensor2 <= 0.152125f)
                                                        return 4;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor0 <= 0.573926f)
                                                        return 5;
                                                    else
                                                        return 2;
                                            else
                                                if (sensor3 <= 0.454539f)
                                                    return 0;
                                                else
                                                    return 2;
                                        else
                                            if (sensor1 <= 0.820848f)
                                                if (sensor1 <= 0.811509f)
                                                    if (sensor5 <= 0.304775f)
                                                        return 0;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= -0.173299f)
                                                        return 1;
                                                    else
                                                        return 0;
                                            else
                                                if (sensor4 <= 0.854825f)
                                                    if (sensor2 <= 0.189094f)
                                                        return 5;
                                                    else
                                                        return 1;
                                                else
                                                    return 5;
                                    else
                                        if (actual_vz <= 1.112266f)
                                            if (actual_omega <= -0.167931f)
                                                return 5;
                                            else
                                                if (sensor3 <= 0.449847f)
                                                    return 1;
                                                else
                                                    return 5;
                                        else
                                            if (sensor4 <= 0.865451f)
                                                if (sensor2 <= 0.225443f)
                                                    if (sensor0 <= 0.659488f)
                                                        return 1;
                                                    else
                                                        return 0;
                                                else
                                                    if (sensor0 <= 0.654159f)
                                                        return 2;
                                                    else
                                                        return 5;
                                            else
                                                return 0;
                                else
                                    if (actual_omega <= -0.206990f)
                                        if (actual_vx <= 0.004584f)
                                            if (sensor5 <= 0.276933f)
                                                if (actual_vz <= 1.238187f)
                                                    return 5;
                                                else
                                                    if (sensor2 <= 0.176675f)
                                                        return 2;
                                                    else
                                                        return 0;
                                            else
                                                if (sensor0 <= 0.446393f)
                                                    return 3;
                                                else
                                                    if (last_action_state <= 0.050000f)
                                                        return 0;
                                                    else
                                                        return 1;
                                        else
                                            if (actual_vz <= 1.190929f)
                                                if (sensor0 <= 0.467957f)
                                                    if (actual_omega <= -0.263507f)
                                                        return 5;
                                                    else
                                                        return 10;
                                                else
                                                    if (sensor5 <= 0.209305f)
                                                        return 1;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor2 <= 0.260406f)
                                                    if (actual_vx <= 0.016922f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    return 10;
                                    else
                                        if (sensor0 <= 0.611402f)
                                            if (sensor5 <= 0.248433f)
                                                if (sensor0 <= 0.467872f)
                                                    return 1;
                                                else
                                                    if (sensor3 <= 0.689126f)
                                                        return 5;
                                                    else
                                                        return 1;
                                            else
                                                if (sensor2 <= 0.175751f)
                                                    if (sensor5 <= 0.326462f)
                                                        return 1;
                                                    else
                                                        return 0;
                                                else
                                                    if (sensor3 <= 0.559972f)
                                                        return 5;
                                                    else
                                                        return 5;
                                        else
                                            if (actual_vx <= 0.016865f)
                                                if (sensor1 <= 0.798729f)
                                                    return 2;
                                                else
                                                    if (actual_omega <= -0.197565f)
                                                        return 1;
                                                    else
                                                        return 1;
                                            else
                                                if (actual_omega <= -0.199786f)
                                                    return 5;
                                                else
                                                    if (sensor5 <= 0.219480f)
                                                        return 5;
                                                    else
                                                        return 1;
                        else
                            if (sensor0 <= 0.513108f)
                                if (actual_vz <= 1.098509f)
                                    if (sensor0 <= 0.467858f)
                                        if (sensor5 <= 0.585239f)
                                            if (actual_vx <= -0.001653f)
                                                if (sensor5 <= 0.311608f)
                                                    if (sensor3 <= 0.550688f)
                                                        return 5;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_vx <= -0.003290f)
                                                        return 0;
                                                    else
                                                        return 1;
                                            else
                                                if (sensor0 <= 0.428073f)
                                                    if (actual_vx <= 0.011021f)
                                                        return 5;
                                                    else
                                                        return 0;
                                                else
                                                    if (actual_omega <= -0.068985f)
                                                        return 5;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor5 <= 0.605153f)
                                                if (actual_omega <= -0.065194f)
                                                    if (sensor0 <= 0.394451f)
                                                        return 1;
                                                    else
                                                        return 0;
                                                else
                                                    return 1;
                                            else
                                                if (sensor1 <= 0.809796f)
                                                    return 0;
                                                else
                                                    return 5;
                                    else
                                        if (actual_vz <= 1.071839f)
                                            if (actual_vx <= 0.000212f)
                                                if (sensor2 <= 0.163683f)
                                                    if (actual_vx <= -0.003925f)
                                                        return 0;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= -0.136340f)
                                                        return 5;
                                                    else
                                                        return 1;
                                            else
                                                if (sensor5 <= 0.290827f)
                                                    if (sensor0 <= 0.507408f)
                                                        return 5;
                                                    else
                                                        return 0;
                                                else
                                                    if (sensor5 <= 0.295745f)
                                                        return 1;
                                                    else
                                                        return 5;
                                        else
                                            if (actual_vx <= 0.004489f)
                                                if (sensor5 <= 0.299036f)
                                                    if (sensor5 <= 0.277822f)
                                                        return 0;
                                                    else
                                                        return 0;
                                                else
                                                    if (sensor0 <= 0.502359f)
                                                        return 1;
                                                    else
                                                        return 0;
                                            else
                                                if (sensor4 <= 0.849963f)
                                                    if (sensor0 <= 0.490864f)
                                                        return 0;
                                                    else
                                                        return 2;
                                                else
                                                    if (sensor4 <= 0.868390f)
                                                        return 5;
                                                    else
                                                        return 0;
                                else
                                    if (actual_vx <= 0.005281f)
                                        if (sensor0 <= 0.448473f)
                                            if (sensor5 <= 0.305105f)
                                                if (sensor0 <= 0.437128f)
                                                    if (actual_vx <= -0.002896f)
                                                        return 1;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor0 <= 0.446880f)
                                                        return 1;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_omega <= -0.119030f)
                                                    if (actual_vz <= 1.288174f)
                                                        return 5;
                                                    else
                                                        return 0;
                                                else
                                                    if (sensor2 <= 0.225012f)
                                                        return 0;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor4 <= 0.873114f)
                                                if (sensor2 <= 0.168329f)
                                                    if (sensor1 <= 0.807664f)
                                                        return 1;
                                                    else
                                                        return 0;
                                                else
                                                    if (actual_omega <= -0.127479f)
                                                        return 5;
                                                    else
                                                        return 0;
                                            else
                                                if (last_action_state <= 0.050000f)
                                                    if (sensor0 <= 0.452967f)
                                                        return 0;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor3 <= 0.336461f)
                                                        return 0;
                                                    else
                                                        return 1;
                                    else
                                        if (actual_omega <= -0.095182f)
                                            if (sensor2 <= 0.235218f)
                                                if (sensor2 <= 0.198148f)
                                                    if (sensor1 <= 0.801775f)
                                                        return 5;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor0 <= 0.421895f)
                                                        return 0;
                                                    else
                                                        return 2;
                                            else
                                                if (actual_omega <= -0.122195f)
                                                    if (actual_vz <= 1.270864f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor3 <= 0.370269f)
                                                        return 5;
                                                    else
                                                        return 1;
                                        else
                                            if (sensor0 <= 0.492673f)
                                                if (sensor1 <= 0.843433f)
                                                    return 5;
                                                else
                                                    if (actual_omega <= -0.075208f)
                                                        return 5;
                                                    else
                                                        return 1;
                                            else
                                                if (sensor2 <= 0.271388f)
                                                    if (sensor4 <= 0.859754f)
                                                        return 2;
                                                    else
                                                        return 0;
                                                else
                                                    return 1;
                            else
                                if (actual_vx <= 0.005616f)
                                    if (sensor5 <= 0.318232f)
                                        if (actual_vz <= 1.022349f)
                                            if (sensor0 <= 0.549095f)
                                                return 5;
                                            else
                                                return 0;
                                        else
                                            if (sensor0 <= 0.657814f)
                                                if (sensor3 <= 0.344081f)
                                                    if (sensor1 <= 0.816664f)
                                                        return 0;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor5 <= 0.272924f)
                                                        return 1;
                                                    else
                                                        return 1;
                                            else
                                                return 2;
                                    else
                                        return 0;
                                else
                                    if (sensor0 <= 0.548912f)
                                        if (actual_omega <= -0.086306f)
                                            if (sensor5 <= 0.265393f)
                                                if (actual_vz <= 1.198005f)
                                                    if (sensor3 <= 0.362590f)
                                                        return 0;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor2 <= 0.236666f)
                                                        return 2;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor1 <= 0.861011f)
                                                    if (actual_vx <= 0.009278f)
                                                        return 1;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= -0.122368f)
                                                        return 5;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor1 <= 0.861140f)
                                                if (sensor0 <= 0.545203f)
                                                    if (sensor1 <= 0.854289f)
                                                        return 1;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor4 <= 0.869015f)
                                                        return 5;
                                                    else
                                                        return 1;
                                            else
                                                return 0;
                                    else
                                        if (sensor3 <= 0.388514f)
                                            if (sensor2 <= 0.244409f)
                                                if (sensor1 <= 0.849580f)
                                                    if (sensor3 <= 0.360996f)
                                                        return 1;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_vx <= 0.008059f)
                                                        return 1;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor2 <= 0.248315f)
                                                    if (sensor3 <= 0.369432f)
                                                        return 0;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vz <= 1.133884f)
                                                        return 5;
                                                    else
                                                        return 1;
                                        else
                                            if (sensor0 <= 0.574538f)
                                                if (actual_vz <= 1.249011f)
                                                    if (sensor2 <= 0.253182f)
                                                        return 5;
                                                    else
                                                        return 0;
                                                else
                                                    if (actual_omega <= -0.095084f)
                                                        return 1;
                                                    else
                                                        return 0;
                                            else
                                                if (actual_vz <= 1.246636f)
                                                    if (sensor3 <= 0.388953f)
                                                        return 5;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor4 <= 0.868844f)
                                                        return 0;
                                                    else
                                                        return 1;
                    else
                        if (sensor2 <= 0.266169f)
                            if (actual_omega <= -0.086307f)
                                if (last_action_state <= 0.850000f)
                                    if (sensor0 <= 0.569648f)
                                        if (actual_vx <= 0.025497f)
                                            if (actual_vz <= 1.153157f)
                                                if (actual_vx <= 0.001598f)
                                                    if (sensor1 <= 0.820165f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= -0.223444f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vx <= -0.000416f)
                                                    if (sensor0 <= 0.394095f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor3 <= 0.583477f)
                                                        return 5;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor1 <= 0.803310f)
                                                return 5;
                                            else
                                                if (actual_omega <= -0.124997f)
                                                    return 10;
                                                else
                                                    return 5;
                                    else
                                        if (actual_vz <= 1.197926f)
                                            if (actual_omega <= -0.086950f)
                                                if (sensor0 <= 0.602384f)
                                                    if (sensor1 <= 0.840821f)
                                                        return 5;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_vx <= 0.012218f)
                                                        return 1;
                                                    else
                                                        return 5;
                                            else
                                                return 1;
                                        else
                                            if (actual_vx <= 0.013262f)
                                                if (sensor0 <= 0.619992f)
                                                    if (actual_vx <= 0.011283f)
                                                        return 1;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor4 <= 0.848365f)
                                                        return 0;
                                                    else
                                                        return 1;
                                            else
                                                if (sensor1 <= 0.809814f)
                                                    if (actual_vx <= 0.015211f)
                                                        return 1;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor5 <= 0.252039f)
                                                        return 5;
                                                    else
                                                        return 2;
                                else
                                    if (actual_vx <= 0.022218f)
                                        if (actual_omega <= -0.165097f)
                                            if (sensor3 <= 0.711525f)
                                                if (sensor5 <= 0.314661f)
                                                    if (sensor1 <= 0.815235f)
                                                        return 10;
                                                    else
                                                        return 10;
                                                else
                                                    if (actual_omega <= -0.182770f)
                                                        return 5;
                                                    else
                                                        return 6;
                                            else
                                                if (actual_omega <= -0.185291f)
                                                    return 9;
                                                else
                                                    return 8;
                                        else
                                            if (actual_vx <= 0.003519f)
                                                if (sensor1 <= 0.832422f)
                                                    if (sensor2 <= 0.169184f)
                                                        return 10;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_vx <= 0.001553f)
                                                        return 10;
                                                    else
                                                        return 6;
                                            else
                                                if (sensor5 <= 0.260098f)
                                                    if (sensor0 <= 0.634023f)
                                                        return 5;
                                                    else
                                                        return 8;
                                                else
                                                    if (sensor5 <= 0.266571f)
                                                        return 10;
                                                    else
                                                        return 5;
                                    else
                                        if (actual_vz <= 0.895529f)
                                            return 5;
                                        else
                                            if (sensor3 <= 0.531327f)
                                                return 10;
                                            else
                                                if (sensor2 <= 0.226077f)
                                                    return 8;
                                                else
                                                    return 10;
                            else
                                if (actual_vz <= 1.131974f)
                                    if (actual_vz <= 1.024390f)
                                        if (sensor5 <= 0.555110f)
                                            if (sensor0 <= 0.450591f)
                                                if (sensor2 <= 0.193146f)
                                                    return 2;
                                                else
                                                    if (sensor0 <= 0.377239f)
                                                        return 2;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor5 <= 0.292353f)
                                                    if (actual_omega <= 0.001324f)
                                                        return 5;
                                                    else
                                                        return 0;
                                                else
                                                    if (actual_vz <= 1.014877f)
                                                        return 5;
                                                    else
                                                        return 1;
                                        else
                                            if (sensor1 <= 0.826389f)
                                                return 0;
                                            else
                                                if (actual_vx <= 0.005573f)
                                                    return 1;
                                                else
                                                    if (sensor1 <= 0.846094f)
                                                        return 5;
                                                    else
                                                        return 0;
                                    else
                                        if (actual_omega <= -0.055887f)
                                            if (sensor1 <= 0.829885f)
                                                if (sensor0 <= 0.618766f)
                                                    if (sensor5 <= 0.293201f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor5 <= 0.231705f)
                                                        return 5;
                                                    else
                                                        return 0;
                                            else
                                                if (actual_vz <= 1.025543f)
                                                    return 1;
                                                else
                                                    if (actual_vz <= 1.104935f)
                                                        return 5;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor1 <= 0.842020f)
                                                if (last_action_state <= 0.550000f)
                                                    if (sensor0 <= 0.414998f)
                                                        return 5;
                                                    else
                                                        return 1;
                                                else
                                                    if (sensor4 <= 0.829863f)
                                                        return 4;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor0 <= 0.454214f)
                                                    if (sensor1 <= 0.848406f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor5 <= 0.312818f)
                                                        return 5;
                                                    else
                                                        return 1;
                                else
                                    if (sensor0 <= 0.502257f)
                                        if (last_action_state <= 0.700000f)
                                            if (sensor5 <= 0.280261f)
                                                if (sensor0 <= 0.418527f)
                                                    if (sensor3 <= 0.455715f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor1 <= 0.881767f)
                                                        return 5;
                                                    else
                                                        return 0;
                                            else
                                                if (sensor1 <= 0.814910f)
                                                    if (sensor4 <= 0.905588f)
                                                        return 1;
                                                    else
                                                        return 0;
                                                else
                                                    if (actual_omega <= -0.040572f)
                                                        return 5;
                                                    else
                                                        return 0;
                                        else
                                            if (actual_vx <= 0.011113f)
                                                if (sensor3 <= 0.483883f)
                                                    if (sensor1 <= 0.833106f)
                                                        return 5;
                                                    else
                                                        return 4;
                                                else
                                                    if (sensor3 <= 0.663135f)
                                                        return 5;
                                                    else
                                                        return 9;
                                            else
                                                return 9;
                                    else
                                        if (actual_omega <= -0.049222f)
                                            if (sensor0 <= 0.551243f)
                                                if (sensor0 <= 0.507443f)
                                                    return 2;
                                                else
                                                    if (sensor2 <= 0.156492f)
                                                        return 2;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vx <= 0.005021f)
                                                    if (actual_omega <= -0.057614f)
                                                        return 0;
                                                    else
                                                        return 1;
                                                else
                                                    if (actual_vz <= 1.185194f)
                                                        return 5;
                                                    else
                                                        return 1;
                                        else
                                            if (actual_vz <= 1.262257f)
                                                if (actual_vx <= 0.001075f)
                                                    if (sensor5 <= 0.283009f)
                                                        return 0;
                                                    else
                                                        return 0;
                                                else
                                                    if (actual_vz <= 1.177218f)
                                                        return 1;
                                                    else
                                                        return 1;
                                            else
                                                return 5;
                        else
                            if (sensor0 <= 0.423074f)
                                if (actual_vz <= 1.113626f)
                                    if (last_action_state <= 0.900000f)
                                        if (actual_vx <= -0.002462f)
                                            return 0;
                                        else
                                            if (actual_vx <= 0.000525f)
                                                if (sensor0 <= 0.399093f)
                                                    if (sensor5 <= 0.270480f)
                                                        return 5;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor1 <= 0.877277f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vx <= 0.000529f)
                                                    return 1;
                                                else
                                                    if (sensor0 <= 0.415052f)
                                                        return 5;
                                                    else
                                                        return 5;
                                    else
                                        return 10;
                                else
                                    if (actual_omega <= 0.000651f)
                                        if (actual_vx <= 0.007000f)
                                            if (sensor5 <= 0.312905f)
                                                if (sensor2 <= 0.268496f)
                                                    if (actual_omega <= -0.000501f)
                                                        return 0;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor0 <= 0.377603f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor2 <= 0.273343f)
                                                    return 1;
                                                else
                                                    if (sensor5 <= 0.313107f)
                                                        return 0;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor1 <= 0.814622f)
                                                return 1;
                                            else
                                                return 4;
                                    else
                                        if (last_action_state <= 0.900000f)
                                            if (actual_omega <= 0.000654f)
                                                if (sensor1 <= 0.872383f)
                                                    return 0;
                                                else
                                                    return 1;
                                            else
                                                if (actual_vz <= 1.113764f)
                                                    return 0;
                                                else
                                                    if (sensor3 <= 0.297475f)
                                                        return 5;
                                                    else
                                                        return 5;
                                        else
                                            if (sensor2 <= 0.305746f)
                                                return 10;
                                            else
                                                return 8;
                            else
                                if (actual_vz <= 1.133936f)
                                    if (actual_omega <= 0.001464f)
                                        if (sensor0 <= 0.496404f)
                                            if (sensor1 <= 0.889498f)
                                                if (sensor0 <= 0.423764f)
                                                    return 0;
                                                else
                                                    if (sensor5 <= 0.298349f)
                                                        return 5;
                                                    else
                                                        return 5;
                                            else
                                                if (sensor4 <= 0.792401f)
                                                    if (sensor5 <= 0.270163f)
                                                        return 1;
                                                    else
                                                        return 0;
                                                else
                                                    if (sensor0 <= 0.446548f)
                                                        return 5;
                                                    else
                                                        return 5;
                                        else
                                            if (actual_omega <= -0.020786f)
                                                if (sensor2 <= 0.268496f)
                                                    if (sensor4 <= 0.855782f)
                                                        return 5;
                                                    else
                                                        return 0;
                                                else
                                                    if (sensor4 <= 0.826548f)
                                                        return 0;
                                                    else
                                                        return 5;
                                            else
                                                if (actual_vz <= 1.097228f)
                                                    return 0;
                                                else
                                                    return 1;
                                    else
                                        if (sensor1 <= 0.860722f)
                                            if (actual_omega <= 0.002353f)
                                                return 2;
                                            else
                                                return 1;
                                        else
                                            if (sensor3 <= 0.280107f)
                                                if (sensor2 <= 0.331479f)
                                                    return 0;
                                                else
                                                    return 1;
                                            else
                                                if (sensor3 <= 0.294367f)
                                                    if (actual_vx <= -0.000990f)
                                                        return 1;
                                                    else
                                                        return 5;
                                                else
                                                    if (actual_omega <= 0.004210f)
                                                        return 5;
                                                    else
                                                        return 5;
                                else
                                    if (sensor5 <= 0.278972f)
                                        if (actual_vz <= 1.262986f)
                                            if (sensor2 <= 0.268509f)
                                                if (sensor1 <= 0.857259f)
                                                    if (sensor4 <= 0.847239f)
                                                        return 5;
                                                    else
                                                        return 0;
                                                else
                                                    if (sensor3 <= 0.364870f)
                                                        return 5;
                                                    else
                                                        return 2;
                                            else
                                                if (sensor4 <= 0.870024f)
                                                    return 5;
                                                else
                                                    if (sensor1 <= 0.864419f)
                                                        return 5;
                                                    else
                                                        return 1;
                                        else
                                            return 6;
                                    else
                                        if (actual_omega <= -0.000616f)
                                            if (sensor0 <= 0.488694f)
                                                if (sensor4 <= 0.878506f)
                                                    if (actual_vx <= -0.000615f)
                                                        return 1;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor4 <= 0.917800f)
                                                        return 1;
                                                    else
                                                        return 3;
                                            else
                                                if (sensor4 <= 0.845625f)
                                                    if (sensor0 <= 0.499314f)
                                                        return 2;
                                                    else
                                                        return 5;
                                                else
                                                    if (sensor1 <= 0.878501f)
                                                        return 1;
                                                    else
                                                        return 0;
                                        else
                                            if (sensor1 <= 0.877222f)
                                                if (sensor4 <= 0.860889f)
                                                    return 1;
                                                else
                                                    if (sensor2 <= 0.274609f)
                                                        return 0;
                                                    else
                                                        return 1;
                                            else
                                                if (actual_vx <= 0.000439f)
                                                    if (actual_vx <= 0.000324f)
                                                        return 0;
                                                    else
                                                        return 0;
                                                else
                                                    if (actual_vx <= 0.000962f)
                                                        return 5;
                                                    else
                                                        return 0;
    }
}
