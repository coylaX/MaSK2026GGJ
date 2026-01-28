using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EvacuationPoint : MonoBehaviour
{
    [Header("撤离配置")]
    public float requiredTime = 3.0f;
    public string promptMessage = "正在离开梦境...";

    [Header("动画配置")]
    private Animator anim;
    // 动画参数名，需与 Animator 窗口中一致
    private string animParam = "EvacProgress"; 

    private float timer = 0f;
    private bool isPlayerStanding = false;

    // 快捷访问单例 UI
    private Image WhiteFilter => SettlementUI.Instance != null ? SettlementUI.Instance.evacWhiteFilter : null;
    private TextMeshProUGUI Subtitle => SettlementUI.Instance != null ? SettlementUI.Instance.evacSubtitle : null;

    private void Awake()
    {
        // 尝试从自身或子物体获取 Animator
        anim = GetComponentInChildren<Animator>();
    }

    private void Start() 
    { 
        ResetEvacuation(); 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerStanding = true;
            if (WhiteFilter != null) {
                WhiteFilter.gameObject.SetActive(true);
                WhiteFilter.color = new Color(1, 1, 1, 0);
            }
            if (Subtitle != null) {
                Subtitle.gameObject.SetActive(true);
                Subtitle.text = promptMessage;
                SetTextAlpha(0);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ResetEvacuation();
        }
    }

    private void Update()
    {
        if (isPlayerStanding)
        {
            timer += Time.deltaTime;
            
            // 计算进度 (0.0 到 1.0)
            float progress = Mathf.Clamp01(timer / requiredTime);
            
            // --- 新增：同步动画进度 ---
            // 因为你希望 3秒内走到第 6 帧（总共 7 帧），
            // 归一化时间 0.0=第1帧，0.83=第6帧，1.0=第7帧。
            // 如果你想精确停在第 6 帧，可以将 progress 乘以 0.83f
            if (anim != null)
            {
                anim.SetFloat(animParam, progress * 0.83f); 
            }

            // 同步滤镜和文字
            if (WhiteFilter != null) WhiteFilter.color = new Color(1, 1, 1, progress * 0.8f);
            if (Subtitle != null) SetTextAlpha(progress);

            if (timer >= requiredTime)
            {
                CompleteEvacuation();
            }
        }
    }

    private void ResetEvacuation()
    {
        isPlayerStanding = false;
        timer = 0f;

        // --- 新增：动画重置回第一帧 ---
        if (anim != null)
        {
            anim.SetFloat(animParam, 0f);
        }

        if (Subtitle != null) {
            SetTextAlpha(0);
            Subtitle.gameObject.SetActive(false);
        }
        if (WhiteFilter != null) {
            WhiteFilter.color = new Color(1, 1, 1, 0);
            WhiteFilter.gameObject.SetActive(false);
        }
    }

    private void SetTextAlpha(float alpha)
    {
        if (Subtitle == null) return;
        Color c = Subtitle.color;
        c.a = alpha;
        Subtitle.color = c;
    }

    private void CompleteEvacuation()
    {
        isPlayerStanding = false;
        if (Subtitle != null) Subtitle.gameObject.SetActive(false);
        
        // 成功时让动画卡在最后一刻（第 6 帧）
        if (anim != null) anim.SetFloat(animParam, 0.83f);

        if (SettlementUI.Instance != null)
        {
            SettlementUI.Instance.ShowSettlement(true);
        }
    }
}