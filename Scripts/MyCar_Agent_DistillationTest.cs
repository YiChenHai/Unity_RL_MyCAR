using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 决策树蒸馏测试脚本
/// 功能：采集观测数据 → 决策树预测 → 控制车辆
/// 参考 MyCar_Agent.cs 的接口，使用 DecisionTreeRules.cs 的决策树，输出给 MyCar_Motion.cs
/// </summary>
public class MyCar_Agent_DistillationTest : MonoBehaviour
{
    [Header("Decision Tree Model")]
    [Tooltip("决策树规则库实例（从 DecisionTreeRules.cs 生成）\n注意：DecisionTreeRules是普通C#类，不需要在Inspector中分配，代码会自动创建实例")]
    public DecisionTreeRules decisionTree;  // 通常为null，代码会自动创建实例
    
    [Header("Refs (参考 MyCar_Agent.cs)")]
    public MagneticTape tape;
    [Tooltip("传感器顺序: [0]=前左, [1]=前中, [2]=前右, [3]=后左, [4]=后中, [5]=后右")]
    public Transform[] sensors = new Transform[6];
    public Rigidbody rb;
    public MyCar_Motion myCarMotion;
    
    [Header("Control limits (与 MyCar_Agent.cs 保持一致)")]
    public float constantForwardSpeed = 0.2f;  // vz 固定前进速度 m/s
    public float maxLateralSpeed = 0.15f;       // vx (横向速度) m/s
    public float maxOmegaDeg = 80f;            // omega (自转角速度) deg/s
    
    [Header("Discrete Action Space (与 MyCar_Agent.cs 保持一致)")]
    [Tooltip("急左转/急右转的角速度值(归一化-1~1)")]
    [Range(0f, 1f)]
    public float turnSharpAngularValue = 0.5625f;
    [Tooltip("大左转/大右转的角速度值（归一化，-1~1）")]
    [Range(0f, 1f)]
    public float turnLargeAngularValue = 0.4375f;
    [Tooltip("普通左转/右转的角速度值(归一化-1~1)")]
    [Range(0f, 1f)]
    public float turnAngularValue = 0.25f;
    [Tooltip("小左转/小右转的角速度值（归一化，-1~1）")]
    [Range(0f, 1f)]
    public float turnSmallAngularValue = 0.125f;
    [Tooltip("微左转/微右转的角速度值（归一化，-1~1）")]
    [Range(0f, 1f)]
    public float turnMicroAngularValue = 0.0625f;
    
    [Header("Normalization")]
    public float maxField = 8f;                // 磁场最大值
    
    [Header("Output Smoothing")]
    [Range(0f, 1f)]
    public float smoothingAlpha = 0.4f;  // 指数平滑系数（与 MyCar_Agent.cs 一致）
    
    [Header("Debug")]
    [Tooltip("显示决策树预测的动作值")]
    public bool showPredictedActions = false;
    [Tooltip("显示观测值")]
    public bool showObservations = false;
    
    // ===== 内部状态（与 MyCar_Agent.cs 保持一致） =====
    private float[] currentObservations = new float[10];  // 当前10维观测值（与 MyCar_Agent.cs 一致）
    private int lastDiscreteAction = 5;  // 上次输出的离散动作索引（初始化为Forward=5）
    private float lastOutputLateralSpeed = 0f;  // 上一次输出的横向速度
    private float lastOutputAngularSpeed = 0f;  // 上一次输出的自转速度
    private float smoothedLateralSpeed = 0f;   // 平滑后的横向速度
    private float smoothedAngularSpeed = 0f;   // 平滑后的角速度
    
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
        
        // 注意：修改 DecisionTreeRules.cs 后，Unity会自动重新编译，无需手动重新绑定
        Debug.Log("[DistillationTest] 提示：如果修改了 DecisionTreeRules.cs，Unity会自动使用最新编译的版本");
        
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
        
        // ========== 步骤2：决策树预测（使用 DecisionTreeRules.cs 的决策树） ==========
        if (decisionTree != null)
        {
            // 设置观测值到决策树（10维）
            decisionTree.SetObservations(currentObservations);
            
            // 获取预测结果（离散动作索引 0-10）
            int discreteAction = decisionTree.PredictDiscreteAction();
            discreteAction = Mathf.Clamp(discreteAction, 0, 10);  // 边界检查
            
            // 将离散动作映射到连续输出值（参考 MyCar_Agent.cs 的 MapDiscreteActionToContinuous）
            float a_x, a_w;
            MapDiscreteActionToContinuous(discreteAction, out a_x, out a_w);
            
            // ========== 步骤3：动作后处理（参考 MyCar_Agent.cs 的平滑逻辑） ==========
            // 映射到物理量
            float rawLateralSpeed = a_x * maxLateralSpeed;
            float rawAngularSpeed = a_w * maxOmegaDeg * Mathf.Deg2Rad;
            
            // 应用指数平滑（与 MyCar_Agent.cs 一致）
            smoothedLateralSpeed = Mathf.Lerp(smoothedLateralSpeed, rawLateralSpeed, smoothingAlpha);
            smoothedAngularSpeed = Mathf.Lerp(smoothedAngularSpeed, rawAngularSpeed, smoothingAlpha);
            
            // 保存输出（用于下一次观察）
            lastOutputLateralSpeed = smoothedLateralSpeed;
            lastOutputAngularSpeed = smoothedAngularSpeed;
            lastDiscreteAction = discreteAction;  // 保存离散动作索引
            
            // 调试输出
            if (showPredictedActions)
            {
                string actionName = GetActionName(discreteAction);
                Debug.Log($"[DistillationTest] Predicted: discrete_action={discreteAction}({actionName}), a_x={a_x:F4}, a_w={a_w:F4} | " +
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
    /// 观测空间：10维（与 MyCar_Agent.cs 完全一致）
    ///   索引0-5:  六个传感器的归一化磁场强度（sensor0-sensor5）
    ///   索引6:    上次输出状态（last_action_state：离散动作索引归一化到0-1范围）
    ///   索引7:    实际前进速度（actual_vz：归一化）
    ///   索引8:    实际横向速度（actual_vx：归一化）
    ///   索引9:    实际角速度（actual_omega：归一化）
    /// </summary>
    void CollectObservations()
    {
        // 观测空间：10维
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

        // 7: 上次输出状态（离散动作索引归一化到0-1范围）
        // 将离散动作索引（0-10）归一化到0-1范围
        float lastActionState = lastDiscreteAction / 10f;  // 0/10=0, 10/10=1
        currentObservations[6] = lastActionState;

        // 8-10: 车身实际运动状态（物理反馈）
        Vector3 localVel = transform.InverseTransformDirection(rb != null ? rb.linearVelocity : Vector3.zero);
        float angularVel = rb != null ? rb.angularVelocity.y : 0f;
        float maxOmegaRad = maxOmegaDeg * Mathf.Deg2Rad;
        
        currentObservations[7] = Mathf.Clamp(localVel.z / Mathf.Max(0.001f, constantForwardSpeed), -2f, 2f);  // 8: 实际前进速度
        currentObservations[8] = Mathf.Clamp(localVel.x / Mathf.Max(0.001f, maxLateralSpeed), -2f, 2f);      // 9: 实际横向速度
        currentObservations[9] = Mathf.Clamp(angularVel / maxOmegaRad, -2f, 2f);                              // 10: 实际角速度
        
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
    /// 将离散动作映射到连续输出值（参考 MyCar_Agent.cs 的 MapDiscreteActionToContinuous）
    /// </summary>
    void MapDiscreteActionToContinuous(int action, out float a_x, out float a_w)
    {
        a_x = 0f;  // 所有动作的横向速度都为0
        a_w = 0f;  // 默认角速度为0
        
        switch (action)
        {
            case 0:  // TurnLeftSharp - 急左转
                a_x = 0f;
                a_w = turnSharpAngularValue;
                break;
            case 1:  // TurnLeftLarge - 大左转
                a_x = 0f;
                a_w = turnLargeAngularValue;
                break;
            case 2:  // TurnLeft - 左转
                a_x = 0f;
                a_w = turnAngularValue;
                break;
            case 3:  // TurnLeftSmall - 小左转
                a_x = 0f;
                a_w = turnSmallAngularValue;
                break;
            case 4:  // TurnLeftMicro - 微左转
                a_x = 0f;
                a_w = turnMicroAngularValue;
                break;
            case 5:  // Forward - 直行
                a_x = 0f;
                a_w = 0f;
                break;
            case 6:  // TurnRightMicro - 微右转
                a_x = 0f;
                a_w = -turnMicroAngularValue;
                break;
            case 7:  // TurnRightSmall - 小右转
                a_x = 0f;
                a_w = -turnSmallAngularValue;
                break;
            case 8:  // TurnRight - 右转
                a_x = 0f;
                a_w = -turnAngularValue;
                break;
            case 9:  // TurnRightLarge - 大右转
                a_x = 0f;
                a_w = -turnLargeAngularValue;
                break;
            case 10: // TurnRightSharp - 急右转
                a_x = 0f;
                a_w = -turnSharpAngularValue;
                break;
            default:
                a_x = 0f;
                a_w = 0f;
                break;
        }
    }
    
    /// <summary>
    /// 获取动作名称（用于调试显示）
    /// </summary>
    string GetActionName(int action)
    {
        string[] names = { "急左转", "大左转", "左转", "小左转", "微左转", "直行", 
                         "微右转", "小右转", "右转", "大右转", "急右转" };
        if (action >= 0 && action < names.Length)
            return names[action];
        return $"未知({action})";
    }
    
    /// <summary>
    /// 获取当前观测值（供外部访问）
    /// </summary>
    public float[] GetCurrentObservations()
    {
        return (float[])currentObservations.Clone();
    }
    
    /// <summary>
    /// 获取决策树预测的离散动作（供外部访问）
    /// </summary>
    public int GetPredictedDiscreteAction()
    {
        if (decisionTree != null)
        {
            decisionTree.SetObservations(currentObservations);
            return decisionTree.PredictDiscreteAction();
        }
        return 5;  // 默认返回直行
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
        lastDiscreteAction = 5;  // 初始化为直行
        lastOutputLateralSpeed = 0f;
        lastOutputAngularSpeed = 0f;
        smoothedLateralSpeed = 0f;
        smoothedAngularSpeed = 0f;
    }
}
