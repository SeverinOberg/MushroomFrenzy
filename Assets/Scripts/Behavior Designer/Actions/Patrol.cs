using BehaviorDesigner.Runtime.Tasks;
using System.Collections.Generic;
using UnityEngine;

[TaskDescription("Gets the waypoints in our level OnAwake() and begins to patrol between them.")]
public class Patrol : Action
{
    [SerializeField] private SharedEnemy self;
    [SerializeField] private float waypointReachedDistance = 2.5f;

    private List<Transform> waypoints;
    private int   currentWaypointIndex = 0;
    

    public override void OnAwake()
    {
        waypoints = GetWaypoints();
    }

    public override TaskStatus OnUpdate()
    {
        if (!self.Value.aiDestinationSetter.target)
        {
            self.Value.aiDestinationSetter.target = waypoints[currentWaypointIndex];
        }

        if (Vector2.Distance(transform.position, waypoints[currentWaypointIndex].position) < waypointReachedDistance && waypoints.Count - 1 > currentWaypointIndex)
        {
            currentWaypointIndex++;
            self.Value.aiDestinationSetter.target = waypoints[currentWaypointIndex].transform;

            if (currentWaypointIndex >= waypoints.Count)
            {
                currentWaypointIndex = 0;
            }
        }

        return TaskStatus.Success;
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
