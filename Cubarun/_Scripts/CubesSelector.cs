using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubesSelector : MonoBehaviour
{
    public enum cubesTypes
    {
        RedCube, BlueCube, PurpleCube
    }

    public const string Seperator = "---";
    private const string lockedKey = "Locked";
    private const string unlockedKey = "Unlocked";
    public static CubesSelector Instance;
    [SerializeField] private CubeInfo[] cubes;

    private void Awake()
    {
        Instance = this;

        if (PlayerPrefs.GetString(Keys.GameReset) == "")
        {
            ResetCubeData();
        }
        else
        {
            loadCubesData();
        }
    }

    public CubeInfo GetCube(int index)
    {
        return cubes[index];
    }

    public int GetMaxCubes()
    {
        return cubes.Length;
    }

    public void ResetCubeData()
    {
            PlayerPrefs.SetString(Keys.GameReset, "InUse");

        for (int i = 1; i < cubes.Length; i++)
        {
            cubes[i].locked = true;
            var state = cubes[i].locked == true ? lockedKey : unlockedKey;
            PlayerPrefs.SetString(cubes[i].CubeName, state);
        }
    }

    private void loadCubesData()
    {
        for (int i = 0; i < cubes.Length; i++)
        {
            cubes[i].locked = PlayerPrefs.GetString(cubes[i].CubeName) == lockedKey;
        }
    }

    public void UnlockCube(CubeInfo cube)
    {
        cube.locked = false;
        PlayerPrefs.SetString(cube.CubeName, unlockedKey);
    }
}

[System.Serializable]
public class CubeInfo
{
    [InspectorName("Name")] public string CubeName;
    [TextArea(3,20)]
    public string CubeDescription;
    public int price;
    public bool locked = true;
    public Material Material;
}
