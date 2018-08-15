using System;

namespace Sakuno.ING.Game.Models
{
    partial class AirForceSquadron
    {
        partial void UpdateCore(IRawAirForceSquadron raw, DateTimeOffset timeStamp)
        {
            Equipment = owner.AllEquipment[raw.EquipmentId];
        }
    }
}
