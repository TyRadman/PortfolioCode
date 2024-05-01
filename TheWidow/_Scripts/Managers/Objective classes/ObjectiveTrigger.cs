using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveTrigger : MonoBehaviour
{
    [HideInInspector] public int ID;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ObjectivesManager.Instance.ObjectiveFinished(ID);
            gameObject.SetActive(false);
        }
    }
}
