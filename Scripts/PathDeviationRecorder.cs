using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// 独立记录前、后中心磁传感器相对磁条的横向偏差、车体航向误差，以及车体实际前进速度。
/// 不依赖 MyCarAgent 或模糊控制器，PPO 与规则库控制时都可以挂在车上使用。
/// </summary>
public class PathDeviationRecorder : MonoBehaviour
{
    [Header("开关")]
    [Tooltip("勾选后按固定物理步记录。取消勾选即停止，已写入的文件会保留。")]
    public bool enableRecording = false;

    [Header("引用")]
    public MagneticTape tape;
    [Tooltip("前中磁传感器")]
    public Transform frontCenterSensor;
    [Tooltip("后中磁传感器")]
    public Transform rearCenterSensor;
    [Tooltip("用于读取车体速度。留空则使用本物体或其父物体上的 Rigidbody。前进速度为刚体速度在车头方向上的投影，单位 m/s。")]
    public Rigidbody vehicleBody;

    [Header("保存")]
    [Tooltip("保存目录。留空则使用 Application.persistentDataPath")]
    public string saveDirectory = "";
    [Tooltip("文件名前缀，便于区分场景和控制器，例如 fuzzy_45deg")]
    public string filePrefix = "path_deviation";
    [Tooltip("采样间隔（秒）。0 表示每个 FixedUpdate 记一行")]
    public float sampleInterval = 0f;

    float nextSampleTime;
    string filePath;
    bool headerWritten;
    bool warned;
    bool warnedMissingBody;
    int sampleCount;
    StreamWriter writer;

    void Awake()
    {
        if (vehicleBody == null)
            vehicleBody = GetComponent<Rigidbody>();
        if (vehicleBody == null)
            vehicleBody = GetComponentInParent<Rigidbody>();
    }

    void OnDisable()
    {
        CloseWriter();
    }

    void OnApplicationQuit()
    {
        CloseWriter();
    }

    void FixedUpdate()
    {
        if (!enableRecording)
        {
            CloseWriter();
            return;
        }

        if (sampleInterval > 0f && Time.time < nextSampleTime)
            return;
        nextSampleTime = Time.time + Mathf.Max(0f, sampleInterval);

        if (!TrySample(out float frontLat, out float rearLat, out float headingDeg))
        {
            if (!warned)
            {
                warned = true;
                Debug.LogWarning("[PathDeviation] 未能计算偏差。请指定 MagneticTape、前中传感器和后中传感器。");
            }
            return;
        }

        EnsureWriter();
        if (writer == null)
            return;

        var culture = CultureInfo.InvariantCulture;
        var line = new StringBuilder();
        line.Append(Time.time.ToString("F4", culture)).Append(',');
        line.Append(frontLat.ToString("F6", culture)).Append(',');
        line.Append(rearLat.ToString("F6", culture)).Append(',');
        line.Append(headingDeg.ToString("F4", culture)).Append(',');
        line.Append(SampleForwardSpeed().ToString("F4", culture));
        writer.WriteLine(line.ToString());
        sampleCount++;
        if (sampleCount % 200 == 0)
            writer.Flush();
    }

    bool TrySample(out float frontLat, out float rearLat, out float headingDeg)
    {
        frontLat = 0f;
        rearLat = 0f;
        headingDeg = 0f;
        if (tape == null || frontCenterSensor == null || rearCenterSensor == null)
            return false;

        Vector3 fwd = transform.forward;
        fwd.y = 0f;
        if (fwd.sqrMagnitude < 1e-8f)
            return false;
        fwd.Normalize();

        Vector3 frontPos = frontCenterSensor.position;
        Vector3 rearPos = rearCenterSensor.position;
        if (!tape.TryGetHorizontalDeviation(frontPos, out _, out Vector3 frontTangent, out frontLat))
            return false;
        if (!tape.TryGetHorizontalDeviation(rearPos, out _, out Vector3 rearTangent, out rearLat))
            return false;

        // 折线顶点顺序可能和车的行驶方向相反。反向时切线与车头夹角接近 180°，
        // 横向“右侧”也会跟着反号。统一翻到与车头同向后再计算。
        AlignTangentToTravel(fwd, ref frontTangent, ref frontLat);
        AlignTangentToTravel(fwd, ref rearTangent, ref rearLat);

        Vector3 refPos = (frontPos + rearPos) * 0.5f;
        float unused = 0f;
        if (!tape.TryGetHorizontalDeviation(refPos, out _, out Vector3 tangent, out unused))
            return false;
        AlignTangentToTravel(fwd, ref tangent, ref unused);
        if (tangent.sqrMagnitude < 1e-8f)
            return false;

        headingDeg = Vector3.SignedAngle(tangent.normalized, fwd, Vector3.up);
        return true;
    }

    float SampleForwardSpeed()
    {
        if (vehicleBody == null)
        {
            if (!warnedMissingBody)
            {
                warnedMissingBody = true;
                Debug.LogWarning("[PathDeviation] 未找到 Rigidbody，前进速度记为 0。");
            }
            return 0f;
        }

        return Vector3.Dot(vehicleBody.linearVelocity, transform.forward);
    }

    static void AlignTangentToTravel(Vector3 travelForward, ref Vector3 tangent, ref float signedLateral)
    {
        tangent.y = 0f;
        if (tangent.sqrMagnitude < 1e-8f)
            return;
        if (Vector3.Dot(tangent, travelForward) < 0f)
        {
            tangent = -tangent;
            signedLateral = -signedLateral;
        }
    }

    void EnsureWriter()
    {
        if (writer != null)
            return;

        string directory = string.IsNullOrEmpty(saveDirectory)
            ? Application.persistentDataPath
            : saveDirectory;
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        string prefix = string.IsNullOrEmpty(filePrefix) ? "path_deviation" : filePrefix;
        string fileName = $"{prefix}_{System.DateTime.Now:yyyyMMdd_HHmmss}.csv";
        filePath = Path.Combine(directory, fileName);
        writer = new StreamWriter(filePath, false, new UTF8Encoding(false));
        writer.WriteLine("time_s,front_lat_m,rear_lat_m,heading_deg,forward_speed_mps");
        headerWritten = true;
        sampleCount = 0;
        Debug.Log($"[PathDeviation] 开始记录: {filePath}");
    }

    void CloseWriter()
    {
        if (writer == null)
            return;
        writer.Flush();
        writer.Dispose();
        writer = null;
        if (headerWritten)
            Debug.Log($"[PathDeviation] 已保存 {sampleCount} 行: {filePath}");
        headerWritten = false;
    }
}
