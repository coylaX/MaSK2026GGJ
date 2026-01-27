using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BagManager : MonoBehaviour
{
    public static BagManager Instance;

    private EmotionTraitID emotionTraitID;
    public MemoryTraitID memoryTraitID;
    SaveData data = MorningGameManager.Instance.currentSaveData;

    //UI要素
    public TextMeshProUGUI pigmentText;     // 颜色数量
    public TextMeshProUGUI xiText;     // 情绪数量
    public TextMeshProUGUI nuText;     // 情绪数量
    public TextMeshProUGUI aiText;     // 情绪数量
    public TextMeshProUGUI leText;     // 情绪数量

    //memory对应的格子按钮
    public Button buttonA;
    public Button buttonB;
    public Button buttonC;
    public Button buttonD;
    public Button buttonE;

    #region 颜料、情绪、记忆消耗和更新
    /// <summary>
    /// 消耗颜料数并更新UI
    /// </summary>
    public void usePigment(int amount)
    {
        data.morningInventory.pigmentAmount += amount;
        pigmentText.text = data.morningInventory.pigmentAmount.ToString();
    }

    /// <summary>
    /// 查询颜料数
    /// </summary>
    public int getpigment()
    {
        return data.morningInventory.pigmentAmount;
    }

    ///<summary>
    ///消耗情绪并更新UI
    /// </summary>
    public void useEmotion(EmotionTraitID id, int amount)
    {
        switch (id)
        {
            case EmotionTraitID.XI: data.morningInventory.xiCount += amount; xiText.text = data.morningInventory.xiCount.ToString(); break;
            case EmotionTraitID.NU: data.morningInventory.nuCount += amount; nuText.text = data.morningInventory.nuCount.ToString(); break;
            case EmotionTraitID.AI: data.morningInventory.aiCount += amount; aiText.text = data.morningInventory.aiCount.ToString(); break;
            case EmotionTraitID.LE: data.morningInventory.leCount += amount; leText.text = data.morningInventory.leCount.ToString(); break;
        }
    }

    ///<summary>
    ///消耗情绪并更新UI
    /// </summary>
    public int GetEmotion(EmotionTraitID id)
    {
        switch (id)
        {
            case EmotionTraitID.XI: return data.morningInventory.xiCount;
            case EmotionTraitID.NU: return data.morningInventory.nuCount;
            case EmotionTraitID.AI: return data.morningInventory.aiCount;
            case EmotionTraitID.LE: return data.morningInventory.leCount;
            default: return 0;
        }
    }

    ///<summary>
    ///推进记忆进度
    /// </summary>
    public void earnMemory(MemoryTraitID id)
    {
        data.morningInventory.memoryGet.Add(id);
        //TODO:更新UI
        
    }

    ///<summary>
    ///查询记忆进度?
    /// </summary>

    #endregion
    #region 合成消耗资源（绑定到合成Button上）
    public void craftUse(EmotionTraitID id)
    {
        usePigment(10);
        useEmotion(id,1);
    }
    #endregion
}
