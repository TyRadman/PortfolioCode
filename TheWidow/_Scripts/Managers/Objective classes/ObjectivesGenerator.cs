using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectivesGenerator : MonoBehaviour
{
    [System.Serializable]
    public struct ObjectivesLoad
    {
        public ObjectiveEntity[] Objectives;
        public ObjectiveTypes Type;
    }

    [SerializeField] private ObjectivesLoad[] m_Objectives;

    private void Start()
    {
        // set the objective type based on what it is in the objectivesLoad
        for (int i = 0; i < m_Objectives.Length; i++)
        {
            System.Array.ForEach(m_Objectives[i].Objectives, o => o.ObjectiveType = m_Objectives[i].Type);
        }

        // send the objectives to the objectives manager
        System.Array.ForEach(m_Objectives, o => ObjectivesManager.Instance.AddObjectives(o.Objectives, o.Type));
    }
}
