using System.Collections.Generic;

namespace BehaviourTree
{
    public class Selector : Node
    {
        public Selector() : base() { }
        public Selector(List<Node> children) : base(children) { }

        public override NodeState Evalute()
        {
            foreach (Node node in children)
            {
                switch (node.Evalute())
                {
                    case NodeState.FAILURE:
                        continue;
                    case NodeState.SUCCESS:
                        return state = NodeState.SUCCESS;
                    case NodeState.RUNNING:
                        return state = NodeState.RUNNING;
                    default:
                        continue;
                }
            }
            return state = NodeState.FAILURE;
        }

    }
}


