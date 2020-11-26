namespace Sakuno.ING.Game.Models.MasterData
{
    public partial class ExpeditionInfo
    {
        partial void UpdateCore(RawExpeditionInfo raw)
        {
            MapArea = _owner.MapAreas[raw.MapAreaId];
        }
    }
}
