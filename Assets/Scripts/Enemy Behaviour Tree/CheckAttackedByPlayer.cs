using BehaviourTree;
using UnityEngine;

public class CheckAttackedByPlayer : Node 
{

    EnemyBT self;

    private bool tookDamage;

    public CheckAttackedByPlayer(EnemyBT self)
    {
        this.self = self;
        self.OnTakeDamage += OnTakeDamage;
        self.OnDisableAction += OnDisable;
    }

    private void OnDisable()
    {
        self.OnTakeDamage -= OnTakeDamage;
        self.OnDisableAction -= OnDisable;
    }

    public override NodeState Evalute()
    {
        if (tookDamage)
        {
            tookDamage = false;
            return state = NodeState.SUCCESS;
        }
        return state = NodeState.FAILURE;
    }

    private void OnTakeDamage()
    {
        tookDamage = !tookDamage;
    }


}
