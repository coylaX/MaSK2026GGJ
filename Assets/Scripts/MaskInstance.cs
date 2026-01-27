using System;
using System.Collections.Generic;

[Serializable]
public class MaskInstance
{
    public string instanceId;        // 每个实例唯一
    public string displayName;
    public EmotionTraitID emotionTraitID;
    public MemoryTraitID memoryTraitID;
    public ColorTraitID colorTraitID;

    public MaskInstance(string instanceId, string displayName, EmotionTraitID emotionTraitID, MemoryTraitID memoryTraitID, ColorTraitID colorTraitID)
    {

        this.instanceId = instanceId;
        this.displayName = displayName;
        this.emotionTraitID = emotionTraitID;
        this.memoryTraitID = memoryTraitID;
        this.colorTraitID = colorTraitID;
    }
}
