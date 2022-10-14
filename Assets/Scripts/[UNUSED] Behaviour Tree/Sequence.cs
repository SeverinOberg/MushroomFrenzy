using System.Collections.Generic;

namespace BehaviourTree
{
    public class Sequence : Node
    {
        public Sequence() : base() { }
        public Sequence(List<Node> children) : base(children) { }

        public override NodeState Evalute()
        {
            bool anyChildIsRunning = false;

            foreach (Node node in children)
            {
                switch (node.Evalute())
                {
                    case NodeState.FAILURE:
                        return state = NodeState.FAILURE;
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.RUNNING:
                        anyChildIsRunning = true;
                        continue;
                    default:
                        return state = NodeState.SUCCESS;
                }
            }
            return state = anyChildIsRunning ? NodeState.RUNNING : NodeState.SUCCESS;
        }

    }
}


