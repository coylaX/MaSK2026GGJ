using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DeathSettlementUI : MonoBehaviour
{
    [Header("渐变配置")]
    public CanvasGroup canvasGroup; // 需要在面板上添加该组件
    public float fadeDuration = 1.0f;

    [Header("UI 联动")]
    public List<GameObject> combatUIs; // 在这里拖入战斗时显示的 HUD、小地图等

    [Header("文案配置")]
    [TextArea(1, 2)] public string titleContent = "迷失梦中";
    [TextArea(3, 5)] public string descriptionContent = "忘记了梦中的一些事物。那种感觉很熟悉，却记不清了...";

    [Header("UI 组件引用")]
    public GameObject settlementPanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI remainingText;
    public Image backgroundDim;
    public Button continueButton;

    private void OnEnable()
    {
        PlayerEvents.OnPlayerDeath += ShowSettlement;
    }

    private void OnDisable()
    {
        PlayerEvents.OnPlayerDeath -= ShowSettlement;
    }

    private void Start()
    {
        if (settlementPanel != null) settlementPanel.SetActive(false);
        if (canvasGroup != null) canvasGroup.alpha = 0;
        if (continueButton != null) continueButton.onClick.AddListener(OnContinuePressed);
    }

    private void ShowSettlement()
    {
        // 1. 立即隐藏战斗相关 UI
        foreach (GameObject ui in combatUIs)
        {
            if (ui != null) ui.SetActive(false);
        }

        // 2. 数据损失逻辑
        if (CollectionManager.Instance != null) CollectionManager.Instance.HandleDeathLoss();

        // 3. 更新文本
        if (titleText != null) titleText.text = titleContent;
        if (descriptionText != null) descriptionText.text = descriptionContent;
        UpdateRemainingItemsDisplay();

        // 4. 开启渐变协程
        settlementPanel.SetActive(true);
        StartCoroutine(FadeInRoutine());

        // 5. 暂停游戏
        Time.timeScale = 0;
    }

    private IEnumerator FadeInRoutine()
    {
        float timer = 0;
        while (timer < fadeDuration)
        {
            // 注意：因为 Time.timeScale 为 0，必须使用 unscaledDeltaTime
            timer += Time.unscaledDeltaTime;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            }
            yield return null;
        }
        if (canvasGroup != null) canvasGroup.alpha = 1;
    }

    // ... UpdateRemainingItemsDisplay 和 OnContinuePressed 保持不变 ...
    private void UpdateRemainingItemsDisplay()
    {
        if (CollectionManager.Instance == null) return;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        int xi = CollectionManager.Instance.GetEmotionCount(EmotionTraitID.XI);
        int nu = CollectionManager.Instance.GetEmotionCount(EmotionTraitID.NU);
        int ai = CollectionManager.Instance.GetEmotionCount(EmotionTraitID.AI);
        int le = CollectionManager.Instance.GetEmotionCount(EmotionTraitID.LE);
        sb.Append($"XI x {xi} | NU x {nu} | AI x {ai} | LE x {le}");
        remainingText.text = sb.ToString();
    }

    private void OnContinuePressed()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}