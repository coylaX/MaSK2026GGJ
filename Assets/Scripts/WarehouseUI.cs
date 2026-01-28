using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WarehouseUI : MonoBehaviour
{
    [Header("Logic data")]
    
     // 你的逻辑仓库（也可以改成 PlayerInventory.I.warehouse）

    [Header("UI slots (pre-placed in scene, in order)")]
    public GameObject[] slots;   // 在Inspector按顺序拖入：Slot0, Slot1, Slot2...

    [Header("Placeholder")]
    public Sprite XI;     // 没美术就留空，或者用一个默认图
    public Sprite NU;
    public Sprite AI;
    public Sprite LE;

<<<<<<< Updated upstream
=======
    private void OnEnable()
    {
        // 开始监听：只要 SaveManager 喊 "OnLoadComplete"，我就执行 Refresh
        SaveManager.OnLoadComplete += Refresh;
    }

    private void OnDisable()
    {
        // 记得取消监听，防止报错
        SaveManager.OnLoadComplete -= Refresh;
    }
>>>>>>> Stashed changes

    public void Refresh()
    {
        
        int slotCount = slots.Length;
        int maskCount = MaskInventory.I.maskInstances.Count;

        // 1) 先把所有slot清空
        for (int i = 0; i < slotCount; i++)
        {
            slots[i].GetComponent<Image>().sprite = null;
        }

        if (maskCount > slotCount)
        {
            Debug.Log($"[WarehouseUI] Overflow: {maskCount} masks, but only {slotCount} slots shown.");
            return;
        }
        // 2) 再把仓库里的面具按顺序填进去（最多填满slots）
        int showCount = Mathf.Min(maskCount, slotCount);
        for (int i = 0; i < showCount; i++)
        {
            MaskInstance mask = MaskInventory.I.maskInstances[i];
            switch (mask.emotionTraitID)
            {
                case EmotionTraitID.None:
                    break;
                case EmotionTraitID.XI:
                    slots[i].GetComponent<Image>().sprite = XI;
                    break;
                case EmotionTraitID.NU:
                    slots[i].GetComponent<Image>().sprite = NU;
                    break;
                case EmotionTraitID.AI:
                    slots[i].GetComponent<Image>().sprite = AI;
                    break;
                case EmotionTraitID.LE:
                    slots[i].GetComponent<Image>().sprite = LE;
                    break;
            }
            switch (mask.colorTraitID)
            {
                case ColorTraitID.None:
                    slots[i].GetComponent<Image>().color = Color.white;
                    break;
                case ColorTraitID.RED:
                    slots[i].GetComponent<Image>().color = Color.red;
                    break;
                case ColorTraitID.YELLOW:
                    slots[i].GetComponent<Image>().color = Color.yellow;
                    break;
                case ColorTraitID.BLUE:
                    slots[i].GetComponent<Image>().color = Color.blue;
                    break;
                case ColorTraitID.GREEN:
                    slots[i].GetComponent<Image>().color = Color.green;
                    break;
                case ColorTraitID.BLACK:
                    slots[i].GetComponent<Image>().color = Color.gray;
                    break;
                case ColorTraitID.WHITE:
                    slots[i].GetComponent<Image>().color = Color.white;
                    break;
            }
            slots[i].GetComponent<WarehouseItemUI>().mask = mask;
        }

        // 3) 如果仓库比slot多，你可以先debug提示（后面做分页/滚动）

    }
}



