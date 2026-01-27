using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMCButton : MonoBehaviour
{
    public bool isEmotionButton;
    public bool isMemoryButton;
    public bool isColorButton;

    public GameObject emotionP;
    public GameObject memoryP;
    public GameObject colorP;
    public void OnClick()
    {
        if (isEmotionButton)
        {
            emotionP.SetActive(true);
            colorP.SetActive(false);
            memoryP.SetActive(false);
        }
        if (isMemoryButton)
        {
            memoryP.SetActive(true);
            colorP.SetActive(false);
            emotionP.SetActive(false);
        }
        if (isColorButton)
        {
            colorP.SetActive(true);
            memoryP.SetActive(false);
            emotionP.SetActive(false);
        }
    }
}
