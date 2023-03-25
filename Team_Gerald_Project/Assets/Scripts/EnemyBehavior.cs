using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

//this script requires these components on the enemy to function
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Seeker))]
public class EnemyBehavior : MonoBehaviour
{
    //enums

    //manages the state that the enemy is currently in
    private enum EnemyState { Passive, Chase, Attack };

    //serialized parameters

    //when the enemy is this far from the current waypoint, they move on to the next waypoint
    [SerializeField] float nextWaypointDistance;
    //the transform that the enemy will pathfind towards when chasing
    [SerializeField] Transform chaseTarget;
    //the speed at which the enemy will chase the target
    [SerializeField] float chaseSpeed;
    
    //references

    private Seeker seeker;
    private Rigidbody2D rb;

    //private variables

    //stores the current path the enemy is following
    private Path p;
    //stores the waypoint that the enemy is located at
    private int currentWaypoint = 0;
    private bool pathCompleted = false;
    //stores the state that the enemy is currently in
    private EnemyState state = EnemyState.Passive;
    //stores the time since a path has been generated
    private float timeSincePath;

    
    void Start()
    {
        //get the components from our enemy
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        //auto-assign the target to the player
        if (chaseTarget == null) chaseTarget = GameObject.FindWithTag("Player").transform;

        //makes the enemy chase the player by default, for testing
        state = EnemyState.Chase;
        
    }

    //creates a path and stores the value in p
    void GeneratePath(Transform target)
    {
        //generate a path
        timeSincePath = 0f;
        p = seeker.StartPath(transform.position, target.position);
    }

    //applies a force pushing the rigidbody along the path
    void FollowPath(float amount)
    {
        //check if path is null
        if (p == null) return;
        //check if the path is completed
        if(currentWaypoint == p.vectorPath.Count)
        {
            pathCompleted = true;
            return;
        }
        else
        {
            pathCompleted = false;
        }
        //find the direction to move in
        Vector2 direction = ((Vector2)p.vectorPath[currentWaypoint+1] - rb.position).normalized;
        //apply a force in that direction
        rb.AddForce(amount * direction);
        
        //check if we've reached the next waypoint
        if(Vector2.Distance(rb.position, (Vector2) p.vectorPath[currentWaypoint+1]) < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }
    
    void Update()
    {
        //increment timers
        timeSincePath += Time.deltaTime;

        //the enemy state machine
        switch (state)
        {
            case EnemyState.Passive:
                break;
            case EnemyState.Chase:
                //generates a path towards the chase target (by default, the player)
                //does not generate a path if one has been generated recently
                if(timeSincePath>0.3f) GeneratePath(chaseTarget);
                //move along the path at chasing speed
                FollowPath(chaseSpeed * Time.deltaTime);
                break;
            case EnemyState.Attack:
                break;
        }
    }

    private void FixedUpdate()
    {

    }

}
