using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class MapAreaInfo : RawDataWrapper<RawMapAreaInfo>, IID
    {
        public int ID => RawData.ID;

        public string Name => RawData.Name;

        public bool IsEventMap => RawData.IsEventMap;

        internal MapAreaInfo(RawMapAreaInfo rpRawData) : base(rpRawData) { }

        public override string ToString() => $"ID = {ID}, Name = \"{Name}\"";
    }
}
