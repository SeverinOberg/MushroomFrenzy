using UnityEngine;
using BehaviourTree;

public class TaskChase : Node
{

    private EnemyBT self;

    public TaskChase(EnemyBT self)
    {
        this.self = self;
    }

    public override NodeState Evalute()
    {
        self.HandleStuck();
        self.FlipTowardsTarget();

        if (self.IsWithinMeleeAttackRange())
        {
            state = NodeState.SUCCESS;
            return state;
        }
  
        state = NodeState.RUNNING;
        return state;
    }

}
