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
    public Vector2 displayPosition = new Vector2(10, 10);
    public Vector2 displaySize = new Vector2(500, 550);

    private static Texture2D _bgTexture; // 静态背景纹理，避免每帧创建

    void Start()
    {
        // 自动查找组件（如果未手动绑定）
        if (myCarMotion == null)
            myCarMotion = GetComponent<MyCar_Motion>();
        
        if (myCarAgent == null)
            myCarAgent = GetComponent<MyCarAgent>();
        
        if (rb == null)
            rb = GetComponent<Rigidbody>();
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
