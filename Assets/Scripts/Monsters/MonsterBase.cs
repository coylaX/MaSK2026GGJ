using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class MonsterBase : MonoBehaviour {
    [Header("基础属性")]
    public float health = 100f;
    public float moveSpeed = 3f;
    public float contactDamage = 10f;
    public float knockbackForce = 5f;

    [Header("受击反馈")]
    public Color flashColor = Color.white;  // 建议受击闪烁设为白色，以区分精英怪底色
    public float flashDuration = 0.1f;
    private Color originalColor;            // 回归你习惯的命名：记录当前的“底色”
    protected SpriteRenderer sr;

    [Header("寻路配置")]
    public bool blocksPathfinding = false; 

    [HideInInspector] public RoomController myRoom;
    protected Rigidbody2D rb;
    private bool isKnockingBack = false;
    private Coroutine flashCoroutine;

    [Header("掉落设置")]
    public GameObject emotionLootPrefab; 
    public bool isElite = false; 

    protected virtual void Awake() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        
        if (sr != null) {
            originalColor = sr.color; // 初始记录为白色
        }

        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private void Start(){
        // 确保启动时伤害同步
        var ds = GetComponent<DamageSource>();
        if(ds != null) ds.damageAmount = contactDamage;
    }

    public void TakeDamage(float damage, Vector2 attackerPos) {
        health -= damage;
        
        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashRoutine());

        Vector2 dir = ((Vector2)transform.position - attackerPos).normalized;
        StartCoroutine(KnockbackRoutine(dir));
        
        if (health <= 0) Die();
    }

    private IEnumerator FlashRoutine() {
        if (sr == null) yield break;

        Color prevColor = sr.color;
        sr.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        
        // 关键：受击闪烁结束后，必须变回当前的 originalColor
        sr.color = originalColor; 
        flashCoroutine = null;
    }

    private IEnumerator KnockbackRoutine(Vector2 dir) {
        isKnockingBack = true;
        rb.velocity = Vector2.zero;
        rb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.2f);
        isKnockingBack = false;
    }

    public bool IsInKnockback() => isKnockingBack;

    public void Die() {
        // 掉落逻辑：精英怪 80%，普通怪 30%
        float dropChance = isElite ? 0.8f : 0.3f; 
        
        if (UnityEngine.Random.value <= dropChance) {
            if (emotionLootPrefab != null) {
                // 获取全局掉落物容器
                Transform parent = LevelManager.Instance != null ? LevelManager.Instance.lootContainer : null;
                
                // 使用带 parent 参数的重载
                Instantiate(emotionLootPrefab, transform.position, Quaternion.identity, parent);
            }
        }

        gameObject.SetActive(false); 
        if (myRoom != null) {
            myRoom.OnMonsterKilled(this);
        }
        Destroy(gameObject);
    }

    // 处理精英怪强化的核心逻辑
    public void ApplyEliteBuff(int type) {
        isElite = true; 
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();

        switch (type) {
            case 0: // 移速型
                moveSpeed *= 1.5f;
                SetGlow(Color.yellow);
                break;
            case 1: // 生命型
                health *= 2f;
                SetGlow(Color.green);
                break;
            case 2: // 攻击型
                contactDamage *= 2f;
                SetGlow(Color.magenta); // 使用洋红色避开受击的红色
                break;
        }
        
        // 【核心修复】：更新底色记录
        // 这样后续的受击闪烁 FlashRoutine 就会知道该变回精英怪颜色了
        if (sr != null) {
            originalColor = sr.color;
        }

        var ds = GetComponent<DamageSource>();
        if(ds != null) ds.damageAmount = contactDamage;
        
        Debug.Log($"[精英怪激活] {gameObject.name} 类型: {type}, 颜色锁定为: {originalColor}");
    }

    private void SetGlow(Color c) {
        if (sr != null) sr.color = c;
    }
}