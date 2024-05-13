using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.Cam
{
    public class CameraShake : MonoBehaviour
    {
        [SerializeField] private CameraShakeSettings _defaultShakeSettings;
        [SerializeField] private List<CameraShakeSettings> _cameraShakeSettings;
        private CinemachineBasicMultiChannelPerlin _mainBasicMultiChannelPerlin;

        public void SetUp(CinemachineVirtualCamera camera)
        {
            _mainBasicMultiChannelPerlin = camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            _mainBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
            _mainBasicMultiChannelPerlin.m_FrequencyGain = _defaultShakeSettings.Frequency;
        }

        public void ShakeCamera(CameraShakeType type)
        {
            CameraShakeSettings setting = _cameraShakeSettings.Find(s => s.Type == type);

            if(setting == null) 
            {
                Debug.LogError($"No shake settings of type {type}");
                return;
            }

            _mainBasicMultiChannelPerlin.m_AmplitudeGain = setting.Intensity;
            StartCoroutine(ShakeCameraRoutine(setting.Intensity, setting.Time, setting.Smooth));
        }

        public void ShakeCamera(CameraShakeSettings shake)
        {
            if (shake == null)
            {
                Debug.LogError($"No Shake passed");
                return;
            }

            _mainBasicMultiChannelPerlin.m_AmplitudeGain = shake.Intensity;
            StartCoroutine(ShakeCameraRoutine(shake.Intensity, shake.Time, shake.Smooth));
        }

        private IEnumerator ShakeCameraRoutine(float startingIntensity, float time, bool smooth)
        {
            float timer = 0f;

            while (timer < time)
            {
                if (smooth)
                    _mainBasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(startingIntensity, 0f, timer / time);

                timer += Time.deltaTime;
                yield return null;
            }

            _mainBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
        }
    }
}
