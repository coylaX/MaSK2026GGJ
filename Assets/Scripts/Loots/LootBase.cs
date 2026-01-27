using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class LootBase : MonoBehaviour
{
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnPickedUp();
            Destroy(gameObject); // 触发后自我销毁
        }
    }

    // 由子类实现具体逻辑
    protected abstract void OnPickedUp();
}