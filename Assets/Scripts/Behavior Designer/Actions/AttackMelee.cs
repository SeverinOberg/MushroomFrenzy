using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskDescription("Attacks a target as long as we are within melee range and the target still exists.")]
public class AttackMelee : Action
{

    [SerializeField] private SharedEnemy self;
    private float attackTimer;

    public override void OnStart()
    {
        attackTimer = self.Value.AttackCooldown;
    }

    public override TaskStatus OnUpdate()
    {
        if (!self.Value.Target)
        {
            return TaskStatus.Failure;
        }

        if (self.Value.Target.IsDead)
        {
            self.Value.ClearTarget();
            return TaskStatus.Failure;
        }

        attackTimer += Time.deltaTime;
        if (attackTimer >= self.Value.AttackCooldown)
        {
            attackTimer = 0;
            self.Value.TriggerAnimation("Attack");

            self.Value.Target.TakeDamage(Utilities.GetMinMaxDamageRoll(self.Value.MinDamage, self.Value.MaxDamage));
            self.Value.Target.Blink(Color.red);

            Vector2 targetDirection = (transform.position - self.Value.Target.transform.position).normalized;
            self.Value.Target.AddForce(-targetDirection, self.Value.AttackKnockbackForce);
        }

        return TaskStatus.Running;
    }
    
}
