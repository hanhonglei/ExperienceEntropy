using UnityEngine;
using System.Collections;

public class EnemyBehavior : Actor {
    public float patrolSpeed = 2f;                          // The nav mesh agent's speed when patrolling.
    public float chaseSpeed = 5f;                           // The nav mesh agent's speed when chasing.
    public float chaseWaitTime = 5f;                        // The amount of time to wait when the last sighting is reached.
    public float patrolWaitTime = 1f;                       // The amount of time to wait when the patrol way point is reached.
    public Transform patrolWayPoints;                     // An array of transforms for the patrol route.
    public float shootRotSpeed = 5f;                        // 瞄准时候旋转朝向的速度
    public GameObject disappearEffect = null;
    public float  effectDurationTime = 1f;

    
    
    private EnemySight enemySight;                          // Reference to the EnemySight script.
    private UnityEngine.AI.NavMeshAgent nav;                               // Reference to the nav mesh agent.
    private Transform player;                               // Reference to the player's transform.
    //private PlayerHealth playerHealth;                      // Reference to the PlayerHealth script.
    //private LastPlayerSighting lastPlayerSighting;          // Reference to the last global sighting of the player.
    private float chaseTimer;                               // A timer for the chaseWaitTime.
    private float patrolTimer;                              // A timer for the patrolWaitTime.
    private int wayPointIndex;                              // A counter for the way point array.

    private bool chase = false;                                     // 当遇到攻击或者在射击的时候玩家跑开的话
	// Update is called once per frame
    //void Update () {
    //    if((int)(Random.value * 500) == 1)
    //        Fire(player.position);	
    //}
    protected override void Die()
    {
        //SendMessage("DropItem");
        player.GetComponent<PlayerControl>().Kill();
        if (disappearEffect)
        {
            GameObject g = (GameObject)GameObject.Instantiate(disappearEffect, transform.position, Quaternion.identity);
            Destroy(g, effectDurationTime);
        }

        GameObject.Destroy(gameObject);
    }
    //public void Fire(Vector3 target)
    //{
    //    Vector3 dir = target - bulletPos.position;
    //    Vector3 xyProject = Vector3.ProjectOnPlane(dir, Vector3.up);
    //    GameObject go = (GameObject)Instantiate(bullet, bulletPos.position, Quaternion.FromToRotation(Vector3.forward, xyProject.normalized));
    //    go.GetComponent<bulletBehavior>().isEnemyBullet = true;

    //}    
    
    void Awake ()
    {
        // Setting up the references.
        enemySight = transform.Find("EnemySight").GetComponent<EnemySight>();
        Debug.Assert(enemySight);
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag(Tags.player).transform;
        //Debug.Log(player);
        //playerHealth = player.GetComponent<PlayerHealth>();
        //lastPlayerSighting = GameObject.FindGameObjectWithTag("GameController").GetComponent<LastPlayerSighting>();
    }
    
    
    void Update ()
    {
        //Debug.Log("Update!");
        // If the player is in sight and is alive...
        if (enemySight.playerInSight /*&& player.gameObject.GetComponent<PlayerControl>().health > 0f*/)
        // ... shoot.
        {
            Shooting();
            chase = true;
        }

        // If the player has been sighted and isn't dead...
        else if (chase /*&& player.gameObject.GetComponent<PlayerControl>().health > 0f*/)
            // ... chase.
            Chasing();

        // Otherwise...
        else
        // ... patrol.
        {
            Patrolling();
            //Debug.Log("Patrol!");
        }
    }

    override public void Damage(float damage)
    {
        //Debug.Log("enemy damaged!");
        if (!enemySight.playerInSight)
        {
            enemySight.personalLastSighting = player.position;
            chase = true;
        }
        base.Damage(damage);
    }

    
    void Shooting ()
    {
        Vector3 lookPos = player.position;
        lookPos.y = transform.position.y;

        Vector3 targetDir = lookPos - transform.position;
        float step = shootRotSpeed * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
        transform.rotation = Quaternion.LookRotation(newDir);

        //transform.LookAt(lookPos);
        // Stop the enemy where it is.
        Fire(player.position);
        nav.Stop();
             //Debug.Log("Shoot player!");
   }
    
    
    void Chasing ()
    {
        //Debug.Log("Chasing");
        nav.Resume();
        StopFire();
        // Create a vector from the enemy to the last sighting of the player.
        Vector3 sightingDeltaPos = enemySight.personalLastSighting - transform.position;
        
        // If the the last personal sighting of the player is not close...
        if(sightingDeltaPos.sqrMagnitude > 4f)
            // ... set the destination for the NavMeshAgent to the last personal sighting of the player.
            nav.destination = enemySight.personalLastSighting;
        
        // Set the appropriate speed for the NavMeshAgent.
        nav.speed = chaseSpeed;
        
        // If near the last personal sighting...
        if (nav.remainingDistance < nav.stoppingDistance)
        {
            // ... increment the timer.
            chaseTimer += Time.deltaTime;

            // If the timer exceeds the wait time...
            if (chaseTimer >= chaseWaitTime)
            {
                // ... reset last global sighting, the last personal sighting and the timer.
                //lastPlayerSighting.position = lastPlayerSighting.resetPosition;
                chase = false;
                chaseTimer = 0f;
            }
        }
        else
            // If not near the last sighting personal sighting of the player, reset the timer.
            chaseTimer = 0f;
    }

    
    void Patrolling ()
    {
        nav.Resume();
        StopFire();
        // Set an appropriate speed for the NavMeshAgent.
        nav.speed = patrolSpeed;
        
        // If near the next waypoint or there is no destination...
        if(nav.remainingDistance < nav.stoppingDistance)
        {
            // ... increment the timer.
            patrolTimer += Time.deltaTime;
            
            // If the timer exceeds the wait time...
            if(patrolTimer >= patrolWaitTime)
            {
                // ... increment the wayPointIndex.
                if(wayPointIndex == patrolWayPoints.childCount - 1)
                    wayPointIndex = 0;
                else
                    wayPointIndex++;
                
                // Reset the timer.
                patrolTimer = 0;
            }
        }
        else
            // If not near a destination, reset the timer.
            patrolTimer = 0;
        
        // Set the destination to the patrolWayPoint.
        nav.destination = patrolWayPoints.GetChild(wayPointIndex).position;
    }
}