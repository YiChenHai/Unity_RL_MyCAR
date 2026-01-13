// 决策树规则库 - 自动生成
// 数据样本数: 100000
// R² Score - action_x: 0.7796, action_w: 0.6464
// 树深度: 8

using System;

public class DecisionTreeRules
{
    // 特征变量（需要从观测中获取）
    private float sensor0, sensor1, sensor2, sensor3, sensor4, sensor5;
    private float smoothed_vx, smoothed_omega;
    private float raw_action_x, raw_action_w;
    private float actual_vz, actual_vx, actual_omega;
    
    // 设置观测值
    public void SetObservations(float[] obs)
    {
        if (obs.Length < 13) throw new ArgumentException("观测数组长度不足13");
        sensor0 = obs[0];
        sensor1 = obs[1];
        sensor2 = obs[2];
        sensor3 = obs[3];
        sensor4 = obs[4];
        sensor5 = obs[5];
        smoothed_vx = obs[6];
        smoothed_omega = obs[7];
        raw_action_x = obs[8];
        raw_action_w = obs[9];
        actual_vz = obs[10];
        actual_vx = obs[11];
        actual_omega = obs[12];
    }
    
    // 预测横向速度 action_x
    public float PredictActionX()
    {
        if (smoothed_vx <= -0.423385f)
            if (raw_action_x <= -0.926680f)
                if (sensor0 <= 0.411129f)
                    if (sensor0 <= 0.363524f)
                        if (raw_action_w <= -0.556134f)
                            if (sensor2 <= 0.222053f)
                                return -0.766464f;
                            else
                                return -0.310738f;
                        else
                            if (sensor4 <= 0.842332f)
                                if (sensor2 <= 0.218370f)
                                    if (sensor4 <= 0.753174f)
                                        return 0.112632f;
                                    else
                                        return -0.114309f;
                                else
                                    return -0.306223f;
                            else
                                return 0.248761f;
                    else
                        if (actual_omega <= 0.003259f)
                            if (sensor4 <= 0.598937f)
                                return -0.653644f;
                            else
                                if (actual_omega <= -0.092714f)
                                    return -0.017087f;
                                else
                                    if (smoothed_omega <= -0.279508f)
                                        return -0.201510f;
                                    else
                                        return -0.510596f;
                        else
                            if (actual_vx <= -0.111665f)
                                if (actual_vx <= -0.607696f)
                                    if (actual_vz <= 0.920518f)
                                        return -0.560721f;
                                    else
                                        return -0.821113f;
                                else
                                    if (sensor2 <= 0.180429f)
                                        return -0.912066f;
                                    else
                                        return -0.707928f;
                            else
                                if (actual_omega <= 0.232900f)
                                    if (sensor2 <= 0.275457f)
                                        return -0.706710f;
                                    else
                                        return -0.268229f;
                                else
                                    return -0.718666f;
                else
                    if (sensor1 <= 0.757213f)
                        if (sensor3 <= 0.235647f)
                            if (sensor5 <= 0.446951f)
                                if (sensor0 <= 0.761030f)
                                    return -1.000000f;
                                else
                                    return -0.962768f;
                            else
                                if (smoothed_vx <= -0.807936f)
                                    if (sensor5 <= 0.836731f)
                                        return -0.753712f;
                                    else
                                        return -0.564289f;
                                else
                                    return -0.236032f;
                        else
                            if (sensor1 <= 0.621539f)
                                if (sensor3 <= 0.256231f)
                                    if (actual_vx <= -0.578835f)
                                        return -0.785316f;
                                    else
                                        return -0.993803f;
                                else
                                    if (raw_action_x <= -0.983357f)
                                        return -0.995707f;
                                    else
                                        return -0.934046f;
                            else
                                if (actual_vx <= -0.618546f)
                                    if (sensor2 <= 0.132547f)
                                        return -0.956786f;
                                    else
                                        return -0.817769f;
                                else
                                    if (sensor5 <= 0.752921f)
                                        return -0.988191f;
                                    else
                                        return -0.867651f;
                    else
                        if (actual_omega <= -0.178286f)
                            if (actual_vz <= 0.751626f)
                                if (sensor2 <= 0.152167f)
                                    if (sensor5 <= 0.242331f)
                                        return -0.991859f;
                                    else
                                        return -0.794563f;
                                else
                                    if (actual_vz <= 0.648992f)
                                        return -0.762511f;
                                    else
                                        return -0.539424f;
                            else
                                if (actual_omega <= -0.219746f)
                                    return -0.203588f;
                                else
                                    return -0.476104f;
                        else
                            if (sensor0 <= 0.962375f)
                                if (sensor2 <= 0.167107f)
                                    if (sensor4 <= 0.778014f)
                                        return -0.974051f;
                                    else
                                        return -0.891179f;
                                else
                                    if (sensor0 <= 0.575034f)
                                        return -0.747748f;
                                    else
                                        return -0.882657f;
                            else
                                if (sensor1 <= 0.872129f)
                                    if (sensor2 <= 0.266343f)
                                        return -0.988132f;
                                    else
                                        return -0.949120f;
                                else
                                    if (smoothed_vx <= -0.789156f)
                                        return -0.809541f;
                                    else
                                        return -0.954825f;
            else
                if (sensor0 <= 0.362976f)
                    if (sensor0 <= 0.329292f)
                        if (actual_vx <= -0.566429f)
                            if (actual_omega <= 0.007993f)
                                if (raw_action_w <= -0.815753f)
                                    return 0.133963f;
                                else
                                    if (sensor1 <= 0.774861f)
                                        return 0.404230f;
                                    else
                                        return 0.688374f;
                            else
                                if (sensor3 <= 0.653540f)
                                    return 0.448980f;
                                else
                                    if (actual_vx <= -0.659956f)
                                        return 0.329808f;
                                    else
                                        return -0.020077f;
                        else
                            if (smoothed_omega <= -0.574458f)
                                if (sensor0 <= 0.323411f)
                                    if (raw_action_x <= -0.406532f)
                                        return -0.196828f;
                                    else
                                        return -0.363934f;
                                else
                                    if (actual_omega <= 0.075906f)
                                        return -0.336443f;
                                    else
                                        return -0.633682f;
                            else
                                if (raw_action_w <= -0.712095f)
                                    return 0.174975f;
                                else
                                    if (sensor3 <= 0.580154f)
                                        return -0.046529f;
                                    else
                                        return -0.208219f;
                    else
                        if (actual_vx <= -0.576508f)
                            if (sensor5 <= 0.174506f)
                                if (raw_action_x <= -0.058550f)
                                    if (raw_action_w <= 0.083529f)
                                        return -0.319484f;
                                    else
                                        return -0.040755f;
                                else
                                    return -0.643436f;
                            else
                                if (smoothed_omega <= -0.569972f)
                                    return -0.387872f;
                                else
                                    if (sensor1 <= 0.755483f)
                                        return -0.277649f;
                                    else
                                        return 0.066761f;
                        else
                            if (sensor2 <= 0.218183f)
                                if (sensor4 <= 0.828258f)
                                    if (actual_vx <= -0.558720f)
                                        return -0.526336f;
                                    else
                                        return -0.808663f;
                                else
                                    return -0.229202f;
                            else
                                if (smoothed_omega <= -0.210161f)
                                    if (sensor2 <= 0.503724f)
                                        return -0.464676f;
                                    else
                                        return -0.152615f;
                                else
                                    if (raw_action_x <= -0.765498f)
                                        return 0.054552f;
                                    else
                                        return -0.300118f;
                else
                    if (sensor3 <= 0.177668f)
                        if (smoothed_vx <= -0.743750f)
                            if (sensor5 <= 0.880196f)
                                if (actual_vx <= -0.641966f)
                                    if (raw_action_w <= 0.996944f)
                                        return -0.441113f;
                                    else
                                        return -0.733950f;
                                else
                                    if (sensor1 <= 0.365408f)
                                        return -0.854310f;
                                    else
                                        return -0.620667f;
                            else
                                if (actual_omega <= -0.070776f)
                                    if (sensor1 <= 0.341239f)
                                        return -0.601916f;
                                    else
                                        return -0.409177f;
                                else
                                    if (smoothed_omega <= 0.500102f)
                                        return -0.460707f;
                                    else
                                        return -0.185511f;
                        else
                            if (smoothed_vx <= -0.633559f)
                                if (sensor5 <= 0.897194f)
                                    if (actual_omega <= -0.048145f)
                                        return -0.330446f;
                                    else
                                        return -0.671751f;
                                else
                                    if (sensor0 <= 0.722607f)
                                        return -0.357435f;
                                    else
                                        return -0.108947f;
                            else
                                if (sensor0 <= 0.760288f)
                                    if (sensor5 <= 0.891560f)
                                        return -0.271654f;
                                    else
                                        return -0.105021f;
                                else
                                    return 0.239646f;
                    else
                        if (smoothed_vx <= -0.677674f)
                            if (sensor0 <= 0.410719f)
                                if (smoothed_omega <= -0.132824f)
                                    if (actual_vx <= -0.666034f)
                                        return -0.368961f;
                                    else
                                        return -0.710973f;
                                else
                                    if (sensor4 <= 0.788528f)
                                        return -0.427346f;
                                    else
                                        return 0.094743f;
                            else
                                if (actual_omega <= -0.068609f)
                                    if (smoothed_vx <= -0.838886f)
                                        return -0.822456f;
                                    else
                                        return -0.642532f;
                                else
                                    if (actual_omega <= 0.596652f)
                                        return -0.842943f;
                                    else
                                        return -0.964220f;
                        else
                            if (actual_omega <= 0.411114f)
                                if (sensor0 <= 0.784570f)
                                    if (sensor4 <= 0.616237f)
                                        return -0.343898f;
                                    else
                                        return -0.535146f;
                                else
                                    if (actual_vx <= -0.414841f)
                                        return -0.591139f;
                                    else
                                        return -0.799953f;
                            else
                                if (smoothed_omega <= -0.740182f)
                                    if (sensor1 <= 0.918722f)
                                        return -0.885043f;
                                    else
                                        return -0.723524f;
                                else
                                    if (sensor2 <= 0.240296f)
                                        return -0.789177f;
                                    else
                                        return -0.641864f;
        else
            if (smoothed_vx <= 0.419590f)
                if (smoothed_vx <= 0.267845f)
                    if (actual_omega <= 0.005237f)
                        if (actual_vx <= 0.005328f)
                            if (sensor0 <= 0.343489f)
                                if (raw_action_x <= -0.026063f)
                                    if (sensor2 <= 0.275982f)
                                        return 0.134595f;
                                    else
                                        return 0.240523f;
                                else
                                    if (actual_vx <= -0.019376f)
                                        return 0.102858f;
                                    else
                                        return -0.006462f;
                            else
                                if (smoothed_omega <= 0.884823f)
                                    if (sensor5 <= 0.190886f)
                                        return -0.296201f;
                                    else
                                        return -0.064036f;
                                else
                                    if (sensor0 <= 0.758146f)
                                        return 0.190344f;
                                    else
                                        return 0.485906f;
                        else
                            if (raw_action_x <= 0.031303f)
                                if (sensor0 <= 0.635122f)
                                    if (actual_omega <= -0.033540f)
                                        return 0.052831f;
                                    else
                                        return -0.036795f;
                                else
                                    if (sensor4 <= 0.845827f)
                                        return 0.121691f;
                                    else
                                        return -0.814578f;
                            else
                                if (sensor5 <= 0.370403f)
                                    if (actual_vx <= 0.030603f)
                                        return -0.109855f;
                                    else
                                        return -0.221724f;
                                else
                                    if (sensor0 <= 0.360701f)
                                        return 0.001499f;
                                    else
                                        return 0.176859f;
                    else
                        if (actual_vx <= -0.010405f)
                            if (sensor4 <= 0.843008f)
                                if (actual_omega <= 0.469764f)
                                    if (sensor2 <= 0.528521f)
                                        return -0.148048f;
                                    else
                                        return 0.202827f;
                                else
                                    if (sensor2 <= 0.411043f)
                                        return -0.614516f;
                                    else
                                        return -0.267449f;
                            else
                                if (raw_action_x <= -0.043731f)
                                    if (sensor0 <= 0.396536f)
                                        return 0.127172f;
                                    else
                                        return -0.327133f;
                                else
                                    if (actual_vx <= -0.044058f)
                                        return 0.075193f;
                                    else
                                        return -0.057836f;
                        else
                            if (sensor2 <= 0.283348f)
                                if (raw_action_x <= -0.014921f)
                                    if (sensor0 <= 0.372463f)
                                        return -0.073497f;
                                    else
                                        return -0.699040f;
                                else
                                    if (raw_action_x <= 0.283034f)
                                        return -0.189751f;
                                    else
                                        return -0.284048f;
                            else
                                if (sensor3 <= 0.308285f)
                                    if (sensor2 <= 0.311488f)
                                        return 0.092022f;
                                    else
                                        return 0.334931f;
                                else
                                    if (smoothed_vx <= -0.182883f)
                                        return -0.331690f;
                                    else
                                        return -0.014932f;
                else
                    if (sensor0 <= 0.453828f)
                        if (sensor2 <= 0.304895f)
                            if (sensor5 <= 0.444094f)
                                if (actual_vx <= -0.141672f)
                                    if (actual_vx <= -0.457132f)
                                        return 0.651472f;
                                    else
                                        return 0.267282f;
                                else
                                    if (sensor4 <= 0.799585f)
                                        return 0.135019f;
                                    else
                                        return -0.133799f;
                            else
                                if (sensor4 <= 0.659522f)
                                    if (sensor0 <= 0.371667f)
                                        return 0.603297f;
                                    else
                                        return 0.311404f;
                                else
                                    if (sensor2 <= 0.279064f)
                                        return 0.098126f;
                                    else
                                        return 0.261771f;
                        else
                            if (sensor3 <= 0.716097f)
                                if (sensor0 <= 0.240037f)
                                    if (raw_action_w <= -0.491467f)
                                        return 0.551383f;
                                    else
                                        return 0.904018f;
                                else
                                    if (sensor2 <= 0.334481f)
                                        return 0.216338f;
                                    else
                                        return 0.353625f;
                            else
                                if (actual_vx <= 0.436089f)
                                    if (smoothed_vx <= 0.366287f)
                                        return 0.147166f;
                                    else
                                        return -0.043997f;
                                else
                                    if (raw_action_w <= -0.641895f)
                                        return 0.018888f;
                                    else
                                        return -0.182446f;
                    else
                        if (sensor4 <= 0.857988f)
                            if (smoothed_omega <= -0.606261f)
                                if (sensor2 <= 0.565603f)
                                    if (actual_vx <= -0.127127f)
                                        return 0.195309f;
                                    else
                                        return -0.291466f;
                                else
                                    if (actual_vz <= 0.334805f)
                                        return 0.207869f;
                                    else
                                        return 0.423652f;
                            else
                                if (sensor2 <= 0.765676f)
                                    if (sensor5 <= 0.742077f)
                                        return 0.473436f;
                                    else
                                        return 0.619589f;
                                else
                                    if (raw_action_w <= -0.507922f)
                                        return 0.535484f;
                                    else
                                        return 0.743122f;
                        else
                            if (sensor2 <= 0.361624f)
                                if (sensor3 <= 0.292175f)
                                    if (actual_omega <= 0.125281f)
                                        return -0.396845f;
                                    else
                                        return 0.074849f;
                                else
                                    if (actual_vz <= 1.076465f)
                                        return 0.372319f;
                                    else
                                        return 0.023236f;
                            else
                                if (sensor3 <= 0.368721f)
                                    if (actual_omega <= 0.139872f)
                                        return 0.409747f;
                                    else
                                        return 0.590315f;
                                else
                                    if (sensor3 <= 0.658553f)
                                        return 0.202256f;
                                    else
                                        return -0.198160f;
            else
                if (smoothed_vx <= 0.818343f)
                    if (smoothed_vx <= 0.614426f)
                        if (sensor4 <= 0.847403f)
                            if (sensor1 <= 0.445885f)
                                if (raw_action_x <= 0.231084f)
                                    if (sensor5 <= 0.334726f)
                                        return 0.640809f;
                                    else
                                        return 0.900844f;
                                else
                                    if (actual_vx <= -0.144405f)
                                        return 0.575689f;
                                    else
                                        return 0.720093f;
                            else
                                if (sensor3 <= 0.241915f)
                                    if (sensor5 <= 0.543999f)
                                        return 0.283174f;
                                    else
                                        return 0.530052f;
                                else
                                    if (sensor2 <= 0.802130f)
                                        return 0.541155f;
                                    else
                                        return 0.675516f;
                        else
                            if (sensor5 <= 0.317242f)
                                if (smoothed_omega <= -0.324154f)
                                    if (sensor0 <= 0.204059f)
                                        return 0.880879f;
                                    else
                                        return 0.606039f;
                                else
                                    if (actual_omega <= 0.138117f)
                                        return 0.356569f;
                                    else
                                        return 0.534057f;
                            else
                                if (sensor5 <= 0.558477f)
                                    if (sensor1 <= 0.860070f)
                                        return 0.160899f;
                                    else
                                        return 0.386232f;
                                else
                                    if (sensor3 <= 0.717755f)
                                        return 0.517368f;
                                    else
                                        return 0.827776f;
                    else
                        if (sensor4 <= 0.875150f)
                            if (sensor3 <= 0.649575f)
                                if (sensor3 <= 0.241709f)
                                    if (sensor2 <= 0.581526f)
                                        return 0.697559f;
                                    else
                                        return 0.876600f;
                                else
                                    if (sensor2 <= 0.823810f)
                                        return 0.617282f;
                                    else
                                        return 0.780092f;
                            else
                                if (sensor5 <= 0.499270f)
                                    return 0.713054f;
                                else
                                    if (actual_vx <= 0.581313f)
                                        return 0.774808f;
                                    else
                                        return 0.944974f;
                        else
                            if (sensor5 <= 0.574589f)
                                if (actual_vx <= -0.269929f)
                                    if (sensor0 <= 0.237312f)
                                        return 0.960631f;
                                    else
                                        return 0.787707f;
                                else
                                    if (actual_vz <= 0.453019f)
                                        return 0.551400f;
                                    else
                                        return 0.331222f;
                            else
                                if (actual_omega <= -0.241680f)
                                    if (actual_omega <= -0.863083f)
                                        return 0.827540f;
                                    else
                                        return 0.962237f;
                                else
                                    if (sensor0 <= 0.291907f)
                                        return 0.779869f;
                                    else
                                        return 0.534706f;
                else
                    if (smoothed_vx <= 0.929242f)
                        if (sensor3 <= 0.209573f)
                            if (actual_vz <= 0.495733f)
                                if (sensor3 <= 0.185723f)
                                    if (sensor2 <= 0.155406f)
                                        return 0.828814f;
                                    else
                                        return 0.976963f;
                                else
                                    if (sensor2 <= 0.623784f)
                                        return 0.699141f;
                                    else
                                        return 0.932553f;
                            else
                                if (sensor2 <= 0.068645f)
                                    if (actual_vx <= 0.422708f)
                                        return 0.720823f;
                                    else
                                        return 0.395177f;
                                else
                                    if (raw_action_x <= 0.998321f)
                                        return 0.870479f;
                                    else
                                        return 0.728691f;
                        else
                            if (actual_omega <= -0.699307f)
                                if (sensor1 <= 0.701877f)
                                    if (sensor1 <= 0.608434f)
                                        return 0.604151f;
                                    else
                                        return 0.522439f;
                                else
                                    if (sensor5 <= 0.619117f)
                                        return 0.935001f;
                                    else
                                        return 0.986200f;
                            else
                                if (sensor3 <= 0.433854f)
                                    if (sensor2 <= 0.214356f)
                                        return 0.294808f;
                                    else
                                        return 0.807439f;
                                else
                                    if (sensor5 <= 0.589970f)
                                        return 0.575337f;
                                    else
                                        return 0.793799f;
                    else
                        if (sensor5 <= 0.698512f)
                            if (sensor3 <= 0.208783f)
                                if (smoothed_omega <= -0.005467f)
                                    if (sensor5 <= 0.396761f)
                                        return 0.940904f;
                                    else
                                        return 0.998861f;
                                else
                                    return 0.826211f;
                            else
                                if (sensor5 <= 0.641007f)
                                    if (sensor4 <= 0.993643f)
                                        return 0.845811f;
                                    else
                                        return 0.670521f;
                                else
                                    if (sensor0 <= 0.508033f)
                                        return 0.948916f;
                                    else
                                        return 0.843492f;
                        else
                            if (sensor0 <= 0.538994f)
                                if (raw_action_x <= 0.855194f)
                                    if (raw_action_w <= 0.559145f)
                                        return 1.000000f;
                                    else
                                        return 0.756721f;
                                else
                                    if (sensor5 <= 0.760659f)
                                        return 0.989669f;
                                    else
                                        return 0.999145f;
                            else
                                if (sensor3 <= 0.220200f)
                                    if (sensor2 <= 0.223773f)
                                        return 0.935167f;
                                    else
                                        return 0.989326f;
                                else
                                    if (sensor1 <= 0.804805f)
                                        return 0.821061f;
                                    else
                                        return 0.930388f;
    }
    
    // 预测角速度 action_w
    public float PredictActionW()
    {
        if (smoothed_omega <= -0.392944f)
            if (smoothed_omega <= -0.663229f)
                if (smoothed_omega <= -0.827579f)
                    if (actual_omega <= 0.604592f)
                        if (smoothed_omega <= -0.918759f)
                            if (sensor5 <= 0.196761f)
                                if (sensor3 <= 0.616423f)
                                    if (sensor4 <= 0.764511f)
                                        return -0.532624f;
                                    else
                                        return -0.924150f;
                                else
                                    if (smoothed_vx <= -0.367072f)
                                        return -0.890048f;
                                    else
                                        return -0.971073f;
                            else
                                if (sensor2 <= 0.643321f)
                                    if (raw_action_x <= -0.630608f)
                                        return -0.744640f;
                                    else
                                        return -0.865818f;
                                else
                                    if (smoothed_omega <= -0.976789f)
                                        return -0.954878f;
                                    else
                                        return -0.913085f;
                        else
                            if (sensor3 <= 0.767753f)
                                if (sensor0 <= 0.199544f)
                                    if (sensor2 <= 0.547707f)
                                        return -0.736849f;
                                    else
                                        return -0.907835f;
                                else
                                    if (sensor3 <= 0.090882f)
                                        return -0.627405f;
                                    else
                                        return -0.830186f;
                            else
                                if (sensor5 <= 0.219185f)
                                    if (sensor0 <= 0.463856f)
                                        return -0.965268f;
                                    else
                                        return -0.825907f;
                                else
                                    if (raw_action_w <= -0.879631f)
                                        return -0.861314f;
                                    else
                                        return -0.976262f;
                    else
                        if (sensor0 <= 0.207557f)
                            if (sensor0 <= 0.166784f)
                                if (smoothed_vx <= 0.998663f)
                                    if (sensor3 <= 0.041237f)
                                        return -0.958448f;
                                    else
                                        return -0.997404f;
                                else
                                    if (raw_action_w <= -0.908453f)
                                        return -0.901540f;
                                    else
                                        return -0.975478f;
                            else
                                if (raw_action_w <= -0.995634f)
                                    if (actual_omega <= 0.757989f)
                                        return -0.809313f;
                                    else
                                        return -0.612754f;
                                else
                                    if (actual_omega <= 0.741152f)
                                        return -0.910042f;
                                    else
                                        return -0.823152f;
                        else
                            if (smoothed_vx <= 0.999972f)
                                if (sensor2 <= 0.055611f)
                                    if (sensor4 <= 0.872433f)
                                        return -0.423478f;
                                    else
                                        return -0.646070f;
                                else
                                    if (sensor0 <= 0.618114f)
                                        return -0.704572f;
                                    else
                                        return -0.772193f;
                            else
                                if (raw_action_w <= -0.768134f)
                                    if (sensor0 <= 0.263585f)
                                        return -0.620348f;
                                    else
                                        return -0.386206f;
                                else
                                    return -0.806884f;
                else
                    if (sensor5 <= 0.748121f)
                        if (sensor1 <= 0.521520f)
                            if (raw_action_w <= -0.824130f)
                                if (sensor0 <= 0.365836f)
                                    if (sensor1 <= 0.507297f)
                                        return -0.838221f;
                                    else
                                        return -0.944275f;
                                else
                                    return -0.497789f;
                            else
                                if (actual_omega <= 0.512026f)
                                    if (raw_action_w <= -0.649678f)
                                        return -0.917120f;
                                    else
                                        return -0.965312f;
                                else
                                    return -0.672827f;
                        else
                            if (sensor2 <= 0.447416f)
                                if (actual_vx <= -0.225416f)
                                    if (smoothed_omega <= -0.696647f)
                                        return -0.764888f;
                                    else
                                        return -0.659802f;
                                else
                                    if (sensor1 <= 0.944248f)
                                        return -0.553420f;
                                    else
                                        return -0.814919f;
                            else
                                if (sensor5 <= 0.278328f)
                                    if (sensor2 <= 0.506926f)
                                        return -0.811819f;
                                    else
                                        return -0.905583f;
                                else
                                    if (raw_action_w <= -0.755998f)
                                        return -0.723755f;
                                    else
                                        return -0.802898f;
                    else
                        if (sensor1 <= 0.717961f)
                            if (sensor2 <= 0.038416f)
                                if (sensor2 <= 0.023702f)
                                    if (sensor0 <= 0.690293f)
                                        return -0.685688f;
                                    else
                                        return -0.458445f;
                                else
                                    if (sensor5 <= 0.869723f)
                                        return -0.450785f;
                                    else
                                        return -0.654551f;
                            else
                                if (sensor1 <= 0.690562f)
                                    if (actual_vx <= -0.920468f)
                                        return -0.655402f;
                                    else
                                        return -0.798296f;
                                else
                                    if (raw_action_w <= -0.748246f)
                                        return -0.562081f;
                                    else
                                        return -0.746595f;
                        else
                            if (sensor2 <= 0.843220f)
                                if (smoothed_omega <= -0.773940f)
                                    if (sensor3 <= 0.025269f)
                                        return -0.653894f;
                                    else
                                        return -0.317928f;
                                else
                                    if (sensor2 <= 0.601587f)
                                        return -0.429975f;
                                    else
                                        return -0.546640f;
                            else
                                if (smoothed_omega <= -0.736418f)
                                    if (sensor0 <= 0.364094f)
                                        return -0.892705f;
                                    else
                                        return -0.576948f;
                                else
                                    if (actual_vz <= 0.393620f)
                                        return -0.701980f;
                                    else
                                        return -0.832014f;
            else
                if (smoothed_omega <= -0.483850f)
                    if (sensor2 <= 0.479708f)
                        if (sensor3 <= 0.482650f)
                            if (raw_action_w <= -0.623184f)
                                if (sensor1 <= 0.992416f)
                                    if (sensor1 <= 0.755230f)
                                        return -0.293171f;
                                    else
                                        return -0.091878f;
                                else
                                    if (actual_vx <= -0.152343f)
                                        return -0.636563f;
                                    else
                                        return -0.319735f;
                            else
                                if (sensor0 <= 0.256081f)
                                    if (sensor4 <= 0.921470f)
                                        return -0.768393f;
                                    else
                                        return -0.336000f;
                                else
                                    if (raw_action_x <= -0.327624f)
                                        return 0.154706f;
                                    else
                                        return -0.320775f;
                        else
                            if (smoothed_vx <= -0.998093f)
                                if (sensor0 <= 0.717562f)
                                    if (smoothed_omega <= -0.488576f)
                                        return -0.437060f;
                                    else
                                        return -0.716329f;
                                else
                                    if (sensor5 <= 0.914373f)
                                        return -0.239135f;
                                    else
                                        return -0.414139f;
                            else
                                if (sensor1 <= 0.819269f)
                                    if (actual_omega <= -0.009857f)
                                        return -0.863685f;
                                    else
                                        return -0.638095f;
                                else
                                    if (actual_omega <= 0.235960f)
                                        return -0.420776f;
                                    else
                                        return -0.597584f;
                    else
                        if (smoothed_vx <= 0.999994f)
                            if (sensor0 <= 0.538987f)
                                if (actual_vx <= 0.751330f)
                                    if (sensor1 <= 0.579318f)
                                        return -0.909484f;
                                    else
                                        return -0.774491f;
                                else
                                    if (sensor1 <= 0.737583f)
                                        return -0.671684f;
                                    else
                                        return -0.369815f;
                            else
                                if (sensor2 <= 0.834644f)
                                    if (smoothed_omega <= -0.567001f)
                                        return -0.612548f;
                                    else
                                        return -0.362406f;
                                else
                                    if (actual_vz <= 0.965311f)
                                        return -0.809835f;
                                    else
                                        return -0.623162f;
                        else
                            if (raw_action_w <= -0.426509f)
                                if (sensor1 <= 0.759429f)
                                    if (actual_vz <= 0.919804f)
                                        return -0.514982f;
                                    else
                                        return -0.366594f;
                                else
                                    if (sensor2 <= 0.619736f)
                                        return -0.262442f;
                                    else
                                        return -0.421670f;
                            else
                                if (sensor2 <= 0.578543f)
                                    if (raw_action_w <= -0.345874f)
                                        return -0.619048f;
                                    else
                                        return -0.367252f;
                                else
                                    if (sensor1 <= 0.770292f)
                                        return -0.748818f;
                                    else
                                        return -0.558764f;
                else
                    if (sensor2 <= 0.472126f)
                        if (sensor3 <= 0.343341f)
                            if (sensor2 <= 0.374712f)
                                if (sensor4 <= 0.799869f)
                                    if (actual_vz <= 0.411048f)
                                        return -0.483243f;
                                    else
                                        return -0.050796f;
                                else
                                    if (sensor2 <= 0.319183f)
                                        return 0.398125f;
                                    else
                                        return 0.064689f;
                            else
                                if (raw_action_w <= -0.299870f)
                                    if (sensor2 <= 0.465605f)
                                        return -0.230879f;
                                    else
                                        return 0.043824f;
                                else
                                    if (sensor5 <= 0.824129f)
                                        return -0.523153f;
                                    else
                                        return -0.191197f;
                        else
                            if (actual_omega <= 0.059931f)
                                if (actual_vx <= -0.511333f)
                                    if (sensor5 <= 0.185778f)
                                        return -0.910853f;
                                    else
                                        return -0.717589f;
                                else
                                    if (raw_action_w <= -0.495784f)
                                        return -0.393941f;
                                    else
                                        return -0.641754f;
                            else
                                if (raw_action_w <= -0.310856f)
                                    if (sensor0 <= 0.350359f)
                                        return -0.063782f;
                                    else
                                        return -0.312267f;
                                else
                                    if (sensor5 <= 0.211016f)
                                        return -0.268385f;
                                    else
                                        return -0.495110f;
                    else
                        if (sensor0 <= 0.531208f)
                            if (smoothed_vx <= 0.999999f)
                                if (sensor5 <= 0.393659f)
                                    if (sensor0 <= 0.524708f)
                                        return -0.824874f;
                                    else
                                        return -0.551753f;
                                else
                                    if (raw_action_w <= -0.252774f)
                                        return -0.530142f;
                                    else
                                        return -0.768313f;
                            else
                                if (sensor3 <= 0.283595f)
                                    if (actual_vz <= 0.739965f)
                                        return -0.346124f;
                                    else
                                        return -0.560584f;
                                else
                                    if (sensor5 <= 0.712438f)
                                        return -0.042624f;
                                    else
                                        return -0.250271f;
                        else
                            if (sensor0 <= 0.826566f)
                                if (sensor1 <= 0.685539f)
                                    if (smoothed_omega <= -0.409329f)
                                        return -0.458496f;
                                    else
                                        return -0.778234f;
                                else
                                    if (sensor3 <= 0.351149f)
                                        return -0.261708f;
                                    else
                                        return -0.502706f;
                            else
                                if (sensor1 <= 0.960189f)
                                    if (sensor2 <= 0.830624f)
                                        return -0.392638f;
                                    else
                                        return -0.618422f;
                                else
                                    if (sensor1 <= 0.979689f)
                                        return -0.814219f;
                                    else
                                        return -0.507516f;
        else
            if (smoothed_omega <= 0.470680f)
                if (actual_omega <= -0.003918f)
                    if (sensor0 <= 0.334567f)
                        if (raw_action_x <= 0.266694f)
                            if (actual_omega <= -0.052308f)
                                if (sensor2 <= 0.280094f)
                                    if (sensor2 <= 0.172560f)
                                        return -0.023261f;
                                    else
                                        return -0.367457f;
                                else
                                    if (raw_action_w <= 0.382519f)
                                        return -0.566986f;
                                    else
                                        return -0.399014f;
                            else
                                if (sensor2 <= 0.277918f)
                                    if (smoothed_omega <= -0.008726f)
                                        return -0.039150f;
                                    else
                                        return -0.179639f;
                                else
                                    if (smoothed_omega <= 0.012719f)
                                        return -0.193827f;
                                    else
                                        return -0.356422f;
                        else
                            if (sensor5 <= 0.869805f)
                                if (sensor2 <= 0.276702f)
                                    if (sensor4 <= 0.941327f)
                                        return -0.021816f;
                                    else
                                        return 0.220248f;
                                else
                                    if (smoothed_omega <= 0.257831f)
                                        return -0.161582f;
                                    else
                                        return 0.112949f;
                            else
                                if (sensor0 <= 0.187644f)
                                    if (sensor1 <= 0.731973f)
                                        return 0.258727f;
                                    else
                                        return 0.089231f;
                                else
                                    if (actual_vx <= 0.179431f)
                                        return -0.122112f;
                                    else
                                        return 0.493937f;
                    else
                        if (raw_action_w <= 0.055323f)
                            if (smoothed_omega <= -0.177634f)
                                if (sensor3 <= 0.357287f)
                                    if (sensor2 <= 0.320803f)
                                        return 0.003975f;
                                    else
                                        return -0.302300f;
                                else
                                    if (sensor0 <= 0.505784f)
                                        return -0.678322f;
                                    else
                                        return -0.379729f;
                            else
                                if (sensor1 <= 0.659987f)
                                    if (sensor3 <= 0.237694f)
                                        return 0.695356f;
                                    else
                                        return 0.180998f;
                                else
                                    if (sensor3 <= 0.473745f)
                                        return -0.063227f;
                                    else
                                        return -0.306977f;
                        else
                            if (sensor4 <= 0.832500f)
                                if (raw_action_w <= 0.540700f)
                                    if (raw_action_w <= 0.316245f)
                                        return 0.118966f;
                                    else
                                        return 0.251611f;
                                else
                                    if (sensor3 <= 0.398342f)
                                        return 0.595978f;
                                    else
                                        return 0.201011f;
                            else
                                if (sensor0 <= 0.337893f)
                                    if (smoothed_omega <= 0.133284f)
                                        return 0.000297f;
                                    else
                                        return -0.153701f;
                                else
                                    if (raw_action_w <= 0.682579f)
                                        return 0.004394f;
                                    else
                                        return 0.144413f;
                else
                    if (sensor2 <= 0.309275f)
                        if (sensor1 <= 0.764201f)
                            if (sensor0 <= 0.262383f)
                                if (actual_omega <= 0.179712f)
                                    if (sensor2 <= 0.181191f)
                                        return -0.053052f;
                                    else
                                        return -0.236101f;
                                else
                                    if (raw_action_x <= 0.235947f)
                                        return -0.026067f;
                                    else
                                        return 0.199030f;
                            else
                                if (sensor3 <= 0.240154f)
                                    if (sensor2 <= 0.151877f)
                                        return 0.541399f;
                                    else
                                        return 0.122753f;
                                else
                                    if (smoothed_omega <= -0.202075f)
                                        return -0.145289f;
                                    else
                                        return 0.016717f;
                        else
                            if (actual_omega <= 0.040837f)
                                if (sensor2 <= 0.275851f)
                                    if (smoothed_omega <= -0.015320f)
                                        return 0.251146f;
                                    else
                                        return 0.091116f;
                                else
                                    if (smoothed_omega <= -0.078717f)
                                        return 0.108114f;
                                    else
                                        return -0.048949f;
                            else
                                if (actual_vx <= -0.123688f)
                                    if (smoothed_vx <= -0.303956f)
                                        return -0.240143f;
                                    else
                                        return 0.111540f;
                                else
                                    if (sensor2 <= 0.273027f)
                                        return 0.477241f;
                                    else
                                        return 0.272803f;
                    else
                        if (sensor2 <= 0.718441f)
                            if (sensor4 <= 0.936849f)
                                if (sensor3 <= 0.632149f)
                                    if (actual_omega <= 0.079495f)
                                        return -0.213740f;
                                    else
                                        return -0.094446f;
                                else
                                    if (sensor5 <= 0.553017f)
                                        return -0.498150f;
                                    else
                                        return 0.251588f;
                            else
                                if (sensor1 <= 0.759634f)
                                    if (sensor0 <= 0.371708f)
                                        return -0.682089f;
                                    else
                                        return -0.261773f;
                                else
                                    if (raw_action_x <= 0.556762f)
                                        return 0.003354f;
                                    else
                                        return 0.313827f;
                        else
                            if (sensor1 <= 0.691412f)
                                if (sensor1 <= 0.655866f)
                                    if (raw_action_w <= -0.288949f)
                                        return -0.941004f;
                                    else
                                        return -0.743916f;
                                else
                                    if (smoothed_omega <= -0.297452f)
                                        return -0.574249f;
                                    else
                                        return -0.754893f;
                            else
                                if (actual_vz <= 0.990433f)
                                    if (actual_omega <= 0.280272f)
                                        return -0.514012f;
                                    else
                                        return -0.286312f;
                                else
                                    if (sensor3 <= 0.330003f)
                                        return -0.221672f;
                                    else
                                        return -0.370053f;
            else
                if (raw_action_w <= 0.994586f)
                    if (smoothed_omega <= 0.638531f)
                        if (sensor1 <= 0.527209f)
                            if (sensor2 <= 0.133624f)
                                if (raw_action_w <= 0.331299f)
                                    return 0.897863f;
                                else
                                    if (actual_omega <= -0.038192f)
                                        return 0.997347f;
                                    else
                                        return 0.963674f;
                            else
                                if (sensor1 <= 0.487232f)
                                    if (smoothed_vx <= -0.988941f)
                                        return 0.725479f;
                                    else
                                        return 0.537209f;
                                else
                                    if (actual_omega <= -0.523807f)
                                        return 0.558814f;
                                    else
                                        return 0.808807f;
                        else
                            if (sensor3 <= 0.976959f)
                                if (raw_action_w <= 0.506252f)
                                    if (sensor3 <= 0.638250f)
                                        return 0.106567f;
                                    else
                                        return 0.340574f;
                                else
                                    if (sensor3 <= 0.355534f)
                                        return 0.588029f;
                                    else
                                        return 0.359749f;
                            else
                                if (sensor0 <= 0.223550f)
                                    if (actual_vz <= 0.810780f)
                                        return 0.557697f;
                                    else
                                        return 0.319079f;
                                else
                                    if (actual_omega <= -0.588977f)
                                        return 0.607676f;
                                    else
                                        return 0.758308f;
                    else
                        if (sensor0 <= 0.380230f)
                            if (sensor0 <= 0.221007f)
                                if (sensor0 <= 0.197850f)
                                    return -0.055950f;
                                else
                                    if (actual_omega <= -0.629303f)
                                        return 0.233425f;
                                    else
                                        return 0.414006f;
                            else
                                if (raw_action_x <= 0.625347f)
                                    if (sensor2 <= 0.154176f)
                                        return 0.693119f;
                                    else
                                        return 0.104941f;
                                else
                                    if (sensor2 <= 0.256107f)
                                        return 0.684391f;
                                    else
                                        return 0.559988f;
                        else
                            if (sensor2 <= 0.440665f)
                                if (sensor4 <= 0.580796f)
                                    if (sensor0 <= 0.741829f)
                                        return 0.714765f;
                                    else
                                        return 0.508232f;
                                else
                                    if (sensor1 <= 0.889919f)
                                        return 0.833530f;
                                    else
                                        return 0.349273f;
                            else
                                if (smoothed_omega <= 0.718542f)
                                    if (sensor5 <= 0.722167f)
                                        return -0.040559f;
                                    else
                                        return 0.243444f;
                                else
                                    if (sensor2 <= 0.469765f)
                                        return 0.623825f;
                                    else
                                        return 0.355359f;
                else
                    if (sensor3 <= 0.598952f)
                        if (sensor2 <= 0.228449f)
                            if (sensor2 <= 0.107510f)
                                if (sensor5 <= 0.594280f)
                                    if (sensor4 <= 0.777318f)
                                        return 0.960173f;
                                    else
                                        return 0.722034f;
                                else
                                    if (sensor4 <= 0.742794f)
                                        return 0.999912f;
                                    else
                                        return 0.988208f;
                            else
                                if (sensor4 <= 0.925074f)
                                    if (sensor0 <= 0.306294f)
                                        return 0.592376f;
                                    else
                                        return 0.954359f;
                                else
                                    return 0.544148f;
                        else
                            if (sensor0 <= 0.557164f)
                                if (raw_action_x <= 0.451789f)
                                    if (actual_vz <= 0.543395f)
                                        return 0.091707f;
                                    else
                                        return 0.639228f;
                                else
                                    if (sensor2 <= 0.475166f)
                                        return 0.868111f;
                                    else
                                        return 0.452018f;
                            else
                                if (sensor5 <= 0.309365f)
                                    if (sensor3 <= 0.291166f)
                                        return 0.497445f;
                                    else
                                        return 0.800450f;
                                else
                                    if (sensor1 <= 0.907845f)
                                        return 0.973499f;
                                    else
                                        return 0.816325f;
                    else
                        if (sensor3 <= 0.922046f)
                            if (smoothed_vx <= -0.994023f)
                                if (sensor1 <= 0.422552f)
                                    if (smoothed_vx <= -0.999998f)
                                        return 0.839475f;
                                    else
                                        return 0.692401f;
                                else
                                    if (actual_omega <= -0.705364f)
                                        return 0.449835f;
                                    else
                                        return 0.665973f;
                            else
                                if (sensor4 <= 0.940022f)
                                    if (sensor3 <= 0.721533f)
                                        return 0.867118f;
                                    else
                                        return 0.762553f;
                                else
                                    if (smoothed_vx <= 0.991100f)
                                        return 0.602962f;
                                    else
                                        return 0.350650f;
                        else
                            if (sensor2 <= 0.319062f)
                                if (actual_vz <= 0.808125f)
                                    if (sensor5 <= 0.663386f)
                                        return 0.274746f;
                                    else
                                        return 0.533807f;
                                else
                                    if (actual_omega <= -0.594819f)
                                        return 0.620053f;
                                    else
                                        return 0.877192f;
                            else
                                if (smoothed_omega <= 0.708842f)
                                    return 0.492950f;
                                else
                                    return 0.063270f;
    }
    
    // 同时预测两个动作
    public (float action_x, float action_w) Predict()
    {
        return (PredictActionX(), PredictActionW());
    }
}
