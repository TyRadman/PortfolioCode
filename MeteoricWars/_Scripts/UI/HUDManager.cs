using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Space.UI.HUD
{
    public class HUDManager : MonoBehaviour
    {
        [SerializeField] private List<PlayerHUD> m_PlayerUIReferences;
        [SerializeField] private Gradient m_HealthGradient;

        public void SetUp()
        {
            // start with all the HUDs disabled
            m_PlayerUIReferences.ForEach(h => h.Enable(false));
        }

        public void EnablePlayerHUD(int playerIndex, bool enable)
        {
            m_PlayerUIReferences[playerIndex].Enable(enable);
        }

        public void UpdatePlayerHealth(float _amount, int _playerIndex, string _healthAmountText)
        {
            m_PlayerUIReferences[_playerIndex].HealthBarImage.fillAmount = _amount;
            Color healthColor = m_HealthGradient.Evaluate(_amount);
            m_PlayerUIReferences[_playerIndex].HealthBarImage.color = healthColor;
            m_PlayerUIReferences[_playerIndex].HealthIconImage.color = healthColor;
            m_PlayerUIReferences[_playerIndex].HealthText.text = _healthAmountText;
        }

        public void UpdatePlayerRegeneratedHealthBar(float _amount, int _playerIndex)
        {
            m_PlayerUIReferences[_playerIndex].RegenerationBarImage.fillAmount = _amount;
        }

        public void UpdatePlayerShootingPoints(float _amount, int _playerIndex, string _amountText, string _level)
        {
            m_PlayerUIReferences[_playerIndex].ShooterPointsImage.fillAmount = _amount;
            m_PlayerUIReferences[_playerIndex].ShooterPointsAmountText.text = _amountText;
            m_PlayerUIReferences[_playerIndex].ShooterLevelText.text = _level;
        }

        public void UpdateScoreText(float _amount, int _playerIndex)
        {
            m_PlayerUIReferences[_playerIndex].ScoreText.text = _amount.ToString();
        }

        public void SetLivesCount(int _lives, int _playerIndex)
        {
            m_PlayerUIReferences[_playerIndex].LivesText.text = $"x {_lives}";
        }

        public void UpdateCoins(int _coins, int _playerIndex)
        {
            m_PlayerUIReferences[_playerIndex].CoinsText.text = _coins.ToString();
        }
    }
}
