using UnityEngine;

public class PlayerMoveAndFlip : MonoBehaviour
{
    [Header("Movement (Force)")]
    public float moveForce = 40f;   // 推动力（越大加速越快）
    public float maxSpeed = 6f;     // 最大速度

    [Header("Render")]
    public SpriteRenderer spriteRenderer;

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.freezeRotation = true;

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        // A 取消 flipX，D 勾上 flipX（按下那一下触发）
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (spriteRenderer != null) spriteRenderer.flipX = true;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            if (spriteRenderer != null) spriteRenderer.flipX = false;
        }
    }

    private void FixedUpdate()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector2 input = new Vector2(x, y);
        if (input.sqrMagnitude > 1f) input.Normalize(); // 斜向不更快

        // 受力移动
        _rb.AddForce(input * moveForce, ForceMode2D.Force);

        // 限速
        Vector2 v = _rb.velocity;
        if (v.magnitude > maxSpeed)
            _rb.velocity  = v.normalized * maxSpeed;
    }
}
