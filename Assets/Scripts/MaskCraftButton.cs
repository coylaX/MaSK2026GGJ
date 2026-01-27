using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class MaskCraftButton : MonoBehaviour
{
    [Header("UI")]
    public Button craftButton;              

    [Header("Templates")]
    public EmotionTraitID emotionTrait;    // 测试时配词条进来，实际游玩过程需要再加代码判断是什么词条
    public MemoryTraitID memoryTrait;
    public ColorTraitID colorTrait;
    [Header("三种材料")]
    public GameObject emotion;
    public GameObject color;
    public GameObject memory;
    [Header("逻辑仓库")]
    public MaskInventory maskInventoryLogic;
    List<MaskInstance> warehouse; // 逻辑仓库（也可以改成 PlayerInventory.I.warehouse）

    [Header("显示仓库")]
    public WarehouseUI warehouseUI;

    private void Awake()
    {
        if (craftButton != null)
            craftButton.onClick.AddListener(OnCraftClicked);
    }

    private void OnDestroy()
    {
        if (craftButton != null)
            craftButton.onClick.RemoveListener(OnCraftClicked);
    }

    private void OnCraftClicked()
    {
        emotionTrait = emotion.GetComponent<EmotionSource>().emotionTraitID;
        memoryTrait = memory.GetComponent<MemorySource>().memoryTraitID;
        colorTrait = color.GetComponent<ColorSource>().colorTraitID;

        

        var instance = new MaskInstance(Guid.NewGuid().ToString("N"), "xxx",emotionTrait,memoryTrait,colorTrait);
        if (warehouse == null)
        {
            warehouse = maskInventoryLogic.maskInstances;
        }
        warehouse.Add(instance);//给仓库添加此面具
        warehouseUI.Refresh();

        Debug.Log($"[MaskCraft] Crafted: {instance.displayName} | id={instance.instanceId} | instance={instance.instanceId} | traits={instance.colorTraitID}|{instance.emotionTraitID}|{instance.memoryTraitID}");
    }
}
