using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class MiniMapManager : MonoBehaviour {
    public static MiniMapManager Instance;

    [Header("UI 引用")]
    public RectTransform iconContainer; 
    public GameObject iconPrefab;      

    [Header("颜色配置")]
    public Color colorCurrent = new Color(0.9f, 0.9f, 0.9f);   // 1. 玩家所在房间 (亮灰色)
    public Color colorCleared = new Color(0.5f, 0.5f, 0.5f);   // 2. 已清空房间 (灰色)
    public Color colorDiscovered = new Color(0.2f, 0.2f, 0.2f); // 3. 未清空/已发现 (深灰色)

    [Header("平滑移动")]
    public float scrollSpeed = 8f;
    public Vector2 iconSize = new Vector2(30f, 18f); 
    public float padding = 1f;

    private Dictionary<RoomController, Image> roomToIconMap = new Dictionary<RoomController, Image>();
    private RoomController currentRoom; // 【核心】：记录玩家当前所在的房间
    private Vector2 targetContainerPos;

    void Awake() { Instance = this; }

    void Update() {
        // 平滑滚动，保持当前房间居中
        iconContainer.anchoredPosition = Vector2.Lerp(iconContainer.anchoredPosition, targetContainerPos, Time.deltaTime * scrollSpeed);
    }

    public void GenerateMiniMap(List<RoomController> allRooms) {
        foreach (Transform child in iconContainer) Destroy(child.gameObject);
        roomToIconMap.Clear();

        List<float> distinctX = GetDistinctCoords(allRooms.Select(r => r.transform.position.x).ToList());
        List<float> distinctY = GetDistinctCoords(allRooms.Select(r => r.transform.position.y).ToList());

        foreach (var room in allRooms) {
            GameObject iconObj = Instantiate(iconPrefab, iconContainer);
            RectTransform rect = iconObj.GetComponent<RectTransform>();
            Image iconImage = iconObj.GetComponent<Image>();

            rect.sizeDelta = iconSize;
            int gridX = FindCoordIndex(distinctX, room.transform.position.x);
            int gridY = FindCoordIndex(distinctY, room.transform.position.y);
            rect.anchoredPosition = new Vector2(gridX * (iconSize.x + padding), gridY * (iconSize.y + padding));

            // 【新功能】：检查是否是精英房间并显示边框
            // 假设你的图标预制体上有一个名为 "Border" 的子物体
            Transform border = iconObj.transform.Find("Border");
            if (border != null) {
                border.gameObject.SetActive(room.isGuaranteedRoom);
            }
            roomToIconMap.Add(room, iconImage);
            iconObj.SetActive(false);
        }
    }

    // 【关键函数】：由 RoomController 在 ActivateRoom 或传送成功后调用
    public void UpdateCurrentRoom(RoomController room) {
        currentRoom = room; // 更新当前坐标

        // 1. 计算居中位置
        targetContainerPos = -roomToIconMap[room].rectTransform.anchoredPosition;

        // 2. 刷新所有房间的颜色和可见性
        RefreshMap();
    }

    public void RefreshMap() {
        foreach (var kvp in roomToIconMap) {
            RoomController room = kvp.Key;
            Image icon = kvp.Value;

            // --- 优先级颜色逻辑 ---
            
            // 逻辑 1：如果这是玩家当前所在的房间，强制显示“亮灰色”
            if (room == currentRoom) {
                icon.color = colorCurrent;
            }
            // 逻辑 2：如果已经清空（且玩家不在这里），显示“灰色”
            else if (room.state == RoomController.RoomState.Cleared) {
                icon.color = colorCleared;
            }
            // 逻辑 3：如果未清空（且玩家不在这里），显示“深灰色”
            else {
                icon.color = colorDiscovered;
            }

            // --- 迷雾可见性逻辑 ---
            bool shouldShow = IsRoomDiscovered(room);
            icon.gameObject.SetActive(shouldShow);
        }
    }

    private bool IsRoomDiscovered(RoomController room) {
        // 自己进入过，显示
        if (room.state != RoomController.RoomState.Inactive) return true;
        // 玩家当前在隔壁，显示（显示邻居）
        foreach (Door door in room.doors) {
            if (door.targetRoom != null && door.targetRoom.state != RoomController.RoomState.Inactive) return true;
        }
        return false;
    }

    // --- 排序辅助代码 ---
    private List<float> GetDistinctCoords(List<float> coords) {
        List<float> distinct = new List<float>();
        coords.Sort();
        if (coords.Count == 0) return distinct;
        distinct.Add(coords[0]);
        for (int i = 1; i < coords.Count; i++) {
            if (Mathf.Abs(coords[i] - distinct[distinct.Count - 1]) > 1.0f) distinct.Add(coords[i]);
        }
        return distinct;
    }
    private int FindCoordIndex(List<float> sortedList, float value) {
        for (int i = 0; i < sortedList.Count; i++) {
            if (Mathf.Abs(sortedList[i] - value) < 1.0f) return i;
        }
        return 0;
    }
}