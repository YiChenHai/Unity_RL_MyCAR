using UnityEngine;

/// <summary>
/// 模糊控制器测试脚本
/// 功能：采集传感器数据 → 模糊推理 → 控制车辆
/// 使用由 generate_fuzzy_rules.py 自动生成的 FuzzyController.cs
/// 
/// 用法：
///   1. 运行 generate_fuzzy_rules.py 生成 FuzzyController.cs
///   2. 将 FuzzyController.cs 放入 Scripts 文件夹
///   3. 将本脚本挂到小车上，配置传感器和引用
///   4. 运行测试
/// </summary>
public class MyCar_Agent_FuzzyTest : MonoBehaviour
{
    [Header("Refs（与 MyCar_Agent.cs 保持一致）")]
    public MagneticTape tape;
    [Tooltip("传感器顺序: [0]=前左, [1]=前中, [2]=前右, [3]=后左, [4]=后中, [5]=后右")]
    public Transform[] sensors = new Transform[6];
    public Rigidbody rb;
    public MyCar_Motion myCarMotion;
    
    [Header("Control limits（与 MyCar_Agent.cs 保持一致）")]
    public float constantForwardSpeed = 0.2f;  // vz 固定前进速度 m/s
    public float maxLateralSpeed = 0.2f;       // vx 最大横向速度 m/s
    public float maxOmegaDeg = 45f;            // omega 最大角速度 deg/s
    
    [Header("Normalization")]
    public float maxField = 8f;                // 磁场最大值
    
    [Header("Output Smoothing")]
    [Tooltip("输出平滑系数（0=无平滑，1=完全使用新值）\n建议0.3~0.5，值越小输出越平滑但响应越慢")]
    [Range(0.05f, 1f)]
    public float smoothingAlpha = 0.3f;
    
    [Header("Debug")]
    [Tooltip("显示模糊控制器输出")]
    public bool showDebugInfo = false;
    [Tooltip("显示传感器原始值")]
    public bool showSensorValues = false;
    
    // 内部状态
    private float smoothedVx = 0f;
    private float smoothedOmega = 0f;
    private float maxOmegaRad;
    
    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        maxOmegaRad = maxOmegaDeg * Mathf.Deg2Rad;
        
        // 验证引用
        if (tape == null) Debug.LogError("[FuzzyTest] MagneticTape 未分配！");
        if (myCarMotion == null) Debug.LogError("[FuzzyTest] MyCar_Motion 未分配！");
        if (sensors == null || sensors.Length != 6) Debug.LogError("[FuzzyTest] Sensors 数组未正确配置！需要6个传感器");
        
        Debug.Log($"[FuzzyTest] 模糊控制器初始化完成 - {FuzzyController.GetInfo()}");
    }
    
    void FixedUpdate()
    {
        // ========== 步骤1：读取传感器 ==========
        float[] rawSensor = new float[6];
        for (int i = 0; i < sensors.Length; i++)
        {
            if (sensors[i] != null && tape != null)
            {
                Vector3 mag = tape.GetMagneticField(sensors[i].position);
                rawSensor[i] = mag.magnitude;
            }
        }
        
        if (showSensorValues && Time.frameCount % 30 == 0)
        {
            Debug.Log($"[FuzzyTest] 传感器: FL={rawSensor[0]:F3}, FC={rawSensor[1]:F3}, FR={rawSensor[2]:F3}, " +
                     $"RL={rawSensor[3]:F3}, RC={rawSensor[4]:F3}, RR={rawSensor[5]:F3}");
        }
        
        // ========== 步骤2：模糊推理 ==========
        float outputVxNorm, outputOmegaNorm;
        FuzzyController.EvaluateFromSensors(
            rawSensor[0], rawSensor[1], rawSensor[2],  // 前左, 前中, 前右
            rawSensor[3], rawSensor[4], rawSensor[5],  // 后左, 后中, 后右
            maxField,
            out outputVxNorm, out outputOmegaNorm
        );
        
        // ========== 步骤3：映射到物理量 ==========
        float rawVx = outputVxNorm * maxLateralSpeed;
        float rawOmega = outputOmegaNorm * maxOmegaRad;
        
        // ========== 步骤4：输出平滑 ==========
        smoothedVx = Mathf.Lerp(smoothedVx, rawVx, smoothingAlpha);
        smoothedOmega = Mathf.Lerp(smoothedOmega, rawOmega, smoothingAlpha);
        
        // ========== 步骤5：下发控制 ==========
        if (myCarMotion != null)
        {
            myCarMotion.SetControl(constantForwardSpeed, smoothedVx, smoothedOmega);
        }
        
        // 调试输出
        if (showDebugInfo && Time.frameCount % 30 == 0)
        {
            Debug.Log($"[FuzzyTest] 模糊输出: vx_norm={outputVxNorm:F4}, omega_norm={outputOmegaNorm:F4} | " +
                     $"平滑后: vx={smoothedVx:F4} m/s, omega={smoothedOmega * Mathf.Rad2Deg:F2} deg/s");
        }
    }
    
    /// <summary>
    /// 重置状态（新回合或手动重置时调用）
    /// </summary>
    public void ResetState()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        smoothedVx = 0f;
        smoothedOmega = 0f;
    }
}
