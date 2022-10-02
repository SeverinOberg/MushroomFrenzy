using UnityEngine;

public class EnemyStateManager : MonoBehaviour 
{
    public Enemy enemy;

    private EnemyBaseState currentState;

    public EnemyScanState   ScanState   = new EnemyScanState();
    public EnemyAttackState AttackState = new EnemyAttackState();

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        currentState = ScanState;
    }

    private void Start()
    {
        // currentState = ScanState; // tut put this here, maybe for a reason, but testing it in Awake() for now
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
