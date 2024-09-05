using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using UnityEngine;
using UnityEngine.UI;

namespace TankLike.UI.HUD
{
    public class PlayersOffScreenIndicator : MonoBehaviour
    {
        [SerializeField] private List<OffScreenIndicatorProfile> _players;
        [SerializeField] private List<OffScreenIcon> _indicatorIcons;
        [SerializeField] private Transform _indicatorsParent;
        private Camera _camera;
        [SerializeField] private float _offset = 10f;
        [SerializeField] private float _lerpSpeed = 5f;
        private bool _isActive = false;
        private List<OffScreenIndicatorProfile> _targets = new List<OffScreenIndicatorProfile>();

        private void Awake()
        {
            _camera = Camera.main;
        }

        public void SetUp()
        {
        }

        /// <summary>
        /// Adds a new target for the off screen indicator manager to keep track of.
        /// </summary>
        /// <param name="profile"></param>
        public void AddTarget(OffScreenIndicatorProfile profile)
        {
            _targets.Add(profile);

            if(profile.Icon == null)
            {
                profile.Icon = Instantiate(profile.IconPrefab, _indicatorsParent);
            }
        }

        public void RemoveTarget(OffScreenIndicatorProfile profile)
        {
            _targets.Remove(profile);
        }

        // TODO: run it manually if there are two players. Remove the update function
        private void Update()
        {
            for (int i = 0; i < _targets.Count; i++)
            {
                DetectTarget(_targets[i]);
            }
        }

        private void DetectTarget(OffScreenIndicatorProfile targetProfile)
        {
            if (!targetProfile.FollowTarget)
            {
                return;
            }

            // get the target's screen position
            Vector3 screenPosition = _camera.WorldToScreenPoint(targetProfile.TargetTransform.position);

            if (screenPosition.z > 0 && !IsInCameraView(screenPosition))
            {
                Transform icon = targetProfile.Icon.transform;

                if (!targetProfile.IsShown)
                {
                    targetProfile.IsShown = true;

                    targetProfile.Icon.ShowIcon();

                    icon.position = ClampToScreen(screenPosition);

                    return;
                }

                icon.position = Vector3.Lerp(icon.position, ClampToScreen(screenPosition),
                    _lerpSpeed * Time.deltaTime);

                // rotation
                icon.eulerAngles = Vector3.forward * GetAngle(icon.position);
            }
            else
            {
                if (targetProfile.IsShown)
                {
                    targetProfile.IsShown = false;
                    targetProfile.Icon.HideIcon();
                }
            }
        }

        /// <summary>
        /// Checks if an object's screen position is within the screen
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private bool IsInCameraView(Vector3 position)
        {
            return position.x > 0 && position.x < Screen.width &&
                position.y > 0 && position.y < Screen.height;
        }

        /// <summary>
        /// Ensures a position value is within the screen with a given offset
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private Vector3 ClampToScreen(Vector3 position)
        {
            position.x = Mathf.Clamp(position.x, _offset, Screen.width - _offset);
            position.y = Mathf.Clamp(position.y, _offset, Screen.height - _offset);
            return position;
        }

        private float GetAngle(Vector3 screenPosition)
        {
            Vector2 direction = (screenPosition - new Vector3(Screen.width / 2, Screen.height / 2, 0)).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            return angle - 90; 
        }

        public void EnableOffScreenIndicatorForPlayer(int playerIndex, bool enableOffScreenIndicator)
        {
            if(_players.Count == 0)
            {
                return;
            }

            _players[playerIndex].FollowTarget = enableOffScreenIndicator;

            if(!enableOffScreenIndicator && _players[playerIndex].IsShown)
            {
                _players[playerIndex].Icon.HideIcon();
            }
        }

        public void Enable(bool enable)
        {
            _isActive = enable;

            if(!_isActive)
            {
                ForceHideIcons();
            }
        }

        private void ForceHideIcons()
        {
            for (int i = 0; i < _players.Count; i++)
            {
                OffScreenIndicatorProfile player = _players[i];

                if(player.IsShown)
                {
                    player.IsShown = false;
                    player.Icon.HideIcon(2f);
                }
            }
        }
    }
}
