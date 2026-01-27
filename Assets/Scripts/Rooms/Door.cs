using UnityEngine;

public class Door : MonoBehaviour {
    public enum Dir { North, South, East, West }
    public Dir direction;
    
    [Header("关联引用")]
    public RoomController myRoom;
    public RoomController targetRoom;
    public GameObject wallVisual; 
    public GameObject openVisual; 

    [Header("传送配置")]
    public static float lastTeleportTime = 0f; // 全局静态冷却计时
    public float teleportCooldown = 1.0f;     // 冷却时长
    public float exitPushDist = 0f;         // 进门后向内推进的距离（建议 > 1.2）

    private bool isLocked = false;

    public void SetLock(bool locked) {
        isLocked = locked;
        if(wallVisual) wallVisual.SetActive(locked);
        if(openVisual) openVisual.SetActive(!locked);
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            // 1. 时间冷却检查
            if (Time.time - lastTeleportTime < teleportCooldown) return;

            // 2. 状态检查：房间清空且未锁
            if (!isLocked && myRoom.state == RoomController.RoomState.Cleared) {
                HandleTeleport(other.transform);
            }
        }
    }

    private void HandleTeleport(Transform player) {
        // 更新全局冷却时间
        lastTeleportTime = Time.time;

        // 1. 寻找对面那道门作为基准坐标
        Dir oppositeDir = GetOppositeDir(direction);
        Vector3 spawnBasePos = targetRoom.transform.position; 
        bool foundDoor = false;

        foreach (Door d in targetRoom.doors) {
            if (d.direction == oppositeDir) {
                spawnBasePos = d.transform.position;
                foundDoor = true;
                break;
            }
        }

        // 2. 切换摄像机
        if (CameraManager.Instance != null) {
            CameraManager.Instance.SwitchToRoom(targetRoom.transform.position);
        }

        // 3. 计算“向内推”的偏移向量
        // 如果我们进入的是目标房间的南门，我们就向上(Up)推
        Vector3 pushVector = Vector3.zero;
        switch (oppositeDir) {
            case Dir.North: pushVector = Vector3.down * exitPushDist; break;
            case Dir.South: pushVector = Vector3.up * exitPushDist; break;
            case Dir.East:  pushVector = Vector3.left * exitPushDist; break;
            case Dir.West:  pushVector = Vector3.right * exitPushDist; break;
        }

        // 4. 执行传送
        player.position = spawnBasePos + pushVector;

        // 5. 激活目标房间
        targetRoom.ActivateRoom();
        
        Debug.Log($"<color=green>传送成功：</color> 玩家已进入 {targetRoom.name}，并向内推进了 {exitPushDist} 单位");
    }

    private Dir GetOppositeDir(Dir dir) {
        switch (dir) {
            case Dir.North: return Dir.South;
            case Dir.South: return Dir.North;
            case Dir.East:  return Dir.West;
            case Dir.West:  return Dir.East;
            default: return Dir.North;
        }
    }
}