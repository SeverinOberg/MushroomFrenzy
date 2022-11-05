using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityTransform;
using Pathfinding;
using System.Collections.Generic;
using UnityEngine;

[TaskDescription("Gets the waypoints in our level OnAwake() and begins to patrol between them.")]
public class Patrol : Action
{
    [Header("Required")]
    [SerializeField] private AIDestinationSetter aiDestinationSetter;
    [SerializeField] private SharedTransform     currentWaypoint;
    [SerializeField] private float               waypointReachedDistance = 2.5f;
    [SerializeField] private string              waypointsParentName;

    private Transform waypointsParent;
    
    private List<Transform> waypoints;
    private int currentWaypointIndex = 0;

    public override void OnAwake()
    {
        waypointsParent = GameObject.Find(waypointsParentName).GetComponent<Transform>();
        waypoints = GetWaypoints();
        currentWaypoint.Value = waypoints[currentWaypointIndex];
    }

    public override void OnStart()
    {
        aiDestinationSetter.target = waypoints[currentWaypointIndex];
    }

    public override TaskStatus OnUpdate()
    {

        if (Vector2.Distance(transform.position, waypoints[currentWaypointIndex].position) < waypointReachedDistance && waypoints.Count >= currentWaypointIndex)
        {
            currentWaypointIndex++;

            if (currentWaypointIndex >= waypoints.Count) 
            {
                currentWaypointIndex = 0;
            }

            
            aiDestinationSetter.target = waypoints[currentWaypointIndex].transform;
            currentWaypoint.Value = aiDestinationSetter.target;
        }

        if (!IsPathPossible.Validate(transform, aiDestinationSetter.target))
        {
            return TaskStatus.Failure;
        }

        return TaskStatus.Running;
    }

    private List<Transform> GetWaypoints()
    {
        Transform[] foundWaypoints = waypointsParent.GetComponentsInChildren<Transform>();
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
