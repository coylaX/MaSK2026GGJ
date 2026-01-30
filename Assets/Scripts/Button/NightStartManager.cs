using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NightStartManager : MonoBehaviour
{
    [Header("UI 配置")]
    public Button startNightButton;
    [Tooltip("点击开始后需要开启的战斗 UI 列表")]
    public List<GameObject> combatUIsToActivate;
    [Tooltip("点击开始后需要隐藏的初始/白天 UI")]
    public GameObject startMenuPanel;

    [Header("玩家配置")]
    public GameObject playerObject;

    private void Start()
    {
        // 1. 绑定按钮点击事件
        if (startNightButton != null)
        {
            startNightButton.onClick.AddListener(OnStartNightPressed);
        }
    }

    public void OnStartNightPressed()
    {
        // 2. 开启战斗所需的 UI 列表
        foreach (GameObject ui in combatUIsToActivate)
        {
            if (ui != null) ui.SetActive(true);
        }

        // 3. 隐藏开始菜单或白天 UI
        if (startMenuPanel != null)
        {
            startMenuPanel.SetActive(false);
        }

        // 4. 将 Player 设为 Active 并移动至 (0,0)
        if (playerObject != null)
        {
            playerObject.SetActive(true);
            // 在 2D 坐标系中精准复位
            playerObject.transform.position = new Vector3(0, 0, 0); 
            Debug.Log("<color=yellow>[战斗开始]</color> 玩家已就位，战斗 UI 已开启。");
        }
        else
        {
            // 如果没拖拽引用，尝试通过 Tag 寻找
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null)
            {
                p.SetActive(true);
                p.transform.position = Vector3.zero;
            }
        }

        // 5. (可选) 触发关卡生成
        /*
        if (LevelGenerator.Instance != null)
        {
            LevelGenerator.Instance.GenerateLevel();
        }
        */
    }
}