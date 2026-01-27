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
    public void InitializeDungeon() {
        allRooms.Clear();
        allRooms.AddRange(FindObjectsOfType<RoomController>());

        // 清理旧门
        foreach (var room in allRooms) {
            foreach (var d in room.doors) if(d != null) DestroyImmediate(d.gameObject);
            room.doors.Clear();
        }

        // 生成门并建立邻居关系
        foreach (var room in allRooms) {
            LinkNeighbors(room);
        }
        
        ActivateStartingRoom();
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