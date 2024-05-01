using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ButtonClass
{
    public KeyTypes Name;
    public KeyCode Key;
    public bool Enabled = true;
    // an event for when the keys are pressed in different ways
    [HideInInspector] public UnityAction DownAction;    
    [HideInInspector] public UnityAction UpAction;
    [HideInInspector] public UnityAction PressAction;

    // setting up functions for each type of key press. A default value of null is given to keys press types that won't be used
    public void SetUpFunctions(UnityAction _down = null, UnityAction _up = null, UnityAction _press = null)
    {
        DownAction += _down;
        UpAction += _up;
        PressAction += _press;
    }

    public void Down()
    {
        if(DownAction != null)
        {
            DownAction();
        }
    }
    public void Up()
    {
        if (UpAction != null)
        {
            UpAction();
        }
    }
    public void Press()
    {
        if (PressAction != null)
        {
            PressAction();
        }
    }
}

public enum KeyTypes
{
    Inventory, Tips, Enter, ObjectivesToggle
}

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    public ButtonClass[] Buttons;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        checkForInput();
    }

    private void checkForInput()
    {
        for (int i = 0; i < Buttons.Length; i++)
        {
            if (Input.GetKeyDown(Buttons[i].Key))
            {
                Buttons[i].Down();
            }

            if (Input.GetKeyUp(Buttons[i].Key))
            {
                Buttons[i].Up();
            }

            if (Input.GetKey(Buttons[i].Key))
            {
                Buttons[i].Press();
            }
        }
    }

    // sets functions for the selected button
    public void SetUpButton(KeyTypes _type, UnityAction _down = null, UnityAction _up = null, UnityAction _press = null)
    {
        System.Array.Find(Buttons, b => b.Name == _type).SetUpFunctions(_down, _up, _press);
    }
}
