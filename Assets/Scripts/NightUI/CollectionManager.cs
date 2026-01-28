using UnityEngine;
using System.Collections.Generic;
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


    // 定义广播事件
    public static System.Action<NightSummaryReport> OnNightEndSummary;

    // 获取当前所有数据的快照
    public NightSummaryReport GetCurrentReport(bool success) {
        return new NightSummaryReport {
            // 创建副本，防止场景销毁后引用丢失
            finalEmotions = new Dictionary<EmotionTraitID, int>(emotionCounts),
            finalMemories = collectedMemories.ToList(),
            isSuccess = success
        };
    }
}