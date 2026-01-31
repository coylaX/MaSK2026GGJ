using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI; // 用于简化列表查询代码

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }

    public bool maskChooseState = true;
    private string currentTargetOrderID;
    private MaskInstance selectedMask;
    [Header("请保持该项为空")]
    public WarehouseItemUI selectedUI;
    [Header("提交的UI 组件")]
    public GameObject selectionOverlayPanel; // 全屏遮罩
    public GameObject confirmPopupPanel;     // 确认弹窗
    public TextMeshProUGUI confirmText;      // (可选) 弹窗上的文字，显示面具名
    //public Button exitMaskSubmit; //退出面具提交的按钮，触发CloseAll()

    EmotionResult emotionMatch = EmotionResult.Right;

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

    #region 2. 核心逻辑：数据层刷新每日订单
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
                if (template.memoryTraitID == MemoryTraitID.A)
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
        MorningGameManager.Instance.UpdateOrderUI();
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
    public void NextMemoeyOrder()
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
            if (template.memoryTraitID == data.morningInventory.memoryNight)
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
    // 1.当玩家在 UI 上点击提交面具时调用，UI进入面具选择模式
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
    // 2. 中间流程：玩家点击了仓库里的某个面具，数据层读取
    // ==================================================
    public void OnMaskSelected(MaskInstance mask)
    {
        selectedMask = mask;
        // 显示确认弹窗
        confirmPopupPanel.SetActive(true);
        if (confirmText != null) confirmText.text = $"确定要提交 [{mask.displayName}] 吗？";
    }

    // ==================================================
    // 3. 确认提交：按钮调用，弹窗点击“确定”
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
    // 4. 取消提交：按钮调用，弹窗点击“取消”
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
    /// <summary>
    /// 对比结果枚举
    /// </summary>
    public enum SubmitResult
    {
        Best,
        Success,
        Afail,
        Bfail,
        Cfail,
        AMRfail,
        BMRfail,
        CMRfail,
        AMFfail,
        BMFfail,
        CMFfail,
        MemoryFail
    }

    /// <summary>
    /// 调用和修改存档数据
    /// </summary>
    /// <param name="orderID"></param>
    /// <param name="mask"></param>
    public void Submitorder(string orderID, MaskInstance mask)
    {
        Debug.Log("[Submitorder]开始运行");
        // 1. 获取当前的存档数据引用
        SaveData data = MorningGameManager.Instance.currentSaveData;

        // 2. 找到对应的订单模板（template）和存档数据（activeData）
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
        SubmitResult matchResult = CheckRequirements(template, mask);

        // 4. 计算颜料奖励
        int finalreward = 0;
        if(emotionMatch == EmotionResult.Right)
        {
            finalreward = template.baseReward;
        }
        else if(emotionMatch != EmotionResult.Right)
        {
            finalreward = template.baseReward / 2;
        }

        // 4. 计算评价
        activeData.daysRemaining = matchResult switch
        {
            // 真好评
            SubmitResult.Best => -1,

            // 次好评
            SubmitResult.Success => -3,

            // 普通差评 (a, b, c)
            SubmitResult.Afail => -5,
            SubmitResult.Bfail => -6,
            SubmitResult.Cfail => -7,

            // 记忆匹配差评 (a, b, c)
            SubmitResult.AMRfail => -8,
            SubmitResult.BMRfail => -9,
            SubmitResult.CMRfail => -10,

            // 记忆错误差评 (a, b, c)
            SubmitResult.AMFfail => -11,
            SubmitResult.BMFfail => -12,
            SubmitResult.CMFfail => -13,

            // 完全差评
            SubmitResult.MemoryFail => -14,

            // 兜底（理论上枚举全覆盖了，不需要这个，但为了语法安全填个0）
            _ => -1
        };

        //特殊：刷新下一个记忆订单
        if (template.ifMemory)
        {
            NextMemoeyOrder();
        }

        Debug.Log("对比成功");
        //更新订单状态
        MorningGameManager.Instance.UpdateOrderUI();

        // 5. (基础版暂定) 提交后，无论成功失败，都要消耗掉这个面具
        // 仓库移除提交的面具以及更新UI（仓库和背包UI）
        MaskInventory.I.maskInstances.Remove(mask);
        selectedUI.mask = null;
        selectedUI.GetComponent<Image>().color = Color.white;
        MorningGameManager.Instance.Refresh();

        //提交订单获得的资金
        BagManager.Instance.EarnPigment(finalreward);
    }

    // =========================================================
    // 🔍 比对逻辑 (Private)
    // =========================================================
    enum EmotionResult
    {
        Right,
        a,
        b,
        c,
    }
    /// <summary>
    /// 执行对比逻辑，输出判定结果（5个）
    /// </summary>
    /// <param name="template"></param>
    /// <param name="mask"></param>
    /// <returns></returns>
    private SubmitResult CheckRequirements(OrderTemplate template, MaskInstance mask)
    {
        bool memoryMatch=true;
        bool colorMatch = true;
        Debug.Log("[CheckRequirements]已运行");
        // --- A. 检查 情绪 (Emotion) ---
        // 逻辑：如果模板要求不是 None，且 面具符合，就是sucess
        if (template.emotionTraitID != EmotionTraitID.None &&
            template.emotionTraitID == mask.emotionTraitID)
        {
            Debug.Log($"判定失败：情绪不匹配。要求 {template.emotionTraitID}，实际 {mask.emotionTraitID}");
            emotionMatch = EmotionResult.Right;
        }
        else
        {
            emotionMatch = (template.emotionTraitID, mask.emotionTraitID) switch
            {
                // --- 情况 1：Template 为 XI ---
                // 剩余顺序: NU, AI, LE -> 对应 a, b, c
                (EmotionTraitID.XI, EmotionTraitID.NU) => EmotionResult.a,
                (EmotionTraitID.XI, EmotionTraitID.AI) => EmotionResult.b,
                (EmotionTraitID.XI, EmotionTraitID.LE) => EmotionResult.c,

                // --- 情况 2：Template 为 NU ---
                // 剩余顺序: XI, AI, LE -> 对应 a, b, c
                (EmotionTraitID.NU, EmotionTraitID.XI) => EmotionResult.a,
                (EmotionTraitID.NU, EmotionTraitID.AI) => EmotionResult.b,
                (EmotionTraitID.NU, EmotionTraitID.LE) => EmotionResult.c,

                // --- 情况 3：Template 为 AI ---
                // 剩余顺序: XI, NU, LE -> 对应 a, b, c
                (EmotionTraitID.AI, EmotionTraitID.XI) => EmotionResult.a,
                (EmotionTraitID.AI, EmotionTraitID.NU) => EmotionResult.b,
                (EmotionTraitID.AI, EmotionTraitID.LE) => EmotionResult.c,

                // --- 情况 4：Template 为 LE ---
                // 剩余顺序: XI, NU, AI -> 对应 a, b, c
                (EmotionTraitID.LE, EmotionTraitID.XI) => EmotionResult.a,
                (EmotionTraitID.LE, EmotionTraitID.NU) => EmotionResult.b,
                (EmotionTraitID.LE, EmotionTraitID.AI) => EmotionResult.c,

                // --- 默认情况 (比如两者相等，或者发生错误) ---
                // 返回 emotionMatch 原有的值，或者返回 0，根据你的需求修改
                _ => emotionMatch
            };
        }

        // --- B. 检查 记忆 (Memory) ---
        if (template.memoryTraitID != MemoryTraitID.None &&
            template.memoryTraitID != mask.memoryTraitID)
        {
            Debug.Log($"判定失败：记忆不匹配。要求 {template.memoryTraitID}，实际 {mask.memoryTraitID}");
            memoryMatch = false;
        }

        // --- C. 检查 颜色 (Color) ---
        if (template.colorTraitIDs.Contains(ColorTraitID.None) &&
            template.colorTraitIDs.Contains(mask.colorTraitID))
        {
            Debug.Log($"判定失败：颜色不匹配。要求 {string.Join(", ", template.colorTraitIDs)}，实际 {mask.colorTraitID}");
            colorMatch = false;
        }

        // 返回结果
        if (!template.ifMemory)
        {
            // 使用元组匹配：(emotion, memory, color)
            return (emotionMatch, memoryMatch, colorMatch) switch
            {
                // 1. (Right，A，T) → Best
                // _ 代表 "A" (All/Any)，即忽略 memoryMatch 是 T 还是 F
                (EmotionResult.Right, _, true) => SubmitResult.Best,

                // 2. (Right，A，F) → Success
                (EmotionResult.Right, _, false) => SubmitResult.Success,

                // 3. (a，A，T) → Afail
                (EmotionResult.a, _, true) => SubmitResult.Afail,

                // 4. (b，A，T) → Bfail
                (EmotionResult.b, _, true) => SubmitResult.Bfail,

                // 5. (c，A，T) → Cfail
                (EmotionResult.c, _, true) => SubmitResult.Cfail,

                // --- 扩展逻辑预留 ---
                // 你提供的规则只覆盖了上面5种情况。
                // 如果输入了 (a, _, false) 这种未定义的组合，需要一个默认返回值。
                // 这里暂时返回 MemoryFail 作为兜底，你可以根据需要修改。
                _ => SubmitResult.MemoryFail
            };
        }else if (template.ifMemory)
        {
            // 逻辑判定表：(Emotion, Memory, Color)
            return (emotionMatch, memoryMatch, colorMatch) switch
            {
                // --- Right 且 Memory 为 True 的情况 ---
                // (Right, T, T) → Best
                (EmotionResult.Right, true, true) => SubmitResult.Best,
                // (Right, T, F) → Success
                (EmotionResult.Right, true, false) => SubmitResult.Success,

                // --- Memory 为 True，Emotion 错误的情况 (忽略 Color) ---
                // (a, T, A) → AMRfail
                (EmotionResult.a, true, _) => SubmitResult.AMRfail,
                // (b, T, A) → BMRfail
                (EmotionResult.b, true, _) => SubmitResult.BMRfail,
                // (c, T, A) → CMRfail
                (EmotionResult.c, true, _) => SubmitResult.CMRfail,

                // --- Memory 为 False，Emotion 错误的情况 (忽略 Color) ---
                // (a, F, A) → AMFfail
                (EmotionResult.a, false, _) => SubmitResult.AMFfail,
                // (b, F, A) → BMFfail
                (EmotionResult.b, false, _) => SubmitResult.BMFfail,
                // (c, F, A) → CMFfail
                (EmotionResult.c, false, _) => SubmitResult.CMFfail,

                // --- Memory 为 False，但 Emotion 对了的情况 (忽略 Color) ---
                // (Right, F, A) → MemoryFail
                (EmotionResult.Right, false, _) => SubmitResult.MemoryFail,

                // 兜底（理论上上面的逻辑已经覆盖了 100% 的可能性）
                _ => SubmitResult.MemoryFail
            };
        }
        Debug.Log("出现判定错误，请检查代码或面具");
        return SubmitResult.Best;
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

