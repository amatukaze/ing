namespace Sakuno.ING.Game.Models
{
    partial class Map
    {
        partial void UpdateCore(IRawMap raw)
        {
            Info = owner.MasterData.MapInfos[raw.Id];
        }
    }
}
