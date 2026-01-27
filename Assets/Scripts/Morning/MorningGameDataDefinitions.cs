using System.Collections.Generic;
using UnityEngine;
using static TMPro.Examples.ObjectSpin;

// --- 文件名: MorningGameDataDefinitions.cs ---

#region 1. 枚举定义 (Enums)


#endregion

#region 2. 情绪存储容器 (EmotionStorage)
// ==========================================
// 专门用于处理4种情绪数量的类
// ==========================================

[System.Serializable]

#endregion

#region 3. 经营背包数据 (Inventory)
// ==========================================
// 包含颜料、情绪、记忆的数据结构
// ==========================================


public class MorningInventoryData
{
    public EmotionTraitID emotionTraitID;

    // --- 资源 A: 颜料 ---
    public int pigmentAmount;

    // --- 资源 B: 情绪 (引用上面的类) ---
    public int xiCount;
    public int nuCount;
    public int aiCount;
    public int leCount;

    // --- 资源 C: 记忆进度 (只记录进度) ---
    public int memoryProcess;

    public MorningInventoryData()
    {
        pigmentAmount = 0;
        xiCount = 0;
        nuCount = 0;
        aiCount = 0;
        leCount = 0;
        memoryProcess = 0;
    }
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