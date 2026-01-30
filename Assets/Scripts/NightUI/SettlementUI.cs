using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq; 

public class SettlementUI : MonoBehaviour
{
    public static SettlementUI Instance;

    [Header("渐变与延迟配置")]
    public CanvasGroup canvasGroup; 
    public float fadeDuration = 1.0f;     
    public float buttonDelay = 1.5f;      

    [Header("UI 联动 (仅用于结算显示时隐藏)")]
    public List<GameObject> combatUIs; 

    [Header("重置 - UI 显隐配置")]
    [Tooltip("该根节点的所有深层子物体都将被设为 Active (不影响根节点本身)")]
    public GameObject uiRootToActivate;    
    
    [Tooltip("该根节点的所有深层子物体都将被设为 Inactive (不影响根节点本身)")]
    public GameObject uiRootToDeactivate;  
    
    [Tooltip("列表中的这些特定 GameObject 将被单独设为 Inactive")]
    public List<GameObject> specificUIsToInActivate;

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
        currentSettlementIsSuccess = isSuccess; 

        // 1. 隐藏战斗 UI 和撤离小字
        foreach (GameObject ui in combatUIs) if (ui != null) ui.SetActive(false);
        if (evacSubtitle != null) evacSubtitle.gameObject.SetActive(false);

        // 2. 只有在失败时执行损失逻辑
        if (!isSuccess)
        {
            if (CollectionManager.Instance != null) CollectionManager.Instance.HandleDeathLoss();
            if (evacWhiteFilter != null) evacWhiteFilter.gameObject.SetActive(false);
        }

        // 3. 应用样式与文案
        ApplySettlementStyle(isSuccess);

        // 4. 显示面板并处理 Alpha 逻辑
        if (settlementPanel != null) settlementPanel.SetActive(true);
        
        if (isSuccess && canvasGroup != null)
        {
            canvasGroup.alpha = 1;
        }
        else if (canvasGroup != null)
        {
            canvasGroup.alpha = 0;
        }
        
        UpdateRemainingItemsDisplay();

        // 5. 开启计时逻辑
        StopAllCoroutines(); 
        StartCoroutine(FadeAndEnableButtonRoutine(isSuccess));

        Time.timeScale = 0;
    }

    private void ApplySettlementStyle(bool isSuccess)
    {
        titleText.text = isSuccess ? successTitle : failTitle;
        descriptionText.text = isSuccess ? successDesc : failDesc;

        if (backgroundDim != null) 
        {
            Color bgColor = isSuccess ? Color.white : Color.black;
            bgColor.a = 1f; 
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
        
        if (canvasGroup != null) canvasGroup.alpha = 1;

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
        MorningGameManager.Instance.AdvanceDay();

        continueButton.interactable = false;

        // 【修改点】：直接调用 CollectionManager 的结算方法
        if (CollectionManager.Instance != null)
        {
            // 这一个方法包含了：广播报告 -> 写入库存 -> 清空临时背包
            CollectionManager.Instance.FinalizeNightCollection(currentSettlementIsSuccess);
        }

        // 执行场景软重置 (删除怪物、房间等)
        PerformSoftReset();

        Time.timeScale = 1;
        Debug.Log("<color=green>[系统]</color> 场景软重置完成。");
    }

    private void PerformSoftReset()
    {
        // --- 任务 1: 删除掉落物 ---
        if (LevelManager.Instance != null && LevelManager.Instance.lootContainer != null)
        {
            foreach (Transform child in LevelManager.Instance.lootContainer)
            {
                Destroy(child.gameObject);
            }
        }

        // --- 任务 2: 删除关卡房间和怪物 ---
        if (LevelManager.Instance != null)
        {
            foreach (Transform child in LevelManager.Instance.transform)
            {
                if (child != LevelManager.Instance.lootContainer)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        // --- 【关键修改】：彻底清空小地图旧数据，准备迎接第二天的新房间 ---
        if (MiniMapManager.Instance != null) {
            MiniMapManager.Instance.ResetMap();
        }

        // --- 任务 3: UI 显隐状态重置 ---
        
        SetRecursiveActive(uiRootToActivate, true);
        SetRecursiveActive(uiRootToDeactivate, false);

        foreach (GameObject ui in specificUIsToInActivate)
        {
            if (ui != null) ui.SetActive(false);
        }

        if (settlementPanel != null) settlementPanel.SetActive(false);

        // --- 任务 4: 玩家与摄像机复位 ---
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.transform.position = Vector3.zero;
        }

        if (Camera.main != null)
        {
            Camera.main.transform.position = new Vector3(0, 0, -10f);
            if (CameraManager.Instance != null)
            {
                CameraManager.Instance.SwitchToRoom(Vector3.zero);
            }
        }

        if (evacWhiteFilter != null) evacWhiteFilter.gameObject.SetActive(false);
        if (evacSubtitle != null) evacSubtitle.gameObject.SetActive(false);
    }

    private void SetRecursiveActive(GameObject root, bool active)
    {
        if (root == null) return;
        Transform[] allTransforms = root.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in allTransforms)
        {
            if (t.gameObject == root) continue;
            t.gameObject.SetActive(active);
        }
    }
}