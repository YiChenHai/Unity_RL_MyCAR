using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 蒸馏助手：快速导出数据和对比NN vs 规则库性能
/// 使用方法：在MyCar Agent对象上添加此脚本，或通过Console调用静态方法
/// </summary>
public class DistillationHelper : MonoBehaviour
{
    private static MyCarAgent targetAgent = null;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        Debug.Log("[DistillationHelper] 初始化完成。使用方法:");
        Debug.Log("  DistillationHelper.ExportData()  - 导出收集的数据");
        Debug.Log("  DistillationHelper.SetAgent(agent) - 设置目标Agent");
    }

    /// <summary>
    /// 通过Console调用：DistillationHelper.ExportData()
    /// </summary>
    public static void ExportData()
    {
        MyCarAgent agent = FindObjectOfType<MyCarAgent>();
        if (agent == null)
        {
            Debug.LogError("[DistillationHelper] 场景中找不到MyCarAgent!");
            return;
        }

        agent.ExportCollectedData();
        Debug.Log("[DistillationHelper] ✓ 数据导出完成");
    }

    /// <summary>
    /// 设置目标Agent（如果有多个）
    /// </summary>
    public static void SetAgent(MyCarAgent agent)
    {
        targetAgent = agent;
        Debug.Log($"[DistillationHelper] 已设置目标Agent: {agent.name}");
    }

    /// <summary>
    /// 获取当前数据收集进度
    /// </summary>
    public static void GetCollectionProgress()
    {
        MyCarAgent agent = targetAgent ?? FindObjectOfType<MyCarAgent>();
        if (agent == null)
        {
            Debug.LogError("[DistillationHelper] 找不到Agent");
            return;
        }

        // 这里需要在MyCarAgent中添加公开属性来获取进度
        // 临时方案：通过enableDataCollection状态推断
        if (agent.enableDataCollection)
        {
            Debug.Log("[DistillationHelper] 数据收集中... 请查看Agent的Inspector获取进度");
        }
        else
        {
            Debug.Log("[DistillationHelper] 数据收集已停止");
        }
    }

    /// <summary>
    /// 快速启用/禁用数据收集
    /// </summary>
    public static void ToggleCollection(bool enable)
    {
        MyCarAgent agent = targetAgent ?? FindObjectOfType<MyCarAgent>();
        if (agent == null)
        {
            Debug.LogError("[DistillationHelper] 找不到Agent");
            return;
        }

        agent.enableDataCollection = enable;
        Debug.Log($"[DistillationHelper] 数据收集已{(enable ? "启用" : "禁用")}");
    }

    // ========== 以下为Editor快捷菜单 ==========
    // 在Unity编辑器中，Editor菜单会自动显示这些方法

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/ML-Agents/导出训练数据")]
    public static void MenuExportData() => ExportData();

    [UnityEditor.MenuItem("Tools/ML-Agents/启用数据收集")]
    public static void MenuEnableCollection() => ToggleCollection(true);

    [UnityEditor.MenuItem("Tools/ML-Agents/禁用数据收集")]
    public static void MenuDisableCollection() => ToggleCollection(false);

    [UnityEditor.MenuItem("Tools/ML-Agents/查看收集进度")]
    public static void MenuGetProgress() => GetCollectionProgress();
#endif
}
