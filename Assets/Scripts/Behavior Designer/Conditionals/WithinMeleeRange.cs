using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskDescription("Checks if we are within melee range of our target.")]
public class WithinMeleeRange : Conditional
{

    [SerializeField] private SharedEnemy self;

    public override TaskStatus OnUpdate()
    {
        if (!self.Value.Target || self.Value.Target.IsDead)
        {
            return TaskStatus.Failure;
        }

        if (Vector2.Distance(transform.position, self.Value.Target.transform.position) <= self.Value.MeleeAttackRange)
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }

}
