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

        if (IsTargetTooFarAway())
        {
            self.ClearTarget();
            return state = NodeState.SUCCESS;
        }

        if (self.IsWithinMeleeAttackRange())
        {
            return state = NodeState.SUCCESS;
        }
  
        return state = NodeState.RUNNING; ;
    }

    private bool IsTargetTooFarAway()
    {
        if (Utilities.GetDistanceBetween(self.transform.position, self.target.transform.position) > self.scanDiameter)
        {
            return true;
        }

        return false;
    }

}
