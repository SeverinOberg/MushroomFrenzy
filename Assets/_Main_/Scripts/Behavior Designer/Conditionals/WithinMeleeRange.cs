using BehaviorDesigner.Runtime.Tasks;
using Pathfinding;
using UnityEngine;

[TaskDescription("Checks if we are within melee range of our target.")]
public class WithinMeleeRange : Conditional
{
    [SerializeField] private AIDestinationSetter aiDestinationSetter;
    [SerializeField] private SharedEnemy self;

    public override TaskStatus OnUpdate()
    {
        if (!aiDestinationSetter.target)
        {
            return TaskStatus.Failure;
        }

        if (Vector2.Distance(transform.position, aiDestinationSetter.target.position) <= self.Value.MeleeAttackRange)
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }

}
