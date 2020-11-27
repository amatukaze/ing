namespace Sakuno.ING.Game.Models
{
    public partial class PlayerFleet
    {
        partial void UpdateCore(RawFleet raw)
        {
            ExpeditionState = raw.ExpeditionStatus.State;
            Expedition = _owner.MasterData.Expeditions[raw.ExpeditionStatus.ExpeditionId];
        }
    }
}
