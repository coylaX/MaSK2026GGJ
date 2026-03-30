using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LastClickMaskInfo : MonoBehaviour
{
    public static LastClickMaskInfo Instance { get; private set; }

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

    //当玩家点击面具时，调用此方法，生成面具的说明
    public void RefreshMaskInfo(EmotionTraitID emotionid, MemoryTraitID memoryid, ColorTraitID colorid)
    {
        // 1. 初始化变量
        string emotionText = "<b>未知情绪</b>";
        string memoryText = "<b>无记忆</b>";
        string colorText = "<b>无颜色</b>";

        // 2. 刷新情绪描述 (修正了变量名)
        switch (emotionid)
        {
            case EmotionTraitID.XI: emotionText = "<b>情绪：</b> 喜 \n佩戴时使用喜之步枪攻击敌人"; break;
            case EmotionTraitID.NU: emotionText = "<b>情绪：</b> 怒 \n佩戴时使用愤怒之刃攻击敌人"; break;
            case EmotionTraitID.AI: emotionText = "<b>情绪：</b> 哀 \n佩戴时使用悲伤泡泡攻击敌人"; break;
            case EmotionTraitID.LE: emotionText = "<b>情绪：</b> 乐 \n佩戴时使用开心波攻击敌人"; break;
        }

        // 3. 刷新记忆描述
        switch (memoryid)
        {
            case MemoryTraitID.A: memoryText = "<b>记忆：</b> 损坏的录音 \n佩戴上由该记忆铸成的面具时，将获得10s的无敌效果"; break;
            case MemoryTraitID.B: memoryText = "<b>记忆：</b> 夏日橘子汽水的味道 \n佩戴上由该记忆铸成的面具时，将损失50%当前清醒值（生命值），但造成的伤害大幅提升"; break;
            case MemoryTraitID.C: memoryText = "<b>记忆：</b> 永恒涟漪 \n佩戴上由该记忆铸成的面具时，立刻获得3个炸弹"; break;
            case MemoryTraitID.D: memoryText = "<b>记忆：</b> 献祭的欢愉 \n暂无特殊佩戴效果"; break;
            case MemoryTraitID.E: memoryText = "<b>记忆：</b> 断桥的栏杆铁锈 \n暂无特殊佩戴效果"; break;
        }

        // 4. 刷新颜色描述 (修正了变量名)
        switch (colorid)
        {
            case ColorTraitID.RED: colorText = "<b>颜色：</b> 红 \n红色面具被撕掉后，将降低攻速，但对近距离的敌人大幅提高伤害"; break;
            case ColorTraitID.YELLOW: colorText = "<b>颜色：</b> 黄 \n黄色面具被撕掉后，会使后续的攻击可以阻挡敌人的弹幕"; break;
            case ColorTraitID.BLUE: colorText = "<b>颜色：</b> 蓝 \n蓝色面具被撕掉后，会使后续的攻击可以穿透敌人与障碍物"; break;
            case ColorTraitID.GREEN: colorText = "<b>颜色：</b> 绿 \n绿色面具被撕掉后，会对远距离的敌人大幅提高伤害"; break;
            case ColorTraitID.BLACK: colorText = "<b>颜色：</b> 黑 \n黑色面具被撕掉后，将大幅增加移速"; break;
            case ColorTraitID.WHITE: colorText = "<b>颜色：</b> 白 \n白色面具被撕掉后，将大幅回复清醒值（生命值）"; break;
        }

        // 5. 组合文本，使用 \n 进行换行排版
        effectText.text = $"当前面具效果：\n{emotionText}\n{memoryText}\n{colorText}";

        // 1. 初始化变量
        string emotion1Text = "<b>未知情绪</b>";
        string memory1Text = "<b>无记忆</b>";
        string color1Text = "<b>无颜色</b>";
        // 2. 刷新情绪描述 (修正了变量名)
        switch (emotionid)
        {
            case EmotionTraitID.XI: emotion1Text = "<b>情绪：</b> 喜"; break;
            case EmotionTraitID.NU: emotion1Text = "<b>情绪：</b> 怒"; break;
            case EmotionTraitID.AI: emotion1Text = "<b>情绪：</b> 哀"; break;
            case EmotionTraitID.LE: emotion1Text = "<b>情绪：</b> 乐"; break;
        }

        // 3. 刷新记忆描述
        switch (memoryid)
        {
            case MemoryTraitID.A: memory1Text = "<b>记忆：</b> 损坏的录音"; break;
            case MemoryTraitID.B: memory1Text = "<b>记忆：</b> 夏日橘子汽水的味道"; break;
            case MemoryTraitID.C: memory1Text = "<b>记忆：</b> 永恒涟漪"; break;
            case MemoryTraitID.D: memory1Text = "<b>记忆：</b> 献祭的欢愉"; break;
            case MemoryTraitID.E: memory1Text = "<b>记忆：</b> 断桥的栏杆铁锈"; break;
        }

        // 4. 刷新颜色描述 (修正了变量名)
        switch (colorid)
        {
            case ColorTraitID.RED: color1Text = "<b>颜色：</b> 红"; break;
            case ColorTraitID.YELLOW: color1Text = "<b>颜色：</b> 黄"; break;
            case ColorTraitID.BLUE: color1Text = "<b>颜色：</b> 蓝"; break;
            case ColorTraitID.GREEN: color1Text = "<b>颜色：</b> 绿"; break;
            case ColorTraitID.BLACK: color1Text = "<b>颜色：</b> 黑"; break;
            case ColorTraitID.WHITE: color1Text = "<b>颜色：</b> 白"; break;
        }
        orderSubmitEffectText = $"待提交面具效果：\n{emotion1Text}\n{memory1Text}\n{color1Text}";
    }

    public TextMeshProUGUI effectText;   // 效果描述
    public string orderSubmitEffectText;
}
