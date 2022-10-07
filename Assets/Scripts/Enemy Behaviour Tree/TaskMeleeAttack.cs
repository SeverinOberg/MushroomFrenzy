using UnityEngine;
using BehaviourTree;

public class TaskMeleeAttack : Node
{

    private EnemyBT self;

    private float attackTimer;

    public TaskMeleeAttack(EnemyBT self)
    {
        this.self = self;
    }

    public override NodeState Evalute()
    {
        attackTimer += Time.deltaTime;
        if (attackTimer >= self.attackCooldown && self.IsWithinMeleeAttackRange())
        {
            attackTimer = 0;
                
            bool died = self.target.TakeDamage(Utilities.GetMinMaxDamageRoll(self.minDamage, self.maxDamage));
            
            self.animator.SetTrigger("Attack");
            if (died)
            {
                self.animator.SetFloat("Run", 1);
                return state = NodeState.SUCCESS;
            }
        }

        return state = NodeState.RUNNING;
    }
}
