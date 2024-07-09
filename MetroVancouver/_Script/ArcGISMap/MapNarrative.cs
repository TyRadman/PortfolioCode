using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapNarrative : MonoBehaviour
{
    [SerializeField] private List<Animation> _pinPoints;
    [SerializeField] private AudioSource _source;
    [SerializeField] private TransitionController _transition;

    private void Start()
    {
        StartCoroutine(Process());
    }

    private IEnumerator Process()
    {
        _source.Play();
        yield return new WaitForSeconds(15f);
        _pinPoints.ForEach(p => p.gameObject.SetActive(true));
        _pinPoints.ForEach(p => p.Play());
        yield return new WaitForSeconds(_source.clip.length + 1f - 15f);
        _transition.FadeIn();
        yield return new WaitForSeconds(_transition.fadeInClip.length);
        SceneManager.LoadScene(2);
    }
}
