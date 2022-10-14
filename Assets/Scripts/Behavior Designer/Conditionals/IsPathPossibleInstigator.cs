using BehaviorDesigner.Runtime.Tasks;
using Pathfinding;
using UnityEngine;

public class IsPathPossibleInstigator : Conditional
{
    [SerializeField] private SharedEnemy self;

    public override TaskStatus OnUpdate()
    {
        if (!self.Value.instigatorTarget)
        {
            return TaskStatus.Failure;
        }

        GraphNode fromNode = AstarPath.active.GetNearest(transform.position, NNConstraint.Default).node;
        GraphNode toNode = AstarPath.active.GetNearest(self.Value.instigatorTarget.transform.position, NNConstraint.Default).node;

        if (PathUtilities.IsPathPossible(fromNode, toNode))
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }
}
