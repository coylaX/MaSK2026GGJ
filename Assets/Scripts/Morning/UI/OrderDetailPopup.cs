using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrderDetailPopup : MonoBehaviour
{
    public static OrderDetailPopup Instance; // 单例，方便全局调用

    [Header("UI 组件")]
    public GameObject canvasGroup; // 或者直接用 GameObject panelRoot
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI contentText;
    public Button exitButton; // click exit button to leave
    public Button backgroundExitButton; // click background to leave

    private void Awake()
    {
        // 简单的单例模式
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // 初始隐藏
        Hide();

        // 绑定关闭事件
        if (exitButton) exitButton.onClick.AddListener(Hide);
        if (backgroundExitButton) backgroundExitButton.onClick.AddListener(Hide);
    }

    public void Show(string title, string content)
    {
        gameObject.SetActive(true); // 确保物体开启
        if (canvasGroup) canvasGroup.SetActive(true);

        if (titleText) titleText.text = title;
        contentText.text = content;

        // 强制把滚动条复位到顶部 (如果你用了 Scroll View)
        // contentText.rectTransform.anchoredPosition = Vector2.zero; 
    }

    public void Hide()
    {
        if (canvasGroup) canvasGroup.SetActive(false);
        // gameObject.SetActive(false); // 如果想彻底关掉物体
    }
}