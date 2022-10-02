using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using Pathfinding;

public class TaskPatrol : Node 
{
    private readonly EnemyBT    self;
    private Animator            animator;
    private AIPath              aiPath;
    private AIDestinationSetter aIDestinationSetter;

    private List<Transform> waypoints;

    private int currentWaypointIndex = 0;
    private float waypointReachedDistance = 1.0f;

    public TaskPatrol(EnemyBT self)
    {
        this.self = self;

        waypoints = GetWaypoints();

        animator            = self.GetComponent<Animator>();
        aiPath              = self.GetComponent<AIPath>();
        aIDestinationSetter = self.GetComponent<AIDestinationSetter>();

        aIDestinationSetter.target = waypoints[0];
        animator.SetFloat("Run", 1);
    }

    public override NodeState Evalute()
    {
        MoveTowardsWaypoints();

        state = NodeState.RUNNING;
        return state;
    }

    private void MoveTowardsWaypoints()
    {
        if (Vector2.Distance(self.transform.position, waypoints[currentWaypointIndex].position) < waypointReachedDistance && waypoints.Count - 1 > currentWaypointIndex)
        {
            currentWaypointIndex++;
            aIDestinationSetter.target = waypoints[currentWaypointIndex].transform;

            if (currentWaypointIndex >= waypoints.Count - 1)
            {
                currentWaypointIndex = 0;
            }
        }
    }

    private List<Transform> GetWaypoints()
    {
        Transform[] foundWaypoints = GameObject.Find("Waypoints").GetComponentsInChildren<Transform>();
        List<Transform> waypoints = new List<Transform>();
        for (int i = 0; i < foundWaypoints.Length; i++)
        {
            // Skip the empty parent
            if (i == 0)
            {
                continue;
            }
            waypoints.Add(foundWaypoints[i]);
        }

        return waypoints;
    }

}
