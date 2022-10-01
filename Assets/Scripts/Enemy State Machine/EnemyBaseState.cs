using UnityEngine;

public abstract class EnemyBaseState 
{

    public abstract void StartState(EnemyStateManager manager);

    public abstract void UpdateState(EnemyStateManager manager);

    public abstract void OnCollisionEnter2D(EnemyStateManager manager, Collision2D collision);

}
