using UnityEngine;

namespace BehaviourTree
{
    public abstract class Tree : MonoBehaviour
    {
        private Node root = null;

        protected virtual void Start()
        {
            root = SetupTree();
        }

        protected virtual void Update()
        {
            if (root != null)
                root.Evalute();
        }

        protected abstract Node SetupTree();
    }
}

