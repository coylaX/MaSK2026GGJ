using UnityEngine;
using System.Collections.Generic;

public class TestOrderSubmitter : MonoBehaviour
{
    public string targetOrderID; // 在 Inspector 里填入你要测试的那个订单 ID
    public List<string> testTags; // 在 Inspector 里填入模拟的面具标签 (比如 "Sad")

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // 按空格提交
        {
            Debug.Log("正在模拟提交订单...");
            //OrderManager.Instance.SubmitOrder(targetOrderID, testTags);
        }
    }
}