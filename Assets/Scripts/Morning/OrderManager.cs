using System.Collections.Generic;
using UnityEngine;
using System.Linq; // 用于简化列表查询代码

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance;

    #region 1. 完整的订单列表，包含所有Order，你可以在Inspector中增加order
    // ==========================================
    // 这里的列表相当于“字典”，存放游戏中所有可能出现的订单配置
    // 请在 Unity 编辑器里把做好的 OrderTemplate 文件拖进去
    // ==========================================
    [Header("订单数据库")]
    public List<OrderTemplate> allOrderTemplates;

    private void Awake()
    {
        Instance = this;
    }
    #endregion

    #region 2. 核心逻辑：刷新每日订单
    // ==========================================
    // 每天早上调用，检查有哪些订单应该今天出现
    // ==========================================
    public void RefreshDailyOrders(int currentDay)
    {
        // 1. 获取当前的存档数据引用
        SaveData data = MorningGameManager.Instance.currentSaveData;

        // 2. 遍历所有模板，寻找符合今天日期的
        foreach (var template in allOrderTemplates)
        {
            if (template.appearOnDay == currentDay)
            {
                // 防重复检查：防止同一天多次刷新导致重复添加
                bool alreadyExists = data.activeOrders.Any(o => o.orderID == template.orderID);

                if (!alreadyExists)
                {
                    // 3. 创建精简版订单 (只存ID和天数)
                    OrderData newOrder = new OrderData(template.orderID, template.daysLimit);
                    data.activeOrders.Add(newOrder);

                    Debug.Log($"[OrderManager] 新订单已生成: {template.customerName}");
                }
            }
        }

        // 4. 处理旧订单的时间流逝
        ProcessOrdersTimePass(data);
    }

    // 辅助：让所有存活的订单倒计时 -1
    private void ProcessOrdersTimePass(SaveData data)
    {
        foreach (var order in data.activeOrders)
        {
            // 如果订单还在进行中 (daysRemaining > 0)
            if (order.daysRemaining > 0)
            {
                order.daysRemaining--;

                // 如果减完变成了0，标记为过期 (-2)
                if (order.daysRemaining == 0)
                {
                    order.daysRemaining = -2;
                    Debug.Log($"[OrderManager] 订单已过期: {order.orderID}");
                }
            }
        }
    }
    #endregion

    #region 3. 核心逻辑：提交与结算
    // ==========================================
    // 当玩家在 UI 上点击提交面具时调用
    // maskTags: 面具拥有的标签 (比如 ["Happy", "Red"])
    // ==========================================
    public void SubmitOrder(string orderID, List<string> maskTags)
    {
        // 1. 找到存档里的动态数据
        SaveData data = MorningGameManager.Instance.currentSaveData;
        OrderData activeOrder = data.activeOrders.Find(o => o.orderID == orderID);

        // 2. 找到数据库里的静态模板 (查表)
        OrderTemplate template = GetTemplateByID(orderID);

        if (activeOrder == null || template == null) return;

        // 3. 判定逻辑：检查面具是否包含了模板要求的所有标签
        // (这里假设要求是全匹配，你也可以改成匹配部分)
        bool isSuccess = CheckTagsMatch(template.tags, maskTags);

        // 4. 计算奖励
        int finalReward = 0;
        string feedbackText = "";

        if (isSuccess)
        {
            activeOrder.daysRemaining = -1; // 完美完成 (好评)
            finalReward = template.baseReward;
            feedbackText = template.successReviewText;
            Debug.Log($"[结算] 好评！获得 {finalReward}");
            if (template.ifMemory)
            {
                BagManager.Instance.earnMemory(template.id);
            }
        }
        else
        {
            activeOrder.daysRemaining = -3; // 勉强完成 (差评)
            finalReward = template.baseReward / 2; // 失败获得一半低保
            feedbackText = template.failReviewText;
            Debug.Log($"[结算] 差评... 获得 {finalReward}");
        }

        // 5. 发放奖励 (更新背包)
        data.morningInventory.pigmentAmount += finalReward;

        // 6. 标记订单完成 (-1 代表已完成)
        activeOrder.daysRemaining = -1;

        // 7. 触发存档
        MorningGameManager.Instance.SaveGame();
        FindObjectOfType<OrderUIController>()?.RefreshOrderList();
        // TODO: 这里可以发送一个事件给 UI，显示 feedbackText
    }

    // 辅助：比对标签
    private bool CheckTagsMatch(List<string> required, List<string> provided)
    {
        // 如果没有要求，直接通过
        if (required == null || required.Count == 0) return true;

        // 检查 required 里的每一个标签，是否都在 provided 里出现过
        foreach (var tag in required)
        {
            if (!provided.Contains(tag))
            {
                return false; // 只要缺一个，就算失败
            }
        }
        return true;
    }
    #endregion

    #region 4. 工具方法：查表 (Rehydration)
    // ==========================================
    // 给 UI 使用：通过 ID 拿回名字、头像、描述等信息
    // ==========================================
    public OrderTemplate GetTemplateByID(string id)
    {
        return allOrderTemplates.Find(t => t.orderID == id);
    }

    // 给 UI 使用：获取当前所有“进行中”的订单
    public List<OrderData> GetActiveOnlyOrders()
    {
        SaveData data = MorningGameManager.Instance.currentSaveData;
        // 返回所有 daysRemaining > 0 的订单
        return data.activeOrders.FindAll(o => o.daysRemaining > 0);
    }
    #endregion
}
