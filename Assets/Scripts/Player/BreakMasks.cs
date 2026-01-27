using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakMasks : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TearMask();
        }
    }

    // 撕毁面具方法（先留空，之后你再补逻辑）
    private void TearMask()
    {
        GetComponent<MaskRead>().MaskNum += 1;//
        
        //调用亡语效果
        
    }
}
