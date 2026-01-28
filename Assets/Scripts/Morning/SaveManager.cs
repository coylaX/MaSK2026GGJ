using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO; // 必须引用，用于读写文件

// --- 文件名: SaveManager.cs ---

public static class SaveManager
{
    // 定义存档文件的名称
    private static readonly string SAVE_FILE_NAME = "GameSaveData.json";

    // 定义一个静态事件：当读档完成时触发
    public static event Action OnLoadComplete;
    #region 1. 获取存档路径
    // ==========================================
    // 获取当前设备上的安全存档路径
    // PC: C:/Users/你的用户名/AppData/LocalLow/你的公司名/你的游戏名/
    // Mac: ~/Library/Application Support/你的公司名/你的游戏名/
    // ==========================================
    private static string GetSavePath()
    {
        return Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
    }
    #endregion

    #region 2. 保存功能 (Save)
    // ==========================================
    // 将内存中的 SaveData 对象转成 JSON 字符串写入硬盘
    // ==========================================
    public static void Save(SaveData data)
    {
        // 把当前场景中仓库里的面具数据，同步给存档对象
        data.maskInventoryList = new List<MaskInstance>(MaskInventory.I.maskInstances);
        data.backPackList = new List<MaskInstance>(BackPackLogic.I.maskInstances);

        try
        {
            // 1. 序列化：把对象变成文本
            // prettyPrint: true 表示生成的 JSON 会有换行和缩进，方便人阅读调试
            string json = JsonUtility.ToJson(data, true);

            // 2. 写入：覆盖写入文件
            File.WriteAllText(GetSavePath(), json);

            Debug.Log($"[SaveManager] 存档成功! 路径: {GetSavePath()}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveManager] 存档失败: {e.Message}");
        }
    }
    #endregion

    #region 3. 读取功能 (Load)
    // ==========================================
    // 从硬盘读取文件，并还原成 SaveData 对象
    // ==========================================
    public static SaveData Load()
    {
        string path = GetSavePath();

        // 1. 检查文件是否存在
        if (!File.Exists(path))
        {
            Debug.Log("[SaveManager] 未找到存档文件，创建全新存档。");
            return new SaveData(); // 返回一个初始化的新数据（第1天，空背包）
        }

        try
        {
            // 2. 读取文本
            string json = File.ReadAllText(path);

            // 3. 反序列化：把文本变回对象
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            // 把存档里的面具数据，还原给单例仓库，并且刷新UI。注意，只读取仓库的面具，没有背包的面具！
            MaskInventory.I.maskInstances = new List<MaskInstance>(data.maskInventoryList);

            // 读档最后，发出广播通知刷新面具UI
            // ?.Invoke 的意思是：如果有人在听，就喊一声；没人听就不喊
            OnLoadComplete?.Invoke();

            Debug.Log("[SaveManager] 读档成功!");
            return data;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveManager] 读档出错 (可能是存档损坏): {e.Message}");
            // 如果读取出错，为了防止卡死，返回一个新的空存档
            return new SaveData();
        }

    }
    #endregion

    #region 4. 辅助功能 (删除存档)
    // ==========================================
    // 开发测试用：清空进度
    // ==========================================
    public static void DeleteSaveFile()
    {
        string path = GetSavePath();
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("[SaveManager] 存档已删除。");
        }
    }
    #endregion
}