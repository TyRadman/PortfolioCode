using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public class ObstaclesVanisher : MonoBehaviour
    {
        private Transform _camera;
        [SerializeField] private LayerMask _wallsLayermask;
        [SerializeField] private List<Transform> _players = new List<Transform>();
        [SerializeField] [Range(0f, 1f)] private float _minAlphaValue = 0f;
        private VanishingWall _previousWall;

        public void SetUp()
        {
            var players = GameManager.Instance.PlayersManager.GetPlayerProfiles();
            players.ForEach(p => _players.Add(p.transform));
        }

        private void Awake()
        {
            _camera = Camera.main.transform;

        }

        private void Update()
        {
            LookAtPlayer();
        }

        private void LookAtPlayer()
        {
            for (int i = 0; i < _players.Count; i++)
            {
                Vector3 directionToPlayer = _players[i].position - _camera.position;

                if (Physics.Raycast(_camera.position, directionToPlayer, out RaycastHit hit, 50, _wallsLayermask))
                {
                    if(hit.collider != null)
                    {
                        // if it's a new wall then, otherwise, do nothing
                        if (_previousWall == null || _previousWall.gameObject != hit.collider.gameObject)
                        {
                            // show the previous wall if it exists
                            if (_previousWall != null)
                            {
                                _previousWall.ShowObject(_minAlphaValue);
                            }

                            // set a new wall
                            _previousWall = hit.collider.GetComponent<VanishingWall>();

                            if (_previousWall != null)
                            {
                                _previousWall.HideObject(_minAlphaValue);
                            }
                        }
                    }
                }
            }
        }
    }
}
