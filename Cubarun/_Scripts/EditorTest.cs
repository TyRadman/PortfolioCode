using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorTest : MonoBehaviour
{
    [Range(0, 6)] public int PlayerIndex;

    public void ChangePlayerSelection()
    {
        PlayerPrefs.SetInt(Keys.SelectedCube, PlayerIndex);
    }
}
