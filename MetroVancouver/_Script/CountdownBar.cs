using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CountdownBar : MonoBehaviour
{
    public static CountdownBar i; 
    [SerializeField] private Image _icon; 
    [SerializeField] private float _duration = 5;
    [SerializeField] private TextMeshProUGUI titleText; 
    [SerializeField] private Animation anim;
    [SerializeField] private AnimationClip fadeInClip; 
    [SerializeField] private AnimationClip fadeOutClip;
    private float _currTime = 0; 
    private const float Y_OFFSET = 8; 
    Coroutine displayCanvasCoroutine;

    // Initialize singleton 
    private void Awake()
    {
        i = this;
    }

    public void DisplayElementsAtWaypoint(WaypointInfo waypoint)
    {
        if (waypoint != null)
        {
            FadeIn();

            // Make countdown element a child of the waypoint 
            transform.parent = waypoint.transform; 

            // Reset the location of the UI 
            transform.localPosition = Vector3.zero; 
            Vector3 localPosition = transform.localPosition; 
            localPosition.y = Y_OFFSET; 
            transform.localPosition = localPosition; 

            if (titleText != null)
            {
                // Display title text 
                titleText.text = string.Format(waypoint.waypointTitle);
            }

            // Start countdown graphic 
            displayCanvasCoroutine = StartCoroutine(UpdateTimerProcess());   
        }
    }

    // Stop coroutine (called when finger leaves waypoint's collider)
    public void RemoveElementsAtWaypoint(WaypointInfo waypoint)
    {
        if (displayCanvasCoroutine != null)
        {
            StopCoroutine(displayCanvasCoroutine);
        }

        FadeOut();
    }

    // Run the coroutine 
    private IEnumerator UpdateTimerProcess()
    {
        _currTime = 0; 

        while (_currTime <= _duration)
        {
            _currTime += Time.deltaTime;
            _icon.fillAmount = _currTime / _duration; // To make value between 0 and 1
            yield return null; // Says this frame is done and we can move to next
        }

        // Transform the player's location and hide the canvas
        FingerObject.i.TransformPlayer();
        FadeOut();
    }

    // Play the canvas fade in clip 
    private void FadeIn()
    {
        if (anim != null && fadeInClip != null)
        {
            anim.clip = fadeInClip; 
            anim.Play();
        }
    }

    // Play the canvas fade out clip 
    private void FadeOut()
    {
        if (anim != null && fadeOutClip != null)
        {
            anim.clip = fadeOutClip; 
            anim.Play();
        }
    }
}
