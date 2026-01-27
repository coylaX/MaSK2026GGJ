using UnityEngine;
using System.Collections.Generic;

public class RoomController : MonoBehaviour {
    public enum RoomState { Inactive, Active, Cleared }
    public RoomState state = RoomState.Inactive;

    public List<MonsterBase> monsters = new List<MonsterBase>();
    public List<Door> doors = new List<Door>();

    void Awake() {
        MonsterBase[] mbs = GetComponentsInChildren<MonsterBase>(true);
        foreach (var m in mbs) {
            monsters.Add(m);
            m.myRoom = this;
            m.gameObject.SetActive(false);
        }
    }

    // 在 RoomController 类中修改 ActivateRoom 方法
    public void ActivateRoom() {
        if (state == RoomState.Cleared || state == RoomState.Active) return;

        state = RoomState.Active;
        Debug.Log($"激活房间: {gameObject.name}");

        // 唤醒怪物
        foreach (var m in monsters) {
            if (m != null) m.gameObject.SetActive(true);
        }

        // 检查是否需要关门（如果有怪就关，没怪就直接清空）
        CheckDoors();
    }

    private void CheckDoors() {
        // 只有在 Active 状态且有怪物时才真正“锁死”门
        bool needsLock = (state == RoomState.Active && monsters.Count > 0);
        foreach (var d in doors) {
            d.SetLock(needsLock);
        }
        
        // 如果激活时发现没怪，直接转为清空状态
        if (state == RoomState.Active && monsters.Count == 0) {
            state = RoomState.Cleared;
            foreach (var d in doors) d.SetLock(false);
        }
    }

    // 在 RoomController.cs 中修改 OnMonsterKilled
    public void OnMonsterKilled(MonsterBase m) {
        if (monsters.Contains(m)) monsters.Remove(m);
        
        if (monsters.Count <= 0) {
            state = RoomState.Cleared; // 必须先设为 Cleared
            Debug.Log($"{gameObject.name} 已清空！");
            CheckDoors(); // 然后开门
        }
    }

}