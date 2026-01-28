using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakMasks : MonoBehaviour
{
    [Header("亡语增加的单位速度")]
    public float addspeed;
    [Header("亡语增加的单位生命")]
    public float addHealth;
    [Header("面具UI")]
    public GameObject MaskUI;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TearMask();
        }
    }

    // 撕毁面具方法
    private void TearMask()
    {
        GetComponent<MaskRead>().invincibleNum = 0;
        GetComponent<MaskRead>().baoxuenum = 0;
        GetComponent<MaskRead>().bombAddNum = 0;
        //调用亡语效果
        if (GetComponent<MaskRead>().currentMask == null)
        {
            return;
        }
        //撕面具掉血量上限
        
        GetComponent<SleepHealth>().maxSleep -= 20;
        Debug.Log(11);
        
        switch (GetComponent<MaskRead>().currentMask.colorTraitID)
        {
            case ColorTraitID.RED:
                //对范围小于5的敌人大幅提高攻击力
                PlayerBuff.PlayerBuffInstance.attackCloseEnemy = true;
                break;
            case ColorTraitID.YELLOW:
                //后续武器攻击可以抵消敌人弹幕
                PlayerBuff.PlayerBuffInstance.breakEnemyBullet = true;
                break;
            case ColorTraitID.BLUE:
                //子弹穿透亡语
                PlayerBuff.PlayerBuffInstance.bulletPenetrate = true;
                break;
            case ColorTraitID.GREEN:
                //后续武器对越远的敌人造成伤害更高
                PlayerBuff.PlayerBuffInstance.attackFarEnemy = true;
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

        //移出面具，UI同步更改
        BackPackLogic.I.maskInstances.Remove(GetComponent<MaskRead>().currentMask);
        MaskUI.GetComponent<MaskUI>().Refresh();



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
