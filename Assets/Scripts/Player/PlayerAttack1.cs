using UnityEngine;

public class PlayerAttack1 : MonoBehaviour
{
   
    
    [Header("References")]
    public Camera mainCamera;
    public Transform firePoint;              // ��ѡ��ǹ��/����㣨Ϊ�վ������λ�ã�

    [Header("���ǹ�淨")]
    public bool isMachineGun;
    public GameObject projectilePrefab1;      // Ҫ���ɵ����壨�ӵ�prefab��
    public float MachineGunCD;
    public float MachineGunDamage;


    [Header("���������淨")]
    public bool isSad;
    public GameObject projectilePrefab2;      // Ҫ���ɵ����壨�ӵ�prefab��
    public float sadGunCD;
    public float sadGunDamage;

    [Header("���淨")]
    public bool isBlade;
    public GameObject projectilePrefab3;      // Ҫ���ɵ����壨�ӵ�prefab��
    public float bladeCD;
    public float bladeDamage;

    [Header("���Ĳ��淨")]
    public bool isHappy;
    public GameObject projectilePrefab4;      // Ҫ���ɵ����壨�ӵ�prefab��
    public float happyCD;
    public float happyDamage;

    [Header("Spawn")]
    public float spawnDistance = 0.5f;       // �����/ǹ������ƫ�ƶ�������
    public bool rotateProjectileToDirection = true; // ���ӵ����������һ��

    [Header("Fire")]
    public KeyCode fireKey = KeyCode.Mouse0; // ������
    public float fireCooldown = 0.15f;       // ���������룩
    public float damage;
    public GameObject prefab;
    private float _nextFireTime;

    
    
    

    // ������������ͻỷ�η���6��
    public void FireRadial6()
    {
       

        Vector3 origin = transform.position;

        int count = 6;
        float step = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float angleDeg = i * step;
            float angleRad = angleDeg * Mathf.Deg2Rad;

            // ������������λ������
            Vector2 dir = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)).normalized;

            // ����λ�ã���������ƫ��һ�㣬���������ص�
            Vector3 spawnPos = origin + (Vector3)(dir * spawnDistance);

            // ���ӵ�������루��ѡ��
            Quaternion rot = Quaternion.identity;
            if (rotateProjectileToDirection)
            {
                // �����ӵ�prefab�ġ��ҷ���(+X)����ǰ������
                rot = Quaternion.Euler(0f, 0f, angleDeg);
            }

            GameObject go = Instantiate(prefab, spawnPos, rot);

            // �ѷ��򴫸����нű�
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

        if(isMachineGun){
            if( AudioManager.Instance != null )
                AudioManager.Instance.PlayRemoteShoot2();
        }
        else if(isSad){
            if( AudioManager.Instance != null )
                AudioManager.Instance.PlayRemoteShoot3();
        }
        else if(isBlade){
            if( AudioManager.Instance != null )
                AudioManager.Instance.PlayMeleeSwing();
        }
        else if(isHappy){
            if( AudioManager.Instance != null )
                AudioManager.Instance.PlayRemoteShoot1();
        }

        if (isHappy)
        {
            FireRadial6();
            return;
        }
        Vector3 origin = (firePoint != null) ? firePoint.position : transform.position;

        // 2D�������Ļ���� -> ��������
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = origin.z; // ����ͬһƽ�棨2D��

        Vector2 dir = (mouseWorld - origin);
        if (dir.sqrMagnitude < 0.0001f) return; // ��ֹ����Ϊ0
        dir.Normalize();

        Vector3 spawnPos = origin + (Vector3)(dir * spawnDistance);

        Quaternion rot = Quaternion.identity;
        if (rotateProjectileToDirection)
        {
            // �����ӵ� prefab �ġ��ҷ���+X������ǰ������
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            rot = Quaternion.Euler(0f, 0f, angle);
        }

        GameObject go = Instantiate(prefab, spawnPos, rot);

        // ����ӵ����� ProjectileSimple2D���Ͱѷ��򴫹�ȥ
        var proj = go.GetComponent<ProjectileSimple2D>();
        if (proj != null)
        {
            proj.SetDirection(dir);
        }
    }
}
