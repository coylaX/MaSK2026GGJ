using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BulletHitDestroy2D : MonoBehaviour
{
    [Header("Optional")]
    public bool destroyOnTrigger = true;   
    public bool destroyOnCollision = true; 
    public LayerMask ignoreLayers;         

    private Collider2D _col;
    private SpriteRenderer _spriteRenderer; // 用于控制子弹本身透明度

    [Header("子弹伤害")]
    public float damage;

    [Header("命中特效")]
    [Tooltip("子弹命中敌人时产生的 Prefab")]
    public GameObject hitEffectPrefab; 

    [Header("Buff 视觉反馈")]
    [Tooltip("当 PlayerBuff.baoxue 为 true 时显示")]
    public GameObject redHalo;    // 拖入子物体 RedHalo
    [Tooltip("当 PlayerBuff.breakEnemyBullet 为 true 时显示")]
    public GameObject yellowHalo; // 拖入子物体 YellowHalo

    private void Awake()
    {
        _col = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>(); 

        if (_col == null)
        {
            Debug.LogError("[BulletHitDestroy2D] Missing Collider2D!");
        }
    }

    private void Start()
    {
        // 建议：如果 MainPlayer 找不到，先做个空引用保护
        GameObject player = GameObject.Find("MainPlayer");
        if (player != null)
        {
            damage = player.GetComponent<PlayerAttack1>().damage;
        }

        // --- 应用所有 Buff 视觉效果 ---
        ApplyBuffVisuals();
    }

    private void ApplyBuffVisuals()
    {
        if (PlayerBuff.PlayerBuffInstance == null) return;

        // 1. 穿透子弹半透明效果 (检测 bulletPenetrate)
        if (PlayerBuff.PlayerBuffInstance.bulletPenetrate && _spriteRenderer != null)
        {
            Color c = _spriteRenderer.color;
            _spriteRenderer.color = new Color(c.r, c.g, c.b, 0.5f);
        }

        // 2. 红色光圈效果 (检测 baoxue - 伤害翻倍/扣血Buff)
        if (PlayerBuff.PlayerBuffInstance.baoxue && redHalo != null)
        {
            redHalo.SetActive(true);
        }

        // 3. 黄色光圈效果 (检测 breakEnemyBullet - 抵消弹幕)
        if (PlayerBuff.PlayerBuffInstance.breakEnemyBullet && yellowHalo != null)
        {
            yellowHalo.SetActive(true);
        }
    }

    private bool IsIgnored(GameObject other)
    {
        return (ignoreLayers.value & (1 << other.layer)) != 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsIgnored(other.gameObject)) return;

        // --- 伤害加成逻辑保留 ---
        if (PlayerBuff.PlayerBuffInstance.attackFarEnemy)
        {
            GameObject playerGO = GameObject.Find("MainPlayer");
            if (playerGO != null)
            {
                Vector2 hitPoint = other.ClosestPoint(transform.position);
                float dist = Vector2.Distance(hitPoint, (Vector2)playerGO.transform.position);
                if (dist >= PlayerBuff.PlayerBuffInstance.farDis)
                {
                    damage *= PlayerBuff.PlayerBuffInstance.AttackBeilvfar;
                }
            }
        }
        if (PlayerBuff.PlayerBuffInstance.attackCloseEnemy)
        {
            GameObject playerGO = GameObject.Find("MainPlayer");
            if (playerGO != null)
            {
                Vector2 hitPoint = other.ClosestPoint(transform.position);
                float dist = Vector2.Distance(hitPoint, (Vector2)playerGO.transform.position);
                if (dist <= PlayerBuff.PlayerBuffInstance.closeDis)
                {
                    damage *= PlayerBuff.PlayerBuffInstance.AttackBeilvclose;
                }
            }
        }

        if (PlayerBuff.PlayerBuffInstance.baoxue)
        {
            damage *= PlayerBuff.PlayerBuffInstance.baoxueBeilv;
        }

        if (!destroyOnTrigger) return;
        if (other == null) return;
        if (other.gameObject == gameObject) return;

        // --- 【核心修改】：命中怪物处理 ---
        if (other.GetComponent<MonsterBase>())
        {
            Vector2 hitPoint = other.ClosestPoint(transform.position);
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, hitPoint, Quaternion.identity);
            }
            other.GetComponent<MonsterBase>().TakeDamage(damage, hitPoint);
        }

        // 穿透逻辑保留
        if (PlayerBuff.PlayerBuffInstance.bulletPenetrate)
        {
            return;
        }

        // 消弹逻辑保留
        if (other.gameObject.layer == LayerMask.NameToLayer("enemyBullet"))
        {
            if (PlayerBuff.PlayerBuffInstance.breakEnemyBullet)
            {
                Destroy(other.gameObject);
            }
            return;
        }

        Destroy(gameObject);
    }
}