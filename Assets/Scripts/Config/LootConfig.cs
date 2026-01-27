using UnityEngine;
using System;

// 注意：这个文件不需要继承 MonoBehaviour，因为它只是数据的容器
// 也不需要定义与文件名相同的类，除非你想把它们包裹在一个大类里

/// <summary>
/// 概率战利品条目：用于配置权重和对应的预制体
/// </summary>
[Serializable]
public class LootEntry {
    public GameObject prefab;    // 战利品的预制体
    [Range(0, 100)]
    public float weight;         // 权重（权重越高，随机到的概率越大）
}

/// <summary>
/// 必定生成战利品条目：用于配置必须出现的物品及其数量
/// </summary>
[Serializable]
public class GuaranteedLootEntry {
    public GameObject prefab;    // 战利品的预制体
    public int count;            // 必须生成的数量
}