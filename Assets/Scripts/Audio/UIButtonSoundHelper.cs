using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIButtonSoundHelper : MonoBehaviour
{
    void Start()
    {
        BindAllButtons();
    }

    public void BindAllButtons()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);

        foreach (var btn in buttons)
        {
            // 1. 绑定点击音效
            btn.onClick.RemoveListener(OnBtnClick); // 防止重复绑定
            btn.onClick.AddListener(OnBtnClick);

            // 2. 绑定进入和退出音效 (通过 EventTrigger)
            EventTrigger trigger = btn.gameObject.GetComponent<EventTrigger>();
            if (trigger == null) trigger = btn.gameObject.AddComponent<EventTrigger>();

            // 清理旧监听
            trigger.triggers.RemoveAll(x => x.eventID == EventTriggerType.PointerEnter || x.eventID == EventTriggerType.PointerExit);

            // 鼠标移入
            var enterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            enterEntry.callback.AddListener((data) => { AudioManager.Instance.PlayUI(AudioManager.Instance.btnHover); });
            trigger.triggers.Add(enterEntry);

            // 鼠标移出
            var exitEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            exitEntry.callback.AddListener((data) => { AudioManager.Instance.PlayUI(AudioManager.Instance.btnExit); });
            trigger.triggers.Add(exitEntry);
        }
    }

    private void OnBtnClick()
    {
        AudioManager.Instance.PlayUI(AudioManager.Instance.btnClick);
    }
}