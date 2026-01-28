using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SettlementUI : MonoBehaviour
{
    public static SettlementUI Instance;

    [Header("渐变与延迟配置")]
    public CanvasGroup canvasGroup; 
    public float fadeDuration = 1.0f;     
    public float buttonDelay = 1.5f;      

    [Header("UI 联动")]
    public List<GameObject> combatUIs; 

    [Header("文案配置 - 失败 (沉睡)")]
    [TextArea(1, 2)] public string failTitle = "迷失梦中";
    [TextArea(3, 5)] public string failDesc = "忘记了梦中的一些事物。那种感觉很熟悉，却记不清了...";

    [Header("文案配置 - 成功 (撤离)")]
    [TextArea(1, 2)] public string successTitle = "醒来了";
    [TextArea(3, 5)] public string successDesc = "精神抖擞的醒来，梦中的事物仍历历在目";

    [Header("UI 组件引用")]
    public GameObject settlementPanel;
    public Image backgroundDim;           
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI remainingText;
    
    [Header("撤离过程视觉引用")]
    public Image evacWhiteFilter;
    public TextMeshProUGUI evacSubtitle;

    [Header("继续按钮样式控制")]
    public Button continueButton;
    public Image continueBtnImage;        
    public TextMeshProUGUI continueBtnText; 

    private void Awake() { Instance = this; }

    private void OnEnable()
    {
        // 死亡时触发失败结算
        PlayerEvents.OnPlayerDeath += () => ShowSettlement(false);
    }

    private void Start()
    {
        if (settlementPanel != null) settlementPanel.SetActive(false);
        if (canvasGroup != null) canvasGroup.alpha = 0;
        
        if (continueButton != null) 
        {
            continueButton.onClick.AddListener(OnContinuePressed);
            continueButton.interactable = false; 
        }

        // 初始化强制隐藏撤离相关 UI
        if (evacWhiteFilter != null) evacWhiteFilter.gameObject.SetActive(false);
        if (evacSubtitle != null) evacSubtitle.gameObject.SetActive(false);
    }

    public void ShowSettlement(bool isSuccess)
    {
        // 1. 隐藏战斗 UI 和撤离小字
        foreach (GameObject ui in combatUIs) if (ui != null) ui.SetActive(false);
        if (evacSubtitle != null) evacSubtitle.gameObject.SetActive(false);

        // 2. 只有在失败时执行损失逻辑
        if (!isSuccess)
        {
            if (CollectionManager.Instance != null) CollectionManager.Instance.HandleDeathLoss();
            // 死亡时不需要白光滤镜，直接关闭
            if (evacWhiteFilter != null) evacWhiteFilter.gameObject.SetActive(false);
        }

        // 3. 应用样式与文案
        ApplySettlementStyle(isSuccess);

        // 4. 显示面板并处理 Alpha 逻辑
        if (settlementPanel != null) settlementPanel.SetActive(true);
        
        if (isSuccess && canvasGroup != null)
        {
            // 【核心修改】：成功撤离，Alpha 直接拉满，实现背景瞬间变白
            canvasGroup.alpha = 1;
        }
        else if (canvasGroup != null)
        {
            // 失败：初始透明度为 0
            canvasGroup.alpha = 0;
        }
        
        UpdateRemainingItemsDisplay();

        // 5. 开启计时逻辑 (传入成功/失败状态以区分是否跳过渐变)
        StopAllCoroutines(); 
        StartCoroutine(FadeAndEnableButtonRoutine(isSuccess));

        Time.timeScale = 0;
    }

    private void ApplySettlementStyle(bool isSuccess)
    {
        titleText.text = isSuccess ? successTitle : failTitle;
        descriptionText.text = isSuccess ? successDesc : failDesc;

        // 背景颜色反转逻辑
        if (backgroundDim != null) 
        {
            Color bgColor = isSuccess ? Color.white : Color.black;
            bgColor.a = 1f; // 确保不透明度为 100%
            backgroundDim.color = bgColor;
        }

        Color mainTextColor = isSuccess ? Color.black : Color.white;
        if (titleText != null) titleText.color = mainTextColor;
        if (descriptionText != null) descriptionText.color = mainTextColor;
        if (remainingText != null) remainingText.color = mainTextColor;

        if (isSuccess)
        {
            if (continueBtnImage != null) continueBtnImage.color = Color.black;
            if (continueBtnText != null) continueBtnText.color = Color.white;
        }
        else
        {
            if (continueBtnImage != null) continueBtnImage.color = Color.white;
            if (continueBtnText != null) continueBtnText.color = Color.black;
        }
    }

    private IEnumerator FadeAndEnableButtonRoutine(bool isSuccess)
    {
        // 只有失败（死亡）时才执行 Alpha 的 Lerp 渐变
        if (!isSuccess)
        {
            float timer = 0;
            while (timer < fadeDuration)
            {
                timer += Time.unscaledDeltaTime; 
                if (canvasGroup != null)
                    canvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
                yield return null;
            }
        }
        
        // 确保最终 Alpha 是 1
        if (canvasGroup != null) canvasGroup.alpha = 1;

        // 处理按钮点击延迟
        // 成功撤离瞬间出现，所以直接等待 full buttonDelay
        // 死亡时由于经历了渐变，只需补足差值即可
        float waitTime = isSuccess ? buttonDelay : Mathf.Max(0, buttonDelay - fadeDuration);
        
        if (waitTime > 0)
        {
            yield return new WaitForSecondsRealtime(waitTime);
        }

        if (continueButton != null) continueButton.interactable = true;
    }

    private void UpdateRemainingItemsDisplay()
    {
        if (CollectionManager.Instance == null) return;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        int xi = CollectionManager.Instance.GetEmotionCount(EmotionTraitID.XI);
        int nu = CollectionManager.Instance.GetEmotionCount(EmotionTraitID.NU);
        int ai = CollectionManager.Instance.GetEmotionCount(EmotionTraitID.AI);
        int le = CollectionManager.Instance.GetEmotionCount(EmotionTraitID.LE);
        sb.Append($"喜 x {xi} | 怒 x {nu} | 哀 x {ai} | 乐 x {le}");
        remainingText.text = sb.ToString();
    }

    private void OnContinuePressed()
    {
        continueButton.interactable = false;
        
        // 彻底清理所有撤离视觉
        if (evacWhiteFilter != null) evacWhiteFilter.gameObject.SetActive(false);
        if (evacSubtitle != null) evacSubtitle.gameObject.SetActive(false);

        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}