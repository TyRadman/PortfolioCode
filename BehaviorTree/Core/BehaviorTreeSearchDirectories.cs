using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BT
{
    using Nodes;

    [System.Serializable]
    public class NodeSearchDirectory
    {
        [HideInInspector] public string ClassName;
        public string Directory;
        public Type ClassType;
    }

    [CreateAssetMenu()]
    public class BehaviorTreeSearchDirectories : ScriptableObject
    {
        [field: SerializeField] public List<NodeSearchDirectory> Directories { get; private set; } = new List<NodeSearchDirectory>();
        private List<Type> DefaultNodes = new List<Type>
        {
            typeof(PrintNode), typeof(WaitNode), typeof(SequenceNode), 
            typeof(RepeatNode), typeof(SelectorNode),
            typeof(LoopNode), typeof(InvertNode), typeof(ParallelNode), typeof(BehaviorTreeNode),
            typeof(EmptyActionNode), typeof(ForceStateNode)
        };

        public void RefreshDictionary()
        {
            TypeCache.TypeCollection nodeTypes = TypeCache.GetTypesDerivedFrom(typeof(BaseNode));
            List<Type> projectTypes = nodeTypes.ToList();

            // Remove types that don't exist in the project
            for (int i = Directories.Count - 1; i >= 0; i--)
            {
                if(!projectTypes.Exists(t => t == Directories[i].ClassType))
                {
                    Directories.RemoveAt(i);
                }
            }

            for (int i = Directories.Count - 1; i >= 0; i--)
            {
                string[] names = Directories[i].Directory.Split('/');
                string className = names.Last();

                if(nodeTypes.ToList().Exists(c => c == Directories[i].ClassType))
                {
                    Directories.Remove(Directories[i]);
                }
            }

            foreach (Type nodeType in nodeTypes)
            {
                if(Directories.Exists(d => d.ClassType == nodeType) || nodeType.IsAbstract || nodeType == typeof(RootNode))
                {
                    continue;
                }

                string directory = DefaultNodes.Exists(n => n == nodeType) ? "Defaults/" : "Custom/";
                string childDirectory = $"{ObjectNames.NicifyVariableName(nodeType.BaseType.Name + "s")}/{ObjectNames.NicifyVariableName(nodeType.Name)}";

                Directories.Add(new NodeSearchDirectory()
                {
                    ClassName = nodeType.Name,
                    Directory = directory + childDirectory,
                    ClassType = nodeType
                });
            }
        }

        public void ResetDictionary()
        {
            Directories.Clear();
        }
    }
}
