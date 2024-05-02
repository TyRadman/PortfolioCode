using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsResetter : MonoBehaviour
{
    public void ResetValues()
    {
        PlayerPrefs.DeleteAll();
    }
}
