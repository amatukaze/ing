namespace Sakuno.KanColle.Amatsukaze.Game.MasterData
{
    public abstract class ExpeditionInfo : IIdentifiable
    {
        public int Id { get; protected set; }
        public string DisplayId { get; protected set; }
        public string Name { get; protected set; }
        public string Description { get; protected set; }

        public MapAreaInfo Area { get; protected set; }

        public int Duration { get; protected set; }

        public int RequiredShipCount { get; protected set; }

        public double FuelConsumptionPercentage { get; protected set; }
        public double BulletConsumptionPercentage { get; protected set; }

        public bool IsCancellable { get; protected set; }
    }
}
