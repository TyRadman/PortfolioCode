public class Medicine : InteractionClass
{
    protected override void Interact()
    {
        base.Interact();
        ClearText();
        PlayerStats.Instance.AddMedicine();
        HUDManager.Instance.ShowMedicinePieces();
        Destroy(gameObject);
    }
}