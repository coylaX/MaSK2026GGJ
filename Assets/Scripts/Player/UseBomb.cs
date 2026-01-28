using UnityEngine;

public class UseBomb : MonoBehaviour
{
    public int bombNum;
   
    [Header("Prefab")]
    public GameObject bombPrefab;

    [Header("Input")]
    public KeyCode bombKey = KeyCode.Mouse1; // Êó±êÓÒ¼ü

    [Header("Optional")]
    public float cooldown = 0.3f;
    private float _nextTime;

    private void OnEnable()
    {
        LootEvents.OnBombPicked += StoreBomb;
    }

    public void StoreBomb()
    {
        bombNum++;
   
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
    }
}
