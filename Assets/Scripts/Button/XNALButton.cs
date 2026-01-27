using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XNALButton : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isXI;
    public bool isNU;
    public bool isAI;
    public bool isLE;
    public GameObject emotionSource;

    public Sprite XI;
    public Sprite NU;
    public Sprite AI;
    public Sprite LE;

    public void OnClick()
    {
        if (isXI)
        {
            emotionSource.GetComponent<Image>().sprite = XI;
        }
        if (isNU)
        {
            emotionSource.GetComponent<Image>().sprite = NU;
        }
        if (isAI)
        {
            emotionSource.GetComponent<Image>().sprite = AI;
        }
        if (isLE)
        {
            emotionSource.GetComponent<Image>().sprite = LE;
        }
    }
}
