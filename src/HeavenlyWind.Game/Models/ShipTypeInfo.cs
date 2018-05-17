using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class ShipTypeInfo : RawDataWrapper<RawShipType>, IID, ITranslatedName
    {
        public static ShipTypeInfo Dummy { get; } = new ShipTypeInfo(new RawShipType() { ID = -1, Name = "?" });

        public int ID => RawData.ID;

        public int SortNumber => RawData.SortNumber;

        public string Name => RawData.Name;
        public string TranslatedName => StringResources.Instance.Extra?.GetShipTypeName(ID) ?? Name;

        internal ShipTypeInfo(RawShipType rpRawData) : base(rpRawData) { }

        public override string ToString() => $"ID = {ID}, Name = \"{Name}\"";
    }
}
