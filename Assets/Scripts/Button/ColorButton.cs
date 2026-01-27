using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorButton : MonoBehaviour
{
    // Start is called before the first frame update
    public bool red;
    public bool yellow;
    public bool blue;
    public bool green;
    public bool black;
    public bool white;

    public GameObject emotionSource;

    

    public void OnClick()
    {
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
        if (green)
        {
            emotionSource.GetComponent<Image>().color = Color.green;
        }
        if (black)
        {
            emotionSource.GetComponent<Image>().color = Color.black;
        }
        if (white)
        {
            emotionSource.GetComponent<Image>().color = Color.white;
        }
    }
}
