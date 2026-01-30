using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Bomb : MonoBehaviour
{
    [Header("Explosion")]
    public float fuseSeconds = 2f;                // 定时炸弹爆炸时间
    public float explosionRadius = 3f;            // 【新增】爆炸半径
    public GameObject explosionVfxPrefab;         // 爆炸特效预制体
    public bool destroyBombOnExplode = true;
    public float explosionDamage = 100;

    [Header("Screen Shake")]
    public float shakeDuration = 0.1f;            // 【新增】抖动时长
    public float shakeIntensity = 0.005f;           // 【新增】抖动强度

    [Header("Detection")]
    public LayerMask detectionLayers;             // 【修改】检测哪些层级的物体
    public bool debugLogTargets = false;

    private void Start()
    {
        StartCoroutine(ExplodeAfterDelay());
    }

    private IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(fuseSeconds);
        Explode();
    }

    private void Explode()
    {
        // 1) 播放爆炸特效
        if (explosionVfxPrefab != null)
            Instantiate(explosionVfxPrefab, transform.position, Quaternion.identity);

        // 2) 【核心变更】：使用物理圆域检测获取范围内所有碰撞体
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, explosionRadius, detectionLayers);
        
        // 3) 处理伤害
        HandleTargets(targets);

        // 4) 执行屏幕抖动
        if (CameraManager.Instance != null)
        {
            CameraManager.Instance.Shake(shakeDuration, shakeIntensity);
        }

        // 5) 销毁炸弹
        if (destroyBombOnExplode)
            Destroy(gameObject);
    }

    private void HandleTargets(Collider2D[] targets)
    {
        if (targets == null) return;

        foreach (Collider2D col in targets)
        {
            GameObject target = col.gameObject;
            if (target == null) continue;

            // 1) 怪物伤害
            if (target.TryGetComponent<MonsterBase>(out var monster))
            {
                monster.TakeDamage(explosionDamage, transform.position);
                if (debugLogTargets) Debug.Log($"炸弹伤害了怪物: {target.name}");
            }
            
            // 2) 障碍物破坏
            if (target.TryGetComponent<Obstacle>(out var obstacle))
            {
                if (obstacle.isDestructible)
                {
                    Destroy(obstacle.gameObject);
                    if (debugLogTargets) Debug.Log($"炸弹摧毁了障碍物: {target.name}");
                }
            }
        }
    }

    // 在编辑器里画出爆炸范围，方便调试
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}