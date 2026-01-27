using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoomGrid))]
public class RoomGridEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        RoomGrid grid = (RoomGrid)target;

        if (GUILayout.Button("对齐子物体到网格 (Snap Children)")) {
            Undo.RecordObjects(grid.GetComponentsInChildren<Transform>(), "Snap to Grid");
            foreach (Transform child in grid.transform) {
                // 计算相对于房间中心的局部坐标
                Vector3 localPos = child.localPosition;
                Vector3 origin = grid.GetLocalOrigin();

                // 反推网格行列
                int col = Mathf.FloorToInt((localPos.x - origin.x) / grid.cellSize);
                int row = Mathf.FloorToInt((localPos.y - origin.y) / grid.cellSize);

                // 限制在 7x13 范围内
                col = Mathf.Clamp(col, 0, RoomGrid.COLS - 1);
                row = Mathf.Clamp(row, 0, RoomGrid.ROWS - 1);

                // 重新设置对齐后的局部位置
                child.localPosition = grid.GridToLocalPos(col, row);
            }
        }
    }
}