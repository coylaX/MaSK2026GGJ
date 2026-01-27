using UnityEngine;

public class RoomGrid : MonoBehaviour {
    public const int ROWS = 7; //
    public const int COLS = 13; //
    public float cellSize = 1f;

    [Header("Layer Config")]
    public LayerMask obstacleLayer; // 对应策划案中的 block 层

    // 获取房间左下角的局部起点
    public Vector3 GetLocalOrigin() {
        return new Vector3(-(COLS * cellSize) / 2f, -(ROWS * cellSize) / 2f, 0);
    }

    // 1. 核心修复：网格坐标 -> 局部坐标 (用于编辑器对齐子物体)
    public Vector3 GridToLocalPos(int col, int row) {
        Vector3 origin = GetLocalOrigin();
        return new Vector3(
            origin.x + (col * cellSize) + (cellSize / 2f),
            origin.y + (row * cellSize) + (cellSize / 2f),
            0
        );
    }

    // 在 RoomGrid.cs 中确保此方法存在
    public Vector2 GridToWorld(Vector2Int gridPos) {
        Vector3 origin = GetLocalOrigin();
        // 计算格子的物理中心点
        Vector3 localPos = new Vector3(
            origin.x + (gridPos.x * cellSize) + (cellSize / 2f),
            origin.y + (gridPos.y * cellSize) + (cellSize / 2f),
            0
        );
        return transform.TransformPoint(localPos); 
    }

    // 3. 辅助功能：世界坐标 -> 网格坐标 (用于定位玩家或怪物位置)
    public Vector2Int WorldToGrid(Vector3 worldPos) {
        Vector3 localPos = transform.InverseTransformPoint(worldPos);
        Vector3 origin = GetLocalOrigin();
        int col = Mathf.FloorToInt((localPos.x - origin.x) / cellSize);
        int row = Mathf.FloorToInt((localPos.y - origin.y) / cellSize);
        return new Vector2Int(Mathf.Clamp(col, 0, COLS - 1), Mathf.Clamp(row, 0, ROWS - 1));
    }

    // 替换 RoomGrid.cs 中的对应方法
    public bool IsEmpty(int x, int y) {
        // 强制同步物理变换，确保编辑器能实时感知物体位置
        Physics2D.SyncTransforms(); 

        Vector2 worldPos = GridToWorld(new Vector2Int(x, y));
        
        // 稍微放大检测范围 (0.95f) 确保能碰触到格子内的边缘
        // 使用所有 block 层的物体进行检测
        Collider2D hit = Physics2D.OverlapBox(worldPos, new Vector2(cellSize * 0.95f, cellSize * 0.95f), 0, obstacleLayer);
        
        // 调试用：如果这一格检测到了东西，会在编辑器画个红方块（仅在执行时）
        if (hit != null) {
            // Debug.Log($"格子 {x},{y} 检测到障碍物: {hit.name}");
        }
        
        return hit == null;
    }

    // 检测周围是否有障碍物 (用于采样导航拐角)
    public bool IsNearObstacle(int x, int y) {
        for (int i = -1; i <= 1; i++) {
            for (int j = -1; j <= 1; j++) {
                if (i == 0 && j == 0) continue;
                int checkX = x + i;
                int checkY = y + j;
                if (checkX >= 0 && checkX < COLS && checkY >= 0 && checkY < ROWS) {
                    if (!IsEmpty(checkX, checkY)) return true;
                }
            }
        }
        return false;
    }

    // 绘制 Gizmos 方便策划在 Scene 窗口观察
    private void OnDrawGizmos() {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = new Color(0, 1, 1, 0.3f);
        Vector3 origin = GetLocalOrigin();

        for (int x = 0; x <= COLS; x++) {
            float xPos = origin.x + x * cellSize;
            Gizmos.DrawLine(new Vector3(xPos, origin.y, 0), new Vector3(xPos, origin.y + ROWS * cellSize, 0));
        }
        for (int y = 0; y <= ROWS; y++) {
            float yPos = origin.y + y * cellSize;
            Gizmos.DrawLine(new Vector3(origin.x, yPos, 0), new Vector3(origin.x + COLS * cellSize, yPos, 0));
        }
    }
}