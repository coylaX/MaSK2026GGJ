using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewOrderTemplate", menuName = "Game/Order Template")]
public class OrderTemplate : ScriptableObject
{
    private MemoryTraitID memoryTraitID;

    [Header("核心数据")]
    public string orderID;      // 比如 "ORDER_001" (唯一ID)

    [Header("显示信息")]
    public string customerName; // 比如 "王奶奶"

    // ✨ 建议新增：直接引用图片，比用字符串ID去找图片更直观、更不容易出错
    public Sprite portrait;

    [TextArea]
    public string requirementText;

    [Header("数值配置")]
    public int daysLimit;       // 期限
    public int baseReward;      // 报酬
    public int appearOnDay;     // 第几天出现
    public bool ifMemory; //是否获得Memory
    public MemoryTraitID id; //对应获得的Memory

    [Header("判定与反馈")]
    public List<string> tags;   // 需求标签，如 ["Happy", "Blue"]

    [TextArea] public string successReviewText; // 好评
    [TextArea] public string failReviewText;    // 差评
}
