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
    public static float lastTeleportTime = 0f; 
    public float teleportCooldown = 1.0f;     

    private bool isLocked = false;

    private void Start() {
        // 【新增】：初始化时根据方向调整旋转
        ApplyRotation();
    }

    // --- 新增：处理旋转的逻辑 ---
    public void ApplyRotation() {
        switch (direction) {
            case Dir.North:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case Dir.South:
                transform.rotation = Quaternion.Euler(0, 0, 180);
                break;
            case Dir.East:
                transform.rotation = Quaternion.Euler(0, 0, -90); // 向右转
                break;
            case Dir.West:
                transform.rotation = Quaternion.Euler(0, 0, 90);  // 向左转
                break;
        }
    }

    public void SetLock(bool locked) {
        isLocked = locked;
        if(wallVisual) wallVisual.SetActive(locked);
        if(openVisual) openVisual.SetActive(!locked);
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            if (Time.time - lastTeleportTime < teleportCooldown) return;

            // 检查：房间清空且未锁
            if (!isLocked && myRoom != null && myRoom.state == RoomController.RoomState.Cleared) {
                HandleTeleport(other.transform);
            }
        }
    }

    private void HandleTeleport(Transform player) {
        lastTeleportTime = Time.time;

        Dir oppositeDir = GetOppositeDir(direction);
        Transform spawnPoint = targetRoom.GetSpawnPoint(oppositeDir);

        if (spawnPoint != null) {
            player.position = spawnPoint.position;
        } else {
            player.position = targetRoom.transform.position;
            Debug.LogWarning($"{targetRoom.name} 缺少 {oppositeDir} 方向的 SpawnPoint 锚点！");
        }

        // 切换摄像机
        // 注意：此处引用的是你项目中的 CameraManager
        if (CameraManager.Instance != null) {
            CameraManager.Instance.SwitchToRoom(targetRoom.transform.position);
        }

        targetRoom.ActivateRoom();
        Debug.Log($"<color=green>传送成功：</color> 玩家已进入 {targetRoom.name}");
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