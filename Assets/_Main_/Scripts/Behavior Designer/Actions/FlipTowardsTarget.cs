using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class FlipTowardsTarget : Action
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SharedTransform target;

    public override TaskStatus OnUpdate()
    {
        Vector2 directionFromTarget = (target.Value.position - transform.position).normalized;
        if (directionFromTarget.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }

        target.Value = null;
        return TaskStatus.Success;
    }

}
