using BehaviorDesigner.Runtime.Tasks;
using Pathfinding;
using UnityEngine;

public class IsPathPossibleInstigator : Conditional
{
    [SerializeField] private SharedEnemy self;

    public override TaskStatus OnUpdate()
    {
        if (!self.Value.Instigator)
        {
            return TaskStatus.Failure;
        }

        if (self.Value.IsPathPossible(transform.position, self.Value.Instigator.transform.position))
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }
}
