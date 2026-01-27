using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {
    [Header("核心房间配置")]
    public GameObject startRoomPrefab;      // 必须存在的初始房间
    public List<GameObject> roomPool;       // 随机房间池
    
    [Header("生成数量")]
    [Range(10, 12)]
    public int additionalRoomCount = 10;    // 除去初始房间外，额外生成的数量

    [Header("布局设置")]
    public Vector2 roomSpacing = new Vector2(14f, 8f); // 根据7*13建议设置

    private HashSet<Vector2Int> occupiedCoords = new HashSet<Vector2Int>();
    private List<GameObject> spawnedRooms = new List<GameObject>();

    void Start() {
        GenerateLevel();
    }

    [ContextMenu("Generate New Level")] // 允许你在编辑器模式下右键脚本测试
    public void GenerateLevel() {
        ClearLevel();
        
        // 1. 放置初始房间（Starter Room）
        Vector2Int startCoord = Vector2Int.zero;
        GameObject startRoom = PlaceRoom(startRoomPrefab, startCoord);
        startRoom.name = "== START_ROOM ==";
        // 给初始房间一个特殊颜色或标记以便调试
        // startRoom.GetComponentInChildren<SpriteRenderer>().color = Color.green;

        // 2. 确定本次生成的总数 (10-12)
        int targetCount = additionalRoomCount; 

        // 3. 递归/循环寻找邻居放置后续房间
        for (int i = 0; i < targetCount; i++) {
            Vector2Int nextCoord = GetRandomNeighborCoord();
            if (nextCoord == Vector2Int.zero && i > 0) break; // 安全检查

            GameObject randomPrefab = roomPool[Random.Range(0, roomPool.Count)];
            PlaceRoom(randomPrefab, nextCoord);
        }
    }

    private Vector2Int GetRandomNeighborCoord() {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        // 遍历所有已占用的格子，寻找它们周围的空位
        foreach (var coord in occupiedCoords) {
            foreach (var dir in directions) {
                Vector2Int potential = coord + dir;
                if (!occupiedCoords.Contains(potential)) {
                    neighbors.Add(potential);
                }
            }
        }
        
        if (neighbors.Count == 0) return Vector2Int.zero;
        return neighbors[Random.Range(0, neighbors.Count)];
    }

    private GameObject PlaceRoom(GameObject prefab, Vector2Int coord) {
        Vector3 worldPos = new Vector3(coord.x * roomSpacing.x, coord.y * roomSpacing.y, 0);
        GameObject room = Instantiate(prefab, worldPos, Quaternion.identity, transform);
        
        occupiedCoords.Add(coord);
        spawnedRooms.Add(room);
        return room;
    }

    public void ClearLevel() {
        // 确保在编辑器和运行时都能干净清理
        if (spawnedRooms.Count > 0) {
            foreach (var room in spawnedRooms) {
                if (Application.isPlaying) Destroy(room);
                else DestroyImmediate(room);
            }
        }
        spawnedRooms.Clear();
        occupiedCoords.Clear();
    }
}