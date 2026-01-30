using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackPackView : MonoBehaviour
{
    [Header("Logic data")]
    public BackPackLogic backPackLogic;
    

    [Header("UI slots (pre-placed in scene, in order)")]
    public GameObject[] slots;   // 在Inspector按顺序拖入：Slot0, Slot1, Slot2...

    [Header("Placeholder")]
    public Sprite XI;     // 没美术就留空，或者用一个默认图
    public Sprite NU;
    public Sprite AI;
    public Sprite LE;



    public void Refresh()
    {
        
        int slotCount = slots.Length;
        int maskCount = BackPackLogic.I.maskInstances.Count;

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
        for (int i = 0; i < BackPackLogic.I.maskInstances.Count; i++)
        {
            MaskInstance mask = BackPackLogic.I.maskInstances[i];
            switch (mask.emotionTraitID)
            {
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
            slots[i].GetComponent<BackItemUI>().mask = mask;
        }
        for (int i = BackPackLogic.I.maskInstances.Count; i < slotCount; i++)
        {
            slots[i].GetComponent<BackItemUI>().mask = null;
            slots[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        }

    }
}
