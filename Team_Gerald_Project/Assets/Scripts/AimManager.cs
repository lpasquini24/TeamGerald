using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimManager : MonoBehaviour
{
   
    private Camera Cam;
    //[SerializeField]
    //private Transform Crosshair;
    [SerializeField]
    private Transform held;
    [SerializeField]
    private float HandOffset = 1f;
    public Vector3 to;
    private Transform Player;
    //we use a value to store all of our eulerangle changes and then implement them at the end
    //this is for strange magical unity angle reasons that I don't understand
    private Vector3 currentEulerAngles;
    // Start is called before the first frame update
    void Start()
    {
        Cam = Camera.main;
        Player = this.transform.parent;
        currentEulerAngles = transform.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        //find the mouse position in world space
        Vector3 mousePos = Cam.ScreenToWorldPoint(Input.mousePosition);
        //get the player's position
        Vector3 playerPos = Player.position;
        //if the mouse is on the left side of the player, flip the hand
        if (held != null && mousePos.x < playerPos.x)
        {
            //set the sprite to point left - I don't know why I flip with x and not Y here
            held.localEulerAngles = new Vector3(180f,0f,0f);
        }
        else
        {
            //set the sprite to point right
            held.localEulerAngles = new Vector3(0f,0f,0f);
        }
      //  if(Crosshair != null)
       // {
            //place a crosshair on the mouse cursor
        //    Crosshair.position = new Vector3(mousePos.x, mousePos.y, 0f);
       // }
        
        //get a vector going from the mouse position to the player position
        to = mousePos - playerPos;
        //calculate the angle of the vector
        float angle = (Mathf.Atan2(to.y, to.x) * Mathf.Rad2Deg);
        //rotate the gun to face the mouse cursor

        currentEulerAngles.z = angle;
        
        //move the hand to the correct position around the player (complicated vector math ig)
        this.transform.position = playerPos + new Vector3(HandOffset * Mathf.Cos(angle* Mathf.Deg2Rad), HandOffset * Mathf.Sin(angle * Mathf.Deg2Rad), 0f);
        //set our eulerangles to the current eulerangles
        transform.localEulerAngles = currentEulerAngles;
    }


}
