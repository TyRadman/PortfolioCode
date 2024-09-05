using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TankLike.Minimap;
using TankLike.UnitControllers;
using TankLike.Cam;

namespace TankLike.Cam
{
    public class CameraManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private CinemachineVirtualCamera _mainVirtualCamera;
        [field: SerializeField] public CameraShake Shake { get; private set; }
        [field: SerializeField] public CameraZoom Zoom { get; private set; }

        [field: SerializeField] public MainCameraFollow PlayerCameraFollow;
        [SerializeField] private CameraFollow _minimapCameraFollow;

        public CameraFollow MinimapCameraFollow => _minimapCameraFollow;
        public bool IsActive { get; private set; }

        #region IManager
        public void SetUp(bool testingScene)
        {
            IsActive = true;

            Shake.SetUp(_mainVirtualCamera);
            Zoom.SetUp(_mainVirtualCamera);

            if (testingScene)
            {
                return;
            }

            PlayerCameraFollow.SetUp();
            _minimapCameraFollow.SetUp();
        }

        public void Dispose()
        {
            IsActive = false;
        }
        #endregion

        public void SetCamerasLimits(CameraLimits limits)
        {
            PlayerCameraFollow.SetLimits(limits);
            _minimapCameraFollow.SetLimits(limits);
        }

        // Used for test scenes
        public void ResetCameraLimit()
        {
            PlayerCameraFollow.ResetLimits();
            //_minimapCameraFollow.SetCurrentLimits(limits);
        }

        public void EnableCamerasInterpolation(bool enable)
        {
            PlayerCameraFollow.EnableInterpolation(enable);
            _minimapCameraFollow.EnableInterpolation(enable);
        }

        public Transform GetMainCamera()
        {
            return _mainCamera.transform;
        }
    }
}