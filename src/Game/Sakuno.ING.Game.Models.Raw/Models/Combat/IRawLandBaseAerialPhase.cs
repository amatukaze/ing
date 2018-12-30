using System.Collections.Generic;

namespace Sakuno.ING.Game.Models.Combat
{
    public interface IRawLandBaseAerialPhase : IRawAerialPhase
    {
        IReadOnlyList<EquipmentRecord> Squadrons { get; }
    }
}
