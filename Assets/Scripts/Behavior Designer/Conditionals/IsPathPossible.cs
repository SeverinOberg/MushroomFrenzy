using BehaviorDesigner.Runtime.Tasks;
using Pathfinding;
using UnityEngine;

public class IsPathPossible : Conditional
{
    [SerializeField] private SharedEnemy self;

    public override TaskStatus OnUpdate()
    {
        if (!self.Value.Target)
        {
            return TaskStatus.Failure;
        }

        if (self.Value.IsPathPossible(transform.position, self.Value.Target.transform.position))
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }
}
