using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class MapAreaInfo : RawDataWrapper<RawMapAreaInfo>, IID, ITranslatedName
    {
        public int ID => RawData.ID;

        public string Name => RawData.Name;
        public string TranslatedName => StringResources.Instance.Extra?.GetAreaName(ID) ?? Name;

        public bool IsEventArea => RawData.IsEventArea;

        internal MapAreaInfo(RawMapAreaInfo rpRawData) : base(rpRawData) { }

        public override string ToString() => $"ID = {ID}, Name = \"{Name}\"";
    }
}
