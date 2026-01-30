using UnityEngine;
using System; // 必须引用 System 命名空间以使用 Action

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
        if (bombPrefab == null)
        {
            Debug.LogWarning("[UseBomb] bombPrefab is null.");
            return;
        }
        if (bombNum <= 0)
        {
            return;
        }
        Instantiate(bombPrefab, transform.position, Quaternion.identity);
        bombNum--;
        
        // --- 发送更新事件 ---
        OnBombCountChanged?.Invoke(bombNum);
    }
}