using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class MapMasterInfo : RawDataWrapper<RawMapMasterInfo>, IID, IMapMasterInfo
    {
        public int ID => RawData.ID;

        public int AreaID => RawData.MapAreaID;
        public int AreaSubID => RawData.MapNo;

        public string Name => RawData.Name;

        public int? RequiredDefeatCount => RawData.RequiredDefeatCount;

        internal MapMasterInfo(RawMapMasterInfo rpRawData) : base(rpRawData)
        {
            OnRawDataUpdated();
        }

        protected override void OnRawDataUpdated()
        {
            var rTranslatedName = StringResources.Instance.Extra?.GetMapName(ID);
            if (rTranslatedName != null)
                RawData.Name = rTranslatedName;
        }

        public override string ToString() => $"ID = {ID}, Name = \"{Name}\"";
    }
}
