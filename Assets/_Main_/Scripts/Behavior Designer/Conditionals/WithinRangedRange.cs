using BehaviorDesigner.Runtime.Tasks;
using Pathfinding;
using UnityEngine;

[TaskDescription("Checks if we are within ranged range of our target.")]
public class WithinRangedRange : Conditional
{
    [SerializeField] private AIDestinationSetter aiDestinationSetter;
    [SerializeField] private SharedEnemy self;

    public override TaskStatus OnUpdate()
    {
        if (!aiDestinationSetter.target)
        {
            return TaskStatus.Failure;
        }

        if (Vector2.Distance(transform.position, aiDestinationSetter.target.transform.position) <= self.Value.RangedAttackRange)
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }

}
