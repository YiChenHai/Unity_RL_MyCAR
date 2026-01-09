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
    public Vector2 displaySize = new Vector2(500, 550);
    
    [Header("Curve Display Settings")]
    public int curveHistoryLength = 200;  // 曲线历史数据点数
    public Vector2 curveAreaPosition = new Vector2(520, 10);
    public Vector2 curveAreaSize = new Vector2(400, 250);

    private static Texture2D _bgTexture; // 静态背景纹理，避免每帧创建
    
    // 曲线数据缓冲区
    private float[] _lateralSpeedHistory;
    private float[] _angularSpeedHistory;
    private int _historyIndex = 0;

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
    }

    void OnGUI()
    {
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

        // 记录当前输出到历史缓冲区
        if (myCarAgent != null)
        {
            _lateralSpeedHistory[_historyIndex] = myCarAgent.maxLateralSpeed > 0 
                ? (myCarMotion.vx_input / myCarAgent.maxLateralSpeed) 
                : 0f;
            _angularSpeedHistory[_historyIndex] = myCarAgent.maxOmegaDeg > 0 
                ? (myCarMotion.omega_input * Mathf.Rad2Deg / myCarAgent.maxOmegaDeg) 
                : 0f;
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
        
        // 内部绘制区域（留出边距）
        Rect innerRect = new Rect(curveRect.x + 10, curveRect.y + 25, curveRect.width - 20, curveRect.height - 35);
        
        // 绘制网格和曲线
        DrawCurveGraph(innerRect);
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
        
        // 绘制当前值标签
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 11,
            normal = { textColor = Color.white }
        };
        
        float labelX = graphRect.x + 10;
        float labelY = graphRect.y - 20;
        
        GUILayout.BeginArea(new Rect(labelX, labelY, 200, 30));
        GUILayout.Label($"Vx: {currentLateralNorm:F2} | ω: {currentAngularNorm:F2}", labelStyle);
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
    
    void DrawYAxisLabels(Rect graphRect)
    {
        // Y轴刻度值
        float[] values = { 1f, 0.5f, 0f, -0.5f, -1f };
        
        GUIStyle scaleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 10,
            normal = { textColor = Color.gray },
            alignment = TextAnchor.MiddleRight
        };
        
        foreach (float val in values)
        {
            // 计算Y坐标：val = 1 在上面，val = -1 在下面
            float screenY = graphRect.center.y - val * (graphRect.height / 2);
            screenY = Mathf.Clamp(screenY, graphRect.y, graphRect.yMax);
            
            // 绘制标签（在图表左侧外部）
            Rect labelRect = new Rect(graphRect.x - 40, screenY - 10, 35, 20);
            GUI.Label(labelRect, val.ToString("F1"), scaleStyle);
        }
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
    void DrawLine(Vector2 start, Vector2 end, Color color)
    {
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
        
        var mat = new Material(Shader.Find("Hidden/Internal-Colored"));
        mat.SetPass(0);
        
        GL.Begin(GL.LINES);
        GL.Color(color);
        GL.Vertex(start);
        GL.Vertex(end);
        GL.End();
        
        GL.PopMatrix();
    }

    private void OnDestroy()
    {
        // 清理静态资源
        if (_bgTexture != null)
        {
            Destroy(_bgTexture);
            _bgTexture = null;
        }
    }
}
