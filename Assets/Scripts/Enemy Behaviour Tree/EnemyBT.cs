using BehaviourTree;
using System.Collections.Generic;

public class EnemyBT : Unit
{
    public float scanRadius = 15;
    public float minDamage  = 2;
    public float maxDamage  = 5;

    protected override Node SetupTree()
    {
        Node root = new Selector(new List<Node> 
        { 
            new Sequence(new List<Node>
            {
                new CheckFOVScan(this),
                new TaskChase(this),
            }),
            new TaskPatrol(this),
        });

        return root;
    }

}
