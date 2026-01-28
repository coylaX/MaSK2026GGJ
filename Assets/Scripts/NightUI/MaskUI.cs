using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaskUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Refresh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public GameObject[] slots;//显示槽位
    [Header("Placeholder")]
    public Sprite XI;     
    public Sprite NU;
    public Sprite AI;
    public Sprite LE;
    //撕毁面具时调用
    public void Refresh()
    {
        
        int slotCount = slots.Length;
        int maskCount = BackPackLogic.I.maskInstances.Count;
        Debug.Log(slotCount);
        Debug.Log(maskCount);
        if (slotCount > maskCount)
        {
            //mask如果被撕毁了，slot的末位就要消失，对应序号刚好是maskcount
            
            slots[maskCount].SetActive(false);
        }

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
            
        }


    }
}
