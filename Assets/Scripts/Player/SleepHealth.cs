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
    public Material flashMaterial;      
    private Material originalMaterial;
    public float invincibilityDuration = 1.0f; // 受击产生的无敌时长
    public float flashInterval = 0.1f;         // 闪烁频率（单次闪白的时长）

    // 原有变量保持不变
    private bool isInvincible = false; 
    private SpriteRenderer sr;
    private Collider2D playerCollider;  
    private ContactFilter2D damageFilter; 

    // --- 新增：内部计时器 ---
    private float hurtInvincibleTimer = 0f;    // 处理受击后的无敌倒计时
    private Coroutine flashCoroutine;          // 引用当前的闪烁协程

    public bool IsDead => currentSleep <= 0f;

    // 综合判断：当前是否处于无敌状态（受击无敌 OR 道具无敌）
    public bool IsActuallyInvincible => (hurtInvincibleTimer > 0f) || 
                                         (PlayerBuff.PlayerBuffInstance != null && PlayerBuff.PlayerBuffInstance.isInvinvible);

    private void Awake()
    {
        currentSleep = Mathf.Clamp(currentSleep, 0f, maxSleep);
        sr = GetComponentInChildren<SpriteRenderer>();
        playerCollider = GetComponent<Collider2D>();

        if (sr != null) originalMaterial = sr.material;
        damageFilter.useTriggers = true;
    }

    private void OnEnable() { LootEvents.OnHealthPicked += Heal; }
    private void OnDisable() { LootEvents.OnHealthPicked -= Heal; }

    private void Update()
    {
        if (IsDead) return;

        // 1. 原有功能：自然流失
        if (drainPerSecond > 0f) ChangeSleep(-drainPerSecond * Time.deltaTime);

        // 2. 处理受击无敌计时器
        if (hurtInvincibleTimer > 0f)
        {
            hurtInvincibleTimer -= Time.deltaTime;
        }

        // 3. 处理外部道具无敌逻辑 (保留并同步 PlayerBuff 状态)
        if (PlayerBuff.PlayerBuffInstance != null && PlayerBuff.PlayerBuffInstance.isInvinvible)
        {
            // 外部无敌时，确保内部标志位同步
            isInvincible = true; 
            PlayerBuff.PlayerBuffInstance.invincibleTime -= Time.deltaTime;
            
            if (PlayerBuff.PlayerBuffInstance.invincibleTime <= 0f)
            {
                PlayerBuff.PlayerBuffInstance.isInvinvible = false;
                // 注意：这里不直接设 isInvincible = false，由状态统一控制
            }
        }

        // 4. 【核心修改】：自适应闪烁驱动
        // 只要处于任何形式的无敌，且闪烁协程没在运行，就开启它
        if (IsActuallyInvincible && !IsDead)
        {
            isInvincible = true;
            if (flashCoroutine == null)
            {
                flashCoroutine = StartCoroutine(AdaptiveFlashRoutine());
            }
        }
        else
        {
            isInvincible = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) => HandleCollision(other.gameObject);
    private void OnCollisionEnter2D(Collision2D collision) => HandleCollision(collision.gameObject);

    private void HandleCollision(GameObject obj)
    {
        // 使用综合无敌状态判断
        if (IsActuallyInvincible || IsDead) return;

        DamageSource source = obj.GetComponent<DamageSource>();
        if (source != null)
        {
            ApplyDamage(source);
        }
    }

    private void ApplyDamage(DamageSource source)
    {
        TakeDamage(source.damageAmount);
        
        if(AudioManager.Instance != null)
            AudioManager.Instance.PlayerHurt();

        // 【修改】：不再直接开启协程，而是设置计时器，由 Update 触发闪烁
        hurtInvincibleTimer = invincibilityDuration;

        if (source.destroyOnHit) Destroy(source.gameObject);
    }

    // --- 【核心逻辑】：自适应闪烁协程 ---
    private IEnumerator AdaptiveFlashRoutine()
    {
        // 只要任何一种无敌状态还在，就一直循环
        while (IsActuallyInvincible && !IsDead)
        {
            if (sr != null) sr.material = flashMaterial;
            yield return new WaitForSeconds(flashInterval);

            if (sr != null) sr.material = originalMaterial;
            yield return new WaitForSeconds(flashInterval);
        }

        // 确保退出时恢复材质
        if (sr != null) sr.material = originalMaterial;
        flashCoroutine = null;

        // 无敌彻底结束后，检查环境伤害（如一直站在地刺上）
        CheckForPersistentDamage();
    }

    private void CheckForPersistentDamage()
    {
        List<Collider2D> results = new List<Collider2D>();
        playerCollider.OverlapCollider(damageFilter, results);

        foreach (var col in results)
        {
            DamageSource source = col.GetComponent<DamageSource>();
            if (source != null && !source.destroyOnHit)
            {
                ApplyDamage(source);
                break; 
            }
        }
    }

    // --- 原有功能函数全部保留 ---
    public void TakeDamage(float amount) { if (!IsDead) ChangeSleep(-amount); }
    public void Heal(float amount) { if (!IsDead) ChangeSleep(amount); }

    private void ChangeSleep(float delta)
    {
        currentSleep = Mathf.Clamp(currentSleep + delta, 0f, maxSleep);
        if (currentSleep <= 0f) Die();
    }

    public void SetSleep(float value)
    {
        currentSleep = Mathf.Clamp(value, 0f, maxSleep);
        if (currentSleep <= 0f) Die();
    }

    public void ResetVisuals()
    {
        StopAllCoroutines();
        flashCoroutine = null;
        hurtInvincibleTimer = 0f;
        isInvincible = false;
        
        if (sr != null && originalMaterial != null) 
        {
            sr.material = originalMaterial;
            sr.color = Color.white;
        }
        
        Animator anim = GetComponentInChildren<Animator>();
        if (anim != null) anim.enabled = true; 
    }

    public void RestoreFullSleep()
    {
        ResetVisuals();
        SetSleep(maxSleep);
    }

    private void Die() 
    { 
        if(AudioManager.Instance != null)
            AudioManager.Instance.PlayerDie();

        ResetVisuals();
        Animator anim = GetComponentInChildren<Animator>();
        if (anim != null) anim.enabled = false;
        PlayerEvents.OnPlayerDeath?.Invoke(); 
    }
}