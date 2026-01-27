using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class MonsterBase : MonoBehaviour {
    public float health = 100f;
    public float moveSpeed = 3f;
    public float contactDamage = 10f;
    public float knockbackForce = 5f;

    public static event Action OnMonsterDeath;
    private Rigidbody2D rb;
    private bool isKnockingBack;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    public void TakeDamage(float damage, Vector2 source) {
        health -= damage;
        Vector2 dir = ((Vector2)transform.position - source).normalized;
        StartCoroutine(Knockback(dir));
        if (health <= 0) Die();
    }

    IEnumerator Knockback(Vector2 dir) {
        isKnockingBack = true;
        rb.velocity = Vector2.zero;
        rb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.2f);
        isKnockingBack = false;
    }

    public bool IsInKnockback() => isKnockingBack;
    void Die() { OnMonsterDeath?.Invoke(); Destroy(gameObject); }
}