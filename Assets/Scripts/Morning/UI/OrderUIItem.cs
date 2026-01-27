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

    public void Setup(OrderData data, OrderTemplate template)
    {
        // 1. 基础信息
        nameText.text = template.customerName;
        avatarImage.sprite = template.portrait;

        // 2. 状态分流
        // --- A. 进行中 ---
        if (data.daysRemaining > 0)
        {
            timeText.text = $"剩余 <color=red>{data.daysRemaining}</color> 天";
            reviewText.text = template.requirementText;
            rewardText.text = $"报酬: {template.baseReward}";

            //buttonText.text = "提交面具";
            submitButton.interactable = true;
            submitButton.image.color = activeBtnColor;

            submitButton.onClick.RemoveAllListeners();
            submitButton.onClick.AddListener(() => {
                Debug.Log($"打开仓库，准备提交订单: {template.orderID}");
                // TODO: 在这里调用面具选择界面
            });
        }
        // --- B. 已完成 (好评 -1 或 差评 -3) ---
        // 🔴 修正：这里必须用 || (或)，不能用 && (且)
        else if (data.daysRemaining == -1 || data.daysRemaining == -3)
        {
            timeText.text = "<color=green>已完成</color>";

            // 判断是 -1(好评) 还是 -3(差评)
            bool isGood = (data.daysRemaining == -1);
            reviewText.text = isGood ? template.successReviewText : template.failReviewText;

            rewardText.text = ""; // 完成后隐藏报酬显示

            //buttonText.text = isGood ? "完美完成" : "勉强完成";
            submitButton.interactable = false;
            submitButton.image.color = completedBtnColor;
        }
        // --- C. 已逾期 ---
        else if (data.daysRemaining == -2)
        {
            timeText.text = "<color=grey>已逾期</color>";
            reviewText.text = template.requirementText; // 或者显示 "任务失效"
            rewardText.text = "报酬: 0";

            //buttonText.text = "失效";
            submitButton.interactable = false;
            submitButton.image.color = expiredBtnColor;
        }

        // 3. 展开逻辑初始化
        isExpanded = false;

        // 绑定点击事件
        expandButton.onClick.RemoveAllListeners();
        expandButton.onClick.AddListener(() => {
            isExpanded = !isExpanded;
            ExpandReviewText();
        });
    }

    void ExpandReviewText()
    {
        // 1. 获取当前显示的文字
        // 因为你在 Setup 里已经根据状态（进行中/已完成/逾期）设置好了 reviewText.text
        // 所以直接读它就是最准确的。
        string currentContent = reviewText.text;

        // 2. 获取当前标题 (可选，可以用客户名字，或者直接写“订单详情”)
        string title = nameText.text + " 的订单";

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