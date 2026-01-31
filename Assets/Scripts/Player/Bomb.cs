using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Bomb : MonoBehaviour
{
    [Header("Explosion")]
    public float fuseSeconds = 2f;                
    public float explosionRadius = 3f;            
    public GameObject explosionVfxPrefab;         
    public bool destroyBombOnExplode = true;
    public float explosionDamage = 100;

    [Header("Screen Shake")]
    public float shakeDuration = 0.1f;            
    public float shakeIntensity = 0.005f;           

    [Header("Detection")]
    public LayerMask detectionLayers;             
    public bool debugLogTargets = false;

    private Collider2D _myCol;
    

    private void Awake()
    {
        _myCol = GetComponent<Collider2D>();
        
        // --- 【新增】：防止生成时挤开玩家 ---
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Collider2D[] playerCols = player.GetComponentsInChildren<Collider2D>();
            foreach (var pCol in playerCols)
            {
                // 告诉物理引擎：这个炸弹暂时忽略与玩家碰撞体的碰撞
                Physics2D.IgnoreCollision(_myCol, pCol, true);
            }
            
            // 启动协程：等玩家走开了再恢复碰撞
            StartCoroutine(RestoreCollisionWithPlayer(playerCols));
        }
    }

    private void Start()
    {
        StartCoroutine(ExplodeAfterDelay());
    }

    // --- 【新增】：智能恢复碰撞逻辑 ---
    private IEnumerator RestoreCollisionWithPlayer(Collider2D[] playerCols)
    {
        bool stillTouching = true;
        while (stillTouching)
        {
            stillTouching = false;
            foreach (var pCol in playerCols)
            {
                // 检查玩家是否还在踩着炸弹
                if (pCol != null && _myCol != null && pCol.IsTouching(_myCol))
                {
                    stillTouching = true;
                    break;
                }
            }
            yield return new WaitForSeconds(0.1f); // 每0.1秒检查一次，节省性能
        }

        // 玩家已走开，恢复碰撞（现在玩家可以推炸弹了）
        if (_myCol != null)
        {
            foreach (var pCol in playerCols)
            {
                if (pCol != null) Physics2D.IgnoreCollision(_myCol, pCol, false);
            }
        }
    }

    private IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(fuseSeconds);
        Explode();
    }

    private void Explode()
    {
        if(AudioManager.Instance != null)
            AudioManager.Instance.BombExplode();
        if (explosionVfxPrefab != null)
            Instantiate(explosionVfxPrefab, transform.position, Quaternion.identity);

        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, explosionRadius, detectionLayers);
        
        HandleTargets(targets);

        if (CameraManager.Instance != null)
        {
            CameraManager.Instance.Shake(shakeDuration, shakeIntensity);
        }

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

            if (target.TryGetComponent<MonsterBase>(out var monster))
            {
                monster.TakeDamage(explosionDamage, transform.position);
            }
            
            // 使用 TakeDamage 而不是 Destroy，触发你新写的破碎逻辑
            if (target.TryGetComponent<Obstacle>(out var obstacle))
            {
                obstacle.TakeDamage(explosionDamage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}