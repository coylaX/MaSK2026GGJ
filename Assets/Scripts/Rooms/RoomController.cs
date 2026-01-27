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
        if (state == RoomState.Cleared || state == RoomState.Active) return;

        state = RoomState.Active;
        if (MiniMapManager.Instance != null) MiniMapManager.Instance.UpdateCurrentRoom(this);

        // 唤醒怪物
        foreach (var m in monsters) {
            if (m != null) {
                m.gameObject.SetActive(true);
                // 【新功能】：如果是保底房间，激活时施加增益
                if (isGuaranteedRoom && !buffApplied) {
                    ApplyRandomBuffToMonster(m);
                }
            }
        }
        buffApplied = true;

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