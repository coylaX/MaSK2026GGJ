using UnityEngine;
// 记忆脚本
public class LootMemory : LootBase
{
    protected override void OnPickedUp()
    {
        // 目前先硬编码为 A，未来可以通过方法动态设置
        MemoryTraitID memory = MemoryTraitID.A;
        
        Debug.Log($"玩家获取了一个记忆: {memory}");
        LootEvents.OnMemoryPicked?.Invoke(memory);
    }
}
