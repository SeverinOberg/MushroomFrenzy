using UnityEngine;
using BehaviourTree;

public class Wait : Node
{

    public float seconds;
    private float timeSinceNodeStart;

    public Wait(float seconds)
    {
        this.seconds = seconds;
    }

    public override NodeState Evalute()
    {
        timeSinceNodeStart += Time.deltaTime;
        if (timeSinceNodeStart < seconds) 
        {
            return state = NodeState.FAILURE;
        }

        timeSinceNodeStart = 0;
        return state = NodeState.SUCCESS;
    }
}
