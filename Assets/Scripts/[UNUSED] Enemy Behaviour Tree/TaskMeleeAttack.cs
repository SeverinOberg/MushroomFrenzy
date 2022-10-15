//using UnityEngine;
//using BehaviourTree;

//public class TaskMeleeAttack : Node
//{

//    private EnemyBT self;

//    private float attackTimer;

//    public TaskMeleeAttack(EnemyBT self)
//    {
//        this.self = self;
//    }

//    public override NodeState Evalute()
//    {
//        if (!self.target)
//        {
//            return state = NodeState.FAILURE;
//        }

//        if (self.target.IsDead || !self.IsWithinMeleeAttackRange())
//        {
//            self.ClearTarget();
//            return state = NodeState.FAILURE;
//        }
            
//        attackTimer += Time.deltaTime;
//        if (attackTimer >= self.attackCooldown)
//        {
//            attackTimer = 0;
//            self.animator.SetTrigger("Attack");
//            self.target.TakeDamage(Utilities.GetMinMaxDamageRoll(self.minDamage, self.maxDamage));
//            return state = NodeState.SUCCESS;
//        }

//        return state = NodeState.FAILURE;
//    }
//}
