using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 改进后的磁条模拟器 Magnetic Tape Simulator
/// 支持直线段、直角拐弯，磁场平滑且随 xyz 距离衰减
/// 超过有效范围（球形区域）则忽略
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class MagneticTape : MonoBehaviour
{
    private LineRenderer line;
    private Vector3[] points;

    [Header("磁条参数")]
    public float B0 = 1.0f;        // 磁条强度标量
    public float z0 = 0.002f;      // 防止离磁条过近无限大
    public float alpha = 1.5f;     // 垂直衰减指数
    public float sigma = 0.05f;    // 水平衰减控制磁条宽度
    public float scale = 1f;       // 总缩放因子
    public float effectiveRange = 0.2f; // 20cm 有效影响范围（球形）

    [Header("积分参数")]
    public int integrationStepsPerSegment = 100; // 每段线段积分步数（越大越精细）

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        points = new Vector3[line.positionCount];
        line.GetPositions(points);
    }

    /// <summary>
    /// 计算传感器位置的磁场矢量
    /// </summary>
    public Vector3 GetMagneticField(Vector3 sensorPosition)
    {
        if (points == null || points.Length < 2) return Vector3.zero;

        Vector3 totalMagneticField = Vector3.zero; // 存储总的磁场矢量

        // 遍历每条线段
        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector3 start = points[i];
            Vector3 end = points[i + 1];
            float segmentLength = Vector3.Distance(start, end);
            if (segmentLength <= 0f)
            {
                continue;
            }

            // 对每条线段做离散积分
            for (int s = 0; s < integrationStepsPerSegment; s++)
            {
                // 中点积分法
                float t = (s + 0.5f) / integrationStepsPerSegment;
                Vector3 samplePos = Vector3.Lerp(start, end, t);

                // 计算传感器到积分点的三维向量
                Vector3 delta = sensorPosition - samplePos;
                float distance = delta.magnitude;

                // 超出球形有效范围直接忽略
                if (distance > effectiveRange)
                    continue;

                // 垂直距离
                float dz = Mathf.Abs(delta.y);
                // 水平距离
                Vector3 horizontal = new Vector3(delta.x, 0f, delta.z);
                float r_h = horizontal.magnitude;

                // 磁场公式：垂直幂律衰减 * 水平高斯衰减
                float B = B0 / Mathf.Pow(dz + z0, alpha) * Mathf.Exp(-r_h * r_h / (2f * sigma * sigma));

                // 计算磁场矢量（方向与距离有关）
                Vector3 direction = delta.normalized; // 磁场的方向是从磁条采样点指向传感器 单位向量
                Vector3 magneticFieldVector = direction * B;

                // 按线段长度归一化，避免点密度不一致导致强度变化
                totalMagneticField += magneticFieldVector * (segmentLength / integrationStepsPerSegment);
            }
        }

        // 返回总磁场矢量（乘缩放系数）
        return totalMagneticField * scale;
    }

    /// <summary>
    /// 水平面上，点到磁条折线的最近点、该段切线，以及带符号横向偏差。
    /// 横向偏差：沿磁条前进方向看，点在右侧为正，单位 m。
    /// </summary>
    public bool TryGetHorizontalDeviation(Vector3 worldPos, out Vector3 closestWorld, out Vector3 tangentWorld, out float signedLateral)
    {
        closestWorld = worldPos;
        tangentWorld = Vector3.forward;
        signedLateral = 0f;

        EnsurePoints();
        if (points == null || points.Length < 2)
            return false;

        Vector3 p = worldPos;
        p.y = 0f;
        float bestDistSq = float.PositiveInfinity;
        bool found = false;

        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector3 a = ToWorld(points[i]);
            Vector3 b = ToWorld(points[i + 1]);
            a.y = 0f;
            b.y = 0f;
            Vector3 ab = b - a;
            float lenSq = ab.sqrMagnitude;
            if (lenSq < 1e-10f)
                continue;

            float t = Mathf.Clamp01(Vector3.Dot(p - a, ab) / lenSq);
            Vector3 c = a + ab * t;
            float distSq = (p - c).sqrMagnitude;
            if (distSq < bestDistSq)
            {
                bestDistSq = distSq;
                closestWorld = c;
                tangentWorld = ab.normalized;
                found = true;
            }
        }

        if (!found)
            return false;

        // 切线右侧单位向量：T=(tx,0,tz) 时右向为 (tz, 0, -tx)
        Vector3 right = new Vector3(tangentWorld.z, 0f, -tangentWorld.x);
        signedLateral = Vector3.Dot(p - closestWorld, right);
        return true;
    }

    void EnsurePoints()
    {
        if (line == null)
            line = GetComponent<LineRenderer>();
        if (line == null)
        {
            points = null;
            return;
        }
        if (points == null || points.Length != line.positionCount)
            points = new Vector3[line.positionCount];
        line.GetPositions(points);
    }

    Vector3 ToWorld(Vector3 p)
    {
        if (line != null && line.useWorldSpace)
            return p;
        return transform.TransformPoint(p);
    }

    /// <summary>
    /// 获取磁场标量（用于可视化箭头）
    /// </summary>
    public float GetMagneticFieldScalar(Vector3 sensorPosition)
    {
        Vector3 magneticFieldVector = GetMagneticField(sensorPosition);
        return magneticFieldVector.magnitude;  // 返回磁场的标量强度
    }

    /// <summary>
    /// 在编辑器中显示磁条的每个采样点
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (line == null) line = GetComponent<LineRenderer>();

        if (points == null || points.Length == 0)
        {
            points = new Vector3[line.positionCount];
            line.GetPositions(points);
        }

        Gizmos.color = new Color(0, 1, 0, 0.3f);
        foreach (var p in points)
            Gizmos.DrawWireSphere(p, 0.05f);
    }
}
