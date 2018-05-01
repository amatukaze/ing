using System.Collections.Generic;

namespace Sakuno.ING.Game.Events.Shipyard
{
    public class EquipmentDismantling
    {
        public IReadOnlyCollection<int> EquipmentIds { get; internal set; }
    }
}
