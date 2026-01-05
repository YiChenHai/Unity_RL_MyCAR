using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class MyCarAgent : Agent
{
    [Header("Config Priority")]
    [Tooltip("勾选: 使用Unity Inspector(场景/Prefab序列化)中的值。\n不勾选: 运行时与编辑器中将被脚本默认值覆盖(以代码为准)。")]
    // 参数优先级开关：
    // - true  : 以 Inspector(序列化) 为准（方便在 Unity 中调参）
    // - false : 以代码默认值为准（强制覆盖 Inspector，避免旧序列化值干扰）
    public bool preferInspectorValues = true;

    [Header("Refs")]
    public MagneticTape tape;
    [Tooltip("传感器顺序: [0]=前左, [1]=前中, [2]=前右, [3]=后左, [4]=后中, [5]=后右")]
    public Transform[] sensors = new Transform[6];
    public Rigidbody rb;
    public MyCar_Motion myCarMotion;

    [Header("Control limits (body frame - Unity标准)")]
    public float constantForwardSpeed = 0.2f;  // vz 固定前进速度 m/s
    public float maxLateralSpeed = 0.1f;       // vx (横向速度) m/s
    public float maxOmegaDeg = 120f;            // omega (自转角速度) deg/s - 防止轮子翻转

    [Header("Normalization")]
    public float maxField = 8f;                // 磁场最大值

    [Header("Termination")]
    public float derailThreshold = 2f;         // 脱轨阈值（中心传感器低于此值终止）

    [Header("Episode")]
    public float maxEpisodeTime = 40f;
    private float episodeTimer = 0f;

    [Header("Start pose")]
    public Vector3 startPos = new Vector3(0f, 0.15f, 1f);
    public Quaternion startRot = Quaternion.Euler(0f, 0f, 0f);

    // ===== Script defaults (used when preferInspectorValues == false) =====
    // 说明：Unity会序列化(保存)Inspector中的字段值；因此“脚本里写的初始化默认值”
    // 并不会自动覆盖已经存在的组件实例(场景对象/Prefab)的序列化数据。
    //
    // 这里的 Default* 变量就是一份“脚本默认值副本”：
    // - 当 preferInspectorValues == false 时，会用这些 Default* 值覆盖实例字段
    // - 用意是让你能明确选择“以代码为准”，且不受旧的序列化值影响
    //
    // 维护建议：
    // - 想改“代码默认值”就改下面这些 Default* 常量
    // - Inspector 里的同名字段只是运行时参数容器，是否生效由 preferInspectorValues 决定

	
    // 默认前进速度 vz（m/s）。当 preferInspectorValues=false 时，会写入 constantForwardSpeed。
    private const float DefaultConstantForwardSpeed = 0.2f;
	
    // 最大横向速度 vx（m/s）。当 preferInspectorValues=false 时，会写入 maxLateralSpeed。
    private const float DefaultMaxLateralSpeed = 0.1f;
	
    // 最大自转角速度上限（deg/s）。当 preferInspectorValues=false 时，会写入 maxOmegaDeg。
    private const float DefaultMaxOmegaDeg = 120f;
	
    // 磁场强度归一化的分母（maxField）。观测中使用 mag.magnitude/maxField 归一化。
    private const float DefaultMaxField = 8f;
	
    // 脱轨判定阈值：前中/后中传感器强度低于该值即判定脱轨并结束回合。
    private const float DefaultDerailThreshold = 1.5f;
	
    // 单回合最大时长（秒）。超过则判定超时结束回合。
    private const float DefaultMaxEpisodeTime = 40f;
	
    // 回合起始位置（世界坐标）。OnEpisodeBegin 时传送到该位置。
    private static readonly Vector3 DefaultStartPos = new Vector3(0f, 0.15f, 1f);
	
    // 回合起始朝向（欧拉角 0,0,0）。OnEpisodeBegin 时设置该旋转。
    // 这里用 default 做占位，ApplyScriptDefaults 内部会写成 Quaternion.Euler(0,0,0)。
    private static readonly Quaternion DefaultStartRot = default;

    // 防止 OnValidate/Initialize 触发连锁赋值时发生重复进入（递归/重入）。
    private bool _applyingDefaults = false;

    public override void Initialize()
    {
        base.Initialize();
        if (rb == null) rb = GetComponent<Rigidbody>();

        if (!preferInspectorValues)
        {
            ApplyScriptDefaults();
        }
    }

    // Unity 编辑器回调：当 Inspector 字段被修改、脚本重载、或勾选/取消勾选开关时会触发。
    private void OnValidate()
    {
        // 在编辑器中切换开关时，立即体现“以代码为准”的效果
        if (!preferInspectorValues)
        {
            ApplyScriptDefaults();
        }
    }

    // 将 Default* 这套“代码默认值副本”覆盖写回到实例字段。
    // 仅在 preferInspectorValues == false 时调用。
    private void ApplyScriptDefaults()
    {
        if (_applyingDefaults) return;
        _applyingDefaults = true;

        constantForwardSpeed = DefaultConstantForwardSpeed;
        maxLateralSpeed = DefaultMaxLateralSpeed;
        maxOmegaDeg = DefaultMaxOmegaDeg;
        maxField = DefaultMaxField;
        derailThreshold = DefaultDerailThreshold;
        maxEpisodeTime = DefaultMaxEpisodeTime;
        startPos = DefaultStartPos;
        startRot = DefaultStartRot == default ? Quaternion.Euler(0f, 0f, 0f) : DefaultStartRot;

        _applyingDefaults = false;
    }

    public override void OnEpisodeBegin() 
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        transform.position = startPos;
        transform.rotation = startRot;

        // 设置固定前进速度，清除其他输入
        if (myCarMotion != null) myCarMotion.SetControl(constantForwardSpeed, 0f, 0f);

        episodeTimer = 0f;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 1-6: 六个传感器的归一化强度（环境感知）
        for (int i = 0; i < sensors.Length; i++)
        {
            if (sensors[i] != null && tape != null)
            {
                Vector3 mag = tape.GetMagneticField(sensors[i].position);
                sensor.AddObservation(Mathf.Clamp01(mag.magnitude / Mathf.Max(1e-9f, maxField)));
            }
            else sensor.AddObservation(0f);
        }

        // 7-9: 当前运动状态（车身坐标系）- AI决策反馈
        Vector3 localVel = transform.InverseTransformDirection(rb != null ? rb.linearVelocity : Vector3.zero);
        float angularVel = rb != null ? rb.angularVelocity.y : 0f;
        
        sensor.AddObservation(localVel.z / Mathf.Max(0.001f, 1));  // 7: 前进速度 (Unity Z轴)
        sensor.AddObservation(localVel.x / Mathf.Max(0.001f, 1));      // 8: 横向速度 (Unity X轴)
        
        float maxOmegaRad = maxOmegaDeg * Mathf.Deg2Rad;
        sensor.AddObservation(Mathf.Clamp(angularVel / maxOmegaRad, -1f, 1f));       // 9: 角速度 omega
    }

    public override void OnActionReceived(ActionBuffers actions)
    { 
        // 连续动作：0=omega比例(自转)，禁用横向移动
        float a_w = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);

        // 映射到真实控制量（vz固定，vx禁用，只控制omega）
        float vz = constantForwardSpeed;                        // 固定前进速度
        float vx = 0f;                                          // 禁用横向移动
        float omega = a_w * maxOmegaDeg * Mathf.Deg2Rad;       // 自转角速度 rad/s

        // 下发给 MyCar_Motion 控制车辆
        if (myCarMotion != null) myCarMotion.SetControl(vz, vx, omega);

        // 读取传感器数据
        float[] sensorValues = new float[6];
        for (int i = 0; i < sensors.Length; i++)
        {
            if (sensors[i] != null && tape != null)
            {
                Vector3 mag = tape.GetMagneticField(sensors[i].position);
                sensorValues[i] = mag.magnitude;
            }
        }
        
        // ========== 终止条件1：脱轨检测 ==========
        float frontCenter = sensorValues[1];  // 前中
        float rearCenter = sensorValues[4];   // 后中
        
        if (frontCenter < derailThreshold || rearCenter < derailThreshold)
        {
            AddReward(-5f);
            Debug.Log($"Episode Ended: derailment. frontCenter={frontCenter:F4}, rearCenter={rearCenter:F4}");
            EndEpisode();
            return;
        }

        // ========== 计算对齐奖励 ==========
        float reward = CalculateReward(sensorValues);
        AddReward(reward * Time.fixedDeltaTime);

        // ========== 终止条件2：超时 ==========
        episodeTimer += Time.fixedDeltaTime;
        if (episodeTimer >= maxEpisodeTime)
        {
            Debug.Log($"Episode Ended: timeout. episodeTimer={episodeTimer:F2}s");
            EndEpisode();
        }  
    }

    float CalculateReward(float[] s)
    {
        if (s == null || s.Length < 6) return 0f;

        // ========== 对齐奖励：中心强度 × 对称性 ==========
        
        // 1. 中心强度因子：前中、后中传感器强度（判断是否在轨迹正上方）
        float frontCenter = s[1];   // 前中
        float rearCenter = s[4];    // 后中
        float centerAvg = (frontCenter + rearCenter) * 0.5f;
        float centerStrength = Mathf.Clamp01(centerAvg / Mathf.Max(1e-6f, maxField));
        
        // 2. 对称性因子：前后左右对称性（判断车身姿态是否对齐）
        // 前排对称：前左 vs 前右
        float frontSymmetry = Mathf.Clamp01(1f - Mathf.Abs(s[0] - s[2]) / maxField);
        // 后排对称：后左 vs 后右
        float rearSymmetry = Mathf.Clamp01(1f - Mathf.Abs(s[3] - s[5]) / maxField);
        // 只有前后都对称时才给高分（取最小值，确保整车对齐）
        float symmetry = Mathf.Min(frontSymmetry, rearSymmetry);
        
        // 综合对齐分数：既要在轨迹上方（中心强），又要姿态对齐（对称）
        float alignment = centerStrength * symmetry;

        // ========== 前进速度因子：分段式速度奖励（转弯宽容） ==========
        Vector3 vel = rb != null ? rb.linearVelocity : Vector3.zero;
        float forwardSpeed = Vector3.Dot(vel, transform.forward);  // 实际前进速度
        
        float speedThreshold = constantForwardSpeed * 0.60f;  // 60%阈值
        float speedRatio;
        
        if (forwardSpeed >= speedThreshold)
        {
            // 速度足够（≥80%目标），给予全额奖励
            speedRatio = 1.0f;
        }
        else if (forwardSpeed >= 0.05f)
        {
            // 速度介于10cm/s和60%阈值之间，线性衰减
            speedRatio = forwardSpeed / speedThreshold;
        }
        else
        {
            // 几乎停止（<10cm/s），无奖励
            speedRatio = 0f;
        }
        
        // 最终奖励 = 对齐分数 × 前进因子
        // 转弯时只要保持≥60%目标速度，就不会损失奖励
        return alignment * speedRatio;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // 不需要手动控制
    }
}