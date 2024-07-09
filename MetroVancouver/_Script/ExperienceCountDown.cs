using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceCountDown : MonoBehaviour
{
    [SerializeField] private Vector2 _experienceDuration;

    void Start()
    {
        StartCoroutine(CountDownProcess());
    }

    private IEnumerator CountDownProcess()
    {
        yield return new WaitForSeconds(_experienceDuration.x * 60 + _experienceDuration.y);

        VideoPlayerManager.Instance.SkipOrbs = true;

        // don't move to the next step before the video stops playing
        while (VideoPlayerManager.Instance.VideoIsPlaying) yield return null;

        VideoPlayerManager.Instance.StartEndExperienceCountDown();
    }
}
