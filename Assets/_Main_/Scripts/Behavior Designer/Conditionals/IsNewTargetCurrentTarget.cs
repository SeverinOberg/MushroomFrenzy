using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Pathfinding;
using UnityEngine;

[TaskDescription("Validates whether the new target is the same as the current one")]
public class IsNewTargetCurrentTarget : Conditional
{

    [Header("Required")]
    [SerializeField, RequiredField] private AIDestinationSetter aiDestinationSetter;
    [SerializeField, RequiredField] private SharedTransform newTarget;

    public override TaskStatus OnUpdate()
    {
        if (aiDestinationSetter.target == newTarget.Value)
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }

}
