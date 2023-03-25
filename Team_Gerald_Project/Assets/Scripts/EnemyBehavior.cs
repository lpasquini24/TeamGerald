using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

//this script requires these components on the enemy to function
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Seeker))]
public class EnemyBehavior : MonoBehaviour
{
    //serialized parameters

    //when the enemy is this far from the current waypoint, they move on to the next waypoint
    [SerializeField] float nextWaypointDistance;
    //the transform that the enemy will pathfind towards when chasing
    [SerializeField] Transform chaseTarget;
    
    //references

    private Seeker seeker;
    private Rigidbody2D rb;

    //private variables

    //stores the current path the enemy is following
    private Path p;

    
    void Start()
    {
        //get the components from our enemy
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        //auto-assign the target to the player
        if (chaseTarget == null) chaseTarget = GameObject.FindWithTag("Player").transform;

        //test generating a path
        GeneratePath(chaseTarget);
    }

    //creates a path and stores the value in p
    void GeneratePath(Transform target)
    {
        p = seeker.StartPath(transform.position, target.position);
    }
    
    void Update()
    {
        
    }

    private void FixedUpdate()
    {

    }
}
