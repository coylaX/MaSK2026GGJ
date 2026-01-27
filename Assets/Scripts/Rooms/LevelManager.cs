using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {
    public static LevelManager Instance;

    public GameObject doorPrefab;
    public float roomWidth = 13f;  
    public float roomHeight = 7f;
    public float doorInset = 1f; // 门向内缩进的距离

    private List<RoomController> allRooms = new List<RoomController>();

    void Awake() { Instance = this; }

    [ContextMenu("Initialize Dungeon")]
    // 在 LevelManager.cs 中

    public void InitializeDungeon() {
        allRooms.Clear();
        allRooms.AddRange(FindObjectsOfType<RoomController>());

        // 【修复代码】：直接从 Generator 获取初始房间
        RoomController startingRoom = null;
        if (LevelGenerator.Instance != null) {
            startingRoom = LevelGenerator.Instance.startRoom;
        }

        // 如果还是没找到（保险措施），就按距离找最近的
        if (startingRoom == null) {
            startingRoom = FindClosestRoomToPlayer();
        }

        // 1. 分配战利品 (传入找到的初始房间)
        if (LootManager.Instance != null) {
            LootManager.Instance.DistributeLoot(allRooms, startingRoom);
        }

        // 2. 建立门、初始化小地图
        foreach (var room in allRooms) LinkNeighbors(room);
        
        if (MiniMapManager.Instance != null) {
            MiniMapManager.Instance.GenerateMiniMap(allRooms);
        }

        // 3. 激活初始房间
        if (startingRoom != null) startingRoom.ActivateRoom();
    }

    // 备用方案：通过物理距离找玩家脚下的房间
    private RoomController FindClosestRoomToPlayer() {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return allRooms.Count > 0 ? allRooms[0] : null;

        RoomController closest = null;
        float minDist = float.MaxValue;
        foreach (var room in allRooms) {
            float d = Vector2.Distance(player.transform.position, room.transform.position);
            if (d < minDist) {
                minDist = d;
                closest = room;
            }
        }
        return closest;
    }

    private void LinkNeighbors(RoomController currentRoom) {
        foreach (var otherRoom in allRooms) {
            if (currentRoom == otherRoom) continue;
            Vector3 diff = otherRoom.transform.position - currentRoom.transform.position;

            // 门位置计算：在边缘基础上向内缩进 doorInset
            if (Mathf.Abs(diff.x) < 0.5f && Mathf.Abs(diff.y - roomHeight) < 0.5f)
                SpawnDoor(currentRoom, otherRoom, Door.Dir.North, new Vector3(0, (roomHeight / 2f) - doorInset, 0));
            else if (Mathf.Abs(diff.x) < 0.5f && Mathf.Abs(diff.y + roomHeight) < 0.5f)
                SpawnDoor(currentRoom, otherRoom, Door.Dir.South, new Vector3(0, -(roomHeight / 2f) + doorInset, 0));
            else if (Mathf.Abs(diff.y) < 0.5f && Mathf.Abs(diff.x - roomWidth) < 0.5f)
                SpawnDoor(currentRoom, otherRoom, Door.Dir.East, new Vector3((roomWidth / 2f) - doorInset, 0, 0));
            else if (Mathf.Abs(diff.y) < 0.5f && Mathf.Abs(diff.x + roomWidth) < 0.5f)
                SpawnDoor(currentRoom, otherRoom, Door.Dir.West, new Vector3(-(roomWidth / 2f) + doorInset, 0, 0));
        }
    }

    private void SpawnDoor(RoomController from, RoomController to, Door.Dir dir, Vector3 localPos) {
        GameObject dObj = Instantiate(doorPrefab, from.transform);
        dObj.transform.localPosition = localPos;
        dObj.name = $"Door_{dir}_To_{to.gameObject.name}";

        Door door = dObj.GetComponent<Door>();
        door.direction = dir;
        door.myRoom = from;
        door.targetRoom = to;
        
        from.doors.Add(door);
        door.SetLock(false); 
    }

    private void ActivateStartingRoom() {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;
        RoomController closest = null;
        float minDist = float.MaxValue;
        foreach (var room in allRooms) {
            float d = Vector2.Distance(player.transform.position, room.transform.position);
            if (d < minDist) { minDist = d; closest = room; }
        }
        if (closest != null) closest.ActivateRoom();
    }
}