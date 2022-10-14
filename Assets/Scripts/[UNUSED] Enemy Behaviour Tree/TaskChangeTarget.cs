using BehaviourTree;

public class TaskChangeTarget : Node 
{

    EnemyBT self;

    public TaskChangeTarget(EnemyBT self)
    {
        this.self = self;
    }

    public override NodeState Evalute()
    {
        // Collider2D collider = Physics2D.OverlapCircle(self.transform.position, self.scanDiameter, LayerMask.GetMask("Player", "Turret"));
        //if (collider && collider.TryGetComponent(out Unit target))
        //{
        if (self.instigatorTarget)
        {
            if (!self.IsPathPossible(self.transform.position, self.instigatorTarget.transform.position))
            {
                return state = NodeState.SUCCESS;
            }

            self.SetTarget(self.instigatorTarget);
            self.instigatorTarget = null; // Reset to be able to check for new instigators
            return state = NodeState.SUCCESS;
        }

        //}

        return state = NodeState.FAILURE;
    }

}
