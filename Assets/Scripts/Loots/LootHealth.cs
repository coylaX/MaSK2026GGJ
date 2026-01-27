using UnityEngine;
// 生命值脚本
public class LootHealth : LootBase
{
    public float healAmount = 20f; // 可在Inspector修改

    protected override void OnPickedUp()
    {
        Debug.Log($"玩家拾取了生命值道具，回复了 {healAmount} 生命值");
        LootEvents.OnHealthPicked?.Invoke(healAmount);
    }
}