using UnityEngine;

public class MonsterAI_Ranged : MonoBehaviour {
    private MonsterBase stats;
    private Transform player;
    
    [Header("远程设置")]
    public GameObject projectilePrefab; 
    public float fireRate = 2f;         
    public float projectileSpeed = 5f;  
    private float fireTimer;

    [Header("视觉反馈")]
    public Color readyColor = Color.white;
    public Color chargingColor = Color.red; 
    private SpriteRenderer sr;

    void Start() {
        stats = GetComponent<MonsterBase>();
        
        // 【改进点 1】：使用 GetComponentInChildren，防止图片在子物体上导致报错
        sr = GetComponentInChildren<SpriteRenderer>();
        
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
        
        fireTimer = fireRate;
    }

    void Update() {
        if (player == null || (stats != null && stats.IsInKnockback())) return;

        fireTimer -= Time.deltaTime;

        // 射线检测
        int mask = LayerMask.GetMask("block");
        RaycastHit2D hit = Physics2D.Linecast(transform.position, player.position, mask);

        if (hit.collider == null) {
            // 【改进点 2】：增加 sr != null 的判断，防止崩溃
            if (sr != null) {
                if (fireTimer < 0.5f) sr.color = chargingColor;
                else sr.color = readyColor;
            }

            if (fireTimer <= 0) {
                Shoot();
                fireTimer = fireRate;
                if (sr != null) sr.color = readyColor;
            }
        } else {
            if (sr != null) sr.color = readyColor;
        }
    }

    void Shoot() {
        if (projectilePrefab == null) {
            Debug.LogError($"{gameObject.name} 缺少子弹预制体！");
            return;
        }

        GameObject bulletObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        MonsterProjectile projectile = bulletObj.GetComponent<MonsterProjectile>();
        
        if (projectile != null) {
            Vector2 shootDir = (player.position - transform.position).normalized;
            projectile.Setup(shootDir, projectileSpeed, stats.contactDamage);
        }
    }
}