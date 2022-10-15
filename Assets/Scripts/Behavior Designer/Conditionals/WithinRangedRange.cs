using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskDescription("Checks if we are within ranged range of our target.")]
public class WithinRangedRange : Conditional
{

    [SerializeField] private SharedEnemy self;

    public override TaskStatus OnUpdate()
    {
        if (!self.Value.Target || self.Value.Target.IsDead)
        {
            return TaskStatus.Failure;
        }

        if (Vector2.Distance(transform.position, self.Value.Target.transform.position) <= self.Value.RangedAttackRange)
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }

}
