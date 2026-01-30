using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour 
{
    public static CameraManager Instance;

    [Header("平滑跟踪")]
    public float lerpSpeed = 8f;
    private Vector3 targetPos;

    [Header("抖动配置")]
    // 存储当前的抖动位移，确保不干扰平滑跟随逻辑
    private Vector3 shakeOffset = Vector3.zero;

    void Awake() 
    { 
        if (Instance == null) Instance = this; 
        else Destroy(gameObject);

        targetPos = transform.position; 
    }

    void Update() 
    {
        // 核心算法：
        // 1. 先把当前位置减去抖动偏移，得到“纯净”的摄像机位置
        // 2. 将其向目标位置平滑插值 (Lerp)
        // 3. 最后再加上新的抖动偏移量
        Vector3 currentPurePos = transform.position - shakeOffset;
        Vector3 nextPurePos = Vector3.Lerp(currentPurePos, targetPos, Time.deltaTime * lerpSpeed);
        
        transform.position = nextPurePos + shakeOffset;
    }

    /// <summary>
    /// 【修复报错的关键方法】：切换摄像机目标房间中心点
    /// </summary>
    public void SwitchToRoom(Vector3 center) 
    {
        targetPos = new Vector3(center.x, center.y, transform.position.z);
    }

    /// <summary>
    /// 触发带衰减效果的屏幕抖动
    /// </summary>
    public void Shake(float duration, float intensity)
    {
        // 停止之前的抖动，防止多个爆炸同时发生时抖动逻辑冲突
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine(duration, intensity));
    }

    private IEnumerator ShakeRoutine(float duration, float intensity)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // --- 衰减逻辑 ---
            // 随着时间推移，percent 从 0 变到 1，damper (衰减器) 从 1 变到 0
            float percent = elapsed / duration;
            float damper = 1f - percent; 

            // 使用 UnityEngine.Random 明确指定命名空间，避免和 System 冲突
            float x = UnityEngine.Random.Range(-1f, 1f) * intensity * damper;
            float y = UnityEngine.Random.Range(-1f, 1f) * intensity * damper;

            // 更新偏移量，Update 方法会自动将其应用到 position 上
            shakeOffset = new Vector3(x, y, 0);

            // 使用 unscaledDeltaTime 确保在 Time.timeScale = 0（比如结算时）也能看到抖动
            elapsed += Time.unscaledDeltaTime; 
            yield return null;
        }

        // 抖动结束，将偏移量清零
        shakeOffset = Vector3.zero; 
    }
}