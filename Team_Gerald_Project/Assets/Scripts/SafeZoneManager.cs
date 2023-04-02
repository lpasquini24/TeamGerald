using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
public class SafeZoneManager : MonoBehaviour
{
    private Light2D light;
    private Generator gen;
    private bool playerInside = false;
    public bool playerIsSafe = false;

    void Start()
    {
        //get references from children
        light = GetComponentInChildren<Light2D>();
        gen = GetComponentInChildren<Generator>();
    }

  
    void Update()
    {
        //set light based on power
        if (gen.power == 0)
        {
            light.enabled = false;
        }
        else
        {
            light.enabled = true;
        }

        //determine if the player is safe
        if(playerInside && gen.power > 0)
        {
            playerIsSafe = true;
        }
        else
        {
            playerIsSafe = false;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInside = false;
        }
    }
}
