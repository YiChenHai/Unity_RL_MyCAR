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

    [Header("Episode")]
    public float maxEpisodeTime = 40f;
    private float episodeTimer = 0f;

    [Header("Reward Params")]
    public float alignedThresholdPercent = 0.15f;  // 对齐状态：左右差值阈值（18%，放宽）
    public float centerThresholdPercent = 0.45f;   // 对齐状态：中心传感器阈值（40%，大幅放宽支持转弯）
    public float alignedBonus = 0.5f;              // 对齐状态的额外奖励
    public float speedHighPercent = 0.45f;         // 速度比例系数为1的阈值（45%）
    public float speedLowPercent = 0.10f;          // 速度惩罚阈值（10%，放宽以允许转弯减速）
    public float speedPenalty = -0.2f;             // 速度过低时的惩罚值（-0.5→-0.2，缓和）
    public float smallOutputBonus = 0.5f;          // 小输出奖励系数（原0.3，增加引导力度）
    public float turningBonus = 0.5f;              // 转弯鼓励奖励幅度（0.2→0.5，加强转弯激励）
    public float turningThreshold = 0.5f;         // 触发转弯奖励的角速度阈值

    [Header("Debug")]
    public bool enableDebugLog = false;  // 调试日志开关

    [Header("Stable Tracking")]
    public float stableAlignedTime = 1.0f;     // 稳定对齐时间阈值（秒）
    private float alignedTimer = 0f;           // 对齐状态计时器
    private bool isStableAligned = false;      // 是否处于稳定对齐状态
    
    // 公开对齐状态供外部访问（如UI显示）
    public bool IsAligned { get; private set; }          // 当前是否对齐
    public bool IsStableAligned => isStableAligned;      // 当前是否稳定对齐
    public float AlignedTimer => alignedTimer;           // 对齐计时器（秒）
    
    // 动作记忆（用于观察空间）
    private float lastOutputLateralSpeed = 0f;  // 上一次输出的横向速度
    private float lastOutputAngularSpeed = 0f;  // 上一次输出的自转速度

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
    private const float DefaultMaxLateralSpeed = 0.2f;
	
    // 最大自转角速度上限（deg/s）。当 preferInspectorValues=false 时，会写入 maxOmegaDeg。
    private const float DefaultMaxOmegaDeg = 80;
	
    // 磁场强度归一化的分母（maxField）。观测中使用 mag.magnitude/maxField 归一化。
    private const float DefaultMaxField = 8f;
	
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
        alignedTimer = 0f;
        isStableAligned = false;
        lastOutputLateralSpeed = 0f;
        lastOutputAngularSpeed = 0f;
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

        // 7-8: 智能体上一次输出的横向速度和自转速度（决策记忆）
        sensor.AddObservation(Mathf.Clamp(lastOutputLateralSpeed / Mathf.Max(0.001f, maxLateralSpeed), -2f, 2f));  // 7: 输出横向速度
        float maxOmegaRad = maxOmegaDeg * Mathf.Deg2Rad;
        sensor.AddObservation(Mathf.Clamp(lastOutputAngularSpeed / maxOmegaRad, -2f, 2f));                          // 8: 输出自转速度

        // 9-11: 车身实际运动状态（物理反馈）
        Vector3 localVel = transform.InverseTransformDirection(rb != null ? rb.linearVelocity : Vector3.zero);
        float angularVel = rb != null ? rb.angularVelocity.y : 0f;
        
        sensor.AddObservation(Mathf.Clamp(localVel.z / Mathf.Max(0.001f, constantForwardSpeed), -2f, 2f));  // 9: 实际前进速度
        sensor.AddObservation(Mathf.Clamp(localVel.x / Mathf.Max(0.001f, maxLateralSpeed), -2f, 2f));      // 10: 实际横向速度
        sensor.AddObservation(Mathf.Clamp(angularVel / maxOmegaRad, -2f, 2f));                              // 11: 实际角速度
    }

    public override void OnActionReceived(ActionBuffers actions)
    { 
        // 连续动作：0=横向速度比例，1=自转速度比例
        float a_x = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float a_w = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

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

        // ========== 判断对齐状态 ==========
        bool isAligned = CheckAlignmentState(sensorValues);
        IsAligned = isAligned;  // 更新公开属性
        
        // 更新对齐计时器
        if (isAligned)
        {
            alignedTimer += Time.fixedDeltaTime;
            if (alignedTimer >= stableAlignedTime)
            {
                isStableAligned = true;
            }
        }
        else
        {
            alignedTimer = 0f;
            isStableAligned = false;
        }

        // ========== 动作输出处理 ==========
        float outputVx, outputOmega;
        
        // 正常响应智能体输出（移除衰减，改为奖励引导）
        outputVx = a_x * maxLateralSpeed;
        outputOmega = a_w * maxOmegaDeg * Mathf.Deg2Rad;

        // 保存本次输出（用于下一次观察）
        // 注意：这里保存的是衰减后的实际控制量，而非神经网络的原始输出
        // 如需保存原始输出，改为: lastOutputLateralSpeed = a_x * maxLateralSpeed;
        lastOutputLateralSpeed = outputVx;
        lastOutputAngularSpeed = outputOmega;

        // 映射到真实控制量（vz固定）
        float vz = constantForwardSpeed;  // 固定前进速度

        // 下发给 MyCar_Motion 控制车辆
        if (myCarMotion != null) myCarMotion.SetControl(vz, outputVx, outputOmega);

        // ========== 终止条件1：脱轨检测（立即判定） ==========
        float frontCenter = sensorValues[1];  // 前中
        float rearCenter = sensorValues[4];   // 后中
        float derailThresholdValue = maxField * 0.2f;  // 20%最大磁场强度
        
        if (frontCenter < derailThresholdValue || rearCenter < derailThresholdValue)
        {
            // 中心传感器低于20% → 立即脱轨，无时间缓冲
            AddReward(-5f);
            if (enableDebugLog)
            {
                Debug.Log($"Episode Ended: derailment (immediate). frontCenter={frontCenter:F4}, rearCenter={rearCenter:F4}");
            }
            EndEpisode();
            return;
        }

        // ========== 计算奖励 ==========
        float reward = CalculateReward(sensorValues, isAligned, isStableAligned, a_x, a_w);
        AddReward(reward * Time.fixedDeltaTime);

        // ========== 终止条件2：超时 ==========
        episodeTimer += Time.fixedDeltaTime;
        if (episodeTimer >= maxEpisodeTime)
        {
            if (enableDebugLog)
            {
                Debug.Log($"Episode Ended: timeout. episodeTimer={episodeTimer:F2}s");
            }
            EndEpisode();
        }  
    }

    // 判断是否处于对齐状态
    bool CheckAlignmentState(float[] s)
    {
        if (s == null || s.Length < 6) return false;

        // 前后两对左右传感器的差值
        float frontDiff = Mathf.Abs(s[0] - s[2]);  // 前左 vs 前右
        float rearDiff = Mathf.Abs(s[3] - s[5]);   // 后左 vs 后右
        
        // 两个中心传感器的值
        float frontCenter = s[1];  // 前中
        float rearCenter = s[4];   // 后中
        
        // 判断条件：
        // 1. 前后左右差值都小于最大磁场强度的15%
        // 2. 两个中心传感器都大于最大磁场强度的65%
        float diffThreshold = maxField * alignedThresholdPercent;
        float centerThreshold = maxField * centerThresholdPercent;
        
        bool leftRightAligned = (frontDiff < diffThreshold) && (rearDiff < diffThreshold);
        bool centerStrong = (frontCenter > centerThreshold) && (rearCenter > centerThreshold);
        
        return leftRightAligned && centerStrong;
    }

    float CalculateReward(float[] s, bool isAligned, bool isStableAligned, float a_x, float a_w)
    {
        if (s == null || s.Length < 6) return -1f;

        // ========== 1. 计算基础对齐奖励 ==========
        float alignmentReward;
        
        if (isAligned)
        {
            // 对齐状态：给予全额奖励 + 额外奖励
            alignmentReward = 1.0f + alignedBonus;
        }
        else
        {
            // 非对齐状态：组合对称性 + 中心强度（鼓励沿轨迹行进）
            // 计算对称性因子
            float frontSymmetry = Mathf.Clamp01(1f - Mathf.Abs(s[0] - s[2]) / maxField);
            float rearSymmetry = Mathf.Clamp01(1f - Mathf.Abs(s[3] - s[5]) / maxField);
            float symmetry = Mathf.Min(frontSymmetry, rearSymmetry);
            
            // 计算中心强度因子（指示是否在轨迹上）
            float frontCenter = s[1];
            float rearCenter = s[4];
            float centerAvg = (frontCenter + rearCenter) * 0.5f;
            float centerStrength = Mathf.Clamp01(centerAvg / Mathf.Max(1e-6f, maxField));
            
            // 综合对齐分数：对称性70% + 中心强度30%（支持转弯时的不完全对齐）
            alignmentReward = symmetry * 0.7f + centerStrength * 0.3f;
        }

        // ========== 2. 计算前进速度比例系数 ==========
        Vector3 vel = rb != null ? rb.linearVelocity : Vector3.zero;
        float forwardSpeed = Vector3.Dot(vel, transform.forward);  // 实际前进速度
        
        float speedCoefficient;
        float highSpeedThreshold = constantForwardSpeed * speedHighPercent;  // 50%阈值
        float lowSpeedThreshold = constantForwardSpeed * speedLowPercent;    // 15%阈值
        
        if (forwardSpeed >= highSpeedThreshold)
        {
            // 速度 >= 50%预设速度：系数为1
            speedCoefficient = 1.0f;
        }
        else if (forwardSpeed >= lowSpeedThreshold)
        {
            // 速度在15%-50%之间：比例减少（线性插值）
            speedCoefficient = (forwardSpeed - lowSpeedThreshold) / (highSpeedThreshold - lowSpeedThreshold);
        }
        else
        {
            // 速度 < 15%预设速度：轻度惩罚而非直接-1（允许转弯减速）
            return speedPenalty;
        }
        
        // ========== 3. 稳定对齐状态下的小输出奖励 ==========
        float outputBonus = 0f;
        if (isStableAligned)
        {
            // 计算动作幅度（0-1范围）
            float actionMagnitude = Mathf.Sqrt(a_x * a_x + a_w * a_w) / Mathf.Sqrt(2f);
            // 动作越小，奖励越高（鼓励平稳跟随）
            outputBonus = (1f - actionMagnitude) * smallOutputBonus;
        }

        // ========== 4. 转弯鼓励奖励（仅在非对齐状态，即转弯时启用） ==========
        float turningReward = 0f;
        if (!isAligned)  // 只在非对齐模式（转弯阶段）启用
        {
            float angularMagnitude = Mathf.Abs(a_w);  // 角速度幅度 (0-1)
            if (angularMagnitude > turningThreshold)  // 使用参数化阈值（默认0.15）
            {
                // 转弯幅度越大，奖励越多，但不超过 turningBonus
                turningReward = Mathf.Min(angularMagnitude * turningBonus, turningBonus);
            }
        }
        
        // ========== 5. 最终奖励 = 对齐奖励 × 速度系数 + 稳定对齐的小输出奖励 + 转弯奖励 ==========
        return alignmentReward * speedCoefficient + outputBonus + turningReward;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // 不需要手动控制
    }
}