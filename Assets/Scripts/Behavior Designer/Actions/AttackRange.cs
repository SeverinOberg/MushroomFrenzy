using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using UnityEngine;

public class AttackRange : Action
{
    [SerializeField] private SharedEnemy self;
    [SerializeField] private GameObject rockPrefab;

    private RockThrow rockThrow;

    public override void OnAwake()
    {
        rockThrow = rockPrefab.GetComponent<RockThrow>();
    }

    public override TaskStatus OnUpdate()
    {
        self.Value.PausePathing(2);
        StartCoroutine(Throw());
        
        return TaskStatus.Success;
    }

    private IEnumerator Throw()
    {
        //self.Value.animator.SetTrigger("Throw Rock");

        // Animation delay
        yield return new WaitForSeconds(1.5f);
        rockThrow.Spawn(rockPrefab, transform.position, self.Value, self.Value.Target);
    }

}
