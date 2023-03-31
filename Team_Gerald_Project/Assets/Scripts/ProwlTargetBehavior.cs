using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProwlTargetBehavior : MonoBehaviour
{
    //variables
    private Transform player;
    [SerializeField] float radius;

   
    void Start()
    {
        if (player == null) player = GameObject.FindWithTag("Player").transform;
    }

    
    void Update()
    {
        
    }
}
