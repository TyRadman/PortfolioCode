using System.Collections;
using System.Collections.Generic;
using TankLike.Attributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.Combat.SkillTree
{
    public class SkillTreeInput : MonoBehaviour, IInput
    {
        [SerializeField, InSelf] private SkillTreeHolder _skillTreeHolder;

        public void SetUpInput(int playerIndex)
        {
            //Debug.Log("Setting up input for player " + playerIndex);
            PlayerInputActions c = InputManager.Controls;
            InputActionMap UIMap = InputManager.GetMap(playerIndex, ActionMap.UI);

            UIMap.FindAction(c.UI.Navigate_Up.name).started += NavigateUpStart;
            UIMap.FindAction(c.UI.Navigate_Down.name).started += NavigateDownStart;
            UIMap.FindAction(c.UI.Navigate_Right.name).started += NavigateRightStart;
            UIMap.FindAction(c.UI.Navigate_Left.name).started += NavigateLeftStart;

            UIMap.FindAction(c.UI.Navigate_Up.name).canceled += NavigateUpEnd;
            UIMap.FindAction(c.UI.Navigate_Down.name).canceled += NavigateDownEnd;
            UIMap.FindAction(c.UI.Navigate_Right.name).canceled += NavigateRightEnd;
            UIMap.FindAction(c.UI.Navigate_Left.name).canceled += NavigateLeftEnd;

            UIMap.FindAction(c.UI.Submit.name).started += StartSelection;
            UIMap.FindAction(c.UI.Submit.name).canceled += EndSelection;
        }

        public void DisposeInput(int playerIndex)
        {
            //Debug.Log("Disposing input for player " + playerIndex);
            PlayerInputActions c = InputManager.Controls;
            InputActionMap UIMap = InputManager.GetMap(playerIndex, ActionMap.UI);
            UIMap.FindAction(c.UI.Navigate_Up.name).started -= NavigateUpStart;
            UIMap.FindAction(c.UI.Navigate_Down.name).started -= NavigateDownStart;
            UIMap.FindAction(c.UI.Navigate_Right.name).started -= NavigateRightStart;
            UIMap.FindAction(c.UI.Navigate_Left.name).started -= NavigateLeftStart;

            UIMap.FindAction(c.UI.Navigate_Up.name).canceled -= NavigateUpEnd;
            UIMap.FindAction(c.UI.Navigate_Down.name).canceled -= NavigateDownEnd;
            UIMap.FindAction(c.UI.Navigate_Right.name).canceled -= NavigateRightEnd;
            UIMap.FindAction(c.UI.Navigate_Left.name).canceled -= NavigateLeftEnd;

            UIMap.FindAction(c.UI.Submit.name).started -= StartSelection;
            UIMap.FindAction(c.UI.Submit.name).canceled -= EndSelection;
        }

        private void StartSelection(InputAction.CallbackContext _)
        {
            _skillTreeHolder.StartSelection();
        }

        private void EndSelection(InputAction.CallbackContext _)
        {
            _skillTreeHolder.EndSelection();
        }

        public void NavigateLeftStart(InputAction.CallbackContext _)
        {
            _skillTreeHolder.NavigateByDirection(Direction.Left);
        }

        public void NavigateLeftEnd(InputAction.CallbackContext _)
        {
            _skillTreeHolder.StopNavigationByDirection(Direction.Left);
        }

        public void NavigateRightStart(InputAction.CallbackContext _)
        {
            _skillTreeHolder.NavigateByDirection(Direction.Right);
        }

        public void NavigateRightEnd(InputAction.CallbackContext _)
        {
            _skillTreeHolder.StopNavigationByDirection(Direction.Right);
        }

        public void NavigateUpStart(InputAction.CallbackContext _)
        {
            _skillTreeHolder.NavigateByDirection(Direction.Up);
        }

        public void NavigateUpEnd(InputAction.CallbackContext _)
        {
            _skillTreeHolder.StopNavigationByDirection(Direction.Up);
        }

        public void NavigateDownStart(InputAction.CallbackContext _)
        {
            _skillTreeHolder.NavigateByDirection(Direction.Down);
        }

        public void NavigateDownEnd(InputAction.CallbackContext _)
        {
            _skillTreeHolder.StopNavigationByDirection(Direction.Down);
        }
    }
}
