using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using UnityEngine;

public class AttackRange : Action
{
    [SerializeField] private SharedEnemy self;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float animationDelay;

    private EnemyProjectile enemyWeapon;

    public override void OnAwake()
    {
        enemyWeapon = projectilePrefab.GetComponent<EnemyProjectile>();
    }

    public override TaskStatus OnUpdate()
    {
        self.Value.PausePathing(2);
        if (animationDelay > 0)
        {
            StartCoroutine(Throw(animationDelay));
        }
        else
        {
            Throw();
        }
        
        return TaskStatus.Success;
    }

    private void Throw()
    {
        self.Value.TriggerAnimation("Throw");
        enemyWeapon.Spawn(projectilePrefab, transform.position, self.Value, self.Value.Target);
    }

    private IEnumerator Throw(float delay)
    {
        yield return new WaitForSeconds(delay);
        self.Value.TriggerAnimation("Throw");
        enemyWeapon.Spawn(projectilePrefab, transform.position, self.Value, self.Value.Target);
    }

}
