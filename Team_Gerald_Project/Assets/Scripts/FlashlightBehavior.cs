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
    [SerializeField] private float flickerThreshold = 20f;
    [SerializeField] private float flickerAmount = 1f;
    [SerializeField] private float flickerLength = 0.1f;
    [SerializeField] private float minFlickerDelay = 0.2f;
    [SerializeField] private float maxFlickerDelay = 1f;

    private Coroutine co;
    private float intensity = 1f;
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
            light.enabled = (power > 0);
            playerMovement.canMove = true;
            power = Mathf.Clamp(power - (powerLossSpeed * Time.deltaTime), 0, 100);
        }

        //update values with respect to power
        intensity = Mathf.Lerp(minIntensity, maxIntensity, ((power) / 100));
        if (power > flickerThreshold) light.intensity = intensity;
        light.pointLightOuterRadius = Mathf.Lerp(minRange, maxRange, (power / 100));

        //start flickering if below the flicker threshold
        if (power < flickerThreshold)
        {
            if(co == null) co = StartCoroutine("Flicker");
        }
        else if(co!=null)
        {
            StopCoroutine(co);
            co = null;
        }

    }

    //coroutine for random flickering
    IEnumerator Flicker()
    {
        while (true)
        {
            light.intensity += Random.Range(-flickerAmount, flickerAmount);
            yield return new WaitForSeconds(Random.Range(0.0001f, flickerLength));
            light.intensity = intensity;
            yield return new WaitForSeconds(Random.Range(minFlickerDelay, minFlickerDelay));
        }
    }
}
