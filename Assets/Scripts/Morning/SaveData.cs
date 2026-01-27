using System.Collections.Generic; // 必须引用，为了使用 List
using UnityEngine;

[System.Serializable] // 🔴 极其重要！没有这一行，JsonUtility 就无法保存它
public class SaveData
{
    // ==========================================
    // 1. 全局进度
    // ==========================================
    public int currentDay; // 当前是第几天

    // ==========================================
    // 2. 经营背包数据
    // (这个类定义在 MorningGameDataDefinitions.cs 中)
    // ==========================================
    public MorningInventoryData morningInventory;

    // ==========================================
    // 3. 订单列表
    // (注意：这里存储的是我们精简后的 OrderData，只包含 ID 和 剩余天数)
    // ==========================================
    public List<OrderData> activeOrders;

    // ==========================================
    // 构造函数 (初始化)
    // 当 SaveManager 创建“新游戏”时，会调用这个
    // ==========================================
    public SaveData()
    {
        // 默认从第 1 天开始
        currentDay = 1;

        // 初始化背包 (必须 new 出来，否则是 null 会报错)
        morningInventory = new MorningInventoryData();

        // 初始化订单列表 (必须 new 出来，否则是 null)
        activeOrders = new List<OrderData>();
    }
}