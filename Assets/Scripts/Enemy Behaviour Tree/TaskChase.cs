using UnityEngine;
using BehaviourTree;

public class TaskChase : Node
{

    private EnemyBT self;

    private Unit target;

    public TaskChase(EnemyBT self)
    {
        this.self = self;

        target = (Unit)GetData("target");
    }

    public override NodeState Evalute()
    {

        if (Vector2.Distance(self.transform.position, target.transform.position) > 0.1f)
        {
            self.transform.position = Vector2.MoveTowards(self.transform.position, target.transform.position, self.MovementSpeed * Time.deltaTime);
        }

        state = NodeState.RUNNING;
        return state;
    }

}
