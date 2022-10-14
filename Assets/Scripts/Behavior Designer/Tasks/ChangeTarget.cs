using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class ChangeTarget : Action
{
    [SerializeField] private SharedEnemy self;

    public override TaskStatus OnUpdate()
    {
        if (self.Value.instigatorTarget)
        {
            self.Value.SetTarget(self.Value.instigatorTarget);
            self.Value.instigatorTarget = null; // Reset to be able to check for new instigators
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;
    }
}
