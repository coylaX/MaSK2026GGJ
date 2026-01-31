using UnityEngine;

public class Obstacle : MonoBehaviour {
    public bool isDestructible;
    public float health = 10f;

    [Header("破坏表现")]
    public Sprite brokenSprite;           // 留在地上的碎片图片（底图）
    public GameObject shatterVfxPrefab;   // 专门播放碎裂动画的预制体（挂载 ExplosionEffect）

    private SpriteRenderer sr;
    private Collider2D col;
    private Rigidbody2D rb;
    private bool isBroken = false;

    void Awake() {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(float damage) {
        if (!isDestructible || isBroken) return; 

        health -= damage;
        if (health <= 0) {
            BreakObstacle();
        }
    }

    private void BreakObstacle() {
        isBroken = true;

        // 1. 生成碎裂动画特效 
        // 这个 Prefab 建议挂载你之前的 ExplosionEffect 脚本，实现自动音效和销毁
        if (shatterVfxPrefab != null) {
            Instantiate(shatterVfxPrefab, transform.position, Quaternion.identity);
        }

        // 2. 切换当前物体的图片为“碎片底图”
        if (sr != null && brokenSprite != null) {
            sr.sprite = brokenSprite;
            // 建议：把排序 order 降低，防止它挡住掉落物或玩家
            sr.sortingOrder -= 1; 
        }

        // 3. 彻底取消物理特性，使其变成纯粹的背景装饰
        if (col != null) col.enabled = false;
        
        if (rb != null) {
            // 使用 simulated = false 可以瞬间停止所有物理交互，且比 Destroy(rb) 更安全
            rb.simulated = false; 
        }

        Debug.Log($"{gameObject.name} 已转化为背景碎片。");
    }
}