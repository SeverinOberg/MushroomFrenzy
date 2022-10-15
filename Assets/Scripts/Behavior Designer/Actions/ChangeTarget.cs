using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class ChangeTarget : Action
{
    [SerializeField] private SharedEnemy self;

    public override TaskStatus OnUpdate()
    {
        if (self.Value.Instigator)
        {
            self.Value.SetTarget(self.Value.Instigator);
            self.Value.ClearInstigator();
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;
    }
}
