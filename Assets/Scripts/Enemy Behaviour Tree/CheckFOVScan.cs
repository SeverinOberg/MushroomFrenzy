using BehaviourTree;
using UnityEngine;

public class CheckFOVScan : Node
{
    private readonly EnemyBT self;

    public CheckFOVScan(EnemyBT self)
    {
        this.self = self;
    }

    public override NodeState Evalute()
    {
        object target = GetData("target");
        if (target == null)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(self.transform.position, self.scanRadius);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].CompareTag("Enemy") || !colliders[i].TryGetComponent(out Unit unit) || unit.isDead)
                {
                    continue;
                }

                if (unit)
                {
                    parent.parent.SetData("target", unit);
                    state = NodeState.SUCCESS;
                    return state;
                }
            }
        }
        else
        {
            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}
