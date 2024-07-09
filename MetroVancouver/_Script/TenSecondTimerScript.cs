using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class TenSecondTimerScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float remainingTime; 
    public SceneHandler sceneHandler;
    [SerializeField] private int _sceneNumber = 0;

    void Update()
    {
        // Decrement seconds 
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
        }

        // When time runs out, trigger scene transition back to first scene
        else if (remainingTime < 0)
        {
            remainingTime = 0; 
            if (sceneHandler != null)
            {
                sceneHandler.SceneTransition(_sceneNumber);
            }
        }
        
        // Display 10 second countdown 
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
