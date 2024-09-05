using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.UI
{
    [CreateAssetMenu(fileName = "InputIcons_DB_Default", menuName = Directories.UI + "Input Icons DB")]
    public class InputIconsDatabase : ScriptableObject
    {
        [SerializeField] private InputActionAsset _controls;
        [SerializeField] private List<InputIconsData> _inputIcons = new List<InputIconsData>();

        public int GetSpriteIndexFromBinding(string action, int controlSchema)
        {
            foreach (var inputIcon in _inputIcons)
            {
                if(inputIcon.Action == action)
                {
                    if(controlSchema == 0)
                    {
                        return inputIcon.KeyboardSpriteIndex;
                    }
                    else
                    {
                        return inputIcon.ControllerSpriteIndex;
                    }
                }
            }

            Debug.LogError("No entry found for " + action);

            return -1;
        }

        //private void OnValidate()
        //{
        //    // Check if the inputActionAsset is assigned
        //    if (_controls != null)
        //    {
        //        // Step 2: Iterate through each action map
        //        foreach (var actionMap in _controls.actionMaps)
        //        {
        //            Debug.Log("Action Map: " + actionMap.name);

        //            // Step 3: Iterate through each action in the action map
        //            foreach (var action in actionMap.actions)
        //            {
        //                string bindingDisplayStrings = GetFormattedBindingDisplayStrings(action);
        //                Debug.Log("  Action: " + action.name + " Bindings: " + bindingDisplayStrings);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        Debug.LogError("Input Action Asset is not assigned.");
        //    }
        //}

        // Step 4: Method to get and format binding display strings
        string GetFormattedBindingDisplayStrings(InputAction action)
        {
            var bindingStrings = new System.Text.StringBuilder();

            // Iterate through each binding of the action
            for (int i = 0; i < action.bindings.Count; i++)
            {
                if (i > 0)
                {
                    bindingStrings.Append("|");
                }
                // Get the display string for the binding and remove whitespaces
                string displayString = action.GetBindingDisplayString(i);
                bindingStrings.Append(displayString);
            }

            return bindingStrings.ToString();
        }
    }
}
