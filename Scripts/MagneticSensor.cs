using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ģ��Ŵ����� Magnetic Sensor
/// </summary>
public class MagneticSensor : MonoBehaviour
{
    [Header("磁传感器配置")]
    public MagneticTape magneticTape;  // 绑定磁带对象

    // 当前测到的磁场强度（仅用于显示）
    public float fieldStrength;

    // 当前测到的磁场方向（仅用于显示）
    public Vector3 fieldDirection;

    // 是否在场景视图中绘制磁场方向箭头
    public bool showDirection = false;

    void Update()
    {
        if (magneticTape != null)
        {
            // ���ô����ű��ķ�������ȡ�ų�ʸ��
            Vector3 magneticField = magneticTape.GetMagneticField(transform.position);
            fieldStrength = magneticField.magnitude;  // ��ȡ�ų�ǿ�ȣ�������
            fieldDirection = magneticField.normalized;  // ��ȡ�ų����򣨵�λ������
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.02f);

        // ��ѡ�����ӻ��ų�����
        if (showDirection && magneticTape != null)
        {
            // ʹ�� GetMagneticField ����ȡ�ų�ʸ��
            Vector3 magneticField = magneticTape.GetMagneticField(transform.position);
            float visualScale = 10f; // �ɵ����Կ�����ͷ
            if (magneticField.magnitude > 1e-8f)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, transform.position + magneticField.normalized * visualScale * magneticField.magnitude);
            }
        }
    }
}
