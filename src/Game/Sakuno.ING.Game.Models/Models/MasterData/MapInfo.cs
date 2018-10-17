using System;
using System.Linq;

namespace Sakuno.ING.Game.Models.MasterData
{
    public partial class MapInfo
    {
        partial void UpdateCore(IRawMapInfo raw, DateTimeOffset timeStamp)
        {
            MapArea = owner.MapAreas[raw.MapAreaId];
            itemAcquirements.Query = raw.ItemAcquirements.Select(x => owner.UseItems[x]);
        }
    }
}
