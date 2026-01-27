using UnityEngine;

public class Door : MonoBehaviour {
    public enum Dir { North, South, East, West }
    public Dir direction;
    
    public RoomController myRoom;
    public RoomController targetRoom;
    
    public GameObject wallVisual; 
    public GameObject openVisual; 
    private bool isLocked = false;

    public void SetLock(bool locked) {
        isLocked = locked;
        if(wallVisual) wallVisual.SetActive(locked);
        if(openVisual) openVisual.SetActive(!locked);
    }

    // 使用 Stay 保证你用鼠标拖拽玩家到门口时也能检测到
    private void OnTriggerStay2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            // 只要玩家碰到了，先打印一个日志，证明物理层通了
            // Debug.Log($"[Door] 物理接触成功！当前房间状态: {myRoom.state}, 锁门状态: {isLocked}");

            if (!isLocked && myRoom.state == RoomController.RoomState.Cleared) {
                HandleTeleport(other.transform);
            }
        }
    }

    private void HandleTeleport(Transform player) {
        // 1. 寻找对面那道门
        Dir oppositeDir = GetOppositeDir(direction);
        Vector3 spawnPosition = targetRoom.transform.position; 
        bool foundOppositeDoor = false;

        foreach (Door d in targetRoom.doors) {
            if (d.direction == oppositeDir) {
                spawnPosition = d.transform.position;
                foundOppositeDoor = true;
                break;
            }
        }

        if (!foundOppositeDoor) {
            Debug.LogWarning($"[Door] 在目标房间 {targetRoom.name} 中没找到对应的 {oppositeDir} 门！将传送到房间中心。");
        }

        // 2. 摄像机与位置平移
        CameraManager.Instance.SwitchToRoom(targetRoom.transform.position);
        
        // 传送后稍微向房间内推一点，防止刚过去又传回来
        Vector3 pushOffset = Vector3.zero;
        if (oppositeDir == Dir.North) pushOffset = Vector3.down * 0.6f;
        else if (oppositeDir == Dir.South) pushOffset = Vector3.up * 0.6f;
        else if (oppositeDir == Dir.East) pushOffset = Vector3.left * 0.6f;
        else if (oppositeDir == Dir.West) pushOffset = Vector3.right * 0.6f;
        
        player.position = spawnPosition + pushOffset;

        // 3. 激活新房间
        targetRoom.ActivateRoom();
        Debug.Log($"<color=green>[Teleport Success]</color> 从 {myRoom.name} 传送到 {targetRoom.name}");
    }

    private Dir GetOppositeDir(Dir dir) {
        switch (dir) {
            case Dir.North: return Dir.South;
            case Dir.South: return Dir.North;
            case Dir.East: return Dir.West;
            case Dir.West: return Dir.East;
            default: return Dir.North;
        }
    }
}