using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TankLike
{
    public class CheatsManager : MonoBehaviour
    {
        [SerializeField] private bool _cheatsActive = false;
        [SerializeField] private GameObject _cheatsVisuals;
        [SerializeField] private TextMeshProUGUI _superToggleText;
        [SerializeField] private TextMeshProUGUI _invincibilityToggleText;
        private const string SUPER_TOGGLE_TEXT = "2\nToggle super: ";
        private const string INVINCIBILITY_TOGGLE_TEXT = "5\nInvincible: ";
        private const int COINS_TO_ADD = 200;
        private bool _superIsOn = false;
        private bool _isInvincible = false;

        private void Awake()
        {
            _cheatsVisuals.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Slash))
            {
                _cheatsActive = !_cheatsActive;
                _cheatsVisuals.SetActive(_cheatsActive);
            }

            HandleCheats();
        }

        private void HandleCheats()
        {
            ChargeSuper();
            ToggleSuper();
            AddCoins();
            ReFillHealth();
            EnableInvincibility();
            DestroyAllEnemies();
            KillPlayer();
        }

        private void ChargeSuper()
        {
            if (!_cheatsActive) return;

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                GameManager.Instance.PlayersManager.GetPlayerProfiles().ForEach(p => p.SuperRecharger.FullyChargeSuperAbility());
            }
        }

        private void AddCoins()
        {
            if (!_cheatsActive) return;

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                GameManager.Instance.PlayersManager.AddCoins(COINS_TO_ADD);
            }
        }

        private void ToggleSuper()
        {
            if (_superIsOn)
            {
                GameManager.Instance.PlayersManager.GetPlayerProfiles().ForEach(p => p.SuperRecharger.FullyChargeSuperAbility());
            }

            if (!_cheatsActive) return;

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                _superIsOn = !_superIsOn;
                string onOff = _superIsOn ? "On" : "Off";
                _superToggleText.text = $"{SUPER_TOGGLE_TEXT} {onOff}";
            }
        }

        private void ReFillHealth()
        {
            if (!_cheatsActive) return;

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                GameManager.Instance.PlayersManager.GetPlayerProfiles().ForEach(p => p.Health.Heal(1000));
            }
        }

        private void EnableInvincibility()
        {
            if (_isInvincible)
            {
                GameManager.Instance.PlayersManager.GetPlayerProfiles().ForEach(p => p.Health.EnableTakingDamage(!_isInvincible));
            }

            if (!_cheatsActive) return;

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                _isInvincible = !_isInvincible;
                GameManager.Instance.PlayersManager.GetPlayerProfiles().ForEach(p => p.Health.EnableTakingDamage(!_isInvincible));
                _invincibilityToggleText.text = $"{INVINCIBILITY_TOGGLE_TEXT} {(_isInvincible ? "on" : "off")}";
            }
        }

        private void DestroyAllEnemies()
        {
            if (!_cheatsActive)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                GameManager.Instance.EnemiesManager.DestroyAllEnemies();
            }
        }

        public void KillPlayer()
        {
            if (!_cheatsActive)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                GameManager.Instance.PlayersManager.GetPlayer(0).Health.Die();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                GameManager.Instance.PlayersManager.GetPlayer(1).Health.Die();
            }
        }
    }
}
