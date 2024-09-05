using System.Collections;
using System.Collections.Generic;
using TankLike.Cheats;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike
{
	[CreateAssetMenu(fileName = NAME + "Invincibility", menuName = ROOT + "Invincibility")]
	public class Cheat_Invincibility : Cheat
    {
        [SerializeField] private AbilityConstraint _constaint;
         
        public override void Initiate()
        {
            base.Initiate();
            Button.SetButtonColor(false);
        }

        public override void PerformCheat()
        {
            _enabled = !_enabled;
            //GameManager.Instance.PlayersManager.GetPlayerProfiles().ForEach(p => p.Health.SetInvincible(_enabled));
            GameManager.Instance.PlayersManager.GetPlayerProfiles().ForEach(p => p.Constraints.ApplyConstraints(_enabled, _constaint));
            Button.SetButtonColor(_enabled);
        }
	}
}
