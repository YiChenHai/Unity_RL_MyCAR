using UnityEngine;

/// <summary>
/// 模糊控制器测试脚本
/// 功能：采集传感器数据 → 模糊推理 → 控制车辆
/// 使用由 generate_fuzzy_rules.py 自动生成的规则库 .cs（按文件名选择，类名从文件中读取）
/// 
/// 用法：
///   1. 运行 generate_fuzzy_rules.py，--output 指向 Scripts/RuleLib/规则库名.cs
///   2. 将本脚本挂到小车上，配置传感器和引用
///   3. ruleLibrary 填入要使用的规则库名（ruleLibraryFolder 中的文件名，不含 .cs）；其他位置的同名类不会被使用
///   4. 运行测试
/// </summary>
public class MyCar_Agent_FuzzyTest : MonoBehaviour
{
    [Header("Rule Library")]
    [Tooltip("规则库文件夹，只使用此文件夹中的规则库")]
    public string ruleLibraryFolder = "Assets/Scripts/RuleLib";
    [Tooltip("当前使用的规则库：规则库文件夹中 .cs 的文件名，不含扩展名。\n" +
             "文件可改名；但文件夹中各规则库的类名必须互不相同，否则无法编译")]
    public string ruleLibrary = "FuzzyController041105";

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

    [Header("Rate Limiter（与 LabVIEW 变化率限制器一致）")]
    [Tooltip("开启后在平滑之后按每步最大变化量限速（AbsoluteRateLimited 模式蒸馏的规则库应开启）")]
    public bool enableRateLimiter = false;
    [Tooltip("每个 FixedUpdate 横向速度最大变化量 m/s（= LabVIEW vxRateMax × dt）")]
    public float maxDeltaLateralSpeed = 0.02f;
    [Tooltip("每个 FixedUpdate 角速度最大变化量 deg/s（= LabVIEW omRateMax(deg) × dt）")]
    public float maxDeltaOmegaDeg = 4f;

    [Header("Control Delay（测试用）")]
    [Tooltip("固定控制延迟步数，用于检验规则库对实车延迟的鲁棒性；0=无延迟")]
    [Range(0, 10)]
    public int controlDelaySteps = 0;
    
    [Header("Debug")]
    [Tooltip("显示模糊控制器输出")]
    public bool showDebugInfo = false;
    [Tooltip("显示传感器原始值")]
    public bool showSensorValues = false;
    
    // 规则库 EvaluateFromSensors 的签名
    private delegate void EvaluateFromSensorsFn(
        float sensorFL, float sensorFC, float sensorFR,
        float sensorRL, float sensorRC, float sensorRR,
        float maxField, out float outputVx, out float outputOmega);

    // 内部状态
    private EvaluateFromSensorsFn evaluateFromSensors;
    private float smoothedVx = 0f;
    private float smoothedOmega = 0f;
    private float maxOmegaRad;
    private float limitedVx = 0f;
    private float limitedOmega = 0f;
    private Vector2[] delayBuffer;
    private int delayHead = 0;
    
    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        maxOmegaRad = maxOmegaDeg * Mathf.Deg2Rad;
        
        // 验证引用
        if (tape == null) Debug.LogError("[FuzzyTest] MagneticTape 未分配！");
        if (myCarMotion == null) Debug.LogError("[FuzzyTest] MyCar_Motion 未分配！");
        if (sensors == null || sensors.Length != 6) Debug.LogError("[FuzzyTest] Sensors 数组未正确配置！需要6个传感器");

        string libraryPath = FindRuleLibraryPath(ruleLibraryFolder, ruleLibrary);
        if (libraryPath == null)
        {
            Debug.LogError($"[FuzzyTest] 规则库文件夹 {ruleLibraryFolder} 中没有 {ruleLibrary}.cs");
            enabled = false;
            return;
        }

        string className = ReadClassName(libraryPath);
        System.Type type = System.Type.GetType(className);
        var method = type?.GetMethod("EvaluateFromSensors",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
        if (method == null)
        {
            Debug.LogError($"[FuzzyTest] {libraryPath} 中的类 {className} 不可用，或缺少 EvaluateFromSensors");
            enabled = false;
            return;
        }
        evaluateFromSensors = (EvaluateFromSensorsFn)System.Delegate.CreateDelegate(typeof(EvaluateFromSensorsFn), method);

        string info = type.GetMethod("GetInfo")?.Invoke(null, null) as string ?? className;
        Debug.Log($"[FuzzyTest] 当前规则库: {ruleLibrary}（类名 {className}）\n路径: {libraryPath}\n{info}");
    }

    /// <summary>读取规则库文件中定义的类名（文件可被改名，类名不变）；构建版本中无源文件，按文件名作类名</summary>
    string ReadClassName(string path)
    {
#if UNITY_EDITOR
        var match = System.Text.RegularExpressions.Regex.Match(
            System.IO.File.ReadAllText(path), @"^\s*public\s+(?:static\s+)?class\s+(\w+)",
            System.Text.RegularExpressions.RegexOptions.Multiline);
        if (match.Success)
            return match.Groups[1].Value;
#endif
        return ruleLibrary;
    }

    /// <summary>只在 folder 中查找名为 name.cs 的规则库，找不到返回 null</summary>
    static string FindRuleLibraryPath(string folder, string name)
    {
#if UNITY_EDITOR
        if (!UnityEditor.AssetDatabase.IsValidFolder(folder))
            return null;
        foreach (string guid in UnityEditor.AssetDatabase.FindAssets(name + " t:MonoScript", new[] { folder }))
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            if (System.IO.Path.GetFileNameWithoutExtension(path) == name)
                return path;
        }
        return null;
#else
        return folder + "/" + name + ".cs";
#endif
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
        evaluateFromSensors(
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

        // ========== 步骤5：变化率限制 ==========
        if (enableRateLimiter)
        {
            float dW = maxDeltaOmegaDeg * Mathf.Deg2Rad;
            limitedVx += Mathf.Clamp(smoothedVx - limitedVx, -maxDeltaLateralSpeed, maxDeltaLateralSpeed);
            limitedOmega += Mathf.Clamp(smoothedOmega - limitedOmega, -dW, dW);
        }
        else
        {
            limitedVx = smoothedVx;
            limitedOmega = smoothedOmega;
        }

        // ========== 步骤6：控制延迟 + 下发控制 ==========
        Vector2 cmd = new Vector2(limitedVx, limitedOmega);
        if (controlDelaySteps > 0)
        {
            if (delayBuffer == null || delayBuffer.Length != controlDelaySteps + 1)
            {
                delayBuffer = new Vector2[controlDelaySteps + 1];
                delayHead = 0;
            }
            delayBuffer[delayHead] = cmd;
            cmd = delayBuffer[(delayHead + 1) % delayBuffer.Length];
            delayHead = (delayHead + 1) % delayBuffer.Length;
        }
        if (myCarMotion != null)
        {
            myCarMotion.SetControl(constantForwardSpeed, cmd.x, cmd.y);
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
        limitedVx = 0f;
        limitedOmega = 0f;
        if (delayBuffer != null) System.Array.Clear(delayBuffer, 0, delayBuffer.Length);
        delayHead = 0;
    }
}
