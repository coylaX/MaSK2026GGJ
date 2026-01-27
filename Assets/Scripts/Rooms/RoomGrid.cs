using UnityEngine;

public class RoomGrid : MonoBehaviour {
    public const int ROWS = 7;
    public const int COLS = 13;
    public float cellSize = 1f;
    public LayerMask obstacleLayer;

    public Vector3 GetLocalOrigin() => new Vector3(-(COLS * cellSize) / 2f, -(ROWS * cellSize) / 2f, 0);

    // 格子坐标 -> 局部坐标
    public Vector3 GridToLocalPos(int col, int row) {
        Vector3 origin = GetLocalOrigin();
        return new Vector3(origin.x + (col * cellSize) + (cellSize / 2f),
                           origin.y + (row * cellSize) + (cellSize / 2f), 0);
    }

    // 格子坐标 -> 世界坐标
    public Vector2 GridToWorld(Vector2Int gridPos) {
        return transform.TransformPoint(GridToLocalPos(gridPos.x, gridPos.y)); 
    }

    // 世界坐标 -> 格子坐标 (核心：用于扫描子物体落在哪一格)
    public Vector2Int WorldToGrid(Vector3 worldPos) {
        Vector3 localPos = transform.InverseTransformPoint(worldPos);
        Vector3 origin = GetLocalOrigin();
        int col = Mathf.FloorToInt((localPos.x - origin.x) / cellSize);
        int row = Mathf.FloorToInt((localPos.y - origin.y) / cellSize);
        return new Vector2Int(Mathf.Clamp(col, 0, COLS - 1), Mathf.Clamp(row, 0, ROWS - 1));
    }
}