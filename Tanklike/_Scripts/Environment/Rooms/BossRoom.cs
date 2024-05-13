using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike.Environment
{
    public class BossRoom : Room
    {
        private BossData _bossData;
        private GameObject _boss;
        private Transform _bossSpawnPoint;
        [SerializeField] private AbilityConstraint _onCinematicConstraints;
        [field: SerializeField] public Vector3 RoomSize { get; private set; }


        public override void SetUpRoom()
        {
            base.SetUpRoom();

            foreach (GateInfo gate in GatesInfo.Gates)
            {
                gate.Gate.Setup(true, this);
            }
        }

        public override void LoadRoom()
        {
            base.LoadRoom();
            _boss = Instantiate(_bossData.BossPrefab, _bossSpawnPoint.position, Quaternion.Euler(0f, 180f, 0f));
        }

        public void SetBossSpawnPoint(Transform point)
        {
            _bossSpawnPoint = point;
        }

        public Transform GetSpawnPoint()
        {
            return _bossSpawnPoint;
        }

        public void SetBossData(BossData bossData)
        {
            _bossData = bossData;
            RoomSize = bossData.RoomSize;
        }

        public override void OnRoomEnteredHandler()
        {
            base.OnRoomEnteredHandler();

            // close the gates
            CloseGates();
            StartCoroutine(BossStartProcess());
        }

        private IEnumerator BossStartProcess()
        {
            // disable the players movement
            GameManager.Instance.PlayersManager.ApplyConstraints(true, _onCinematicConstraints);

            // Fade out background music
            GameManager.Instance.AudioManager.FadeOutBGMusic();

            GameManager.Instance.CameraManager.PlayerCameraFollow.MoveToPoint(
                _boss.transform, _bossData.CameraMovementToBossDuration);
            yield return new WaitForSeconds(_bossData.CameraMovementToBossDuration);

            GameManager.Instance.BossesManager.AddBoss(_boss.GetComponent<BossComponents>());
            yield return new WaitForSeconds(_bossData.BossAnimationDuration);
            GameManager.Instance.HUDController.EnableBossHUD(true);
            GameManager.Instance.CameraManager.Zoom.SetToZoomValue(_bossData.ZoomValue);

            GameManager.Instance.PlayersManager.ApplyConstraints(false, _onCinematicConstraints);
            // Switch the background music to the boss theme
            GameManager.Instance.AudioManager.SwitchBGMusic(_bossData.BossBGMusic);
            // Fade in background music
            GameManager.Instance.AudioManager.FadeInBGMusic();
            GameManager.Instance.CameraManager.PlayerCameraFollow.MoveBackToPlayers(_bossData.CameraMovementToBossDuration);

            yield return new WaitForSeconds(_bossData.CameraMovementToBossDuration);
            GameManager.Instance.CameraManager.PlayerCameraFollow.EnableFollowingPlayer();
        }
    }
}
