using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskFlip : MonoBehaviour
{
    // Start is called before the first frame update
    SpriteRenderer spriteRenderer;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        // A 取消 flipX，D 勾上 flipX（按下那一下触发）
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (spriteRenderer != null) spriteRenderer.flipX = true;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            if (spriteRenderer != null) spriteRenderer.flipX = false;
        }
    }

}
