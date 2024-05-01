using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsUI : MonoBehaviour
{
    public static PlayerStatsUI Instance;
    [Header("Health")]
    [SerializeField] private Image m_HealthBar;
    [SerializeField] private Animation m_RedDamageScreen;
    [Header("Stamina")]
    [SerializeField] private Image m_StaminaBar;
    [SerializeField] private Gradient m_StaminaTransitions;
    [Header("Batteries")]
    [SerializeField] private Image m_BatteryBar;
    [Header("Medicine")]
    [SerializeField] private Text m_MedicineText;

    private void Awake()
    {
        Instance = this;
    }

    // the amount must be between 0 and 1
    public void UpdateHealthBar(float _amount)
    {
        m_HealthBar.fillAmount = _amount;
    }

    public void TakeDamageRedScreen(bool _damageTaken)
    {
        if(!_damageTaken)
        {
            return;
        }

        m_RedDamageScreen.Play();
    }

    public void UpdateStaminaBar(float _amount)
    {
        m_StaminaBar.fillAmount = _amount;
        m_StaminaBar.color = m_StaminaTransitions.Evaluate(_amount);
    }

    public void UpdateMedicineText(string _text)
    {
        m_MedicineText.text = _text;
    }

}
