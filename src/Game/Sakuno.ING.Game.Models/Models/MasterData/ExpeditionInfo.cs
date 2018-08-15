using System;

namespace Sakuno.ING.Game.Models.MasterData
{
    partial class ExpeditionInfo
    {
        partial void UpdateCore(IRawExpeditionInfo raw, DateTimeOffset timeStamp)
        {
            MapArea = owner.MapAreas[raw.MapAreaId];
        }
    }
}
