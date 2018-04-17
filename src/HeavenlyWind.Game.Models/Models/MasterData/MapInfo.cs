using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.MasterData
{
    partial class MapInfo
    {
        partial void UpdateCore(IRawMapInfo raw)
        {
            MapArea = mapAreaInfos[raw.MapAreaId];
            itemAcquirements.Query = raw.ItemAcquirements.Select(useItemInfos.TryGetOrDummy);
        }
    }
}
