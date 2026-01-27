using UnityEngine;

public class MorningGameManager : MonoBehaviour
{
    // ==========================================
    // 1. 单例设置 (Singleton)
    // 方便其他脚本通过 MorningGameManager.Instance 访问
    // ==========================================
    public static MorningGameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ==========================================
    // 2. 核心数据 (The Data)
    // 游戏运行时唯一的内存数据源，所有修改都发生在这里
    // ==========================================
    [Header("游戏运行时数据")]
    public SaveData currentSaveData;

    // ==========================================
    // 3. 初始化流程 (Initialization)
    // ==========================================
    private void Start()
    {
        // 第一步：加载存档 (如果没有存档，SaveManager会返回一个全新的第1天数据)
        LoadGame();

        // 第二步：根据当前天数，刷新订单
        // 注意：必须先有 currentSaveData，才能刷新订单
        if (OrderManager.Instance != null)
        {
            OrderManager.Instance.RefreshDailyOrders(currentSaveData.currentDay);
            UpdateUI();
        }
        else
        {
            Debug.LogError("场景中缺少 OrderManager 组件！请检查。");
        }

        ///TODO:(未来) 通知 UI 更新显示
        // UIManager.Instance.UpdateAllUI();
    }

    // ==========================================
    // 4. 游戏流程控制 (Game Flow)
    // ==========================================
    
    /// <summary>
    /// 进入战斗时调用
    /// </summary>
    public void EnterNight()
    {
        SaveGame();
        Debug.Log($"[进入战斗] 准备完毕！进入战斗。");
    }
    /// <summary>
    /// 当玩家梦境战斗结算时调用：进入下一天，刷新订单，保存游戏
    /// </summary>
    public void AdvanceDay()
    {
        Debug.Log("准备睡觉，进入下一天...");

        // 1. 天数 +1
        currentSaveData.currentDay++;

        // 2. 刷新新的一天的订单
        // (这也处理了旧订单的倒计时和过期逻辑)
        OrderManager.Instance.RefreshDailyOrders(currentSaveData.currentDay);

        // 3. 强制保存 (防止玩家刷初始)
        SaveGame();

        Debug.Log($"[新的一天] 战斗结束，进入下一天！现在是第 {currentSaveData.currentDay} 天。");
        UpdateUI();
        // TODO: 这里通常会播放转场动画，或者重新加载场景
    }

    // 辅助方法：订单 UI 刷新
    private void UpdateUI()
    {
        OrderUIController ui = FindObjectOfType<OrderUIController>();
        if (ui != null)
        {
            ui.RefreshOrderList();
        }
        else
        {
            // 如果场景里还没放 UI，这行日志提醒你
            Debug.LogWarning("场景里没找到 OrderUIController，UI 未刷新");
        }
    }

    // ==========================================
    // 5. 存档与读档封装
    // ==========================================

    public void SaveGame()
    {
        SaveManager.Save(currentSaveData);
    }

    private void LoadGame()
    {
        currentSaveData = SaveManager.Load();

        Debug.Log($"[数据加载] 载入完成。当前天数: {currentSaveData.currentDay}, " +
                  $"颜料: {currentSaveData.morningInventory.pigmentAmount}");
    }

    // ==========================================
    // 6. 调试工具 (Debug Tools)
    // 在 Unity 编辑器里右键脚本组件名即可调用
    // ==========================================

    [ContextMenu("下一天")]
    public void TestNextDay()
    {
        AdvanceDay();
        Debug.LogWarning("test已进入下一天。");
    }

    [ContextMenu("删除存档 (重置游戏)")]
    public void DeleteSaveFile()
    {
        SaveManager.DeleteSaveFile();
        Debug.LogWarning("存档已删除，请停止运行并重新开始游戏以生效。");
    }

    [ContextMenu("增加 100 颜料")]
    public void CheatAddMoney()
    {
        currentSaveData.morningInventory.pigmentAmount += 100;
        Debug.Log("作弊成功：颜料 +100");
    }
}
