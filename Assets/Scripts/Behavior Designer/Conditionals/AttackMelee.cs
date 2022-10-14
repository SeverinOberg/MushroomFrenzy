using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskDescription("Attacks a target as long as we are within melee range and the target still exists.")]
public class AttackMelee : Action
{

    [SerializeField] private SharedEnemy self;
    private float attackTimer;

    public override TaskStatus OnUpdate()
    {
        if (!self.Value.target)
        {
            return TaskStatus.Failure;
        }

        if (self.Value.target.IsDead)
        {
            self.Value.ClearTarget();
            return TaskStatus.Failure;
        }

        attackTimer += Time.deltaTime;
        if (attackTimer >= self.Value.attackCooldown)
        {
            attackTimer = 0;
            self.Value.animator.SetTrigger("Attack");
            self.Value.target.TakeDamage(Utilities.GetMinMaxDamageRoll(self.Value.minDamage, self.Value.maxDamage));
        }

        return TaskStatus.Running;
    }


    
}
