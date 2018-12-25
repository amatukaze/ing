using System.Collections.Generic;

namespace Sakuno.ING.Game.Models.Battle
{
    public interface IRawLandBaseAerialPhase : IRawAerialPhase
    {
        IReadOnlyList<EquipmentRecord> Squadrons { get; }
    }
}
