using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.Rendering.Universal;

//this script requires these components on the enemy to function
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Seeker))]
public class EnemyBehavior : MonoBehaviour
{
    //enums

    //manages the state that the enemy is currently in
    private enum EnemyState { Start, Prowl, Hunt, Chase, Kill, Flee };

    //serialized parameters

    //when the enemy is this far from the current waypoint, they move on to the next waypoint
    [SerializeField] float nextWaypointDistance;

    //the player transform
    [SerializeField] Transform player;
    //the target that the enemy will follow when prowling
    [SerializeField] float prowlRadius;
    //the speed at which the enemy will prowl around
    [SerializeField] float prowlSpeed;
    //the speed at which the enemy will hunt the player
    [SerializeField] float huntSpeed;
    //the speed at which the enemy will chase the player
    [SerializeField] float chaseSpeed;
    //the speed at which the enemy will flee
    [SerializeField] float fleeSpeed;

    //references

    private Seeker seeker;
    private Rigidbody2D rb;
    private Light2D light;

    //private variables

    //stores the current path the enemy is following
    private Path p;
    //stores the waypoint that the enemy is located at
    private int currentWaypoint = 0;
    private bool pathCompleted = false;
    //stores the state that the enemy is currently in
    [SerializeField] private EnemyState state = EnemyState.Start;
    //stores the time since a path has been generated
    private float timeSincePath;
    private Vector3 spawnPoint;
    private Vector3 targetPoint;

    private EnemyState prevState;
    
    
    void Start()
    {
        //get the components from our enemy
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        light = GetComponentInChildren<Light2D>();
        //auto-assign the target to the player
        if (player == null) player = GameObject.FindWithTag("Player").transform;
        //autoassign the spawnpoint
        spawnPoint = transform.position;
        
    }

    //creates a path and stores the value in p
    void GeneratePath(Transform target)
    {
        //generate a path
        timeSincePath = 0f;
        p = seeker.StartPath(transform.position, target.position);
        currentWaypoint = 0;
        pathCompleted = false;
    }

    //overload method for a vector3
    void GeneratePath(Vector3 target)
    {
        //generate a path
        timeSincePath = 0f;
        p = seeker.StartPath(transform.position, target);
        currentWaypoint = 0;
        pathCompleted = false;
    }

    //applies a force pushing the rigidbody along the path
    void FollowPath(float amount)
    {
        Debug.Log(currentWaypoint);
        //check if path is null
        if (p == null || !p.IsDone()) return;
        //check if the path is completed
        if(currentWaypoint == p.vectorPath.Count-1)
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
        Debug.Log(prevState);
        if (prevState != state)
        {
            prevState = state;
            p = null;
        }
        //increment timers
        timeSincePath += Time.deltaTime;

        //the enemy state machine
        switch (state)
        {
            case EnemyState.Start:
                if (light.enabled == true) light.enabled = false;
                break;
            case EnemyState.Prowl:
                if (p == null || pathCompleted == true)
                {
                    targetPoint = player.position + new Vector3(Random.Range(-prowlRadius, prowlRadius), Random.Range(-prowlRadius, prowlRadius), 0f);   
                }
                if (timeSincePath > 0.3f) GeneratePath(targetPoint);
                FollowPath(prowlSpeed * Time.deltaTime);
                if (light.enabled == true) light.enabled = false;
                break;
            case EnemyState.Hunt:
                if (timeSincePath > 0.3f) GeneratePath(player);
                FollowPath(huntSpeed * Time.deltaTime);
                if (light.enabled == true) light.enabled = false;
                break;
            case EnemyState.Chase:
                if (timeSincePath > 0.3f) GeneratePath(player);
                FollowPath(chaseSpeed * Time.deltaTime);
                if (light.enabled == false) light.enabled = true;
                break;
            case EnemyState.Kill:
                if (timeSincePath > 0.3f) GeneratePath(player);
                FollowPath(chaseSpeed * Time.deltaTime);
                if (light.enabled == false) light.enabled = true;
                break;
            case EnemyState.Flee:
                if (timeSincePath > 0.3f) GeneratePath(spawnPoint);
                FollowPath(prowlSpeed * Time.deltaTime);
                if (light.enabled == true) light.enabled = false;
                break;
        }

        
    }

    private void FixedUpdate()
    {

    }

}
