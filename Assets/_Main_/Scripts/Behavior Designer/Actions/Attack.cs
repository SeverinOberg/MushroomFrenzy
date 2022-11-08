using UnityEngine;
using Pathfinding;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[TaskDescription("Attacks a target as long as we are within melee range and the target still exists")]
public class Attack : Action
{
    private enum Type
    {
        Melee,
        Ranged,
        Hybrid,
    }

    [SerializeField, RequiredField] private AIDestinationSetter aiDestinationSetter;
    [SerializeField, RequiredField] private AIPath              aiPath;
    [SerializeField, RequiredField] private SharedUnit          self;
    [SerializeField, RequiredField] private SharedTransform     currentWaypoint;

    [SerializeField] private Type type;

    private Unit targetUnit;

    public override void OnStart()
    {
        aiPath.canMove = false;
        targetUnit     = aiDestinationSetter.target.GetComponent<Unit>();
    }

    public override TaskStatus OnUpdate()
    {
        if (!aiDestinationSetter.target || aiDestinationSetter.target.CompareTag("Waypoint"))
        {
            return TaskStatus.Failure;
        }

        switch (type)
        {
            case Type.Melee:
                if (Melee()) return TaskStatus.Success;
                break;
            case Type.Ranged:
                if (Ranged()) return TaskStatus.Success;
                break;
            case Type.Hybrid:
                if (Hybrid()) return TaskStatus.Success;
                break;
            default:
                break;
        }

        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        aiPath.canMove = true;
    }

    private bool Melee()
    {
        
        if (self.Value.Attack(targetUnit))
        {
            aiDestinationSetter.target = currentWaypoint.Value;
            return true;
        }

        return false;
    }

    private bool Ranged()
    {
        return true;
    }

    private bool Hybrid()
    {
        return true;
    }

}
