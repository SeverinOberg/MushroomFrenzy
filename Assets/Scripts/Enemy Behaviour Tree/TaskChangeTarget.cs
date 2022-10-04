using UnityEngine;
using BehaviourTree;

public class TaskChangeTarget : Node 
{

    EnemyBT self;

    public TaskChangeTarget(EnemyBT self)
    {
        this.self = self;
    }

    public override NodeState Evalute()
    {
        Collider2D collider = Physics2D.OverlapCircle(self.transform.position, self.scanDiameter, LayerMask.GetMask("Player"));
        if (collider && collider.TryGetComponent(out Unit player))
        {
            self.SetTarget(player);
            return state = NodeState.SUCCESS;
        }

        return state = NodeState.FAILURE;
    }

}
