using UnityEngine;

public class EnemyStateManager : MonoBehaviour 
{

    private EnemyBaseState currentState;

    public EnemyScanState   ScanState   = new EnemyScanState();
    public EnemyAttackState AttackState = new EnemyAttackState();

    private void Start()
    {
        currentState = ScanState;

        currentState.StartState(this);
    }

    private void Update()
    {
        currentState.UpdateState(this);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        currentState.OnCollisionEnter2D(this, collision);
    }

    public void SwitchState(EnemyBaseState state)
    {
        currentState = state;
        state.StartState(this);
    }

}
