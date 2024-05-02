using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    [SerializeField] private bool isAndroid;
    [SerializeField] private GameObject movementButtons;
    [SerializeField] private GameObject emptiness;

    private void Awake()
    {
        movementButtons.SetActive(isAndroid);
        emptiness.SetActive(!isAndroid);
    }
}
