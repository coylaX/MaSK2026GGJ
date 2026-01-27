using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmotionSource : MonoBehaviour
{
    // Start is called before the first frame update
    public Sprite XI;
    public Sprite NU;
    public Sprite AI;
    public Sprite LE;

    public EmotionTraitID emotionTraitID;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Image>().sprite == XI)
        {
            emotionTraitID = EmotionTraitID.XI;
        }
        if (GetComponent<Image>().sprite == NU)
        {
            emotionTraitID = EmotionTraitID.NU;
        }
        if (GetComponent<Image>().sprite == AI)
        {
            emotionTraitID = EmotionTraitID.AI;
        }
        if (GetComponent<Image>().sprite == LE)
        {
            emotionTraitID = EmotionTraitID.LE;
        }
    }
}
