using UnityEngine;

public class PlayerAttack1 : MonoBehaviour
{
   
    
    [Header("References")]
    public Camera mainCamera;
    public Transform firePoint;              // 可选：枪口/发射点（为空就用玩家位置）

    [Header("冲锋枪玩法")]
    public bool isMachineGun;
    public GameObject projectilePrefab1;      // 要生成的物体（子弹prefab）
    public float MachineGunCD;
    public float MachineGunDamage;


    [Header("悲伤泡泡玩法")]
    public bool isSad;
    public GameObject projectilePrefab2;      // 要生成的物体（子弹prefab）
    public float sadGunCD;
    public float sadGunDamage;

    [Header("刀玩法")]
    public bool isBlade;
    public GameObject projectilePrefab3;      // 要生成的物体（子弹prefab）
    public float bladeCD;
    public float bladeDamage;

    [Header("开心波玩法")]
    public bool isHappy;
    public GameObject projectilePrefab4;      // 要生成的物体（子弹prefab）
    public float happyCD;
    public float happyDamage;

    [Header("Spawn")]
    public float spawnDistance = 0.5f;       // 从玩家/枪口往外偏移多少生成
    public bool rotateProjectileToDirection = true; // 让子弹朝向跟方向一致

    [Header("Fire")]
    public KeyCode fireKey = KeyCode.Mouse0; // 鼠标左键
    public float fireCooldown = 0.15f;       // 发射间隔（秒）
    public float damage;
    public GameObject prefab;
    private float _nextFireTime;

    
    
    

    // 调用这个方法就会环形发射6颗
    public void FireRadial6()
    {
       

        Vector3 origin = transform.position;

        int count = 6;
        float step = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float angleDeg = i * step;
            float angleRad = angleDeg * Mathf.Deg2Rad;

            // 方向向量（单位向量）
            Vector2 dir = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)).normalized;

            // 生成位置：中心往外偏移一点，避免和玩家重叠
            Vector3 spawnPos = origin + (Vector3)(dir * spawnDistance);

            // 让子弹朝向对齐（可选）
            Quaternion rot = Quaternion.identity;
            if (rotateProjectileToDirection)
            {
                // 假设子弹prefab的“右方向(+X)”是前进方向
                rot = Quaternion.Euler(0f, 0f, angleDeg);
            }

            GameObject go = Instantiate(prefab, spawnPos, rot);

            // 把方向传给飞行脚本
            var proj = go.GetComponent<ProjectileSimple2D>();
            if (proj != null)
            {
                proj.SetDirection(dir);
            }
        }
    }
    private void Awake()
    {
        if (mainCamera == null) mainCamera = Camera.main;
    }

    private void Update()
    {
        if (isMachineGun)
        {
            damage = MachineGunDamage;
            fireCooldown = MachineGunCD;
            prefab = projectilePrefab1;
        }
        if (isSad)
        {
            damage = sadGunDamage;
            fireCooldown = sadGunCD;
            prefab = projectilePrefab2;
        }
        if (isBlade)
        {
            damage = bladeDamage;
            fireCooldown = bladeCD;
            prefab = projectilePrefab3;
        }
        if (isHappy)
        {
            damage = happyDamage;
            fireCooldown = happyCD;
            prefab = projectilePrefab4;
        }
        if (Input.GetKey(fireKey) && Time.time >= _nextFireTime)
        {
            Fire();
            _nextFireTime = Time.time + fireCooldown;
        }
    }

    private void Fire()
    {
        if (prefab == null || mainCamera == null) return;
        if (isHappy)
        {
            FireRadial6();
            return;
        }
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

        GameObject go = Instantiate(prefab, spawnPos, rot);

        // 如果子弹上有 ProjectileSimple2D，就把方向传过去
        var proj = go.GetComponent<ProjectileSimple2D>();
        if (proj != null)
        {
            proj.SetDirection(dir);
        }
    }
}
