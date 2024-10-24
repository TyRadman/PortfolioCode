using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BT
{
    using Nodes;
    using System;

    [CreateAssetMenu()]
    public class BehaviorTree : ScriptableObject
    {
        public BaseNode RootNode;
        public NodeState TreeState = NodeState.Running;
        public List<BaseNode> Nodes = new List<BaseNode>();
        public BlackboardVariablesContainer BlackboardContainer;

        public void Start()
        {
            RootNode.State = NodeState.Running;
            RootNode.OnAwake();
        }

        public NodeState Update()
        {
            if(RootNode.State == NodeState.Running )
            {
                TreeState = RootNode.Update();
            }

            return TreeState;
        }

#if UNITY_EDITOR
        private const string UNDO_REDO_CREATE_NODE_ID = "Behavior Tree (CreateNode)";
        private const string UNDO_REDO_DELETE_NODE_ID = "Behavior Tree (DeleteNode)";
        [HideInInspector] public bool IsMinimapDisplayed = false;
        [HideInInspector] public bool IsBlackboardDisplayed = false;
        // the values that center the view at the zoom of one.
        [HideInInspector] public Vector3 ViewPosition = new Vector3(431, 358, 0f);
        [HideInInspector] public Vector3 ViewZoom = Vector3.one;

        public BaseNode CreateNode(System.Type type)
        {
            BaseNode node = ScriptableObject.CreateInstance(type) as BaseNode;
            node.name = type.Name;
            node.GUID = GUID.Generate().ToString();
            node.Blackboard = BlackboardContainer;

            Undo.RecordObject(this, UNDO_REDO_CREATE_NODE_ID);
            Nodes.Add(node);

            // if the application is not playing, then save the object to the data assets
            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(node, this);
            }

            Undo.RegisterCreatedObjectUndo(node, UNDO_REDO_CREATE_NODE_ID);

            node.ViewDetails.Name = ObjectNames.NicifyVariableName(node.GetType().Name.Replace("Node", string.Empty));

            AssetDatabase.SaveAssets();
            return node;
        }

        public void DeleteNode(BaseNode node)
        {
            Undo.RecordObject(this, UNDO_REDO_DELETE_NODE_ID);
            Nodes.Remove(node);

            Undo.DestroyObjectImmediate(node);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Returns a runtime duplicate of the behavior tree
        /// </summary>
        /// <returns></returns>
        public BehaviorTree Clone()
        {
            BehaviorTree tree = Instantiate(this);
            tree.RootNode = tree.RootNode.Clone();
            tree.Nodes = new List<BaseNode>();

            // fill the tree with all the children of the root node. Perform a depth first traversel :,)
            Traverse(tree.RootNode, n =>
            {
                tree.Nodes.Add(n);
            });

            return tree;
        }

        public void Traverse(BaseNode node, System.Action<BaseNode> visitor)
        {
            if (node == null)
            {
                return;
            }
         
            visitor?.Invoke(node);
            List<BaseNode> children = node.GetChildren();
            children.ForEach(n => Traverse(n, visitor));

        }

        public void Bind(GameObject agent)
        {
            Traverse(RootNode, node =>
            {
                node.Agent = agent;
                node.Blackboard = BlackboardContainer;
            });
        }

        #region Blackboard
        public void CreateBlackboardContainer()
        {
            if(BlackboardContainer != null)
            {
                return;
            }

            BlackboardContainer = ScriptableObject.CreateInstance<BlackboardVariablesContainer>();
            BlackboardContainer.name = "Blackboard";
            BlackboardContainer.BehaviorTree = this;


            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(BlackboardContainer, this);
            }

            AssetDatabase.SaveAssets();
        }
        #endregion

        public void Refresh()
        {
            if(BlackboardContainer == null)
            {
                return;
            }

            if (BlackboardContainer.BehaviorTree == null)
            {
                BlackboardContainer.BehaviorTree = this;
            }

            string path = AssetDatabase.GetAssetPath(this);
            UnityEngine.Object[] subAssets = AssetDatabase.LoadAllAssetsAtPath(path);

            Nodes.ForEach(n => n.Blackboard = BlackboardContainer);

            // TODO: uncomment
            if (BlackboardContainer == null || BlackboardContainer.Variables == null || BlackboardContainer.Variables.Count == 0)
            {
                return;
            }

            foreach (var asset in subAssets)
            {
                if (asset != this)
                {
                    if (asset is ExposedProperty && !BlackboardContainer.Variables.Contains((ExposedProperty)asset))
                    {
                        AssetDatabase.RemoveObjectFromAsset(asset);

                        // Then, destroy the ScriptableObject
                        DestroyImmediate(asset, true);

                        // Finally, save the changes to the AssetDatabase
                        AssetDatabase.SaveAssets();
                    }
                }
            }
        }
#endif
    }
}
