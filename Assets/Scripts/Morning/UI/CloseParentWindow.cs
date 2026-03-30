using UnityEngine;

public class CloseParentWindow : MonoBehaviour
{
    private void Start()
    {
        CloseWindow();
    }

    // 这个方法将绑定到按钮的 OnClick 事件上
    public void CloseWindow()
    {
        // 检查当前物体是否有父物体（防止报错）
        if (transform.parent != null)
        {
            // 关闭它的父物体（即你的窗口/面板）
            transform.parent.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("这个按钮没有父物体！只能关闭自己了。");
            gameObject.SetActive(false); // 退一步，关闭自己
        }
    }
}