using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Pathfinding;
using UnityEngine;

public class ChangeTarget : Action
{
    [SerializeField] private SharedEnemy self;
    [SerializeField] private AIDestinationSetter aiDestinationSetter;
    [SerializeField] private SharedTransform target;
    [SerializeField] private bool clearSharedTargetAfterComplete;

    private float initialMeleeAttackRange;

    public override void OnAwake()
    {
        initialMeleeAttackRange = self.Value.MeleeAttackRange;
    }

    public override TaskStatus OnUpdate()
    {
        if (!target.Value)
        {
            Debug.LogError("no target set, failure unexpected");
            return TaskStatus.Failure;
        }

        aiDestinationSetter.target = target.Value;
        self.Value.MeleeAttackRange = target.Value.GetComponent<Collider2D>().bounds.size.x * 0.5f + initialMeleeAttackRange;

        if (clearSharedTargetAfterComplete)
        {
            target.Value = null;
        }

        return TaskStatus.Success;
    }

}
