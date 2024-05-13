using System;
using System.Collections;
using System.Collections.Generic;
using TankLike.Combat;
using TankLike.UI.InGame;
using TankLike.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace TankLike.UnitControllers
{
    public class PlayerHoldAction : TankOnHoldAction, IController, IInput, IDisplayedInput
    {
        [SerializeField] private float _holdDuration;
        private Coroutine _holdCoroutine;
        private Coroutine _holdUpCoroutine;
        private bool _successfulHold;
        [SerializeField] private PlayerComponents _components;
        [SerializeField] private PlayerOverHeat _overHeat;
        private Ability _currentOnHoldAbility;
        [SerializeField] private HoldAbilityHolder _holdSkillPrefab;
        [SerializeField] private List<Ability> _onHoldDownSkills = new List<Ability>();
        [SerializeField] private SegmentedBar _bar;

        [Header("Effects")]
        [SerializeField] private ParticleSystem _holdChargeEffect;
        [SerializeField] private ParticleSystem _holdReadyEffect;
        [SerializeField] private ParticleSystem _holdReleaseEffect;
        private HoldAbilityHolder _Ability;
        [SerializeField] protected bool _isActive;

        public bool IsActive { get; private set; }

        public void SetUp()
        {
            SetUpInput(_components.PlayerIndex);
            _isActive = false;
            AddHoldDownSkill(_holdSkillPrefab);
            _bar.SetUp();
            _bar.SetTotalAmount(0f);
        }

        #region Input
        public void SetUpInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            playerMap.FindAction(c.Player.Hold.name).performed += OnHoldDown;
            playerMap.FindAction(c.Player.Hold.name).canceled += OnHoldUp;
        }

        public void DisposeInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap playerMap = InputManager.GetMap(_components.PlayerIndex, ActionMap.Player);
            playerMap.FindAction(c.Player.Hold.name).performed -= OnHoldDown;
            playerMap.FindAction(c.Player.Hold.name).canceled -= OnHoldUp;
        }

        public void UpdateInput(int playerIndex)
        {
            // set the input key for the hold action
            string key = GameManager.Instance.InputManager.GetButtonBindingKey(
                InputManager.Controls.Player.Hold.name, _components.PlayerIndex);
            GameManager.Instance.HUDController.PlayerHUDs[_components.PlayerIndex].SetHoldDownInfo(_Ability.Ability.GetIcon(), key);
        }
        #endregion

        private void OnHoldDown(InputAction.CallbackContext context)
        {
            if (!_isActive)
            {
                return;
            }

            if (_holdCoroutine != null)
            {
                StopCoroutine(_holdCoroutine);
            }

            _holdCoroutine = StartCoroutine(HoldRoutine());
        }

        private void OnHoldUp(InputAction.CallbackContext context)
        {
            _holdChargeEffect.Stop();

            if (_successfulHold)
            {
                _currentOnHoldAbility.OnActivateAbility();
                _overHeat.AddShotBars(-1);
                _holdReadyEffect.Stop();
                _holdReleaseEffect.Play();
            }

            if (_holdCoroutine != null)
            {
                StopCoroutine(_holdCoroutine);
            }

            if (_holdUpCoroutine != null)
            {
                StopCoroutine(_holdUpCoroutine);
            }

            _holdUpCoroutine = StartCoroutine(HoldUpRoutine());

            _successfulHold = false;
        }

        private IEnumerator HoldRoutine()
        {
            float timer = 0f;
            _successfulHold = false;
            _holdChargeEffect.Play();

            while (timer < _holdDuration)
            {
                timer += Time.deltaTime;
                float amount = timer / _holdDuration;
                _bar.SetTotalAmount(amount);
                GameManager.Instance.HUDController.PlayerHUDs[_components.PlayerIndex].SetHoldDownChargeAmount(1 - amount, _components.PlayerIndex);
                yield return null;
            }

            GameManager.Instance.HUDController.PlayerHUDs[_components.PlayerIndex].SetHoldDownChargeAmount(0f, _components.PlayerIndex);
            _successfulHold = true;
            _holdChargeEffect.Stop();
            _holdReadyEffect.Play();
        }

        private IEnumerator HoldUpRoutine()
        {
            float t = Mathf.InverseLerp(1f, 0f, _bar.GetAmount());
            float timer = Mathf.Lerp(0, 0.2f, t);

            while (timer < 0.2f)
            {
                timer += Time.deltaTime;
                float amount = Mathf.Clamp(1 - timer / 0.2f, 0f, 1f);
                _bar.SetTotalAmount(amount); 
                GameManager.Instance.HUDController.PlayerHUDs[_components.PlayerIndex].SetHoldDownChargeAmount(1 - amount, _components.PlayerIndex);

                yield return null;
            }

            _bar.SetTotalAmount(0f); 
        }

        public void AddHoldDownSkill(HoldAbilityHolder ability)
        {
            _Ability = ability;
            _currentOnHoldAbility = Instantiate(ability.Ability);
            UpdateInput(_components.PlayerIndex);
            _onHoldDownSkills.Add(_currentOnHoldAbility);
            _currentOnHoldAbility.SetUp(transform);
            _holdDuration = ability.HoldDownDuration;

            if (_currentOnHoldAbility != null)
            {
                _isActive = true;
            }
        }

        public Ability GetHoldDownSkill()
        {
            return _currentOnHoldAbility;
        }

        public List<Ability> GetHoldDownSkills()
        {
            return _onHoldDownSkills;
        }

        public void SetOnHoldSkill(Ability ability)
        {
            _currentOnHoldAbility = ability;

            if (!_onHoldDownSkills.Exists(s => s == ability))
            {
                _onHoldDownSkills.Add(ability);
            }
        }

        public void ForceStopHoldAction()
        {
            _successfulHold = false;

            if (_holdCoroutine != null)
            {
                StopCoroutine(_holdCoroutine);
            }

            _bar.SetTotalAmount(0f);
            _holdChargeEffect.Stop();
            _holdReadyEffect.Stop();
            _holdReleaseEffect.Stop();
        }

        public void Enable(bool enable)
        {
            _isActive = enable;

            if (!_isActive)
            {
                ForceStopHoldAction();
            }
        }

        #region IController
        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
            ForceStopHoldAction();
        }

        public void Restart()
        {
            DisposeInput(_components.PlayerIndex);
            IsActive = false;

            if (_holdCoroutine != null)
            {
                StopCoroutine(_holdCoroutine);
            }

            if (_holdUpCoroutine != null)
            {
                StopCoroutine(_holdUpCoroutine);
            }
        }

        public void Dispose()
        {
            IsActive = false;
        }
        #endregion
    }
}
