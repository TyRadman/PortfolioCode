using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings_01", menuName = Constants.MAIN_DIRECTORY + "Settings")]
public class GameSettings : ScriptableObject
{
    public DifficultyModifier[] DifficultyValues;
    public DifficultyModifier CurrentDifficulty;
}
