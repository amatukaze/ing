namespace Sakuno.ING.Game.Models
{
    partial class AirForceSquadron
    {
        partial void UpdateCore(IRawAirForceSquadron raw)
        {
            Equipment = owner.AllEquipment[raw.EquipmentId];
        }
    }
}
