using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using TankLike.UI.Inventory;

namespace TankLike.UI.Workshop
{
    public class ResurrectionNavigatable : Navigatable, IInput
    {
        [SerializeField] private Image _processBar;
        [SerializeField] private float _loadDuration = 3f;
        [SerializeField] private TextMeshProUGUI _coinsText;
        [SerializeField] private Color _sufficientCoinsColor;
        [SerializeField] private Color _insufficientCoinsColor;
        [SerializeField] private TextMeshProUGUI _resurrectionActionText;
        [SerializeField] private TextMeshProUGUI _coinsWarningText;
        private const float WARNING_DURATION = 1.5f;
        private const string REVIVE_TEXT = "Revive player ";
        private const string REVIVE_ACTION_TEXT = " - Hold ";
        private const string NO_REVIVAL_MESSAGE = "No dead players to revive";
        private string _actionKey;
        private bool _revived = false;

        private void Start()
        {
            _coinsWarningText.enabled = false;
        }

        #region Open and Close
        public override void Open(int playerIndex)
        {
            if (!IsPlayerDead())
            {
                // do other things to indicate the page is not available like changing the font color
                _resurrectionActionText.text = NO_REVIVAL_MESSAGE;
                return;
            }

            // set the text that will display the message to the player "Revive player 2 - hold Q" for instance
            SetReviveText();
            base.Open(playerIndex);
            SetPlayerIndex(playerIndex);
            SetUpInput(playerIndex);
            
            _coinsText.text = $"{Constants.REVIVAL_COST} / {GameManager.Instance.PlayersManager.CoinsAmount}";
            _coinsText.color = PlayerHasEnoughCoins() ? _sufficientCoinsColor : _insufficientCoinsColor;
        }

        public override void Close(int playerIndex)
        {
            if (!IsPlayerDead())
            {
                return;
            }

            _revived = false;
            base.Close(playerIndex);
            SetPlayerIndex(-1);
            DisposeInput(playerIndex);
        }
        #endregion

        #region Input
        public void SetUpInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap UIMap = InputManager.GetMap(playerIndex, ActionMap.UI);
            UIMap.FindAction(c.UI.Submit.name).started += LoadResurrection;
            UIMap.FindAction(c.UI.Submit.name).canceled += StopResurrectionLoading;
            _actionKey = UIMap.FindAction(c.UI.Submit.name).GetBindingDisplayString();
        }

        public void DisposeInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap UIMap = InputManager.GetMap(playerIndex, ActionMap.UI);
            UIMap.FindAction(c.UI.Submit.name).started -= LoadResurrection;
            UIMap.FindAction(c.UI.Submit.name).canceled -= StopResurrectionLoading;
        }
        #endregion

        private void SetReviveText()
        {
            int playerNumber = GameManager.Instance.PlayersManager.GetInactivePlayerIndex() + 1;
            _resurrectionActionText.text = $"{REVIVE_TEXT} {playerNumber} {REVIVE_ACTION_TEXT} {_actionKey}";
        }

        private void LoadResurrection(InputAction.CallbackContext _)
        {
            StopAllCoroutines();

            if (!PlayerHasEnoughCoins())
            {
                CancelInvoke();
                _coinsWarningText.enabled = true;
                Invoke(nameof(DisableCoinsWarningText), WARNING_DURATION);
                return;
            }

            if(!IsPlayerDead())
            {
                // there are no dead players
                return;
            }

            StartCoroutine(LoadingProcess());
        }

        private void DisableCoinsWarningText()
        {
            _coinsWarningText.enabled = false;
        }

        private IEnumerator LoadingProcess()
        {
            float time = _processBar.fillAmount * _loadDuration;

            while(time < _loadDuration)
            {
                time += Time.deltaTime;
                float t = time / _loadDuration;
                _processBar.fillAmount = t;
                yield return null;
            }

            _revived = true;
            _resurrectionActionText.text = NO_REVIVAL_MESSAGE;
            _processBar.fillAmount = 0f;
            // deduct cash
            GameManager.Instance.PlayersManager.AddCoins(-Constants.REVIVAL_COST);
            _coinsText.text = GameManager.Instance.PlayersManager.CoinsAmount.ToString();
            // subscribe so that the effect takes place only when the workshop is exitted
            GameManager.Instance.ShopsManager.WorkShopArea.OnInteractorExit += RevivePlayer;
        }

        public void RevivePlayer()
        {
            // revive player
            Vector3 respawnPosition = GameManager.Instance.ShopsManager.WorkShopArea.transform.position;
            int deadPlayerIndex = GameManager.Instance.PlayersManager.GetInactivePlayerIndex();
            GameManager.Instance.PlayersManager.RevivePlayer(deadPlayerIndex, respawnPosition);
            GameManager.Instance.ShopsManager.WorkShopArea.OnInteractorExit -= RevivePlayer;
        }

        private void StopResurrectionLoading(InputAction.CallbackContext _)
        {
            StopAllCoroutines();
            StartCoroutine(UnloadingProcess());
        }

        private IEnumerator UnloadingProcess()
        {
            while(_processBar.fillAmount > 0f)
            {
                _processBar.fillAmount -= Time.deltaTime;
                yield return null;
            }
        }

        private bool PlayerHasEnoughCoins()
        {
            return GameManager.Instance.PlayersManager.CoinsAmount >= Constants.REVIVAL_COST;
        }

        private bool IsPlayerDead()
        {
            return PlayersManager.PlayersCount == 2 && PlayersManager.ActivePlayersCount == 1 && !_revived;
        }
    }
}
