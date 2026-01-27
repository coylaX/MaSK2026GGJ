using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuff : MonoBehaviour
{
    public bool attackFarEnemy;//实际上是子弹的buff，让子弹读取playerbuff的状态，如果是true，那么子弹就在打到敌人的时候计算玩家和敌人的距离，满足距离条件就让damage*2，再将参数传给怪物的takedamage
    public bool attackCloseEnemy;

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
