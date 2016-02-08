using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class MapAreaInfo : RawDataWrapper<RawMapAreaInfo>, IID
    {
        public int ID => RawData.ID;

        public string Name => RawData.Name;

        public bool IsEventArea => RawData.IsEventArea;

        internal MapAreaInfo(RawMapAreaInfo rpRawData) : base(rpRawData)
        {
            OnRawDataUpdated();
        }

        protected override void OnRawDataUpdated()
        {
            var rTranslatedName = StringResources.Instance.Extra?.GetAreaName(ID);
            if (rTranslatedName != null)
                RawData.Name = rTranslatedName;
        }

        public override string ToString() => $"ID = {ID}, Name = \"{Name}\"";
    }
}
