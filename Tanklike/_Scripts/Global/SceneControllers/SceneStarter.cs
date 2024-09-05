using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public class SceneStarter : MonoBehaviour
    {
        [SerializeField] private SceneController _sceneController;

        public void StartScene()
        {
            Debug.Log("START SCENE USING SceneStater");
            _sceneController.StarterSetUp();
        }
    }
}
