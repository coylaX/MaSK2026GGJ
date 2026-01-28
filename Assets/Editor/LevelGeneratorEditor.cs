using UnityEngine;
using UnityEditor;

// 指定我们要为 LevelGenerator 类创建自定义编辑器
[CustomEditor(typeof(LevelGenerator))]
public class LevelGeneratorEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        // 1. 绘制原本在 Inspector 里的所有变量（如 roomPool, roomSpacing 等）
        DrawDefaultInspector();

        // 获取目标脚本的引用
        LevelGenerator generator = (LevelGenerator)target;

        // 加上一些间距
        EditorGUILayout.Space();

        // 2. 绘制按钮
        // GUILayout.Button 会在点击时返回 true
        if (GUILayout.Button("Generate New Level", GUILayout.Height(30)))
        {
            // 点击后执行生成逻辑
            generator.GenerateLevel();
            
            // 标记场景已修改，确保撤销系统能记录此操作
            EditorUtility.SetDirty(generator);
        }

        if (GUILayout.Button("Clear Level", GUILayout.Height(25)))
        {
            generator.ClearLevel();
            EditorUtility.SetDirty(generator);
        }
    }
}