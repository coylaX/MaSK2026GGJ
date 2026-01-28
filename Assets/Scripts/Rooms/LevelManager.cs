using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {
    public static LevelManager Instance;

    public GameObject doorPrefab;
    // 这里的 roomWidth/Height 仅用于判断邻居关系，不参与门的位置计算
    public float roomWidth = 14f;  
    public float roomHeight = 8f;

    private List<RoomController> allRooms = new List<RoomController>();

    [Header("UI 自动管理")]
    public List<GameObject> combatUIs; // 拖入需要开局显示的 UI

    void Awake() { Instance = this; }

    public void InitializeDungeon() {
        allRooms.Clear();
        allRooms.AddRange(FindObjectsOfType<RoomController>());

        RoomController startingRoom = null;
        if (LevelGenerator.Instance != null) {
            startingRoom = LevelGenerator.Instance.startRoom;
        }

        if (startingRoom == null) startingRoom = FindClosestRoomToPlayer();

        if (LootManager.Instance != null) {
            LootManager.Instance.DistributeLoot(allRooms, startingRoom);
        }

        // 2. 建立门
        foreach (var room in allRooms) LinkNeighbors(room);
        
        if (MiniMapManager.Instance != null) {
            MiniMapManager.Instance.GenerateMiniMap(allRooms);
        }

        if (startingRoom != null) startingRoom.ActivateRoom();

        // 【新增】：关卡开始时激活所有战斗 UI
        ShowCombatUIs(true);

        if (startingRoom != null) startingRoom.ActivateRoom();
    }

    public void ShowCombatUIs(bool show) {
        foreach (GameObject ui in combatUIs) {
            if (ui != null) ui.SetActive(show);
        }
    }

    private void LinkNeighbors(RoomController currentRoom) {
        foreach (var otherRoom in allRooms) {
            if (currentRoom == otherRoom) continue;
            Vector3 diff = otherRoom.transform.position - currentRoom.transform.position;

            // 【重构逻辑】：检测邻居方向，不再手动传入 Vector3 坐标
            if (Mathf.Abs(diff.x) < 0.5f && Mathf.Abs(diff.y - roomHeight) < 0.5f)
                SpawnDoor(currentRoom, otherRoom, Door.Dir.North);
            else if (Mathf.Abs(diff.x) < 0.5f && Mathf.Abs(diff.y + roomHeight) < 0.5f)
                SpawnDoor(currentRoom, otherRoom, Door.Dir.South);
            else if (Mathf.Abs(diff.y) < 0.5f && Mathf.Abs(diff.x - roomWidth) < 0.5f)
                SpawnDoor(currentRoom, otherRoom, Door.Dir.East);
            else if (Mathf.Abs(diff.y) < 0.5f && Mathf.Abs(diff.x + roomWidth) < 0.5f)
                SpawnDoor(currentRoom, otherRoom, Door.Dir.West);
        }
    }

    private void SpawnDoor(RoomController from, RoomController to, Door.Dir dir) {
        // 【核心】：根据方向从 RoomController 获取锚点位置
        Transform anchor = from.GetDoorPoint(dir);
        if (anchor == null) {
            Debug.LogWarning($"{from.name} 缺少方向为 {dir} 的门锚点！");
            return;
        }

        // 在锚点的位置和旋转下生成门
        GameObject dObj = Instantiate(doorPrefab, anchor.position, anchor.rotation, from.transform);
        dObj.name = $"Door_{dir}_To_{to.gameObject.name}";

        Door door = dObj.GetComponent<Door>();
        door.direction = dir;
        door.myRoom = from;
        door.targetRoom = to;
        
        from.doors.Add(door);
        door.SetLock(false); 
    }

    // 【新增】：当玩家触发传送时调用此方法
    public void MovePlayerToRoom(RoomController nextRoom, Door.Dir entryDirection) {
        // 如果玩家走进 North 门，意味着他进入下个房间的 South 位置
        Door.Dir spawnSide = GetOppositeDirection(entryDirection);
        Transform spawnPoint = nextRoom.GetSpawnPoint(spawnSide);

        if (spawnPoint != null) {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) {
                player.transform.position = spawnPoint.position;
                // 同时将相机移动到新房间中心
                Camera.main.transform.position = new Vector3(nextRoom.transform.position.x, nextRoom.transform.position.y, -10);
            }
        }
        nextRoom.ActivateRoom();
    }

    private Door.Dir GetOppositeDirection(Door.Dir dir) {
        switch (dir) {
            case Door.Dir.North: return Door.Dir.South;
            case Door.Dir.South: return Door.Dir.North;
            case Door.Dir.East:  return Door.Dir.West;
            case Door.Dir.West:  return Door.Dir.East;
            default: return dir;
        }
    }

    private RoomController FindClosestRoomToPlayer() {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return allRooms.Count > 0 ? allRooms[0] : null;
        RoomController closest = null;
        float minDist = float.MaxValue;
        foreach (var room in allRooms) {
            float d = Vector2.Distance(player.transform.position, room.transform.position);
            if (d < minDist) { minDist = d; closest = room; }
        }
        return closest;
    }
}