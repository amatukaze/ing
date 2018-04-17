using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.MasterData
{
    partial class MapInfo
    {
        partial void UpdateCore(IRawMapInfo raw)
        {
            MapArea = mapAreaInfos[raw.MapAreaId];
            ItemAcquirements = raw.ItemAcquirements.Select(x => useItemInfos.TryGetOrDummy(x)).ToArray();
        }
    }
}
