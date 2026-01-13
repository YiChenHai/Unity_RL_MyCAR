using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class MyCarAgent : Agent
{
    [Header("Data Collection for Distillation")]
    public bool enableDataCollection = false;  // 启用数据收集模式
    [Tooltip("累积模式：多回合累积数据，手动导出\n每回合模式：每回合结束时自动追加到同一文件")]
    public bool exportPerEpisode = false;      // true=每回合追加，false=累积模式
    [Tooltip("是否启用记录条数限制")]
    public bool enableMaxSamplesLimit = false;  // 是否启用最大记录条数限制
    [Tooltip("最大记录条数（仅在启用限制时有效）\n达到此数量后，累积模式会自动导出并停止收集\n每回合模式不受此限制影响")]
    [Range(1, 1000000)]
    public int maxDataSamples = 10000;         // 最大记录条数
    [Tooltip("CSV文件保存路径（留空则使用默认路径）\n可以是目录路径（如: D:/Data/）或完整文件路径\n留空时使用: Application.persistentDataPath")]
    public string customSavePath = "";         // 用户指定的保存路径
    private List<string> collectedData = new List<string>();  // CSV格式: obs0,obs1,...,obs12,action0,action1
    private int episodeDataCount = 0;          // 当前回合收集的数据数量（用于每回合模式）
    private string episodeDataFilePath = null;  // 每回合模式使用的文件路径
    private bool episodeDataHeaderWritten = false;  // 是否已写入CSV头部（每回合模式）
    
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
    public float alignedThresholdPercent = 0.15f;  // 对齐状态：左右差值阈值（15%，放宽）
    public float centerThresholdPercent = 0.45f;   // 对齐状态：中心传感器阈值（45%，放宽支持转弯）
    public float alignedBonus = 0.5f;              // 对齐状态的额外奖励
    public float speedHighPercent = 0.45f;         // 速度比例系数为1的阈值（45%）
    public float speedLowPercent = 0.10f;          // 速度惩罚阈值（10%，放宽以允许转弯减速）
    public float speedPenalty = -0.2f;             // 速度过低时的惩罚值（-0.5→-0.2，缓和）
    public float smallOutputBonus = 1.0f;          // 小输出奖励系数（稳定对齐时的精细控制激励）
    public float turningBonus = 0.3f;              // 转弯鼓励奖励幅度（避免过度激励）
    public float turningThreshold = 0.3f;          // 触发转弯奖励的角速度阈值（0.3，容易触发）
    public float smoothnessBonus = 2.0f;           // 输出平稳性奖励幅度（转弯时）
    public float stableSmoothnessBonus = 4.0f;     // 稳定对齐时的平稳性奖励幅度（强化版，鼓励极度平稳）
    [Range(0f, 5f)]
    public float angularSmoothnessWeight = 4.0f;   // 自转速度平稳性权重（越大越强调自转平稳）

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
    private float prevLateralSpeed = 0f;        // 前一帧的横向速度（用于平稳性计算）
    private float prevAngularSpeed = 0f;        // 前一帧的角速度（用于平稳性计算）
    private float lastRawLateralAction = 0f;    // 上一帧的原始神经网络输出（横向速度比例）
    private float lastRawAngularAction = 0f;    // 上一帧的原始神经网络输出（角速度比例）
    
    [Header("Output Smoothing")]
    [Range(0f, 1f)]
    public float smoothingAlpha = 0.3f;  // 指数平滑系数（0=完全平滑，1=无平滑）。建议0.2-0.4
    private float smoothedLateralSpeed = 0f;   // 平滑后的横向速度
    private float smoothedAngularSpeed = 0f;   // 平滑后的角速度

    [Header("Start pose")]
    public Quaternion startRot = Quaternion.Euler(0f, 0f, 0f);
    
    [Header("Random Spawn")]
    [Tooltip("出生点位置数组")]
    public Vector3[] spawnPositions = new Vector3[] 
    { 
        new Vector3(0f, 0.15f, 1f)
    };
    [Tooltip("对应的Y旋转角度数组（与spawnPositions保持相同长度）")]
    public float[] spawnYawAngles = new float[] 
    { 
        0f 
    };
    [Range(0f, 45f)]
    public float randomYawRange = 8f;  // 随机Y角度范围（±度数，叠加在基础角度上）

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
        
        // ========== 从出生点数组中随机选择 ==========
        int spawnIndex = Random.Range(0, Mathf.Max(1, spawnPositions.Length));
        Vector3 selectedPosition = spawnPositions.Length > spawnIndex 
            ? spawnPositions[spawnIndex]
            : new Vector3(0f, 0.15f, 1f);
        
        float selectedYawAngle = (spawnYawAngles.Length > spawnIndex && spawnPositions.Length > 0)
            ? spawnYawAngles[spawnIndex]
            : 0f;
        
        transform.position = selectedPosition;
        
        // ========== 随机Y角度（叠加在出生点的基础角度上） ==========
        float randomYaw = Random.Range(-randomYawRange, randomYawRange);
        float totalYaw = selectedYawAngle + randomYaw;
        Quaternion randomRotation = startRot * Quaternion.Euler(0f, totalYaw, 0f);
        transform.rotation = randomRotation;

        // 设置固定前进速度，清除其他输入
        if (myCarMotion != null) myCarMotion.SetControl(constantForwardSpeed, 0f, 0f);

        episodeTimer = 0f;
        alignedTimer = 0f;
        isStableAligned = false;
        lastOutputLateralSpeed = 0f;
        lastOutputAngularSpeed = 0f;
        prevLateralSpeed = 0f;
        prevAngularSpeed = 0f;
        smoothedLateralSpeed = 0f;  // 初始化平滑缓冲
        smoothedAngularSpeed = 0f;  // 初始化平滑缓冲
        lastRawLateralAction = 0f;  // 初始化原始动作
        lastRawAngularAction = 0f;  // 初始化原始动作
        
        // ========== 数据收集：回合结束处理 ==========
        if (enableDataCollection && exportPerEpisode && collectedData.Count > 0)
        {
            // 每回合模式：追加上一回合的数据到文件（在 OnEpisodeBegin 时，上一回合已结束）
            AppendEpisodeDataToFile();
            collectedData.Clear();  // 清理内存中的数据，准备新回合
            episodeDataCount = 0;
        }
        // 累积模式：不清理，继续累积数据
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

        // 7-8: 智能体上一次输出的平滑后的横向速度和自转速度（决策记忆）
        sensor.AddObservation(Mathf.Clamp(lastOutputLateralSpeed / Mathf.Max(0.001f, maxLateralSpeed), -2f, 2f));  // 7: 平滑输出横向速度
        float maxOmegaRad = maxOmegaDeg * Mathf.Deg2Rad;
        sensor.AddObservation(Mathf.Clamp(lastOutputAngularSpeed / maxOmegaRad, -2f, 2f));                          // 8: 平滑输出自转速度

        // 9-10: 原始神经网络输出动作（用于感知平滑延迟）
        sensor.AddObservation(Mathf.Clamp(lastRawLateralAction, -1f, 1f));   // 9: 原始横向动作
        sensor.AddObservation(Mathf.Clamp(lastRawAngularAction, -1f, 1f));   // 10: 原始角速度动作

        // 11-13: 车身实际运动状态（物理反馈）
        Vector3 localVel = transform.InverseTransformDirection(rb != null ? rb.linearVelocity : Vector3.zero);
        float angularVel = rb != null ? rb.angularVelocity.y : 0f;
        
        sensor.AddObservation(Mathf.Clamp(localVel.z / Mathf.Max(0.001f, constantForwardSpeed), -2f, 2f));  // 11: 实际前进速度
        sensor.AddObservation(Mathf.Clamp(localVel.x / Mathf.Max(0.001f, maxLateralSpeed), -2f, 2f));      // 12: 实际横向速度
        sensor.AddObservation(Mathf.Clamp(angularVel / maxOmegaRad, -2f, 2f));                              // 13: 实际角速度
    }

    public override void OnActionReceived(ActionBuffers actions)
    { 
        // 连续动作：0=横向速度比例，1=自转速度比例
        float a_x = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float a_w = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

        // ========== 数据收集（如果启用） ==========
        if (enableDataCollection)
        {
            // 检查记录条数限制（仅累积模式且启用限制时）
            if (!exportPerEpisode && enableMaxSamplesLimit && collectedData.Count >= maxDataSamples)
            {
                // 达到上限时自动导出
                ExportCollectedData();
                enableDataCollection = false;  // 停止收集
                Debug.Log($"[DataCollection] Reached max samples limit ({maxDataSamples}), auto-exported and disabled collection.");
            }
            else
            {
                RecordSample(a_x, a_w);
                if (exportPerEpisode)
                {
                    episodeDataCount++;  // 记录当前回合的数据数量
                }
            }
        }

        // 保存原始动作用于下一帧观察（供平滑延迟感知）
        lastRawLateralAction = a_x;
        lastRawAngularAction = a_w;

        // ========== 应用指数平滑滤波减少震荡 ==========
        // 指数平滑：smoothed = α·raw + (1-α)·smoothed_prev
        // α 越小越平滑（但响应延迟增加），建议 0.2-0.4
        float rawLateralSpeed = a_x * maxLateralSpeed;
        float rawAngularSpeed = a_w * maxOmegaDeg * Mathf.Deg2Rad;
        
        smoothedLateralSpeed = Mathf.Lerp(smoothedLateralSpeed, rawLateralSpeed, smoothingAlpha);
        smoothedAngularSpeed = Mathf.Lerp(smoothedAngularSpeed, rawAngularSpeed, smoothingAlpha);

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
        // 使用平滑后的输出
        float outputVx = smoothedLateralSpeed;
        float outputOmega = smoothedAngularSpeed;

        // 保存本次输出（用于下一次观察）
        lastOutputLateralSpeed = outputVx;
        lastOutputAngularSpeed = outputOmega;

        // 映射到真实控制量（vz固定）
        float vz = constantForwardSpeed;  // 固定前进速度

        // 下发给 MyCar_Motion 控制车辆
        if (myCarMotion != null) myCarMotion.SetControl(vz, outputVx, outputOmega);
        
        // 更新前一帧数据（用于计算平稳性）
        prevLateralSpeed = outputVx;
        prevAngularSpeed = outputOmega;

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
        float reward = CalculateReward(sensorValues, isAligned, isStableAligned, a_x, a_w, outputVx, outputOmega);
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

    float CalculateReward(float[] s, bool isAligned, bool isStableAligned, float a_x, float a_w, float outputVx, float outputOmega)
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
        float highSpeedThreshold = constantForwardSpeed * speedHighPercent;  // 45%阈值
        float lowSpeedThreshold = constantForwardSpeed * speedLowPercent;    // 10%阈值
        
        if (forwardSpeed >= highSpeedThreshold)
        {
            // 速度 >= 45%预设速度：系数为1
            speedCoefficient = 1.0f;
        }
        else if (forwardSpeed >= lowSpeedThreshold)
        {
            // 速度在10%-45%之间：比例减少（线性插值）
            speedCoefficient = (forwardSpeed - lowSpeedThreshold) / (highSpeedThreshold - lowSpeedThreshold);
        }
        else
        {
            // 速度 < 15%预设速度：轻度惩罚而非直接-1（允许转弯减速）
            return -1.0f;
        }
        
        // ========== 3. 平稳性奖励（扩展到所有状态，不仅仅对齐状态） ==========
        // 计算输出平稳性：奖励变化小的输出，抑制频繁的方向改变
        float lateralDelta = Mathf.Abs(prevLateralSpeed - outputVx) / Mathf.Max(0.001f, maxLateralSpeed);
        float angularDelta = Mathf.Abs(prevAngularSpeed - outputOmega) / Mathf.Max(0.001f, maxOmegaDeg * Mathf.Deg2Rad);
        
        // 平稳性指标：变化越小越好（指数衰减）
        // 对自转速度应用权重，使其变化更显著地影响平稳度评分
        // 例如：angularSmoothnessWeight=2.0 时，角速度变化的影响翻倍
        float weightedDelta = lateralDelta + (angularDelta * angularSmoothnessWeight);
        float smoothness = Mathf.Exp(-weightedDelta * 2f);  // *2f 使衰减更陡峭
        
        // 在稳定对齐状态使用强化的平稳性奖励
        float appliedSmoothnessBonus = isStableAligned ? stableSmoothnessBonus : smoothnessBonus;
        
        // 奖励平稳输出，但要确保有正数项基准
        // 当 smoothness=1（完全平稳）时：reward 随 appliedSmoothnessBonus 变化
        // 当 smoothness=0.37（e^-1）时：reward ≈ 0
        float smoothnessReward = (smoothness - 0.37f) * appliedSmoothnessBonus;  // 中立点在 e^-1 ≈ 0.37
        
        // ========== 4. 稳定对齐状态下的小输出奖励 ==========
        float outputBonus = 0f;
        if (isStableAligned)
        {
            // 计算动作幅度（0-1范围）
            float actionMagnitude = Mathf.Sqrt(a_x * a_x + a_w * a_w) / Mathf.Sqrt(2f);
            // 动作越小，奖励越高（鼓励平稳跟随）
            outputBonus = (1f - actionMagnitude) * smallOutputBonus;
        }

        // ========== 5. 转弯鼓励奖励：在转弯时（非对齐状态）奖励坚持转弯 ==========
        float turningReward = 0f;
        if (!isAligned)  // 只在非对齐模式（转弯阶段）启用
        {
            float angularMagnitude = Mathf.Abs(a_w);  // 角速度幅度 (0-1)
            if (angularMagnitude > turningThreshold)  // 使用参数化阈值（默认0.5）
            {
                // 转弯幅度越大，奖励越多，但不超过 turningBonus
                // 目的：鼓励智能体坚持转弯而不是频繁改变方向
                turningReward = Mathf.Min(angularMagnitude * turningBonus, turningBonus);
            }
        }
        
        // ========== 6. 最终奖励 ==========
        // 对齐奖励 × 速度系数：基础轨迹跟踪奖励
        // + 平稳性奖励：鼓励稳定输出，抑制频繁震荡
        // + 小输出奖励：稳定对齐时鼓励精细控制
        // + 转弯奖励：转弯时鼓励坚持而不是改变
        return alignmentReward * speedCoefficient + smoothnessReward + outputBonus + turningReward;
    }

    // ========== 数据收集方法 ==========
    private void RecordSample(float actionX, float actionW)
    {
        // 重新计算观测（与CollectObservations逻辑相同）
        List<float> observations = new List<float>();

        // 1-6: 六个传感器的归一化强度
        float[] sensorValues = new float[6];
        for (int i = 0; i < sensors.Length; i++)
        {
            if (sensors[i] != null && tape != null)
            {
                Vector3 mag = tape.GetMagneticField(sensors[i].position);
                sensorValues[i] = mag.magnitude;
                observations.Add(Mathf.Clamp01(mag.magnitude / Mathf.Max(1e-9f, maxField)));
            }
            else observations.Add(0f);
        }

        // 7-8: 平滑后的输出
        observations.Add(Mathf.Clamp(lastOutputLateralSpeed / Mathf.Max(0.001f, maxLateralSpeed), -2f, 2f));
        float maxOmegaRad = maxOmegaDeg * Mathf.Deg2Rad;
        observations.Add(Mathf.Clamp(lastOutputAngularSpeed / maxOmegaRad, -2f, 2f));

        // 9-10: 原始动作
        observations.Add(Mathf.Clamp(lastRawLateralAction, -1f, 1f));
        observations.Add(Mathf.Clamp(lastRawAngularAction, -1f, 1f));

        // 11-13: 物理状态
        Vector3 localVel = transform.InverseTransformDirection(rb != null ? rb.linearVelocity : Vector3.zero);
        float angularVel = rb != null ? rb.angularVelocity.y : 0f;
        observations.Add(Mathf.Clamp(localVel.z / Mathf.Max(0.001f, constantForwardSpeed), -2f, 2f));
        observations.Add(Mathf.Clamp(localVel.x / Mathf.Max(0.001f, maxLateralSpeed), -2f, 2f));
        observations.Add(Mathf.Clamp(angularVel / maxOmegaRad, -2f, 2f));

        // 构建CSV行：obs[0-12],action[0-1]
        StringBuilder sb = new StringBuilder();
        foreach (float obs in observations)
        {
            sb.Append(obs.ToString("F6"));
            sb.Append(",");
        }
        sb.Append(actionX.ToString("F6"));
        sb.Append(",");
        sb.Append(actionW.ToString("F6"));
        
        collectedData.Add(sb.ToString());
    }

    // 获取保存目录路径（如果用户指定了路径则使用，否则使用默认路径）
    private string GetSaveDirectory()
    {
        if (!string.IsNullOrEmpty(customSavePath))
        {
            // 如果用户指定的是完整文件路径，提取目录部分
            if (Path.HasExtension(customSavePath))
            {
                string dir = Path.GetDirectoryName(customSavePath);
                return string.IsNullOrEmpty(dir) ? Application.persistentDataPath : dir;
            }
            // 如果是指定的目录路径，直接使用
            return customSavePath;
        }
        // 默认使用持久化数据路径
        return Application.persistentDataPath;
    }

    // 获取完整文件路径
    private string GetFullFilePath(string fileName)
    {
        // 如果用户指定了完整文件路径，直接使用（忽略fileName参数）
        if (!string.IsNullOrEmpty(customSavePath) && Path.HasExtension(customSavePath))
        {
            return customSavePath;
        }
        
        // 否则组合目录和文件名
        string directory = GetSaveDirectory();
        return Path.Combine(directory, fileName);
    }

    // 追加回合数据到文件（每回合模式使用）
    private void AppendEpisodeDataToFile()
    {
        if (collectedData.Count == 0)
        {
            return;
        }

        // 首次写入时，初始化文件路径并写入头部
        if (episodeDataFilePath == null)
        {
            // 如果用户指定了完整文件路径，使用它（每回合模式追加到同一文件）
            if (!string.IsNullOrEmpty(customSavePath) && Path.HasExtension(customSavePath))
            {
                episodeDataFilePath = customSavePath;
            }
            else
            {
                // 否则使用目录+自动生成的文件名
                string fileName = $"episode_data_{System.DateTime.Now:yyyyMMdd_HHmmss}.csv";
                episodeDataFilePath = GetFullFilePath(fileName);
            }
            
            episodeDataHeaderWritten = false;
            
            // 确保目录存在
            string directory = Path.GetDirectoryName(episodeDataFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                try
                {
                    Directory.CreateDirectory(directory);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[DataCollection] Failed to create directory: {directory}\nError: {e.Message}");
                    return;
                }
            }
        }

        try
        {
            // 如果文件不存在或未写入头部，先写入头部
            if (!File.Exists(episodeDataFilePath) || !episodeDataHeaderWritten)
            {
                // 注意：最后两列是当前帧的原始动作（actionX, actionW），不是输出动作
                string header = "sensor0,sensor1,sensor2,sensor3,sensor4,sensor5," +
                               "smoothed_vx,smoothed_omega," +
                               "raw_action_x,raw_action_w," +
                               "actual_vz,actual_vx,actual_omega," +
                               "action_x,action_w";
                File.WriteAllText(episodeDataFilePath, header + System.Environment.NewLine);
                episodeDataHeaderWritten = true;
            }

            // 追加数据行
            using (StreamWriter writer = new StreamWriter(episodeDataFilePath, append: true))
            {
                foreach (string dataLine in collectedData)
                {
                    writer.WriteLine(dataLine);
                }
            }

            Debug.Log($"[DataCollection] Appended {collectedData.Count} samples from episode to:\n{episodeDataFilePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[DataCollection] Failed to append episode data: {e.Message}");
        }
    }

    // 导出收集的数据为CSV文件（累积模式或手动导出使用）
    public void ExportCollectedData(string customFileName = null)
    {
        if (collectedData.Count == 0)
        {
            Debug.LogWarning("[DataCollection] No data collected yet!");
            return;
        }

        // 生成文件名（带时间戳）
        string fileName = customFileName ?? $"training_data_{System.DateTime.Now:yyyyMMdd_HHmmss}.csv";
        string filePath = GetFullFilePath(fileName);
        
        // 确保目录存在
        string directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            try
            {
                Directory.CreateDirectory(directory);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[DataCollection] Failed to create directory: {directory}\nError: {e.Message}");
                return;
            }
        }
        
        // 生成CSV头部
        List<string> csv = new List<string>();
        // 注意：最后两列是当前帧的原始动作（actionX, actionW），不是输出动作
        string header = "sensor0,sensor1,sensor2,sensor3,sensor4,sensor5," +
                       "smoothed_vx,smoothed_omega," +
                       "raw_action_x,raw_action_w," +
                       "actual_vz,actual_vx,actual_omega," +
                       "action_x,action_w";
        csv.Add(header);
        csv.AddRange(collectedData);

        // 写入文件
        try
        {
            File.WriteAllLines(filePath, csv);
            Debug.Log($"[DataCollection] Successfully exported {collectedData.Count} samples to:\n{filePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[DataCollection] Failed to export data: {e.Message}");
        }
    }

    // 清理收集的数据
    public void ClearCollectedData()
    {
        int count = collectedData.Count;
        collectedData.Clear();
        episodeDataCount = 0;
        
        // 重置每回合模式的文件状态
        if (exportPerEpisode)
        {
            episodeDataFilePath = null;
            episodeDataHeaderWritten = false;
        }
        
        Debug.Log($"[DataCollection] Cleared {count} samples from memory.");
    }

    // 获取当前收集的数据数量
    public int GetCollectedDataCount()
    {
        return collectedData.Count;
    }

    // 重置每回合模式的文件（开始新的文件）
    public void ResetEpisodeDataFile()
    {
        if (exportPerEpisode)
        {
            episodeDataFilePath = null;
            episodeDataHeaderWritten = false;
            Debug.Log("[DataCollection] Reset episode data file. Next episode will create a new file.");
        }
    }

    // 获取数据保存路径（用于调试或显示）
    public string GetDataSavePath()
    {
        return GetSaveDirectory();
    }

    // 打印数据保存路径到控制台
    [ContextMenu("Print Data Save Path")]
    public void PrintDataSavePath()
    {
        string actualPath = GetSaveDirectory();
        string defaultPath = Application.persistentDataPath;
        
        string message = $"[DataCollection] CSV文件保存路径:\n";
        
        if (!string.IsNullOrEmpty(customSavePath))
        {
            message += $"用户指定路径: {customSavePath}\n";
            message += $"实际保存目录: {actualPath}\n";
            if (Path.HasExtension(customSavePath))
            {
                message += $"完整文件路径: {customSavePath}\n";
            }
        }
        else
        {
            message += $"使用默认路径: {actualPath}\n";
        }
        
        message += $"\n默认路径（未指定时使用）:\n{defaultPath}\n\n";
        message += $"平台特定路径:\n";
        message += $"Windows: %userprofile%\\AppData\\LocalLow\\<CompanyName>\\<ProductName>\n";
        message += $"Mac: ~/Library/Application Support/<CompanyName>/<ProductName>\n";
        message += $"Linux: ~/.config/unity3d/<CompanyName>/<ProductName>";
        
        Debug.Log(message);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // 不需要手动控制
    }
}