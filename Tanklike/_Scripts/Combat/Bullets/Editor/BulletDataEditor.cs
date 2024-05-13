using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace TankLike.Combat
{
    [CustomEditor(typeof(AmmunationData))]
    public class BulletDataEditor : UnityEditor.Editor
    {
        private void OnEnable()
        {
            ((AmmunationData)target).GUID = ((AmmunationData)target).GUID.Length == 0? "Name" : ((AmmunationData)target).GUID;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //serializedObject.Update();

            AmmunationData bulletData = (AmmunationData)target;

            if (bulletData.name == bulletData.GUID)
            {
                return;
            }

            if (bulletData.GUID.Length > 0)
            {
                var fileName = string.Join("", bulletData.name.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
                bulletData.GUID = string.Concat(fileName);
            }
        }
    }
}
