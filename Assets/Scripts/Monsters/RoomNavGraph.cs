using UnityEngine;
using System.Collections.Generic;

public class RoomNavGraph : MonoBehaviour {
    public List<Vector3> localWaypoints = new List<Vector3>(); 
    private RoomGrid grid;

    [ContextMenu("Bake Waypoints (Local)")]
    public void BakeWaypoints() {
        grid = GetComponent<RoomGrid>();
        if (grid == null) return;

        localWaypoints.Clear();

        // 1. 定义一个 13x7 的占用地图，初始全部为空地 (true)
        bool[,] occupancyMap = new bool[RoomGrid.COLS, RoomGrid.ROWS];
        for (int x = 0; x < RoomGrid.COLS; x++) {
            for (int y = 0; y < RoomGrid.ROWS; y++) {
                occupancyMap[x, y] = true;
            }
        }

        // 2. 遍历所有子物体
        // GetComponentsInChildren<Transform>(true) 会包含隐藏的物体
        Transform[] allChildren = GetComponentsInChildren<Transform>(true);
        int obstacleDetected = 0;

        foreach (Transform child in allChildren) {
            // 跳过房间根物体本身
            if (child == transform) continue;

            // 检查子物体的 Layer 是否属于 obstacleLayer
            if (((1 << child.gameObject.layer) & grid.obstacleLayer.value) != 0) {
                // 将物体的世界坐标转换成格子索引
                Vector2Int gridCoord = grid.WorldToGrid(child.position);
                
                // 标记该格子为阻挡 (false)
                if (occupancyMap[gridCoord.x, gridCoord.y]) {
                    occupancyMap[gridCoord.x, gridCoord.y] = false;
                    obstacleDetected++;
                }
            }
        }

        // 3. 根据扫描结果生成路点
        for (int x = 0; x < RoomGrid.COLS; x++) {
            for (int y = 0; y < RoomGrid.ROWS; y++) {
                if (occupancyMap[x, y]) {
                    localWaypoints.Add(grid.GridToLocalPos(x, y));
                }
            }
        }

        Debug.Log($"<color=cyan>[Bake Report]</color> 房间: {gameObject.name}, 扫描到障碍物物体: {obstacleDetected} 个格子, 剩余空地: {localWaypoints.Count}");
    }

    private void OnDrawGizmosSelected() {
        if (grid == null) grid = GetComponent<RoomGrid>();
        
        // 画出路点
        Gizmos.color = Color.yellow;
        foreach (var lwp in localWaypoints) {
            Gizmos.DrawSphere(transform.TransformPoint(lwp), 0.1f);
        }
    }
}