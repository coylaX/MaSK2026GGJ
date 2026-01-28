using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EvacuationPoint : MonoBehaviour
{
    [Header("撤离配置")]
    public float requiredTime = 3.0f;
    public string promptMessage = "正在离开梦境...";

    private float timer = 0f;
    private bool isPlayerStanding = false;

    private Image WhiteFilter => SettlementUI.Instance != null ? SettlementUI.Instance.evacWhiteFilter : null;
    private TextMeshProUGUI Subtitle => SettlementUI.Instance != null ? SettlementUI.Instance.evacSubtitle : null;

    private void Start() { ResetEvacuation(); }

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
            // 只有在中途离开时才重置。如果已经撤离成功，逻辑交给结算系统
            ResetEvacuation(); 
        }
    }

    private void Update()
    {
        if (isPlayerStanding)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / requiredTime);
            
            if (WhiteFilter != null) WhiteFilter.color = new Color(1, 1, 1, progress * 0.8f);
            if (Subtitle != null) SetTextAlpha(progress);

            if (timer >= requiredTime)
            {
                CompleteEvacuation();
            }
        }
    }

    // 该函数仅用于“中断”或“初始化”时清空视觉效果
    private void ResetEvacuation()
    {
        isPlayerStanding = false;
        timer = 0f;
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
        // 关键修改：停止计时，但不要调用 ResetEvacuation，保持白光不消失
        isPlayerStanding = false; 
        
        // 字幕可以先关掉，因为结算面板会有自己的大标题
        if (Subtitle != null) Subtitle.gameObject.SetActive(false);
        
        if (SettlementUI.Instance != null)
        {
            SettlementUI.Instance.ShowSettlement(true);
        }
    }
}