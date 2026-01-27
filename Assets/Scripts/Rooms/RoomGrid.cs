using UnityEngine;

public class RoomGrid : MonoBehaviour {
    public const int ROWS = 7;
    public const int COLS = 13;
    public float cellSize = 1f;

    // 计算局部偏移，使网格居中
    // 默认 (0,0) 是房间中心，我们要计算出左下角的起始点
    public Vector3 GetLocalOrigin() {
        return new Vector3(
            -(COLS * cellSize) / 2f, 
            -(ROWS * cellSize) / 2f, 
            0
        );
    }

    // 将网格坐标 (col, row) 转换为房间内的局部坐标
    public Vector3 GridToLocalPos(int col, int row) {
        Vector3 origin = GetLocalOrigin();
        return new Vector3(
            origin.x + (col * cellSize) + (cellSize / 2f),
            origin.y + (row * cellSize) + (cellSize / 2f),
            0
        );
    }

    // 辅助线绘制：现在使用 Gizmos.matrix 确保辅助线随物体旋转/移动
    private void OnDrawGizmos() {
        Gizmos.matrix = transform.localToWorldMatrix; // 关键：将 Gizmos 转换到局部空间
        Gizmos.color = Color.cyan;

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