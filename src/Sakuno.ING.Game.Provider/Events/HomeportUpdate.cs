using System.Collections.Generic;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.Knowledge;

namespace Sakuno.ING.Game.Events
{
    public class HomeportUpdate
    {
        public IReadOnlyCollection<IRawFleet> Fleets { get; internal set; }
        public IReadOnlyCollection<IRawShip> Ships { get; internal set; }
        public KnownCombinedFleet CombinedFleetType { get; internal set; }
    }
}
