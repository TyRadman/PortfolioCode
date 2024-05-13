using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace TankLike.UI.PauseMenu
{
    public class MenuSelectable : MonoBehaviour
    {
        [SerializeField] private SelectableAction _selectionAction;
        [SerializeField] private List<SelectableAction> _actions = new List<SelectableAction>()
        {
            new SelectableAction(){Direction = Direction.Up, Name = Direction.Up.ToString()},
            new SelectableAction(){Direction = Direction.Down, Name = Direction.Down.ToString()},
            new SelectableAction(){Direction = Direction.Left, Name = Direction.Left.ToString()},
            new SelectableAction(){Direction = Direction.Right, Name = Direction.Right.ToString()},
        };

        [Header("References")]
        [SerializeField] private GameObject _highlight;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Color _normalColor;
        [SerializeField] private Color _highlightedColor;

        private void Awake()
        {
            _text.color = _normalColor;
        }

        public void Highlight(bool highlight)
        {
            //_highlight.SetActive(highlight);
            _text.color = highlight? _highlightedColor : _normalColor;
        }

        public void InvokeAction(Direction direction = Direction.None)
        {
            if(direction == Direction.None)
            {
                _selectionAction.Action.Invoke();
            }
            else
            {
                _actions.Find(a => a.Direction == direction).Action.Invoke();
            }
        }
    }
}
