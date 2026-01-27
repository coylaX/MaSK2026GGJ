using UnityEngine;
using UnityEngine.UI;

public class SleepBarUI : MonoBehaviour
{
    public SleepHealth target; // 玩家身上的 SleepHealth
    public Slider slider;      // UI条

    private void Awake()
    {
        if (slider == null) slider = GetComponent<Slider>();
    }

    private void Start()
    {
        if (target != null && slider != null)
        {
            slider.minValue = 0f;
            slider.maxValue = target.maxSleep;
            slider.value = target.currentSleep;
        }
    }

    private void Update()
    {
        if (target == null || slider == null) return;

        // 如果 maxSleep 会变，确保条的上限同步
        if (slider.maxValue != target.maxSleep)
            slider.maxValue = target.maxSleep;

        slider.value = target.currentSleep;
    }
}
