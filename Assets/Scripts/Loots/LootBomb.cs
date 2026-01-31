using UnityEngine;
// 炸弹脚本
public class LootBomb : LootBase
{
    protected override void OnPickedUp()
    {
        if(AudioManager.Instance != null)
            AudioManager.Instance.OnBookPickUp();
        Debug.Log("玩家拾取了炸弹");
        LootEvents.OnBombPicked?.Invoke();
    }
}