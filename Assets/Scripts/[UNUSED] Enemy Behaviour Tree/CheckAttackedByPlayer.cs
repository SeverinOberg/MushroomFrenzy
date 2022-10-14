using BehaviourTree;
using UnityEngine;

public class CheckAttackedByPlayer : Node 
{

    EnemyBT self;

    private bool tookDamage;

    public CheckAttackedByPlayer(EnemyBT self)
    {
        this.self = self;
        //self.OnTakeDamage += OnTakePlayerDamage;
        self.OnDisableAction += OnDisable;
    }

    private void OnDisable()
    {
        //self.OnTakeDamage -= OnTakePlayerDamage;
        self.OnDisableAction -= OnDisable;
    }

    public override NodeState Evalute()
    {
        if (tookDamage)
        {
            tookDamage = false; // Reset for next Evaluate()
            return state = NodeState.SUCCESS;
        }
        return state = NodeState.FAILURE;
    }

    private void OnTakePlayerDamage(Unit instigator)
    {
        tookDamage = !tookDamage;
        self.instigatorTarget = instigator;
    }


}
