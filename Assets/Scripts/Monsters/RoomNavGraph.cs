using UnityEngine;
using System.Collections.Generic;

public class RoomNavGraph : MonoBehaviour {
    public List<Vector2> waypoints = new List<Vector2>();
    private RoomGrid grid;

    [ContextMenu("Bake Waypoints")]
    public void BakeWaypoints() {
        grid = GetComponent<RoomGrid>();
        if (grid == null) return;

        waypoints.Clear();
        // 遍历 13x7 的每一个格子
        for (int x = 0; x < RoomGrid.COLS; x++) {
            for (int y = 0; y < RoomGrid.ROWS; y++) {
                // 如果该格子是空的（没有障碍物），则在正中心生成一个路点
                if (grid.IsEmpty(x, y)) {
                    waypoints.Add(grid.GridToWorld(new Vector2Int(x, y)));
                }
            }
        }
        Debug.Log($"<color=cyan>网格中心路点生成完毕：{waypoints.Count} 个</color>");
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        foreach (var wp in waypoints) Gizmos.DrawSphere(wp, 0.1f);
    }
}