using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaskUI : MonoBehaviour
{
    public GameObject[] slots; // 拖入 3 个显示槽位

    [Header("表情图标配置")]
    public Sprite XI; 
    public Sprite NU;
    public Sprite AI;
    public Sprite LE;

    // 使用 OnEnable 确保 UI 每次从隐藏变为显示时都会强制刷新
    private void OnEnable()
    {
        // 延迟到一帧结束执行，防止 BackPackLogic 尚未初始化
        StartCoroutine(LateRefresh());
    }

    private IEnumerator LateRefresh()
    {
        yield return null; 
        Refresh();
    }

    public void Refresh()
    {
        if (BackPackLogic.I == null || BackPackLogic.I.maskInstances == null)
        {
            Debug.LogWarning("[MaskUI] 无法找到背包逻辑实例，取消刷新。");
            return;
        }

        int maskCount = BackPackLogic.I.maskInstances.Count;
        int slotCount = slots.Length;

        // --- 全量同步逻辑 ---
        for (int i = 0; i < slotCount; i++)
        {
            if (i < maskCount)
            {
                // 1. 如果有面具，激活槽位并更新内容
                slots[i].SetActive(true);
                UpdateSlotContent(slots[i], BackPackLogic.I.maskInstances[i]);
            }
            else
            {
                // 2. 如果没有面具，直接关闭槽位（这解决了残留问题）
                slots[i].SetActive(false);
            }
        }
    }

    private void UpdateSlotContent(GameObject slotObj, MaskInstance mask)
    {
        Image img = slotObj.GetComponent<Image>();
        if (img == null) return;

        // 更新表情图片
        switch (mask.emotionTraitID)
        {
            case EmotionTraitID.XI: img.sprite = XI; break;
            case EmotionTraitID.NU: img.sprite = NU; break;
            case EmotionTraitID.AI: img.sprite = AI; break;
            case EmotionTraitID.LE: img.sprite = LE; break;
            default: img.sprite = null; break;
        }

        // 更新颜色 (保持原有逻辑)
        switch (mask.colorTraitID)
        {
            case ColorTraitID.RED:    img.color = Color.red; break;
            case ColorTraitID.YELLOW: img.color = Color.yellow; break;
            case ColorTraitID.BLUE:   img.color = Color.blue; break;
            case ColorTraitID.GREEN:  img.color = Color.green; break;
            case ColorTraitID.BLACK:  img.color = Color.gray; break;
            case ColorTraitID.WHITE:  img.color = Color.white; break;
            default:                  img.color = Color.white; break;
        }
    }
}