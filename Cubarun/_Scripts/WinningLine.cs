using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinningLine : MonoBehaviour
{
    [TextArea(1, 4)]
    [SerializeField] private string message;
    [SerializeField] private Color panelColor;
    [SerializeField] private Color textColor;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerMovement.Instance.isMoving = false;
            FadingScreen.Instance.Fade(5f, 0.5f, 0f, panelColor, false);
            FadingScreen.Instance.FadeText(message, 20f, 2f, 0f, textColor);
            FindObjectOfType<PlayerFallingDetector>().enabled = false;
        }
    }
}
