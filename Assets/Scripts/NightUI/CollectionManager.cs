using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;

// 广播每晚收获用的数据结构
public struct NightSummaryReport {
    public Dictionary<EmotionTraitID, int> finalEmotions;
    public List<MemoryTraitID> finalMemories;
    public bool isSuccess; // 是撤离还是死亡
}

public class CollectionManager : MonoBehaviour
{
    public static CollectionManager Instance;

    // --- 新增：专门通知 UI 刷新的事件 ---
    public static System.Action OnCollectionUpdated;

    [Header("情绪统计")]
    private Dictionary<EmotionTraitID, int> emotionCounts = new Dictionary<EmotionTraitID, int>();

    [Header("记忆集合")]
    private HashSet<MemoryTraitID> collectedMemories = new HashSet<MemoryTraitID>();

    // 定义广播事件
    public static System.Action<NightSummaryReport> OnNightEndSummary;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        foreach (EmotionTraitID trait in System.Enum.GetValues(typeof(EmotionTraitID)))
        {
            emotionCounts[trait] = 0;
        }
    }

    private void OnEnable()
    {
        LootEvents.OnEmotionPicked += RecordEmotion;
        LootEvents.OnMemoryPicked += RecordMemory;
    }

    private void OnDisable()
    {
        LootEvents.OnEmotionPicked -= RecordEmotion;
        LootEvents.OnMemoryPicked -= RecordMemory;
    }

    private void RecordEmotion(EmotionTraitID trait)
    {
        emotionCounts[trait]++;
        Debug.Log($"<color=yellow>[数据更新]</color> {trait} 增加到: {emotionCounts[trait]}");
        
        // 【关键修复】：确保数据变了之后再通知 UI
        OnCollectionUpdated?.Invoke();
    }

    private void RecordMemory(MemoryTraitID trait)
    {
        if (!collectedMemories.Contains(trait))
        {
            collectedMemories.Add(trait);
            Debug.Log($"<color=cyan>[数据更新]</color> 解锁新记忆: {trait}");
            
            // 【关键修复】：通知 UI 刷新记忆列表
            OnCollectionUpdated?.Invoke();
        }
    }

    public int GetEmotionCount(EmotionTraitID trait)
    {
        return emotionCounts.ContainsKey(trait) ? emotionCounts[trait] : 0;
    }

    public bool HasMemory(MemoryTraitID trait)
    {
        return collectedMemories.Contains(trait);
    }

    public void HandleDeathLoss()
    {
        // 1. 清空记忆
        collectedMemories.Clear();

        // 2. 损失一半情绪
        int totalEmotions = 0;
        foreach (var count in emotionCounts.Values) totalEmotions += count;
        int toRemove = totalEmotions / 2;

        for (int i = 0; i < toRemove; i++)
        {
            var availableKeys = emotionCounts
                .Where(kvp => kvp.Value > 0)
                .Select(kvp => kvp.Key)
                .ToList();
            
            if (availableKeys.Count > 0)
            {
                EmotionTraitID target = availableKeys[Random.Range(0, availableKeys.Count)];
                emotionCounts[target]--;
            }
        }

        // 【新增】：死亡惩罚执行完后，也要通知 UI 变动（变成剩下的一半）
        OnCollectionUpdated?.Invoke();
    }

    // 获取当前所有数据的快照
    public NightSummaryReport GetCurrentReport(bool success) {
        return new NightSummaryReport {
            // 创建副本，防止场景销毁后引用丢失
            finalEmotions = new Dictionary<EmotionTraitID, int>(emotionCounts),
            finalMemories = collectedMemories.ToList(),
            isSuccess = success
        };
    }

    // --- 【关键新增】：执行最终结算并转化到永久库存 ---
    /// <summary>
    /// 处理夜晚结束的最终结算：广播 -> 写入库存 -> 清空临时背包
    /// </summary>
    public void FinalizeNightCollection(bool isSuccess)
    {
        // 1. 执行广播 (告知其他系统如 Achievement 或 UI 最终收获情况)
        NightSummaryReport report = GetCurrentReport(isSuccess);
        OnNightEndSummary?.Invoke(report);

        // 2. 计入库存 (通过 BagManager 持久化)
        if (BagManager.Instance != null)
        {
            // 存入情绪及其对应数量
            foreach (var kvp in emotionCounts)
            {
                if (kvp.Value > 0)
                {
                    BagManager.Instance.EarnEmotion(kvp.Key, kvp.Value);
                }
            }

            // 存入记忆
            foreach (var memory in collectedMemories)
            {
                BagManager.Instance.EarnMemory(memory);
            }
        }
        else
        {
            Debug.LogWarning("CollectionManager: 找不到 BagManager 实例，收获无法存入库存！");
        }

        // 3. 将临时背包清零 (数据重置)
        ClearTemporaryCollection();
    }

    /// <summary>
    /// 清空当晚收集的临时数据
    /// </summary>
    private void ClearTemporaryCollection()
    {
        // 清空情绪字典 (将所有 Value 设为 0)
        List<EmotionTraitID> keys = emotionCounts.Keys.ToList();
        foreach (var key in keys)
        {
            emotionCounts[key] = 0;
        }

        // 清空记忆集合
        collectedMemories.Clear();

        // 再次通知 UI (此时显示应全为 0)
        OnCollectionUpdated?.Invoke();
        Debug.Log("<color=green>[系统]</color> 夜晚临时背包已清空。");
    }
}