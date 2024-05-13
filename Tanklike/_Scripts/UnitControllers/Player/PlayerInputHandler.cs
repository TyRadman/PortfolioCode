using UnityEngine;
using UnityEngine.InputSystem;
namespace TankLike.UnitControllers
{
    public class PlayerInputHandler : MonoBehaviour
    {
        public PlayerInputActions Controls { set; get; }
        [field: SerializeField] public PlayerInput Playerinputs { set; get; }
        private static int ConfirmationNumber = 0;
        private static int MaxConfirmationNumber = 0;
        private bool _confirmed = false;
        [SerializeField] private PlayerTempInfoSaver _playersInfoSaver;
        private InputActionMap _lobbyActionMap;

        public void SetUp()
        {
            Controls = new PlayerInputActions();

            _lobbyActionMap = Playerinputs.actions.FindActionMap(Controls.Lobby.Get().name);
            _lobbyActionMap.FindAction(Controls.Lobby.Join.name).performed += ConfirmSelection;
            _lobbyActionMap.FindAction(Controls.Lobby.Return.name).performed += ReturnButton;
            MaxConfirmationNumber++;

            if (MaxConfirmationNumber == 2)
            {
                FindObjectOfType<PlayerInputManager>().DisableJoining();
            }

            _playersInfoSaver.AddPlayerInputHandler(this);
        }

        public void ReturnButton(InputAction.CallbackContext _)
        {
            if (_confirmed)
            {
                Deconfirm();
            }
            else
            {
                Disconnect();
            }
        }

        public void Disconnect()
        {
            if (MaxConfirmationNumber < 2)
            {
                FindObjectOfType<PlayerInputManager>().EnableJoining();
            }

            _playersInfoSaver.RemovePlayerInputHandler(this);
            MaxConfirmationNumber--;
            Destroy(gameObject);
        }

        private void Deconfirm()
        {
            ConfirmationNumber--;
            FindObjectOfType<PlayerJoinManager>().EnableConfirmedButton(Playerinputs.playerIndex, false);
            _confirmed = false;
        }

        public void ConfirmSelection(InputAction.CallbackContext _)
        {
            print($"Confirmed at {Time.time}");

            if (_confirmed)
            {
                return;
            }

            ConfirmationNumber++;
            FindObjectOfType<PlayerJoinManager>().EnableConfirmedButton(Playerinputs.playerIndex, true);
            _confirmed = true;
            print($"Confirmation: {ConfirmationNumber}. Max: {MaxConfirmationNumber}");

            if (ConfirmationNumber == MaxConfirmationNumber)
            {
                _playersInfoSaver.DisableAllInputs();
                FindObjectOfType<PlayerJoinManager>().LoadGameplayScene();
            }
        }

        public void Join()
        {
            print($"Joined at {Time.time}");
        }

        private void OnDestroy()
        {
            if(_lobbyActionMap == null)
            {
                return;
            }

            _lobbyActionMap.FindAction("Join").performed -= ConfirmSelection;
            _lobbyActionMap.FindAction("Return").performed -= ReturnButton;
        }
    }
}

// Reference: https://forum.unity.com/threads/manual-local-multiplayer-using-inputdevices.1295664/
// Alternative approach
// private InputActionMap _lobbyActionMap;
// guaranteed solution, but harder
//_lobbyActionMap = _input.actions.FindActionMap("Lobby");
//_lobbyActionMap.FindAction("Return").performed += Disconnect;