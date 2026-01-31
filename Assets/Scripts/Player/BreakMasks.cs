using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakMasks : MonoBehaviour
{
    [Header("�������ӵĵ�λ�ٶ�")]
    public float addspeed;
    [Header("�������ӵĵ�λ����")]
    public float addHealth;
    [Header("���UI")]
    public GameObject MaskUI;

    // --- 【新增 1】 ---
    [Header("特效预制体")]
    [Tooltip("撕碎白面具回血时播放的粒子特效")]
    public GameObject healVfxPrefab; 
    // ------------------

    private float _initialMaxSpeed; // 【新增】用于记录初始速度

    private void Start()
    {
        // 记录游戏开始时的初始速度
        var moveScript = GetComponent<PlayerMoveAndFlip>();
        if (moveScript != null)
        {
            _initialMaxSpeed = moveScript.maxSpeed;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TearMask();
        }
    }

    // ˺����߷���
    private void TearMask()
    {
        GetComponent<MaskRead>().invincibleNum = 0;
        GetComponent<MaskRead>().baoxuenum = 0;
        GetComponent<MaskRead>().bombAddNum = 0;
        //��������Ч��
        if (GetComponent<MaskRead>().currentMask == null)
        {
            return;
        }
        //˺��ߵ�Ѫ������
        
        GetComponent<SleepHealth>().maxSleep -= 20;
        Debug.Log(11);
        
        switch (GetComponent<MaskRead>().currentMask.colorTraitID)
        {
            case ColorTraitID.RED:
                //�Է�ΧС��5�ĵ��˴����߹�����
                PlayerBuff.PlayerBuffInstance.attackCloseEnemy = true;
                break;
            case ColorTraitID.YELLOW:
                //���������������Ե������˵�Ļ
                PlayerBuff.PlayerBuffInstance.breakEnemyBullet = true;
                break;
            case ColorTraitID.BLUE:
                //�ӵ���͸����
                PlayerBuff.PlayerBuffInstance.bulletPenetrate = true;
                break;
            case ColorTraitID.GREEN:
                //����������ԽԶ�ĵ�������˺�����
                PlayerBuff.PlayerBuffInstance.attackFarEnemy = true;
                break;
            case ColorTraitID.BLACK:
                //����������
                AddSpeed();
                break;
            case ColorTraitID.WHITE:
                //����������
                AddHealth();
                break;
        }

        //�Ƴ���ߣ�UIͬ������
        BackPackLogic.I.maskInstances.Remove(GetComponent<MaskRead>().currentMask);
        MaskUI.GetComponent<MaskUI>().Refresh();



    }
    public void AddSpeed()
    {
        GetComponent<PlayerMoveAndFlip>().maxSpeed += addspeed;

        // 【核心新增】
        GhostEffect ghost = GetComponent<GhostEffect>();
        if (ghost != null) {
            ghost.isVisible = true; // 开启残影
        }
    }
    public void AddHealth()
    {
        GetComponent<SleepHealth>().Heal(addHealth);

        // --- 【新增 2】 ---
        // 生成回血特效
        if (healVfxPrefab != null)
        {
            // 在玩家当前脚下位置生成，不跟随旋转
            Instantiate(healVfxPrefab, transform.position, Quaternion.identity);
            Debug.Log("<color=green>[视觉]</color> 播放回血粒子特效。");
        }
        // ------------------
    }


    // 【核心新增】：重置那些“直接改写”的效果
    public void ResetTemporaryEffects()
    {
        // 1. 恢复速度
        var moveScript = GetComponent<PlayerMoveAndFlip>();
        if (moveScript != null)
        {
            moveScript.maxSpeed = _initialMaxSpeed;
        }

        // 2. 关闭残影视觉效果
        GhostEffect ghost = GetComponent<GhostEffect>();
        if (ghost != null)
        {
            ghost.isVisible = false;
        }
    }
}
