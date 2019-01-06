using System;

namespace Sakuno.ING.Game.Models.MasterData
{
    public partial class ExpeditionInfo
    {
        partial void UpdateCore(RawExpeditionInfo raw, DateTimeOffset timeStamp)
        {
            MapArea = owner.MapAreas[raw.MapAreaId];
        }
    }
}
