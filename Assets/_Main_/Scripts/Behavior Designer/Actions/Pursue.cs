using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using Pathfinding;


[TaskDescription("Will FAILURE if the target has gone too far away, or SUCCESS if we are within range")]
public class Pursue : Action
{

    [SerializeField] private AIDestinationSetter aiDestinationSetter;
    [SerializeField] private SharedEnemy self;

    public override TaskStatus OnUpdate()
    {
        if (!aiDestinationSetter.target)
        {
            return TaskStatus.Failure;
        }

        if (IsWithinMeleeAttackRange())
        {
            return TaskStatus.Success;
        }

        if (IsTargetTooFarAway())
        {
            return TaskStatus.Failure;
        }

        if (!IsPathPossible.Validate(transform, aiDestinationSetter.target))
        {
            return TaskStatus.Failure;
        }

        return TaskStatus.Running;
    }

    private bool IsTargetTooFarAway()
    {
        if (Vector2.Distance(transform.position, aiDestinationSetter.target.position) > self.Value.ScanRange)
        {
            return true;
        }

        return false;
    }

    private bool IsWithinMeleeAttackRange()
    {
        if (Vector2.Distance(transform.position, aiDestinationSetter.target.position) <= self.Value.MeleeAttackRange)
        {
            return true;
        }
        return false;
    }
}
