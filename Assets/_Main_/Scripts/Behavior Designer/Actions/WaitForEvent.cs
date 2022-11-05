using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Pathfinding;
using UnityEngine;

[TaskDescription("Will continue to wait/run until an end event is recieved")]
[TaskIcon("{SkinColor}WaitIcon.png")]
public class WaitForEvent : Action
{

    //[SerializeField] private AIPath aiPath;
    [SerializeField] private string eventName;

    //private float initialMaxSpeed;

    private bool eventReceieved;
    private bool registered;

    public override void OnStart()
    {
        if (!registered) 
        {
            Owner.RegisterEvent(eventName, OnEventReceived);
            registered = true;
        }
        
        //initialMaxSpeed = aiPath.maxSpeed; 
        //aiPath.maxSpeed = 0;
    }

    public override TaskStatus OnUpdate()
    {
        return eventReceieved ? TaskStatus.Success : TaskStatus.Running;
    }

    public override void OnEnd()
    {
        Owner.UnregisterEvent(eventName, OnEventReceived);

        //aiPath.maxSpeed = initialMaxSpeed;

        eventReceieved  = false;
        registered      = false;
    }

    private void OnEventReceived()
    {
        eventReceieved = true;
    }

}
