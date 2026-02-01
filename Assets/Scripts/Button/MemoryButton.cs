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
    [TextArea] public string effectContent = "1";

    public void OnClick()
    {
        effectText.text= effectContent;
        emotionSource.GetComponent<Image>().sprite = GetComponent<Image>().sprite;
    }
}
