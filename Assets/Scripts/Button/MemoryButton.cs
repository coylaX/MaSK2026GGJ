using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MemoryButton : MonoBehaviour
{
    public MemoryTraitID memoryID;

    public MemorySource memorySource;

    public TextMeshProUGUI effectText;   // Ð§¹ûÃèÊö
    [TextArea] public string effectContent = "1";

    public void OnClick()
    {
        effectText.text= effectContent;
        memorySource.GetComponent<Image>().sprite = GetComponent<Image>().sprite;
        memorySource.memoryTraitID = memoryID;
    }
}
