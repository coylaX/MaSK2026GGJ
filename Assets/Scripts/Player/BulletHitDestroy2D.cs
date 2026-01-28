using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BulletHitDestroy2D : MonoBehaviour
{
    [Header("Optional")]
    public bool destroyOnTrigger = true;   // 勾上：触发器碰撞也销毁
    public bool destroyOnCollision = true; // 勾上：普通碰撞也销毁
    public LayerMask ignoreLayers;         // 可选：忽略哪些层（比如 Player）

    private Collider2D _col;

    [Header("子弹伤害")]
    public float damage;
    private void Awake()
    {
        _col = GetComponent<Collider2D>();
        if (_col == null)
        {
            Debug.LogError("[BulletHitDestroy2D] Missing Collider2D!");
        }
    }
    private void Start()
    {
        damage = GameObject.Find("MainPlayer").GetComponent<PlayerAttack1>().damage;
    }
    private bool IsIgnored(GameObject other)
    {
        // 如果 other 的 layer 在 ignoreLayers 里，就忽略
        return (ignoreLayers.value & (1 << other.layer)) != 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        

        //距离相关buff具体实现功能
        if (PlayerBuff.PlayerBuffInstance.attackFarEnemy)
        {
            GameObject playerGO = GameObject.Find("MainPlayer");
            if (playerGO == null)
            {
                Debug.LogWarning("[Bullet] Cannot find MainPlayer in scene.");
                return;
            }

            // 3) 获取碰撞点（Trigger 没有 contacts，用 ClosestPoint 近似）
            Vector2 hitPoint = other.ClosestPoint(transform.position);

            // 4) 计算碰撞点到玩家的距离
            float dist = Vector2.Distance(hitPoint, (Vector2)playerGO.transform.position);
            if (dist >= PlayerBuff.PlayerBuffInstance.farDis)
            {
                damage *= PlayerBuff.PlayerBuffInstance.AttackBeilvfar;
            }
        }
        if (PlayerBuff.PlayerBuffInstance.attackCloseEnemy)
        {
            GameObject playerGO = GameObject.Find("MainPlayer");
            if (playerGO == null)
            {
                Debug.LogWarning("[Bullet] Cannot find MainPlayer in scene.");
                return;
            }

            // 3) 获取碰撞点（Trigger 没有 contacts，用 ClosestPoint 近似）
            Vector2 hitPoint = other.ClosestPoint(transform.position);

            // 4) 计算碰撞点到玩家的距离
            float dist = Vector2.Distance(hitPoint, (Vector2)playerGO.transform.position);
            if (dist <= PlayerBuff.PlayerBuffInstance.closeDis)
            {
                damage *= PlayerBuff.PlayerBuffInstance.AttackBeilvclose;
            }
        }

        if (PlayerBuff.PlayerBuffInstance.baoxue)
        {
            damage *= PlayerBuff.PlayerBuffInstance.baoxueBeilv;
        }

        if (!destroyOnTrigger) return;
        if (other == null) return;
        if (other.gameObject == gameObject) return;
        if (IsIgnored(other.gameObject)) return;
        //如果碰到怪物就掉血
        if (other.GetComponent<MonsterBase>())
        {
            Vector2 hitPoint = other.ClosestPoint(transform.position);
            other.GetComponent<MonsterBase>().TakeDamage(damage,hitPoint);
        }
        //有穿透buff不销毁
        if (PlayerBuff.PlayerBuffInstance.bulletPenetrate)
        {
            return;
        }
        //如果碰到的是子弹就不销毁
        if (other.gameObject.layer == LayerMask.NameToLayer("enemyBullet"))
        {
            if (PlayerBuff.PlayerBuffInstance.breakEnemyBullet)
            {
                //如果有buff就销毁敌方子弹
                Destroy(other.gameObject);
            }
            return;
        }
        Destroy(gameObject);
    }

    
}
