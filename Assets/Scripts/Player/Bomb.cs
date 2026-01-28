using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Bomb : MonoBehaviour
{
    [Header("Explosion")]
    public float fuseSeconds = 2f;                 // 延时多久爆炸
    public GameObject explosionVfxPrefab;          // 爆炸特效预制体（生成在原地）
    public bool destroyBombOnExplode = true;

    [Header("Detection")]
    public LayerMask ignoreLayers;                 // 忽略的层
    public bool debugLogTargets = false;

    private Collider2D _triggerCol;
    public List<GameObject> _inside = new List<GameObject>();

    private void Awake()
    {
        _triggerCol = GetComponent<Collider2D>();

        // 你说“检测自身碰撞体trigger里面有哪些物体”，所以确保是 Trigger
        if (!_triggerCol.isTrigger)
            _triggerCol.isTrigger = true;
    }

    private void Start()
    {
        StartCoroutine(ExplodeAfterDelay());
    }

    private IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(fuseSeconds);
        Explode();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;
        if (other == _triggerCol) return;

        // 忽略层
        if (IsIgnored(other.gameObject.layer)) return;

        _inside.Add(other.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == null) return;
        _inside.Remove(other.gameObject);
    }

    private bool IsIgnored(int layer)
    {
        return (ignoreLayers.value & (1 << layer)) != 0;
    }

    private void Explode()
    {
        // 1) 生成爆炸特效
        if (explosionVfxPrefab != null)
            Instantiate(explosionVfxPrefab, transform.position, Quaternion.identity);

     
        // 3) 交给后续分类处理
        HandleTargets(_inside);

        // 4) 销毁炸弹
        if (destroyBombOnExplode)
            Destroy(gameObject);
    }

    /// <summary>
    /// 你后面要对检测到的物体进行分类处理，就在这里写。
    /// </summary>
    private void HandleTargets(List<GameObject> _inside)
    {
        if (_inside == null) return;

        for (int i = 0; i < _inside.Count; i++)
        {
            GameObject target = _inside[i];
            if (target == null) continue;

           //player不用处理，因为自己会扣血
            // 1) 如果包含 MonsterBase：扣血 50
            if (target.TryGetComponent<MonsterBase>(out var monster))
            {
                monster.health -= 50;
            }
            if(target.TryGetComponent<Obstacle>(out var obstacle))
            {
                if (obstacle.isDestructible)
                {
                    Destroy(obstacle.gameObject);
                }
            }
            
        }
    }
}
