using UnityEngine;
using UnityEngine.UI;
using TMPro; // 确保你安装了 TextMeshPro

public class OrderUIItem : MonoBehaviour
{
    [Header("UI 组件")]
    public Image avatarImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI timeText;     // 剩余天数/状态
    public TextMeshProUGUI reviewText;   // 需求/评价
    public TextMeshProUGUI rewardText;   // 报酬金额
    public Button submitButton;          // 提交按钮
    public Button expandButton;          // 点击文字展开的隐形按钮

    public bool isFinish=false;

    [Header("颜色配置")]
    public Color activeBtnColor = Color.white;
    public Color completedBtnColor = new Color(0.8f, 1f, 0.8f); // 淡绿
    public Color expiredBtnColor = Color.gray;

    private bool isExpanded = false;
    private RectTransform rectTransform; // 用于强制刷新布局

    private void Awake()
    {
        // 获取自身的 RectTransform，用于后续刷新高度
        rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>
    /// 刷新订单Ui的所有信息
    /// </summary>
    /// <param name="data"></param>
    /// <param name="template"></param>
    public void Setup(OrderData data, OrderTemplate template)
    {
        // 1. 基础信息
        nameText.text = template.customerName;
        avatarImage.sprite = template.portrait;
        Debug.Log("[OrderUIItem]开始加载订单UI");
        // 2. 状态分流
        // --- 记忆订单 ---
        if (template.ifMemory && data.daysRemaining == -4)
        {
            Debug.Log("[OrderUIItem]这是一个未完成的记忆订单");
            //处理记忆订单
            timeText.text = "记忆订单";
            reviewText.text = template.requirementText;
            rewardText.text = $"Reward: {template.baseReward}";

            //buttonText.text = "提交面具";
            submitButton.interactable = true;
            submitButton.image.color = activeBtnColor;

            submitButton.onClick.RemoveAllListeners();
            submitButton.onClick.AddListener(() => {
                Debug.Log($"打开仓库，准备提交订单: {template.orderID}");
                OrderManager.Instance.SubmitOrder(template.orderID);
            });
        }
        // --- A. 进行中 ---
        else if (data.daysRemaining > 0)
        {
            Debug.Log("[OrderUIItem]这是一个未完成的普通订单");
            timeText.text = $"Remain <color=red>{data.daysRemaining}</color> Days";
            reviewText.text = template.requirementText;
            rewardText.text = $"Reward: {template.baseReward}";

            //buttonText.text = "提交面具";
            submitButton.interactable = true;
            submitButton.image.color = activeBtnColor;

            submitButton.onClick.RemoveAllListeners();
            submitButton.onClick.AddListener(() => {
                Debug.Log($"打开仓库，准备提交订单: {template.orderID}");
                OrderManager.Instance.SubmitOrder(template.orderID);
            });
        }
        // --- C. 已逾期 ---
        else if (data.daysRemaining == -2)
        {
            Debug.Log("[OrderUIItem]这是一个过期的记忆订单");
            timeText.text = "<color=grey>已过期</color>";
            reviewText.text = template.requirementText; // 或者显示 "任务失效"
            rewardText.text = "Reward: 0";

            //buttonText.text = "失效";
            submitButton.interactable = false;
            submitButton.image.color = expiredBtnColor;
        }
        // --- D. 已完成 (-1 -3 -5等等) ---
        else
        {
            Debug.Log("[OrderUIItem]这是一个完成的订单");
            isFinish = true;
            

            // 判断是 -1(好评) 还是 -3(差评)
            // 根据 daysRemaining 的值选择对应的字符串
            string content = data.daysRemaining switch
            {
                -1 => template.bestReviewText,
                -3 => template.successReviewText,

                // 普通差评 (-5 到 -7)
                -5 => template.aFailReviewText,
                -6 => template.bFailReviewText,
                -7 => template.cFailReviewText,

                // 记忆匹配差评 (-8 到 -10)
                -8 => template.aMRFailReviewText,
                -9 => template.bMRFailReviewText,
                -10 => template.cMRFailReviewText,

                // 记忆错误差评 (-11 到 -13)
                -11 => template.aMFFailReviewText,
                -12 => template.bMFFailReviewText,
                -13 => template.cMFFailReviewText,

                // 完全差评 (-14)
                -14 => template.MemoryFailRviewText,

                // 默认情况（如果没有匹配到上述数字）
                _ => ""
            };

            Debug.Log("orderUI已经更新了");
            // 最后赋值给 UI
            timeText.text = "<color=green>已完成</color>";
            reviewText.text = content;
            if (data.daysRemaining==-1|| data.daysRemaining == -3|| data.daysRemaining == -14)
            {
                rewardText.text = $"Reward: {template.baseReward}"; // 成功的报酬显示
            }
            else
            {
                rewardText.text = $"Reward: {template.baseReward/2}"; // 不成功的报酬显示
            }
            

            //buttonText.text = isGood ? "完美完成" : "勉强完成";
            submitButton.interactable = false;
            submitButton.image.color = completedBtnColor;
        }

        // 3. 展开逻辑初始化
        isExpanded = false;

        // 绑定点击事件
        expandButton.onClick.RemoveAllListeners();
        expandButton.onClick.AddListener(() => {
            isExpanded = !isExpanded;
            ExpandReviewText();
        });
        Debug.Log("结束加载订单UI");
    }

    void ExpandReviewText()
    {
        // 1. 获取当前显示的文字
        // 因为你在 Setup 里已经根据状态（进行中/已完成/逾期）设置好了 reviewText.text
        // 所以直接读它就是最准确的。
        string currentContent = reviewText.text;

        // 2. 获取当前标题 (可选，可以用客户名字，或者直接写“订单详情”)
        string title="";
        if (!isFinish)
            title = nameText.text + " 的订单";
        else if (isFinish)
        {
            title = nameText.text + " 的评价";
        }
        // 3. 呼叫弹窗
        if (OrderDetailPopup.Instance != null)
        {
            OrderDetailPopup.Instance.Show(title, currentContent);
        }
        else
        {
            Debug.LogError("场景里找不到 OrderDetailPopup！请确保你把弹窗Prefab放进了场景里。");
        }
    }
}