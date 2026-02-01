using UnityEngine;
using UnityEngine.UI; // 如果你用的是旧版 UI Button，需要这个
// using TMPro; // 如果你的按钮文字是 TextMeshPro，可能需要这个

public class StartGameButtonHandler : MonoBehaviour
{
    // 这个引用指向整个开始界面的根 Canvas 对象
    public GameObject startScreenCanvasRoot; 

    void Start()
    {
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(OnStartGame);
        }
        else
        {
            Debug.LogError("StartGameButtonHandler 未找到 Button 组件!", this);
        }

        if (startScreenCanvasRoot == null)
        {
            // 尝试自动查找 Canvas，如果脚本挂在按钮上，通常 Canvas 是其父级的父级
            startScreenCanvasRoot = GetComponentInParent<Canvas>()?.gameObject;
            if (startScreenCanvasRoot == null)
            {
                Debug.LogError("StartGameButtonHandler 无法自动找到 StartScreenCanvasRoot. 请手动拖入.", this);
            }
        }
    }

    public void OnStartGame()
    {
        Debug.Log("开始游戏按钮被点击，销毁开始界面...");
        
        // 播放按钮点击音效 (可选)
        if (AudioManager.Instance != null && AudioManager.Instance.btnClick != null)
        {
            AudioManager.Instance.PlayUI(AudioManager.Instance.btnClick);
        }

        // 彻底销毁整个开始界面 Canvas 对象
        if (startScreenCanvasRoot != null)
        {
            Destroy(startScreenCanvasRoot);
            // 这里可以添加加载主场景的代码
            // UnityEngine.SceneManagement.SceneManager.LoadScene("MainGameScene");
        }
        else
        {
            Debug.LogError("StartScreenCanvasRoot 对象为空，无法销毁开始界面！", this);
        }
    }
}