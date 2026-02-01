using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class WarehouseItemUI : MonoBehaviour
{
    // Start is called before the first frame update
    public MaskInstance mask;
    
    
    public WarehouseUI warehouseUI;
    public BackPackView backPackUI;

    public string memoryText;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       // Debug.Log($"[Update] UIObj={gameObject.name} uiId={GetInstanceID()} mask={(mask == null ? "<null>" : mask.displayName)} ");

      //  Debug.Log(mask.displayName);
    }
    public void Onclick()
    {
        //刷新面具描述
        switch (mask.memoryTraitID)
        {
            case MemoryTraitID.A:
                memoryText = "损坏的录音";
                break; 
            case MemoryTraitID.B:
                memoryText = "夏日橘子汽水的味道";
                break;
            case MemoryTraitID.C:
                memoryText = "永恒涟漪";
                break;
            case MemoryTraitID.D:
                memoryText = "献祭的欢愉";
                break;
            case MemoryTraitID.E:
                memoryText = "断桥的栏杆铁锈";
                break;
        }
        LastClickMaskInfo.Instance.effectText.text = $"Last mask is:{mask.emotionTraitID.ToString().ToLower()} & {memoryText} & {mask.colorTraitID.ToString().ToLower()}";

        if (OrderManager.Instance.maskChooseState)
        {

        
            //  Debug.Log($"[Update] UIObj={gameObject.name} uiId={GetInstanceID()} mask={(mask == null ? "<null>" : mask.displayName)} ");
            Debug.Log(mask.displayName);
        if (mask.displayName == ""||mask==null)
        {
            //说明这个位置没有面具
            return;
        }

        //超出数量则不能再加
            if (BackPackLogic.I.maskInstances.Count >= backPackUI.slots.Length)
            {
                return;
            }
            MaskInventory.I.maskInstances.Remove(mask);//移出仓库
            BackPackLogic.I.maskInstances.Add(mask);//加入背包
            this.mask = null;
            //GetComponent<Image>().color = Color.white;
            warehouseUI.Refresh();
            backPackUI.Refresh();
        }
        else
        {
            OrderManager.Instance.OnMaskSelected(mask);
            OrderManager.Instance.selectedUI = this;
        }

        
    }
}
