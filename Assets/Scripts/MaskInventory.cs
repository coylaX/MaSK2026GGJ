using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskInventory : MonoBehaviour
{
    //Âß¼­²Ö¿â
    public List<MaskInstance> maskInstances;

    public static MaskInventory I { get; private set; }

    private void Awake()
    {
        maskInstances = new List<MaskInstance>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
