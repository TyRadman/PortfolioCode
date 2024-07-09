using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    [SerializeField] private TransitionController _transitionScreenCanvas; 
    private int _nextSceneBuildIndex = 1; 

    // Function to trigger fade in animation and play the next scene 
    public void SceneTransition(int sceneIndex)
    {
        _nextSceneBuildIndex = sceneIndex;

        if (_transitionScreenCanvas != null)
        {
            // Trigger animation 
            _transitionScreenCanvas.FadeIn();

            // Invoke next scene after animation completes 
            Invoke(nameof(LoadGameScene), _transitionScreenCanvas.fadeInClip.length);
        }
    }

    private void LoadGameScene()
    {
        SceneManager.LoadScene(_nextSceneBuildIndex);
    }
}
