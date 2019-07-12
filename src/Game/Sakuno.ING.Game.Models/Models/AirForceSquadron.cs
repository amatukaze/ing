using System;

namespace Sakuno.ING.Game.Models
{
    public partial class AirForceSquadron
    {
        partial void UpdateCore(RawAirForceSquadron raw, DateTimeOffset timeStamp) => Equipment = owner.AllEquipment[raw.EquipmentId];
    }
}
