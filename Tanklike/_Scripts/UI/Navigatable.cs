using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.UI
{
    using Signifiers;
    using TankLike.Sound;

    public abstract class Navigatable : MonoBehaviour, ISignifiersDisplayer
    {
        public bool IsActive = false;
        public System.Action OnOpened { get; set; } 
        public System.Action OnClosed { get; set; }
        public System.Action<int> CloseAction;
        public int PlayerIndex { get; private set; } = -1;
        public ISignifierController SignifierController { get; set; }

        

        public virtual void NavigateLeft(InputAction.CallbackContext _)
        {
        }
        public virtual void NavigateRight(InputAction.CallbackContext _)
        {
        }
        public virtual void NavigateUp(InputAction.CallbackContext _)
        {
        }
        public virtual void NavigateDown(InputAction.CallbackContext _)
        {
        }

        public virtual void Navigate(Direction direction)
        {        
        }
        public virtual void Select()
        {
        }
        public virtual void Return()
        {
        }

        public virtual void DehighlightCells()
        {

        }

        public virtual void MoveFromActiveCell()
        {

        }

        public virtual void Open(int playerIndex)
        {
            IsActive = true;
            SetPlayerIndex(playerIndex);
        }

        public virtual void Close(int playerIndex)
        {
            IsActive = false;
            CloseAction?.Invoke(playerIndex);
        }

        public virtual void SetActiveCellAsFirstCell()
        {

        }

        public virtual void SetUp()
        {
            IsActive = false;
            SetUpInput();
        }

        public virtual void Load(int playerIndex = 0)
        {

        }

        public virtual void SetUpInput()
        {

        }

        public void SetPlayerIndex(int index)
        {
            PlayerIndex = index;
        }

        #region Signifiers
        public virtual void SetUpActionSignifiers(ISignifierController signifierController)
        {

        }

        public virtual void SetUpSignifiers()
        {

        }
        #endregion
    }
}
