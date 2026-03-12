using UnityEngine;

/// <summary>
/// 车辆状态显示脚本：在屏幕上显示车辆和轮子的实时信息
/// </summary>
public class MyCar_StateDisplay : MonoBehaviour
{
    [Header("References")]
    public MyCar_Motion myCarMotion;
    public MyCarAgent myCarAgent;
    public Rigidbody rb;
    public MagneticTape tape;
    [Tooltip("传感器顺序: [0]=前左, [1]=前中, [2]=前右, [3]=后左, [4]=后中, [5]=后右")]
    public Transform[] sensors = new Transform[6];

    [Header("Display Settings")]
    public bool showDebugInfo = true;
    public bool showOutputCurves = true;  // 显示输出曲线开关
    public Vector2 displayPosition = new Vector2(10, 10);
    public Vector2 displaySize = new Vector2(500, 560);
    
    [Header("Curve Display Settings")]
    public int curveHistoryLength = 200;  // 曲线历史数据点数
    public Vector2 curveAreaPosition = new Vector2(520, 10);
    public Vector2 curveAreaSize = new Vector2(400, 250);
    [Tooltip("增量曲线显示位置")]
    public Vector2 incrementCurvePosition = new Vector2(930, 10);
    [Tooltip("增量曲线显示大小")]
    public Vector2 incrementCurveSize = new Vector2(400, 250);
    
    [Header("Reward Display Settings")]
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

    private static Texture2D _bgTexture;
    private static Texture2D _cyanTexture;
    private static Texture2D _magentaTexture;
    private static Texture2D _yellowTexture;
    private static Texture2D _greenTexture;
    private static Material _glLineMaterial;
    
    private float[] _lateralSpeedHistory;
    private float[] _angularSpeedHistory;
    private float[] _deltaXHistory;
    private float[] _deltaWHistory;
    private int _historyIndex = 0;

    // 缓存 GUIStyle，避免每帧 new
    private GUIStyle _greenLabelStyle;
    private GUIStyle _diffStyle;
    private GUIStyle _alignmentStyle;
    private GUIStyle _detailStyle;
    private GUIStyle _curveLabelStyle;
    private GUIStyle _scaleStyle;
    private GUIStyle _legendStyle;
    private GUIStyle _rewardTitleStyle;
    private GUIStyle _rewardLabelStyle;
    private GUIStyle _rewardTotalStyle;
    private GUIStyle _rangeStyle;
    private GUIStyle _gridLabelStyle;
    private GUIStyle _errorStyle;
    private bool _stylesInitialized = false;

    // 缓存临时数组
    private float[] _sensorValues = new float[6];
    private static readonly string[] WheelNames = { "FL", "RL", "RR", "FR" };
    private static readonly string[] SensorLabels = { "前左", "前中", "前右", "后左", "后中", "后右" };

    void Start()
    {
        // 自动查找组件（如果未手动绑定）
        if (myCarMotion == null)
            myCarMotion = GetComponent<MyCar_Motion>();
        
        if (myCarAgent == null)
            myCarAgent = GetComponent<MyCarAgent>();
        
        if (rb == null)
            rb = GetComponent<Rigidbody>();
        
        // 初始化曲线缓冲区
        _lateralSpeedHistory = new float[curveHistoryLength];
        _angularSpeedHistory = new float[curveHistoryLength];
        _deltaXHistory = new float[curveHistoryLength];
        _deltaWHistory = new float[curveHistoryLength];
        
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
        if (_yellowTexture == null)
        {
            _yellowTexture = new Texture2D(1, 1);
            _yellowTexture.SetPixel(0, 0, Color.yellow);
            _yellowTexture.Apply();
        }
        if (_greenTexture == null)
        {
            _greenTexture = new Texture2D(1, 1);
            _greenTexture.SetPixel(0, 0, Color.green);
            _greenTexture.Apply();
        }
    }

    void InitStyles()
    {
        if (_stylesInitialized) return;
        _stylesInitialized = true;

        _errorStyle = new GUIStyle(GUI.skin.label) { normal = { textColor = Color.red } };
        _greenLabelStyle = new GUIStyle(GUI.skin.label)
        {
            normal = { textColor = Color.green },
            fontSize = 12,
            fontStyle = FontStyle.Bold
        };
        _diffStyle = new GUIStyle(GUI.skin.label)
        {
            normal = { textColor = Color.yellow },
            fontSize = 12,
            fontStyle = FontStyle.Bold
        };
        _alignmentStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 14,
            fontStyle = FontStyle.Bold
        };
        _detailStyle = new GUIStyle(GUI.skin.label) { fontSize = 11 };
        _curveLabelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 11,
            normal = { textColor = Color.white }
        };
        _scaleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 10,
            normal = { textColor = Color.gray },
            alignment = TextAnchor.MiddleRight
        };
        _legendStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 11,
            normal = { textColor = Color.white },
            fontStyle = FontStyle.Bold
        };
        _rewardTitleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 14,
            fontStyle = FontStyle.Bold
        };
        _rewardLabelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 11,
            normal = { textColor = Color.white }
        };
        _rewardTotalStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 12,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.cyan }
        };
        _rangeStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 10,
            normal = { textColor = Color.gray }
        };
        _gridLabelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 10,
            normal = { textColor = Color.gray },
            alignment = TextAnchor.MiddleRight
        };
    }

    void OnGUI()
    {
        if (!showDebugInfo) return;
        InitStyles();

        if (myCarMotion == null)
        {
            GUI.Label(new Rect(displayPosition.x, displayPosition.y, 300, 50), 
                "ERROR: myCarMotion is null! Please bind MyCar_Motion in Inspector.", 
                _errorStyle);
            return;
        }

        if (_bgTexture == null)
        {
            _bgTexture = new Texture2D(1, 1);
            _bgTexture.SetPixel(0, 0, new Color(0.2f, 0.2f, 0.2f, 0.5f));
            _bgTexture.Apply();
        }
        GUI.DrawTexture(new Rect(displayPosition.x, displayPosition.y, displaySize.x, displaySize.y), _bgTexture);

        // 记录当前输出到历史缓冲区
        if (myCarAgent != null)
        {
            _lateralSpeedHistory[_historyIndex] = myCarAgent.maxLateralSpeed > 0 
                ? (myCarMotion.vx_input / myCarAgent.maxLateralSpeed) 
                : 0f;
            _angularSpeedHistory[_historyIndex] = myCarAgent.maxOmegaDeg > 0 
                ? (myCarMotion.omega_input * Mathf.Rad2Deg / myCarAgent.maxOmegaDeg) 
                : 0f;
            // 记录输出增量（已经是归一化值 [-1, 1]）
            _deltaXHistory[_historyIndex] = myCarAgent.CurrentDeltaX;
            _deltaWHistory[_historyIndex] = myCarAgent.CurrentDeltaW;
            _historyIndex = (_historyIndex + 1) % curveHistoryLength;
        }

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

            GUILayout.Label($"{WheelNames[i]}: Speed={wheelSpeed:F2}m/s Angle={steerDeg:F1}° RPM={wheelRpm:F0} | Motor={motorTorque:F1}Nm Brake={brakeTorque:F1}Nm", 
                _greenLabelStyle, GUILayout.Width(displaySize.x - 20));
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
            
            // 显示输出增量（归一化值）
            GUILayout.Space(5);
            GUILayout.Label("═══ Agent Output Increments ═══", GUILayout.Width(displaySize.x - 20));
            float deltaXNorm = myCarAgent.CurrentDeltaX;
            float deltaWNorm = myCarAgent.CurrentDeltaW;
            
            // 计算实际增量值
            float deltaXReal = deltaXNorm * myCarAgent.maxDeltaLateralSpeed;  // m/s
            float deltaWReal = deltaWNorm * myCarAgent.maxDeltaOmegaDeg;      // deg/s
            
            GUILayout.Label($"归一化增量 - ΔVx: {deltaXNorm:F3} | Δω: {deltaWNorm:F3}", 
                _diffStyle, GUILayout.Width(displaySize.x - 20));
            GUILayout.Label($"实际增量 - ΔVx: {deltaXReal:F4} m/s | Δω: {deltaWReal:F2} deg/s", 
                _detailStyle, GUILayout.Width(displaySize.x - 20));
        }

        GUILayout.Space(10);

        // ========== 磁传感器信息 ==========
        GUILayout.Label("═══ Magnetic Sensors ═══", GUILayout.Width(displaySize.x - 20));
        
        if (tape != null && sensors != null && sensors.Length == 6)
        {
            for (int i = 0; i < 6; i++)
            {
                _sensorValues[i] = 0f;
                if (sensors[i] != null)
                {
                    Vector3 mag = tape.GetMagneticField(sensors[i].position);
                    _sensorValues[i] = mag.magnitude;
                }
            }
            
            GUILayout.Label($"{SensorLabels[0]}={_sensorValues[0]:F2} | {SensorLabels[1]}={_sensorValues[1]:F2} | {SensorLabels[2]}={_sensorValues[2]:F2}", 
                GUILayout.Width(displaySize.x - 20));
            GUILayout.Label($"{SensorLabels[3]}={_sensorValues[3]:F2} | {SensorLabels[4]}={_sensorValues[4]:F2} | {SensorLabels[5]}={_sensorValues[5]:F2}", 
                GUILayout.Width(displaySize.x - 20));
            
            // 计算并显示左右差值
            float frontDiff = _sensorValues[0] - _sensorValues[2];
            float rearDiff = _sensorValues[3] - _sensorValues[5];
            
            GUILayout.Label($"前排差值(左-右): {frontDiff:F3} | 后排差值(左-右): {rearDiff:F3}", 
                _diffStyle, GUILayout.Width(displaySize.x - 20));
            
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
                bool centerStrong = (_sensorValues[1] > centerThreshold) && (_sensorValues[4] > centerThreshold);
                
                string alignmentStatus;
                string stableStatus = "";
                
                if (isStableAligned)
                {
                    _alignmentStyle.normal.textColor = Color.green;
                    alignmentStatus = "✓ 对齐";
                    stableStatus = " [稳定]";
                }
                else if (isAligned)
                {
                    _alignmentStyle.normal.textColor = Color.yellow;
                    float progress = myCarAgent.AlignedTimer / myCarAgent.stableAlignedTime;
                    alignmentStatus = "⊙ 对齐中";
                    stableStatus = $" [确认中 {myCarAgent.AlignedTimer:F1}s / {myCarAgent.stableAlignedTime:F1}s ({progress*100:F0}%)]";
                }
                else
                {
                    _alignmentStyle.normal.textColor = Color.red;
                    alignmentStatus = "✗ 未对齐";
                    stableStatus = " [不满足对齐标准]";
                }
                
                GUILayout.Label($"对齐状态: {alignmentStatus}{stableStatus}", _alignmentStyle, GUILayout.Width(displaySize.x - 20));
                
                _detailStyle.normal.textColor = leftRightAligned ? Color.green : Color.gray;
                GUILayout.Label($"  左右对称: {(leftRightAligned ? "✓" : "✗")} (前={frontDiffAbs:F2}<{diffThreshold:F2}, 后={rearDiffAbs:F2}<{diffThreshold:F2})", 
                    _detailStyle, GUILayout.Width(displaySize.x - 20));
                
                _detailStyle.normal.textColor = centerStrong ? Color.green : Color.gray;
                GUILayout.Label($"  中心强度: {(centerStrong ? "✓" : "✗")} (前={_sensorValues[1]:F2}>{centerThreshold:F2}, 后={_sensorValues[4]:F2}>{centerThreshold:F2})", 
                    _detailStyle, GUILayout.Width(displaySize.x - 20));
            }
        }
        else
        {
            GUILayout.Label("未配置磁带或传感器", GUILayout.Width(displaySize.x - 20));
        }

        GUILayout.EndArea();

        // ========== 绘制输出曲线 ==========
        if (showOutputCurves && myCarAgent != null)
        {
            DrawOutputCurves();
            DrawIncrementCurves();
        }
        
        // ========== 绘制奖励信息 ==========
        if (showRewardInfo && myCarAgent != null && myCarAgent.enableRewardTracking)
        {
            DrawRewardComponents();
            DrawRewardCurve();
        }
    }

    /// <summary>
    /// 绘制智能体输出的横向速度和角速度曲线
    /// </summary>
    void DrawOutputCurves()
    {
        Rect curveRect = new Rect(curveAreaPosition.x, curveAreaPosition.y, curveAreaSize.x, curveAreaSize.y);
        
        // 绘制背景
        GUI.DrawTexture(curveRect, _bgTexture);
        
        // 绘制边框
        GUI.Box(curveRect, "Agent Output Curves");
        
        // 绘制图例（在标题下方）
        DrawCurveLegend(new Rect(curveRect.x + 10, curveRect.y + 25, curveRect.width - 20, 20));
        
        // 内部绘制区域（留出边距，为图例留出空间）
        Rect innerRect = new Rect(curveRect.x + 10, curveRect.y + 45, curveRect.width - 20, curveRect.height - 55);
        
        // 绘制网格和曲线
        DrawCurveGraph(innerRect);
    }

    /// <summary>
    /// 绘制智能体输出增量曲线
    /// </summary>
    void DrawIncrementCurves()
    {
        Rect curveRect = new Rect(incrementCurvePosition.x, incrementCurvePosition.y, incrementCurveSize.x, incrementCurveSize.y);
        
        // 绘制背景
        GUI.DrawTexture(curveRect, _bgTexture);
        
        // 绘制边框
        GUI.Box(curveRect, "Agent Output Increments");
        
        // 绘制图例（在标题下方）
        DrawIncrementLegend(new Rect(curveRect.x + 10, curveRect.y + 25, curveRect.width - 20, 20));
        
        // 内部绘制区域（留出边距，为图例留出空间）
        Rect innerRect = new Rect(curveRect.x + 10, curveRect.y + 45, curveRect.width - 20, curveRect.height - 55);
        
        // 绘制网格和曲线
        DrawIncrementGraph(innerRect);
    }

    void DrawCurveGraph(Rect graphRect)
    {
        // 获取当前值（正规化到 -1 ~ 1）
        float currentLateralNorm = myCarAgent.maxLateralSpeed > 0 
            ? (myCarMotion.vx_input / myCarAgent.maxLateralSpeed) 
            : 0f;
        float currentAngularNorm = myCarAgent.maxOmegaDeg > 0 
            ? (myCarMotion.omega_input * Mathf.Rad2Deg / myCarAgent.maxOmegaDeg) 
            : 0f;
        
        // 绘制坐标轴和网格
        DrawGraphGrid(graphRect);
        
        // 绘制两条曲线（需要先clamp值）
        DrawCurveLineWithColor(graphRect, _lateralSpeedHistory, Color.cyan, "Lateral Vx");
        DrawCurveLineWithColor(graphRect, _angularSpeedHistory, Color.magenta, "Angular ω");
        
        float labelX = graphRect.x + 10;
        float labelY = graphRect.yMax + 5;

        float currentLateralReal = myCarMotion.vx_input;
        float currentAngularReal = myCarMotion.omega_input * Mathf.Rad2Deg;
        
        GUILayout.BeginArea(new Rect(labelX, labelY, 360, 50));
        GUILayout.Label($"归一化 - 横向速度(Vx): {currentLateralNorm:F2} | 角速度(ω): {currentAngularNorm:F2}", _curveLabelStyle);
        GUILayout.Label($"真实值 - 横向速度(Vx): {currentLateralReal:F2} m/s | 角速度(ω): {currentAngularReal:F1} deg/s", _curveLabelStyle);
        GUILayout.EndArea();
    }

    void DrawIncrementGraph(Rect graphRect)
    {
        // 获取当前增量值（已经是归一化值 [-1, 1]）
        float currentDeltaXNorm = myCarAgent.CurrentDeltaX;
        float currentDeltaWNorm = myCarAgent.CurrentDeltaW;
        
        // 绘制坐标轴和网格
        DrawGraphGrid(graphRect);
        
        // 绘制两条增量曲线
        DrawCurveLineWithColor(graphRect, _deltaXHistory, Color.yellow, "ΔVx");
        DrawCurveLineWithColor(graphRect, _deltaWHistory, Color.green, "Δω");
        
        float labelX = graphRect.x + 10;
        float labelY = graphRect.yMax + 5;

        // 计算实际增量值
        float currentDeltaXReal = currentDeltaXNorm * myCarAgent.maxDeltaLateralSpeed;  // m/s
        float currentDeltaWReal = currentDeltaWNorm * myCarAgent.maxDeltaOmegaDeg;      // deg/s
        
        GUILayout.BeginArea(new Rect(labelX, labelY, 360, 50));
        GUILayout.Label($"归一化增量 - ΔVx: {currentDeltaXNorm:F3} | Δω: {currentDeltaWNorm:F3}", _curveLabelStyle);
        GUILayout.Label($"实际增量 - ΔVx: {currentDeltaXReal:F4} m/s | Δω: {currentDeltaWReal:F2} deg/s", _curveLabelStyle);
        GUILayout.EndArea();
    }

    void DrawGraphGrid(Rect graphRect)
    {
        // 绘制Y轴刻度和标签
        DrawYAxisLabels(graphRect);
        
        // 绘制中心线（0值）
        DrawLine(new Vector2(graphRect.x, graphRect.center.y), new Vector2(graphRect.xMax, graphRect.center.y), Color.gray);
        
        // 绘制 ±1 线
        float topY = graphRect.y + graphRect.height * 0.1f;  // +1
        float bottomY = graphRect.yMax - graphRect.height * 0.1f;  // -1
        Color gridColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        DrawLine(new Vector2(graphRect.x, topY), new Vector2(graphRect.xMax, topY), gridColor);
        DrawLine(new Vector2(graphRect.x, bottomY), new Vector2(graphRect.xMax, bottomY), gridColor);
        
        // 绘制 ±0.5 虚线
        float midTopY = graphRect.center.y - graphRect.height * 0.25f;  // +0.5
        float midBottomY = graphRect.center.y + graphRect.height * 0.25f;  // -0.5
        Color faintGridColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);
        DrawLine(new Vector2(graphRect.x, midTopY), new Vector2(graphRect.xMax, midTopY), faintGridColor);
        DrawLine(new Vector2(graphRect.x, midBottomY), new Vector2(graphRect.xMax, midBottomY), faintGridColor);
    }
    
    private static readonly float[] YAxisValues = { 1f, 0.5f, 0f, -0.5f, -1f };

    void DrawYAxisLabels(Rect graphRect)
    {
        foreach (float val in YAxisValues)
        {
            float screenY = graphRect.center.y - val * (graphRect.height / 2);
            screenY = Mathf.Clamp(screenY, graphRect.y, graphRect.yMax);
            
            Rect labelRect = new Rect(graphRect.x - 40, screenY - 10, 35, 20);
            GUI.Label(labelRect, val.ToString("F1"), _scaleStyle);
        }
    }

    /// <summary>
    /// 绘制图例，标明哪条曲线是哪个
    /// </summary>
    void DrawCurveLegend(Rect legendRect)
    {
        DrawLegendItem(new Rect(legendRect.x, legendRect.y, 150, 18), Color.cyan, "横向速度 (Vx)", _legendStyle);
        DrawLegendItem(new Rect(legendRect.x + 160, legendRect.y, 150, 18), Color.magenta, "角速度 (ω)", _legendStyle);
    }

    /// <summary>
    /// 绘制增量曲线图例
    /// </summary>
    void DrawIncrementLegend(Rect legendRect)
    {
        DrawLegendItem(new Rect(legendRect.x, legendRect.y, 150, 18), Color.yellow, "横向增量 (ΔVx)", _legendStyle);
        DrawLegendItem(new Rect(legendRect.x + 160, legendRect.y, 150, 18), Color.green, "角速度增量 (Δω)", _legendStyle);
    }
    
    /// <summary>
    /// 绘制单个图例项（颜色块 + 文字）
    /// </summary>
    void DrawLegendItem(Rect rect, Color color, string label, GUIStyle style)
    {
        Rect colorRect = new Rect(rect.x, rect.y + 2, 12, 12);
        Texture2D colorTex = null;
        
        if (color == Color.cyan && _cyanTexture != null)
            colorTex = _cyanTexture;
        else if (color == Color.magenta && _magentaTexture != null)
            colorTex = _magentaTexture;
        
        if (colorTex != null)
            GUI.DrawTexture(colorRect, colorTex);
        
        Rect labelRect = new Rect(rect.x + 16, rect.y, rect.width - 16, rect.height);
        style.normal.textColor = color;
        GUI.Label(labelRect, label, style);
    }

    void DrawCurveLineWithColor(Rect graphRect, float[] data, Color color, string label)
    {
        if (data == null || data.Length < 2) return;
        
        for (int i = 0; i < data.Length - 1; i++)
        {
            // 计算实际的数组索引（考虑环形缓冲）
            int idx1 = (_historyIndex + i) % data.Length;
            int idx2 = (_historyIndex + i + 1) % data.Length;
            
            // 将值 [-1, 1] 映射到屏幕坐标（严格clamp）
            float value1 = Mathf.Clamp(data[idx1], -1f, 1f);
            float value2 = Mathf.Clamp(data[idx2], -1f, 1f);
            
            float screenX1 = graphRect.x + (i / (float)(data.Length - 1)) * graphRect.width;
            float screenX2 = graphRect.x + ((i + 1) / (float)(data.Length - 1)) * graphRect.width;
            
            // 反转Y轴：上 = +1，下 = -1
            // 计算方式：graphRect.center.y 是 0 点，向上正，向下负
            float screenY1 = graphRect.center.y - value1 * (graphRect.height / 2);
            float screenY2 = graphRect.center.y - value2 * (graphRect.height / 2);
            
            // 二次clamp确保在范围内（不应该超过）
            screenY1 = Mathf.Clamp(screenY1, graphRect.y, graphRect.yMax);
            screenY2 = Mathf.Clamp(screenY2, graphRect.y, graphRect.yMax);
            
            DrawLine(new Vector2(screenX1, screenY1), new Vector2(screenX2, screenY2), color);
        }
    }
    
    /// <summary>
    /// 使用 GL 绘制直线（运行时用）
    /// </summary>
    static void EnsureLineMaterial()
    {
        if (_glLineMaterial == null)
        {
            _glLineMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
            _glLineMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
    }

    void DrawLine(Vector2 start, Vector2 end, Color color)
    {
        EnsureLineMaterial();

        GL.PushMatrix();
        GL.LoadOrtho();
        
        start.x /= Screen.width;
        start.y /= Screen.height;
        end.x /= Screen.width;
        end.y /= Screen.height;
        
        start.y = 1f - start.y;
        end.y = 1f - end.y;
        
        _glLineMaterial.SetPass(0);
        
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

        _rewardTitleStyle.normal.textColor = cumulativeReward >= 0 ? Color.green : Color.red;
        GUILayout.Label($"累计奖励: {cumulativeReward:F3}", _rewardTitleStyle);
        GUILayout.Space(5);

        float alignmentSpeedReward = components.alignmentReward * components.speedCoefficient;
        _rewardLabelStyle.normal.textColor = alignmentSpeedReward >= 0 ? Color.green : Color.red;
        GUILayout.Label($"1. 对齐奖励×速度系数: {alignmentSpeedReward:F4} (对齐={components.alignmentReward:F3} × 速度={components.speedCoefficient:F3})", 
            _rewardLabelStyle, GUILayout.Width(rewardRect.width - 20));
        
        _rewardLabelStyle.normal.textColor = Color.Lerp(Color.green, Color.red, components.straightOutputPenaltyPercent / 100f);
        GUILayout.Label($"2. 直线输出限制: {components.straightOutputPenalty:F4} (惩罚: {components.straightOutputPenaltyPercent:F1}%)", 
            _rewardLabelStyle, GUILayout.Width(rewardRect.width - 20));
        
        _rewardLabelStyle.normal.textColor = components.velocityChangeRatePenalty < 0 ? Color.red : Color.green;
        GUILayout.Label($"3. 速度变化率惩罚: {components.velocityChangeRatePenalty:F4}", 
            _rewardLabelStyle, GUILayout.Width(rewardRect.width - 20));
        
        _rewardLabelStyle.normal.textColor = components.warningPenalty < 0 ? Color.red : Color.green;
        GUILayout.Label($"4. 预警区域惩罚: {components.warningPenalty:F4}", 
            _rewardLabelStyle, GUILayout.Width(rewardRect.width - 20));
        
        GUILayout.Space(5);
        
        _rewardTotalStyle.normal.textColor = Color.cyan;
        GUILayout.Label($"总奖励(×dt前): {components.totalReward:F4}", _rewardTotalStyle, GUILayout.Width(rewardRect.width - 20));
        _rewardTotalStyle.normal.textColor = components.rewardThisFrame >= 0 ? Color.green : Color.red;
        GUILayout.Label($"本帧合计(×dt后): {components.rewardThisFrame:F4}", _rewardTotalStyle, GUILayout.Width(rewardRect.width - 20));
        
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
        
        GUI.Label(new Rect(innerRect.x, innerRect.yMax + 5, innerRect.width, 20), 
            $"Range: [{minReward:F2}, {maxReward:F2}]", _rangeStyle);
    }
    
    void DrawRewardGrid(Rect graphRect, float minVal, float maxVal)
    {
        // 绘制0线
        float zeroY = Mathf.Lerp(graphRect.yMax, graphRect.y, 
            (0f - minVal) / (maxVal - minVal));
        zeroY = Mathf.Clamp(zeroY, graphRect.y, graphRect.yMax);
        DrawLine(new Vector2(graphRect.x, zeroY), new Vector2(graphRect.xMax, zeroY), Color.gray);
        
        float[] labelValues = { maxVal, (maxVal + minVal) / 2f, minVal };
        foreach (float val in labelValues)
        {
            float screenY = Mathf.Lerp(graphRect.yMax, graphRect.y, (val - minVal) / (maxVal - minVal));
            screenY = Mathf.Clamp(screenY, graphRect.y, graphRect.yMax);
            Rect labelRect = new Rect(graphRect.x - 50, screenY - 10, 45, 20);
            GUI.Label(labelRect, val.ToString("F2"), _gridLabelStyle);
        }
    }
    
    void DrawRewardCurveLine(Rect graphRect, float[] data, int length, int startIndex, float minVal, float maxVal)
    {
        if (data == null || length < 2) return;
        
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
        _stylesInitialized = false;

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
        if (_yellowTexture != null)
        {
            Destroy(_yellowTexture);
            _yellowTexture = null;
        }
        if (_greenTexture != null)
        {
            Destroy(_greenTexture);
            _greenTexture = null;
        }
        if (_glLineMaterial != null)
        {
            Destroy(_glLineMaterial);
            _glLineMaterial = null;
        }
    }
}
