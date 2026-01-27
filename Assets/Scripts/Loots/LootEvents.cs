using System;

public static class LootEvents
{
    // 炸弹：无参数
    public static Action OnBombPicked;
    // 生命值：回血量
    public static Action<float> OnHealthPicked;
    // 情绪：具体类型
    public static Action<EmotionTraitID> OnEmotionPicked;
    // 记忆：具体类型
    public static Action<MemoryTraitID> OnMemoryPicked;
}