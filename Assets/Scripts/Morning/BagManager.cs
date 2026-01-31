using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class BagManager : MonoBehaviour
{
    public static BagManager Instance { get; private set; }

    public int craftPigment;
    public int craftEmotion;

    private void Awake()
    {
        // 意思是：游戏一开始，把自己赋值给 Instance
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // 防止场景里不小心放了两个 BagManager，把多余的删掉
            Destroy(gameObject);
        }
        
    }


    private EmotionTraitID emotionTraitID;
    private MemoryTraitID memoryTraitID;
    

    //UI要素
    public TextMeshProUGUI pigmentText;     // 颜色数量
    public TextMeshProUGUI xiText;     // 情绪数量
    public TextMeshProUGUI nuText;     // 情绪数量
    public TextMeshProUGUI aiText;     // 情绪数量
    public TextMeshProUGUI leText;     // 情绪数量

    //memory对应的格子按钮，memory只要传递获得状态
    public Button buttonA;
    public Button buttonB;
    public Button buttonC;
    public Button buttonD;
    public Button buttonE;
    public Button buttonF;

    //memory对应的图标
    public Sprite imageLock;
    public Sprite imageA;
    public Sprite imageB;
    public Sprite imageC;
    public Sprite imageD;
    public Sprite imageE;
    public Sprite imageF;
    //改变合成按钮的图标
    public GameObject emotionSource;

    private void Start()
    {
        SaveData data = MorningGameManager.Instance.currentSaveData;
        List<MemoryTraitID> memoryGet = data.morningInventory.memoryGet;

        foreach (MemoryTraitID memory in memoryGet)
        {
            SetMemoryButton(memory);        
        }
    }

    #region 颜料、情绪、记忆消耗和更新
    /// <summary>
    /// 消耗颜料数并更新UI
    /// </summary>
    public bool EarnPigment(int amount)
    {
        // 1. 获取当前的存档数据引用
        SaveData data = MorningGameManager.Instance.currentSaveData;
        if (data.morningInventory.pigmentAmount + amount < 0)
        {
            if (OrderDetailPopup.Instance != null)
            {
                OrderDetailPopup.Instance.Show("资源不足!", "");
            }
            else
            {
                Debug.LogError("场景里找不到 OrderDetailPopup！请确保你把弹窗Prefab放进了场景里。");
            }
            Debug.Log("资源数量不够");
            return false;
        }
        data.morningInventory.pigmentAmount += amount;
        pigmentText.text = data.morningInventory.pigmentAmount.ToString();
        return true;
    }

    /// <summary>
    /// 查询颜料数
    /// </summary>
    public int Getpigment()
    {
        // 1. 获取当前的存档数据引用
        SaveData data = MorningGameManager.Instance.currentSaveData;
        return data.morningInventory.pigmentAmount;
    }

    #region 通过读取Memory获取情况控制memoryUI是否可用、显示
    ///<summary>
    ///刷新Memory按钮使用情况和对应UI
    /// </summary>
    public void SetMemoryButton(MemoryTraitID id)
    {
        switch (id)
        {
            case MemoryTraitID.A: buttonA.interactable =true; break;
            case MemoryTraitID.B: buttonB.interactable = true; break;
            case MemoryTraitID.C: buttonC.interactable = true; break;
            case MemoryTraitID.D: buttonD.interactable = true; break;
            case MemoryTraitID.E: buttonE.interactable = true; break;
            case MemoryTraitID.F: buttonF.interactable = true; break;
        }
        if (id == MemoryTraitID.A)
        {
            buttonA.GetComponent<Image>().sprite = imageA;
        }
        else if (id == MemoryTraitID.B)
        {
            buttonB.GetComponent<Image>().sprite = imageB;
        }
        else if (id == MemoryTraitID.C)
        {
            buttonC.GetComponent<Image>().sprite = imageC;
        }
        else if (id == MemoryTraitID.D)
        {
            buttonD.GetComponent<Image>().sprite = imageD;
        }
        else if (id == MemoryTraitID.E)
        {
            buttonE.GetComponent<Image>().sprite = imageE;
        }
        else if (id == MemoryTraitID.F)
        {
            buttonF.GetComponent<Image>().sprite = imageF;
        }
    }
    #endregion

    ///<summary>
    ///消耗情绪并更新UI
    /// </summary>
    public bool EarnEmotion(EmotionTraitID id, int amount)
    {
        // 1. 获取当前的存档数据引用
        SaveData data = MorningGameManager.Instance.currentSaveData;
        switch (id)
        {
            case EmotionTraitID.XI: 
                if(data.morningInventory.xiCount + amount < 0) {
                    if (OrderDetailPopup.Instance != null)
                    {
                        OrderDetailPopup.Instance.Show("资源不足!", "");
                    }
                    else
                    {
                        Debug.LogError("场景里找不到 OrderDetailPopup！请确保你把弹窗Prefab放进了场景里。");
                    }
                    Debug.Log("资源数量不够");
                    return false;
                }
                break;
            case EmotionTraitID.NU:
                if (data.morningInventory.nuCount + amount < 0)
                {
                    if (OrderDetailPopup.Instance != null)
                    {
                        OrderDetailPopup.Instance.Show("资源不足!", "");
                    }
                    else
                    {
                        Debug.LogError("场景里找不到 OrderDetailPopup！请确保你把弹窗Prefab放进了场景里。");
                    }
                    Debug.Log("资源数量不够");
                    return false;
                }
                break;
            case EmotionTraitID.AI:
                if (data.morningInventory.aiCount + amount < 0)
                {
                    if (OrderDetailPopup.Instance != null)
                    {
                        OrderDetailPopup.Instance.Show("资源不足!", "");
                    }
                    else
                    {
                        Debug.LogError("场景里找不到 OrderDetailPopup！请确保你把弹窗Prefab放进了场景里。");
                    }
                    Debug.Log("资源数量不够");
                    return false;
                }
                break;
            case EmotionTraitID.LE:
                if (data.morningInventory.leCount + amount < 0)
                {
                    if (OrderDetailPopup.Instance != null)
                    {
                        OrderDetailPopup.Instance.Show("资源不足!", "");
                    }
                    else
                    {
                        Debug.LogError("场景里找不到 OrderDetailPopup！请确保你把弹窗Prefab放进了场景里。");
                    }
                    Debug.Log("资源数量不够");
                    return false;
                }
                    
                break;
        }
        switch (id)
        {
            case EmotionTraitID.XI: data.morningInventory.xiCount += amount; xiText.text = data.morningInventory.xiCount.ToString(); break;
            case EmotionTraitID.NU: data.morningInventory.nuCount += amount; nuText.text = data.morningInventory.nuCount.ToString(); break;
            case EmotionTraitID.AI: data.morningInventory.aiCount += amount; aiText.text = data.morningInventory.aiCount.ToString(); break;
            case EmotionTraitID.LE: data.morningInventory.leCount += amount; leText.text = data.morningInventory.leCount.ToString(); break;
        }
        return true;
    }

    ///<summary>
    ///消耗情绪并更新UI
    /// </summary>
    public int GetEmotion(EmotionTraitID id)
    {
        // 1. 获取当前的存档数据引用
        SaveData data = MorningGameManager.Instance.currentSaveData;
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
    ///得到记忆=，战斗结算时调用，memoryGet新增，MemoryNight由Order提交更新
    /// </summary>
    public void EarnMemory(MemoryTraitID id)
    {
        // 1. 获取当前的存档数据引用
        SaveData data = MorningGameManager.Instance.currentSaveData;
        //data.morningInventory.memoryGet.Add(id);
        if (id==data.morningInventory.memoryNight)
        {

            // memory背包+1
            data.morningInventory.memoryGet.Add(id);

            Debug.Log($"获取记忆 {id} ，并从关卡移除了");

            SetMemoryButton(id);
        }
        else
        {
            Debug.LogWarning("操作失败：关卡中不存在这个记忆！");
        }
    }
    
    ///<summary>
    ///获取晚上关卡刷新的记忆
    /// </summary>
    public MemoryTraitID GetMemoryNight()
    {
        // 1. 获取当前的存档数据引用
        SaveData data = MorningGameManager.Instance.currentSaveData;
        return data.morningInventory.memoryNight;
    }
    #endregion

    #region 合成消耗资源（绑定到合成Button上）
    public bool CraftUse(EmotionTraitID id)
    {
        if (!EarnPigment(craftPigment))
            return false;
        if (!EarnEmotion(id, craftEmotion))
            return false;
        return true;
    }
    #endregion

    #region 更新资源数量UI
    public void RefreshBagUI()
    {
        // 1. 获取当前的存档数据引用
        SaveData data = MorningGameManager.Instance.currentSaveData;
        pigmentText.text = data.morningInventory.pigmentAmount.ToString();
        xiText.text = data.morningInventory.xiCount.ToString();
        nuText.text = data.morningInventory.nuCount.ToString(); 
        aiText.text = data.morningInventory.aiCount.ToString(); 
        leText.text = data.morningInventory.leCount.ToString();
        Debug.Log("已经更新资源UI");
    }
    #endregion
    [ContextMenu("增加 100 颜料")]
    public void CheatAddMoney()
    {
        // 1. 获取当前的存档数据引用
        SaveData data = MorningGameManager.Instance.currentSaveData;
        data.morningInventory.pigmentAmount += 100;
        Debug.Log("作弊成功：颜料 +100");
    }
}
