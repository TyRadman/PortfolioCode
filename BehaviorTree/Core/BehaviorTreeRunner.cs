using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    using Nodes;

    public class BehaviorTreeRunner : MonoBehaviour
    {
        public BehaviorTree Tree;
        [SerializeField] private bool _runOnStart = false;

        private void Start()
        {
            if (_runOnStart)
            {
                Run();
            }
        }

        public void Run()
        {
            // pass the controller of the B
            Tree = Tree.Clone();
            Tree.Bind(gameObject);
            Tree.Start();
        }

        private void Update()
        {
            Tree.Update();
        }
    }
}
