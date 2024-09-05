using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TankLike.UnitControllers;
using TMPro;
using TankLike.UI.HUD;
using TankLike.Combat;

namespace TankLike.UI
{
    public class HUDController : MonoBehaviour, IManager
    {
        [field: SerializeField] public List<PlayerHUD> PlayerHUDs { get; private set; }
        [field: SerializeField] public BossHUD BossHUD { private set; get; }
        [field: SerializeField] public CoinsHUD CoinsHUD { private set; get; }

        public bool IsActive { get; private set; }

        public void SetUp()
        {
            BossHUD.Enable(false);
            EnableHUD(true);

            GameManager.Instance.PlayersManager.Coins.OnCoinsAdded += DisplayCoins;
        }

        private void DisplayCoins(int addedCoins, int totalCoins)
        {
            CoinsHUD.DisplayCoinsText(totalCoins);
        }

        public void EnableHUD(bool enable)
        {
            PlayerHUDs.ForEach(h => h.Enable(false));

            if (!enable)
            {
                return;
            }

            for (int i = 0; i < PlayersManager.PlayersCount; i++)
            {
                PlayerHUDs[i].Enable(enable);
            }
        }

        public void EnableBossHUD(bool enable)
        {
            BossHUD.Enable(enable);
        }

        public void Dispose()
        {
            GameManager.Instance.PlayersManager.Coins.OnCoinsAdded -= DisplayCoins;
        }
    }
}
