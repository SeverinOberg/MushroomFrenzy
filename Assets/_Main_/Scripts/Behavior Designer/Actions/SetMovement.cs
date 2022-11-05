using UnityEngine;
using Pathfinding;
using BehaviorDesigner.Runtime.Tasks;
using TooltipAttribute = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

public class SetMovement : Action
{
    private enum Type
    {
        Default,
        Zero,
        Current,
        Custom,
    }

    [Header("Required")]
    [Tooltip("Our Unit stores our current states")]
    [SerializeField] private Unit   unit;
    [Tooltip("Our aiPath is what controls our AI's movement through aiPath.maxSpeed")]
    [SerializeField] private AIPath aiPath;
    [Tooltip("Depending on what Type you pick, it will set your movement accordingly")]
    [SerializeField] private Type   type;

    [Header("Optional")]
    [Tooltip("Use 'speed' to set the 'Custom' type's movement speed")]
    [SerializeField] private float speed;


    public override TaskStatus OnUpdate()
    {
        switch (type)
        {
            case Type.Default:
                aiPath.maxSpeed = unit.DefaultMovementSpeed;
                break;
            case Type.Zero:
                aiPath.maxSpeed = 0;
                break;
            case Type.Current:
                aiPath.maxSpeed = unit.MovementSpeed;
                break;
            case Type.Custom:
                aiPath.maxSpeed = speed;
                break;
        }

        return TaskStatus.Success;
    }
}
