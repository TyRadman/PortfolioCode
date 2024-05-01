using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : MonoBehaviour
{
    public static PlayerLife Instance;
    public float MaxHealth;
    private float m_CurrentHealth;
    private BloodSplash m_HUDClass;
    [SerializeField] private float m_RefillLimit = 0.4f;
    [SerializeField] private float m_TimeBeforeRefill = 5f;
    [SerializeField] private float m_TimeToFullyRefill = 10f;
    private Coroutine m_RefillCoroutine;
    private WaitForSeconds m_RefillWaiting;

    private void Awake()
    {
        Instance = this;
        m_CurrentHealth = MaxHealth;
        m_HUDClass = FindObjectOfType<BloodSplash>();
        m_HUDClass.UpdateBloodSplash(1f);
        m_HUDClass.SetCriticalLifePoint(m_RefillLimit);
        m_RefillWaiting = new WaitForSeconds(m_TimeBeforeRefill);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            AddLife(-10f);
        }
    }

    public void AddLife(float _amount)
    {
        HUDManager.Instance.ToggleBars(true);
        m_CurrentHealth += _amount;
        
        if(m_CurrentHealth < 0f)
        {
            m_CurrentHealth = 0f;
            // game over
        }

        // if health is to be decreased then we stop any refilling and start over
        if(_amount < 0f)
        {
            if (m_RefillCoroutine != null)
            {
                StopCoroutine(m_RefillCoroutine);
                m_RefillCoroutine = null;
            }

            // if the health is lower than the refill limit and this function was called to decrease the player's health then we wait and refill
            if (LifePercentage() < m_RefillLimit)
            {
                m_RefillCoroutine = StartCoroutine(refillHealth());
            }
        }

        // update UI
        m_HUDClass.UpdateBloodSplash(LifePercentage());
        PlayerStatsUI.Instance.UpdateHealthBar(LifePercentage());
        PlayerStatsUI.Instance.TakeDamageRedScreen(_amount < 0);
    }

    IEnumerator refillHealth()
    {
        // wait for a bit before refilling
        yield return m_RefillWaiting;
        float time = 0f;
        // we calculate the amount of health the refill limit represents
        float criticalHealthAmount = MaxHealth * m_RefillLimit;

        // as long as the health is below the refill limit, the health will be generated
        while(time < m_TimeToFullyRefill && LifePercentage() < m_RefillLimit)
        {
            time += Time.deltaTime;
            AddLife(criticalHealthAmount * (Time.deltaTime / m_TimeToFullyRefill));
            yield return null;
        }

        HUDManager.Instance.ToggleBars(false);
    }

    public bool HealthIsAboveRefill()
    {
        return LifePercentage() > m_RefillLimit;
    }

    public bool HealthIsFull()
    {
        return m_CurrentHealth == MaxHealth;
    }

    public float LifePercentage()
    {
        return m_CurrentHealth / MaxHealth;
    }

    public float GetLifeByPercentage(float _perc)
    {
        return MaxHealth * _perc;
    }
}
