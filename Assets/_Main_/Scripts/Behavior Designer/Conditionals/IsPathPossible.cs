using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using Pathfinding;

public class IsPathPossible : Conditional
{
    [RequiredField]
    [SerializeField] private AIDestinationSetter aiDestinationSetter;

    public override TaskStatus OnUpdate()
    {
        if (!aiDestinationSetter.target)
        {
            Debug.LogError($"{Owner.name}: aiDestinationSetter.target not set!");
            return TaskStatus.Failure;
        }

        if (Validate(transform, aiDestinationSetter.target))
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;   
    }

    public static bool Validate(Transform owner, Transform target)
    {
        GraphNode fromNode = AstarPath.active.GetNearest(owner.position,            NNConstraint.Default).node;
        GraphNode toNode   = AstarPath.active.GetNearest(target.transform.position, NNConstraint.Default).node;

        if (PathUtilities.IsPathPossible(fromNode, toNode))
        {
            return true;
        }

        return false;
    }


}
