using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
   public GameObject[] shixiaoGOs;
   public GameObject[] activeGOs;
    
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
        foreach (var go in activeGOs)
        {
            if (go != null)
                go.SetActive(true);
        }
       
    }
}
