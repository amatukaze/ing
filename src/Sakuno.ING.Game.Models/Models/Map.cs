namespace Sakuno.ING.Game.Models
{
    partial class Map
    {
        partial void UpdateCore(IRawMap raw)
        {
            Info = mapInfoTable[raw.Id];
        }
    }
}
