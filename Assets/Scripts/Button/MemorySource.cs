using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemorySource : MonoBehaviour
{

    public MemoryTraitID memoryTraitID;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Image>().color == Color.red)
        {
            memoryTraitID = MemoryTraitID.A;
        }
        if (GetComponent<Image>().color == Color.yellow)
        {
            memoryTraitID = MemoryTraitID.B;
        }
        if (GetComponent<Image>().color == Color.blue)
        {
            memoryTraitID = MemoryTraitID.C;
        }
        //if (GetComponent<Image>().color == Color.green)
        //{
        //    colorTraitID = ColorTraitID.GREEN;
        //}
        //if (GetComponent<Image>().color == Color.black)
        //{
        //    colorTraitID = ColorTraitID.BLACK;
        //}
        //if (GetComponent<Image>().color == Color.white)
        //{
        //    colorTraitID = ColorTraitID.WHITE;
        //}
    }
}
