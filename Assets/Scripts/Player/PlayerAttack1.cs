using UnityEngine;

public class PlayerAttack1 : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;
    public Transform firePoint;              // 可选：枪口/发射点（为空就用玩家位置）
    public GameObject projectilePrefab;      // 要生成的物体（子弹prefab）

    [Header("Spawn")]
    public float spawnDistance = 0.5f;       // 从玩家/枪口往外偏移多少生成
    public bool rotateProjectileToDirection = true; // 让子弹朝向跟方向一致

    [Header("Fire")]
    public KeyCode fireKey = KeyCode.Mouse0; // 鼠标左键
    public float fireCooldown = 0.15f;       // 发射间隔（秒）

    private float _nextFireTime;

    private void Awake()
    {
        if (mainCamera == null) mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKey(fireKey) && Time.time >= _nextFireTime)
        {
            Fire();
            _nextFireTime = Time.time + fireCooldown;
        }
    }

    private void Fire()
    {
        if (projectilePrefab == null || mainCamera == null) return;

        Vector3 origin = (firePoint != null) ? firePoint.position : transform.position;

        // 2D：鼠标屏幕坐标 -> 世界坐标
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = origin.z; // 保持同一平面（2D）

        Vector2 dir = (mouseWorld - origin);
        if (dir.sqrMagnitude < 0.0001f) return; // 防止方向为0
        dir.Normalize();

        Vector3 spawnPos = origin + (Vector3)(dir * spawnDistance);

        Quaternion rot = Quaternion.identity;
        if (rotateProjectileToDirection)
        {
            // 假设子弹 prefab 的“右方向（+X）”是前进方向
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            rot = Quaternion.Euler(0f, 0f, angle);
        }

        GameObject go = Instantiate(projectilePrefab, spawnPos, rot);

        // 如果子弹上有 ProjectileSimple2D，就把方向传过去
        var proj = go.GetComponent<ProjectileSimple2D>();
        if (proj != null)
        {
            proj.SetDirection(dir);
        }
    }
}
