using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MyCar_Motion : MonoBehaviour
{
    public enum ControlSource { Agent = 0, Manual = 1 }

    [Header("Control source")]
    public ControlSource controlSource = ControlSource.Agent;

    [Header("Wheel order: FL, RL, RR, FR")]
    public WheelCollider[] wheelColliders = new WheelCollider[4];
    public Transform[] wheelMeshes = new Transform[4];

    [Header("Vehicle geometry (m)")]
    public float wheelBase = 0.76f;   // 轴距 L
    public float trackWidth = 0.47f;  // 轮距 W

    [Header("Control inputs (body frame - Unity标准: X=横向右, Z=前进)")]
    public float vz_input = 0f;  // 前进速度 (Unity Z轴)
    public float vx_input = 0f;  // 横向速度 (Unity X轴)
    public float omega_input = 0f;  // 自转角速度

    [Header("Manual inputs (Inspector)")]
    [Tooltip("前进速度（Unity Z轴），单位 m/s")]
    public float manualVz = 0f;
    [Tooltip("横向速度（Unity X轴，右为正），单位 m/s")]
    public float manualVx = 0f;
    [Tooltip("自转角速度，单位 deg/s")]
    public float manualOmega = 0f;

    [Header("Kinematic scaling & deadzone")]
    public float inputScaleVz = 1f;  // 前进速度缩放
    public float inputScaleVx = 1f;  // 横向速度缩放
    public float inputScaleOmega = 1f;  // 角速度缩放
    public float deadzone = 0.02f;

    [Header("Wheel / Drive")]
    public float maxWheelLinearSpeed = 4.0f;
    public float maxMotorTorque = 15f;
    public float brakeTorqueHigh = 8f;
    public float brakeGain = 30f;
    [Tooltip("物理增益：每 1 Nm 扭矩产生多少 m/s 车速（从 testMode 标定）")]
    public float torqueToSpeedGain = 0.29f;

    [Header("Speed PID (per wheel)")]
    [Tooltip("作用于速度误差(m/s)，输出为扭矩修正(Nm)")]
    public float speed_Kp = 3f;
    public float speed_Ki = 1f;
    public float speed_Kd = 0f;
    public float speed_integratorLimit = 2f;
    public float speed_outputMin = -5f;
    public float speed_outputMax = 5f;
    public float speedDeadband = 0.01f;

    [Header("Steer PID (per wheel)")]
    public float steer_Kp = 40f;  // 增大比例增益，大误差时响应更快
    public float steer_Ki = 2f;   // 增大积分增益，消除稳态误差
    public float steer_Kd = 5f;   // 增大微分增益，减少超调
    public float steer_integratorLimit = 10f;
    public float maxSteerRateDeg = 360f;  // 最大转向角速度（度/秒）
    public float steerDeadbandDeg = 0.2f;

    [Header("Visual options")]
    public bool forceWheelZto90 = true;

    [Header("Debug")]
    public bool enableDebugLog = true;

    [Header("Test - 极简物理诊断")]
    [Tooltip("勾选后绕过所有PID/运动学，直接给WheelCollider施加固定扭矩")]
    public bool testMode = false;
    public float testTorque = 1.0f;

    Rigidbody rb;

    private float[] kinSteer = new float[4]; // rad
    private float[] kinSpeed = new float[4]; // m/s

    private float[] appliedSteerDeg = new float[4];
    private float[] appliedSpeed = new float[4];

    private PIDController[] speedPIDs = new PIDController[4];
    private PIDController[] steerPIDs = new PIDController[4];

    private float[] steerCmdDeg = new float[4];
    private float[] prevWheelSpeedCmd = new float[4]; // 记录前一帧的速度命令

    [HideInInspector] public float[] steerAngles = new float[4];
    [HideInInspector] public float[] wheelSpeeds = new float[4];

    private Vector2[] wheelPos = new Vector2[4];

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        // 启用插值以平滑渲染
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        for (int i = 0; i < 4; i++)
        {
            speedPIDs[i] = new PIDController(speed_Kp, speed_Ki, speed_Kd, -speed_integratorLimit, speed_integratorLimit);
            speedPIDs[i].SetOutputLimits(speed_outputMin, speed_outputMax);

            steerPIDs[i] = new PIDController(steer_Kp, steer_Ki, steer_Kd, -steer_integratorLimit, steer_integratorLimit);
            steerPIDs[i].SetOutputLimits(-maxSteerRateDeg, maxSteerRateDeg);  // PID输出为角速率（度/秒）

            steerCmdDeg[i] = 0f;
            prevWheelSpeedCmd[i] = 0f;
        }

        UpdateWheelPositions();
    }

    void OnValidate()
    {
        for (int i = 0; i < 4; i++)
        {
            if (speedPIDs[i] != null)
            {
                speedPIDs[i].SetGains(speed_Kp, speed_Ki, speed_Kd);
                speedPIDs[i].SetIntegratorLimits(-speed_integratorLimit, speed_integratorLimit);
                speedPIDs[i].SetOutputLimits(speed_outputMin, speed_outputMax);
            }
            if (steerPIDs[i] != null)
            {
                steerPIDs[i].SetGains(steer_Kp, steer_Ki, steer_Kd);
                steerPIDs[i].SetIntegratorLimits(-steer_integratorLimit, steer_integratorLimit);
                steerPIDs[i].SetOutputLimits(-maxSteerRateDeg, maxSteerRateDeg);
            }
        }
        UpdateWheelPositions();
    }

    void UpdateWheelPositions()
    {
        wheelPos[0] = new Vector2( wheelBase / 2f,  trackWidth / 2f);
        wheelPos[1] = new Vector2(-wheelBase / 2f,  trackWidth / 2f);
        wheelPos[2] = new Vector2(-wheelBase / 2f, -trackWidth / 2f);
        wheelPos[3] = new Vector2( wheelBase / 2f, -trackWidth / 2f);
    }

    void FixedUpdate()
    {
        if (enableDebugLog && Time.frameCount % 200 == 1)
            Debug.Log("[MyCar_Motion] alive frame=" + Time.frameCount + " testMode=" + testMode);

        if (testMode)
        {
            RunTestMode();
            return;
        }

        float vz, vx, omega;
        if (controlSource == ControlSource.Agent)
        {
            vz = vz_input * inputScaleVz;
            vx = vx_input * inputScaleVx;
            omega = -omega_input * inputScaleOmega;
        }
        else
        {
            vz = manualVz * inputScaleVz;
            vx = manualVx * inputScaleVx;
            omega = -(manualOmega * Mathf.Deg2Rad) * inputScaleOmega;
        }

        ComputeKinematics(vz, vx, omega);
        MapAndNormalize();
        ApplyPIDControl();
    }

    void RunTestMode()
    {
        for (int i = 0; i < 4; i++)
        {
            if (wheelColliders != null && i < wheelColliders.Length && wheelColliders[i] != null)
            {
                wheelColliders[i].motorTorque = testTorque;
                wheelColliders[i].brakeTorque = 0f;
                wheelColliders[i].steerAngle = 0f;
            }
        }

        if (Time.frameCount % 25 == 0)
        {
            float bodySpeed = rb.linearVelocity.magnitude;
            float bodyFwd = Vector3.Dot(rb.linearVelocity, transform.forward);
            string rpmInfo = "";
            for (int i = 0; i < 4; i++)
            {
                if (wheelColliders != null && i < wheelColliders.Length && wheelColliders[i] != null)
                    rpmInfo += $" w{i}rpm={wheelColliders[i].rpm:F0}";
            }
            Debug.Log($"[TEST] torque={testTorque:F1} bodyFwd={bodyFwd:F3} bodyMag={bodySpeed:F3}{rpmInfo}");
        }
    }

    void LateUpdate()
    {
        UpdateVisualWheels();
    }

    public void SetControl(float vz, float vx, float omega)
    {
        vz_input = vz;  // 前进速度 (Unity Z轴)
        vx_input = vx;  // 横向速度 (Unity X轴)
        omega_input = omega;  // 自转角速度
    }

    /// <summary>
    /// 运动学求解：根据车体速度(vz, vx, omega)计算四个轮子的转向角和轮速
    /// 核心原理：刚体运动学 - 每个轮子的速度 = 车体平移速度 + 旋转产生的切向速度
    /// </summary>
    void ComputeKinematics(float vz, float vx, float omega)
    {
        // 死区处理：所有输入接近零时，停止所有轮子
        if (Mathf.Abs(vz) < deadzone && Mathf.Abs(vx) < deadzone && Mathf.Abs(omega) < deadzone)
        {
            for (int i = 0; i < 4; i++)
            {
                kinSteer[i] = 0f;
                kinSpeed[i] = 0f;
            }
            return;
        }

        // 速度合成 - 刚体运动学公式
        // v_wheel = v_body + omega × r （叉乘）
        for (int i = 0; i < 4; i++)
        {
            // 旋转产生的切向速度分量（2D叉乘简化形式） 
            // 标准公式：v_x = omega * y, v_z = omega * x
            // wheelPos.x = 轮子在纵向(Z轴)的位置, wheelPos.y = 轮子在横向(X轴)的位置
            float vz_rot =  omega * wheelPos[i].y;  // 纵向分量 (前后方向)
            float vx_rot =  omega * wheelPos[i].x;  // 横向分量 (左右方向)

            // 合成总速度：平移速度 + 旋转速度
            float vz_total = vz + vz_rot;
            float vx_total = vx + vx_rot;

            // 第二步：转换为极坐标（速度向量 → 转向角 + 轮速标量）
            kinSteer[i] = Mathf.Atan2(vx_total, vz_total);  // Unity: atan2(X横向, Z纵向)
            kinSpeed[i] = Mathf.Sqrt(vx_total * vx_total + vz_total * vz_total);
            
            // 第三步：角度折叠到±90°范围（舵轮物理限制）
            float angleDeg = kinSteer[i] * Mathf.Rad2Deg;
            if (angleDeg > 90f)
            {
                kinSteer[i] = (angleDeg - 180f) * Mathf.Deg2Rad;
                kinSpeed[i] = -kinSpeed[i];  // 速度反向
            }
            else if (angleDeg < -90f)
            {
                kinSteer[i] = (angleDeg + 180f) * Mathf.Deg2Rad;
                kinSpeed[i] = -kinSpeed[i];  // 速度反向
            }
        }

        // 调试日志：每 30 帧打印一次运动学解析结果
        if (enableDebugLog && Time.frameCount % 30 == 0)
        {
            //Debug.Log($"[ComputeKinematics] Input: vz={vz:F3}, vx={vx:F3}, omega={omega:F3}");
            //Debug.Log($"[kinSpeed] FL={kinSpeed[0]:F3}, RL={kinSpeed[1]:F3}, RR={kinSpeed[2]:F3}, FR={kinSpeed[3]:F3}");
           // Debug.Log($"[kinSteer] FL={kinSteer[0]*Mathf.Rad2Deg:F1}°, RL={kinSteer[1]*Mathf.Rad2Deg:F1}°, RR={kinSteer[2]*Mathf.Rad2Deg:F1}°, FR={kinSteer[3]*Mathf.Rad2Deg:F1}°");
            Debug.Log($"[steerCmdDeg] FL={steerCmdDeg[0]:F1}°, RL={steerCmdDeg[1]:F1}°, RR={steerCmdDeg[2]:F1}°, FR={steerCmdDeg[3]:F1}°");
        }
    }
/// <summary>
    /// 速度归一化和映射：确保轮速不超过物理限制，并映射到实际轮子数组
    /// </summary>
    void MapAndNormalize()
    {
        // 第一步：全局速度限幅 - 找出最大轮速
        float max_speed_abs = 0f;
        for (int i = 0; i < 4; i++) max_speed_abs = Mathf.Max(max_speed_abs, Mathf.Abs(kinSpeed[i]));
        
        // 如果最大轮速超过限制，按比例缩放所有轮子（保持相对速度比例）
        if (max_speed_abs > maxWheelLinearSpeed && max_speed_abs > 0f)
        {
            float scale = maxWheelLinearSpeed / max_speed_abs;
            for (int i = 0; i < 4; i++) kinSpeed[i] *= scale;
        }

        // 第二步：直接映射到 wheelColliders 数组（索引顺序一致）
        // kinIdx 顺序：FL(0), RL(1), RR(2), FR(3)
        // wheelColliders 顺序：FL(0), RL(1), RR(2), FR(3)
        for (int i = 0; i < 4; i++)
        {
            appliedSteerDeg[i] = kinSteer[i] * Mathf.Rad2Deg;  // 弧度→角度
            appliedSpeed[i] = kinSpeed[i];                      // 速度直接赋值
        }

        // 调试打印：映射后的结果
        if (enableDebugLog && Time.frameCount % 30 == 0)
        {
           // Debug.Log($"[appliedSpeed] FL={appliedSpeed[0]:F3}, RL={appliedSpeed[1]:F3}, RR={appliedSpeed[2]:F3}, FR={appliedSpeed[3]:F3}");
            //Debug.Log($"[appliedSteerDeg] FL={appliedSteerDeg[0]:F1}°, RL={appliedSteerDeg[1]:F1}°, RR={appliedSteerDeg[2]:F1}°, FR={appliedSteerDeg[3]:F1}°");
        }
    }

    /// <summary>
    /// PID控制器应用：将运动学解算结果转换为实际控制指令
    /// 含舵轮最短路径优化、逐轮方向反转检测、转向角PID跟随、轮速PID控制
    /// </summary>
    void ApplyPIDControl()
    {
        float dt = Time.fixedDeltaTime;

        // 车体速度（仅用于日志显示）
        Vector3 bodyVelWorld = rb.linearVelocity;
        Vector3 bodyVelLocal = transform.InverseTransformDirection(bodyVelWorld);

        for (int j = 0; j < 4; j++)
        {
            WheelCollider wc = (wheelColliders != null && j < wheelColliders.Length) ? wheelColliders[j] : null;
            float wheelRadius = (wc != null) ? Mathf.Max(1e-4f, wc.radius) : 0.05f;

            float desiredSteerDeg = appliedSteerDeg[j];
            float desired_v = appliedSpeed[j];

            // ComputeKinematics 已将角度折叠到 ±90°，这里只做安全限位
            desiredSteerDeg = Mathf.Clamp(desiredSteerDeg, -90f, 90f);

            // ========== 逐轮方向反转检测 ==========
            float prevSign = Mathf.Sign(prevWheelSpeedCmd[j]);
            float currSign = Mathf.Sign(desired_v);
            if (prevSign != 0f && currSign != 0f && prevSign != currSign)
            {
                speedPIDs[j].Reset();
                if (enableDebugLog)
                    Debug.Log($"[Wheel {j}] Direction reversed, speed PID reset.");
            }

            // ========== 转向角PID跟随控制 ==========
            float steerErrorDeg = Mathf.DeltaAngle(steerCmdDeg[j], desiredSteerDeg);
            float steerRateCmdDeg = 0f;

            if (Mathf.Abs(steerErrorDeg) < steerDeadbandDeg)
            {
                steerPIDs[j].Reset();
            }
            else
            {
                steerRateCmdDeg = steerPIDs[j].Update(steerErrorDeg, steerCmdDeg[j], dt);
            }

            float prevSteerError = steerErrorDeg;
            steerCmdDeg[j] += steerRateCmdDeg * dt;
            float newSteerError = Mathf.DeltaAngle(steerCmdDeg[j], desiredSteerDeg);
            if (Mathf.Sign(newSteerError) != Mathf.Sign(prevSteerError)
                && Mathf.Abs(prevSteerError) > steerDeadbandDeg)
            {
                steerCmdDeg[j] = desiredSteerDeg;
            }

            // ========== 轮速控制（前馈 + PID 微调，基于车体速度反馈）==========
            // 用 Rigidbody.GetPointVelocity 获取该轮接触点的真实速度（含平动+旋转）
            // 再投影到轮子前进方向，得到精确的 groundSpeed
            float groundSpeed = 0f;
            if (wc != null)
            {
                Vector3 wheelWorldPos = wc.transform.TransformPoint(wc.center);
                Vector3 pointVel = rb.GetPointVelocity(wheelWorldPos);
                Vector3 pointVelLocal = transform.InverseTransformDirection(pointVel);
                float steerRad = steerCmdDeg[j] * Mathf.Deg2Rad;
                groundSpeed = pointVelLocal.z * Mathf.Cos(steerRad) + pointVelLocal.x * Mathf.Sin(steerRad);
            }

            float speedError = desired_v - groundSpeed;

            if (Mathf.Abs(desired_v) < speedDeadband)
            {
                speedPIDs[j].Reset();
                if (wc != null)
                {
                    wc.motorTorque = 0f;
                    wc.brakeTorque = Mathf.Clamp(brakeGain * Mathf.Abs(groundSpeed), 0f, brakeTorqueHigh);
                }
            } 
            else
            { 
                // 前馈：直接从标定的物理增益反推所需扭矩
                float feedforward = desired_v / Mathf.Max(0.01f, torqueToSpeedGain);
                // PID：仅修正误差（输出单位也是 Nm）
                float pidOut = speedPIDs[j].Update(speedError, groundSpeed, dt);
                float torqueCmd = Mathf.Clamp(feedforward + pidOut, -maxMotorTorque, maxMotorTorque);

                if (wc != null)
                {
                    wc.motorTorque = torqueCmd;
                    // 超速时辅助刹车：速度超过目标时施加制动，与电机反扭矩叠加
                    float overspeed = Mathf.Abs(groundSpeed) - Mathf.Abs(desired_v);
                    if (overspeed > speedDeadband && Mathf.Sign(groundSpeed) == Mathf.Sign(desired_v))
                        wc.brakeTorque = Mathf.Clamp(brakeGain * overspeed, 0f, brakeTorqueHigh);
                    else
                        wc.brakeTorque = 0f;
                }
            }

            // 物理限位：实际舵轮角度限制在 ±90°
            float beforeClamp = steerCmdDeg[j];
            steerCmdDeg[j] = Mathf.Clamp(steerCmdDeg[j], -90f, 90f);
            if (beforeClamp != steerCmdDeg[j])
                steerPIDs[j].Reset();

            if (wc != null) wc.steerAngle = steerCmdDeg[j];

            steerAngles[j] = steerCmdDeg[j] * Mathf.Deg2Rad;
            wheelSpeeds[j] = groundSpeed;
            prevWheelSpeedCmd[j] = desired_v;
        }

        if (enableDebugLog && Time.frameCount % 30 == 0)
        {
            float bodyFwd = bodyVelLocal.z;
            float bodyLat = bodyVelLocal.x;
            float yawDeg = rb.angularVelocity.y * Mathf.Rad2Deg;
            Debug.Log($"[Motion] bodyFwd={bodyFwd:F3} bodyLat={bodyLat:F3} yawRate={yawDeg:F1}°/s | groundV: FL={wheelSpeeds[0]:F3} RL={wheelSpeeds[1]:F3} RR={wheelSpeeds[2]:F3} FR={wheelSpeeds[3]:F3}");
            Debug.Log($"[Motion] desired: FL={appliedSpeed[0]:F3} RL={appliedSpeed[1]:F3} RR={appliedSpeed[2]:F3} FR={appliedSpeed[3]:F3} | steer: FL={steerCmdDeg[0]:F1}° RL={steerCmdDeg[1]:F1}° RR={steerCmdDeg[2]:F1}° FR={steerCmdDeg[3]:F1}°");
            if (wheelColliders != null && wheelColliders.Length >= 4)
            {
                var w0 = wheelColliders[0]; var w1 = wheelColliders[1];
                var w2 = wheelColliders[2]; var w3 = wheelColliders[3];
                Debug.Log($"[Motion] motor: FL={w0.motorTorque:F2} RL={w1.motorTorque:F2} RR={w2.motorTorque:F2} FR={w3.motorTorque:F2} | brake: FL={w0.brakeTorque:F2} RL={w1.brakeTorque:F2}");
            }
        }
    }

    void UpdateVisualWheels()
    {
        if (wheelColliders == null || wheelMeshes == null) return;
        for (int i = 0; i < wheelColliders.Length && i < wheelMeshes.Length; i++)
        {
            var wc = wheelColliders[i];
            var mesh = wheelMeshes[i];
            if (wc == null || mesh == null) continue;
            
            // 获取位置（物理位置是准确的）
            Vector3 pos; Quaternion rot;
            wc.GetWorldPose(out pos, out rot);
            mesh.position = pos;

            if (forceWheelZto90)
            {
                // 椭圆轮专用：手动构建旋转，只使用转向角，忽略物理滚动
                // 1. 获取车体的世界旋转
                Quaternion vehicleRotation = transform.rotation;
                
                // 2. 获取轮子的转向角（相对于车体）
                float steerAngleDeg = wc.steerAngle;
                
                // 3. 构建旋转：车体旋转 + 转向（绕Y轴）+ 固定滚动90°（绕局部Z轴）
                mesh.rotation = vehicleRotation * Quaternion.Euler(0, steerAngleDeg, 90f);
            }
            else
            {
                // 使用 WheelCollider 的完整物理旋转
                mesh.rotation = rot;
            }
        }
    }

    class PIDController
    {
        public float Kp, Ki, Kd;
        private float integrator;
        private float lastError;
        private float lastMeasurement;
        private bool hasPrevMeasurement;
        private float integMin = -Mathf.Infinity, integMax = Mathf.Infinity;
        private float outMin = -Mathf.Infinity, outMax = Mathf.Infinity;

        public PIDController(float p, float i, float d, float integMin_, float integMax_)
        {
            Kp = p; Ki = i; Kd = d;
            Reset();
            integMin = integMin_; integMax = integMax_;
        }

        public void SetGains(float p, float i, float d) { Kp = p; Ki = i; Kd = d; }
        public void SetIntegratorLimits(float lo, float hi) { integMin = lo; integMax = hi; }
        public void SetOutputLimits(float lo, float hi) { outMin = lo; outMax = hi; }

        public void Reset()
        {
            integrator = 0f;
            lastError = 0f;
            lastMeasurement = 0f;
            hasPrevMeasurement = false;
        }

        public float Update(float error, float dt)
        {
            if (dt <= 0f) return 0f;
            integrator = Mathf.Clamp(integrator + error * dt, integMin, integMax);
            float deriv = (error - lastError) / dt;
            lastError = error;
            return Mathf.Clamp(Kp * error + Ki * integrator + Kd * deriv, outMin, outMax);
        }

        /// <summary> Derivative-on-measurement variant: avoids setpoint derivative kick </summary>
        public float Update(float error, float measurement, float dt)
        {
            if (dt <= 0f) return 0f;
            integrator = Mathf.Clamp(integrator + error * dt, integMin, integMax);
            float deriv = hasPrevMeasurement ? -(measurement - lastMeasurement) / dt : 0f;
            lastMeasurement = measurement;
            hasPrevMeasurement = true;
            lastError = error;
            return Mathf.Clamp(Kp * error + Ki * integrator + Kd * deriv, outMin, outMax);
        }
    }
}