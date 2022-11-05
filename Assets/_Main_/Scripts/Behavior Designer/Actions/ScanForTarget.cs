using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Pathfinding;
using UnityEngine;

[TaskDescription("Scans within a circle radius for a specific target to find and attach a target AND a destination target")]
public class ScanForTarget : Action
{

    [Header("Required")]
    [SerializeField, RequiredField] private string targetScanLayer;
    [SerializeField, RequiredField] private AIDestinationSetter aiDestinationSetter;
    [SerializeField, RequiredField] private float scanRadius;

    private LayerMask scanLayerMask;

    public override void OnAwake()
    {
        scanLayerMask = LayerMask.GetMask(targetScanLayer);
    }

    public override TaskStatus OnUpdate()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, scanRadius, scanLayerMask);

        if (colliders.Length == 0 || colliders.Length == 1 && colliders[0].CompareTag("Untargetable"))
        {
            return TaskStatus.Failure;
        }

        int randomIndex = 0;
        while (true)
        {
            randomIndex = Random.Range(0, colliders.Length);

            if (colliders[randomIndex].CompareTag("Untargetable"))
            {
                continue;
            }

            aiDestinationSetter.target = colliders[randomIndex].transform;
            break;
        }

        return TaskStatus.Success;
    }

}
