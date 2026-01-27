using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
   public GameObject[] shixiaoGOs;
   public GameObject[] dongjieGOs;
    public GameObject PLAYER;
   public void onClick()
    {
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
    }
}
