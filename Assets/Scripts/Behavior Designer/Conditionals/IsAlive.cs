using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class IsAlive : Conditional
{

    [SerializeField] private SharedEnemy self;

    public override TaskStatus OnUpdate()
    {
        if (self.Value.IsDead)
        {
            return TaskStatus.Failure;
        }
        else
        {
            return TaskStatus.Success;
        }
    }

}
