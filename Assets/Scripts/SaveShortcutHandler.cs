using UnityEngine;

public class SaveShortcutHandler : MonoBehaviour
{
    [Header("UI 设置")]
    [Tooltip("将你制作好的提示框 Prefab 拖入这里")]
    public GameObject deleteNoticePrefab; 
    
    [Tooltip("提示框显示多久后消失")]
    public float displayDuration = 2.0f;

    void Update()
    {
        // 组合键检测：Ctrl + Alt + D
        bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        bool alt = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
        bool dDown = Input.GetKeyDown(KeyCode.D);

        if (ctrl && alt && dDown)
        {
            TriggerDeleteProcess();
        }
    }

    private void TriggerDeleteProcess()
    {
        // 1. 调用删除逻辑
        MorningGameManager.Instance.DeleteSaveFile();
        Debug.Log("<color=yellow>存档已删除！</color>");

        // 2. 实例化 UI Prefab
        if (deleteNoticePrefab != null)
        {
            // 自动寻找当前场景中的 Canvas
            Canvas canvas = FindObjectOfType<Canvas>();
            
            if (canvas != null)
            {
                // 生成 Prefab 并将其设为 Canvas 的子物体
                GameObject noticeInstance = Instantiate(deleteNoticePrefab, canvas.transform);
                
                // 确保它出现在 UI 层级最前方
                noticeInstance.transform.SetAsLastSibling();

                // 3. 自动销毁
                Destroy(noticeInstance, displayDuration);
            }
            else
            {
                Debug.LogError("场景中没有 Canvas，无法显示删除提示！");
            }
        }
    }
}