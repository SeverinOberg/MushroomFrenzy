using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using UnityEngine;

public class AttackRange : Action
{
    [SerializeField] private SharedEnemy self;
    [SerializeField] private GameObject projectilePrefab;

    private EnemyProjectile enemyWeapon;

    public override void OnAwake()
    {
        enemyWeapon = projectilePrefab.GetComponent<EnemyProjectile>();
    }

    public override TaskStatus OnUpdate()
    {
        self.Value.PausePathing(2);
        StartCoroutine(Throw());
        
        return TaskStatus.Success;
    }

    private IEnumerator Throw()
    {
        //self.Value.animator.SetTrigger("Throw");

        // Animation delay
        yield return new WaitForSeconds(1.5f);
        enemyWeapon.Spawn(projectilePrefab, transform.position, self.Value, self.Value.Target);
    }

}
