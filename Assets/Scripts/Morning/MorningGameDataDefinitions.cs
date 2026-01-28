using System.Collections.Generic;
using UnityEngine;
using static TMPro.Examples.ObjectSpin;


#region 3. 经营背包数据 (Inventory)
// ==========================================
// 包含颜料、情绪、记忆的数据结构
// ==========================================
[System.Serializable]
public class MorningInventoryData
{
    public EmotionTraitID emotionTraitID;
    public MemoryTraitID memoryTraitID;

    // --- 资源 A: 颜料 ---
    public int pigmentAmount;

    // --- 资源 B: 情绪 (引用上面的类) ---
    public int xiCount;
    public int nuCount;
    public int aiCount;
    public int leCount;

    // --- 资源 C: 记忆进度 (前面是获得的记忆，后面是关卡当前阶段的记忆) ---
    public List<MemoryTraitID> memoryGet;
    public MemoryTraitID memoryNight;

    public MorningInventoryData()
    {
        pigmentAmount = 10;
        xiCount = 5;
        nuCount = 5;
        aiCount = 5;
        leCount = 5;
        memoryGet = new List<MemoryTraitID>();
        memoryNight = MemoryTraitID.A;
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