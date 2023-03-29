using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlashlightBehavior : MonoBehaviour
{
    //references

     private Light2D light;
    [SerializeField] PlayerMovement playerMovement;

    //variables

    //the amount of power left in the flashlight (serialized to make development easier)
    [SerializeField] [Range(0, 100)] private float power = 100f;
    //the normal intensity of the light
    [SerializeField] private float maxIntensity = 1f;
    [SerializeField] private float minIntensity = 0f;
    [SerializeField] private float maxRange = 1f;
    [SerializeField] private float minRange = 0f;
    [SerializeField] private float powerLossSpeed = 1f;
    [SerializeField] private float powerChargeSpeed = 1f;

    
    void Start()
    {
        //set up references
        light = this.gameObject.GetComponentInChildren<Light2D>();
    }

    
    void Update()
    {
        //determine if the flashlight is being charged or not
        if (Input.GetButton("Charge"))
        {
            light.enabled = false;
            playerMovement.canMove = false;
            power = Mathf.Clamp(power + (powerChargeSpeed * Time.deltaTime), 0, 100);
            
        }
        else
        {
            light.enabled = true;
            playerMovement.canMove = true;
            power = Mathf.Clamp(power - (powerLossSpeed * Time.deltaTime), 0, 100);
        }

        //update values with respect to power
        light.intensity = Mathf.Lerp(minIntensity, maxIntensity, (power / 100));
        light.pointLightOuterRadius = Mathf.Lerp(minRange, maxRange, (power / 100));
    }
}
