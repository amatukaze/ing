using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class ShipType : RawDataWrapper<RawShipType>, IID
    {
        public static ShipType Dummy { get; } = new ShipType(new RawShipType() { ID = -1, Name = "?" });

        public int ID => RawData.ID;

        public int SortNumber => RawData.SortNumber;

        public string Name => RawData.Name;

        internal ShipType(RawShipType rpRawData) : base(rpRawData)
        {
            OnRawDataUpdated();
        }

        protected override void OnRawDataUpdated()
        {
            var rTranslatedName = StringResources.Instance.Extra?.GetShipTypeName(ID);
            if (rTranslatedName != null)
                RawData.Name = rTranslatedName;
        }

        public override string ToString() => $"ID = {ID}, Name = \"{Name}\"";
    }
}
