using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
       MaskInventory.I.maskInstances.Add(mask);//移出仓库
        BackPackLogic.I.maskInstances.Remove(mask);//加入背包
        mask = null;
        warehouseUI.Refresh();
        backPackUI.Refresh();
    }
}
