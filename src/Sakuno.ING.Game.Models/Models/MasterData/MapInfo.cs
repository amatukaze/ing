using System.Linq;

namespace Sakuno.ING.Game.Models.MasterData
{
    partial class MapInfo
    {
        partial void UpdateCore(IRawMapInfo raw)
        {
            MapArea = owner.MapAreas[raw.MapAreaId];
            itemAcquirements.Query = raw.ItemAcquirements.Select(owner.UseItems.TryGetOrDummy);
        }
    }
}
