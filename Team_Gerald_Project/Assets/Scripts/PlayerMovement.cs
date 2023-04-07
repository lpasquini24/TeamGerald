using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float Acceleration;
    [SerializeField]
    private float MaxSpeed;
    private Rigidbody2D rb;
    public bool canMove = true;
    static public int batteryCount;
    [SerializeField] TextMeshProUGUI batteryCounter;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        batteryCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        batteryCounter.text = batteryCount.ToString();
        Vector2 NewVelocity = rb.velocity;
        NewVelocity.x += Input.GetAxisRaw("Horizontal") * Acceleration* Time.deltaTime;
        //if (rb.velocity.x > MaxSpeed)
        //{
        //    NewVelocity.x = MaxSpeed;
        //}
        //else if(rb.velocity.x < -MaxSpeed)
        //{
        //    NewVelocity.x = -MaxSpeed;
        //}
        NewVelocity.y += Input.GetAxisRaw("Vertical") * Acceleration * Time.deltaTime;
        //if (rb.velocity.y > MaxSpeed)
        //{
        //   NewVelocity.y = MaxSpeed;
        //}else if (rb.velocity.y < -MaxSpeed)
        //{
        //   NewVelocity.y = -MaxSpeed;
        //}
        if (NewVelocity.magnitude > MaxSpeed) NewVelocity = NewVelocity.normalized * MaxSpeed;
        if (canMove) rb.velocity = NewVelocity;
    }


}