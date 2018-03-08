namespace Sakuno.KanColle.Amatsukaze.Game.MasterData
{
    public abstract class ItemInfo : IIdentifiable
    {
        public int Id { get; protected set; }
        public string Name { get; protected set; }
        public string Description { get; protected set; }
    }
}
