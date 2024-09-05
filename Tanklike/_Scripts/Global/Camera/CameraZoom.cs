using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace TankLike.Cam
{
    public class CameraZoom : MonoBehaviour
    {
        [SerializeField] private float _normalZoomValue = 18.7f;
        [SerializeField] private float _fightZoomValue = 25f;
        [SerializeField] private float _zoomDuration = 1f;
        [SerializeField] private AnimationCurve _zoomCurve;
        private CinemachineFramingTransposer _transposer;

        public void SetUp(CinemachineVirtualCamera camera)
        {
            _transposer = camera.GetCinemachineComponent<CinemachineFramingTransposer>();
            _transposer.m_CameraDistance = _normalZoomValue;
        }

        [ContextMenu("Zoom out")]
        public void SetToFightZoom()
        {
            GameManager.Instance.CameraManager.PlayerCameraFollow.SetOffsetMultiplier(_fightZoomValue / _normalZoomValue);
            StartCoroutine(ZoomProcess(_fightZoomValue));

        }

        [ContextMenu("Zoom in")]
        public void SetToNormalZoom()
        {
            GameManager.Instance.CameraManager.PlayerCameraFollow.SetOffsetMultiplier(1f);
            StartCoroutine(ZoomProcess(_normalZoomValue)); 
        }

        // original: 8
        // bossZoom: 4
        // bossFight:16
        //
        public void SetToZoomValue(float zoomValue)
        {
            GameManager.Instance.CameraManager.PlayerCameraFollow.SetOffsetMultiplier(zoomValue / _normalZoomValue);
            StartCoroutine(ZoomProcess(zoomValue));
        }

        private IEnumerator ZoomProcess(float newZoomValue)
        {
            float time = 0f;
            float startZoom = _transposer.m_CameraDistance;
            
            while (time < _zoomDuration)
            {
                time += Time.deltaTime;
                float t = _zoomCurve.Evaluate(time / _zoomDuration);
                _transposer.m_CameraDistance = Mathf.Lerp(startZoom, newZoomValue, t);
                yield return null;
            }
        }
    }
}
