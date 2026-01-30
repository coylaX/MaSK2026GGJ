using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    [Header("UI 显隐配置")]
    public GameObject[] shixiaoGOs; 
    public GameObject[] activeGOs;  

    [Header("引用配置")]
    public LevelGenerator generator; 
    public GameObject player;        
    
    public void onClick()
    {
        // 1. 原有面具数量检查逻辑
        if (BackPackLogic.I.maskInstances.Count < 3)
        {
            return;
        }

        // 2. 原有 UI 批量显隐逻辑
        foreach (var go in shixiaoGOs)
        {
            if (go != null) go.SetActive(false);
        }

        foreach (var go in activeGOs)
        {
            if (go != null) go.SetActive(true);
        }

        // 3. 玩家复位与生命值重置
        if (player != null)
        {
            player.SetActive(true);
            player.transform.position = Vector3.zero;

            // --- 新增：尝试获取生命值组件并回满 ---
            SleepHealth health = player.GetComponent<SleepHealth>();
            if (health != null)
            {
                health.RestoreFullSleep(); // 调用刚才新增的方法
            }
        }

        generator.GenerateLevel();
    }
}