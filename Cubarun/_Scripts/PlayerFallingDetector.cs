using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallingDetector : MonoBehaviour
{
    private static PlayerFallingDetector Instance;

    private void Awake()
    {
        Instance = this;
    }

    public static void TurnOff()
    {
        Instance.gameObject.SetActive(false);   
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerMovement.Instance.isMoving = false;
            GamePlayManager.Instance.GameOver = true;
            playerMovement.Instance.PlayState(true);
            PlayerComponents.Instance.Rotate();
        }
    }
}
