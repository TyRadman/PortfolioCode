public class Coin : Hittable
{
    protected override void Effect()
    {
        base.Effect();

        // play the collection effect
        EffectsPoolingManager.instance.SpawnEffect("Coin Collected", transform.position);
        // increase number of coins collected
        PlayerStats.Instance.AddCoin();
        // destroys the coin
        Destroy(gameObject);
    }
}
