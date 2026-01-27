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
        if (!destroyOnTrigger) return;
        if (other == null) return;
        if (other.gameObject == gameObject) return;
        if (IsIgnored(other.gameObject)) return;
        if (other.GetComponent<MonsterBase>())
        {
            Vector2 hitPoint = other.ClosestPoint(transform.position);
            other.GetComponent<MonsterBase>().TakeDamage(damage,hitPoint);
        }
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!destroyOnCollision) return;
        if (collision == null) return;
        if (collision.gameObject == gameObject) return;
        if (IsIgnored(collision.gameObject)) return;

        Destroy(gameObject);
    }
}
