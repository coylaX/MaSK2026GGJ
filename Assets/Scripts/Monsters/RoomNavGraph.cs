using UnityEngine;
using System.Collections.Generic;

public class RoomNavGraph : MonoBehaviour {
    [Header("数据存储")]
    public List<Vector3> localWaypoints = new List<Vector3>(); 
    private RoomGrid grid;

    [ContextMenu("Bake Waypoints (Local)")]
    public void BakeWaypoints() {
        grid = GetComponent<RoomGrid>();
        if (grid == null) return;

        // 1. 彻底清空旧路点
        localWaypoints.Clear();

        // 2. 【修复 CS0841】首先定义并初始化占用地图
        bool[,] occupancyMap = new bool[RoomGrid.COLS, RoomGrid.ROWS];
        for (int x = 0; x < RoomGrid.COLS; x++) {
            for (int y = 0; y < RoomGrid.ROWS; y++) {
                occupancyMap[x, y] = true; // 默认全部可行走
            }
        }

        // 3. 【修复 CS0841】首先获取所有子物体
        Transform[] allChildren = GetComponentsInChildren<Transform>(false); 

        // 4. 执行扫描逻辑
        foreach (Transform child in allChildren) {
            // 跳过根物体或已禁用的物体（配合 MonsterBase.Die 中的 SetActive(false)）
            if (child == transform || !child.gameObject.activeInHierarchy) continue;

            // 检查物理障碍层
            bool isObstacle = ((1 << child.gameObject.layer) & grid.obstacleLayer.value) != 0;
            
            // 检查特定怪物（Shooter 等）
            MonsterBase monster = child.GetComponent<MonsterBase>();
            bool isMonsterBlocking = monster != null && 
                                     monster.blocksPathfinding && 
                                     monster.health > 0;

            if (isObstacle || isMonsterBlocking) {
                Vector2Int gridCoord = grid.WorldToGrid(child.position);
                // 确保坐标在网格范围内
                if (gridCoord.x >= 0 && gridCoord.x < RoomGrid.COLS && 
                    gridCoord.y >= 0 && gridCoord.y < RoomGrid.ROWS) {
                    occupancyMap[gridCoord.x, gridCoord.y] = false;
                }
            }
        }

        // 5. 将结果转换为局部坐标路径点
        for (int x = 0; x < RoomGrid.COLS; x++) {
            for (int y = 0; y < RoomGrid.ROWS; y++) {
                if (occupancyMap[x, y]) {
                    localWaypoints.Add(grid.GridToLocalPos(x, y));
                }
            }
        }
    }

    private void OnDrawGizmosSelected() {
        if (grid == null) grid = GetComponent<RoomGrid>();
        Gizmos.color = Color.yellow;
        foreach (var lwp in localWaypoints) {
            Gizmos.DrawSphere(transform.TransformPoint(lwp), 0.1f);
        }
    }
}