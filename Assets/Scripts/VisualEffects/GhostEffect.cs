using UnityEngine;

public class GhostEffect : MonoBehaviour
{
    public float ghostDelay = 0.05f; // 生成残影的间隔
    private float ghostDelayTimer;
    public GameObject ghostPrefab;   // 一个简单的空物体，带 SpriteRenderer
    
    public bool isVisible = false;   // 是否开启拖尾

    private SpriteRenderer sr;

    void Start() {
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    void Update() {
        if (!isVisible) return;

        if (ghostDelayTimer > 0) {
            ghostDelayTimer -= Time.deltaTime;
        } else {
            // 生成残影
            GameObject currentGhost = Instantiate(ghostPrefab, transform.position, transform.rotation);
            SpriteRenderer ghostSR = currentGhost.GetComponent<SpriteRenderer>();
            
            // 复制当前的图片和翻转状态
            ghostSR.sprite = sr.sprite;
            ghostSR.flipX = sr.flipX;
            
            // 设置初始颜色（透明度）
            ghostSR.color = new Color(0.5f, 0.5f, 1f, 0.6f); // 淡淡的蓝色

            ghostDelayTimer = ghostDelay;
            // 自动销毁由残影物体自己处理（见下文）
            Destroy(currentGhost, 0.5f); 
        }
    }
}