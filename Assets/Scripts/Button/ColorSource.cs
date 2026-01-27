using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorSource : MonoBehaviour
{
    // Start is called before the first frame update
    

    public ColorTraitID colorTraitID;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Image>().color == Color.red)
        {
            colorTraitID =ColorTraitID.RED;
        }
        if (GetComponent<Image>().color == Color.yellow)
        {
            colorTraitID = ColorTraitID.YELLOW;
        }
        if (GetComponent<Image>().color == Color.blue)
        {
            colorTraitID = ColorTraitID.BLUE;
        }
        if (GetComponent<Image>().color == Color.green)
        {
            colorTraitID = ColorTraitID.GREEN;
        }
        if (GetComponent<Image>().color == Color.black)
        {
            colorTraitID = ColorTraitID.BLACK;
        }
        if (GetComponent<Image>().color == Color.white)
        {
            colorTraitID = ColorTraitID.WHITE;
        }
    }
}
