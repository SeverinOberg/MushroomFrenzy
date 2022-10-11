using UnityEngine;
using BehaviourTree;

public class CheckEnemyInMeleeRange : Node 
{

    private EnemyBT self;

    public CheckEnemyInMeleeRange(EnemyBT self)
    {
        this.self = self; ;
    }

    public override NodeState Evalute()
    {
        if (!self.target || self.target.IsDead)
        {
            return state = NodeState.FAILURE; ;
        }

        if (Vector2.Distance(self.transform.position, self.target.transform.position) <= self.meleeAttackRange)
        {
            return state = NodeState.SUCCESS;
        }

        return state = NodeState.FAILURE;
    }

}
