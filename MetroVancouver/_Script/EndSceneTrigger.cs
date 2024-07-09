using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndSceneTrigger : MonoBehaviour
{
    public SceneHandler _sceneHandler; 
    public float _timeLimitMinutes; // change to int 
    [SerializeField] private int _sceneNumber = 0;

    void Start()
    {
        Invoke(nameof(CallSceneTransition), _timeLimitMinutes * 60f);
    }

    private void CallSceneTransition()
    {
        if (_sceneHandler != null)
        {
            _sceneHandler.SceneTransition(_sceneNumber);
        }
    }
}
