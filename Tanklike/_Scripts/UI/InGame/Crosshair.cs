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
                _cursorChild.LookAt(_camera);
                yield return null;
            }
        }

        public void SetColor(Color color)
        {
            Visuals.SetColor(color);
        }
    }
}
