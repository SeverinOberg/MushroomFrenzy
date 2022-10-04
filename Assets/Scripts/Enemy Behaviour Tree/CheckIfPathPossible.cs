using UnityEngine;
using BehaviourTree;

public class CheckIfPathPossible : Node
{

    EnemyBT self;

    public CheckIfPathPossible(EnemyBT self)
    {
        this.self = self;
    }

    public override NodeState Evalute()
    {
        if (self.IsPathPossible(self.transform.position, self.aiDestinationSetter.target.position))
        {
            return state = NodeState.SUCCESS;
        }

        self.ClearTarget();
        return state = NodeState.FAILURE;
    }

}
