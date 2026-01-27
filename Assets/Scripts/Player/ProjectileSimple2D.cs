using UnityEngine;

public class ProjectileSimple2D : MonoBehaviour
{
    public float speed = 12f;
    public float lifeTime = 2f;

    private Vector2 _dir = Vector2.right;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.position += (Vector3)(_dir * speed * Time.deltaTime);
    }

    public void SetDirection(Vector2 dir)
    {
        if (dir.sqrMagnitude > 0.0001f)
            _dir = dir.normalized;
    }
}
