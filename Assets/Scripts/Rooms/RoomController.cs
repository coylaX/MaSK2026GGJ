using UnityEngine;
using System.Collections.Generic;

public class RoomController : MonoBehaviour {
    public enum RoomState { Inactive, Active, Cleared }
    public RoomState state = RoomState.Inactive;

    public List<MonsterBase> monsters = new List<MonsterBase>();
    public List<Door> doors = new List<Door>();

    private RoomNavGraph navGraph;

    [Header("战利品配置")]
    public GameObject assignedLootPrefab; // 由 LootManager 分配
    private bool lootSpawned = false;

    [Header("精英房间设定")]
    public bool isGuaranteedRoom = false; // 是否为必定生成战利品的房间
    private bool buffApplied = false;

    [Header("位置锚点")]
    public Transform doorTop;
    public Transform doorBottom;
    public Transform doorLeft;
    public Transform doorRight;

    [Space]
    public Transform spawnTop;
    public Transform spawnBottom;
    public Transform spawnLeft;
    public Transform spawnRight;

    // 根据方向获取门的位置
    public Transform GetDoorPoint(Door.Dir dir) {
        switch (dir) {
            case Door.Dir.North: return doorTop;
            case Door.Dir.South: return doorBottom;
            case Door.Dir.West:  return doorLeft;
            case Door.Dir.East:  return doorRight;
            default: return null;
        }
    }

    // 根据方向获取玩家生成点
    public Transform GetSpawnPoint(Door.Dir dir) {
        switch (dir) {
            case Door.Dir.North: return spawnTop;
            case Door.Dir.South: return spawnBottom;
            case Door.Dir.West:  return spawnLeft;
            case Door.Dir.East:  return spawnRight;
            default: return null;
        }
    }

    void Awake() {
        navGraph = GetComponent<RoomNavGraph>();
        MonsterBase[] mbs = GetComponentsInChildren<MonsterBase>(true);
        foreach (var m in mbs) {
            monsters.Add(m);
            m.myRoom = this;
            m.gameObject.SetActive(false);
        }
    }

    public void ActivateRoom() {
        // 1. 更新小地图
        if (MiniMapManager.Instance != null) {
            MiniMapManager.Instance.UpdateCurrentRoom(this);
        }

        if (state == RoomState.Cleared || state == RoomState.Active) {
            return; 
        }

        // 2. 激活房间状态
        state = RoomState.Active;

        // 3. 激活所有怪物
        foreach (var m in monsters) {
            if (m != null) m.gameObject.SetActive(true);
        }

        // 【修改后】：遍历房间内所有怪物进行强化
        if (isGuaranteedRoom && !buffApplied && monsters.Count > 0) {
            foreach (var m in monsters) {
                if (m != null) {
                    ApplyRandomBuffToMonster(m);
                }
            }
            buffApplied = true; // 标记已强化，防止重复
            Debug.Log($"{gameObject.name} 是精英房间，已强化该房间内所有怪物（共 {monsters.Count} 个）");
        }

        if (navGraph != null) navGraph.BakeWaypoints();
        CheckDoors();
    }

    private void ApplyRandomBuffToMonster(MonsterBase m) {
        // 随机选择：0-移速, 1-生命, 2-攻击
        int buffIndex = UnityEngine.Random.Range(0, 3);
        m.ApplyEliteBuff(buffIndex);
    }

    public void OnMonsterKilled(MonsterBase m) {
        if (monsters.Contains(m)) monsters.Remove(m);
        if (navGraph != null) navGraph.BakeWaypoints();

        if (monsters.Count <= 0) {
            state = RoomState.Cleared;
            
            // 【核心变更】：怪物清空后尝试生成战利品
            SpawnLoot();
            
            if (MiniMapManager.Instance != null) MiniMapManager.Instance.RefreshMap();
            CheckDoors();
        }
    }

    private void SpawnLoot() {
        if (assignedLootPrefab != null && !lootSpawned) {
            // 获取全局掉落物容器
            Transform parent = LevelManager.Instance != null ? LevelManager.Instance.lootContainer : null;

            // 在房间中心生成并设为 parent 的子物体
            Instantiate(assignedLootPrefab, transform.position, Quaternion.identity, parent);
            
            lootSpawned = true;
            Debug.Log($"{gameObject.name} 战利品已生成至容器。");
        }
    }

    private void CheckDoors() {
        bool shouldLock = (state == RoomState.Active && monsters.Count > 0);
        foreach (var d in doors) d.SetLock(shouldLock);
        
        if (state == RoomState.Active && monsters.Count == 0) {
            state = RoomState.Cleared;
            foreach (var d in doors) d.SetLock(false);
        }
    }
}