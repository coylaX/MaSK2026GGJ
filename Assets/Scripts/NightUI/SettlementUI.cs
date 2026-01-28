using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq; // <--- 补充这一行

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

    // 在 SettlementUI.cs 中增加一个变量记录当前结算状态
    private bool currentSettlementIsSuccess;

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
        currentSettlementIsSuccess = isSuccess; // 记录状态

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

    // 在 SettlementUI 类中修改或添加
    private void OnContinuePressed()
    {
        continueButton.interactable = false;

        // 1. 发送结算广播 (存档/成就系统)
        if (CollectionManager.Instance != null)
        {
            var report = CollectionManager.Instance.GetCurrentReport(currentSettlementIsSuccess);
            CollectionManager.OnNightEndSummary?.Invoke(report);
        }

        // 执行自定义重置功能
        PerformSoftReset();

        // 恢复时间流速
        Time.timeScale = 1;
        Debug.Log("<color=green>[系统]</color> 场景软重置完成，已回到起始状态。");
    }

    private void PerformSoftReset()
    {
        // --- 任务 1: 删除 LootContainer 下的所有子物体 ---
        if (LevelManager.Instance != null && LevelManager.Instance.lootContainer != null)
        {
            foreach (Transform child in LevelManager.Instance.lootContainer)
            {
                Destroy(child.gameObject);
            }
        }

        // --- 任务 2: 删除 LevelManager 下的所有子物体 (即删除关卡房间和怪物) ---
        if (LevelManager.Instance != null)
        {
            foreach (Transform child in LevelManager.Instance.transform)
            {
                // 注意：不要把 LootContainer 自己删了，如果它是 LevelManager 的子物体的话
                if (child != LevelManager.Instance.lootContainer)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        // --- 任务 3: 把 Canvas 下的所有子物体及其深层子物体都设为 Active ---
        // 假设你的脚本挂在 Canvas 的某个物体上，或者直接找场景中的 Canvas
        Canvas mainCanvas = GetComponentInParent<Canvas>();
        if (mainCanvas != null)
        {
            // GetComponentsInChildren(true) 会包含隐藏的物体
            Transform[] allUITransforms = mainCanvas.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in allUITransforms)
            {
                // 排除 Canvas 自身，只激活子物体
                if (t != mainCanvas.transform)
                {
                    t.gameObject.SetActive(true);
                }
            }
            
            // 额外提醒：结算面板 settlementPanel 应该在激活后立即手动隐藏，否则第二天早上会挡住屏幕
            if (settlementPanel != null) settlementPanel.SetActive(false);
        }

        // --- 任务 4: 把 MainPlayer 和 MainCamera 重新移回 (0,0) ---
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.transform.position = Vector3.zero;
        }

        if (Camera.main != null)
        {
            // 2D 场景中摄像机的 Z 轴通常保持为 -10
            Camera.main.transform.position = new Vector3(0, 0, -10f);
            
            // 如果你有 CameraManager，建议调用它的复位方法
            if (CameraManager.Instance != null)
            {
                CameraManager.Instance.SwitchToRoom(Vector3.zero);
            }
        }

        // 特别清理：隐藏撤离过程中的白光和字幕
        if (evacWhiteFilter != null) evacWhiteFilter.gameObject.SetActive(false);
        if (evacSubtitle != null) evacSubtitle.gameObject.SetActive(false);
        if (settlementPanel != null) settlementPanel.gameObject.SetActive(false);
    }
}