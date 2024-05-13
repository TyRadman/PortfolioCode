using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.UnitControllers;
using System;

namespace TankLike.Combat
{
    [CreateAssetMenu(fileName = "Shop Item Info", menuName = "TankLike/Tools/Tool Info")]
    public class ToolInfo : ScriptableObject
    {
        [Header("Metadata")]
        public string GUID;
        [Header("Text")]
        public string Name;
        [TextArea(2, 20)] public string Description;
        [Header("Values")]
        public int Cost;
        //public int Nodes;
        [Header("References")]
        public Sprite IconImage;
        public Tool ToolReference;

        private void OnValidate()
        {
            if (Name.Length > 0)
            {
                var toolName = string.Join("", Name.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
                GUID = string.Concat("TD_", toolName, "_01");
            }
        }
    }
}
