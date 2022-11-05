using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class IsPathPossibleToTarget : Conditional
{
    [SerializeField] private SharedTransform target;

    public override TaskStatus OnUpdate()
    {
        if (!target.Value)
        {
            Debug.LogError("no target set, failure unexpected");
            return TaskStatus.Failure;
        }

        if (IsPathPossible.Validate(transform,  target.Value))
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }
}
