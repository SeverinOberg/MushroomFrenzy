using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Pathfinding;
using UnityEngine;

public class SetDestinationTarget : Action
{
    [SerializeField] private AIDestinationSetter aiDestinationSetter;
    [SerializeField] private SharedTransform target;

    public override TaskStatus OnUpdate()
    {
        aiDestinationSetter.target = target.Value;
        return TaskStatus.Success;
    }
}
