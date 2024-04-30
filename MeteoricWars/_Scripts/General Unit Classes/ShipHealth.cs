using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ShipHealth : MonoBehaviour, IDamagable
{
    [SerializeField] protected float CurrentHealth;
    [SerializeField] protected float MaxHealth;
    [SerializeField] protected bool CanBeShot = true;
    public UnityAction OnDeathAction;

    protected virtual void Awake()
    {
        SetCurrentHealthToMaxHealth();
    }

    public virtual void TakeDamage(float _amount, int _playerIndex)
    {
        if (!CanBeShot)
        {
            return;
        }

        CurrentHealth += _amount;

        if (CurrentHealth <= 0f)
        {
            CurrentHealth = 0;
            // death 
        }

        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
    }

    public virtual void SetCurrentHealthToMaxHealth()
    {
        CurrentHealth = MaxHealth;
        CanBeShot = true;
    }

    public virtual void SetMaxHealth(float _maxHealth)
    {
        MaxHealth = (int)_maxHealth;
    }

    public float GetHealth()
    {
        return CurrentHealth;
    }

    public float GetHealthT()
    {
        return CurrentHealth / MaxHealth;
    }

    public float GetMaxHealth()
    {
        return MaxHealth;
    }
}
