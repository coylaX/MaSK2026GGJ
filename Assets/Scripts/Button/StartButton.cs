using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
   public GameObject[] shixiaoGOs;
   public GameObject[] dongjieGOs;
    public GameObject PLAYER;
    public GameObject HPBar;
   public void onClick()
    {
        if (BackPackLogic.I.maskInstances.Count < 3)
        {
            return;
        }
        foreach (var go in shixiaoGOs)
        {
            if (go != null)
                go.SetActive(false);
        }
        foreach (var go in dongjieGOs)
        {
            if (go != null)
                go.GetComponent<Button>().interactable = false;
        }
        PLAYER.SetActive(true);
        PLAYER.GetComponent<MaskRead>().onStart();
        HPBar.SetActive(true);
    }
}
