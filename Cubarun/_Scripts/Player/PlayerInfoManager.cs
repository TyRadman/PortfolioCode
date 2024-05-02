using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoManager : MonoBehaviour
{
    public static PlayerInfoManager Instance;
    public static int playerSelectedIndex = 0;
    [SerializeField] private MeshRenderer playerMesh;
    [SerializeField] private CubeID[] playerMaterials;

    [System.Serializable]
    public class CubeID
    {
        public int Index;
        public Material CubeMaterial;
        public bool HasPowerUp = true;
        public Color PowerUpColor;
        public bool HasUniqueTrailMaterial = false;
        public Material TrailMaterial;
    }


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        playerSelectedIndex = PlayerPrefs.GetInt(Keys.SelectedCube);
        SetPlayerUp();
    }

    private void SetPlayerUp()
    {
        playerMesh.material = playerMaterials[playerSelectedIndex].CubeMaterial;
        print("Player set " + playerSelectedIndex);
        PlayerEffects.Instance.SetColor(playerMesh.material.color);
        PowerUpsManager.Instance.SetPowerUpButtonColor(playerMaterials[playerSelectedIndex].PowerUpColor, playerMaterials[playerSelectedIndex].HasPowerUp);
    }

    public void SetMaterial(Material newMaterial, float duration)
    {
        playerMesh.material = newMaterial;
        Invoke(nameof(RestoreOriginalMaterial), duration);
    }

    public void RestoreOriginalMaterial()
    {
        playerMesh.material = playerMaterials[playerSelectedIndex].CubeMaterial;
    }

    public Material GetTrailMaterial()
    {
        if (playerMaterials[playerSelectedIndex].HasUniqueTrailMaterial)
        {
            return playerMaterials[playerSelectedIndex].TrailMaterial;
        }

        return null;
    }
}