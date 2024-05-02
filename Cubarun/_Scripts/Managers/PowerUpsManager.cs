using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpsManager : MonoBehaviour
{
    public static PowerUpsManager Instance;
    private List<PowerUp> powerUps = new List<PowerUp>();
    private PowerUp selectedPowerUp;
    private int selectedCubeIndex;
    [SerializeField] private Image powerUpButtonBar;
    [SerializeField] private Animator anim;

    private void Awake()
    {
        Instance = this;
        print(PlayerPrefs.GetInt(Keys.SelectedCube));
        selectedCubeIndex = PlayerPrefs.GetInt(Keys.SelectedCube);
    }

    public void ActivatePowerUp()
    {
        try {
            selectedPowerUp.Activate();
        }
        catch
        {

        }
     }

    public void AddPowerUp(PowerUp powerUp)
    {
        powerUps.Add(powerUp);
        powerUp.enabled = false;

        if (powerUp.GetIndex() == selectedCubeIndex)
        {
            powerUp.SetUIBar(powerUpButtonBar, anim);
            powerUp.enabled = true;
            selectedPowerUp = powerUp;
        }
    }

    public void FullyChargeActivePowerUp()
    {
        if(selectedPowerUp != null)
            selectedPowerUp.FullyCharge();
    }

    public void SetPowerUpButtonColor(Color newColor, bool hasPowerUp)
    {
        powerUpButtonBar.color = newColor;
        
        if (!hasPowerUp)
        {
            powerUpButtonBar.transform.parent.gameObject.SetActive(false);
        }
    }
}
