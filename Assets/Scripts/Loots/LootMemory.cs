using UnityEngine;
// 记忆脚本
public class LootMemory : LootBase
{
    protected override void OnPickedUp()
    {
        // 从BagManager中获取今日的记忆
        MemoryTraitID memory = BagManager.Instance.GetMemoryNight();
        
        Debug.Log($"玩家获取了一个记忆: {memory}");
        LootEvents.OnMemoryPicked?.Invoke(memory);
    }
}
