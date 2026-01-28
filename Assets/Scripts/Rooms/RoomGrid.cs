using UnityEngine;

public class RoomGrid : MonoBehaviour {
    public const int ROWS = 7;
    public const int COLS = 13;
    
    [Header("网格配置")]
    public float cellSize = 1f;
    public LayerMask obstacleLayer; // 刚才漏掉的：报错1解决

    [Header("Gizmo 设置")]
    public Color gizmoColor = new Color(0, 1, 1, 0.3f);
    public bool showGridAlways = false; 

    // --- 核心工具方法：供 RoomNavGraph 调用 ---

    public Vector3 GetLocalOrigin() => new Vector3(-(COLS * cellSize) / 2f, -(ROWS * cellSize) / 2f, 0);

    // 格子坐标 -> 局部坐标 (报错3解决)
    public Vector3 GridToLocalPos(int col, int row) {
        Vector3 origin = GetLocalOrigin();
        return new Vector3(origin.x + (col * cellSize) + (cellSize / 2f),
                           origin.y + (row * cellSize) + (cellSize / 2f), 0);
    }

    // 格子坐标 -> 世界坐标
    public Vector2 GridToWorld(Vector2Int gridPos) {
        return transform.TransformPoint(GridToLocalPos(gridPos.x, gridPos.y)); 
    }

    // 世界坐标 -> 格子坐标 (报错2解决)
    public Vector2Int WorldToGrid(Vector3 worldPos) {
        Vector3 localPos = transform.InverseTransformPoint(worldPos);
        Vector3 origin = GetLocalOrigin();
        int col = Mathf.FloorToInt((localPos.x - origin.x) / cellSize);
        int row = Mathf.FloorToInt((localPos.y - origin.y) / cellSize);
        return new Vector2Int(Mathf.Clamp(col, 0, COLS - 1), Mathf.Clamp(row, 0, ROWS - 1));
    }

    // --- 场景显示逻辑 ---

    private void OnDrawGizmos() {
#if UNITY_EDITOR
        if (showGridAlways || IsHierarchySelected()) {
            DrawGrid();
        }
#endif
    }

#if UNITY_EDITOR
    private bool IsHierarchySelected() {
        GameObject activeGO = UnityEditor.Selection.activeGameObject;
        if (activeGO == null) return false;
        return activeGO == gameObject || activeGO.transform.IsChildOf(transform);
    }

    private void DrawGrid() {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = gizmoColor;

        Vector3 origin = GetLocalOrigin();
        float width = COLS * cellSize;
        float height = ROWS * cellSize;

        for (int c = 0; c <= COLS; c++) {
            float x = origin.x + c * cellSize;
            Gizmos.DrawLine(new Vector3(x, origin.y, 0), new Vector3(x, origin.y + height, 0));
        }

        for (int r = 0; r <= ROWS; r++) {
            float y = origin.y + r * cellSize;
            Gizmos.DrawLine(new Vector3(origin.x, y, 0), new Vector3(origin.x + width, y, 0));
        }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(origin, 0.1f);
    }
#endif
}