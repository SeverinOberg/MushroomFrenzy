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
        if (!self.target)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(self.transform.position, self.scanRadius);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].CompareTag("Enemy") || !colliders[i].TryGetComponent(out Unit unit) || unit.isDead)
                {
                    continue;
                }

                // @TODO: Add complexity such as:
                // Priorities units in order of turrets > players > mushrooms > farm > walls & gates
                // Ignore walls & gates if path to target available
                // Attack walls / gate if path is blocked

                if (unit)
                {
                    self.target = unit;
                    self.aiDestinationSetter.target = unit.transform;
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
