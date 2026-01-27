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
        gameObject.SetActive(false); 
        if (myRoom != null) {
            myRoom.OnMonsterKilled(this);
        }
        Destroy(gameObject);
    }

    public void ApplyEliteBuff(int type) {
        // 获取渲染器用于发光效果
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();

        switch (type) {
            case 0: // 移速 +50%
                moveSpeed *= 1.5f;
                SetGlow(new Color(1f, 1f, 0f, 1f)); // 黄色
                break;
            case 1: // 生命翻倍
                health *= 2f;
                SetGlow(new Color(0f, 1f, 0f, 1f)); // 绿色
                break;
            case 2: // 攻击力翻倍
                contactDamage *= 2f;
                SetGlow(new Color(1f, 0f, 0f, 1f)); // 红色
                break;
        }
    }

    private void SetGlow(Color color) {
        if (sr != null) {
            // 这里我们通过修改材质的颜色来实现发光感
            // 如果你使用了特殊的 Shader，可以改为 sr.material.SetColor("_GlowColor", color);
            sr.color = color; 
            
            // 进阶建议：在怪物脚下实例化一个带颜色的光环 Prefab
            // GameObject halo = Instantiate(eliteHaloPrefab, transform);
            // halo.GetComponent<SpriteRenderer>().color = color;
        }
    }

}