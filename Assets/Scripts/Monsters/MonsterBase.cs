using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class MonsterBase : MonoBehaviour {
    [Header("Base Stats")]
    public float health = 100f;
    public float moveSpeed = 3f;
    public float contactDamage = 10f;
    public float knockbackForce = 5f;

    [HideInInspector] public RoomController myRoom;
    protected Rigidbody2D rb;
    private bool isKnockingBack = false;

    protected virtual void Awake() {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    public void TakeDamage(float damage, Vector2 attackerPos) {
        health -= damage;
        Vector2 dir = ((Vector2)transform.position - attackerPos).normalized;
        StartCoroutine(KnockbackRoutine(dir));
        if (health <= 0) Die();
    }

    private IEnumerator KnockbackRoutine(Vector2 dir) {
        isKnockingBack = true;
        rb.velocity = Vector2.zero;
        rb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.2f);
        isKnockingBack = false;
    }

    public bool IsInKnockback() => isKnockingBack;

    [ContextMenu("Kill Monster (Debug)")]
    public void Die() {
        if (myRoom != null) myRoom.OnMonsterKilled(this);
        Destroy(gameObject);
    }
}