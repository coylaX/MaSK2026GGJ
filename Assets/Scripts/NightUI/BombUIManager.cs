using UnityEngine;
using TMPro; // 必须引用 TMP 命名空间

public class BombUIManager : MonoBehaviour
{
    [Header("UI 引用")]
    public TextMeshProUGUI bombText; // 拖入你的 Bomb-TMP

    private void OnEnable()
    {
        // 订阅炸弹数量改变事件
        UseBomb.OnBombCountChanged += UpdateBombDisplay;
    }

    private void OnDisable()
    {
        // 取消订阅防止内存泄露
        UseBomb.OnBombCountChanged -= UpdateBombDisplay;
    }

    /// <summary>
    /// 接收事件并更新 UI 显示
    /// </summary>
    /// <param name="currentCount">当前的炸弹数量</param>
    private void UpdateBombDisplay(int currentCount)
    {
        if (bombText != null)
        {
            // 格式化显示为：Bomb * n
            bombText.text = $"Bomb * {currentCount}";
        }
    }
}