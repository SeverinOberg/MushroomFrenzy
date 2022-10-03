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
        if (!self.target || self.target.isDead)
        {
            state = NodeState.FAILURE;
            return state;
        }

        if (Vector2.Distance(self.transform.position, self.target.transform.position) <= self.meleeAttackRange)
        {
            self.animator.SetFloat("Run", 0);
            state = NodeState.SUCCESS;
            return state;
        }

        self.animator.SetFloat("Run", 1);
        state = NodeState.FAILURE;
        return state;
    }

}
