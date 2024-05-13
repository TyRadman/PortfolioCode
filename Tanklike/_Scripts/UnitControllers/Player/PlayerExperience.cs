using System.Collections;
using System.Collections.Generic;
using TankLike.Misc;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class PlayerExperience : MonoBehaviour, IController
    {
        [SerializeField] private int[] _maxExperiencePerLevel;
        private int _currentExperience;
        private int _currentLevel;
        public System.Action OnLevelUp;
        [SerializeField] private PlayerComponents _components;
        public Transform Transform => transform;

        public bool IsActive { get; private set; }

        public void SetUp()
        {
            _currentLevel = 0;
            _currentExperience = 0;
            GameManager.Instance.ReportManager.OnEnemyKill += IncreaseExperience;
            GameManager.Instance.HUDController.PlayerHUDs[_components.PlayerIndex].ResetExperienceBar();
            GameManager.Instance.HUDController.PlayerHUDs[_components.PlayerIndex].UpdateLevelText(_currentLevel + 1);
        }

        public void IncreaseExperience(EnemyData data, int playerIndex)
        {
            //dirty
            if (playerIndex != _components.PlayerIndex)
            {
                return;
            }

            if (_currentLevel >= _maxExperiencePerLevel.Length)
            {
                return;
            }

            _currentExperience += data.ExperiencePerKill;
            GameManager.Instance.HUDController.PlayerHUDs[_components.PlayerIndex].UpdateExperienceBar(_currentExperience, _maxExperiencePerLevel[_currentLevel]);

            if (_currentExperience >= _maxExperiencePerLevel[_currentLevel])
            {
                _currentExperience = 0;
                GameManager.Instance.HUDController.PlayerHUDs[_components.PlayerIndex].ResetExperienceBar();
                OnLevelUp?.Invoke(); //add perk point in the player upgrades class?

                _currentLevel++;
                _currentLevel = Mathf.Clamp(_currentLevel, 0, _maxExperiencePerLevel.Length);
                GameManager.Instance.HUDController.PlayerHUDs[_components.PlayerIndex].UpdateLevelText(_currentLevel + 1);

                AddLevelUpEffect();
            }
        }

        public void AddLevelUpEffect()
        {
            ParticleSystemHandler vfx = GameManager.Instance.VisualEffectsManager.Buffs.LevelUp;
            vfx.transform.parent = transform;
            vfx.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(-90f, 0f, 0f));
            vfx.gameObject.SetActive(true);
            vfx.Play();
        }

        #region IController
        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Restart()
        {
            IsActive = false;
        }

        public void Dispose()
        {
        }
        #endregion
    }
}
