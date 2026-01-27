using UnityEngine;
using System.Collections.Generic;

public class RoomController : MonoBehaviour {
    public enum RoomState { Inactive, Active, Cleared }
    public RoomState state = RoomState.Inactive;

    public List<MonsterBase> monsters = new List<MonsterBase>();
    public List<Door> doors = new List<Door>();

    private RoomNavGraph navGraph;

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

        // 1. 先唤醒怪物
        foreach (var m in monsters) if (m != null) m.gameObject.SetActive(true);

        // 【功能 2】：房间激活时立即 Baking 一次，此时会排除掉刚醒来的 Shooter 占据的格子
        if (navGraph != null) navGraph.BakeWaypoints();

        CheckDoors();
    }

    public void OnMonsterKilled(MonsterBase m) {
        if (monsters.Contains(m)) monsters.Remove(m);
        
        // 【功能 1】：怪物死亡时重新 Baking。
        // 这会释放该怪物之前占据的格子（黄点会重新出现），并更新寻路预处理。
        if (navGraph != null) navGraph.BakeWaypoints();

        if (monsters.Count <= 0) {
            state = RoomState.Cleared;
            CheckDoors();
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