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
        // 【核心修复】：只要玩家进入房间，无论房间是什么状态，都必须先更新小地图
        if (MiniMapManager.Instance != null) {
            MiniMapManager.Instance.UpdateCurrentRoom(this);
        }

        // 如果房间已经激活过或已清空，后续的怪物生成逻辑才跳过
        if (state == RoomState.Cleared || state == RoomState.Active) {
            return; 
        }

        // 第一次进入未探索房间的逻辑...
        state = RoomState.Active;
        foreach (var m in monsters) if (m != null) m.gameObject.SetActive(true);
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
            // 在房间正中央生成 (房间根物体的坐标)
            Instantiate(assignedLootPrefab, transform.position, Quaternion.identity);
            lootSpawned = true;
            Debug.Log($"{gameObject.name} 怪物已清空，战利品已出现！");
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