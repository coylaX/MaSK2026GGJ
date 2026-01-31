using UnityEngine;
using System.Collections.Generic;
using System.Linq; // 必须引用，用于排序

public class OrderUIController : MonoBehaviour
{
    [Header("配置")]
    public GameObject orderItemPrefab;
    public GameObject memoryOrderItemPrefab;
    public Transform contentRoot;

    private void Start()
    {
        RefreshOrderList();
    }

    #region 刷新订单列表和订单状态
    public void RefreshOrderList()
    {
        Debug.Log("订单UI刷新了");
        // 1. 清空旧列表
        foreach (Transform child in contentRoot)
        {
            Destroy(child.gameObject);
        }

        // 2. 获取所有数据
        List<OrderData> allOrders = MorningGameManager.Instance.currentSaveData.activeOrders;

        // ================================================================
        // 核心排序逻辑修改
        // ================================================================

        // 第一步：筛选出“进行中”的记忆订单 (days =-4)
        var activeMemoryGroup = allOrders
            .Where(o => o.daysRemaining == -4)
            .OrderBy(o => o.daysRemaining) // 规则：越早到期越靠前 (升序)
            .ToList();

        // 第一步：筛选出“进行中”的订单 (days > 0)
        var activeGroup = allOrders
            .Where(o => o.daysRemaining > 0)
            .OrderBy(o => o.daysRemaining) // 规则：越早到期越靠前 (升序)
            .ToList();

        // 第二步：筛选出“已结束”的订单 (days <= 0，包括完成和逾期)
        var finishedGroup = allOrders
            .Where(o => o.daysRemaining <= 0 && o.daysRemaining != -4)
            .OrderByDescending(o => GetOrderAppearDay(o.orderID)) // 规则：越晚出现的越靠前 (降序)
            .ToList();

        // 第三步：合并 (进行中 在前，已结束 在后)
        var finalSortedList = new List<OrderData>();
        finalSortedList.AddRange(activeMemoryGroup);
        finalSortedList.AddRange(activeGroup);
        finalSortedList.AddRange(finishedGroup);

        // ================================================================

        // 3. 生成列表
        foreach (var data in finalSortedList)
        {
            CreateOrderCard(data);
        }
    }
    #endregion

    #region 辅助方法（被使用的）
    private void CreateOrderCard(OrderData data)
    {
        OrderTemplate template = OrderManager.Instance.GetTemplateByID(data.orderID);
        if (template != null&& !template.ifMemory)
        {
            GameObject obj = Instantiate(orderItemPrefab, contentRoot);
            obj.GetComponent<OrderUIItem>().Setup(data, template);
        }else if(template != null && template.ifMemory)
        {
            GameObject obj = Instantiate(memoryOrderItemPrefab, contentRoot);
            obj.GetComponent<OrderUIItem>().Setup(data, template);
        }
    }

    // 辅助方法：为了排序，我们需要查表获取这个订单是第几天出现的
    private int GetOrderAppearDay(string orderID)
    {
        OrderTemplate t = OrderManager.Instance.GetTemplateByID(orderID);
        if (t.ifMemory)
            return 99;
        // 如果找不到模板（理论不该发生），返回0垫底
        return t != null ? t.appearOnDay : 0;
    }
    #endregion
}