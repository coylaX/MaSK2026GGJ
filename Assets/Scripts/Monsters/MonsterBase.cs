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
    public Color flashColor = Color.red;    // 受击时的颜色
    public float flashDuration = 0.1f;      // 变色持续时间
    private Color originalColor;            // 记录初始颜色
    protected SpriteRenderer sr;            // 渲染器引用

    [Header("寻路配置")]
    public bool blocksPathfinding = false; 

    [HideInInspector] public RoomController myRoom;
    protected Rigidbody2D rb;
    private bool isKnockingBack = false;
    private Coroutine flashCoroutine;       // 用于管理变色协程

    [Header("掉落设置")]
    public GameObject emotionLootPrefab; // 在 Inspector 中拖入 LootEmotion 预制体
    public bool isElite = false;         // 标记是否为强化怪物

    protected virtual void Awake() {
        rb = GetComponent<Rigidbody2D>();
        // 尝试从本体或子物体获取渲染器
        sr = GetComponentInChildren<SpriteRenderer>();
        
        if (sr != null) {
            originalColor = sr.color;
        }

        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    public void TakeDamage(float damage, Vector2 attackerPos) {
        health -= damage;
        
        // 1. 触发变色反馈
        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashRoutine());

        // 2. 触发击退
        Vector2 dir = ((Vector2)transform.position - attackerPos).normalized;
        StartCoroutine(KnockbackRoutine(dir));
        
        if (health <= 0) Die();
    }

    // 变色协程
    private IEnumerator FlashRoutine() {
        if (sr == null) yield break;

        sr.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
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

    [ContextMenu("Kill Monster (Debug)")]
    public void Die() {
        // --- 新增：掉落逻辑 ---
        float dropChance = isElite ? 0.8f : 0.3f; // 强化怪 50%，普通怪 20%
        
        if (UnityEngine.Random.value <= dropChance) {
            if (emotionLootPrefab != null) {
                // 在怪物死亡位置生成情绪战利品
                Instantiate(emotionLootPrefab, transform.position, Quaternion.identity);
            }
        }

        // --- 原有死亡逻辑 ---
        gameObject.SetActive(false); 
        if (myRoom != null) {
            myRoom.OnMonsterKilled(this);
        }
        Destroy(gameObject);
    }

    // 修改之前的 ApplyEliteBuff，记录强化状态
    public void ApplyEliteBuff(int type) {
        isElite = true; // 只要施加了增益，就标记为精英
        
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();
        switch (type) {
            case 0: // 移速
                moveSpeed *= 1.5f;
                SetGlow(Color.yellow);
                break;
            case 1: // 生命
                health *= 2f;
                SetGlow(Color.green);
                break;
            case 2: // 攻击
                contactDamage *= 2f;
                SetGlow(Color.red);
                break;
        }
    }

    // 辅助变色方法（保持简洁）
    private void SetGlow(Color c) {
        if (sr != null) sr.color = c;
    }

}