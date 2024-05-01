public class InventoryEnergy : InventoryInteractable
{
    private float m_AddedStamina;

    public void Start()
    {
        m_AddedStamina = PlayerStats.Instance.GetMaxStamina() * UnityEngine.Random.Range(0.2f, 0.5f);
    }

    public override void OnUsed()
    {
        PlayerStats.Instance.AddStamina(m_AddedStamina);
        base.OnUsed();
    }
}