using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerSelection : MonoBehaviour
{
    [SerializeField] private Camera playerController;
    [SerializeField] private Camera cameraController;
    [SerializeField] private bool isPlayer = true;

    private void OnValidate()
    {
        playerController.enabled = isPlayer;
        cameraController.enabled = !isPlayer;
    }
}
