using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskDescription("Checks if we are within melee range of our target.")]
public class WithinMeleeRange : Conditional
{

    [SerializeField] private SharedEnemy self;

    public override TaskStatus OnUpdate()
    {
        if (!self.Value.target || self.Value.target.IsDead)
        {
            return TaskStatus.Failure;
        }

        if (Vector2.Distance(transform.position, self.Value.target.transform.position) <= self.Value.meleeAttackRange)
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }

}
