using System.Linq;

namespace Sakuno.ING.Game.Models.MasterData
{
    partial class MapInfo
    {
        partial void UpdateCore(IRawMapInfo raw)
        {
            MapArea = mapAreaInfoTable[raw.MapAreaId];
            itemAcquirements.Query = raw.ItemAcquirements.Select(useItemInfoTable.TryGetOrDummy);
        }
    }
}
