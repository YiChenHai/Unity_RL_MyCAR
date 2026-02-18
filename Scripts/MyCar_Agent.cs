using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

/// <summary>
/// 离散动作空间定义（方案C：混合方案）
/// 动作索引顺序：急左转 → 左转 → 微调左 → 前进 → 微调右 → 右转 → 急右转
/// </summary>
public enum DiscreteAction
{
    TurnLeftSharp = 0,     // 急左转：vx=0, omega=+大固定值
    TurnLeft = 1,          // 左转：vx=0, omega=+固定值
    AdjustLeft = 2,        // 微调左：vx=0, omega=+小增量
    Forward = 3,           // 前进：vx=0, omega=0
    AdjustRight = 4,       // 微调右：vx=0, omega=-小增量
    TurnRight = 5,         // 右转：vx=0, omega=-固定值
    TurnRightSharp = 6     // 急右转：vx=0, omega=-大固定值
}

public class MyCarAgent : Agent
{
    [Header("Data Collection for Distillation")]
    public bool enableDataCollection = false;  // 启用数据收集模式
    [Tooltip("是否记录脱轨终止的回合数据\nfalse=不记录脱轨终止的回合（推荐，避免学习错误行为）\ntrue=记录所有回合，包括脱轨终止的回合")]
    public bool recordDerailmentEpisodes = false;  // 是否记录脱轨终止的回合
    [Tooltip("是否启用记录条数限制")]
    public bool enableMaxSamplesLimit = false;  // 是否启用最大记录条数限制
    [Tooltip("目标采集数量（仅在启用限制时有效）\n到达目标后停止写入")]
    [Range(1, 1000000)]
    public int maxDataSamples = 300000;         // 最大记录条数
    [Tooltip("CSV文件保存文件夹路径（留空则使用默认路径）\n例如: D:/Data/ 或 D:/XiaoYiFei/Project/Unity/XYF_Car_Test/Data_Record/\n留空时使用: Application.persistentDataPath\n文件会自动以日期命名：training_data_yyyyMMdd_HHmmss.csv")]
    public string customSavePath = "";         // 用户指定的保存文件夹路径
    private List<string> collectedData = new List<string>();  // CSV格式: obs0,obs1,...,obs12,action0,action1
    private int episodeDataCount = 0;          // 当前回合收集的数据数量
    private int totalCollectedSamples = 0;     // 已累计写入的总样本数（跨回合）
    private string episodeDataFilePath = null;  // 使用的文件路径
    private bool episodeDataHeaderWritten = false;  // 是否已写入CSV头部
    private bool episodeEndedByDerailment = false;  // 标记当前回合是否因脱轨终止
    
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
    public float maxLateralSpeed = 0.15f;       // vx (横向速度) m/s
    public float maxOmegaDeg = 80f;            // omega (自转角速度) deg/s - 防止轮子翻转
    
    [Header("Discrete Action Space (方案C: 混合方案)")]
    [Tooltip("【已强制启用】使用离散动作空间（方案C，7个动作）\n\n【重要】必须在BehaviorParameters中配置：\n- Space Size: 1\n- Branch 0 Size: 7\n- Continuous Actions: 0\n- Vector Observation Size: 10")]
    [System.Obsolete("此参数已废弃，系统强制使用离散动作空间")]
    public bool useDiscreteActions = true;  // 已强制启用离散动作空间
    [Tooltip("普通左转/右转的角速度值（归一化，-1~1）\n例如：0.4 表示使用最大角速度的40%")]
    [Range(0f, 1f)]
    public float turnAngularValue = 0.25f;  // 普通转弯的角速度值
    [Tooltip("急左转/急右转的角速度值（归一化，-1~1）\n例如：0.7 表示使用最大角速度的70%")]
    [Range(0f, 1f)]
    public float turnSharpAngularValue = 0.5625f;  // 急转弯的角速度值
    [Tooltip("微调左/微调右的角速度增量（归一化，-1~1）\n例如：0.1 表示在当前角速度基础上增加/减少10%的最大角速度\n注意：微调动作是增量式的，会基于当前角速度进行调整")]
    [Range(0f, 1f)]
    public float adjustAngularIncrement = 0.0625f;  // 微调的角速度增量

    [Header("Normalization")]
    public float maxField = 8f;                // 磁场最大值

    [Header("Episode")]
    public float maxEpisodeTime = 40f;
    private float episodeTimer = 0f;

    // ========== 奖励参数配置（按奖励项分组） ==========
    
    [Header("1. 基础对齐奖励 (Alignment Reward)")]
    [Tooltip("对齐状态：左右差值阈值（15%，放宽）")]
    public float alignedThresholdPercent = 0.1f;
    [Tooltip("对齐状态：中心传感器阈值（45%，放宽支持转弯）")]
    public float centerThresholdPercent = 0.65f;
    [Tooltip("对齐状态的额外奖励")]
    public float alignedBonus = 0.5f;

    [Header("2. 速度系数 (Speed Coefficient)")]
    [Tooltip("速度比例系数为1的阈值（60%）\n当速度 >= 此阈值时，速度系数 = 1.0（全额奖励）")]
    public float speedHighPercent = 0.6f;
    [Tooltip("速度惩罚阈值（20%）\n当速度 < 此阈值时，直接返回 -2.0（大惩罚，终止奖励计算）\n当速度在此阈值和speedHighPercent之间时，速度系数线性插值（0 → 1）")]
    public float speedLowPercent = 0.20f;
    [Tooltip("已删除: 速度过低时的惩罚值，不再使用")]
    [System.Obsolete("此参数已删除")]
    public float speedPenalty = -0.2f;

    [Header("3. 平稳性奖励 (Smoothness Reward)")]
    [Tooltip("当离散动作索引差值大于4时的惩罚值\n差值=|当前动作索引 - 上一动作索引|\n例如：从Forward(3)跳到TurnLeft(1)，差值为2，不惩罚\n从TurnLeftSharp(0)跳到TurnRightSharp(6)，差值为6，惩罚")]
    public float actionJumpPenalty = -1.0f;

    [Header("4. 小输出奖励 (Small Output Bonus) - [已删除]")]
    [Tooltip("已删除: 小输出奖励已移除，改用直线输出限制惩罚")]
    [System.Obsolete("此奖励项已删除")]
    public float smallOutputBonus = 1.0f;
    [Tooltip("已删除: 横向速度与角速度的配平系数，不再使用")]
    [System.Obsolete("此参数已删除")]
    [Range(0.1f, 2.0f)]
    public float lateralAngularEquivalenceFactor = 0.562f;

    [Header("5. 转弯奖励 (Turning Reward) - [已删除]")]
    [Tooltip("已删除: 转弯奖励已移除")]
    [System.Obsolete("转弯奖励已删除")]
    public bool enableTurningReward = false;
    [Tooltip("已删除: 转弯鼓励奖励幅度")]
    [System.Obsolete("转弯奖励已删除")]
    public float turningBonus = 0.3f;
    [Tooltip("已删除: 触发转弯奖励的角速度阈值")]
    [System.Obsolete("转弯奖励已删除")]
    public float turningThreshold = 0.3f;

    [Header("6. 脱轨/预警惩罚 (Derailment/Warning Penalty)")]
    [Tooltip("脱轨惩罚（负数），脱轨时立即终止回合")]
    public float derailPenalty = -15.0f;
    [Tooltip("预警区域上限（25%，超过此值不惩罚不奖励）")]
    public float warningUpperThresholdPercent = 0.3f;
    [Tooltip("预警惩罚系数（每帧，18%~25%之间的线性惩罚）")]
    public float warningPenaltyCoefficient = -4.0f;

    [Header("7. 直线输出限制 (Straight Output Constraints)")]
    [Tooltip("在对齐条件下，输出微调和前进以外的动作时的惩罚权重\n允许的动作：AdjustLeft(2), Forward(3), AdjustRight(4)\n不允许的动作：TurnLeftSharp(0), TurnLeft(1), TurnRight(5), TurnRightSharp(6)")]
    public float alignedActionPenalty = 1.5f;
    [Tooltip("急转动作的惩罚倍数（TurnLeftSharp/TurnRightSharp，索引0/6）")]
    public float sharpTurnPenaltyMultiplier = 2.0f;
    [Tooltip("普通转弯动作的惩罚倍数（TurnLeft/TurnRight，索引1/5）")]
    public float normalTurnPenaltyMultiplier = 1.5f;

    [Header("Reward Tracking (for Display)")]
    [Tooltip("是否启用奖励跟踪（用于UI显示）")]
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
        public float smoothnessReward;     // 平稳性奖励
        public float turningReward;        // 转弯奖励
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
    [Tooltip("是否启用脱轨日志记录（记录到文件）")]
    public bool enableDerailmentLogging = true;  // 脱轨日志开关
    [Tooltip("脱轨日志文件保存路径（留空则使用默认路径）")]
    public string derailmentLogPath = "";  // 脱轨日志文件路径
    private string derailmentLogFilePath = null;  // 实际使用的日志文件路径
    private int derailmentCount = 0;  // 脱轨次数计数器

    [Header("Stable Tracking")]
    public float stableAlignedTime = 0.3f;     // 稳定对齐时间阈值（秒）
    private float alignedTimer = 0f;           // 对齐状态计时器
    private bool isStableAligned = false;      // 是否处于稳定对齐状态
    
    // 公开对齐状态供外部访问（如UI显示）
    public bool IsAligned { get; private set; }          // 当前是否对齐
    public bool IsStableAligned => isStableAligned;      // 当前是否稳定对齐
    public float AlignedTimer => alignedTimer;           // 对齐计时器（秒）
    
    // 公开动作状态供外部访问（如UI显示）
    public int CurrentDiscreteAction => lastDiscreteAction;  // 当前输出的离散动作索引（0-6）
    public int PreviousDiscreteAction => prevDiscreteAction;  // 上一帧的离散动作索引（0-6）
    
    // 动作记忆（用于观察空间）
    private float lastOutputLateralSpeed = 0f;  // 上一次输出的横向速度
    private float lastOutputAngularSpeed = 0f;  // 上一次输出的自转速度
    private float prevLateralSpeed = 0f;        // 前一帧的横向速度（用于平稳性计算）
    private float prevAngularSpeed = 0f;        // 前一帧的角速度（用于平稳性计算）
    private float lastRawLateralAction = 0f;    // 上一帧的原始神经网络输出（横向速度比例）
    private float lastRawAngularAction = 0f;    // 上一帧的原始神经网络输出（角速度比例）
    private int lastDiscreteAction = 0;         // 上一帧的离散动作索引（用于观察空间）
    private int prevDiscreteAction = 0;         // 前一帧的离散动作索引（用于平稳性奖励计算）
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
                         $"- Branch 0 Size: 7 (对应7个离散动作：TurnLeftSharp, TurnLeft, AdjustLeft, Forward, AdjustRight, TurnRight, TurnRightSharp)\n" +
                         $"- Continuous Actions: 0 (不使用连续动作)");
        }
        else if (numDiscreteActions > 0)
        {
            if (actionSpec.BranchSizes == null || actionSpec.BranchSizes.Length == 0)
            {
                Debug.LogError($"[MyCarAgent] 离散动作空间分支大小未配置！\n" +
                             $"请在BehaviorParameters中设置Branch 0 Size为7");
            }
            else
            {
                int branchSize = actionSpec.BranchSizes[0];
                if (branchSize != 7)
                {
                    Debug.LogError($"[MyCarAgent] 离散动作空间分支大小不匹配！当前={branchSize}，期望=7\n" +
                                 $"请在BehaviorParameters中设置Branch 0 Size为7");
                }
                else
                {
                    Debug.Log($"[MyCarAgent] 离散动作空间配置正确：7个动作，观测空间：10维");
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
        
        // ========== 初始化脱轨日志文件（如果启用） ==========
        if (enableDerailmentLogging && derailmentLogFilePath == null)
        {
            InitializeDerailmentLogFile();
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
        isStableAligned = false;
        lastOutputLateralSpeed = 0f;
        lastOutputAngularSpeed = 0f;
        prevLateralSpeed = 0f;
        prevAngularSpeed = 0f;
        smoothedLateralSpeed = 0f;  // 初始化平滑缓冲
        smoothedAngularSpeed = 0f;  // 初始化平滑缓冲
        lastRawLateralAction = 0f;  // 初始化原始动作
        lastRawAngularAction = 0f;  // 初始化原始动作
        lastDiscreteAction = (int)DiscreteAction.Forward;  // 初始化离散动作为Forward(3)
        prevDiscreteAction = (int)DiscreteAction.Forward;  // 初始化前一帧离散动作为Forward(3)
        
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
        
        // ========== 数据收集：回合结束处理 ==========
        // 检查上一回合是否因脱轨终止（在OnEpisodeBegin时，上一回合已结束）
        bool shouldSaveLastEpisode = recordDerailmentEpisodes || !episodeEndedByDerailment;
        
        // 每回合结束时自动写入
        if (collectedData.Count > 0 && shouldSaveLastEpisode)
        {
            // 保存上一回合的数据到文件
            AppendEpisodeDataToFile();
            Debug.Log($"[DataCollection] 回合数据已保存，样本数: {collectedData.Count}");
        }
        else if (collectedData.Count > 0 && !shouldSaveLastEpisode)
        {
            // 因脱轨终止且不记录脱轨回合，跳过保存
            Debug.Log($"[DataCollection] 回合因脱轨终止，跳过保存（recordDerailmentEpisodes=false），样本数: {collectedData.Count}");
        }
        
        // 清理内存中的数据，准备新回合
        collectedData.Clear();
        episodeDataCount = 0;
        
        // 重置脱轨标志
        episodeEndedByDerailment = false;
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
        // 将离散动作索引（0-6）归一化到0-1范围
        float lastActionState = lastDiscreteAction / 6f;  // 0/6=0, 6/6=1
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
        // 从离散动作空间读取动作（0-6）
        discreteAction = actions.DiscreteActions[0];
        
        // 边界检查：确保动作索引在有效范围内（0-6）
        discreteAction = Mathf.Clamp(discreteAction, 0, 6);
        
        // 将离散动作映射到连续输出
        MapDiscreteActionToContinuous(discreteAction, out a_x, out a_w);
        
        // 注意：lastDiscreteAction 将在计算奖励后更新，用于下一帧观察

        // ========== 数据收集（如果启用） ==========
        if (enableDataCollection)
        {
            // 检查是否达到目标数量
            if (enableMaxSamplesLimit && totalCollectedSamples >= maxDataSamples)
            {
                enableDataCollection = false;
                Debug.Log($"[DataCollection] Reached target samples ({maxDataSamples}), stop collecting.");
            }
            else if (enableMaxSamplesLimit)
            {
                // 检查剩余可采集数量
                int remaining = maxDataSamples - totalCollectedSamples - collectedData.Count;
                if (remaining <= 0)
                {
                    enableDataCollection = false;
                    Debug.Log($"[DataCollection] Reached target samples ({maxDataSamples}), stop collecting.");
                }
                else
                {
                    RecordSample(discreteAction);
                    episodeDataCount++;  // 记录当前回合的数据数量
                }
            }
            else
            {
                // 无限制，正常采集
                RecordSample(discreteAction);
                episodeDataCount++;  // 记录当前回合的数据数量
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
                    smoothnessReward = 0f,
                    turningReward = 0f,
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
            
            // ========== 标记为脱轨终止 ==========
            episodeEndedByDerailment = true;
            
            // ========== 记录脱轨信息 ==========
            RecordDerailment(sensorValues, frontCenter, rearCenter, derailThresholdValue, discreteAction, outputVx, outputOmega);
            
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
        // 使用 prevDiscreteAction（上一帧的）和 discreteAction（当前帧的）计算平稳性奖励
        float reward = CalculateReward(sensorValues, isAligned, isStableAligned, discreteAction, outputVx, outputOmega);
        float rewardThisFrame = reward * Time.fixedDeltaTime;
        AddReward(rewardThisFrame);
        
        // 更新动作记忆（用于下一帧的计算）
        prevDiscreteAction = lastDiscreteAction;  // 保存上一帧的动作，供下一帧平稳性计算使用
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
            // 超时终止，不是脱轨终止
            episodeEndedByDerailment = false;
            
            if (enableDebugLog)
            {
                Debug.Log($"Episode Ended: timeout. episodeTimer={episodeTimer:F2}s");
            }
            EndEpisode();
        }  
    }

    /// <summary>
    /// 将离散动作映射到连续输出值（方案C）
    /// </summary>
    /// <param name="action">离散动作索引（0-6）</param>
    /// <param name="a_x">输出的横向速度比例（-1~1）</param>
    /// <param name="a_w">输出的角速度比例（-1~1）</param>
    void MapDiscreteActionToContinuous(int action, out float a_x, out float a_w)
    {
        a_x = 0f;  // 所有动作的横向速度都为0
        a_w = 0f;  // 默认角速度为0
        
        switch ((DiscreteAction)action)
        {
            case DiscreteAction.TurnLeftSharp:
                // 急左转：vx=0, omega=+大固定值
                a_x = 0f;
                a_w = turnSharpAngularValue;
                break;
                
            case DiscreteAction.TurnLeft:
                // 左转：vx=0, omega=+固定值
                a_x = 0f;
                a_w = turnAngularValue;
                break;
                
            case DiscreteAction.AdjustLeft:
                // 微调左：vx=0, omega=+小增量（基于当前角速度）
                a_x = 0f;
                // 增量式：在当前角速度基础上增加
                float currentAngularNorm = lastOutputAngularSpeed / (maxOmegaDeg * Mathf.Deg2Rad);
                a_w = Mathf.Clamp(currentAngularNorm + adjustAngularIncrement, -1f, 1f);
                break;
                
            case DiscreteAction.Forward:
                // 前进：vx=0, omega=0
                a_x = 0f;
                a_w = 0f;
                break;
                
            case DiscreteAction.AdjustRight:
                // 微调右：vx=0, omega=-小增量（基于当前角速度）
                a_x = 0f;
                // 增量式：在当前角速度基础上减少
                currentAngularNorm = lastOutputAngularSpeed / (maxOmegaDeg * Mathf.Deg2Rad);
                a_w = Mathf.Clamp(currentAngularNorm - adjustAngularIncrement, -1f, 1f);
                break;
                
            case DiscreteAction.TurnRight:
                // 右转：vx=0, omega=-固定值
                a_x = 0f;
                a_w = -turnAngularValue;
                break;
                
            case DiscreteAction.TurnRightSharp:
                // 急右转：vx=0, omega=-大固定值
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
                    smoothnessReward = 0f,
                    turningReward = 0f,
                    straightOutputPenalty = 0f,
                    straightOutputPenaltyPercent = 0f,
                    totalReward = -2.0f,
                    rewardThisFrame = -2.0f * Time.fixedDeltaTime
                };
            }
            return -2.0f;
        }


        // ========== 3. 平稳性奖励：基于离散动作索引差值 ==========
        // 计算当前动作和上一动作的索引差值
        float smoothnessReward = 0f;
        int actionIndexDiff = Mathf.Abs(currentDiscreteAction - prevDiscreteAction);
        
        // 如果索引差值大于4，则惩罚
        if (actionIndexDiff > 4)
        {
            smoothnessReward = actionJumpPenalty;
        }
        
        // ========== 4. 对齐条件下的动作限制惩罚 ==========
        // 在对齐条件下，只允许微调（AdjustLeft/AdjustRight）和前进（Forward）
        // 不允许急转和普通转弯动作
        float straightOutputPenalty = 0f;
        float straightOutputPenaltyPercent = 0f;  // 惩罚百分比（0-100%）
        
        if (isAligned)
        {
            // 允许的动作：AdjustLeft(2), Forward(3), AdjustRight(4)
            // 不允许的动作：TurnLeftSharp(0), TurnLeft(1), TurnRight(5), TurnRightSharp(6)
            bool isAllowedAction = (currentDiscreteAction == (int)DiscreteAction.AdjustLeft) ||
                                  (currentDiscreteAction == (int)DiscreteAction.Forward) ||
                                  (currentDiscreteAction == (int)DiscreteAction.AdjustRight);
            
            if (!isAllowedAction)
            {
                // 根据动作的剧烈程度计算惩罚
                float penaltyMultiplier = 1.0f;
                
                // 急转动作（索引0或6）：最剧烈
                if (currentDiscreteAction == (int)DiscreteAction.TurnLeftSharp || 
                    currentDiscreteAction == (int)DiscreteAction.TurnRightSharp)
                {
                    penaltyMultiplier = sharpTurnPenaltyMultiplier;
                }
                // 普通转弯动作（索引1或5）：次剧烈
                else if (currentDiscreteAction == (int)DiscreteAction.TurnLeft || 
                         currentDiscreteAction == (int)DiscreteAction.TurnRight)
                {
                    penaltyMultiplier = normalTurnPenaltyMultiplier;
                }
                
                // 计算惩罚值：基础惩罚 × 倍数
                float penaltyValue = alignedActionPenalty * penaltyMultiplier;
                straightOutputPenalty -= penaltyValue;
                
                // 计算惩罚百分比（基于动作索引距离中心的距离）
                // 中心是Forward(3)，距离越远惩罚百分比越高
                float distanceFromCenter = Mathf.Abs(currentDiscreteAction - 3);
                straightOutputPenaltyPercent = (distanceFromCenter / 3f) * 100f;  // 最大100%（索引0或6）
            }
        }

        // ========== 5. 最终奖励 ==========
        // 对齐奖励 × 速度系数：基础轨迹跟踪奖励
        // + 平稳性奖励：惩罚动作索引差值大于4的情况
        // + 直线输出限制：在对齐条件下惩罚非微调/前进动作
        float totalReward = alignmentReward * speedCoefficient
                           + smoothnessReward
                           + straightOutputPenalty;
        
        // 保存奖励组成部分（用于详细显示）
        if (enableRewardTracking)
        {
            currentRewardComponents = new RewardComponents
            {
                alignmentReward = alignmentReward,
                speedCoefficient = speedCoefficient,
                smoothnessReward = smoothnessReward,
                turningReward = 0f,  // 已移除转弯奖励
                straightOutputPenalty = straightOutputPenalty,
                straightOutputPenaltyPercent = straightOutputPenaltyPercent,
                totalReward = totalReward,
                rewardThisFrame = 0f  // 将在调用处设置
            };
        }
        
        return totalReward;
    }

    // ========== 数据收集方法 ==========
    private void RecordSample(int discreteAction)
    {
        // 重新计算观测（与CollectObservations逻辑相同，10维）
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

        // 7: 上次输出状态（离散动作索引归一化到0-1范围）
        float lastActionState = lastDiscreteAction / 6f;
        observations.Add(lastActionState);

        // 8-10: 实际运动状态（物理反馈）
        Vector3 localVel = transform.InverseTransformDirection(rb != null ? rb.linearVelocity : Vector3.zero);
        float angularVel = rb != null ? rb.angularVelocity.y : 0f;
        float maxOmegaRad = maxOmegaDeg * Mathf.Deg2Rad;
        observations.Add(Mathf.Clamp(localVel.z / Mathf.Max(0.001f, constantForwardSpeed), -2f, 2f));  // 8: 实际前进速度
        observations.Add(Mathf.Clamp(localVel.x / Mathf.Max(0.001f, maxLateralSpeed), -2f, 2f));      // 9: 实际横向速度
        observations.Add(Mathf.Clamp(angularVel / maxOmegaRad, -2f, 2f));                              // 10: 实际角速度

        // 构建CSV行：obs[0-9],action[0]（离散动作索引）
        StringBuilder sb = new StringBuilder();
        foreach (float obs in observations)
        {
            sb.Append(obs.ToString("F6"));
            sb.Append(",");
        }
        sb.Append(discreteAction.ToString());  // 记录离散动作索引（0-6）
        
        collectedData.Add(sb.ToString());
    }

    // 获取保存目录路径（如果用户指定了路径则使用，否则使用默认路径）
    private string GetSaveDirectory()
    {
        if (!string.IsNullOrEmpty(customSavePath))
        {
            // customSavePath现在只接受目录路径，不再接受完整文件路径
            // 如果用户误输入了完整文件路径，提取目录部分
            if (Path.HasExtension(customSavePath))
            {
                string dir = Path.GetDirectoryName(customSavePath);
                if (!string.IsNullOrEmpty(dir))
                {
                    Debug.LogWarning($"[DataCollection] customSavePath应该是目录路径，检测到文件路径，已提取目录: {dir}");
                    return dir;
                }
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
        // 组合目录和文件名（使用日期命名的文件名）
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
            // 使用日期命名：training_data_yyyyMMdd_HHmmss.csv
            string fileName = $"training_data_{System.DateTime.Now:yyyyMMdd_HHmmss}.csv";
            episodeDataFilePath = GetFullFilePath(fileName);
            
            episodeDataHeaderWritten = false;
            
            // 确保目录存在
            string directory = GetSaveDirectory();
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
                // 观测空间：10维（sensor0-5, last_action_state, actual_vz, actual_vx, actual_omega）
                // 动作：离散动作索引（0-6）
                string header = "sensor0,sensor1,sensor2,sensor3,sensor4,sensor5," +
                               "last_action_state," +
                               "actual_vz,actual_vx,actual_omega," +
                               "discrete_action";
                File.WriteAllText(episodeDataFilePath, header + System.Environment.NewLine);
                episodeDataHeaderWritten = true;
            }

            // 计算本次可写入的数量（考虑目标上限）
            int writeCount = collectedData.Count;
            if (enableMaxSamplesLimit)
            {
                int remaining = maxDataSamples - totalCollectedSamples;
                if (remaining <= 0)
                {
                    Debug.Log($"[DataCollection] Target samples ({maxDataSamples}) reached, no more data will be written.");
                    enableDataCollection = false;
                    return;
                }
                if (remaining < writeCount)
                {
                    writeCount = remaining;
                    Debug.Log($"[DataCollection] Truncate episode data: write {writeCount}/{collectedData.Count} to reach target.");
                }
            }

            // 追加数据行
            using (StreamWriter writer = new StreamWriter(episodeDataFilePath, append: true))
            {
                for (int i = 0; i < writeCount; i++)
                {
                    writer.WriteLine(collectedData[i]);
                }
            }

            totalCollectedSamples += writeCount;
            Debug.Log($"[DataCollection] Appended {writeCount} samples from episode to:\n{episodeDataFilePath}");

            if (enableMaxSamplesLimit && totalCollectedSamples >= maxDataSamples)
            {
                enableDataCollection = false;
                Debug.Log($"[DataCollection] Reached target samples ({maxDataSamples}), collection stopped.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[DataCollection] Failed to append episode data: {e.Message}");
        }
    }

    // 导出收集的数据为CSV文件（手动导出使用，通常不需要，因为每回合自动写入）
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
        string directory = GetSaveDirectory();
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
            
            // 重置文件状态
            episodeDataFilePath = null;
            episodeDataHeaderWritten = false;
            
            Debug.Log($"[DataCollection] Cleared {count} samples from memory.");
        }

    // 获取当前收集的数据数量
    public int GetCollectedDataCount()
    {
        return collectedData.Count;
    }

    // 重置文件（开始新的文件）
    public void ResetEpisodeDataFile()
    {
        episodeDataFilePath = null;
        episodeDataHeaderWritten = false;
        Debug.Log("[DataCollection] Reset episode data file. Next episode will create a new file.");
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

    // ========== 脱轨日志记录方法 ==========
    /// <summary>
    /// 初始化脱轨日志文件
    /// </summary>
    private void InitializeDerailmentLogFile()
    {
        try
        {
            // 确定日志文件路径
            if (!string.IsNullOrEmpty(derailmentLogPath))
            {
                if (Path.HasExtension(derailmentLogPath))
                {
                    // 用户指定了完整文件路径
                    derailmentLogFilePath = derailmentLogPath;
                }
                else
                {
                    // 用户指定了目录路径
                    string fileName = $"derailment_log_{System.DateTime.Now:yyyyMMdd_HHmmss}.csv";
                    derailmentLogFilePath = Path.Combine(derailmentLogPath, fileName);
                }
            }
            else
            {
                // 使用默认路径
                string fileName = $"derailment_log_{System.DateTime.Now:yyyyMMdd_HHmmss}.csv";
                derailmentLogFilePath = Path.Combine(Application.persistentDataPath, fileName);
            }
            
            // 确保目录存在
            string directory = Path.GetDirectoryName(derailmentLogFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            // 写入CSV头部（如果文件不存在）
            if (!File.Exists(derailmentLogFilePath))
            {
                string header = "timestamp,episode_time,derailment_count," +
                               "position_x,position_y,position_z," +
                               "rotation_x,rotation_y,rotation_z," +
                               "velocity_x,velocity_y,velocity_z," +
                               "angular_velocity_y," +
                               "sensor0,sensor1,sensor2,sensor3,sensor4,sensor5," +
                               "front_center,rear_center,threshold," +
                               "discrete_action," +
                               "output_vx,output_omega," +
                               "is_aligned,is_stable_aligned";
                File.WriteAllText(derailmentLogFilePath, header + System.Environment.NewLine);
                Debug.Log($"[DerailmentLog] 脱轨日志文件已初始化: {derailmentLogFilePath}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[DerailmentLog] 初始化脱轨日志文件失败: {e.Message}");
            derailmentLogFilePath = null;
        }
    }
    
    /// <summary>
    /// 记录脱轨信息
    /// </summary>
    private void RecordDerailment(float[] sensorValues, float frontCenter, float rearCenter, 
                                  float threshold, int discreteAction, 
                                  float outputVx, float outputOmega)
    {
        if (!enableDerailmentLogging || derailmentLogFilePath == null)
        {
            return;
        }
        
        try
        {
            derailmentCount++;
            
            // 获取当前时间和位置信息
            string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            Vector3 position = transform.position;
            Vector3 rotation = transform.rotation.eulerAngles;
            Vector3 velocity = rb != null ? rb.linearVelocity : Vector3.zero;
            float angularVelocityY = rb != null ? rb.angularVelocity.y : 0f;
            
            // 构建CSV行
            StringBuilder sb = new StringBuilder();
            sb.Append(timestamp); sb.Append(",");
            sb.Append(episodeTimer.ToString("F4")); sb.Append(",");
            sb.Append(derailmentCount.ToString()); sb.Append(",");
            
            // 位置
            sb.Append(position.x.ToString("F6")); sb.Append(",");
            sb.Append(position.y.ToString("F6")); sb.Append(",");
            sb.Append(position.z.ToString("F6")); sb.Append(",");
            
            // 旋转
            sb.Append(rotation.x.ToString("F6")); sb.Append(",");
            sb.Append(rotation.y.ToString("F6")); sb.Append(",");
            sb.Append(rotation.z.ToString("F6")); sb.Append(",");
            
            // 速度
            sb.Append(velocity.x.ToString("F6")); sb.Append(",");
            sb.Append(velocity.y.ToString("F6")); sb.Append(",");
            sb.Append(velocity.z.ToString("F6")); sb.Append(",");
            
            // 角速度
            sb.Append(angularVelocityY.ToString("F6")); sb.Append(",");
            
            // 传感器值
            for (int i = 0; i < 6; i++)
            {
                sb.Append((sensorValues != null && i < sensorValues.Length ? sensorValues[i] : 0f).ToString("F6"));
                sb.Append(",");
            }
            
            // 脱轨检测相关
            sb.Append(frontCenter.ToString("F6")); sb.Append(",");
            sb.Append(rearCenter.ToString("F6")); sb.Append(",");
            sb.Append(threshold.ToString("F6")); sb.Append(",");
            
            // 离散动作
            sb.Append(discreteAction.ToString()); sb.Append(",");
            
            // 输出
            sb.Append(outputVx.ToString("F6")); sb.Append(",");
            sb.Append(outputOmega.ToString("F6")); sb.Append(",");
            
            // 对齐状态
            sb.Append(IsAligned ? "1" : "0"); sb.Append(",");
            sb.Append(isStableAligned ? "1" : "0");
            
            // 追加到日志文件
            File.AppendAllText(derailmentLogFilePath, sb.ToString() + System.Environment.NewLine);
            
            // 打印到控制台
            Debug.Log($"[DerailmentLog] 脱轨记录 #{derailmentCount} | " +
                     $"位置: ({position.x:F3}, {position.y:F3}, {position.z:F3}) | " +
                     $"时间: {episodeTimer:F2}s | " +
                     $"前中传感器: {frontCenter:F4} | " +
                     $"后中传感器: {rearCenter:F4} | " +
                     $"阈值: {threshold:F4}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[DerailmentLog] 记录脱轨信息失败: {e.Message}");
        }
    }
    
    /// <summary>
    /// 获取脱轨日志文件路径（用于调试或显示）
    /// </summary>
    public string GetDerailmentLogPath()
    {
        return derailmentLogFilePath ?? "未初始化";
    }
    
    /// <summary>
    /// 打印脱轨日志文件路径到控制台
    /// </summary>
    [ContextMenu("Print Derailment Log Path")]
    public void PrintDerailmentLogPath()
    {
        string message = $"[DerailmentLog] 脱轨日志文件路径:\n";
        if (derailmentLogFilePath != null)
        {
            message += $"{derailmentLogFilePath}\n";
            message += $"脱轨次数: {derailmentCount}";
        }
        else
        {
            message += "未初始化（请确保 enableDerailmentLogging = true）";
        }
        Debug.Log(message);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // 不需要手动控制
    }
}