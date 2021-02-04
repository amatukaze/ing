namespace Sakuno.ING.Game.Models
{
    public partial class AirForceSquadron
    {
        partial void UpdateCore(RawAirForceSquadron raw) =>
            SlotItem = _owner.SlotItems[raw.SlotItemId];
    }
}
