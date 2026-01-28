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
    // 【优化】：不再需要 exitPushDist，因为我们在 Prefab 里手动摆放了 SpawnPoint

    private bool isLocked = false;

    public void SetLock(bool locked) {
        isLocked = locked;
        if(wallVisual) wallVisual.SetActive(locked);
        if(openVisual) openVisual.SetActive(!locked);
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            if (Time.time - lastTeleportTime < teleportCooldown) return;

            // 检查：房间清空且未锁
            if (!isLocked && myRoom.state == RoomController.RoomState.Cleared) {
                HandleTeleport(other.transform);
            }
        }
    }

    private void HandleTeleport(Transform player) {
        lastTeleportTime = Time.time;

        // 1. 获取对向方位
        // 如果玩家走出当前房间的北门(North)，他将出现在目标房间的南边(South)
        Dir oppositeDir = GetOppositeDir(direction);

        // 2. 【核心修改】：直接向目标房间索取对应的出生锚点
        // 这一步取代了原来寻找对面 Door 物体以及计算 pushVector 的过程
        Transform spawnPoint = targetRoom.GetSpawnPoint(oppositeDir);

        if (spawnPoint != null) {
            // 执行传送：位置精确对准我们在 Prefab 里摆好的点
            player.position = spawnPoint.position;
        } else {
            // 保险逻辑：如果忘记配置锚点，则回退到目标房间中心
            player.position = targetRoom.transform.position;
            Debug.LogWarning($"{targetRoom.name} 缺少 {oppositeDir} 方向的 SpawnPoint 锚点！");
        }

        // 3. 【保留原逻辑】：切换摄像机
        // 传入目标房间的世界坐标中心，摄像机逻辑完全不受影响
        if (CameraManager.Instance != null) {
            CameraManager.Instance.SwitchToRoom(targetRoom.transform.position);
        }

        // 4. 【保留原逻辑】：激活目标房间
        targetRoom.ActivateRoom();
        
        Debug.Log($"<color=green>传送成功：</color> 玩家已进入 {targetRoom.name}，对准了 {oppositeDir} 锚点");
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