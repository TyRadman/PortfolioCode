using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Space.UI.HUD;

namespace SpaceWar
{
    public class PlayerCoins : MonoBehaviour
    {
        [SerializeField] private int m_Coins = 0;
        [SerializeField] private PlayerStats m_Stats;
        [SerializeField] private ParticlesPooling m_CoinParticles;

        private void Start()
        {
            AddCoins(10000);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Coin"))
            {
                // make the coin disappear
                collision.gameObject.SetActive(false);
                // player effects
                PoolingSystem.Instance.UseParticles(m_CoinParticles, collision.transform.position);
            }
        }

        public void AddCoins(int _amount)
        {
            m_Coins += _amount;
            GameManager.i.HUDManager.UpdateCoins(m_Coins, m_Stats.PlayerIndex);
        }

        public int GetCurrentCoins()
        {
            return m_Coins;
        }
    }
}