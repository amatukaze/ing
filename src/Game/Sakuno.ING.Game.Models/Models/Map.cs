using System;

namespace Sakuno.ING.Game.Models
{
    public partial class Map
    {
        partial void UpdateCore(RawMap raw, DateTimeOffset timeStamp)
        {
            Info = owner.MasterData.MapInfos[raw.Id];
            Gauge = raw.Gauge ??
                (raw.DefeatedCount, Info?.RequiredDefeatCount) switch
                {
                    (int d, int r) => (r - d, r),
                    (null, int r) => (0, r),
                    _ => raw.IsCleared ? (0, 1) : (1, 1)
                };
            IsInProgress = (!IsCleared) || (Gauge?.Current > 0);
        }
    }
}
