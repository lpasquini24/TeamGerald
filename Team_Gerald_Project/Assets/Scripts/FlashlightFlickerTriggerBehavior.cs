using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightFlickerTriggerBehavior : MonoBehaviour
{
    public FlashlightBehavior flashlight;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(flashlight.power > 30f)
            {
                flashlight.power = 30f;
                Destroy(this.gameObject);
            }
        }
    }
}
