using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LootManager : MonoBehaviour {
    public static LootManager Instance;

    [Header("随机配额设置")]
    public int minRandomLoot = 6;
    public int maxRandomLoot = 7;

    [Header("战利品列表")]
    public List<LootEntry> randomLootPool = new List<LootEntry>();
    public List<GuaranteedLootEntry> guaranteedLoots = new List<GuaranteedLootEntry>();

    [Header("精英房特殊视觉")]
    public Sprite eliteRoomBackground; // 【新增】在这里拖入你想替换的精英房背景图

    void Awake() { Instance = this; }

    public void DistributeLoot(List<RoomController> allRooms, RoomController startRoom) {
        List<RoomController> availableRooms = allRooms.Where(r => r != startRoom).ToList();
        Shuffle(availableRooms);

        int currentRoomIdx = 0;

        // 2. 分配“必定生成”的战利品（精英房）
        foreach (var entry in guaranteedLoots) {
            for (int i = 0; i < entry.count; i++) {
                if (currentRoomIdx >= availableRooms.Count) break;
                
                RoomController room = availableRooms[currentRoomIdx];
                room.assignedLootPrefab = entry.prefab;
                room.isGuaranteedRoom = true; 
                
                // 【核心修改】：在这里触发背景替换
                room.UpdateEliteVisuals(eliteRoomBackground);

                currentRoomIdx++;
            }
        }

        // 3. 分配“随机配额”的战利品
        int randomQuota = Random.Range(minRandomLoot, maxRandomLoot + 1);
        for (int i = 0; i < randomQuota; i++) {
            if (currentRoomIdx >= availableRooms.Count) break;
            
            GameObject selectedPrefab = GetRandomLootByWeight();
            if (selectedPrefab != null) {
                availableRooms[currentRoomIdx].assignedLootPrefab = selectedPrefab;
                currentRoomIdx++;
            }
        }

        Debug.Log($"战利品分配完成！精英房背景已同步更新。");
    }

    // 根据权重选择随机战利品
    private GameObject GetRandomLootByWeight() {
        float totalWeight = 0;
        foreach (var entry in randomLootPool) totalWeight += entry.weight;

        float randomVal = Random.Range(0, totalWeight);
        float currentWeightSum = 0;

        foreach (var entry in randomLootPool) {
            currentWeightSum += entry.weight;
            if (randomVal <= currentWeightSum) return entry.prefab;
        }
        return null;
    }

    // 洗牌算法
    private void Shuffle<T>(List<T> list) {
        for (int i = 0; i < list.Count; i++) {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}