using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike
{
    using TankLike.UI;
    using TankLike.UnitControllers;

    public class InputManager : MonoBehaviour, IManager
    {
        private const string KEYBOARD_SCHEME = "Keyboard";
        private const string CONTROLLER_SCHEME = "Controller";
        private List<PlayerInput> _playerInputs = new List<PlayerInput>();
        public static PlayerInputActions Controls { get; private set; }
        [field: SerializeField] public GamePlayPlayerControlsStarter ControlsStarter { get; private set; }

        public bool IsActive { get; private set; }

        private InputIconsDatabase _inputIconsDatabase;

        public class ActionMapReference
        {
            public ActionMap MapTag;
            public int PlayerIndex;
            public InputActionMap Map;
        }

        private static List<ActionMapReference> _maps = new List<ActionMapReference>();

        public static InputActionMap GetMap(int player, ActionMap map)
        {
            if(_maps.Count == 0)
            {
                return null;
            }

            return _maps.Find(m => m.PlayerIndex == player && m.MapTag == map).Map;
        }

        public void SetReferences(InputIconsDatabase iconsDatabase)
        {
            _inputIconsDatabase = iconsDatabase;
        }

        #region IManager
        public void SetUp()
        {
            IsActive = true;

            // check if players have already joined through a lobby. If not, make them manually join
            if (!GameManager.Instance.PlayersTempInfoSaver.HasPlayer())
            {
                // create the players
                ControlsStarter.SetUp();
            }


            Controls = new PlayerInputActions();
            List<PlayerInputHandler> handlers = GameManager.Instance.PlayersTempInfoSaver.InputHandlers;
            FindObjectOfType<PlayerInputManager>().DisableJoining();
            _maps.Clear();

            for (int i = 0; i < handlers.Count; i++)
            {
                _playerInputs.Add(handlers[i].Playerinputs);

                // add the maps to the static list
                for (int j = 0; j < handlers[i].Playerinputs.actions.actionMaps.Count; j++)
                {
                    ActionMapReference map = new ActionMapReference()
                    {
                        MapTag = (ActionMap)j,
                        PlayerIndex = i,
                        Map = handlers[i].Playerinputs.actions.actionMaps[j]
                    };

                    _maps.Add(map);
                }
            }

            EnablePlayerInput();
        }

        public void Dispose()
        {
            IsActive = false;
        }
        #endregion

        public void EnablePlayerInput(int playerIndex = -1)
        {
            EnableInput(ActionMap.Player, playerIndex);
        }

        public void EnableUIInput(int playerIndex = -1)
        {
            EnableInput(ActionMap.UI, playerIndex);
        }

        public void EnableInput(ActionMap actionMap, int playerIndex = -1)
        {
            if (playerIndex == -1)
            {
                _playerInputs.ForEach(i => i.SwitchCurrentActionMap(actionMap.ToString()));
            }
            else
            {
                _playerInputs[playerIndex].SwitchCurrentActionMap(actionMap.ToString());
            }
        }

        public string GetButtonBindingKey(string action, int playerIndex)
        {
            // TODO: fix this shite
            //int schemeIndex = _playerInputs[playerIndex].currentControlScheme == KEYBOARD_SCHEME ? 0 : 1;
            return Controls.FindAction(action).GetBindingDisplayString(playerIndex);
        }

        public int GetButtonBindingIconIndex(string action, int playerIndex)
        {
            int schemeIndex = _playerInputs[playerIndex].currentControlScheme == KEYBOARD_SCHEME ? 0 : 1;
            string actionName = Controls.FindAction(action).name;
            return _inputIconsDatabase.GetSpriteIndexFromBinding(actionName, schemeIndex);
        }

        public string GetVectorBindingKey(string action, int playerIndex)
        {
            // scheme = 0, index = 0: 1. index = 1: 2
            // scheme = 1, index = 0: 2. index = 1: 4
            // 2 * 0 + 0 = 0
            int schemeIndex = _playerInputs[playerIndex].currentControlScheme == KEYBOARD_SCHEME ? 0 : 1;
            return Controls.FindAction(action).GetBindingDisplayString((2 * schemeIndex) + (playerIndex + 1));
        }

        public void DisableInputs(int playerIndex = -1)
        {
            // if the player index doesn't fall within the players' count
            if(playerIndex >= PlayersManager.PlayersCount)
            {
                return;
            }

            EnableInput(ActionMap.Empty, playerIndex);
        }

        public int GetSpriteIndexByScheme(string action, int controlScheme)
        {
            return _inputIconsDatabase.GetSpriteIndexFromBinding(action, controlScheme);
        }
    }

    public enum ActionMap
    {
        Player = 0, UI = 1, Lobby = 2, Inventory = 3, Empty = 4, EditorInput = 5
    }
}
