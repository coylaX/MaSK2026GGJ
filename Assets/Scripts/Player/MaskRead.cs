using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskRead : MonoBehaviour
{
   
    public MaskInstance currentMask;
    public int MaskNum;
    public void onStart()
    {
        MaskNum = 1;
        
    }
    private void Update()
    {
        //撕面具masknum-1
        if (MaskNum > 3)
        {
            //面具撕完了
            GetComponent<PlayerAttack1>().isHappy = false;
            GetComponent<PlayerAttack1>().isBlade = false;
            GetComponent<PlayerAttack1>().isSad = false;
            GetComponent<PlayerAttack1>().isMachineGun = false;
            GetComponent<PlayerAttack1>().prefab = null;
            currentMask = null;
            return;
        }
        currentMask = BackPackLogic.I.maskInstances[MaskNum-1];
        Debug.Log(currentMask);
        //加载武器相关
        switch (currentMask.emotionTraitID)
        {
            case EmotionTraitID.XI:
                GetComponent<PlayerAttack1>().isHappy = true;             
                GetComponent<PlayerAttack1>().isBlade = false;
                GetComponent<PlayerAttack1>().isSad = false;
                GetComponent<PlayerAttack1>().isMachineGun = false;
                break;
            case EmotionTraitID.NU:
                GetComponent<PlayerAttack1>().isHappy = false;
                GetComponent<PlayerAttack1>().isBlade = true;
                GetComponent<PlayerAttack1>().isSad = false;
                GetComponent<PlayerAttack1>().isMachineGun = false;
                break;
            case EmotionTraitID.AI:
                GetComponent<PlayerAttack1>().isHappy = false;
                GetComponent<PlayerAttack1>().isSad = true;
                GetComponent<PlayerAttack1>().isBlade = false;
                GetComponent<PlayerAttack1>().isMachineGun = false;
                break;
            case EmotionTraitID.LE:
                GetComponent<PlayerAttack1>().isHappy = false;
                GetComponent<PlayerAttack1>().isMachineGun = true;
                GetComponent<PlayerAttack1>().isBlade = false;
                GetComponent<PlayerAttack1>().isSad = false;
                break;
        }
        //上记忆相关buff
        switch (currentMask.memoryTraitID)
        {
            case MemoryTraitID.A:
                PlayerBuff.PlayerBuffInstance.isInvinvible = true;
                //无敌五秒
                break;
            case MemoryTraitID.B:
                //扣除血量伤害翻倍
                PlayerBuff.PlayerBuffInstance.baoxue = true;
                GetComponent<SleepHealth>().currentSleep -= 50;
                break;
            case MemoryTraitID.C:
                //获得三枚炸弹
                GetComponent<UseBomb>().bombNum += 3;
                break;
        }
    }
   
}
