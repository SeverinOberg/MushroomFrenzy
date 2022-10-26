using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskDescription("This semi-conditional Action will FAILURE if the target has gone too far away, or SUCCESS if we are within melee or ranged attack range." +
    " If your unit is ranged, remember to set the properties as applicable!")]
public class Pursue : Action
{

    [SerializeField] private SharedEnemy self;

    private float rangedAttackCooldown;

    private float rangedTimer;

    public override void OnStart()
    {
        rangedAttackCooldown = Random.Range(self.Value.MinRangedAttackCooldown, self.Value.MaxRangedAttackCooldown);
    }

    public override TaskStatus OnUpdate()
    {
        if (IsTargetTooFarAway())
        {
            self.Value.ClearTarget();
            return TaskStatus.Failure;
        }

        if (self.Value.IsRanged)
        {
            if (self.Value.IsRanged && !self.Value.IsMelee && self.Value.IsWithinRangedAttackRange())
            {
                self.Value.PausePathing(5);
            }

            rangedTimer += Time.deltaTime;
            if (rangedTimer > rangedAttackCooldown && self.Value.IsWithinRangedAttackRange())
            {
                rangedTimer = 0;
                return TaskStatus.Success;
            }
        }

        if (self.Value.IsWithinMeleeAttackRange())
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }

    private bool IsTargetTooFarAway()
    {
        if (Utilities.GetDistanceBetween(transform.position, self.Value.Target.transform.position) > self.Value.ScanDiameter)
        {
            return true;
        }

        return false;
    }
}
