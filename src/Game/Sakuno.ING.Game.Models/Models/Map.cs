using System;

namespace Sakuno.ING.Game.Models
{
    public partial class Map
    {
        partial void UpdateCore(IRawMap raw, DateTimeOffset timeStamp)
        {
            Info = owner.MasterData.MapInfos[raw.Id];
        }
    }
}
