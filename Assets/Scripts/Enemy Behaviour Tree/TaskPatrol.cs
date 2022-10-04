using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using Pathfinding;

public class TaskPatrol : Node 
{
    private EnemyBT self;

    private List<Transform> waypoints;

    private int currentWaypointIndex = 0;
    private float waypointReachedDistance = 2.5f;

    public TaskPatrol(EnemyBT self)
    {
        this.self = self;

        waypoints = GetWaypoints();

        self.animator.SetFloat("Run", 1);
    }

    public override NodeState Evalute()
    {
        if (!self.aiDestinationSetter.target)
        {
            self.aiDestinationSetter.target = waypoints[currentWaypointIndex];
        }

        self.HandleStuck(true);
        self.FlipTowardsTarget();

        MoveTowardsWaypoints();

        return state = NodeState.RUNNING;
    }

    private void MoveTowardsWaypoints()
    {
        if (Vector2.Distance(self.transform.position, waypoints[currentWaypointIndex].position) < waypointReachedDistance && waypoints.Count - 1 > currentWaypointIndex)
        {
            currentWaypointIndex++;
            self.aiDestinationSetter.target = waypoints[currentWaypointIndex].transform;

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
