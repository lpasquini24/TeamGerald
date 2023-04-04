using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public int power;
    public float timePassed = 0f;

    // Start is called before the first frame update
    void Start()
    {
        power = 80;
    }

    // Update is called once per frame
    void Update()
    {
        timePassed += Time.deltaTime;
        if (timePassed > 10f && power!=0f)
        {
            timePassed = 0;
            power -= 10;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            while(power != 100 && PlayerMovement.batteryCount > 0)
            {
                PlayerMovement.batteryCount -= 1;
                power += 10;
            }
        }
    }
}
