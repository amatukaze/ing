using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class ShipType : RawDataWrapper<RawShipType>, IID, ITranslatedName
    {
        public static ShipType Dummy { get; } = new ShipType(new RawShipType() { ID = -1, Name = "?" });

        public int ID => RawData.ID;

        public int SortNumber => RawData.SortNumber;

        public string Name => RawData.Name;
        public string TranslatedName => StringResources.Instance.Extra?.GetShipTypeName(ID) ?? Name;

        internal ShipType(RawShipType rpRawData) : base(rpRawData) { }

        public override string ToString() => $"ID = {ID}, Name = \"{Name}\"";
    }
}
