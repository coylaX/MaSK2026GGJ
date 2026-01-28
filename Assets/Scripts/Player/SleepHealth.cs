using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SleepHealth : MonoBehaviour
{
    [Header("HP 配置")]
    public float maxSleep = 100f;
    public float currentSleep = 100f;
    public float drainPerSecond = 2f;

    [Header("受击反馈")]
    public Material flashMaterial;      // 放入刚才创建的 M_FlashWhite
    private Material originalMaterial;
    public float invincibilityDuration = 1.0f;
    public int flashCount = 6;          // 增加闪烁次数让视觉更明显

    private bool isInvincible = false;
    private SpriteRenderer sr;
    private Collider2D playerCollider;  // 用于检测持续碰撞
    private ContactFilter2D damageFilter; // 过滤检测目标

    public bool IsDead => currentSleep <= 0f;

    private void Awake()
    {
        currentSleep = Mathf.Clamp(currentSleep, 0f, maxSleep);
        sr = GetComponentInChildren<SpriteRenderer>();
        playerCollider = GetComponent<Collider2D>();

        if (sr != null) originalMaterial = sr.material;

        // 设置物理检测过滤器：只检测触发器和碰撞体
        damageFilter.useTriggers = true;
    }

    private void OnEnable() { LootEvents.OnHealthPicked += Heal; }
    private void OnDisable() { LootEvents.OnHealthPicked -= Heal; }

    private void Update()
    {


        if (IsDead) return;
        if (drainPerSecond > 0f) ChangeSleep(-drainPerSecond * Time.deltaTime);
        if (PlayerBuff.PlayerBuffInstance.isInvinvible)
        {
            isInvincible = true;
            PlayerBuff.PlayerBuffInstance.invincibleTime -= Time.deltaTime;
            if (PlayerBuff.PlayerBuffInstance.invincibleTime <= 0f)
            {
                isInvincible = false;
                PlayerBuff.PlayerBuffInstance.isInvinvible = false;
            }
            //补一个五秒timer
        }
    }

    private void OnTriggerEnter2D(Collider2D other) => HandleCollision(other.gameObject);
    private void OnCollisionEnter2D(Collision2D collision) => HandleCollision(collision.gameObject);

    private void HandleCollision(GameObject obj)
    {
        if (isInvincible || IsDead) return;

        DamageSource source = obj.GetComponent<DamageSource>();
        if (source != null)
        {
            ApplyDamage(source);
        }
    }

    // 将伤害逻辑独立出来，方便重复调用
    private void ApplyDamage(DamageSource source)
    {
        TakeDamage(source.damageAmount);
        StartCoroutine(InvincibilityRoutine());

        if (source.destroyOnHit) Destroy(source.gameObject);
    }

    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;

        for (int i = 0; i < flashCount; i++)
        {
            // 切换到闪白材质
            if (sr != null) sr.material = flashMaterial;
            yield return new WaitForSeconds(invincibilityDuration / (flashCount * 2f));

            // 切回原材质
            if (sr != null) sr.material = originalMaterial;
            yield return new WaitForSeconds(invincibilityDuration / (flashCount * 2f));
        }

        isInvincible = false;

        // 【功能 2】：无敌结束后，检查是否仍重叠在伤害源上
        CheckForPersistentDamage();
    }

    private void CheckForPersistentDamage()
    {
        // 扫描所有与玩家重叠的碰撞体
        List<Collider2D> results = new List<Collider2D>();
        playerCollider.OverlapCollider(damageFilter, results);

        foreach (var col in results)
        {
            DamageSource source = col.GetComponent<DamageSource>();
            // 如果还在碰着怪物（且怪物没死），再次触发受击
            if (source != null && !source.destroyOnHit)
            {
                ApplyDamage(source);
                break; // 触发一次即可，进入下一轮无敌
            }
        }
    }

    public void TakeDamage(float amount) { if (!IsDead) ChangeSleep(-amount); }
    public void Heal(float amount) { if (!IsDead) ChangeSleep(amount); }

    private void ChangeSleep(float delta)
    {
        currentSleep = Mathf.Clamp(currentSleep + delta, 0f, maxSleep);
        if (currentSleep <= 0f) Die();
    }

    private void Die() { PlayerEvents.OnPlayerDeath?.Invoke(); }
}