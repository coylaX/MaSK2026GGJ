using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI; // 用于简化列表查询代码

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance;

    public WarehouseUI warehouseUI;

    public bool maskChooseState = true;
    public OrderUIController orderUICont;
    private string currentTargetOrderID;
    private MaskInstance selectedMask;
    public WarehouseItemUI selectedUI;
    [Header("提交的UI 组件")]
    public GameObject selectionOverlayPanel; // 全屏遮罩
    public GameObject confirmPopupPanel;     // 确认弹窗
    public TextMeshProUGUI confirmText;      // (可选) 弹窗上的文字，显示面具名
    //public Button exitMaskSubmit; //退出面具提交的按钮，触发CloseAll()


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

        //添加第一个记忆订单A
        if (data.currentDay == 1)
        {
            foreach (var template in allOrderTemplates)
            {
                if (template.id == MemoryTraitID.A)
                {
                    // 防重复检查：防止同一天多次刷新导致重复添加
                    bool alreadyExists = data.activeOrders.Any(o => o.orderID == template.orderID);

                    if (!alreadyExists)
                    {
                        // 3. 创建精简版订单 (只存ID和天数)
                        template.daysLimit = -4;
                        OrderData newOrder = new OrderData(template.orderID, template.daysLimit);
                        data.activeOrders.Add(newOrder);
                        Debug.Log($"[OrderManager] 新订单已生成: {template.customerName}");
                    }
                }
            }
        }

        // 2. 遍历所有模板，寻找符合今天日期的
        foreach (var template in allOrderTemplates)
        {
            if (template.ifMemory)
            {
                continue; // 👈 遇到这句，直接跳回第一行 foreach，取下一个值
            }
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
        //更新订单状态
        orderUICont.RefreshOrderList();
    }

    // 辅助：让所有存活的订单倒计时 -1，同时处理过期订单
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
                    OrderTemplate template = GetTemplateByID(order.orderID);
                }
            }
        }
    }

    //处理记忆订单,每次提交记忆订单后执行
    public void nextMemoeyOrder()
    {
        // 1. 获取当前的存档数据引用
        SaveData data = MorningGameManager.Instance.currentSaveData;
        int currentInt = (int)data.morningInventory.memoryNight;
        currentInt++;
        //如果达到最大枚举数，直接跳过
        if (currentInt > System.Enum.GetNames(typeof(MemoryTraitID)).Length) return;
        data.morningInventory.memoryNight = (MemoryTraitID)currentInt;

        //处理记忆订单
        foreach (var template in allOrderTemplates)
        {
            if (!template.ifMemory)
            {
                continue; // 👈 遇到这句，直接跳回第一行 foreach，取下一个值
            }
            if (template.id == data.morningInventory.memoryNight)
            {
                //记忆订单的daysRemaining记为-4
                OrderData newOrder = new OrderData(template.orderID, -4);
                data.activeOrders.Add(newOrder);
                Debug.Log($"[OrderManager] 新记忆订单已生成: {template.customerName}");
            }
        }
    }

    #endregion

    #region 3. 核心逻辑：提交与结算
    // ==========================================
    // 当玩家在 UI 上点击提交面具时调用
    // maskTags: 面具拥有的标签 (比如 ["Happy", "Red"])
    // ==========================================
    public void SubmitOrder(string orderID)
    {
        //进入面具选择模式，切换WarehouseItemUI的响应模式
        maskChooseState = false;
        currentTargetOrderID = orderID;

        // 打开遮罩/提示层
        selectionOverlayPanel.SetActive(true);

        Debug.Log($"开始为订单 {orderID} 选择面具...");
    }

    // ==================================================
    // 2. 中间流程：玩家点击了仓库里的某个面具
    // ==================================================
    public void OnMaskSelected(MaskInstance mask)
    {
        selectedMask = mask;
        // 显示确认弹窗
        confirmPopupPanel.SetActive(true);
        if (confirmText != null) confirmText.text = $"确定要提交 [{mask.displayName}] 吗？";
    }

    // ==================================================
    // 3. 确认提交：弹窗点击“确定”
    // ==================================================
    public void ConfirmSubmit()
    {
        if (selectedMask == null) return;

        // --- A. 数据转换 (把面具属性转成 List<string>) ---
        List<string> tags = new List<string>();
        tags.Add(selectedMask.emotionTraitID.ToString());
        tags.Add(selectedMask.memoryTraitID.ToString());
        tags.Add(selectedMask.colorTraitID.ToString());

        // --- B. 调用对比
        Submitorder(currentTargetOrderID, selectedMask);
        Debug.Log("面具提交成功！");

        // --- C. 结束选择流程 ---
        CloseAll();
    }
    // ==================================================
    // 4. 取消提交：弹窗点击“取消”
    // ==================================================
    public void CancelSubmit()
    {
        // 关闭弹窗，玩家可以重选
        confirmPopupPanel.SetActive(false);
        selectedMask = null;
    }

    // ==================================================
    // 5. 彻底退出选择模式:成功选择面具和退出面具选择模式时触发
    // ==================================================
    public void CloseAll()
    {
        maskChooseState = true;
        selectionOverlayPanel.SetActive(false);
        confirmPopupPanel.SetActive(false);
        selectedMask = null;
        currentTargetOrderID = "";
    }
    // ==================================================
    // 6. 对比订单要求和面具属性
    // ==================================================
    public void Submitorder(string orderID, MaskInstance mask)
    {
        // 1. 获取当前的存档数据引用
        SaveData data = MorningGameManager.Instance.currentSaveData;
        
        // 2. 找到对应的订单模板和存档数据
        OrderTemplate template = GetTemplateByID(orderID);
        OrderData activeData = data.activeOrders.Find(o => o.orderID == orderID);

        if (template == null)
        {
            Debug.LogError($"[OrderManager] 找不到 ID 为 {orderID} 的订单模板！");
            return;
        }

        Debug.Log($"[OrderManager] 开始判定订单: {template.customerName} - {orderID}");
        Debug.Log($"玩家提交的面具: {mask.displayName} (情绪:{mask.emotionTraitID}, 记忆:{mask.memoryTraitID}, 颜色:{mask.colorTraitID})");

        // 3. 执行核心比对逻辑
        bool isSuccess = CheckRequirements(template, mask);

        // 4. 计算奖励
        int finalreward = 0;
        string feedbacktext = "";
        if (isSuccess)
        {
            Debug.Log($"<color=green>【订单完成 - 好评】</color> {template.successReviewText}");
            activeData.daysRemaining = -1; // 完美完成 (好评)
            finalreward = template.baseReward;
            feedbacktext = template.successReviewText;
            Debug.Log($"[结算] 好评！获得 {finalreward}");
        }
        else
        {
            Debug.Log($"<color=green>【订单完成 - 差评】</color> {template.failReviewText}");
            activeData.daysRemaining = -3; // 完美完成 (好评)
            finalreward = template.baseReward/2;
            feedbacktext = template.failReviewText;
            Debug.Log($"[结算] 差评！获得 {finalreward}");
        }
        //刷新下一个记忆订单
        if (template.ifMemory)
        {
            nextMemoeyOrder();
        }
        //更新订单状态
        orderUICont.RefreshOrderList();
        //更新订单列表状态
        MorningGameManager.Instance.UpdateUI();

        // 5. (基础版暂定) 提交后，无论成功失败，都要消耗掉这个面具
        // 仓库移除提交的面具以及更新UI
        MaskInventory.I.maskInstances.Remove(mask);
        selectedUI.mask = null;
        selectedUI.GetComponent<Image>().color = Color.white;
        warehouseUI.Refresh();
        //提交订单获得的资金
        BagManager.Instance.EarnPigment(finalreward);
    }

    // =========================================================
    // 🔍 比对逻辑 (Private)
    // =========================================================
    private bool CheckRequirements(OrderTemplate template, MaskInstance mask)
    {
        // --- A. 检查 情绪 (Emotion) ---
        // 逻辑：如果模板要求不是 None，且 面具不符合，就是失败
        if (template.emotionTraitID != EmotionTraitID.None &&
            template.emotionTraitID != mask.emotionTraitID)
        {
            Debug.Log($"判定失败：情绪不匹配。要求 {template.emotionTraitID}，实际 {mask.emotionTraitID}");
            return false;
        }

        // --- B. 检查 记忆 (Memory) ---
        if (template.memoryTraitID != MemoryTraitID.None &&
            template.memoryTraitID != mask.memoryTraitID)
        {
            Debug.Log($"判定失败：记忆不匹配。要求 {template.memoryTraitID}，实际 {mask.memoryTraitID}");
            return false;
        }

        // --- C. 检查 颜色 (Color) ---
        if (template.colorTraitID != ColorTraitID.None &&
            template.colorTraitID != mask.colorTraitID)
        {
            Debug.Log($"判定失败：颜色不匹配。要求 {template.colorTraitID}，实际 {mask.colorTraitID}");
            return false;
        }

        // 如果三个检查都通过了（或者模板全是 None），那就是成功
        return true;
    }


    // 7. 触发存档
    //morninggamemanager.instance.savegame();
    //    findobjectoftype<orderuicontroller>()?.refreshorderlist();
    // todo: 这里可以发送一个事件给 ui，显示 feedbacktext

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

