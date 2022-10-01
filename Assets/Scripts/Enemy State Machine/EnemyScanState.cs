using UnityEngine;

public class EnemyScanState : EnemyBaseState
{

    public override void StartState(EnemyStateManager manager)
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState(EnemyStateManager manager)
    {
        throw new System.NotImplementedException();
    }

    public override void OnCollisionEnter2D(EnemyStateManager manager, Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Unit unit) && !unit.CompareTag("Enemy"))
        {
            manager.SwitchState(manager.AttackState);
        }
    }

}
