using UnityEngine;
using TMPro; // Required for TextMeshProUGUI
using System.Text;
using System.Collections.Generic;

public class CollectionUI : MonoBehaviour
{
    [Header("UI Reference")]
    public TextMeshProUGUI displayLinesText; // Drag your TMP - Text object here

    private void OnEnable()
    {
        // Subscribe to events
        LootEvents.OnEmotionPicked += RefreshDisplay;
        LootEvents.OnMemoryPicked += RefreshDisplay;
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        LootEvents.OnEmotionPicked -= RefreshDisplay;
        LootEvents.OnMemoryPicked -= RefreshDisplay;
    }

    private void Start()
    {
        // Initial refresh to show 0s
        RefreshDisplay();
    }

    // Main refresh logic
    private void RefreshDisplay()
    {
        if (displayLinesText == null || CollectionManager.Instance == null) return;

        StringBuilder sb = new StringBuilder();

        // 1. Emotions: XI x n | NU x n | AI x n | LE x n
        int xi = CollectionManager.Instance.GetEmotionCount(EmotionTraitID.XI);
        int nu = CollectionManager.Instance.GetEmotionCount(EmotionTraitID.NU);
        int ai = CollectionManager.Instance.GetEmotionCount(EmotionTraitID.AI);
        int le = CollectionManager.Instance.GetEmotionCount(EmotionTraitID.LE);

        sb.Append($"XI x {xi} | NU x {nu} | AI x {ai} | LE x {le}");

        // 2. Memory: Only shows if at least one is collected
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

        // 3. Update TMP Text
        displayLinesText.text = sb.ToString();
    }

    // Glue methods to match the Action<T> signature
    private void RefreshDisplay(EmotionTraitID id) => RefreshDisplay();
    private void RefreshDisplay(MemoryTraitID id) => RefreshDisplay();
}