using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Mixer & Snapshots")]
    public AudioMixer mainMixer;
    public AudioMixerGroup sfxGroup; // 记得在Inspector关联Mixer的SFX组
    public AudioMixerSnapshot daySnapshot;
    public AudioMixerSnapshot nightSnapshot;

    [Header("Music Sources")]
    public AudioSource daySource;
    public AudioSource nightSource;

    [Header("UI Clips")]
    public AudioClip btnHover;
    public AudioClip btnExit;
    public AudioClip btnClick;

    [Header("Combat & Environment Clips")]
    public AudioClip monsterDeath;
    public AudioClip meleeSwing;
    public AudioClip meleeHit;
    public AudioClip doorOpen;
    public AudioClip changeMask;
    public AudioClip remoteShoot1;
    public AudioClip remoteShoot2;
    public AudioClip remoteShoot3;
    public AudioClip remoteHit;
    public AudioClip requestSuccess;
    public AudioClip requestFail;
    public AudioClip bombExplode;
    public AudioClip playerDie;
    public AudioClip pickBook;
    public AudioClip pickWater;

    private AudioSource sfxSource;

    void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else 
        {
            Destroy(gameObject);
            return;
        }
        
        // 初始化SFX播放器并关联Mixer组
        sfxSource = gameObject.AddComponent<AudioSource>();
        if (sfxGroup != null) sfxSource.outputAudioMixerGroup = sfxGroup;
    }

    void Start()
    {
        daySnapshot.TransitionTo(0f); 
        daySource.Play();
        nightSource.Play();
    }

    // --- 核心播放逻辑 (带Pitch随机化) ---
    private void PlaySFX(AudioClip clip, float pitchRange = 0.1f)
    {
        if (clip == null) return;
        sfxSource.pitch = Random.Range(1f - pitchRange, 1f + pitchRange);
        sfxSource.PlayOneShot(clip);
    }

    // --- 供外部调用的音效方法 ---

    public void PlayMonsterDeath() => PlaySFX(monsterDeath);
    public void PlayMeleeSwing()   => PlaySFX(meleeSwing, 0.15f); // 挥动声音随机大一点更自然
    public void PlayMeleeHit()     => PlaySFX(meleeHit);
    public void PlayDoorOpen()     => PlaySFX(doorOpen, 0.05f);
    public void PlayChangeMask()   => PlaySFX(changeMask);
    
    // 远程发射（3种）
    public void PlayRemoteShoot1() => PlaySFX(remoteShoot1);
    public void PlayRemoteShoot2() => PlaySFX(remoteShoot2);
    public void PlayRemoteShoot3() => PlaySFX(remoteShoot3);
    
    public void PlayRemoteHit()    => PlaySFX(remoteHit);

    //订单提交
    public void RequestSuccess()   => PlaySFX(requestSuccess);
    public void RequestFail()      => PlaySFX(requestFail);

    //炸弹爆炸
    public void BombExplode()      => PlaySFX(bombExplode);

    //捡道具
    public void OnBookPickUp()      => PlaySFX(pickBook);
    public void OnWaterPickUp()      => PlaySFX(pickWater);

    //死亡/成功撤离
    public void PlayerDie()        => PlaySFX(playerDie);

    // --- 原有功能保持 ---

    public void SwitchToDay(float duration = 2.0f)
    {
        if (daySnapshot != null) daySnapshot.TransitionTo(duration); 
    }

    public void SwitchToNight(float duration = 2.0f)
    {
        if (nightSnapshot != null) nightSnapshot.TransitionTo(duration);
    }

    public void PlayUI(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.pitch = 1.0f; // UI音效保持稳定
        sfxSource.PlayOneShot(clip);
    }
}