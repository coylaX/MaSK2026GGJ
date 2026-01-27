using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackPackLogic : MonoBehaviour
{
    // Start is called before the first frame update
    public  List<MaskInstance> maskInstances;

    public static BackPackLogic I { get; private set; }
    void Awake()
    {
        I = this;
        maskInstances = new List<MaskInstance>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
