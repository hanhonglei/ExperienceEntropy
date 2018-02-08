using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour {
    public float patrolWaitTime = 1f;                       // The amount of time to wait when the patrol way point is reached.
    private int wayPointIndex = 0;                          // A counter for the way point array.
    private UnityEngine.AI.NavMeshAgent nav;                // Reference to the nav mesh agent.
    public Transform patrolWayPoints;                       // An array of transforms for the patrol route.
    private float patrolTimer;                              // A timer for the patrolWaitTime.

    // Use this for initialization
    void Start () {
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    // Update is called once per frame
    void Update () {
        Patrolling();
    }
    void Patrolling()
    {
        // If near the next waypoint or there is no destination...
        if (nav.remainingDistance < nav.stoppingDistance)
        {
            // ... increment the timer.
            patrolTimer += Time.deltaTime;

            // If the timer exceeds the wait time...
            if (patrolTimer >= patrolWaitTime)
            {
                // ... increment the wayPointIndex.
                if (wayPointIndex == patrolWayPoints.childCount - 1)
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
