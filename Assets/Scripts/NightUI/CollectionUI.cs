using UnityEngine;
using TMPro;
using System.Text;
using System.Collections.Generic;

public class CollectionUI : MonoBehaviour
{
    [Header("UI Reference")]
    public TextMeshProUGUI displayLinesText;

    private void OnEnable()
    {
        // 【核心修改】：订阅 Manager 的数据更新信号，而非 LootEvents
        CollectionManager.OnCollectionUpdated += RefreshDisplay;
    }

    private void OnDisable()
    {
        // 取消订阅
        CollectionManager.OnCollectionUpdated -= RefreshDisplay;
    }

    private void Start()
    {
        RefreshDisplay();
    }

    // 现在这个方法不再需要传入 ID 参数，逻辑更纯粹
    private void RefreshDisplay()
    {
        if (displayLinesText == null || CollectionManager.Instance == null) return;

        StringBuilder sb = new StringBuilder();

        // 1. 拼接情绪数据
        int xi = CollectionManager.Instance.GetEmotionCount(EmotionTraitID.XI);
        int nu = CollectionManager.Instance.GetEmotionCount(EmotionTraitID.NU);
        int ai = CollectionManager.Instance.GetEmotionCount(EmotionTraitID.AI);
        int le = CollectionManager.Instance.GetEmotionCount(EmotionTraitID.LE);

        sb.Append($"喜 x {xi} | 怒 x {nu} | 哀 x {ai} | 乐 x {le}");

        // 2. 拼接记忆数据
        List<string> collectedMemoryNames = new List<string>();
        foreach (MemoryTraitID m in System.Enum.GetValues(typeof(MemoryTraitID)))
        {
            if (CollectionManager.Instance.HasMemory(m))
            {
                collectedMemoryNames.Add(m.ToString());
            }
        }

        if (collectedMemoryNames.Count > 0)
        {
            string memories = string.Join(", ", collectedMemoryNames);
            sb.Append($" | Memory: {memories}");
        }

        // 3. 更新文字
        displayLinesText.text = sb.ToString();
    }
}