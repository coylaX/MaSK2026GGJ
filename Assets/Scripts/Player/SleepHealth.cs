using UnityEngine;

public class SleepHealth : MonoBehaviour
{
    [Header("Sleep Value (HP)")]
    public float maxSleep = 100f;
    public float currentSleep = 100f;

    [Header("Drain Over Time")]
    public float drainPerSecond = 2f; // Ã¿Ãë½µµÍ¶àÉÙ

    public bool IsDead => currentSleep <= 0f;

    private void Awake()
    {
        currentSleep = Mathf.Clamp(currentSleep, 0f, maxSleep);
    }

    private void Update()
    {
        if (IsDead) return;

        if (drainPerSecond > 0f)
        {
            ChangeSleep(-drainPerSecond * Time.deltaTime);
        }
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;
        if (amount <= 0f) return;

        ChangeSleep(-amount);
    }

    public void Heal(float amount)
    {
        if (IsDead) return;
        if (amount <= 0f) return;

        ChangeSleep(amount);
    }

    private void ChangeSleep(float delta)
    {
        currentSleep = Mathf.Clamp(currentSleep + delta, 0f, maxSleep);

        if (currentSleep <= 0f)
        {
            currentSleep = 0f;
            Die(); // Áô¿Õ
        }
    }

    private void Die()
    {
        // TODO: ËÀÍöÂß¼­£¨Áô¿Õ£©
    }

    public float Normalized()
    {
        return maxSleep <= 0f ? 0f : currentSleep / maxSleep;
    }
}
