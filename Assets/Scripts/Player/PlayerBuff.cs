using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuff : MonoBehaviour
{
    [Header("Զ���빥��buff")]
    public bool attackFarEnemy;//ʵ�������ӵ���buff�����ӵ���ȡplayerbuff��״̬�������true����ô�ӵ����ڴ򵽵��˵�ʱ�������Һ͵��˵ľ��룬���������������damage*2���ٽ��������������takedamage\
    public float farDis;
    public float AttackBeilvfar;

    [Header("�����빥��buff")]
    public bool attackCloseEnemy;
    public float closeDis;
    public float AttackBeilvclose;

    [Header("�ӵ���͸Ч��")]
    public bool bulletPenetrate;

    [Header("�ӵ�������Ļ")]
    public bool breakEnemyBullet;

    [Header("�޵�buff")]
    public bool isInvinvible;
    public float invincibleTime;

    [Header("�˺������۳�Ѫ��buff")]
    public bool baoxue;
    public float baoxueBeilv;

    public static PlayerBuff PlayerBuffInstance;
    // Start is called before the first frame update

    void Awake()
    {
        PlayerBuffInstance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
            
       
    }

    // 【核心新增】：重置所有Buff和视觉效果
    public void ResetAllBuffs()
    {
        // 1. 重置所有基础布尔开关
        attackFarEnemy = false;
        attackCloseEnemy = false;
        bulletPenetrate = false;
        breakEnemyBullet = false;
        isInvinvible = false;
        baoxue = false;
        invincibleTime = 0;

        // 2. 重置 MaskRead 中的计数器和攻击状态
        MaskRead mr = GetComponent<MaskRead>();
        if (mr != null) mr.ResetAllState();

        // 3. 重置 BreakMasks 中的临时修改（速度和残影）
        BreakMasks bm = GetComponent<BreakMasks>();
        if (bm != null) bm.ResetTemporaryEffects();
        
        //重置炸弹数量
        UseBomb ub = GetComponent<UseBomb>();
        if (ub != null)
        {
            ub.bombNum = 0; // 或者你想要的初始值
            UseBomb.OnBombCountChanged?.Invoke(ub.bombNum); // 通知UI归零
        }

        Debug.Log("<color=red>[系统]</color> 所有玩家Buff及视觉效果已初始化重置。");
    }
}
