using UnityEngine;
using System; // 必须引用 System 命名空间以使用 Action
using System.Collections;         
using System.Collections.Generic;

public class UseBomb : MonoBehaviour
{
    // --- 新增：炸弹数量改变的事件 ---
    public static Action<int> OnBombCountChanged;

    public int bombNum;
   
    [Header("Prefab")]
    public GameObject bombPrefab;

    [Header("Input")]
    public KeyCode bombKey = KeyCode.Mouse1; 

    [Header("Optional")]
    public float cooldown = 0.3f;
    private float _nextTime;

    private void OnEnable()
    {
        LootEvents.OnBombPicked += StoreBomb;
    }

    private void OnDisable()
    {
        // 良好的习惯：在 Disable 时取消订阅
        LootEvents.OnBombPicked -= StoreBomb;
    }

    // 可以在 Start 中发送一次初始数量，确保 UI 初始化正确
    private void Start()
    {
        OnBombCountChanged?.Invoke(bombNum);
    }

    public void StoreBomb()
    {
        bombNum++;
        // --- 发送更新事件 ---
        OnBombCountChanged?.Invoke(bombNum);
    }

    private void Update()
    {
        if (Input.GetKeyDown(bombKey) && Time.time >= _nextTime)
        {
            SpawnBomb();
            _nextTime = Time.time + cooldown;
        }
    }

    private void SpawnBomb()
    {
        if (bombPrefab == null || bombNum <= 0) return;

        // 1. 生成炸弹
        GameObject bombObj = Instantiate(bombPrefab, transform.position, Quaternion.identity);
        bombNum--;
        OnBombCountChanged?.Invoke(bombNum);

        // 2. 【核心优化】：获取玩家及其子物体上所有的碰撞体
        Collider2D[] playerColliders = GetComponentsInChildren<Collider2D>();
        Collider2D bombCollider = bombObj.GetComponent<Collider2D>();

        if (bombCollider != null && playerColliders.Length > 0)
        {
            // 3. 立即忽略所有碰撞
            foreach (var pCol in playerColliders)
            {
                Physics2D.IgnoreCollision(pCol, bombCollider, true);
            }

            // 4. 【关键】：强制物理引擎同步变换，防止“第一帧排斥”
            Physics2D.SyncTransforms();

            // 5. 启动智能恢复协程
            StartCoroutine(RestoreCollisionWhenClear(playerColliders, bombCollider));
        }
    }

    private IEnumerator RestoreCollisionWhenClear(Collider2D[] pCols, Collider2D bCol)
    {
        // 只要有一个碰撞体还在接触炸弹，就继续等待
        bool isStillTouching = true;
        while (isStillTouching)
        {
            if (bCol == null) yield break; // 炸弹炸了就停止

            isStillTouching = false;
            foreach (var pCol in pCols)
            {
                if (pCol != null && pCol.IsTouching(bCol))
                {
                    isStillTouching = true;
                    break;
                }
            }
            yield return null; 
        }

        // 走开了，全部恢复碰撞
        if (bCol != null)
        {
            foreach (var pCol in pCols)
            {
                if (pCol != null) Physics2D.IgnoreCollision(pCol, bCol, false);
            }
            // Debug.Log("玩家已离开炸弹，实体化成功！");
        }
    }

    // 在 UseBomb.cs 中添加
    public void AddBombs(int amount)
    {
        bombNum += amount;
        // 触发事件通知 UI 更新
        OnBombCountChanged?.Invoke(bombNum);
        Debug.Log($"<color=yellow>[炸弹]</color> 增加了 {amount} 个，当前总量: {bombNum}");
    }
    
}