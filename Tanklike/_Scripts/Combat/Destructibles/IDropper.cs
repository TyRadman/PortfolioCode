namespace TankLike.Combat.Destructible
{
    using Environment.LevelGeneration;

    public interface IDropper
    {
        DropperTag DropperTag { get; }
        void SetCollectablesToSpawn(DestructibleDrop collectables);
    }

    public enum DropperTag
    {
        Crate = 0,
        Stone = 1,
        None = 1000,
    }
}
