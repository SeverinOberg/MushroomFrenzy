using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskDescription("This semi-conditional Action will FAILURE if the target has gone too far away, or SUCCESS if we are within melee attack range.")]
public class Pursue : Action
{

    [SerializeField] SharedEnemy self;

    public override TaskStatus OnUpdate()
    {
        if (IsTargetTooFarAway())
        {
            self.Value.ClearTarget();
            return TaskStatus.Failure;
        }

        if (self.Value.IsWithinMeleeAttackRange())
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }

    private bool IsTargetTooFarAway()
    {
        if (Utilities.GetDistanceBetween(transform.position, self.Value.target.transform.position) > self.Value.scanDiameter)
        {
            return true;
        }

        return false;
    }
}
