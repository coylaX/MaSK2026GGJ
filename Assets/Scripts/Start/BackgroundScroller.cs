using UnityEngine;
using UnityEngine.UI;

public class BackgroundScroller : MonoBehaviour
{
    [Header("拖入你的两个背景Image")]
    public RectTransform bg1;
    public RectTransform bg2;

    [Header("向右滚动的速度")]
    public float scrollSpeed = 100f; 

    private float _width;

    void Start()
    {
        // 获取图片的宽度（假设两张图大小一样）
        _width = bg1.rect.width;

        // 初始对齐位置：bg1在中心，bg2在左边（这样向右滚时bg2会接替进来）
        // 或者按你喜欢的：bg1中心，bg2右边（如果你是向左滚的话）
        // 既然你要向右滚：
        bg1.anchoredPosition = Vector2.zero;
        bg2.anchoredPosition = new Vector2(-_width, 0); 
    }

    void Update()
    {
        // 1. 持续向右移动
        float moveAmount = scrollSpeed * Time.deltaTime;
        bg1.anchoredPosition += new Vector2(moveAmount, 0);
        bg2.anchoredPosition += new Vector2(moveAmount, 0);

        // 2. 检查边界并瞬移复位
        // 如果图1完全滚出右侧边界（它的坐标 > 宽度）
        if (bg1.anchoredPosition.x >= _width)
        {
            // 把它挪到图2的左边接上
            bg1.anchoredPosition = new Vector2(bg2.anchoredPosition.x - _width, 0);
        }

        // 如果图2完全滚出右侧边界
        if (bg2.anchoredPosition.x >= _width)
        {
            // 把它挪到图1的左边接上
            bg2.anchoredPosition = new Vector2(bg1.anchoredPosition.x - _width, 0);
        }
    }
}