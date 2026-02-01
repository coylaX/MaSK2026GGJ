using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LastClickMaskInfo : MonoBehaviour
{
    public static LastClickMaskInfo Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public TextMeshProUGUI effectText;   // Ð§¹ûÃèÊö

}
