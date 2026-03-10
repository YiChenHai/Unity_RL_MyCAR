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
    [Tooltip("是否记录脱轨终止的回合数据\nfalse=不记录脱轨终止的回合（推荐，避免学习错误行为）\ntrue=记录所有回合，包括脱轨终止的回合")]
    public bool recordDerailmentEpisodes = false;  // 是否记录脱轨终止的回合
    [Tooltip("是否启用记录条数限制")]
    public bool enableMaxSamplesLimit = false;  // 是否启用最大记录条数限制
    [Tooltip("目标采集数量（仅在启用限制时有效）\n到达目标后停止写入")]
    [Range(1, 1000000)]
    public int maxDataSamples = 300000;         // 最大记录条数
    [Tooltip("CSV文件保存文件夹路径（留空则使用默认路径）\n例如: D:/Data/ 或 D:/XiaoYiFei/Project/Unity/XYF_Car_Test/Data_Record/\n留空时使用: Application.persistentDataPath\n文件会自动以日期命名：training_data_yyyyMMdd_HHmmss.csv")]
    public string customSavePath = "";         // 用户指定的保存文件夹路径
    private List<string> collectedData = new List<string>();
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
    public float maxLateralSpeed = 0.2f;        // vx (横向速度) m/s
    public float maxOmegaDeg = 45f;            // omega (自转角速度) deg/s

    [Header("Normalization")]
    public float maxField = 8f;                // 磁场最大值

    [Header("Episode")]
    public float maxEpisodeTime = 40f;
    private float episodeTimer = 0f;

    // ========== 奖励参数配置（按奖励项分组） ==========
    
    [Header("1. 基础对齐奖励 (Alignment Reward)")]
    [Tooltip("对齐状态：前后左右差值阈值（占 maxField 的比例）")]
    public float alignedThresholdPercent = 0.1f;
    [Tooltip("对齐状态：中心传感器阈值（占 maxField 的比例）")]
    public float centerThresholdPercent = 0.65f;
    [Tooltip("对齐状态的额外奖励")]
    public float alignedBonus = 0.5f;

    [Header("2. 速度系数 (Speed Coefficient)")]
    [Tooltip("速度比例系数为1的阈值（60%）")]
    public float speedHighPercent = 0.6f;
    [Tooltip("速度惩罚阈值（20%），低于此值直接返回固定惩罚")]
    public float speedLowPercent = 0.20f;
    [Tooltip("速度过低时的固定惩罚值")]
    public float speedPenalty = -2.0f;

    [Header("3. 脱轨/预警惩罚 (Derailment/Warning Penalty)")]
    [Tooltip("脱轨惩罚（负数），脱轨时立即终止回合")]
    public float derailPenalty = -15.0f;
    [Tooltip("脱轨阈值（中心传感器低于此比例立即终止）")]
    public float derailThresholdPercent = 0.18f;
    [Tooltip("预警区域上限，超过此值不惩罚不奖励")]
    public float warningUpperThresholdPercent = 0.3f;
    [Tooltip("预警惩罚系数（每帧，脱轨阈值~预警上限之间的线性惩罚）")]
    public float warningPenaltyCoefficient = -4.0f;

    [Header("4. 直线输出限制 (Straight Output Constraints)")]
    [Tooltip("在【稳定对齐】直线跟踪时，对累积角速度输出的归一化幅度进行额外惩罚的权重")]
    public float alignedAngularPenalty = 1.5f;
    [Tooltip("在【稳定对齐】时，允许的角速度输出死区（归一化值）：小于此值不惩罚")]
    [Range(0f, 0.5f)]
    public float alignedAngularDeadZone = 0.035f;
    [Tooltip("在【稳定对齐】时，角速度输出达到最大惩罚的阈值（归一化值）")]
    [Range(0f, 1f)]
    public float alignedAngularMaxPenaltyThreshold = 0.3f;
    [Tooltip("在【稳定对齐】直线跟踪时，对累积横向速度输出的归一化幅度进行额外惩罚的权重")]
    public float alignedLateralPenalty = 1.5f;
    [Tooltip("在【稳定对齐】时，允许的横向速度输出死区（归一化值）：小于此值不惩罚")]
    [Range(0f, 0.5f)]
    public float alignedLateralDeadZone = 0.05f;
    [Tooltip("在【稳定对齐】时，横向速度输出达到最大惩罚的阈值（归一化值）")]
    [Range(0f, 1f)]
    public float alignedLateralMaxPenaltyThreshold = 0.3f;

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
    
    private System.Random spawnRng;             // 出生点随机数发生器（避免被Unity随机种子重置）
    
    [Header("Incremental Output")]
    [Tooltip("每步最大横向速度变化量 (m/s)，决定从零到满量程的响应速度")]
    public float maxDeltaLateralSpeed = 0.02f;
    [Tooltip("每步最大角速度变化量 (deg/s)，决定从零到满量程的响应速度")]
    public float maxDeltaOmegaDeg = 4.5f;
    [Tooltip("累积输出衰减系数（1.0=无衰减，<1.0=轻微向零回归，防止长期漂移）")]
    [Range(0.99f, 1.0f)]
    public float outputDecayFactor = 1.0f;
    private float accumulatedLateralSpeed = 0f;
    private float accumulatedAngularSpeed = 0f;

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
    private const float DefaultMaxOmegaDeg = 45;
	
    // 磁场强度归一化的分母（maxField）。观测中使用 mag.magnitude/maxField 归一化。
    private const float DefaultMaxField = 8f;
	
    // 单回合最大时长（秒）。超过则判定超时结束回合。
    private const float DefaultMaxEpisodeTime = 40f;

    // 每步最大横向速度变化量（m/s）。增量式输出的步长上限。
    private const float DefaultMaxDeltaLateralSpeed = 0.02f;

    // 每步最大角速度变化量（deg/s）。增量式输出的步长上限。
    private const float DefaultMaxDeltaOmegaDeg = 4.5f;
	
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
        maxDeltaLateralSpeed = DefaultMaxDeltaLateralSpeed;
        maxDeltaOmegaDeg = DefaultMaxDeltaOmegaDeg;
        startRot = Quaternion.Euler(0f, 0f, 0f);

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
        accumulatedLateralSpeed = 0f;
        accumulatedAngularSpeed = 0f;
        
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
        
        collectedData.Clear();
        
        // 重置脱轨标志
        episodeEndedByDerailment = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // obs[0-3]: 磁传感器派生特征（4维）
        float[] rawSensor = new float[6];
        for (int i = 0; i < sensors.Length; i++)
        {
            if (sensors[i] != null && tape != null)
                rawSensor[i] = tape.GetMagneticField(sensors[i].position).magnitude;
        }
        float invMax = 1f / Mathf.Max(1e-9f, maxField);
        sensor.AddObservation(Mathf.Clamp((rawSensor[0] - rawSensor[2]) * invMax, -1f, 1f)); // 前排左右差
        sensor.AddObservation(Mathf.Clamp((rawSensor[3] - rawSensor[5]) * invMax, -1f, 1f)); // 后排左右差
        sensor.AddObservation(Mathf.Clamp01(rawSensor[1] * invMax)); // 前中心
        sensor.AddObservation(Mathf.Clamp01(rawSensor[4] * invMax)); // 后中心

        // obs[4-5]: 当前累积输出状态（归一化到 [-1, 1]）
        float maxOmegaRad = maxOmegaDeg * Mathf.Deg2Rad;
        sensor.AddObservation(Mathf.Clamp(accumulatedLateralSpeed / Mathf.Max(0.001f, maxLateralSpeed), -1f, 1f));
        sensor.AddObservation(Mathf.Clamp(accumulatedAngularSpeed / Mathf.Max(0.001f, maxOmegaRad), -1f, 1f));

        // obs[6-8]: 车身实际运动状态（物理反馈）
        Vector3 localVel = transform.InverseTransformDirection(rb != null ? rb.linearVelocity : Vector3.zero);
        float angularVel = rb != null ? rb.angularVelocity.y : 0f;
        
        sensor.AddObservation(Mathf.Clamp(localVel.z / Mathf.Max(0.001f, constantForwardSpeed), -2f, 2f));
        sensor.AddObservation(Mathf.Clamp(localVel.x / Mathf.Max(0.001f, maxLateralSpeed), -2f, 2f));
        sensor.AddObservation(Mathf.Clamp(angularVel / maxOmegaRad, -2f, 2f));
    }

    public override void OnActionReceived(ActionBuffers actions)
    { 
        // 连续动作：0=横向速度增量比例，1=自转速度增量比例（增量式输出）
        float delta_x = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float delta_w = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

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
                    RecordSample(delta_x, delta_w);
                }
            }
            else
            {
                RecordSample(delta_x, delta_w);
            }
        }

        // ========== 增量式输出：累积器 ==========
        if (outputDecayFactor < 1f)
        {
            accumulatedLateralSpeed *= outputDecayFactor;
            accumulatedAngularSpeed *= outputDecayFactor;
        }
        
        float deltaLateral = delta_x * maxDeltaLateralSpeed;
        float deltaAngular = delta_w * maxDeltaOmegaDeg * Mathf.Deg2Rad;
        
        float maxOmegaRad = maxOmegaDeg * Mathf.Deg2Rad;
        accumulatedLateralSpeed = Mathf.Clamp(
            accumulatedLateralSpeed + deltaLateral, -maxLateralSpeed, maxLateralSpeed);
        accumulatedAngularSpeed = Mathf.Clamp(
            accumulatedAngularSpeed + deltaAngular, -maxOmegaRad, maxOmegaRad);

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
        float outputVx = accumulatedLateralSpeed;
        float outputOmega = accumulatedAngularSpeed;

        // 映射到真实控制量（vz固定）
        float vz = constantForwardSpeed;  // 固定前进速度

        // 下发给 MyCar_Motion 控制车辆
        if (myCarMotion != null) myCarMotion.SetControl(vz, outputVx, outputOmega);

        // ========== 终止条件1：脱轨检测（立即判定） ==========
        float frontCenter = sensorValues[1];  // 前中
        float rearCenter = sensorValues[4];   // 后中
        float derailThresholdValue = maxField * derailThresholdPercent;
        float warningUpperThresholdValue = maxField * warningUpperThresholdPercent;
        
        // 取两个中心传感器的最小值
        float minCenter = Mathf.Min(frontCenter, rearCenter);
        
        if (minCenter < derailThresholdValue)
        {
            AddReward(derailPenalty);
            
            // 记录脱轨惩罚（用于显示）
            if (enableRewardTracking)
            {
                // 设置脱轨惩罚的奖励组成部分
                currentRewardComponents = new RewardComponents
                {
                    alignmentReward = 0f,
                    speedCoefficient = 0f,
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
            RecordDerailment(sensorValues, frontCenter, rearCenter, derailThresholdValue, delta_x, delta_w, outputVx, outputOmega);
            
            if (enableDebugLog)
            {
                Debug.Log($"Episode Ended: derailment (immediate). frontCenter={frontCenter:F4}, rearCenter={rearCenter:F4}, threshold={derailThresholdValue:F4}");
            }
            EndEpisode();
            return;
        }
        
        if (minCenter < warningUpperThresholdValue && minCenter >= derailThresholdValue)
        {
            float dangerRatio = 1f - (minCenter - derailThresholdValue) / (warningUpperThresholdValue - derailThresholdValue);
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
        // 预警上限以上：正常奖励计算

        // ========== 计算奖励 ==========
        float reward = CalculateReward(sensorValues, isAligned, isStableAligned, outputVx, outputOmega);
        float rewardThisFrame = reward * Time.fixedDeltaTime;
        AddReward(rewardThisFrame);
        
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
        
        float diffThreshold = maxField * alignedThresholdPercent;
        float centerThreshold = maxField * centerThresholdPercent;
        
        bool leftRightAligned = (frontDiff < diffThreshold) && (rearDiff < diffThreshold);
        bool centerStrong = (frontCenter > centerThreshold) && (rearCenter > centerThreshold);
        
        return leftRightAligned && centerStrong;
    }

    float CalculateReward(float[] s, bool isAligned, bool isStableAligned, float outputVx, float outputOmega)
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
        float highSpeedThreshold = constantForwardSpeed * speedHighPercent;
        float lowSpeedThreshold = constantForwardSpeed * speedLowPercent;
        
        if (forwardSpeed >= highSpeedThreshold)
        {
            speedCoefficient = 1.0f;
        }
        else if (forwardSpeed >= lowSpeedThreshold)
        {
            speedCoefficient = (forwardSpeed - lowSpeedThreshold) / (highSpeedThreshold - lowSpeedThreshold);
        }
        else
        {
            if (enableRewardTracking)
            {
                currentRewardComponents = new RewardComponents
                {
                    alignmentReward = 0f,
                    speedCoefficient = 0f,
                    straightOutputPenalty = 0f,
                    straightOutputPenaltyPercent = 0f,
                    totalReward = speedPenalty,
                    rewardThisFrame = speedPenalty * Time.fixedDeltaTime
                };
            }
            return speedPenalty;
        }


        // ========== 3. 对齐直线阶段的输出限制（只在稳定对齐时生效）==========
        // 惩罚计算说明：
        // - 死区内（输出 < 死区值）：无惩罚（0%）
        // - 死区到最大值阈值之间：线性惩罚（0% → 100%）
        // - 超过最大值阈值：达到最大惩罚值（100%）
        // 
        // 惩罚计算公式：
        //   惩罚值 = Penalty × (输出值 - 死区) / (最大值阈值 - 死区)
        //   百分比 = (输出值 - 死区) / (最大值阈值 - 死区) × 100%
        // 
        // 当输出值 = 最大值阈值时，惩罚值 = Penalty，百分比 = 100%
        // 当输出值 = 死区时，惩罚值 = 0，百分比 = 0%
        // 
        // 默认参数下的最大惩罚值（当输出达到最大值阈值时）：
        //   角速度：alignedAngularPenalty = 1.5
        //   横向速度：alignedLateralPenalty = 1.5
        float straightOutputPenalty = 0f;
        float straightOutputPenaltyPercent = 0f;  // 惩罚百分比（0-100%）
        if (isStableAligned)
        {
            float angularPenaltyPercent = 0f;
            float lateralPenaltyPercent = 0f;
            
            // 6.1 角速度惩罚：使用累积输出的归一化幅度（0~1）
            float maxOmegaRadLocal2 = maxOmegaDeg * Mathf.Deg2Rad;
            float absAw = Mathf.Abs(outputOmega) / Mathf.Max(0.001f, maxOmegaRadLocal2);
            if (absAw > alignedAngularDeadZone && alignedAngularPenalty > 0f)
            {
                // 计算死区到最大值阈值之间的范围
                float penaltyRange = Mathf.Max(0.001f, alignedAngularMaxPenaltyThreshold - alignedAngularDeadZone);
                
                // 计算比例：(输出值 - 死区) / (最大值阈值 - 死区)
                float ratio = Mathf.Clamp01((absAw - alignedAngularDeadZone) / penaltyRange);
                
                // 惩罚值 = Penalty × 比例
                float penaltyValue = alignedAngularPenalty * ratio;
                straightOutputPenalty -= penaltyValue;
                
                // 计算角速度惩罚百分比（0-100%）
                angularPenaltyPercent = ratio * 100f;
            }
            
            // 6.2 横向速度惩罚：使用累积输出的归一化幅度（0~1）
            float absAx = Mathf.Abs(outputVx) / Mathf.Max(0.001f, maxLateralSpeed);
            if (absAx > alignedLateralDeadZone && alignedLateralPenalty > 0f)
            {
                // 计算死区到最大值阈值之间的范围
                float penaltyRange = Mathf.Max(0.001f, alignedLateralMaxPenaltyThreshold - alignedLateralDeadZone);
                
                // 计算比例：(输出值 - 死区) / (最大值阈值 - 死区)
                float ratio = Mathf.Clamp01((absAx - alignedLateralDeadZone) / penaltyRange);
                
                // 惩罚值 = Penalty × 比例
                float penaltyValue = alignedLateralPenalty * ratio;
                straightOutputPenalty -= penaltyValue;
                
                // 计算横向速度惩罚百分比（0-100%）
                lateralPenaltyPercent = ratio * 100f;
            }
            
            // 惩罚百分比取两者中的较大值（因为惩罚是两者相加的）
            straightOutputPenaltyPercent = Mathf.Max(angularPenaltyPercent, lateralPenaltyPercent);
        }

        // ========== 4. 最终奖励 ==========
        float totalReward = alignmentReward * speedCoefficient
                           + straightOutputPenalty;
        
        if (enableRewardTracking)
        {
            currentRewardComponents = new RewardComponents
            {
                alignmentReward = alignmentReward,
                speedCoefficient = speedCoefficient,
                straightOutputPenalty = straightOutputPenalty,
                straightOutputPenaltyPercent = straightOutputPenaltyPercent,
                totalReward = totalReward,
                rewardThisFrame = 0f  // 将在调用处设置
            };
        }
        
        return totalReward;
    }

    // ========== 数据收集方法 ==========
    // CSV 列说明（共 17 列）：
    //
    // ---- 输入特征（模糊规则库 / 决策树共用）----
    //  [0]  front_lr_diff     前排左右差（归一化 [-1,1]）
    //  [1]  rear_lr_diff      后排左右差（归一化 [-1,1]）
    //  [2]  front_center      前中传感器（归一化 [0,1]）
    //  [3]  rear_center       后中传感器（归一化 [0,1]）
    //  [4]  accumulated_vx    当前累积横向速度（归一化 [-1,1]）
    //  [5]  accumulated_omega 当前累积角速度（归一化 [-1,1]）
    //  [6]  actual_vz         实际前进速度（归一化）
    //  [7]  actual_vx         实际横向速度（归一化）
    //  [8]  actual_omega      实际角速度（归一化）
    //
    // ---- 增量动作（NN 原始输出，用于决策树蒸馏）----
    //  [9]  delta_x           横向速度增量 [-1,1]
    //  [10] delta_w           角速度增量 [-1,1]
    //
    // ---- 直接动作（累积后归一化，用于模糊规则库）----
    //  [11] output_vx_norm    累积横向速度输出（归一化 [-1,1]，= accumulated_vx 更新后）
    //  [12] output_omega_norm 累积角速度输出（归一化 [-1,1]，= accumulated_omega 更新后）
    //
    // ---- 6维原始传感器（用于需要完整传感器的分析）----
    //  [13] sensor_fl         前左传感器（归一化 [0,1]）
    //  [14] sensor_fc         前中传感器（归一化 [0,1]，同 [2]）
    //  [15] sensor_fr         前右传感器（归一化 [0,1]）
    //  [16] sensor_rl         后左传感器（归一化 [0,1]）
    //  [17] sensor_rc         后中传感器（归一化 [0,1]，同 [3]）
    //  [18] sensor_rr         后右传感器（归一化 [0,1]）
    //
    // 模糊规则库建立推荐使用列：
    //   输入: [0]front_lr_diff, [1]rear_lr_diff, [2]front_center, [3]rear_center
    //   输出: [11]output_vx_norm, [12]output_omega_norm
    //
    private void RecordSample(float deltaX, float deltaW)
    {
        // ---- 读取 6 个原始传感器 ----
        float[] rawSensor = new float[6];
        for (int i = 0; i < sensors.Length; i++)
        {
            if (sensors[i] != null && tape != null)
                rawSensor[i] = tape.GetMagneticField(sensors[i].position).magnitude;
        }
        float invMax = 1f / Mathf.Max(1e-9f, maxField);

        // ---- 4 维派生特征 ----
        float frontLRDiff  = Mathf.Clamp((rawSensor[0] - rawSensor[2]) * invMax, -1f, 1f);
        float rearLRDiff   = Mathf.Clamp((rawSensor[3] - rawSensor[5]) * invMax, -1f, 1f);
        float frontCenter  = Mathf.Clamp01(rawSensor[1] * invMax);
        float rearCenter   = Mathf.Clamp01(rawSensor[4] * invMax);

        // ---- 累积输出状态（动作执行前的值）----
        float maxOmegaRad = maxOmegaDeg * Mathf.Deg2Rad;
        float accVxNorm    = Mathf.Clamp(accumulatedLateralSpeed / Mathf.Max(0.001f, maxLateralSpeed), -1f, 1f);
        float accOmegaNorm = Mathf.Clamp(accumulatedAngularSpeed / Mathf.Max(0.001f, maxOmegaRad), -1f, 1f);

        // ---- 物理状态 ----
        Vector3 localVel = transform.InverseTransformDirection(rb != null ? rb.linearVelocity : Vector3.zero);
        float angularVel = rb != null ? rb.angularVelocity.y : 0f;
        float actualVz    = Mathf.Clamp(localVel.z / Mathf.Max(0.001f, constantForwardSpeed), -2f, 2f);
        float actualVx    = Mathf.Clamp(localVel.x / Mathf.Max(0.001f, maxLateralSpeed), -2f, 2f);
        float actualOmega = Mathf.Clamp(angularVel / maxOmegaRad, -2f, 2f);

        // ---- 计算增量执行后的累积输出（直接动作值）----
        // 模拟增量累积后的结果（与 OnActionReceived 中的逻辑一致）
        float newAccLateral = accumulatedLateralSpeed;
        float newAccAngular = accumulatedAngularSpeed;
        if (outputDecayFactor < 1f)
        {
            newAccLateral *= outputDecayFactor;
            newAccAngular *= outputDecayFactor;
        }
        newAccLateral = Mathf.Clamp(
            newAccLateral + deltaX * maxDeltaLateralSpeed, -maxLateralSpeed, maxLateralSpeed);
        newAccAngular = Mathf.Clamp(
            newAccAngular + deltaW * maxDeltaOmegaDeg * Mathf.Deg2Rad, -maxOmegaRad, maxOmegaRad);
        
        float outputVxNorm    = Mathf.Clamp(newAccLateral / Mathf.Max(0.001f, maxLateralSpeed), -1f, 1f);
        float outputOmegaNorm = Mathf.Clamp(newAccAngular / Mathf.Max(0.001f, maxOmegaRad), -1f, 1f);

        // ---- 6 维原始传感器归一化 ----
        float sensorFL = Mathf.Clamp01(rawSensor[0] * invMax);
        float sensorFC = frontCenter;  // 与 [2] 相同
        float sensorFR = Mathf.Clamp01(rawSensor[2] * invMax);
        float sensorRL = Mathf.Clamp01(rawSensor[3] * invMax);
        float sensorRC = rearCenter;   // 与 [3] 相同
        float sensorRR = Mathf.Clamp01(rawSensor[5] * invMax);

        // ---- 构建 CSV 行 ----
        StringBuilder sb = new StringBuilder();
        // 输入特征 [0-8]
        sb.Append(frontLRDiff.ToString("F6"));  sb.Append(",");
        sb.Append(rearLRDiff.ToString("F6"));   sb.Append(",");
        sb.Append(frontCenter.ToString("F6"));  sb.Append(",");
        sb.Append(rearCenter.ToString("F6"));   sb.Append(",");
        sb.Append(accVxNorm.ToString("F6"));    sb.Append(",");
        sb.Append(accOmegaNorm.ToString("F6")); sb.Append(",");
        sb.Append(actualVz.ToString("F6"));     sb.Append(",");
        sb.Append(actualVx.ToString("F6"));     sb.Append(",");
        sb.Append(actualOmega.ToString("F6"));  sb.Append(",");
        // 增量动作 [9-10]（NN原始输出，决策树蒸馏用）
        sb.Append(deltaX.ToString("F6"));       sb.Append(",");
        sb.Append(deltaW.ToString("F6"));       sb.Append(",");
        // 直接动作 [11-12]（累积后归一化，模糊规则库用）
        sb.Append(outputVxNorm.ToString("F6"));    sb.Append(",");
        sb.Append(outputOmegaNorm.ToString("F6")); sb.Append(",");
        // 原始传感器 [13-18]（完整分析用）
        sb.Append(sensorFL.ToString("F6")); sb.Append(",");
        sb.Append(sensorFC.ToString("F6")); sb.Append(",");
        sb.Append(sensorFR.ToString("F6")); sb.Append(",");
        sb.Append(sensorRL.ToString("F6")); sb.Append(",");
        sb.Append(sensorRC.ToString("F6")); sb.Append(",");
        sb.Append(sensorRR.ToString("F6"));
        
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
                string header = "front_lr_diff,rear_lr_diff,front_center,rear_center," +
                               "accumulated_vx,accumulated_omega," +
                               "actual_vz,actual_vx,actual_omega," +
                               "delta_x,delta_w," +
                               "output_vx_norm,output_omega_norm," +
                               "sensor_fl,sensor_fc,sensor_fr,sensor_rl,sensor_rc,sensor_rr";
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
        string header = "front_lr_diff,rear_lr_diff,front_center,rear_center," +
                       "accumulated_vx,accumulated_omega," +
                       "actual_vz,actual_vx,actual_omega," +
                       "delta_x,delta_w," +
                       "output_vx_norm,output_omega_norm," +
                       "sensor_fl,sensor_fc,sensor_fr,sensor_rl,sensor_rc,sensor_rr";
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

    public void ClearCollectedData()
    {
        int count = collectedData.Count;
        collectedData.Clear();
        totalCollectedSamples = 0;
        
        episodeDataFilePath = null;
        episodeDataHeaderWritten = false;
        
        Debug.Log($"[DataCollection] Cleared {count} samples from memory, totalCollectedSamples reset to 0.");
    }

    public int GetCollectedDataCount()
    {
        return collectedData.Count;
    }

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
                               "delta_x,delta_w," +
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
                                  float threshold, float deltaX, float deltaW, 
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
            
            // 动作
            sb.Append(deltaX.ToString("F6")); sb.Append(",");
            sb.Append(deltaW.ToString("F6")); sb.Append(",");
            
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

    private void FlushRemainingData()
    {
        if (collectedData.Count > 0)
        {
            bool shouldSave = recordDerailmentEpisodes || !episodeEndedByDerailment;
            if (shouldSave)
            {
                AppendEpisodeDataToFile();
                Debug.Log($"[DataCollection] 退出前保存剩余数据，样本数: {collectedData.Count}");
            }
            collectedData.Clear();
        }
    }

    private void OnApplicationQuit()
    {
        FlushRemainingData();
    }

    private void OnDestroy()
    {
        FlushRemainingData();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
    }
}