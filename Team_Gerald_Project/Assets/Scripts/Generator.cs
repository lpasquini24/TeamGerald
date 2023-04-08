using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Generator : MonoBehaviour
{
    public float power;
    [SerializeField] private float batteryAmount = 10f;
    [SerializeField] public float lossRate = 1f;
    public Slider slider;
    //public float timePassed = 0f;
    //public float waitTime = 10f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = power/100f;
        if(power > 0)
        {
            power -= lossRate * Time.deltaTime;
            if (power < 0f) power = 0f;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            while(power != 100 && PlayerMovement.batteryCount > 0)
            {
                PlayerMovement.batteryCount -= 1;
                power += batteryAmount;
            }
        }
    }
}
