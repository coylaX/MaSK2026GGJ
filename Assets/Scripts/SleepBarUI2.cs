using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SleepBarUI2 : MonoBehaviour
{
    [Header("Data")]
    public SleepHealth target;   // 玩家身上的 SleepHealth

    [Header("UI")]
    public Image fillImage;      // 血条填充图（Image Type = Filled）

    private void Awake()
    {
        if (fillImage == null)
            fillImage = GetComponent<Image>();
    }

    private void Update()
    {
        if (target == null || fillImage == null) return;

        float max = target.maxSleep;
        float cur = target.currentSleep;

        float normalized = (max <= 0f) ? 0f : Mathf.Clamp01(max / 100);
        fillImage.fillAmount = normalized;
    }
}
