namespace Sakuno.KanColle.Amatsukaze.Game.MasterData
{
    public abstract class EquipmentInfo : IIdentifiable
    {
        public int Id { get; protected set; }
        public string Name { get; protected set; }
        public string Description { get; protected set; }

        public EquipmentTypeInfo Type { get; protected set; }
        public int IconId { get; protected set; }

        public IBindableCollection<ShipInfo> ExtraSlotAcceptingShips { get; protected set; }
    }
}
