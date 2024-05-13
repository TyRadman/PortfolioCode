using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.UI
{
    public abstract class Navigatable : MonoBehaviour
    {
        // means that this navigatable is not controlling a child navigatable (set it to true if this navigatable opened another navigatable and set it to false when this navigatable gains control)
        public bool IsLastLayer = true;
        public bool IsActive = false;
        public bool Usable = true;
        public System.Action<int> CloseAction;
        [field: SerializeField] public int PlayerIndex { get; private set; } = -1;

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
            //if (!IsActive) return;

            //print($"Navigated in {gameObject.name}");
        }
        public virtual void Select()
        {
            if (!IsActive) return;

            //print($"Selected in {gameObject.name}");
        }
        public virtual void Return()
        {
            if (!IsActive) return;

            //print($"Returned in {gameObject.name}");
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
    }
}
