using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryHealthPotion : InventoryInteractable
{
    private float m_AddedHealth;

    private void Awake()
    {
        m_AddedHealth = Random.Range(0.2f, 0.3f) * PlayerLife.Instance.GetLifeByPercentage(1f);
    }

    public override void OnUsed()
    {
        // if the player health is full then we throw an error message and return
        if (PlayerLife.Instance.HealthIsFull())
        {
            Inventory.Instance.ErrorMessage(m_ErrorMessages[0]);
            return;
        }

        // otherwise we add the health
        PlayerLife.Instance.AddLife(m_AddedHealth);
        base.OnUsed();
    }
}
