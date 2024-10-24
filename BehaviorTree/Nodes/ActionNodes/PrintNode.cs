using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT.Nodes
{
    public class PrintNode : ActionNode
    {
        public enum DebugType
        {
            Log = 0, LogWarning = 1, LogError = 2
        }

        [TextArea(2, 4)]
        [SerializeField] private string _message;
        [SerializeField] private DebugType _printType = DebugType.Log;
        [SerializeField] private bool _display = true;

        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {

        }

        protected override NodeState OnUpdate()
        {
            Print();
            return NodeState.Success;
        }

        private void Print()
        {
            if (!_display)
            {
                return;
            }

            switch (_printType)
            {
                case DebugType.Log:
                    Debug.Log($"{_message}");
                    break;
                case DebugType.LogWarning:
                    Debug.LogWarning($"{_message}");
                    break;
                case DebugType.LogError:
                    Debug.LogErrorFormat($"{_message}");
                    break;
            }
        }
    }
}
