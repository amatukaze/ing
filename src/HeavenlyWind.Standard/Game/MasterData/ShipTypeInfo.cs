using Sakuno.KanColle.Amatsukaze.Game.Knowledge;

namespace Sakuno.KanColle.Amatsukaze.Game.MasterData
{
    public abstract class ShipTypeInfo : IIdentifiable
    {
        public int Id { get; protected set; }
        public string Name { get; protected set; }

        public static implicit operator KnownShipType(ShipTypeInfo shipType) => (KnownShipType)shipType.Id;
    }
}
