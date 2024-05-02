using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class selectCube : MonoBehaviour
{
    private string selectedCube_p = "selectedCube";
    private int selectedCubeIndex;

    // list of cubes available
    public GameObject[] cubes;
    public GameObject cube; // the nne to be displayed

    private void Start()
    {
        // displays the selected one
        cubes[PlayerPrefs.GetInt(selectedCube_p)].SetActive(true);
    }

    public void nextButton()
    {
        int oldIndex, newIndex;
        oldIndex = PlayerPrefs.GetInt(selectedCube_p);

        if (PlayerPrefs.GetInt(selectedCube_p) < cubes.Length - 1)
        {
            PlayerPrefs.SetInt(selectedCube_p, PlayerPrefs.GetInt(selectedCube_p) + 1);
        }
        else
        {
            PlayerPrefs.SetInt(selectedCube_p, 0);
        }

        newIndex = PlayerPrefs.GetInt(selectedCube_p);

        switchCube(oldIndex,newIndex);
    }



    public void previousButton()
    {
        int oldIndex, newIndex;
        oldIndex = PlayerPrefs.GetInt(selectedCube_p);

        if (PlayerPrefs.GetInt(selectedCube_p) > 0)
        {
            PlayerPrefs.SetInt(selectedCube_p, PlayerPrefs.GetInt(selectedCube_p) - 1);
        }
        else
        {
            PlayerPrefs.SetInt(selectedCube_p, cubes.Length - 1);
        }

        newIndex = PlayerPrefs.GetInt(selectedCube_p);

        switchCube(oldIndex, newIndex);
    }



    void switchCube(int oldIndex, int newIndex)
    {
        cubes[oldIndex].SetActive(false);
        cubes[newIndex].SetActive(true);
    }
}
