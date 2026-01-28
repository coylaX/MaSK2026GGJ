using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackItemUI : MonoBehaviour
{
    // Start is called before the first frame update
    public MaskInstance mask;
    
    
    public WarehouseUI warehouseUI;
    public BackPackView backPackUI;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Onclick()
    {
        
        if (mask.displayName == ""||mask==null)
        {
            //说明这个位置没有面具
            return;
        }
        if (MaskInventory.I.maskInstances.Count >= warehouseUI.slots.Length)
        {
            return;
        }
        MaskInventory.I.maskInstances.Add(mask);//移进仓库
        BackPackLogic.I.maskInstances.Remove(mask);//移出背包
        mask = null;
        GetComponent<Image>().color = Color.white;
        warehouseUI.Refresh();
        backPackUI.Refresh();
    }
}
