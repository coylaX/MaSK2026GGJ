using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarehouseItemUI : MonoBehaviour
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
       // Debug.Log($"[Update] UIObj={gameObject.name} uiId={GetInstanceID()} mask={(mask == null ? "<null>" : mask.displayName)} ");

      //  Debug.Log(mask.displayName);
    }
    public void Onclick()
    {
      //  Debug.Log($"[Update] UIObj={gameObject.name} uiId={GetInstanceID()} mask={(mask == null ? "<null>" : mask.displayName)} ");
        Debug.Log(mask.displayName);
        if (mask.displayName == ""||mask==null)
        {
            //说明这个位置没有面具
            return;
        }
        MaskInventory.I.maskInstances.Remove(mask);//移出仓库
        BackPackLogic.I.maskInstances.Add(mask);//加入背包
        this.mask = null;
        warehouseUI.Refresh();
        backPackUI.Refresh();
        
    }
}
