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
    private enum EnemyState { Start, Prowl, Hunt, Flee, Chase, Kill, Freeze };

    //serialized parameters

    //when the enemy is this far from the current waypoint, they move on to the next waypoint
    [SerializeField] float nextWaypointDistance;

    //the player transform
    [SerializeField] Transform player;
    //the target that the enemy will follow when prowling
    [SerializeField] float prowlRadius;
    //the distance from the player at which the enemy becomes enraged
    [SerializeField] float rageDistance;
    //the speed at which the enemy will prowl around
    [SerializeField] float prowlSpeed;
    //the speed at which the enemy will hunt the player
    [SerializeField] float huntSpeed;
    //the speed at which the enemy will chase the player
    [SerializeField] float chaseSpeed;
    //this is a lil private variable for the speed increase while chasing
    [SerializeField] private float variableChaseSpeed;
    //the speed at which the enemy will flee
    [SerializeField] float fleeSpeed;
    //the passive aggression increase rate
    [SerializeField] float aggressionIncreaseRate;
    //the aggression threshold for hunting
    [SerializeField] float aggressionHuntThreshold;
    //the aggression threshold for chasing
    [SerializeField] float aggressionChaseThreshold;
    //the time the enemy can be in the flashlight beam before chasing
    [SerializeField] float spottedTimeChaseThreshold;
    //the passive decrease rate of the 'spottedtime' metere
    [SerializeField] float spottedTimeDecreaseRate;

 

    //references

    private Seeker seeker;
    private Rigidbody2D rb;
    private Light2D light;
    private Animator anim;
    private AudioSource source;

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
    //if the monster is within the player's flashlight beam
    [SerializeField] public bool inBeam;
    private EnemyState prevState;
    private SafeZoneManager safeZone;
    public LayerMask lookForPlayer;

    //increases over time, makes the monster more aggressive
    [SerializeField] private float aggression = 0;
    //manages monster behavior when within the player's flashlight beam
    [SerializeField] private float spottedTime = 0;

    

    
    
    void Start()
    {
        //get the components from our enemy
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        light = GetComponentInChildren<Light2D>();
        safeZone = GameObject.FindWithTag("SafeZone").GetComponent<SafeZoneManager>();
        anim = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
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
        anim.SetInteger("State", (int) state);
        if (prevState != state)
        {
            if (state == EnemyState.Chase) AudioManager.sharedInstance.Play("Growl");
            
            prevState = state;
            variableChaseSpeed = chaseSpeed;
            p = null;
            
        }
        //increment timers
        timeSincePath += Time.deltaTime;

        //the enemy state machine
        //transitions that occur from multiple states
        if ((state != EnemyState.Start && state != EnemyState.Flee) && safeZone.playerIsSafe) state = EnemyState.Flee;
        if (state != EnemyState.Freeze) spottedTime = Mathf.Clamp(spottedTime - (spottedTimeDecreaseRate * Time.deltaTime), 0, Mathf.Infinity);

        switch (state)
        {
            case EnemyState.Start:
                aggression = 0;
                if (source.isPlaying) source.Stop();
                if (light.enabled == true) light.enabled = false;
                if (!safeZone.playerIsSafe) state = EnemyState.Prowl;
                break;
            case EnemyState.Prowl:
                aggression += aggressionIncreaseRate * Time.deltaTime;
                if (source.isPlaying) source.Stop();
                if (p == null || pathCompleted == true)
                {
                    targetPoint = player.position + new Vector3(Random.Range(-prowlRadius, prowlRadius), Random.Range(-prowlRadius, prowlRadius), 0f);   
                }
                if (timeSincePath > 0.3f) GeneratePath(targetPoint);
                FollowPath(prowlSpeed * Time.deltaTime);
                if (light.enabled == true) light.enabled = false;
                //enrage code
                if(Vector3.Distance(player.position, transform.position) < rageDistance) state = EnemyState.Chase;
                if (aggression > aggressionHuntThreshold) state = EnemyState.Hunt;
                //freeze if the player is lookin at ya
                if (isSeenByBeam()) state = EnemyState.Freeze;
                break;
            case EnemyState.Hunt:
                aggression += aggressionIncreaseRate * Time.deltaTime;
                if (timeSincePath > 0.3f) GeneratePath(player);
                FollowPath(huntSpeed * Time.deltaTime);
                if (source.isPlaying) source.Stop();
                if (light.enabled == true) light.enabled = false;
                //enrage code
                if (Vector3.Distance(player.position, transform.position) < rageDistance) state = EnemyState.Chase;
                if (aggression > aggressionChaseThreshold) state = EnemyState.Chase;
                //freeze if player lookin at ya
                if (isSeenByBeam()) state = EnemyState.Freeze;
                //go back to prowling, provided its not aggressive
                if (spottedTime == 0 && aggression < aggressionHuntThreshold) state = EnemyState.Prowl;
                break;
            case EnemyState.Chase:
                if (!source.isPlaying) source.Play();
                source.volume = 1;
                if (timeSincePath > 0.3f) GeneratePath(player);
                FollowPath(variableChaseSpeed * Time.deltaTime);
                variableChaseSpeed += 0.7f * Time.deltaTime;
                if (light.enabled == false) light.enabled = true;
                break;
            case EnemyState.Kill:
                if (timeSincePath > 0.3f) GeneratePath(player);
                FollowPath(variableChaseSpeed * Time.deltaTime);
                variableChaseSpeed += 1f * Time.deltaTime;
                if (light.enabled == false) light.enabled = true;
                if (!source.isPlaying) source.Play();
                source.volume = 1;
                break;
            case EnemyState.Flee:
                if (source.isPlaying) source.Stop();
                aggression = 0;
                if (timeSincePath > 0.3f) GeneratePath(spawnPoint);
                FollowPath(prowlSpeed * Time.deltaTime);
                if (light.enabled == true) light.enabled = false;
                if (pathCompleted) state = EnemyState.Start;
                break;
            case EnemyState.Freeze:
                if (!source.isPlaying) source.Play();
                source.volume = Mathf.Lerp(0f,0.6f,spottedTime);
                if (light.enabled == false) light.enabled = true;
                if (!isSeenByBeam())
                {
                    if (spottedTime > 0.5 * spottedTimeChaseThreshold) state = EnemyState.Hunt; else state = EnemyState.Prowl;
                }
                if (spottedTime > spottedTimeChaseThreshold) state = EnemyState.Chase;
                spottedTime += Time.deltaTime;
                break;
        }

        
    }

    private void FixedUpdate()
    {

    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("FlashlightBeam")) inBeam = true;
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("FlashlightBeam")) inBeam = false;
    }

    private bool isSeenByBeam()
    {
        FlashlightBehavior flashlight = player.gameObject.GetComponentInChildren<FlashlightBehavior>();
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, (player.position - transform.position), flashlight.light.pointLightOuterRadius - 2f, lookForPlayer);
        return (inBeam && hit.collider != null && hit.collider.gameObject.CompareTag("Player") && flashlight.isActive);
    }


}
