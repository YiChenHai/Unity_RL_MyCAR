using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 决策树蒸馏测试脚本
/// 功能：采集观测数据 → 决策树预测 → 控制车辆
/// 参考 MyCar_Agent.cs 的接口，使用 if_rules.cs 的决策树，输出给 MyCar_Motion.cs
/// </summary>
public class MyCar_Agent_DistillationTest : MonoBehaviour
{
    [Header("Decision Tree Model")]
    [Tooltip("决策树规则库实例（从 if_rules.cs 生成）\n注意：DecisionTreeRules是普通C#类，不需要在Inspector中分配，代码会自动创建实例")]
    public DecisionTreeRules decisionTree;  // 通常为null，代码会自动创建实例
    
    [Header("Refs (参考 MyCar_Agent.cs)")]
    public MagneticTape tape;
    [Tooltip("传感器顺序: [0]=前左, [1]=前中, [2]=前右, [3]=后左, [4]=后中, [5]=后右")]
    public Transform[] sensors = new Transform[6];
    public Rigidbody rb;
    public MyCar_Motion myCarMotion;
    
    [Header("Control limits (与 MyCar_Agent.cs 保持一致)")]
    public float constantForwardSpeed = 0.2f;  // vz 固定前进速度 m/s
    public float maxLateralSpeed = 0.1f;       // vx (横向速度) m/s
    public float maxOmegaDeg = 120f;            // omega (自转角速度) deg/s
    
    [Header("Normalization")]
    public float maxField = 8f;                // 磁场最大值
    
    [Header("Output Smoothing")]
    [Range(0f, 1f)]
    public float smoothingAlpha = 0.3f;  // 指数平滑系数（与 MyCar_Agent.cs 一致）
    
    [Header("Debug")]
    [Tooltip("显示决策树预测的动作值")]
    public bool showPredictedActions = false;
    [Tooltip("显示观测值")]
    public bool showObservations = false;
    
    // ===== 内部状态（与 MyCar_Agent.cs 保持一致） =====
    private float[] currentObservations = new float[13];  // 当前13维观测值
    private float lastOutputLateralSpeed = 0f;  // 上一次输出的横向速度
    private float lastOutputAngularSpeed = 0f;  // 上一次输出的自转速度
    private float smoothedLateralSpeed = 0f;   // 平滑后的横向速度
    private float smoothedAngularSpeed = 0f;   // 平滑后的角速度
    private float lastRawLateralAction = 0f;    // 上一帧的原始决策树输出（横向）
    private float lastRawAngularAction = 0f;    // 上一帧的原始决策树输出（角速度）
    
    void Start()
    {
        // 初始化 Rigidbody
        if (rb == null) rb = GetComponent<Rigidbody>();
        
        // 初始化决策树规则库
        // 注意：DecisionTreeRules是普通C#类（不是MonoBehaviour），不需要在Inspector中分配
        // 代码会自动创建实例，这是正常行为
        if (decisionTree == null)
        {
            decisionTree = new DecisionTreeRules();
            Debug.Log("[DistillationTest] DecisionTreeRules 已自动创建实例（这是正常行为，无需在Inspector中分配）");
        }
        else
        {
            Debug.Log("[DistillationTest] DecisionTreeRules 使用Inspector中分配的实例");
        }
        
        // 注意：修改 if_rules.cs 后，Unity会自动重新编译，无需手动重新绑定
        Debug.Log("[DistillationTest] 提示：如果修改了 if_rules.cs，Unity会自动使用最新编译的版本");
        
        // 验证关键引用
        if (tape == null) Debug.LogError("[DistillationTest] MagneticTape 未分配！");
        if (myCarMotion == null) Debug.LogError("[DistillationTest] MyCar_Motion 未分配！");
        if (sensors == null || sensors.Length != 6) Debug.LogError("[DistillationTest] Sensors 数组未正确配置！");
        
        Debug.Log($"[DistillationTest] 初始化完成 - 决策树: {(decisionTree != null ? "已分配" : "未分配")}, " +
                 $"Motion: {(myCarMotion != null ? "已分配" : "未分配")}, " +
                 $"Tape: {(tape != null ? "已分配" : "未分配")}");
    }
    
    void FixedUpdate()
    {
        // ========== 步骤1：采集观测数据（参考 MyCar_Agent.cs 的 CollectObservations） ==========
        CollectObservations();
        
        // ========== 步骤2：决策树预测（使用 if_rules.cs 的 DecisionTreeRules） ==========
        if (decisionTree != null)
        {
            // 设置观测值到决策树
            decisionTree.SetObservations(currentObservations);
            
            // 获取预测结果（范围 -1 到 1）
            float predictedActionX = decisionTree.PredictActionX();
            float predictedActionW = decisionTree.PredictActionW();
            
            // 保存原始预测值（用于下一帧观测）
            lastRawLateralAction = predictedActionX;
            lastRawAngularAction = predictedActionW;
            
            // ========== 步骤3：动作后处理（参考 MyCar_Agent.cs 的平滑逻辑） ==========
            // 映射到物理量
            float rawLateralSpeed = predictedActionX * maxLateralSpeed;
            float rawAngularSpeed = predictedActionW * maxOmegaDeg * Mathf.Deg2Rad;
            
            // 应用指数平滑（与 MyCar_Agent.cs 一致）
            smoothedLateralSpeed = Mathf.Lerp(smoothedLateralSpeed, rawLateralSpeed, smoothingAlpha);
            smoothedAngularSpeed = Mathf.Lerp(smoothedAngularSpeed, rawAngularSpeed, smoothingAlpha);
            
            // 保存输出（用于下一次观察）
            lastOutputLateralSpeed = smoothedLateralSpeed;
            lastOutputAngularSpeed = smoothedAngularSpeed;
            
            // 调试输出
            if (showPredictedActions)
            {
                Debug.Log($"[DistillationTest] Predicted: action_x={predictedActionX:F4}, action_w={predictedActionW:F4} | " +
                         $"Smoothed: vx={smoothedLateralSpeed:F4}, omega={smoothedAngularSpeed:F4}");
            }
            
            // ========== 步骤4：输出控制给 MyCar_Motion.cs ==========
            float vz = constantForwardSpeed;  // 固定前进速度
            if (myCarMotion != null)
            {
                // 调用 MyCar_Motion.SetControl() 方法
                myCarMotion.SetControl(vz, smoothedLateralSpeed, smoothedAngularSpeed);
            }
            else
            {
                if (Time.frameCount % 100 == 0)
                {
                    Debug.LogWarning("[DistillationTest] myCarMotion 未分配！车辆无法移动");
                }
            }
        }
        else
        {
            Debug.LogError("[DistillationTest] DecisionTreeRules 未初始化！");
        }
    }
    
    /// <summary>
    /// 采集观测数据（完全参考 MyCar_Agent.cs 的 CollectObservations 逻辑）
    /// 13维观测：
    ///   0-5:  六个传感器的归一化磁场强度
    ///   6-7:  上一次输出的平滑后的横向速度和自转速度
    ///   8-9:  上一帧的原始决策树输出动作
    ///   10-12: 车身实际运动状态（前进速度、横向速度、角速度）
    /// </summary>
    void CollectObservations()
    {
        // 1-6: 六个传感器的归一化强度（环境感知）
        for (int i = 0; i < sensors.Length; i++)
        {
            if (sensors[i] != null && tape != null)
            {
                Vector3 mag = tape.GetMagneticField(sensors[i].position);
                currentObservations[i] = Mathf.Clamp01(mag.magnitude / Mathf.Max(1e-9f, maxField));
            }
            else
            {
                currentObservations[i] = 0f;
            }
        }
        
        // 7-8: 智能体上一次输出的平滑后的横向速度和自转速度（决策记忆）
        float maxOmegaRad = maxOmegaDeg * Mathf.Deg2Rad;
        currentObservations[6] = Mathf.Clamp(lastOutputLateralSpeed / Mathf.Max(0.001f, maxLateralSpeed), -2f, 2f);
        currentObservations[7] = Mathf.Clamp(lastOutputAngularSpeed / maxOmegaRad, -2f, 2f);
        
        // 9-10: 原始决策树输出动作（用于感知平滑延迟）
        currentObservations[8] = Mathf.Clamp(lastRawLateralAction, -1f, 1f);
        currentObservations[9] = Mathf.Clamp(lastRawAngularAction, -1f, 1f);
        
        // 11-13: 车身实际运动状态（物理反馈）
        Vector3 localVel = transform.InverseTransformDirection(rb != null ? rb.linearVelocity : Vector3.zero);
        float angularVel = rb != null ? rb.angularVelocity.y : 0f;
        
        currentObservations[10] = Mathf.Clamp(localVel.z / Mathf.Max(0.001f, constantForwardSpeed), -2f, 2f);
        currentObservations[11] = Mathf.Clamp(localVel.x / Mathf.Max(0.001f, maxLateralSpeed), -2f, 2f);
        currentObservations[12] = Mathf.Clamp(angularVel / maxOmegaRad, -2f, 2f);
        
        // 调试输出
        if (showObservations)
        {
            string obsStr = "Observations: ";
            for (int i = 0; i < currentObservations.Length; i++)
            {
                obsStr += $"obs[{i}]={currentObservations[i]:F4} ";
            }
            Debug.Log(obsStr);
        }
    }
    
    /// <summary>
    /// 获取当前观测值（供外部访问）
    /// </summary>
    public float[] GetCurrentObservations()
    {
        return (float[])currentObservations.Clone();
    }
    
    /// <summary>
    /// 获取决策树预测的动作（供外部访问）
    /// </summary>
    public (float actionX, float actionW) GetPredictedActions()
    {
        if (decisionTree != null)
        {
            decisionTree.SetObservations(currentObservations);
            return decisionTree.Predict();
        }
        return (0f, 0f);
    }
    
    /// <summary>
    /// 重置状态（用于新回合开始）
    /// </summary>
    public void ResetState()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        
        // 重置状态变量
        lastOutputLateralSpeed = 0f;
        lastOutputAngularSpeed = 0f;
        smoothedLateralSpeed = 0f;
        smoothedAngularSpeed = 0f;
        lastRawLateralAction = 0f;
        lastRawAngularAction = 0f;
    }
}
