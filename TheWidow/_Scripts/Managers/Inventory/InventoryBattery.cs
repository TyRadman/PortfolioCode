using UnityEngine;

public class InventoryBattery : InventoryInteractable
{
    [SerializeField] private float m_BatteryValue;

    private void Awake()
    {
        m_BatteryValue = GeneralValues.BatteryValue();
    }

    public override void OnUsed()
    {
        // check if the process is possible
        if (!PlayerActions.Instance.HasFlashLight)
        {
            Inventory.Instance.ErrorMessage(m_ErrorMessages[0]);
            return;
        }
        
        if(PlayerStats.Instance.BatteryIsFull())
        {
            Inventory.Instance.ErrorMessage(m_ErrorMessages[1]);
            return;
        }

        PlayerStats.Instance.AddBattery(m_BatteryValue);

        base.OnUsed();
    }
}