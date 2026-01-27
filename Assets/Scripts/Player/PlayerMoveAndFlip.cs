using UnityEngine;

public class PlayerMoveAndFlip : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Render")]
    public SpriteRenderer spriteRenderer;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        // WASD / Arrow keys 都能用（Unity 默认 Input 轴）
        float x = Input.GetAxisRaw("Horizontal"); // A=-1, D=+1
        float y = Input.GetAxisRaw("Vertical");   // S=-1, W=+1

        Vector3 dir = new Vector3(x, y, 0f);
        if (dir.sqrMagnitude > 1f) dir.Normalize(); // 斜向不更快

        transform.position += dir * moveSpeed * Time.deltaTime;

        // D -> 勾上 flipX, A -> 取消 flipX
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (spriteRenderer != null) spriteRenderer.flipX = true;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            if (spriteRenderer != null) spriteRenderer.flipX = false;
        }
    }
}
