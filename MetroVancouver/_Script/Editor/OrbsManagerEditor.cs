using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(OrbsGroup))]
public class OrbsManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        OrbsGroup manager = (OrbsGroup)target;

        if (GUI.changed && manager.EditorMode)
        {
            List<Transform> orbs = new List<Transform>();
            manager.Orbs.FindAll(o => o.gameObject.activeSelf).ForEach(o => orbs.Add(o.transform));
            float offsetAngle = 360 / orbs.Count;

            for (int i = 0; i < orbs.Count; i++)
            {
                orbs[i].localEulerAngles = new Vector3(0f, offsetAngle * i, 0f);
                orbs[i].localPosition = orbs[i].forward * manager.SpeadRadius;
                orbs[i].localScale = Vector3.one * manager.OrbsSize;
            }
        }
    }
}
