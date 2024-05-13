 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TankLike.UI
{
    public class ResultsUIController : MonoBehaviour
    {
        [SerializeField] private GameObject _gameoverScreen;

        private bool _isGameover;

        public void DisplayGameoverScreen()
        {
            _gameoverScreen.SetActive(true);
            _isGameover = true;
        }

        // super temporary
        private void Update()
        {
            if (!_isGameover) return;

            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
}
