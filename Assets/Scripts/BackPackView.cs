using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackPackView : MonoBehaviour
{
    [Header("Logic data")]
    public BackPackLogic backPackLogic;
    List<MaskInstance> backpackMaskInstances; // 你的逻辑仓库（也可以改成 PlayerInventory.I.warehouse）

    [Header("UI slots (pre-placed in scene, in order)")]
    public GameObject[] slots;   // 在Inspector按顺序拖入：Slot0, Slot1, Slot2...

    [Header("Placeholder")]
    public Sprite placeholderSprite;     // 没美术就留空，或者用一个默认图



    public void Refresh()
    {
        backpackMaskInstances = backPackLogic.maskInstances;
        int slotCount = slots.Length;
        int maskCount = backpackMaskInstances.Count;

        // 1) 先把所有slot清空
        for (int i = 0; i < slotCount; i++)
        {
            slots[i].GetComponent<Image>().sprite = null;
        }

        // 2) 再把仓库里的面具按顺序填进去（最多填满slots）
        int showCount = Mathf.Min(maskCount, slotCount);
        Debug.Log(showCount);
        if (maskCount > slotCount)
        {
            Debug.Log($"[WarehouseUI] Overflow: {maskCount} masks, but only {slotCount} slots shown.");
            return;
        }
        for (int i = 0; i < showCount; i++)
        {
            MaskInstance mask = backpackMaskInstances[i];
            slots[i].GetComponent<Image>().sprite = placeholderSprite;//目前只有一张图
            slots[i].GetComponent<BackItemUI>().mask = mask;
        }

       
    }
}
