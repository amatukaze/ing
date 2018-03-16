namespace Sakuno.KanColle.Amatsukaze.Game.Models.MasterData
{
    public abstract class ShipInfo : IIdentifiable
    {
        public int Id { get; protected set; }
        public string Name { get; protected set; }
        public string Introduction { get; protected set; }

        public ShipTypeInfo Type { get; protected set; }

        public ShipSpeed Speed { get; protected set; }
        public bool IsLandBase => Speed == ShipSpeed.None;

        public int Rarity { get; protected set; }

        public int FuelConsumption { get; protected set; }
        public int BulletConsumption { get; protected set; }

        public IBindableCollection<EquipmentInfo> AvailableEquipmentInExtraSlot { get; protected set; }
    }
}
