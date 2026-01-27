using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WarehouseUI : MonoBehaviour
{
    [Header("Logic data")]
    public MaskInventory maskInventoryLogic;
    List<MaskInstance> warehouse; // 你的逻辑仓库（也可以改成 PlayerInventory.I.warehouse）

    [Header("UI slots (pre-placed in scene, in order)")]
    public GameObject[] slots;   // 在Inspector按顺序拖入：Slot0, Slot1, Slot2...

    [Header("Placeholder")]
    public Sprite placeholderSprite;     // 没美术就留空，或者用一个默认图

   

    public void Refresh()
    {
        warehouse = maskInventoryLogic.maskInstances;
        int slotCount = slots.Length;
        int maskCount = warehouse?.Count ?? 0;

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
            MaskInstance mask = warehouse[i];
            slots[i].GetComponent<Image>().sprite = placeholderSprite;//目前只有一张图
            slots[i].GetComponent<WarehouseItemUI>().mask = mask;
        }

        // 3) 如果仓库比slot多，你可以先debug提示（后面做分页/滚动）
       
    }
}



