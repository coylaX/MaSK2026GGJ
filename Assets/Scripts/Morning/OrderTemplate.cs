using UnityEngine;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

[CreateAssetMenu(fileName = "NewOrderTemplate", menuName = "Game/Order Template")]
public class OrderTemplate : ScriptableObject
{
    [Header("OrderTemplate包含：顾客名字，顾客头像，需求描述，匹配要求x3（情绪，记忆，颜色；没有要求就输出Null），5段评价")]
    [Header("核心数据，保持唯一即可")]
    public string orderID;      // 比如 "ORDER_001" (唯一ID)

    [Header("订单显示信息")]
    public string customerName; // 比如 "王奶奶"
    public Sprite portrait; //导入头像
    public int daysLimit=5;       // 期限
    public int baseReward=100;      // 成功报酬
    public int loseReward = 50;      // 失败报酬

    [Header("需求描述")]
    [TextArea] public string requirementText;
    
    [Header("数值配置")]
    public int appearOnDay;     // 第几天出现
    public bool ifMemory; //是否获得Memory，勾选了下一项才有用
    //public MemoryTraitID id; //对应获得的Memory

    [Header("面具匹配要求")]
    public MemoryTraitID memoryTraitID;
    public List<ColorTraitID> colorTraitIDs = new List<ColorTraitID>();
    public EmotionTraitID emotionTraitID;

    [TextArea] public string bestReviewText; // 真好评
    [TextArea] public string successReviewText; // 次好评
    [TextArea] public string aFailReviewText;    // 差评a
    [TextArea] public string bFailReviewText;    // 差评b
    [TextArea] public string cFailReviewText;    // 差评c

    [Header("记忆订单面具匹配要求")]
    [TextArea] public string aMRFailReviewText;    // 记忆匹配差评a
    [TextArea] public string bMRFailReviewText;    // 记忆匹配差评b
    [TextArea] public string cMRFailReviewText;    // 记忆匹配差评c
    [TextArea] public string aMFFailReviewText;    // 记忆错误差评a
    [TextArea] public string bMFFailReviewText;    // 记忆错误差评b
    [TextArea] public string cMFFailReviewText;    // 记忆错误差评c
    [TextArea] public string MemoryFailRviewText;    // 完全差评
}
