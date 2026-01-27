using System.Collections.Generic;
using UnityEngine;

// --- 文件名: MorningGameDataDefinitions.cs ---

#region 1. 枚举定义 (Enums)
// ==========================================
// 定义游戏中所有固定的类型选项
// ==========================================

public enum EmotionType
{
    Happy,   // 开心
    Angry,   // 愤怒
    Excited, // 兴奋
    Sad      // 悲伤
}

#endregion

#region 2. 情绪存储容器 (EmotionStorage)
// ==========================================
// 专门用于处理4种情绪数量的类
// ==========================================

[System.Serializable]
public class EmotionStorage
{
    public int happyCount;
    public int angryCount;
    public int excitedCount;
    public int sadCount;

    public EmotionStorage()
    {
        happyCount = 0;
        angryCount = 0;
        excitedCount = 0;
        sadCount = 0;
    }

    public void AddEmotion(EmotionType type, int amount)
    {
        switch (type)
        {
            case EmotionType.Happy: happyCount += amount; break;
            case EmotionType.Angry: angryCount += amount; break;
            case EmotionType.Excited: excitedCount += amount; break;
            case EmotionType.Sad: sadCount += amount; break;
        }
    }

    public int GetEmotion(EmotionType type)
    {
        switch (type)
        {
            case EmotionType.Happy: return happyCount;
            case EmotionType.Angry: return angryCount;
            case EmotionType.Excited: return excitedCount;
            case EmotionType.Sad: return sadCount;
            default: return 0;
        }
    }
}
#endregion

#region 3. 经营背包数据 (Inventory)
// ==========================================
// 包含颜料、情绪、记忆的数据结构
// ==========================================

[System.Serializable]
public class MorningInventoryData
{
    // --- 资源 A: 颜料 ---
    public int pigmentAmount;

    // --- 资源 B: 情绪 (引用上面的类) ---
    public EmotionStorage emotions;

    // --- 资源 C: 记忆 (只存已解锁的ID) ---
    public List<string> unlockedMemoryIDs;

    public MorningInventoryData()
    {
        pigmentAmount = 0;
        emotions = new EmotionStorage();
        unlockedMemoryIDs = new List<string>();
    }

    //TODO:
}
#endregion

#region 4. 订单数据类 (OrderData) - 精简版
[System.Serializable]
public class OrderData
{
    public string orderID;      // 对应 OrderTemplate 的唯一 ID
    public int daysRemaining;   // 状态标识：>0 剩余天数, -1 已完成好评, -2 已过期。-3已完成差评

    public OrderData(string id, int days)
    {
        this.orderID = id;
        this.daysRemaining = days;
    }
}
#endregion