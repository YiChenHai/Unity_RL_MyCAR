using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// 车辆状态显示脚本：在屏幕上显示车辆和轮子的实时信息
/// </summary>
public class MyCar_StateDisplay : MonoBehaviour
{
    private const int IncrementalActionMin = 0;
    private const int IncrementalActionMax = 8;
    private const int IncrementalHoldAction = 4;

    [Header("引用组件")]
    public MyCar_Motion myCarMotion;
    public MyCarAgent myCarAgent;  // ML-Agents训练模式
    public MyCar_Agent_DistillationTest myCarAgent_DistillationTest;  // 规则库控制模式
    public Rigidbody rb;
    public MagneticTape tape;
    [Tooltip("传感器顺序: [0]=前左, [1]=前中, [2]=前右, [3]=后左, [4]=后中, [5]=后右")]
    public Transform[] sensors = new Transform[6];

    [Header("显示设置")]
    public bool showDebugInfo = true;
    [Tooltip("训练时自动禁用UI显示（提升性能，避免Unity窗口卡顿）")]
    public bool autoDisableInTraining = true;  // 训练时自动禁用UI
    public Vector2 displayPosition = new Vector2(10, 10);
    public Vector2 displaySize = new Vector2(500, 650);
    
    [Header("曲线显示设置")]
    public int curveHistoryLength = 200;  // 曲线历史数据点数（用于动作索引/奖励曲线）
    
    [Header("动作索引显示设置")]
    [Tooltip("是否显示动作索引折线图")]
    public bool showActionIndexCurve = true;
    [Tooltip("动作索引曲线显示位置")]
    public Vector2 actionCurvePosition = new Vector2(930, 10);
    [Tooltip("动作索引曲线显示大小")]
    public Vector2 actionCurveSize = new Vector2(400, 250);
    
    [Header("奖励显示设置")]
    [Tooltip("是否显示奖励信息")]
    public bool showRewardInfo = true;
    [Tooltip("奖励信息显示位置")]
    public Vector2 rewardDisplayPosition = new Vector2(10, 570);
    [Tooltip("奖励信息显示大小")]
    public Vector2 rewardDisplaySize = new Vector2(500, 240);
    [Tooltip("奖励曲线显示位置")]
    public Vector2 rewardCurvePosition = new Vector2(520, 270);
    [Tooltip("奖励曲线显示大小")]
    public Vector2 rewardCurveSize = new Vector2(400, 200);
    
    [Header("蒸馏数据采集")]
    [Tooltip("启用数据收集模式（在FixedUpdate中记录，避免延迟）")]
    public bool enableDataCollection = false;
    [Tooltip("是否记录脱轨终止的回合数据. false=不记录(推荐), true=记录所有回合")]
    public bool recordDerailmentEpisodes = false;
    [Tooltip("是否启用记录条数限制")]
    public bool enableMaxSamplesLimit = false;
    [Tooltip("目标采集数量(仅在启用限制时有效). 到达目标后停止写入")]
    [Range(1, 1000000)]
    public int maxDataSamples = 300000;
    [Tooltip("内存缓冲区最大容量(防止内存溢出). 超过此数量时自动分批写入文件. 建议值: 10000-50000")]
    [Range(1000, 100000)]
    public int maxBufferSize = 20000;
    [Tooltip("批量写入大小(每次写入的数据条数). 建议值: 5000-20000,过大可能导致写入阻塞")]
    [Range(1000, 50000)]
    public int batchWriteSize = 10000;
    [Tooltip("CSV文件保存文件夹路径. 留空则使用默认路径Application.persistentDataPath. 文件会自动以日期命名training_data_yyyyMMdd_HHmmss.csv")]
    public string customSavePath = "";
    
    [Header("脱轨日志记录")]
    [Tooltip("是否启用脱轨日志记录(记录到文件)")]
    public bool enableDerailmentLogging = true;
    [Tooltip("脱轨日志文件保存路径(留空则使用默认路径)")]
    public string derailmentLogPath = "";
    [Tooltip("脱轨日志缓冲大小(达到此数量时批量写入). 建议值: 10-100,避免频繁写入")]
    [Range(1, 500)]
    public int derailmentLogBufferSize = 50;

    private static Texture2D _bgTexture; // 静态背景纹理，避免每帧创建
    private static Texture2D _cyanTexture; // 青色图例颜色块
    private static Texture2D _magentaTexture; // 洋红色图例颜色块
    private static Material _lineMaterial; // 用于GL绘制的静态材质，避免每帧创建
    
    // 动作索引历史记录
    private int[] _actionIndexHistory;
    private int _actionHistoryIndex = 0;
    
    // ========== 数据收集相关变量 ==========
    private List<string> collectedData = new List<string>();
    private int episodeDataCount = 0;
    private int totalCollectedSamples = 0;
    private string episodeDataFilePath = null;
    private bool episodeDataHeaderWritten = false;
    private bool episodeEndedByDerailment = false;
    private bool wasDerailedLastFrame = false;  // 上一帧是否脱轨
    
    // 公开接口供外部访问（供DistillationHelper使用）
    public int TotalCollectedSamples => totalCollectedSamples;
    public int GetCollectedDataCount() => collectedData.Count;
    public void FlushDataBuffer() => FlushDataBufferInternal();
    
    // ========== 脱轨日志相关变量 ==========
    private string derailmentLogFilePath = null;
    private int derailmentCount = 0;
    private List<string> derailmentLogBuffer = new List<string>();

    void Start()
    {
        // 自动查找组件（如果未手动绑定）
        if (myCarMotion == null)
            myCarMotion = GetComponent<MyCar_Motion>();
        
        if (myCarAgent == null)
            myCarAgent = GetComponent<MyCarAgent>();
        
        if (myCarAgent_DistillationTest == null)
            myCarAgent_DistillationTest = GetComponent<MyCar_Agent_DistillationTest>();
        
        if (rb == null)
            rb = GetComponent<Rigidbody>();
        
        // 初始化动作索引历史记录
        _actionIndexHistory = new int[curveHistoryLength];
        for (int i = 0; i < _actionIndexHistory.Length; i++)
        {
            _actionIndexHistory[i] = IncrementalHoldAction;
        }
        
        // 初始化图例颜色纹理
        if (_cyanTexture == null)
        {
            _cyanTexture = new Texture2D(1, 1);
            _cyanTexture.SetPixel(0, 0, Color.cyan);
            _cyanTexture.Apply();
        }
        if (_magentaTexture == null)
        {
            _magentaTexture = new Texture2D(1, 1);
            _magentaTexture.SetPixel(0, 0, Color.magenta);
            _magentaTexture.Apply();
        }
    }

    void OnGUI()
    {
        // 训练时自动禁用UI（避免Unity窗口卡顿）
        if (autoDisableInTraining)
        {
            // 检查是否处于训练模式（ML-Agents训练时，Academy会存在）
            if (Unity.MLAgents.Academy.Instance != null && Unity.MLAgents.Academy.Instance.IsCommunicatorOn)
            {
                return;  // 训练模式下禁用UI显示
            }
        }
        
        if (!showDebugInfo) return;

        // 调试：检查 myCarMotion 是否为空
        if (myCarMotion == null)
        {
            GUI.Label(new Rect(displayPosition.x, displayPosition.y, 300, 50), 
                "ERROR: myCarMotion is null! Please bind MyCar_Motion in Inspector.", 
                new GUIStyle(GUI.skin.label) { normal = { textColor = Color.red } });
            return;
        }

        // 绘制半透明灰度背景遮罩（只创建一次）
        if (_bgTexture == null)
        {
            _bgTexture = new Texture2D(1, 1);
            _bgTexture.SetPixel(0, 0, new Color(0.2f, 0.2f, 0.2f, 0.5f));
            _bgTexture.Apply();
        }
        GUI.DrawTexture(new Rect(displayPosition.x, displayPosition.y, displaySize.x, displaySize.y), _bgTexture);

        GUILayout.BeginArea(new Rect(displayPosition.x, displayPosition.y, displaySize.x, displaySize.y));
        GUILayout.Box("Vehicle & Wheel Info", GUILayout.Width(displaySize.x - 20));

        // ========== 整车信息 ==========
        GUILayout.Label("═══ Vehicle (Body) ═══", GUILayout.Width(displaySize.x - 20));
        
        Vector3 vel = rb != null ? rb.linearVelocity : Vector3.zero;
        float speed = vel.magnitude;
        float forwardSpeed = Vector3.Dot(vel, transform.forward);
        float lateralSpeed = Vector3.Dot(vel, transform.right);
        
        Vector3 angVel = rb != null ? rb.angularVelocity : Vector3.zero;
        float yawRate = angVel.y * Mathf.Rad2Deg; // deg/s

        GUILayout.Label($"Linear Velocity: {speed:F2} m/s (Forward: {forwardSpeed:F2}, Lateral: {lateralSpeed:F2})", 
            GUILayout.Width(displaySize.x - 20));
        GUILayout.Label($"Yaw Rate: {yawRate:F1} deg/s | Position: ({transform.position.x:F2}, {transform.position.z:F2})", 
            GUILayout.Width(displaySize.x - 20));

        GUILayout.Space(10);

        // ========== 各轮子信息（绿色显示）==========
        GUILayout.Label("═══ Wheels ═══", GUILayout.Width(displaySize.x - 20));

        string[] wheelNames = { "FL", "RL", "RR", "FR" };
        GUIStyle greenLabelStyle = new GUIStyle(GUI.skin.label)
        {
            normal = { textColor = Color.green },
            fontSize = 12,
            fontStyle = FontStyle.Bold
        };
        
        for (int i = 0; i < 4; i++)
        {
            WheelCollider wc = (myCarMotion.wheelColliders != null && i < myCarMotion.wheelColliders.Length) 
                ? myCarMotion.wheelColliders[i] 
                : null;

            float steerDeg = (myCarMotion.steerAngles != null && i < myCarMotion.steerAngles.Length) 
                ? myCarMotion.steerAngles[i] * Mathf.Rad2Deg 
                : 0f;
            
            float wheelSpeed = (myCarMotion.wheelSpeeds != null && i < myCarMotion.wheelSpeeds.Length) 
                ? myCarMotion.wheelSpeeds[i] 
                : 0f;

            float motorTorque = wc != null ? wc.motorTorque : 0f;
            float brakeTorque = wc != null ? wc.brakeTorque : 0f;
            float wheelRpm = wc != null ? wc.rpm : 0f;

            GUILayout.Label($"{wheelNames[i]}: Speed={wheelSpeed:F2}m/s Angle={steerDeg:F1}° RPM={wheelRpm:F0} | Motor={motorTorque:F1}Nm Brake={brakeTorque:F1}Nm", 
                greenLabelStyle, GUILayout.Width(displaySize.x - 20));
        }

        GUILayout.Space(10);

        // ========== 控制输入信息 ==========
        GUILayout.Label("═══ Control Input ═══", GUILayout.Width(displaySize.x - 20));
        GUILayout.Label($"Vz(前进): {myCarMotion.vz_input:F3} m/s | Vx(横向): {myCarMotion.vx_input:F3} m/s | Omega: {myCarMotion.omega_input:F3} rad/s", 
            GUILayout.Width(displaySize.x - 20));
        
        // 显示Agent的速度限制
        if (myCarAgent != null)
        {
            GUILayout.Label($"Agent Limits: ConstVz={myCarAgent.constantForwardSpeed:F2}m/s | MaxVx={myCarAgent.maxLateralSpeed:F2}m/s | MaxOmega={myCarAgent.maxOmegaDeg:F0}°/s", 
                GUILayout.Width(displaySize.x - 20));
        }

        GUILayout.Space(10);

        // ========== 磁传感器信息 ==========
        GUILayout.Label("═══ Magnetic Sensors ═══", GUILayout.Width(displaySize.x - 20));
        
        if (tape != null && sensors != null && sensors.Length == 6)
        {
            float[] sensorValues = new float[6];
            string[] sensorLabels = { "前左", "前中", "前右", "后左", "后中", "后右" };
            
            // 读取传感器数据
            for (int i = 0; i < 6; i++)
            {
                if (sensors[i] != null)
                {
                    Vector3 mag = tape.GetMagneticField(sensors[i].position);
                    sensorValues[i] = mag.magnitude;
                }
            }
            
            // 显示传感器读数（两行显示）
            GUILayout.Label($"{sensorLabels[0]}={sensorValues[0]:F2} | {sensorLabels[1]}={sensorValues[1]:F2} | {sensorLabels[2]}={sensorValues[2]:F2}", 
                GUILayout.Width(displaySize.x - 20));
            GUILayout.Label($"{sensorLabels[3]}={sensorValues[3]:F2} | {sensorLabels[4]}={sensorValues[4]:F2} | {sensorLabels[5]}={sensorValues[5]:F2}", 
                GUILayout.Width(displaySize.x - 20));
            
            // 计算并显示左右差值
            float frontDiff = sensorValues[0] - sensorValues[2];  // 前左 - 前右
            float rearDiff = sensorValues[3] - sensorValues[5];   // 后左 - 后右
            
            GUIStyle diffStyle = new GUIStyle(GUI.skin.label)
            {
                normal = { textColor = Color.yellow },
                fontSize = 12,
                fontStyle = FontStyle.Bold
            };
            
            GUILayout.Label($"前排差值(左-右): {frontDiff:F3} | 后排差值(左-右): {rearDiff:F3}", 
                diffStyle, GUILayout.Width(displaySize.x - 20));
            
            // ========== 对齐状态检测 ==========
            GUILayout.Space(5);
            
            if (myCarAgent != null)
            {
                // 直接从Agent读取对齐状态
                bool isAligned = myCarAgent.IsAligned;
                bool isStableAligned = myCarAgent.IsStableAligned;
                
                // 计算对齐判断的各项指标（仅用于显示）
                float frontDiffAbs = Mathf.Abs(frontDiff);
                float rearDiffAbs = Mathf.Abs(rearDiff);
                float diffThreshold = myCarAgent.maxField * myCarAgent.alignedThresholdPercent;
                float centerThreshold = myCarAgent.maxField * myCarAgent.centerThresholdPercent;
                
                bool leftRightAligned = (frontDiffAbs < diffThreshold) && (rearDiffAbs < diffThreshold);
                bool centerStrong = (sensorValues[1] > centerThreshold) && (sensorValues[4] > centerThreshold);
                
                // 使用不同颜色显示对齐状态
                GUIStyle alignmentStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 14,
                    fontStyle = FontStyle.Bold
                };
                
                string alignmentStatus;
                string stableStatus = "";
                
                if (isStableAligned)
                {
                    // 稳定对齐状态（绿色）
                    alignmentStyle.normal.textColor = Color.green;
                    alignmentStatus = "✓ 对齐";
                    stableStatus = " [稳定]";
                }
                else if (isAligned)
                {
                    // 对齐但未稳定（黄色）
                    alignmentStyle.normal.textColor = Color.yellow;
                    float progress = myCarAgent.AlignedTimer / myCarAgent.stableAlignedTime;
                    alignmentStatus = "⊙ 对齐中";
                    stableStatus = $" [确认中 {myCarAgent.AlignedTimer:F1}s / {myCarAgent.stableAlignedTime:F1}s ({progress*100:F0}%)]";
                }
                else
                {
                    // 未对齐（红色）
                    alignmentStyle.normal.textColor = Color.red;
                    alignmentStatus = "✗ 未对齐";
                    stableStatus = " [不满足对齐标准]";
                }
                
                GUILayout.Label($"对齐状态: {alignmentStatus}{stableStatus}", alignmentStyle, GUILayout.Width(displaySize.x - 20));
                
                // 显示详细判断条件
                GUIStyle detailStyle = new GUIStyle(GUI.skin.label) { fontSize = 11 };
                detailStyle.normal.textColor = leftRightAligned ? Color.green : Color.gray;
                GUILayout.Label($"  左右对称: {(leftRightAligned ? "✓" : "✗")} (前={frontDiffAbs:F2}<{diffThreshold:F2}, 后={rearDiffAbs:F2}<{diffThreshold:F2})", 
                    detailStyle, GUILayout.Width(displaySize.x - 20));
                
                detailStyle.normal.textColor = centerStrong ? Color.green : Color.gray;
                GUILayout.Label($"  中心强度: {(centerStrong ? "✓" : "✗")} (前={sensorValues[1]:F2}>{centerThreshold:F2}, 后={sensorValues[4]:F2}>{centerThreshold:F2})", 
                    detailStyle, GUILayout.Width(displaySize.x - 20));
            }
            
            // ========== 显示当前智能体输出状态 ==========
            GUILayout.Space(5);
            GUIStyle actionStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold
            };
            
            int currentAction = myCarAgent.CurrentDiscreteAction;
            int previousAction = myCarAgent.PreviousDiscreteAction;
            
            // 获取动作名称
            string GetActionName(int actionIndex)
            {
                switch (actionIndex)
                {
                    case 0: return "大左";
                    case 1: return "中左";
                    case 2: return "小左";
                    case 3: return "微左";
                    case 4: return "保持";
                    case 5: return "微右";
                    case 6: return "小右";
                    case 7: return "中右";
                    case 8: return "大右";
                    default: return "未知";
                }
            }
            
            // 根据动作类型设置颜色
            Color GetActionColor(int actionIndex)
            {
                switch (actionIndex)
                {
                    case 0: return new Color(1f, 0.2f, 0.2f);  // 大左
                    case 1: return new Color(1f, 0.4f, 0.2f);  // 中左
                    case 2: return new Color(1f, 0.6f, 0.3f);  // 小左
                    case 3: return new Color(1f, 0.8f, 0.5f);  // 微左
                    case 4: return Color.green;                 // 保持
                    case 5: return new Color(0.5f, 0.8f, 1f);  // 微右
                    case 6: return new Color(0.3f, 0.7f, 1f);  // 小右
                    case 7: return new Color(0.2f, 0.5f, 1f);  // 中右
                    case 8: return new Color(0.1f, 0.35f, 1f); // 大右
                    default: return Color.gray;
                }
            }
            
            actionStyle.normal.textColor = GetActionColor(currentAction);
            GUILayout.Label($"当前动作: [{currentAction}] {GetActionName(currentAction)}", 
                actionStyle, GUILayout.Width(displaySize.x - 20));
            
            // 显示动作变化
            if (currentAction != previousAction)
            {
                GUIStyle changeStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 11,
                    normal = { textColor = Color.yellow }
                };
                int actionDiff = Mathf.Abs(currentAction - previousAction);
                GUILayout.Label($"  变化: [{previousAction}] {GetActionName(previousAction)} → [{currentAction}] {GetActionName(currentAction)} (差值={actionDiff})", 
                    changeStyle, GUILayout.Width(displaySize.x - 20));
            }
            else
            {
                GUIStyle stableStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 11,
                    normal = { textColor = Color.gray }
                };
                GUILayout.Label($"  状态: 动作保持不变", 
                    stableStyle, GUILayout.Width(displaySize.x - 20));
            }
        }
        else
        {
            GUILayout.Label("未配置磁带或传感器", GUILayout.Width(displaySize.x - 20));
        }

        GUILayout.EndArea();
        
        // ========== 绘制动作索引折线图 ==========
        if (showActionIndexCurve && myCarAgent != null)
        {
            DrawActionIndexCurve();
        }
        
        // ========== 绘制奖励信息 ==========
        if (showRewardInfo && myCarAgent != null && myCarAgent.enableRewardTracking)
        {
            DrawRewardComponents();
            DrawRewardCurve();
        }
    }
    
    void Update()
    {
        // 记录当前动作索引到历史记录
        if (myCarAgent != null && _actionIndexHistory != null)
        {
            int currentAction = myCarAgent.CurrentDiscreteAction;
            _actionIndexHistory[_actionHistoryIndex] = currentAction;
            _actionHistoryIndex = (_actionHistoryIndex + 1) % curveHistoryLength;
        }
    }
    
    void FixedUpdate()
    {
        // 获取当前使用的控制脚本（优先使用训练模式，否则使用规则库模式）
        bool useTrainingMode = myCarAgent != null && myCarAgent.isActiveAndEnabled;
        bool useDistillationMode = !useTrainingMode && myCarAgent_DistillationTest != null && myCarAgent_DistillationTest.isActiveAndEnabled;
        
        // ========== 数据收集（在FixedUpdate中执行，避免延迟） ==========
        if (enableDataCollection && (useTrainingMode || useDistillationMode))
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
                    RecordSample();
                    episodeDataCount++;
                }
            }
            else
            {
                // 无限制，正常采集
                RecordSample();
                episodeDataCount++;
            }
        }
        
        // ========== 脱轨检测和记录（支持两种控制模式） ==========
        if (enableDerailmentLogging && tape != null && sensors != null && sensors.Length >= 6)
        {
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
            
            float frontCenter = sensorValues[1];  // 前中
            float rearCenter = sensorValues[4];   // 后中
            
            // 获取maxField（根据当前使用的控制模式）
            float maxFieldValue = 8f;  // 默认值
            if (useTrainingMode)
            {
                maxFieldValue = myCarAgent.maxField;
            }
            else if (useDistillationMode)
            {
                maxFieldValue = myCarAgent_DistillationTest.maxField;
            }
            
            float derailThresholdValue = maxFieldValue * 0.18f;  // 18%最大磁场强度
            
            // 取两个中心传感器的最小值
            float minCenter = Mathf.Min(frontCenter, rearCenter);
            
            // 检测脱轨
            bool isDerailed = minCenter < derailThresholdValue;
            
            // 如果刚脱轨（上一帧未脱轨，当前帧脱轨），记录脱轨信息
            if (isDerailed && !wasDerailedLastFrame)
            {
                RecordDerailment(sensorValues, frontCenter, rearCenter, derailThresholdValue);
            }
            
            wasDerailedLastFrame = isDerailed;
        }
    }
    
    void OnEnable()
    {
        // 初始化脱轨日志文件（如果启用）
        if (enableDerailmentLogging && derailmentLogFilePath == null)
        {
            InitializeDerailmentLogFile();
        }
    }
    
    void OnDisable()
    {
        // 回合结束时刷新数据缓冲
        if (collectedData.Count > 0)
        {
            bool shouldSave = recordDerailmentEpisodes || !episodeEndedByDerailment;
            if (shouldSave)
            {
                FlushDataBuffer();
                Debug.Log($"[DataCollection] 回合数据已保存，样本数: {collectedData.Count}");
            }
            collectedData.Clear();
            episodeDataCount = 0;
        }
        
        // 刷新脱轨日志缓冲
        if (enableDerailmentLogging && derailmentLogBuffer.Count > 0)
        {
            FlushDerailmentLog();
        }
        
        episodeEndedByDerailment = false;
        wasDerailedLastFrame = false;
    }

    
    /// <summary>
    /// 使用 GL 绘制直线（运行时用，使用静态材质避免频繁创建对象）
    /// </summary>
    void DrawLine(Vector2 start, Vector2 end, Color color)
    {
        // 只在重绘事件中绘制，避免在Layout等事件中调用GL
        if (Event.current != null && Event.current.type != EventType.Repaint)
        {
            return;
        }

        // 延迟创建静态材质
        if (_lineMaterial == null)
        {
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            if (shader == null)
            {
                return;
            }

            _lineMaterial = new Material(shader);
            _lineMaterial.hideFlags = HideFlags.HideAndDontSave;

            // 设置基础渲染参数（透明混合、不写深度、无背面剔除）
            _lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            _lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            _lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            _lineMaterial.SetInt("_ZWrite", 0);
        }

        GL.PushMatrix();
        GL.LoadOrtho();
        
        // 将屏幕坐标转换为 GL 坐标 (0-1)
        start.x /= Screen.width;
        start.y /= Screen.height;
        end.x /= Screen.width;
        end.y /= Screen.height;
        
        // Y 轴反向（屏幕坐标 Y 向下，GL 坐标 Y 向上）
        start.y = 1f - start.y;
        end.y = 1f - end.y;
        
        _lineMaterial.SetPass(0);
        
        GL.Begin(GL.LINES);
        GL.Color(color);
        GL.Vertex(start);
        GL.Vertex(end);
        GL.End();
        
        GL.PopMatrix();
    }

    /// <summary>
    /// 绘制奖励组成部分的详细信息
    /// </summary>
    void DrawRewardComponents()
    {
        Rect rewardRect = new Rect(rewardDisplayPosition.x, rewardDisplayPosition.y, rewardDisplaySize.x, rewardDisplaySize.y);
        GUI.DrawTexture(rewardRect, _bgTexture);
        GUI.Box(rewardRect, "Reward Components");
        GUILayout.BeginArea(new Rect(rewardRect.x + 10, rewardRect.y + 25, rewardRect.width - 20, rewardRect.height - 35));

        MyCarAgent.RewardComponents components = myCarAgent.CurrentRewardComponents;
        float cumulativeReward = myCarAgent.CumulativeReward;

        GUIStyle titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 14,
            fontStyle = FontStyle.Bold,
            normal = { textColor = cumulativeReward >= 0 ? Color.green : Color.red }
        };
        GUILayout.Label($"累计奖励: {cumulativeReward:F3}", titleStyle);
        GUILayout.Space(5);

        GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 11,
            normal = { textColor = Color.white }
        };

        float alignmentSpeedReward = components.alignmentReward * components.speedCoefficient;
        Color alignmentColor = alignmentSpeedReward >= 0 ? Color.green : Color.red;
        labelStyle.normal.textColor = alignmentColor;
        GUILayout.Label($"1. 对齐奖励×速度系数: {alignmentSpeedReward:F4} (对齐={components.alignmentReward:F3} × 速度={components.speedCoefficient:F3})", 
            labelStyle, GUILayout.Width(rewardRect.width - 20));
        GUILayout.Space(5);
        
        GUIStyle totalStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 12,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.cyan }
        };
        GUILayout.Label($"总奖励(×dt前): {components.totalReward:F4}", totalStyle, GUILayout.Width(rewardRect.width - 20));
        totalStyle.normal.textColor = components.rewardThisFrame >= 0 ? Color.green : Color.red;
        GUILayout.Label($"本帧奖励(×dt后): {components.rewardThisFrame:F4}", totalStyle, GUILayout.Width(rewardRect.width - 20));
        
        GUILayout.EndArea();
    }

    /// <summary>
    /// 绘制奖励曲线
    /// </summary>
    void DrawRewardCurve()
    {
        Rect curveRect = new Rect(rewardCurvePosition.x, rewardCurvePosition.y, rewardCurveSize.x, rewardCurveSize.y);
        
        // 绘制背景
        GUI.DrawTexture(curveRect, _bgTexture);
        GUI.Box(curveRect, "Reward History");
        
        // 内部绘制区域
        Rect innerRect = new Rect(curveRect.x + 10, curveRect.y + 25, curveRect.width - 20, curveRect.height - 35);
        
        // 获取奖励历史数据
        float[] rewardHistory = myCarAgent.RewardHistory;
        int historyLength = myCarAgent.RewardHistoryLength;
        int historyIndex = myCarAgent.RewardHistoryIndex;
        
        if (rewardHistory == null || historyLength == 0) return;
        
        // 计算Y轴范围（动态缩放）
        float minReward = float.MaxValue;
        float maxReward = float.MinValue;
        for (int i = 0; i < historyLength; i++)
        {
            float val = rewardHistory[i];
            if (val < minReward) minReward = val;
            if (val > maxReward) maxReward = val;
        }
        
        // 如果所有值都相同，设置一个默认范围
        if (Mathf.Approximately(minReward, maxReward))
        {
            minReward = minReward - 0.1f;
            maxReward = maxReward + 0.1f;
        }
        
        // 添加一些边距
        float range = maxReward - minReward;
        minReward -= range * 0.1f;
        maxReward += range * 0.1f;
        
        // 绘制网格和0线
        DrawRewardGrid(innerRect, minReward, maxReward);
        
        // 绘制奖励曲线
        DrawRewardCurveLine(innerRect, rewardHistory, historyLength, historyIndex, minReward, maxReward);
        
        // 显示Y轴范围
        GUIStyle rangeStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 10,
            normal = { textColor = Color.gray }
        };
        GUI.Label(new Rect(innerRect.x, innerRect.yMax + 5, innerRect.width, 20), 
            $"Range: [{minReward:F2}, {maxReward:F2}]", rangeStyle);
    }
    
    void DrawRewardGrid(Rect graphRect, float minVal, float maxVal)
    {
        // 绘制0线
        float zeroY = Mathf.Lerp(graphRect.yMax, graphRect.y, 
            (0f - minVal) / (maxVal - minVal));
        zeroY = Mathf.Clamp(zeroY, graphRect.y, graphRect.yMax);
        DrawLine(new Vector2(graphRect.x, zeroY), new Vector2(graphRect.xMax, zeroY), Color.gray);
        
        // 绘制Y轴标签
        float[] labelValues = { maxVal, (maxVal + minVal) / 2f, minVal };
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 10,
            normal = { textColor = Color.gray },
            alignment = TextAnchor.MiddleRight
        };
        
        foreach (float val in labelValues)
        {
            float screenY = Mathf.Lerp(graphRect.yMax, graphRect.y, (val - minVal) / (maxVal - minVal));
            screenY = Mathf.Clamp(screenY, graphRect.y, graphRect.yMax);
            Rect labelRect = new Rect(graphRect.x - 50, screenY - 10, 45, 20);
            GUI.Label(labelRect, val.ToString("F2"), labelStyle);
        }
    }
    
    /// <summary>
    /// 绘制动作索引折线图
    /// </summary>
    void DrawActionIndexCurve()
    {
        Rect curveRect = new Rect(actionCurvePosition.x, actionCurvePosition.y, actionCurveSize.x, actionCurveSize.y);
        
        // 绘制背景
        GUI.DrawTexture(curveRect, _bgTexture);
        
        // 绘制边框
        GUI.Box(curveRect, "Action Index History");
        
        // 内部绘制区域（留出边距）
        Rect innerRect = new Rect(curveRect.x + 10, curveRect.y + 25, curveRect.width - 20, curveRect.height - 35);
        
        // 绘制网格和曲线
        DrawActionIndexGraph(innerRect);
    }
    
    void DrawActionIndexGraph(Rect graphRect)
    {
        if (_actionIndexHistory == null || _actionIndexHistory.Length < 2) return;
        
        // 动作索引范围（训练模式增量动作）：0-8
        float minVal = IncrementalActionMin;
        float maxVal = IncrementalActionMax;
        
        // 绘制网格
        DrawActionIndexGrid(graphRect, minVal, maxVal);
        
        // 绘制动作索引折线
        DrawActionIndexLine(graphRect, _actionIndexHistory, curveHistoryLength, _actionHistoryIndex, minVal, maxVal);
        
        // 显示当前动作索引值
        if (myCarAgent != null)
        {
            int currentAction = myCarAgent.CurrentDiscreteAction;
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 11,
                normal = { textColor = Color.white }
            };
            GUI.Label(new Rect(graphRect.x, graphRect.yMax + 5, graphRect.width, 20), 
                $"当前动作索引: {currentAction}", labelStyle);
        }
    }
    
    void DrawActionIndexGrid(Rect graphRect, float minVal, float maxVal)
    {
        // 绘制Y轴标签（0, 2, 4, 6, 8）
        float[] labelValues = { 8f, 6f, 4f, 2f, 0f };
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 10,
            normal = { textColor = Color.gray },
            alignment = TextAnchor.MiddleRight
        };
        
        foreach (float val in labelValues)
        {
            float screenY = Mathf.Lerp(graphRect.yMax, graphRect.y, (val - minVal) / (maxVal - minVal));
            screenY = Mathf.Clamp(screenY, graphRect.y, graphRect.yMax);
            Rect labelRect = new Rect(graphRect.x - 35, screenY - 10, 30, 20);
            GUI.Label(labelRect, val.ToString("F0"), labelStyle);
        }
        
        // 绘制中心线（索引4，即保持）
        float centerY = Mathf.Lerp(graphRect.yMax, graphRect.y, (IncrementalHoldAction - minVal) / (maxVal - minVal));
        centerY = Mathf.Clamp(centerY, graphRect.y, graphRect.yMax);
        DrawLine(new Vector2(graphRect.x, centerY), new Vector2(graphRect.xMax, centerY), 
            new Color(0.5f, 0.5f, 0.5f, 0.5f));
        
        // 绘制网格线（每2个索引一条）
        for (int i = 0; i <= IncrementalActionMax; i += 2)
        {
            if (i == IncrementalHoldAction) continue; // 跳过中心线
            float gridY = Mathf.Lerp(graphRect.yMax, graphRect.y, (i - minVal) / (maxVal - minVal));
            gridY = Mathf.Clamp(gridY, graphRect.y, graphRect.yMax);
            Color gridColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);
            DrawLine(new Vector2(graphRect.x, gridY), new Vector2(graphRect.xMax, gridY), gridColor);
        }
    }
    
    void DrawActionIndexLine(Rect graphRect, int[] data, int length, int startIndex, float minVal, float maxVal)
    {
        if (data == null || length < 2) return;
        
        Color actionColor = Color.cyan;
        
        for (int i = 0; i < length - 1; i++)
        {
            int idx1 = (startIndex + i) % length;
            int idx2 = (startIndex + i + 1) % length;
            
            float value1 = data[idx1];
            float value2 = data[idx2];
            
            float screenX1 = graphRect.x + (i / (float)(length - 1)) * graphRect.width;
            float screenX2 = graphRect.x + ((i + 1) / (float)(length - 1)) * graphRect.width;
            
            float screenY1 = Mathf.Lerp(graphRect.yMax, graphRect.y, (value1 - minVal) / (maxVal - minVal));
            float screenY2 = Mathf.Lerp(graphRect.yMax, graphRect.y, (value2 - minVal) / (maxVal - minVal));
            
            screenY1 = Mathf.Clamp(screenY1, graphRect.y, graphRect.yMax);
            screenY2 = Mathf.Clamp(screenY2, graphRect.y, graphRect.yMax);
            
            DrawLine(new Vector2(screenX1, screenY1), new Vector2(screenX2, screenY2), actionColor);
        }
    }
    
    void DrawRewardCurveLine(Rect graphRect, float[] data, int length, int startIndex, float minVal, float maxVal)
    {
        if (data == null || length < 2) return;
        
        Color rewardColor = Color.yellow;
        
        for (int i = 0; i < length - 1; i++)
        {
            int idx1 = (startIndex + i) % length;
            int idx2 = (startIndex + i + 1) % length;
            
            float value1 = data[idx1];
            float value2 = data[idx2];
            
            float screenX1 = graphRect.x + (i / (float)(length - 1)) * graphRect.width;
            float screenX2 = graphRect.x + ((i + 1) / (float)(length - 1)) * graphRect.width;
            
            float screenY1 = Mathf.Lerp(graphRect.yMax, graphRect.y, (value1 - minVal) / (maxVal - minVal));
            float screenY2 = Mathf.Lerp(graphRect.yMax, graphRect.y, (value2 - minVal) / (maxVal - minVal));
            
            screenY1 = Mathf.Clamp(screenY1, graphRect.y, graphRect.yMax);
            screenY2 = Mathf.Clamp(screenY2, graphRect.y, graphRect.yMax);
            
            // 根据奖励值正负选择颜色
            Color lineColor = value1 >= 0 ? Color.green : Color.red;
            DrawLine(new Vector2(screenX1, screenY1), new Vector2(screenX2, screenY2), lineColor);
        }
    }

    private void OnDestroy()
    {
        // 清理静态资源
        if (_bgTexture != null)
        {
            Destroy(_bgTexture);
            _bgTexture = null;
        }
        if (_cyanTexture != null)
        {
            Destroy(_cyanTexture);
            _cyanTexture = null;
        }
        if (_magentaTexture != null)
        {
            Destroy(_magentaTexture);
            _magentaTexture = null;
        }
        if (_lineMaterial != null)
        {
            Destroy(_lineMaterial);
            _lineMaterial = null;
        }
        
        // 清理数据收集缓冲
        if (collectedData.Count > 0)
        {
            FlushDataBuffer();
        }
        
        // 清理脱轨日志缓冲
        if (derailmentLogBuffer.Count > 0)
        {
            FlushDerailmentLog();
        }
    }
    
    // ========== 数据收集方法 ==========
    private void RecordSample()
    {
        // 获取当前使用的控制脚本（优先使用训练模式，否则使用规则库模式）
        bool useTrainingMode = myCarAgent != null && myCarAgent.isActiveAndEnabled;
        bool useDistillationMode = !useTrainingMode && myCarAgent_DistillationTest != null && myCarAgent_DistillationTest.isActiveAndEnabled;
        
        if ((!useTrainingMode && !useDistillationMode) || tape == null || sensors == null || sensors.Length < 6)
            return;
        
        // 获取控制参数（根据当前使用的控制模式）
        float maxFieldValue = 8f;
        float constantForwardSpeed = 0.2f;
        float maxLateralSpeed = 0.15f;
        float maxOmegaDeg = 80f;
        int previousDiscreteAction = IncrementalHoldAction;
        int currentDiscreteAction = IncrementalHoldAction;
        float actionNormalizeDenominator = IncrementalActionMax;
        
        if (useTrainingMode)
        {
            maxFieldValue = myCarAgent.maxField;
            constantForwardSpeed = myCarAgent.constantForwardSpeed;
            maxLateralSpeed = myCarAgent.maxLateralSpeed;
            maxOmegaDeg = myCarAgent.maxOmegaDeg;
            previousDiscreteAction = myCarAgent.PreviousDiscreteAction;
            currentDiscreteAction = myCarAgent.CurrentDiscreteAction;
            actionNormalizeDenominator = IncrementalActionMax;
        }
        else if (useDistillationMode)
        {
            maxFieldValue = myCarAgent_DistillationTest.maxField;
            constantForwardSpeed = myCarAgent_DistillationTest.constantForwardSpeed;
            maxLateralSpeed = myCarAgent_DistillationTest.maxLateralSpeed;
            maxOmegaDeg = myCarAgent_DistillationTest.maxOmegaDeg;
            previousDiscreteAction = myCarAgent_DistillationTest.PreviousDiscreteAction;
            currentDiscreteAction = myCarAgent_DistillationTest.CurrentDiscreteAction;
            actionNormalizeDenominator = 10f;
        }
        
        // 重新计算观测（与MyCar_Agent的CollectObservations逻辑相同，10维）
        List<float> observations = new List<float>();

        // 1-6: 六个传感器的归一化强度
        float[] sensorValues = new float[6];
        for (int i = 0; i < sensors.Length; i++)
        {
            if (sensors[i] != null && tape != null)
            {
                Vector3 mag = tape.GetMagneticField(sensors[i].position);
                sensorValues[i] = mag.magnitude;
                observations.Add(Mathf.Clamp01(mag.magnitude / Mathf.Max(1e-9f, maxFieldValue)));
            }
            else observations.Add(0f);
        }

        // 7: 上次输出状态（离散动作索引归一化到0-1范围）
        float lastActionState = previousDiscreteAction / Mathf.Max(1f, actionNormalizeDenominator);
        observations.Add(lastActionState);

        // 8-10: 实际运动状态（物理反馈）
        Vector3 localVel = transform.InverseTransformDirection(rb != null ? rb.linearVelocity : Vector3.zero);
        float angularVel = rb != null ? rb.angularVelocity.y : 0f;
        float maxOmegaRad = maxOmegaDeg * Mathf.Deg2Rad;
        observations.Add(Mathf.Clamp(localVel.z / Mathf.Max(0.001f, constantForwardSpeed), -2f, 2f));  // 8: 实际前进速度
        observations.Add(Mathf.Clamp(localVel.x / Mathf.Max(0.001f, maxLateralSpeed), -2f, 2f));      // 9: 实际横向速度
        observations.Add(Mathf.Clamp(angularVel / maxOmegaRad, -2f, 2f));                              // 10: 实际角速度

        // 构建CSV行：10维特征 + 1维目标（离散动作索引）
        StringBuilder sb = new StringBuilder();
        
        // 写入10维特征（浮点数，保留6位小数）
        for (int i = 0; i < observations.Count; i++)
        {
            sb.Append(observations[i].ToString("F6"));
            if (i < observations.Count - 1)
            {
                sb.Append(",");
            }
        }
        
        // 写入目标：离散动作索引（训练模式0-8，规则库模式0-10）
        sb.Append(",");
        sb.Append(currentDiscreteAction.ToString());
        
        collectedData.Add(sb.ToString());
        
        // 内存保护：如果缓冲区超过最大容量，自动分批写入
        if (collectedData.Count >= maxBufferSize)
        {
            FlushDataBuffer();
        }
    }

    private string GetSaveDirectory()
    {
        if (!string.IsNullOrEmpty(customSavePath))
        {
            if (Path.HasExtension(customSavePath))
            {
                string dir = Path.GetDirectoryName(customSavePath);
                if (!string.IsNullOrEmpty(dir))
                {
                    Debug.LogWarning($"[DataCollection] customSavePath应该是目录路径，检测到文件路径，已提取目录: {dir}");
                    return dir;
                }
            }
            return customSavePath;
        }
        return Application.persistentDataPath;
    }

    private string GetFullFilePath(string fileName)
    {
        string directory = GetSaveDirectory();
        return Path.Combine(directory, fileName);
    }

    private void InitializeDataFileIfNeeded()
    {
        if (episodeDataFilePath != null)
        {
            return;
        }

        string fileName = $"training_data_{System.DateTime.Now:yyyyMMdd_HHmmss}.csv";
        episodeDataFilePath = GetFullFilePath(fileName);
        episodeDataHeaderWritten = false;
        
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
                episodeDataFilePath = null;
            }
        }
    }

    private void FlushDataBufferInternal()
    {
        if (collectedData.Count == 0)
        {
            return;
        }

        InitializeDataFileIfNeeded();
        if (episodeDataFilePath == null)
        {
            return;
        }

        try
        {
            if (!File.Exists(episodeDataFilePath) || !episodeDataHeaderWritten)
            {
                string header = "sensor0,sensor1,sensor2,sensor3,sensor4,sensor5," +
                               "last_action_state," +
                               "actual_vz,actual_vx,actual_omega," +
                               "discrete_action";
                File.WriteAllText(episodeDataFilePath, header + System.Environment.NewLine);
                episodeDataHeaderWritten = true;
            }

            int remainingInBuffer = collectedData.Count;
            int remainingInTarget = enableMaxSamplesLimit ? (maxDataSamples - totalCollectedSamples) : int.MaxValue;
            int writeCount = Mathf.Min(remainingInBuffer, batchWriteSize, remainingInTarget);

            if (writeCount <= 0)
            {
                if (enableMaxSamplesLimit && totalCollectedSamples >= maxDataSamples)
                {
                    enableDataCollection = false;
                    Debug.Log($"[DataCollection] Target samples ({maxDataSamples}) reached, collection stopped.");
                }
                return;
            }

            StringBuilder batchContent = new StringBuilder(writeCount * 100);
            for (int i = 0; i < writeCount; i++)
            {
                batchContent.AppendLine(collectedData[i]);
            }

            File.AppendAllText(episodeDataFilePath, batchContent.ToString());
            collectedData.RemoveRange(0, writeCount);
            totalCollectedSamples += writeCount;

            if (enableMaxSamplesLimit && totalCollectedSamples >= maxDataSamples)
            {
                enableDataCollection = false;
                collectedData.Clear();
                Debug.Log($"[DataCollection] Reached target samples ({maxDataSamples}), collection stopped.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[DataCollection] Failed to flush data buffer: {e.Message}");
        }
    }
    
    // ========== 脱轨日志记录方法 ==========
    private void InitializeDerailmentLogFile()
    {
        try
        {
            if (!string.IsNullOrEmpty(derailmentLogPath))
            {
                if (Path.HasExtension(derailmentLogPath))
                {
                    derailmentLogFilePath = derailmentLogPath;
                }
                else
                {
                    string fileName = $"derailment_log_{System.DateTime.Now:yyyyMMdd_HHmmss}.csv";
                    derailmentLogFilePath = Path.Combine(derailmentLogPath, fileName);
                }
            }
            else
            {
                string fileName = $"derailment_log_{System.DateTime.Now:yyyyMMdd_HHmmss}.csv";
                derailmentLogFilePath = Path.Combine(Application.persistentDataPath, fileName);
            }
            
            string directory = Path.GetDirectoryName(derailmentLogFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
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
    
    private void RecordDerailment(float[] sensorValues, float frontCenter, float rearCenter, float threshold)
    {
        if (!enableDerailmentLogging || derailmentLogFilePath == null)
        {
            return;
        }
        
        // 获取当前使用的控制脚本（优先使用训练模式，否则使用规则库模式）
        bool useTrainingMode = myCarAgent != null && myCarAgent.isActiveAndEnabled;
        bool useDistillationMode = !useTrainingMode && myCarAgent_DistillationTest != null && myCarAgent_DistillationTest.isActiveAndEnabled;
        
        if (!useTrainingMode && !useDistillationMode)
        {
            return;
        }
        
        try
        {
            derailmentCount++;
            episodeEndedByDerailment = true;
            
            string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            Vector3 position = transform.position;
            Vector3 rotation = transform.rotation.eulerAngles;
            Vector3 velocity = rb != null ? rb.linearVelocity : Vector3.zero;
            float angularVelocityY = rb != null ? rb.angularVelocity.y : 0f;
            
            // 获取控制脚本的状态信息
            float episodeTimer = 0f;
            int currentDiscreteAction = IncrementalHoldAction;
            float lastOutputLateralSpeed = 0f;
            float lastOutputAngularSpeed = 0f;
            bool isAligned = false;
            bool isStableAligned = false;
            
            if (useTrainingMode)
            {
                episodeTimer = myCarAgent.EpisodeTimer;
                currentDiscreteAction = myCarAgent.CurrentDiscreteAction;
                lastOutputLateralSpeed = myCarAgent.LastOutputLateralSpeed;
                lastOutputAngularSpeed = myCarAgent.LastOutputAngularSpeed;
                isAligned = myCarAgent.IsAligned;
                isStableAligned = myCarAgent.IsStableAligned;
            }
            else if (useDistillationMode)
            {
                episodeTimer = myCarAgent_DistillationTest.EpisodeTimer;
                currentDiscreteAction = myCarAgent_DistillationTest.CurrentDiscreteAction;
                lastOutputLateralSpeed = myCarAgent_DistillationTest.LastOutputLateralSpeed;
                lastOutputAngularSpeed = myCarAgent_DistillationTest.LastOutputAngularSpeed;
                isAligned = myCarAgent_DistillationTest.IsAligned;
                isStableAligned = myCarAgent_DistillationTest.IsStableAligned;
            }
            
            StringBuilder sb = new StringBuilder();
            sb.Append(timestamp); sb.Append(",");
            sb.Append(episodeTimer.ToString("F4")); sb.Append(",");
            sb.Append(derailmentCount.ToString()); sb.Append(",");
            
            sb.Append(position.x.ToString("F6")); sb.Append(",");
            sb.Append(position.y.ToString("F6")); sb.Append(",");
            sb.Append(position.z.ToString("F6")); sb.Append(",");
            
            sb.Append(rotation.x.ToString("F6")); sb.Append(",");
            sb.Append(rotation.y.ToString("F6")); sb.Append(",");
            sb.Append(rotation.z.ToString("F6")); sb.Append(",");
            
            sb.Append(velocity.x.ToString("F6")); sb.Append(",");
            sb.Append(velocity.y.ToString("F6")); sb.Append(",");
            sb.Append(velocity.z.ToString("F6")); sb.Append(",");
            
            sb.Append(angularVelocityY.ToString("F6")); sb.Append(",");
            
            for (int i = 0; i < 6; i++)
            {
                sb.Append((sensorValues != null && i < sensorValues.Length ? sensorValues[i] : 0f).ToString("F6"));
                sb.Append(",");
            }
            
            sb.Append(frontCenter.ToString("F6")); sb.Append(",");
            sb.Append(rearCenter.ToString("F6")); sb.Append(",");
            sb.Append(threshold.ToString("F6")); sb.Append(",");
            
            sb.Append(currentDiscreteAction.ToString()); sb.Append(",");
            
            sb.Append(lastOutputLateralSpeed.ToString("F6")); sb.Append(",");
            sb.Append(lastOutputAngularSpeed.ToString("F6")); sb.Append(",");
            
            sb.Append(isAligned ? "1" : "0"); sb.Append(",");
            sb.Append(isStableAligned ? "1" : "0");
            
            derailmentLogBuffer.Add(sb.ToString());
            
            if (derailmentLogBuffer.Count >= derailmentLogBufferSize)
            {
                FlushDerailmentLog();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[DerailmentLog] 记录脱轨信息失败: {e.Message}");
        }
    }
    
    private void FlushDerailmentLog()
    {
        if (derailmentLogBuffer.Count == 0 || derailmentLogFilePath == null)
        {
            return;
        }

        try
        {
            StringBuilder batchContent = new StringBuilder(derailmentLogBuffer.Count * 200);
            foreach (string line in derailmentLogBuffer)
            {
                batchContent.AppendLine(line);
            }

            File.AppendAllText(derailmentLogFilePath, batchContent.ToString());
            derailmentLogBuffer.Clear();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[DerailmentLog] Failed to flush log buffer: {e.Message}");
        }
    }
}
