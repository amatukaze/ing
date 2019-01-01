using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class MapMasterInfo : RawDataWrapper<RawMapMasterInfo>, IID, ITranslatedName, IMapMasterInfo
    {
        public int ID => RawData.ID;

        public int AreaID => RawData.MapAreaID;
        public int AreaSubID => RawData.MapNo;

        public string Name => RawData.Name;
        public string TranslatedName => StringResources.Instance.Extra?.GetMapName(ID) ?? Name;

        public CombinedFleetType SortieFleetType => (CombinedFleetType)RawData.SortieFleetType[1];

        internal MapMasterInfo(RawMapMasterInfo rpRawData) : base(rpRawData) { }

        public override string ToString() => $"ID = {ID}, Name = \"{Name}\"";
    }
}
