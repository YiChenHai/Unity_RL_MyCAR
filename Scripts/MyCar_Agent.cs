using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

/// <summary>
/// 离散动作空间定义（扩展版：11个动作）
/// 动作索引顺序：急左转 → 大左转 → 左转 → 小左转 → 微左转 → 直行 → 微右转 → 小右转 → 右转 → 大右转 → 急右转
/// </summary>
public enum DiscreteAction
{
    TurnLeftSharp = 0,     // 急左转：vx=0, omega=+急转固定值
    TurnLeftLarge = 1,     // 大左转：vx=0, omega=+大转固定值
    TurnLeft = 2,          // 左转：vx=0, omega=+固定值
    TurnLeftSmall = 3,     // 小左转：vx=0, omega=+小转固定值
    TurnLeftMicro = 4,     // 微左转：vx=0, omega=+微转固定值
    Forward = 5,           // 直行：vx=0, omega=0
    TurnRightMicro = 6,    // 微右转：vx=0, omega=-微转固定值
    TurnRightSmall = 7,    // 小右转：vx=0, omega=-小转固定值
    TurnRight = 8,         // 右转：vx=0, omega=-固定值
    TurnRightLarge = 9,    // 大右转：vx=0, omega=-大转固定值
    TurnRightSharp = 10    // 急右转：vx=0, omega=-急转固定值
}

public class MyCarAgent : Agent
{
    
    [Header("Config Priority")]
    [Tooltip("勾选: 使用Unity Inspector中的值. 不勾选: 运行时将被脚本默认值覆盖")]
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
    public float maxLateralSpeed = 0.15f;       // vx (横向速度) m/s
    public float maxOmegaDeg = 80f;            // omega (自转角速度) deg/s - 防止轮子翻转
    
    [Header("Discrete Action Space (扩展版: 11个动作)")]
    [Tooltip("急左转/急右转的角速度值(归一化-1~1). 例如0.5625表示使用最大角速度的56.25%")]
    [Range(0f, 1f)]
    public float turnSharpAngularValue = 0.5625f;  // 急转弯的角速度值
    [Tooltip("大左转/大右转的角速度值（归一化，-1~1）")]
    [Range(0f, 1f)]
    public float turnLargeAngularValue = 0.4375f;  // 大转弯的角速度值
    [Tooltip("普通左转/右转的角速度值(归一化-1~1). 例如0.25表示使用最大角速度的25%")]
    [Range(0f, 1f)]
    public float turnAngularValue = 0.25f;  // 普通转弯的角速度值
    [Tooltip("小左转/小右转的角速度值（归一化，-1~1）")]
    [Range(0f, 1f)]
    public float turnSmallAngularValue = 0.125f;  // 小转弯的角速度值
    [Tooltip("微左转/微右转的角速度值（归一化，-1~1）")]
    [Range(0f, 1f)]
    public float turnMicroAngularValue = 0.0625f;  // 微转弯的角速度值

    [Header("Normalization")]
    public float maxField = 8f;                // 磁场最大值

    [Header("Episode")]
    public float maxEpisodeTime = 40f;
    private float episodeTimer = 0f;

    // ========== 奖励参数配置（按奖励项分组） ==========
    
    [Header("1. 基础对齐奖励 (Alignment Reward)")]
    [Tooltip("对齐状态左右差值阈值(15%放宽)")]
    public float alignedThresholdPercent = 0.1f;
    [Tooltip("对齐状态中心传感器阈值(45%放宽支持转弯)")]
    public float centerThresholdPercent = 0.65f;
    [Tooltip("对齐状态的额外奖励")]
    public float alignedBonus = 0.5f;

    [Header("2. 速度系数 (Speed Coefficient)")]
    [Tooltip("速度比例系数为1的阈值(60%). 当速度>=此阈值时,速度系数=1.0(全额奖励)")]
    public float speedHighPercent = 0.6f;
    [Tooltip("速度惩罚阈值(20%). 速度<此阈值时返回-2.0(大惩罚). 在此阈值和speedHighPercent之间时线性插值")]
    public float speedLowPercent = 0.20f;

    [Header("3. 趋势一致性奖励 (Trend Consistency)")]
    [Tooltip("是否启用趋势一致性奖励（基于误差映射目标动作）")]
    public bool enableTrendConsistencyReward = true;
    [Tooltip("基于传感器误差映射目标动作的增益（越大越激进）")]
    public float targetActionGain = 3.0f;
    [Tooltip("动作偏离目标动作的惩罚权重")]
    public float targetTrackingPenaltyWeight = 0.35f;
    [Tooltip("动作接近目标动作时的奖励（偏差<=1档）")]
    public float targetTrackingCloseBonus = 0.15f;


    [Header("6. 脱轨/预警惩罚 (Derailment/Warning Penalty)")]
    [Tooltip("脱轨惩罚(负数),脱轨时立即终止回合")]
    public float derailPenalty = -15.0f;
    [Tooltip("预警区域上限(25%,超过此值不惩罚不奖励)")]
    public float warningUpperThresholdPercent = 0.3f;
    [Tooltip("预警惩罚系数(每帧,18%~25%之间的线性惩罚)")]
    public float warningPenaltyCoefficient = -4.0f;

    [Header("7. 直线输出限制 (Straight Output Constraints)")]
    [Tooltip("对齐条件下输出微转和直行以外动作时的惩罚权重. 允许动作: TurnLeftMicro(4), Forward(5), TurnRightMicro(6)")]
    public float alignedActionPenalty = 1.5f;
    [Tooltip("对齐条件下输出Forward(直行)动作时的奖励值. 微调动作不奖励也不惩罚,直行动作给予小奖励")]
    public float forwardReward = 0.2f;
    [Tooltip("急转动作的惩罚倍数(TurnLeftSharp/TurnRightSharp索引0/10)")]
    public float sharpTurnPenaltyMultiplier = 2.0f;
    [Tooltip("普通转弯动作的惩罚倍数(TurnLeft/TurnRight索引2/8)")]
    public float normalTurnPenaltyMultiplier = 1.5f;

    [Header("Reward Tracking (for Display)")]
    [Tooltip("是否启用奖励跟踪(用于UI显示)")]
    public bool enableRewardTracking = true;
    private float cumulativeReward = 0f;  // 累计奖励
    private float[] rewardHistory;  // 奖励历史记录（用于绘制曲线）
    private int rewardHistoryIndex = 0;  // 奖励历史索引
    private int rewardHistoryLength = 200;  // 奖励历史长度
    
    // 奖励组成部分（用于详细显示）
    public struct RewardComponents
    {
        public float alignmentReward;      // 对齐奖励
        public float speedCoefficient;      // 速度系数
        public float trendConsistencyReward; // 趋势一致性奖励
        public float straightOutputPenalty; // 直线输出限制惩罚
        public float straightOutputPenaltyPercent; // 直线输出惩罚百分比（0-100%）
        public float totalReward;          // 总奖励（乘以dt前）
        public float rewardThisFrame;      // 本帧奖励（乘以dt后）
    }
    private RewardComponents currentRewardComponents;  // 当前奖励组成部分
    
    // 公开接口供外部访问
    public float CumulativeReward => cumulativeReward;
    public float[] RewardHistory => rewardHistory;
    public int RewardHistoryLength => rewardHistoryLength;
    public int RewardHistoryIndex => rewardHistoryIndex;
    public RewardComponents CurrentRewardComponents => currentRewardComponents;

    [Header("Debug")]
    public bool enableDebugLog = false;  // 调试日志开关

    [Header("Stable Tracking")]
    public float stableAlignedTime = 0.3f;     // 稳定对齐时间阈值（秒）
    private float alignedTimer = 0f;           // 对齐状态计时器
    private bool isStableAligned = false;      // 是否处于稳定对齐状态
    
    // 公开对齐状态供外部访问（如UI显示）
    public bool IsAligned { get; private set; }          // 当前是否对齐
    public bool IsStableAligned => isStableAligned;      // 当前是否稳定对齐
    public float AlignedTimer => alignedTimer;           // 对齐计时器（秒）
    
    // 公开动作状态供外部访问（如UI显示）
    public int CurrentDiscreteAction => lastDiscreteAction;  // 当前输出的离散动作索引（0-10）
    public int PreviousDiscreteAction => prevDiscreteAction;  // 上一帧的离散动作索引（0-10）
    
    // 公开状态供数据记录使用
    public float EpisodeTimer => episodeTimer;  // 当前回合时间
    public float LastOutputLateralSpeed => lastOutputLateralSpeed;  // 上次输出的横向速度
    public float LastOutputAngularSpeed => lastOutputAngularSpeed;  // 上次输出的角速度
    
    // 动作记忆（用于观察空间）
    private float lastOutputLateralSpeed = 0f;  // 上一次输出的横向速度
    private float lastOutputAngularSpeed = 0f;  // 上一次输出的自转速度
    private int lastDiscreteAction = 0;         // 上一帧的离散动作索引（用于观察空间）
    private int prevDiscreteAction = 0;         // 前一帧的离散动作索引（用于趋势奖励的异号判定）
    private System.Random spawnRng;             // 出生点随机数发生器（避免被Unity随机种子重置）
    
    [Header("Output Smoothing")]
    [Range(0f, 1f)]
    public float smoothingAlpha = 0.4f;  // 指数平滑系数（0=完全平滑，1=无平滑）。建议0.2-0.4
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
    [Tooltip("对应的Y旋转角度数组(与spawnPositions保持相同长度)")]
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
        if (spawnRng == null)
        {
            spawnRng = new System.Random(System.Environment.TickCount ^ GetInstanceID());
        }

        if (!preferInspectorValues)
        {
            ApplyScriptDefaults();
        }
        
        // 初始化奖励历史数组
        if (enableRewardTracking && rewardHistory == null)
        {
            rewardHistory = new float[rewardHistoryLength];
        }
        
        // 检查动作空间配置
        CheckActionSpaceConfiguration();
    }
    
    /// <summary>
    /// 检查动作空间配置是否正确
    /// </summary>
    void CheckActionSpaceConfiguration()
    {
        // 获取Behavior Parameters组件
        var behaviorParams = GetComponent<Unity.MLAgents.Policies.BehaviorParameters>();
        if (behaviorParams == null)
        {
            Debug.LogWarning("[MyCarAgent] 未找到BehaviorParameters组件，请确保已添加该组件。");
            return;
        }
        
        // 获取ActionSpec（新版本ML-Agents使用ActionSpec）
        var actionSpec = behaviorParams.BrainParameters.ActionSpec;
        
        // 强制使用离散动作空间（方案C）
        // 检查离散动作空间配置
        int numDiscreteActions = actionSpec.NumDiscreteActions;
        if (numDiscreteActions == 0)
        {
            Debug.LogError($"[MyCarAgent] 必须配置离散动作空间！\n" +
                         $"请在BehaviorParameters中设置：\n" +
                         $"- Space Size: 1\n" +
                         $"- Branch 0 Size: 11 (对应11个离散动作：TurnLeftSharp, TurnLeftLarge, TurnLeft, TurnLeftSmall, TurnLeftMicro, Forward, TurnRightMicro, TurnRightSmall, TurnRight, TurnRightLarge, TurnRightSharp)\n" +
                         $"- Continuous Actions: 0 (不使用连续动作)");
        }
        else if (numDiscreteActions > 0)
        {
            if (actionSpec.BranchSizes == null || actionSpec.BranchSizes.Length == 0)
            {
                Debug.LogError($"[MyCarAgent] 离散动作空间分支大小未配置！\n" +
                             $"请在BehaviorParameters中设置Branch 0 Size为11");
            }
            else
            {
                int branchSize = actionSpec.BranchSizes[0];
                if (branchSize != 11)
                {
                    Debug.LogError($"[MyCarAgent] 离散动作空间分支大小不匹配！当前={branchSize}，期望=11\n" +
                                 $"请在BehaviorParameters中设置Branch 0 Size为11");
                }
                else
                {
                    Debug.Log($"[MyCarAgent] 离散动作空间配置正确：11个动作，观测空间：10维");
                }
            }
        }
        
        // 检查连续动作空间（应该为0）
        if (actionSpec.NumContinuousActions > 0)
        {
            Debug.LogWarning($"[MyCarAgent] 检测到连续动作空间配置（{actionSpec.NumContinuousActions}），但系统使用离散动作空间。\n" +
                           $"建议在BehaviorParameters中将Continuous Actions设置为0");
        }
        
        // 检查观测空间
        int expectedObsSize = 10;  // 6个传感器 + 1个上次输出状态 + 3个实际运动状态
        int actualObsSize = behaviorParams.BrainParameters.VectorObservationSize;
        if (actualObsSize != expectedObsSize)
        {
            Debug.LogWarning($"[MyCarAgent] 观测空间大小不匹配！当前={actualObsSize}，期望={expectedObsSize}\n" +
                           $"请在BehaviorParameters中设置Vector Observation Size为{expectedObsSize}");
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
        int spawnIndex = 0;
        if (spawnPositions != null && spawnPositions.Length > 0)
        {
            if (spawnRng == null)
            {
                spawnRng = new System.Random(System.Environment.TickCount ^ GetInstanceID());
            }
            spawnIndex = spawnRng.Next(0, spawnPositions.Length);
        }
        if (enableDebugLog)
        {
            Debug.Log($"[Spawn] spawnPositions.Length={spawnPositions.Length}, spawnIndex={spawnIndex}");
        }
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
        IsAligned = false;
        isStableAligned = false;
        lastOutputLateralSpeed = 0f;
        lastOutputAngularSpeed = 0f;
        smoothedLateralSpeed = 0f;  // 初始化平滑缓冲
        smoothedAngularSpeed = 0f;  // 初始化平滑缓冲
        lastDiscreteAction = (int)DiscreteAction.Forward;  // 初始化离散动作为Forward(5)
        prevDiscreteAction = (int)DiscreteAction.Forward;  // 初始化前一帧离散动作为Forward(5)
        
        // 重置奖励跟踪
        if (enableRewardTracking)
        {
            cumulativeReward = 0f;
            rewardHistoryIndex = 0;
            if (rewardHistory != null)
            {
                System.Array.Clear(rewardHistory, 0, rewardHistory.Length);
            }
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 观测空间：10维
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

        // 7: 上次输出状态（离散动作索引归一化到0-1范围）
        // 将离散动作索引（0-10）归一化到0-1范围
        float lastActionState = lastDiscreteAction / 10f;  // 0/10=0, 10/10=1
        sensor.AddObservation(lastActionState);

        // 8-10: 车身实际运动状态（物理反馈）
        Vector3 localVel = transform.InverseTransformDirection(rb != null ? rb.linearVelocity : Vector3.zero);
        float angularVel = rb != null ? rb.angularVelocity.y : 0f;
        float maxOmegaRad = maxOmegaDeg * Mathf.Deg2Rad;
        
        sensor.AddObservation(Mathf.Clamp(localVel.z / Mathf.Max(0.001f, constantForwardSpeed), -2f, 2f));  // 8: 实际前进速度
        sensor.AddObservation(Mathf.Clamp(localVel.x / Mathf.Max(0.001f, maxLateralSpeed), -2f, 2f));      // 9: 实际横向速度
        sensor.AddObservation(Mathf.Clamp(angularVel / maxOmegaRad, -2f, 2f));                              // 10: 实际角速度
    }

    public override void OnActionReceived(ActionBuffers actions)
    { 
        float a_x, a_w;
        int discreteAction = 0;
        
        // 强制使用离散动作空间（方案C）
        // 从离散动作空间读取动作（0-10）
        discreteAction = actions.DiscreteActions[0];
        
        // 边界检查：确保动作索引在有效范围内（0-10）
        discreteAction = Mathf.Clamp(discreteAction, 0, 10);
        
        // 将离散动作映射到连续输出
        MapDiscreteActionToContinuous(discreteAction, out a_x, out a_w);
        
        // 注意：lastDiscreteAction 将在计算奖励后更新，用于下一帧观察

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

        // ========== 终止条件1：脱轨检测（立即判定） ==========
        float frontCenter = sensorValues[1];  // 前中
        float rearCenter = sensorValues[4];   // 后中
        float derailThresholdValue = maxField * 0.18f;  // 18%最大磁场强度（下调）
        float warningUpperThresholdValue = maxField * warningUpperThresholdPercent;  // 25%上限
        
        // 取两个中心传感器的最小值
        float minCenter = Mathf.Min(frontCenter, rearCenter);
        
        // ========== 脱轨检测：18%以下立即终止 ==========
        if (minCenter < derailThresholdValue)
        {
            // 中心传感器低于18% → 立即脱轨，无时间缓冲
            AddReward(derailPenalty);
            
            // 记录脱轨惩罚（用于显示）
            if (enableRewardTracking)
            {
                // 设置脱轨惩罚的奖励组成部分
                currentRewardComponents = new RewardComponents
                {
                    alignmentReward = 0f,
                    speedCoefficient = 0f,
                    trendConsistencyReward = 0f,
                    straightOutputPenalty = 0f,
                    straightOutputPenaltyPercent = 0f,
                    totalReward = derailPenalty,
                    rewardThisFrame = derailPenalty
                };
                
                cumulativeReward += derailPenalty;
                if (rewardHistory != null)
                {
                    rewardHistory[rewardHistoryIndex] = derailPenalty;
                    rewardHistoryIndex = (rewardHistoryIndex + 1) % rewardHistoryLength;
                }
            }
            
            if (enableDebugLog)
            {
                Debug.Log($"Episode Ended: derailment (immediate). frontCenter={frontCenter:F4}, rearCenter={rearCenter:F4}, threshold={derailThresholdValue:F4}");
            }
            EndEpisode();
            return;
        }
        
        // ========== 预警区域惩罚：18%~25%之间线性惩罚 ==========
        if (minCenter < warningUpperThresholdValue && minCenter >= derailThresholdValue)
        {
            // 计算危险比例：0（在25%时）到1（在18%时）
            float dangerRatio = 1f - (minCenter - derailThresholdValue) / (warningUpperThresholdValue - derailThresholdValue);
            // 线性惩罚：越接近18%，惩罚越大
            float warningPenalty = warningPenaltyCoefficient * dangerRatio * Time.fixedDeltaTime;
            AddReward(warningPenalty);
            
            // 记录预警惩罚（用于显示）
            if (enableRewardTracking)
            {
                cumulativeReward += warningPenalty;
                if (rewardHistory != null)
                {
                    rewardHistory[rewardHistoryIndex] = warningPenalty;
                    rewardHistoryIndex = (rewardHistoryIndex + 1) % rewardHistoryLength;
                }
            }
            
            if (enableDebugLog)
            {
                Debug.Log($"[Warning] 预警区域！minCenter={minCenter:F4} ({minCenter/maxField*100:F1}%), dangerRatio={dangerRatio:F3}, penalty={warningPenalty:F4}");
            }
        }
        // 25%以上：不惩罚不奖励（正常状态，继续正常奖励计算）

        // ========== 计算奖励 ==========
        // 使用当前对齐状态和动作计算本帧奖励
        float reward = CalculateReward(sensorValues, isAligned, isStableAligned, discreteAction, outputVx, outputOmega);
        float rewardThisFrame = reward * Time.fixedDeltaTime;
        AddReward(rewardThisFrame);
        
        // 更新动作记忆（用于下一帧的计算）
        prevDiscreteAction = lastDiscreteAction;  // 保存上一帧动作，供趋势奖励中的异号判定使用
        lastDiscreteAction = discreteAction;      // 保存当前帧的动作，供下一帧观察使用
        
        // 记录奖励（用于显示）
        if (enableRewardTracking)
        {
            // 更新奖励组成部分中的本帧奖励值
            currentRewardComponents.rewardThisFrame = rewardThisFrame;
            
            cumulativeReward += rewardThisFrame;
            if (rewardHistory != null)
            {
                rewardHistory[rewardHistoryIndex] = rewardThisFrame;
                rewardHistoryIndex = (rewardHistoryIndex + 1) % rewardHistoryLength;
            }
        }

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

    /// <summary>
    /// 将离散动作映射到连续输出值（扩展版：11个动作）
    /// </summary>
    /// <param name="action">离散动作索引（0-10）</param>
    /// <param name="a_x">输出的横向速度比例（-1~1）</param>
    /// <param name="a_w">输出的角速度比例（-1~1）</param>
    void MapDiscreteActionToContinuous(int action, out float a_x, out float a_w)
    {
        a_x = 0f;  // 所有动作的横向速度都为0
        a_w = 0f;  // 默认角速度为0
        
        switch ((DiscreteAction)action)
        {
            case DiscreteAction.TurnLeftSharp:
                // 急左转：vx=0, omega=+急转固定值
                a_x = 0f;
                a_w = turnSharpAngularValue;
                break;
                
            case DiscreteAction.TurnLeftLarge:
                // 大左转：vx=0, omega=+大转固定值
                a_x = 0f;
                a_w = turnLargeAngularValue;
                break;
                
            case DiscreteAction.TurnLeft:
                // 左转：vx=0, omega=+固定值
                a_x = 0f;
                a_w = turnAngularValue;
                break;
                
            case DiscreteAction.TurnLeftSmall:
                // 小左转：vx=0, omega=+小转固定值
                a_x = 0f; 
                a_w = turnSmallAngularValue;
                break;
                
            case DiscreteAction.TurnLeftMicro:
                // 微左转：vx=0, omega=+微转固定值
                a_x = 0f;
                a_w = turnMicroAngularValue;
                break;
                
            case DiscreteAction.Forward:
                // 直行：vx=0, omega=0
                a_x = 0f;
                a_w = 0f;
                break;
                
            case DiscreteAction.TurnRightMicro:
                // 微右转：vx=0, omega=-微转固定值
                a_x = 0f;
                a_w = -turnMicroAngularValue;
                break;
                
            case DiscreteAction.TurnRightSmall:
                // 小右转：vx=0, omega=-小转固定值
                a_x = 0f;
                a_w = -turnSmallAngularValue;
                break;
                
            case DiscreteAction.TurnRight:
                // 右转：vx=0, omega=-固定值
                a_x = 0f;
                a_w = -turnAngularValue;
                break;
                
            case DiscreteAction.TurnRightLarge:
                // 大右转：vx=0, omega=-大转固定值
                a_x = 0f;
                a_w = -turnLargeAngularValue;
                break;
                
            case DiscreteAction.TurnRightSharp:
                // 急右转：vx=0, omega=-急转固定值
                a_x = 0f;
                a_w = -turnSharpAngularValue;
                break;
                
            default:
                // 未知动作，保持为0
                a_x = 0f;
                a_w = 0f;
                break;
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

    float CalculateReward(float[] s, bool isAligned, bool isStableAligned, int currentDiscreteAction, float outputVx, float outputOmega)
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
            
            // 综合对齐分数：对称性60% + 中心强度40%（支持转弯时的不完全对齐）
            alignmentReward = symmetry * 0.6f + centerStrength * 0.4f;
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
            // 速度 < 20%预设速度：返回-2.0（允许转弯减速）
            // 保存速度过低的奖励组成部分
            if (enableRewardTracking)
            {
                currentRewardComponents = new RewardComponents
                {
                    alignmentReward = 0f,
                    speedCoefficient = 0f,
                    trendConsistencyReward = 0f,
                    straightOutputPenalty = 0f,
                    straightOutputPenaltyPercent = 0f,
                    totalReward = -2.0f,
                    rewardThisFrame = -2.0f * Time.fixedDeltaTime
                };
            }
            return -2.0f;
        }


        // ========== 4. 趋势一致性奖励：仅保留误差映射目标动作 ==========
        float trendConsistencyReward = 0f;
        if (enableTrendConsistencyReward && !isAligned)
        {
            // 基于传感器左右误差构造目标动作，惩罚偏离
            float frontErrorNorm = Mathf.Clamp((s[2] - s[0]) / Mathf.Max(1e-6f, maxField), -1f, 1f);
            float rearErrorNorm = Mathf.Clamp((s[5] - s[3]) / Mathf.Max(1e-6f, maxField), -1f, 1f);
            float turnErrorNorm = Mathf.Clamp(frontErrorNorm * 0.6f + rearErrorNorm * 0.4f, -1f, 1f);
            int targetAction = Mathf.Clamp(
                Mathf.RoundToInt((int)DiscreteAction.Forward + turnErrorNorm * targetActionGain),
                0,
                10);

            int actionTargetDiff = Mathf.Abs(currentDiscreteAction - targetAction);

            if (actionTargetDiff == 1)
            {
                trendConsistencyReward += targetTrackingCloseBonus*0.5;
            }
			else if (actionTargetDiff == 0)
			{
                trendConsistencyReward += targetTrackingCloseBonus;
			}
			else
			{
				trendConsistencyReward -= targetTrackingPenaltyWeight * actionTargetDiff;
			}
        }
        
        // ========== 5. 对齐条件下的动作限制惩罚 ==========
        // 在对齐条件下：
        // - 微调动作（TurnLeftMicro/TurnRightMicro）：不奖励也不惩罚
        // - 直行动作（Forward）：给予小奖励
        // - 其他转弯动作：根据剧烈程度惩罚
        float straightOutputPenalty = 0f;
        float straightOutputPenaltyPercent = 0f;  // 惩罚百分比（0-100%）
        
        if (isAligned)
        {
            // 检查动作类型
            bool isMicroAction = (currentDiscreteAction == (int)DiscreteAction.TurnLeftMicro) ||
                                (currentDiscreteAction == (int)DiscreteAction.TurnRightMicro);
            bool isForwardAction = (currentDiscreteAction == (int)DiscreteAction.Forward);
            
            if (isForwardAction)
            {
                // 直行动作：给予小奖励
                straightOutputPenalty = forwardReward;
                straightOutputPenaltyPercent = 0f;  // 奖励时百分比为0
            }
            else if (isMicroAction)
            {
                // 微调动作：不奖励也不惩罚
                straightOutputPenalty = 0f;
                straightOutputPenaltyPercent = 0f;
            }
            else
            {
                // 其他转弯动作：根据剧烈程度计算惩罚
                float penaltyMultiplier = 1.0f;
                
                // 急转动作（索引0或10）：最剧烈
                if (currentDiscreteAction == (int)DiscreteAction.TurnLeftSharp || 
                    currentDiscreteAction == (int)DiscreteAction.TurnRightSharp)
                {
                    penaltyMultiplier = sharpTurnPenaltyMultiplier;
                }
                // 大转动作（索引1或9）：次剧烈
                else if (currentDiscreteAction == (int)DiscreteAction.TurnLeftLarge || 
                         currentDiscreteAction == (int)DiscreteAction.TurnRightLarge)
                {
                    penaltyMultiplier = normalTurnPenaltyMultiplier * 1.2f;  // 比普通转弯稍高
                }
                // 普通转弯动作（索引2或8）：中等
                else if (currentDiscreteAction == (int)DiscreteAction.TurnLeft || 
                         currentDiscreteAction == (int)DiscreteAction.TurnRight)
                {
                    penaltyMultiplier = normalTurnPenaltyMultiplier;
                }
                // 小转动作（索引3或7）：轻微
                else
                {
                    penaltyMultiplier = normalTurnPenaltyMultiplier * 0.5f;  // 小转惩罚较轻
                }
                
                // 计算惩罚值：基础惩罚 × 倍数
                float penaltyValue = alignedActionPenalty * penaltyMultiplier;
                straightOutputPenalty -= penaltyValue;
                
                // 计算惩罚百分比（基于动作索引距离中心的距离）
                // 中心是Forward(5)，距离越远惩罚百分比越高
                float distanceFromCenter = Mathf.Abs(currentDiscreteAction - 5);
                straightOutputPenaltyPercent = (distanceFromCenter / 5f) * 100f;  // 最大100%（索引0或10）
            }
        }

        // ========== 6. 最终奖励 ==========
        // 对齐奖励 × 速度系数：基础轨迹跟踪奖励
        // + 趋势一致性奖励：基于误差映射目标动作，惩罚偏离
        // + 直线输出限制：在对齐条件下，直行奖励，微调不奖励不惩罚，其他动作惩罚
        float totalReward = alignmentReward * speedCoefficient
                           + trendConsistencyReward
                           + straightOutputPenalty;
        
        // 保存奖励组成部分（用于详细显示）
        if (enableRewardTracking)
        {
            currentRewardComponents = new RewardComponents
            {
                alignmentReward = alignmentReward,
                speedCoefficient = speedCoefficient,
                trendConsistencyReward = trendConsistencyReward,
                straightOutputPenalty = straightOutputPenalty,
                straightOutputPenaltyPercent = straightOutputPenaltyPercent,
                totalReward = totalReward,
                rewardThisFrame = 0f  // 将在调用处设置
            };
        }
        
        return totalReward;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // 不需要手动控制
    }
}