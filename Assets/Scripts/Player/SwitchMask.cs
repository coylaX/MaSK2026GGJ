using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchMask : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public GameObject XI;
    public GameObject NU;
    public GameObject AI;
    public GameObject LE;

    SpriteRenderer spriteRenderer;
    // Update is called once per frame
    void Update()
    {
        if (GetComponent<MaskRead>().currentMask == null)
        {
            XI.SetActive(false);
            NU.SetActive(false);
            AI.SetActive(false);
            LE.SetActive(false);
        }

        switch (GetComponent<MaskRead>().currentMask.emotionTraitID)
        {
            case EmotionTraitID.None:
                XI.SetActive(false);
                NU.SetActive(false);
                AI.SetActive(false);
                LE.SetActive(false);
                break;
            case EmotionTraitID.XI:
                XI.SetActive(true);
                NU.SetActive(false);
                AI.SetActive(false);
                LE.SetActive(false);
                spriteRenderer = XI.GetComponent<SpriteRenderer>();
                break;
            case EmotionTraitID.NU:
                XI.SetActive(false);
                NU.SetActive(true);
                AI.SetActive(false);
                LE.SetActive(false);
                spriteRenderer = NU.GetComponent<SpriteRenderer>();
                break;
            case EmotionTraitID.AI:
                XI.SetActive(false);
                NU.SetActive(false);
                AI.SetActive(true);
                LE.SetActive(false);
                spriteRenderer = AI.GetComponent<SpriteRenderer>();
                break;
            case EmotionTraitID.LE:
                XI.SetActive(false);
                NU.SetActive(false);
                AI.SetActive(false);
                LE.SetActive(true);
                spriteRenderer = LE.GetComponent<SpriteRenderer>();
                break;
        }
        if (!spriteRenderer)
        {
            return;
        }

        switch (GetComponent<MaskRead>().currentMask.colorTraitID)
        {
            
            case ColorTraitID.RED:
                spriteRenderer.color = Color.red;
                break;
            case ColorTraitID.YELLOW:
                spriteRenderer.color = Color.yellow;
                break;
            case ColorTraitID.BLUE:
                spriteRenderer.color = Color.blue;
                break;
            case ColorTraitID.GREEN:
                spriteRenderer.color = Color.green;
                break;
            case ColorTraitID.BLACK:
                spriteRenderer.color = Color.gray;
                break;
            case ColorTraitID.WHITE:
                spriteRenderer.color = Color.white;
                break;
        }
    }
}
