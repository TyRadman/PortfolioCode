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
    public class HUDController : MonoBehaviour
    {
        [field: SerializeField] public List<PlayerHUD> PlayerHUDs { get; private set; }
        [field: SerializeField] public BossHUD BossHUD { private set; get; }

        public void SetUp()
        {
            BossHUD.Enable(false);
            EnableHUD(true);
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
    }
}
