using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TankLike.Combat;
using TankLike.Utils;

namespace TankLike.UnitControllers
{
    public class PlayerShooter : TankShooter, IInput, IDisplayedInput
    {
        [SerializeField] private List<Weapon> _playerNormalShots;
        private PlayerComponents _playerComponents;

        public override void SetUp(TankComponents components)
        {
            SetUpInput(((PlayerComponents)components).PlayerIndex);
            _playerComponents = (PlayerComponents)components;

            base.SetUp(components);

            if (_playerComponents.AbilityData != null)
            {
                SetWeapon(_playerComponents.AbilityData.GetNormalShot());
            }
            else
            {
                SetWeapon(_startWeaponHolder);
            }

            OnShootStarted += _playerComponents.Crosshair.PlayShootingAnimation;
            OnShootStarted += _playerComponents.Overheat.ReduceAmmoBarByOne;
        }

        #region Input
        public void SetUpInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            playerMap.FindAction(c.Player.Shoot.name).performed += HandleShootInput;
        }

        public void DisposeInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            playerMap.FindAction(c.Player.Shoot.name).performed -= HandleShootInput;
        }

        public void UpdateInputDisplay(int playerIndex)
        {
            int shootActionIconIndex = GameManager.Instance.InputManager.GetButtonBindingIconIndex(
                InputManager.Controls.Player.Shoot.name, playerIndex);

            GameManager.Instance.HUDController.PlayerHUDs[playerIndex].SetWeaponInfo(_currentWeapon.GetIcon(), Helper.GetInputIcon(shootActionIconIndex));
        }
        #endregion

        private void HandleShootInput(InputAction.CallbackContext _)
        {
            if (!_canShoot)
            {
                return;
            }

            // if there is no ammo left, player the sound effect and return
            if (!_playerComponents.Overheat.HasEnoughShots(1))
            {
                GameManager.Instance.AudioManager.Play(GameManager.Instance.Constants.Audios.OnEmptyAmmoSound);
                return;
            }

            //_playerComponents.Crosshair.PlayShootingAnimation();
            Shoot();
            //_playerComponents.Overheat.ReduceAmmoBarByOne();
        }

        public override void SetWeapon(WeaponHolder weapon)
        {
            base.SetWeapon(weapon);
            UpdateInputDisplay(((PlayerComponents)_components).PlayerIndex);
        }

        //public void AddNormalShot(WeaponHolder weapon)
        //{
        //    // clone this SO and add it to the player
        //    Weapon normalShot = Instantiate(weapon.Weapon);

        //    _playerNormalShots.Add(normalShot);

        //    if (_currentWeapon == null)
        //    {
        //        _currentWeapon = normalShot;
        //    }
        //}

        protected override void EnableShooting()
        {
            StartCoroutine(EnableShootingProcess());
        }

        private IEnumerator EnableShootingProcess()
        {
            float timer = 0f;
            GameManager.Instance.HUDController.PlayerHUDs[((PlayerComponents)_components).PlayerIndex].SetWeaponChargeAmount(1f, 0);

            while (timer < _coolDownTime)
            {
                timer += Time.deltaTime;
                GameManager.Instance.HUDController.PlayerHUDs[((PlayerComponents)_components).PlayerIndex].SetWeaponChargeAmount(1 - timer / _coolDownTime, 0);
                yield return null;
            }

            _canShoot = true;
        }

        #region IController
        public override void Deactivate()
        {
            OnShootStarted = null;
            base.Deactivate();
        }

        public override void Restart()
        {
            OnShootStarted = null;
            base.Restart();
            DisposeInput(((PlayerComponents)_components).PlayerIndex);
        }

        public override void Dispose()
        {
            OnShootStarted = null;
            base.Dispose();
        }
        #endregion
    }
}
