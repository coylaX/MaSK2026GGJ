using UnityEngine;
using TMPro;
using System.Collections;

public class SaveNotificationUI : MonoBehaviour
{
    public static SaveNotificationUI Instance;

    [Header("UI 引用")]
    public TextMeshProUGUI notificationText;

    [Header("配置")]
    public float displayTime = 5f; // 显示时长

    private Coroutine hideCoroutine;

    private void Awake()
    {
        // 简单的单例模式，方便全局调用
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 确保游戏开始时提示是隐藏的
        if (notificationText != null)
        {
            notificationText.gameObject.SetActive(false);
        }
    }

    // 外部调用这个方法来触发提示
    public void ShowSaveSuccess()
    {
        if (notificationText == null) return;

        notificationText.text = "已成功存档...";
        notificationText.gameObject.SetActive(true);

        // 如果当前已经有一个倒计时在跑（比如玩家连续按了两次存档），先停掉它，重新计时
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        // 开启新的 5 秒倒计时
        hideCoroutine = StartCoroutine(HideRoutine());
    }

    private IEnumerator HideRoutine()
    {
        // 使用 Realtime 是为了防止存档时游戏暂停 (Time.timeScale = 0) 导致协程卡死
        yield return new WaitForSecondsRealtime(displayTime);

        // 5 秒后隐藏 UI
        if (notificationText != null)
        {
            notificationText.gameObject.SetActive(false);
        }

        hideCoroutine = null;
    }
}