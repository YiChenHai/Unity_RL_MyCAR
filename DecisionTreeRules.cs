// 决策树规则库 - 自动生成（离散动作分类）
// 数据样本数: 300000
// 准确率: 0.8032
// 树深度: 8
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
        //return 5;
        if (sensor1 <= 0.798045f)
            if (actual_vz <= 1.082263f)
                if (actual_vz <= 0.947619f)
                    if (sensor2 <= 0.302478f)
                        if (sensor3 <= 0.773299f)
                            if (actual_vz <= 0.922181f)
                                if (sensor5 <= 0.573114f)
                                    if (sensor0 <= 0.279750f)
                                        return 9;
                                    else
                                        return 5;
                                else
                                    if (sensor4 <= 0.744904f)
                                        return 5;
                                    else
                                        return 0;
                            else
                                if (sensor5 <= 0.431052f)
                                    if (sensor3 <= 0.663854f)
                                        return 5;
                                    else
                                        return 5;
                                else
                                    if (sensor5 <= 0.772016f)
                                        return 4;
                                    else
                                        return 0;
                        else
                            if (sensor4 <= 0.405063f)
                                if (last_action_state <= 0.150000f)
                                    if (actual_vx <= 0.010834f)
                                        return 6;
                                    else
                                        return 5;
                                else
                                    if (actual_vz <= 0.940531f)
                                        return 5;
                                    else
                                        return 6;
                            else
                                if (sensor5 <= 0.046108f)
                                    if (actual_omega <= -0.199461f)
                                        return 6;
                                    else
                                        return 5;
                                else
                                    if (last_action_state <= 0.350000f)
                                        return 6;
                                    else
                                        return 5;
                    else
                        if (last_action_state <= 0.550000f)
                            if (last_action_state <= 0.450000f)
                                if (actual_omega <= 0.177778f)
                                    if (actual_vx <= -0.010608f)
                                        return 9;
                                    else
                                        return 10;
                                else
                                    if (sensor1 <= 0.648503f)
                                        return 10;
                                    else
                                        return 9;
                            else
                                if (actual_omega <= 0.168213f)
                                    if (actual_vx <= -0.001666f)
                                        return 10;
                                    else
                                        return 10;
                                else
                                    if (sensor2 <= 0.836329f)
                                        return 10;
                                    else
                                        return 10;
                        else
                            if (sensor5 <= 0.754062f)
                                if (sensor5 <= 0.241255f)
                                    if (actual_vx <= -0.013590f)
                                        return 9;
                                    else
                                        return 10;
                                else
                                    if (last_action_state <= 0.750000f)
                                        return 6;
                                    else
                                        return 6;
                            else
                                if (last_action_state <= 0.850000f)
                                    if (last_action_state <= 0.650000f)
                                        return 10;
                                    else
                                        return 7;
                                else
                                    if (last_action_state <= 0.950000f)
                                        return 5;
                                    else
                                        return 5;
                else
                    if (sensor3 <= 0.798018f)
                        if (actual_vx <= -0.008311f)
                            if (sensor0 <= 0.112355f)
                                if (last_action_state <= 0.750000f)
                                    if (last_action_state <= 0.550000f)
                                        return 10;
                                    else
                                        return 6;
                                else
                                    if (sensor4 <= 0.670381f)
                                        return 5;
                                    else
                                        return 5;
                            else
                                if (last_action_state <= 0.750000f)
                                    if (sensor1 <= 0.741024f)
                                        return 6;
                                    else
                                        return 6;
                                else
                                    if (sensor5 <= 0.587902f)
                                        return 6;
                                    else
                                        return 5;
                        else
                            if (sensor3 <= 0.578823f)
                                if (last_action_state <= 0.750000f)
                                    if (sensor0 <= 0.294329f)
                                        return 10;
                                    else
                                        return 5;
                                else
                                    if (sensor5 <= 0.600861f)
                                        return 6;
                                    else
                                        return 5;
                            else
                                if (actual_vz <= 1.028890f)
                                    if (actual_vx <= 0.007245f)
                                        return 5;
                                    else
                                        return 5;
                                else
                                    if (sensor0 <= 0.770953f)
                                        return 1;
                                    else
                                        return 1;
                    else
                        if (actual_vz <= 1.031266f)
                            if (actual_vx <= 0.007164f)
                                if (last_action_state <= 0.250000f)
                                    if (last_action_state <= 0.150000f)
                                        return 6;
                                    else
                                        return 6;
                                else
                                    if (last_action_state <= 0.350000f)
                                        return 5;
                                    else
                                        return 5;
                            else
                                if (sensor4 <= 0.431375f)
                                    if (last_action_state <= 0.150000f)
                                        return 6;
                                    else
                                        return 5;
                                else
                                    if (sensor0 <= 0.837005f)
                                        return 5;
                                    else
                                        return 5;
                        else
                            if (sensor0 <= 0.786662f)
                                if (sensor3 <= 0.830462f)
                                    return 1;
                                else
                                    return 2;
                            else
                                if (actual_vz <= 1.075370f)
                                    if (actual_vz <= 1.073600f)
                                        return 1;
                                    else
                                        return 2;
                                else
                                    if (sensor5 <= 0.013029f)
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
                                        return 5;
                                    else
                                        return 4;
                                else
                                    if (sensor1 <= 0.293720f)
                                        return 8;
                                    else
                                        return 10;
                            else
                                if (sensor5 <= 0.346598f)
                                    if (actual_vx <= -0.010904f)
                                        return 6;
                                    else
                                        return 5;
                                else
                                    if (sensor4 <= 0.516695f)
                                        return 3;
                                    else
                                        return 6;
                        else
                            if (sensor5 <= 0.627182f)
                                if (sensor5 <= 0.570228f)
                                    if (sensor2 <= 0.678179f)
                                        return 5;
                                    else
                                        return 6;
                                else
                                    if (sensor5 <= 0.607580f)
                                        return 5;
                                    else
                                        return 6;
                            else
                                if (actual_omega <= 0.147509f)
                                    if (sensor3 <= 0.032388f)
                                        return 5;
                                    else
                                        return 5;
                                else
                                    if (sensor3 <= 0.068883f)
                                        return 5;
                                    else
                                        return 5;
                    else
                        if (actual_omega <= -0.109020f)
                            if (actual_omega <= -0.158707f)
                                if (sensor5 <= 0.294028f)
                                    if (sensor2 <= 0.125728f)
                                        return 4;
                                    else
                                        return 5;
                                else
                                    if (sensor5 <= 0.383134f)
                                        return 4;
                                    else
                                        return 2;
                            else
                                if (actual_vx <= 0.000900f)
                                    if (sensor0 <= 0.492852f)
                                        return 5;
                                    else
                                        return 1;
                                else
                                    if (last_action_state <= 0.550000f)
                                        return 5;
                                    else
                                        return 1;
                        else
                            if (actual_vx <= -0.000494f)
                                if (sensor4 <= 0.820237f)
                                    if (sensor0 <= 0.535854f)
                                        return 1;
                                    else
                                        return 1;
                                else
                                    if (sensor1 <= 0.781490f)
                                        return 1;
                                    else
                                        return 1;
                            else
                                if (sensor2 <= 0.142471f)
                                    if (actual_vx <= 0.001227f)
                                        return 1;
                                    else
                                        return 1;
                                else
                                    if (sensor0 <= 0.355552f)
                                        return 5;
                                    else
                                        return 1;
                else
                    if (sensor0 <= 0.704733f)
                        if (actual_omega <= -0.150311f)
                            if (sensor0 <= 0.663576f)
                                if (last_action_state <= 0.550000f)
                                    if (sensor2 <= 0.088837f)
                                        return 1;
                                    else
                                        return 1;
                                else
                                    if (actual_vx <= 0.002973f)
                                        return 1;
                                    else
                                        return 1;
                            else
                                if (sensor4 <= 0.561341f)
                                    if (actual_vz <= 1.100098f)
                                        return 1;
                                    else
                                        return 1;
                                else
                                    if (sensor1 <= 0.655946f)
                                        return 1;
                                    else
                                        return 1;
                        else
                            if (actual_vx <= -0.001050f)
                                if (last_action_state <= 0.450000f)
                                    if (actual_vz <= 1.108990f)
                                        return 1;
                                    else
                                        return 5;
                                else
                                    if (sensor0 <= 0.697818f)
                                        return 1;
                                    else
                                        return 1;
                            else
                                if (sensor3 <= 0.746225f)
                                    if (sensor0 <= 0.573198f)
                                        return 1;
                                    else
                                        return 1;
                                else
                                    if (sensor2 <= 0.095847f)
                                        return 1;
                                    else
                                        return 1;
                    else
                        if (sensor0 <= 0.780404f)
                            if (sensor4 <= 0.475204f)
                                if (sensor0 <= 0.732946f)
                                    if (actual_vz <= 1.110191f)
                                        return 1;
                                    else
                                        return 1;
                                else
                                    if (sensor4 <= 0.408946f)
                                        return 1;
                                    else
                                        return 1;
                            else
                                if (actual_omega <= -0.177081f)
                                    if (sensor3 <= 0.844776f)
                                        return 1;
                                    else
                                        return 2;
                                else
                                    if (sensor0 <= 0.735460f)
                                        return 1;
                                    else
                                        return 1;
                        else
                            if (last_action_state <= 0.350000f)
                                if (sensor4 <= 0.568798f)
                                    if (actual_vz <= 1.097751f)
                                        return 2;
                                    else
                                        return 1;
                                else
                                    if (actual_vz <= 1.087587f)
                                        return 2;
                                    else
                                        return 1;
                            else
                                if (sensor4 <= 0.368245f)
                                    if (sensor1 <= 0.404986f)
                                        return 1;
                                    else
                                        return 1;
                                else
                                    if (actual_omega <= -0.197862f)
                                        return 1;
                                    else
                                        return 1;
        else
            if (sensor2 <= 0.319522f)
                if (sensor2 <= 0.280087f)
                    if (actual_omega <= -0.020420f)
                        if (sensor2 <= 0.246995f)
                            if (actual_vz <= 0.988604f)
                                if (sensor5 <= 0.412598f)
                                    if (actual_vx <= 0.010656f)
                                        return 5;
                                    else
                                        return 5;
                                else
                                    if (actual_vz <= 0.836636f)
                                        return 5;
                                    else
                                        return 4;
                            else
                                if (sensor0 <= 0.428750f)
                                    if (actual_omega <= -0.072176f)
                                        return 5;
                                    else
                                        return 5;
                                else
                                    if (sensor0 <= 0.616031f)
                                        return 5;
                                    else
                                        return 1;
                        else
                            if (sensor2 <= 0.273768f)
                                if (last_action_state <= 0.350000f)
                                    if (sensor2 <= 0.262581f)
                                        return 5;
                                    else
                                        return 5;
                                else
                                    if (actual_omega <= -0.086985f)
                                        return 5;
                                    else
                                        return 5;
                            else
                                if (sensor4 <= 0.845007f)
                                    if (sensor2 <= 0.275831f)
                                        return 5;
                                    else
                                        return 6;
                                else
                                    if (actual_omega <= -0.069263f)
                                        return 6;
                                    else
                                        return 5;
                    else
                        if (sensor2 <= 0.243159f)
                            if (actual_vz <= 0.979104f)
                                if (sensor5 <= 0.453268f)
                                    if (sensor0 <= 0.619103f)
                                        return 5;
                                    else
                                        return 5;
                                else
                                    if (sensor4 <= 0.985023f)
                                        return 0;
                                    else
                                        return 5;
                            else
                                if (sensor2 <= 0.232559f)
                                    if (sensor2 <= 0.224058f)
                                        return 1;
                                    else
                                        return 1;
                                else
                                    if (sensor3 <= 0.381136f)
                                        return 5;
                                    else
                                        return 5;
                        else
                            if (sensor5 <= 0.450675f)
                                if (sensor1 <= 0.823563f)
                                    if (sensor2 <= 0.261631f)
                                        return 5;
                                    else
                                        return 5;
                                else
                                    if (sensor2 <= 0.273961f)
                                        return 5;
                                    else
                                        return 5;
                            else
                                if (last_action_state <= 0.350000f)
                                    if (actual_omega <= 0.019039f)
                                        return 4;
                                    else
                                        return 0;
                                else
                                    if (sensor1 <= 0.852446f)
                                        return 5;
                                    else
                                        return 5;
                else
                    if (actual_omega <= -0.019948f)
                        if (actual_vx <= 0.006021f)
                            if (sensor5 <= 0.263781f)
                                if (sensor0 <= 0.341316f)
                                    if (sensor2 <= 0.288952f)
                                        return 6;
                                    else
                                        return 6;
                                else
                                    if (sensor2 <= 0.297198f)
                                        return 5;
                                    else
                                        return 6;
                            else
                                if (actual_omega <= -0.062483f)
                                    if (actual_vz <= 1.077131f)
                                        return 6;
                                    else
                                        return 5;
                                else
                                    if (sensor1 <= 0.873556f)
                                        return 5;
                                    else
                                        return 6;
                        else
                            if (actual_vz <= 0.982231f)
                                if (sensor5 <= 0.342816f)
                                    if (sensor4 <= 0.873911f)
                                        return 5;
                                    else
                                        return 5;
                                else
                                    if (sensor5 <= 0.462664f)
                                        return 5;
                                    else
                                        return 5;
                            else
                                if (sensor0 <= 0.318678f)
                                    if (actual_vz <= 1.072591f)
                                        return 5;
                                    else
                                        return 10;
                                else
                                    if (sensor5 <= 0.388714f)
                                        return 5;
                                    else
                                        return 4;
                    else
                        if (sensor5 <= 0.439924f)
                            if (sensor2 <= 0.290352f)
                                if (sensor4 <= 0.840199f)
                                    if (last_action_state <= 0.550000f)
                                        return 5;
                                    else
                                        return 5;
                                else
                                    if (last_action_state <= 0.550000f)
                                        return 5;
                                    else
                                        return 5;
                            else
                                if (sensor1 <= 0.868044f)
                                    if (actual_vx <= 0.006746f)
                                        return 5;
                                    else
                                        return 5;
                                else
                                    if (last_action_state <= 0.550000f)
                                        return 5;
                                    else
                                        return 5;
                        else
                            if (actual_vx <= -0.006431f)
                                if (actual_vz <= 1.006844f)
                                    if (sensor0 <= 0.338638f)
                                        return 6;
                                    else
                                        return 5;
                                else
                                    if (sensor3 <= 0.379556f)
                                        return 5;
                                    else
                                        return 6;
                            else
                                if (last_action_state <= 0.350000f)
                                    if (sensor5 <= 0.483786f)
                                        return 5;
                                    else
                                        return 0;
                                else
                                    if (sensor5 <= 0.521777f)
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
                                        return 9;
                                    else
                                        return 7;
                                else
                                    if (sensor0 <= 0.335815f)
                                        return 6;
                                    else
                                        return 6;
                            else
                                if (sensor4 <= 0.894221f)
                                    return 4;
                                else
                                    if (sensor4 <= 0.907309f)
                                        return 5;
                                    else
                                        return 4;
                        else
                            if (actual_omega <= 0.017936f)
                                if (actual_vx <= 0.007767f)
                                    if (sensor2 <= 0.326715f)
                                        return 6;
                                    else
                                        return 6;
                                else
                                    if (sensor3 <= 0.310878f)
                                        return 5;
                                    else
                                        return 10;
                            else
                                if (sensor2 <= 0.328441f)
                                    if (sensor3 <= 0.334668f)
                                        return 5;
                                    else
                                        return 5;
                                else
                                    if (actual_vx <= -0.001577f)
                                        return 5;
                                    else
                                        return 6;
                    else
                        if (sensor1 <= 0.851336f)
                            if (actual_vx <= 0.002531f)
                                if (actual_vz <= 0.930198f)
                                    if (sensor0 <= 0.267689f)
                                        return 6;
                                    else
                                        return 6;
                                else
                                    if (actual_vx <= -0.005199f)
                                        return 6;
                                    else
                                        return 6;
                            else
                                if (last_action_state <= 0.750000f)
                                    if (actual_omega <= 0.026756f)
                                        return 10;
                                    else
                                        return 5;
                                else
                                    if (sensor4 <= 0.877085f)
                                        return 5;
                                    else
                                        return 5;
                        else
                            if (sensor2 <= 0.377152f)
                                if (actual_omega <= 0.035790f)
                                    if (sensor2 <= 0.375860f)
                                        return 6;
                                    else
                                        return 5;
                                else
                                    if (sensor1 <= 0.889316f)
                                        return 6;
                                    else
                                        return 6;
                            else
                                if (sensor2 <= 0.588373f)
                                    if (sensor1 <= 0.891610f)
                                        return 6;
                                    else
                                        return 6;
                                else
                                    if (actual_vx <= -0.004979f)
                                        return 6;
                                    else
                                        return 6;
                else
                    if (last_action_state <= 0.850000f)
                        if (sensor2 <= 0.356530f)
                            if (actual_vz <= 1.030618f)
                                if (last_action_state <= 0.350000f)
                                    if (sensor2 <= 0.329763f)
                                        return 6;
                                    else
                                        return 10;
                                else
                                    if (sensor5 <= 0.685192f)
                                        return 6;
                                    else
                                        return 7;
                            else
                                if (last_action_state <= 0.650000f)
                                    if (sensor2 <= 0.337808f)
                                        return 5;
                                    else
                                        return 7;
                                else
                                    if (actual_omega <= 0.099601f)
                                        return 7;
                                    else
                                        return 5;
                        else
                            if (last_action_state <= 0.550000f)
                                if (actual_vx <= -0.003118f)
                                    if (actual_omega <= 0.201071f)
                                        return 6;
                                    else
                                        return 7;
                                else
                                    if (sensor1 <= 0.827832f)
                                        return 10;
                                    else
                                        return 6;
                            else
                                if (actual_vx <= -0.002183f)
                                    if (actual_vz <= 0.959688f)
                                        return 6;
                                    else
                                        return 6;
                                else
                                    if (sensor1 <= 0.838157f)
                                        return 6;
                                    else
                                        return 6;
                    else
                        if (actual_omega <= 0.216773f)
                            if (sensor2 <= 0.367598f)
                                if (actual_vx <= 0.004129f)
                                    if (sensor4 <= 0.803135f)
                                        return 0;
                                    else
                                        return 5;
                                else
                                    return 6;
                            else
                                if (sensor3 <= 0.251223f)
                                    if (actual_vz <= 0.964013f)
                                        return 5;
                                    else
                                        return 5;
                                else
                                    if (last_action_state <= 0.950000f)
                                        return 6;
                                    else
                                        return 5;
                        else
                            if (actual_vz <= 0.946362f)
                                return 6;
                            else
                                if (actual_vx <= -0.004699f)
                                    if (sensor2 <= 0.494190f)
                                        return 5;
                                    else
                                        return 0;
                                else
                                    if (sensor4 <= 0.704462f)
                                        return 4;
                                    else
                                        return 5;
    }
}
