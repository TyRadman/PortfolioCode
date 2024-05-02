using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempManager : MonoBehaviour
{
    [SerializeField] private int playerSelection = 0;

    private void Awake()
    {
        PlayerPrefs.SetInt("selectedCube", playerSelection);
    }
}
