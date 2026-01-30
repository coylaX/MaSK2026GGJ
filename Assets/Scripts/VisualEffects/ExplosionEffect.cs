using UnityEngine;

[RequireComponent(typeof(AudioSource))] // 自动添加 AudioSource 组件
public class ExplosionEffect : MonoBehaviour
{
    [Header("Audio Settings")]
    [Tooltip("爆炸时播放的音效片段")]
    public AudioClip explosionClip;
    [Range(0f, 1f)]
    public float volume = 1f;
    [Tooltip("音效音调的随机范围，增加听觉丰富度")]
    public Vector2 pitchRange = new Vector2(0.9f, 1.1f);

    [Header("Lifecycle Settings")]
    [Tooltip("特效存活时间。如果上面有粒子系统，这个时间应该大于粒子的持续时间。")]
    public float lifeTime = 2f;

    private AudioSource _audioSource;
    private ParticleSystem _particleSystem;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        // 尝试获取粒子系统（如果有的话）
        _particleSystem = GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        // 1. 播放音效
        PlayExplosionSound();

        // 2. 自动调整存活时间（可选优化）
        // 如果这个物体上有粒子系统，我们尝试读取它的持续时间来决定销毁时间
        float actualDestroyDelay = lifeTime;
        if (_particleSystem != null)
        {
            // 粒子的持续时间 + 粒子的最大生命周期 = 安全的销毁时间
            float particleDuration = _particleSystem.main.duration + _particleSystem.main.startLifetime.constantMax;
            // 取两者中较大的一个，确保音效和粒子都能播完
            actualDestroyDelay = Mathf.Max(particleDuration, lifeTime);
        }

        // 3. 定时销毁自身
        // Destroy 方法的第二个参数是延迟时间
        Destroy(gameObject, actualDestroyDelay);
    }

    private void PlayExplosionSound()
    {
        if (explosionClip == null || _audioSource == null) return;

        // 随机化一点点音调，这样每次爆炸听起来都不太一样
        _audioSource.pitch = UnityEngine.Random.Range(pitchRange.x, pitchRange.y);
        _audioSource.volume = volume;
        
        // 使用 PlayOneShot 播放，适合这种短促的音效
        _audioSource.PlayOneShot(explosionClip);
    }
}