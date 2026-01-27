using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakMasks : MonoBehaviour
{
    [Header("亡语增加的单位速度")]
    public float addspeed;
    [Header("亡语增加的单位生命")]
    public float addHealth;
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
        //调用亡语效果
        
        switch (GetComponent<MaskRead>().currentMask.colorTraitID)
        {
            case ColorTraitID.RED:
                //减少攻速，对范围小于5的敌人大幅提高攻击力
                break;
            case ColorTraitID.YELLOW:
                //后续武器攻击可以抵消敌人弹幕
                break;
            case ColorTraitID.BLUE:
                //子弹穿透亡语
                break;
            case ColorTraitID.GREEN:
                //后续武器对越远的敌人造成伤害更高
                break;
            case ColorTraitID.BLACK:
                //加移速亡语
                AddSpeed();
                break;
            case ColorTraitID.WHITE:
                //加生命亡语
                AddHealth();
                break;
        }

        GetComponent<MaskRead>().MaskNum += 1;//



    }
    public void AddSpeed()
    {
        GetComponent<PlayerMoveAndFlip>().maxSpeed += addspeed;
    }
    public void AddHealth()
    {
        GetComponent<SleepHealth>().Heal(addHealth);
    }

}
