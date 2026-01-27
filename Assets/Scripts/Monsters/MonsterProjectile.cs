using UnityEngine;

public class MonsterProjectile : MonoBehaviour {
    private float speed;
    private float damage;
    private Vector2 direction;
    private float lifetime = 5f; // 防止子弹飞出地图后永不销毁

    public void Setup(Vector2 dir, float projectileSpeed, float projectileDamage) {
        direction = dir.normalized;
        speed = projectileSpeed;
        damage = projectileDamage;
        
        // 让子弹旋转指向飞行方向 (可选)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        
        Destroy(gameObject, lifetime);
    }

    void Update() {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Enemy")) return; // 忽略怪物的碰撞
        // 1. 碰到墙壁 (block 层) 销毁
        if (((1 << other.gameObject.layer) & LayerMask.GetMask("block")) != 0) {
            Destroy(gameObject);
        }

        // 2. 碰到玩家扣血
        if (other.CompareTag("Player")) {
            // 这里假设你的 Player 有一个 TakeDamage 函数，或者后续我们要实现的逻辑
            Debug.Log($"弹幕击中了玩家，造成 {damage} 点伤害！");
            // other.GetComponent<PlayerBase>().TakeDamage(damage); 
            Destroy(gameObject);
        }
    }
}