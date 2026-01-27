using UnityEngine;
using System.Collections.Generic;

public class CollectionManager : MonoBehaviour
{
    public static CollectionManager Instance;

    [Header("情绪统计")]
    // 键是情绪种类，值是收集到的数量
    private Dictionary<EmotionTraitID, int> emotionCounts = new Dictionary<EmotionTraitID, int>();

    [Header("记忆集合")]
    // 记录所有已收集到的记忆类型
    private HashSet<MemoryTraitID> collectedMemories = new HashSet<MemoryTraitID>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // 初始化情绪字典，确保每个枚举值都有初始值 0
        foreach (EmotionTraitID trait in System.Enum.GetValues(typeof(EmotionTraitID)))
        {
            emotionCounts[trait] = 0;
        }
    }

    private void OnEnable()
    {
        // 订阅事件
        LootEvents.OnEmotionPicked += RecordEmotion;
        LootEvents.OnMemoryPicked += RecordMemory;
    }

    private void OnDisable()
    {
        // 取消订阅
        LootEvents.OnEmotionPicked -= RecordEmotion;
        LootEvents.OnMemoryPicked -= RecordMemory;
    }

    // --- 监听逻辑 ---

    private void RecordEmotion(EmotionTraitID trait)
    {
        emotionCounts[trait]++;
        Debug.Log($"<color=yellow>[收集更新]</color> 情绪 {trait} 数量变为: {emotionCounts[trait]}");
        
        // 此处可以触发 UI 更新逻辑
    }

    private void RecordMemory(MemoryTraitID trait)
    {
        if (!collectedMemories.Contains(trait))
        {
            collectedMemories.Add(trait);
            Debug.Log($"<color=cyan>[收集更新]</color> 新记忆解锁: {trait}");
        }
        else
        {
            Debug.Log($"[收集提示] 记忆 {trait} 已经存在于集合中");
        }
    }

    // --- 查询接口 (供 UI 或其它系统调用) ---

    /// <summary>
    /// 获取特定情绪的收集数量
    /// </summary>
    public int GetEmotionCount(EmotionTraitID trait)
    {
        return emotionCounts.ContainsKey(trait) ? emotionCounts[trait] : 0;
    }

    /// <summary>
    /// 检查是否拥有特定类型的记忆
    /// </summary>
    public bool HasMemory(MemoryTraitID trait)
    {
        return collectedMemories.Contains(trait);
    }
}