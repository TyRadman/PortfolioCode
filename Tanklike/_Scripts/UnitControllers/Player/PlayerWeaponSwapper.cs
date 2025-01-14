using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.UnitControllers
{
    using Utils;
    using UI.HUD;
    using TankLike.Combat.Abilities;

    public class PlayerWeaponSwapper : MonoBehaviour, IController, IInput, IDisplayedInput
    {
        public enum WeaponType
        {
            BaseWeapon = 0, SuperAbility = 1
        }

        public bool IsActive { get; private set; }

        private WeaponType _weaponType;
        private PlayerComponents _components;
        private PlayerShooter _shooter;
        private PlayerSuperAbilities _superAbilities;
        private PlayerHUD _HUD;
        private bool _canSwap = true;

        private const float COOLDOWN_TIME = 0.4f;

        public void SetUp(IController controller)
        {
            if(controller == null || controller is not PlayerComponents playerComponent)
            {
                Helper.LogSetUpNullReferences(_components.GetType());
                return;
            }

            _components = playerComponent;
            _shooter = _components.Shooter as PlayerShooter;
            _superAbilities = _components.SuperAbilities;

            _HUD = GameManager.Instance.HUDController.PlayerHUDs[_components.PlayerIndex];

            UpdateInputDisplay(_components.PlayerIndex);
        }

        #region Input
        public void SetUpInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);

            playerMap.FindAction(c.Player.SwapWeapons.name).performed += SwapInputAction;
        }

        public void DisposeInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);

            playerMap.FindAction(c.Player.SwapWeapons.name).performed -= SwapInputAction;
        }

        public void UpdateInputDisplay(int playerIndex)
        {
            int actionIconIndex = GameManager.Instance.InputManager.GetButtonBindingIconIndex(InputManager.Controls.Player.SwapWeapons.name, playerIndex);

            GameManager.Instance.HUDController.PlayerHUDs[_components.PlayerIndex].SetWeaponSwapKey(Helper.GetInputIcon(actionIconIndex));
        }
        #endregion

        private void SwapInputAction(InputAction.CallbackContext _)
        {
            if (!IsActive)
            {
                return;
            }

            if(_weaponType == WeaponType.BaseWeapon)
            {
                ActivateSuperAbility();
            }
            else
            {
                ActivateBaseWeapon();
            }
        }

        public void ActivateBaseWeapon()
        {
            if (!_superAbilities.CanUnequip() || !_shooter.CanEquip() || !_canSwap)
            {
                return;
            }

            _weaponType = WeaponType.BaseWeapon;
            _superAbilities.Unequip();
            _shooter.Equip();

            _HUD.OnBaseWeaponEquipped();

            DisableSwapping();
        }

        public void ActivateDefaultWeapon()
        {
            if (!_superAbilities.CanUnequip() || !_shooter.CanEquip() || !_canSwap)
            {
                return;
            }

            _weaponType = WeaponType.BaseWeapon;
            _superAbilities.Unequip();
            _shooter.Equip();

            DisableSwapping();
        }

        public void ActivateSuperAbility()
        {
            if(!_shooter.CanUnequip() || !_superAbilities.CanEquip() || !_canSwap)
            {
                return;
            }

            _weaponType = WeaponType.SuperAbility;

            _shooter.Unequip();
            _superAbilities.Equip();

            _HUD.OnSuperAbilityEquipped();

            DisableSwapping();
        }

        public WeaponType GetSwappingStatus()
        {
            return _weaponType;
        }

        #region Utilities
        private void DisableSwapping()
        {
            _canSwap = false;

            Invoke(nameof(EnableSwapping), COOLDOWN_TIME);
        }

        private void EnableSwapping()
        {
            _canSwap = true;
        }
        #endregion

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
            SetUpInput(_components.PlayerIndex);

            _components.OnPlayerActivatedOnce += ActivateBaseWeapon;
        }

        public void Dispose()
        {
            DisposeInput(_components.PlayerIndex);
        }
        #endregion
    }
}
