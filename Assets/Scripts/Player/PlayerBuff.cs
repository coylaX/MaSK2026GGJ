using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuff : MonoBehaviour
{
    [Header("远距离攻击buff")]
    public bool attackFarEnemy;//实际上是子弹的buff，让子弹读取playerbuff的状态，如果是true，那么子弹就在打到敌人的时候计算玩家和敌人的距离，满足距离条件就让damage*2，再将参数传给怪物的takedamage\
    public float farDis;
    public float AttackBeilvfar;

    [Header("近距离攻击buff")]
    public bool attackCloseEnemy;
    public float closeDis;
    public float AttackBeilvclose;

    [Header("子弹穿透效果")]
    public bool bulletPenetrate;

    [Header("子弹抵消弹幕")]
    public bool breakEnemyBullet;

    [Header("无敌buff")]
    public bool isInvinvible;
    public float invincibleTime;

    [Header("伤害翻倍扣除血量buff")]
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
}
