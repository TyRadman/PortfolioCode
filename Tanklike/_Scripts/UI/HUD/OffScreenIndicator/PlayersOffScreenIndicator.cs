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
        private Camera _camera;
        [SerializeField] private float _offset = 10f;
        [SerializeField] private float _lerpSpeed = 5f;
        private bool _isActive = false;

        private void Awake()
        {
            _camera = Camera.main;
        }

        public void SetUp()
        {
            // enable the off-screen detection only if there are multiple players
            if (PlayersManager.PlayersCount > 1)
            {
                _isActive = true;
                _players = new List<OffScreenIndicatorProfile>();

                for (int i = 0; i < PlayersManager.PlayersCount; i++)
                {
                    Transform playerTransform = GameManager.Instance.PlayersManager.GetPlayer(i).transform;
                    
                    _players.Add(new OffScreenIndicatorProfile()
                    {
                        PlayerTransform = playerTransform,
                        FollowPlayer = true,
                        Icon = _indicatorIcons[i]
                    });

                    _players[i].Icon.SetColor(GameManager.Instance.PlayersManager.GetPlayerColor(i));
                }
            }
        }

        private void Update()
        {
            if (!_isActive)
            {
                return;
            }

            for (int i = 0; i < 2; i++)
            {
                DetectPlayer(i);
            }
        }

        private void DetectPlayer(int playerIndex)
        {
            if (!_players[playerIndex].FollowPlayer)
            {
                return;
            }

            OffScreenIndicatorProfile player = _players[playerIndex];
            Vector3 screenPosition = _camera.WorldToScreenPoint(player.PlayerTransform.position);

            if (screenPosition.z > 0 && !IsInCameraView(screenPosition))
            {
                Transform icon = player.Icon.transform;

                if (!player.IsShown)
                {
                    player.IsShown = true;
                    player.Icon.ShowIcon();
                    // snap it to the desired position so that the players don't see it moving from across the screen with the lerp
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
                if (player.IsShown)
                {
                    player.IsShown = false;
                    player.Icon.HideIcon();
                }
            }
        }

        private bool IsInCameraView(Vector3 position)
        {
            return position.x > 0 && position.x < Screen.width &&
                position.y > 0 && position.y < Screen.height;
        }

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

            _players[playerIndex].FollowPlayer = enableOffScreenIndicator;

            if(!enableOffScreenIndicator && _players[playerIndex].IsShown)
            {
                _players[playerIndex].Icon.HideIcon();
            }
        }
    }
}
