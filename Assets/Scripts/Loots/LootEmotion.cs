using UnityEngine;
// 情绪脚本
public class LootEmotion : LootBase
{
    protected override void OnPickedUp()
    {
        if(AudioManager.Instance != null)
            AudioManager.Instance.OnWaterPickUp();

        // 随机获取枚举中的一个值
        EmotionTraitID randomEmotion = (EmotionTraitID)Random.Range(0, System.Enum.GetValues(typeof(EmotionTraitID)).Length);
        
        Debug.Log($"玩家获取了一个情绪: {randomEmotion}");
        LootEvents.OnEmotionPicked?.Invoke(randomEmotion);
    }
}