using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandInteractor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Orb"))
        {
            Orb orb = other.GetComponent<Orb>();
            orb.OnOrbSelected();
        }
    }
}
