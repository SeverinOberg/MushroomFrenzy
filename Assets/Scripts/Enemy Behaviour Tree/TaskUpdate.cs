using UnityEngine;
using BehaviourTree;

public class TaskUpdate : Node 
{
    EnemyBT self;

    public TaskUpdate(EnemyBT self)
    {
        this.self = self;
    }

    public override NodeState Evalute()
    {
        Debug.Log("TaskUpdate called");
        Utilities.ForceReduceVelocity(ref self.rb);
        return state = NodeState.SUCCESS;
    }
    
}
