namespace Sakuno.KanColle.Amatsukaze.Game.MasterData
{
    public abstract class MapAreaInfo : IIdentifiable
    {
        public int Id { get; protected set; }
        public string Name { get; protected set; }
        public string OperationName { get; protected set; }
        public string Description { get; protected set; }

        public bool IsEventArea { get; protected set; }
    }
}
