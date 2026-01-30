using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskRead : MonoBehaviour
{
    public MaskInstance currentMask;
    
    // --- 【关键新增】：记录上一次检测到的面具实例 ---
    private MaskInstance lastMaskInstance;

    [Header("触发计数器")]
    public int baoxuenum;
    public int invincibleNum;
    public int bombAddNum;

    public void onStart() { }

    private void Update()
    {
        // 1. 如果背包里没面具了
        if (BackPackLogic.I.maskInstances.Count == 0)
        {
            // 清除攻击状态
            var attack = GetComponent<PlayerAttack1>();
            if (attack != null) {
                attack.isHappy = false;
                attack.isBlade = false;
                attack.isSad = false;
                attack.isMachineGun = false;
                attack.prefab = null;
            }
            currentMask = null;
            lastMaskInstance = null; // 重置引用追踪
            return;
        }

        // 2. 获取当前排在第一位的面具
        currentMask = BackPackLogic.I.maskInstances[0];

        // --- 【核心逻辑】：对象引用对比 ---
        // 只要 currentMask 指向的内存地址变了，就说明换面具了
        if (currentMask != lastMaskInstance)
        {
            ResetBuffCounters(); // 自动重置所有计数器
            lastMaskInstance = currentMask; // 更新追踪
            Debug.Log($"<color=orange>[面具系统]</color> 检测到新面具: {currentMask.emotionTraitID}, 效果已重置。");
        }

        // 3. 情绪类效果 (每帧更新，保持原样)
        HandleEmotionTraits();

        // 4. 记忆类效果 (只触发一次的 Buff)
        HandleMemoryTraits();
    }

    private void ResetBuffCounters()
    {
        baoxuenum = 0;
        invincibleNum = 0;
        bombAddNum = 0;
    }

    private void HandleEmotionTraits()
    {
        var attack = GetComponent<PlayerAttack1>();
        if (attack == null) return;

        switch (currentMask.emotionTraitID)
        {
            case EmotionTraitID.XI:
                attack.isHappy = true; attack.isBlade = false; attack.isSad = false; attack.isMachineGun = false;
                break;
            case EmotionTraitID.NU:
                attack.isHappy = false; attack.isBlade = true; attack.isSad = false; attack.isMachineGun = false;
                break;
            case EmotionTraitID.AI:
                attack.isHappy = false; attack.isSad = true; attack.isBlade = false; attack.isMachineGun = false;
                break;
            case EmotionTraitID.LE:
                attack.isHappy = false; attack.isMachineGun = true; attack.isBlade = false; attack.isSad = false;
                break;
        }
    }

    private void HandleMemoryTraits()
    {
        switch (currentMask.memoryTraitID)
        {
            case MemoryTraitID.A:
                if (invincibleNum == 0)
                {
                    PlayerBuff.PlayerBuffInstance.isInvinvible = true;
                    // 【重要修复】：必须在这里给无敌时间赋值，否则无敌会瞬间结束
                    PlayerBuff.PlayerBuffInstance.invincibleTime = 10f; 
                    invincibleNum++;
                }
                break;

            case MemoryTraitID.B:
                PlayerBuff.PlayerBuffInstance.baoxue = true;
                if (baoxuenum == 0)
                {
                    GetComponent<SleepHealth>().currentSleep /= 2;
                    baoxuenum += 1;
                }
                break;

            case MemoryTraitID.C:
                if (bombAddNum == 0)
                {
                    GetComponent<UseBomb>().bombNum += 3;
                    bombAddNum += 1;
                }
                break;
        }
    }
}