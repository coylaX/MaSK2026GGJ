using UnityEngine;

public class Obstacle : MonoBehaviour {
    public bool isDestructible;
    public float health = 10f;

    public void TakeDamage(float damage) {
        if (!isDestructible) return; // 不可破坏直接无视
        health -= damage;
        if (health <= 0) Destroy(gameObject);
    }
}