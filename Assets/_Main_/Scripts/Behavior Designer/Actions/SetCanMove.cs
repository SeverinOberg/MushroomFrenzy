using UnityEngine;
using Pathfinding;
using BehaviorDesigner.Runtime.Tasks;

public class SetCanMove : Action
{

    [SerializeField] private AIPath aiPath;
    [SerializeField] private bool canMove;

    public override TaskStatus OnUpdate()
    {
        aiPath.canMove = canMove;
        return TaskStatus.Success;
    }

}
