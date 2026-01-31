using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MemoryButton : MonoBehaviour
{
    public bool red;
    public bool yellow;
    public bool blue;
    public bool green;
    public bool black;
    public bool white;

    public GameObject emotionSource;

    public TextMeshProUGUI effectText;   // Ð§¹ûÃèÊö
    [TextArea] public string effectContent = "";

    public void OnClick()
    {
        effectText.text= effectContent;
        if (red)
        {
            emotionSource.GetComponent<Image>().color = Color.red;
        }
        if (yellow)
        {
            emotionSource.GetComponent<Image>().color = Color.yellow;
        }
        if (blue)
        {
            emotionSource.GetComponent<Image>().color = Color.blue;
        }
    }
}
