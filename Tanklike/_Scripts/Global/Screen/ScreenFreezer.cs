using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.ScreenFreeze
{
    public class ScreenFreezer : MonoBehaviour
    {
        [SerializeField] private float _freezingScale = 0.1f;
        private bool _isFreezing = false;
        private ScreenFreezeData _currentData;
        private float _timer = 0f;

        public void FreezeScreen(ScreenFreezeData data)
        {
            if (data == null)
            {
                Debug.Log("No freezing data");
                return;
            }

            _isFreezing = true;
            _currentData = data;
            StartCoroutine(ReleaseScreen());
        }

        private IEnumerator ReleaseScreen(float time = 0)
        {
            _timer = time;
            AnimationCurve curve = _currentData.Curve;

            while(_timer < _currentData.Duration)
            {
                _timer += Time.unscaledDeltaTime;

                Time.timeScale = Mathf.Lerp(_freezingScale, 1f, 1 - curve.Evaluate(_timer / _currentData.Duration));
                yield return null;
            }

            _isFreezing = false;
        }

        public void PauseFreeze()
        {
            if (!_isFreezing)
            {
                return;
            }

            StopAllCoroutines();
        }

        public void ResumeFreeze()
        {
            if (!_isFreezing)
            {
                return;
            }

            StartCoroutine(ReleaseScreen(_timer));
        }
    }
}
