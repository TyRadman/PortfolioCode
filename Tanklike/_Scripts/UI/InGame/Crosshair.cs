using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UI.InGame
{
    public class Crosshair : MonoBehaviour
    {
        private Transform _camera;
        [field: SerializeField] public CrossHairVisuals Visuals { get; private set; }
        [SerializeField] private Transform _cursorChild;
        [field: SerializeField] public bool IsActive { set; get; }

        private void Awake()
        {
            _camera = Camera.main.transform;
        }

        public void SetUp()
        {
            gameObject.SetActive(false);
            transform.parent = null;

            Visuals.SetUp();
        }

        public void Enable(bool enable)
        {
            if (enable)
            {
                StartCoroutine(LookAtCameraProcess());
                Visuals.ShowCrossHair();
            }
            else
            {
                Visuals.HideCrossHair();
                StopAllCoroutines();
            }
        }

        private IEnumerator LookAtCameraProcess()
        {
            while (true)
            {
                // Get the forward direction of the camera
                Vector3 cameraForward = _camera.transform.forward;

                // Rotate the object to face the camera's forward direction
                _cursorChild.rotation = Quaternion.LookRotation(-cameraForward, Vector3.up);
                yield return null;
            }
        }

        public void SetColor(Color color)
        {
            Visuals.SetColor(color);
        }
    }
}
