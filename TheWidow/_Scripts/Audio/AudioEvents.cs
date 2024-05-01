using UnityEngine;

/// <summary>
/// A class that is used in the animation events that take no parameters
/// </summary>
public class AudioEvents : MonoBehaviour
{
    public void PlayFootStepAudio() => AudioManager.Instance.PlayEnemyAudio("EnemyFootSteps", true);
    public void ButtonHover() => AudioManager.Instance.PlayEnemyAudio("ButtonHover", true);
    public void ButtonPress() => AudioManager.Instance.PlayEnemyAudio("ButtonPress", true);
}