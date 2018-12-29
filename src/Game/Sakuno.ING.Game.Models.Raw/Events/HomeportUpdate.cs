using System.Collections.Generic;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.Knowledge;

namespace Sakuno.ING.Game.Events
{
    public readonly struct HomeportUpdate
    {
        public HomeportUpdate(IReadOnlyCollection<IRawShip> ships, KnownCombinedFleet combinedFleetType)
        {
            Ships = ships;
            CombinedFleetType = combinedFleetType;
        }

        public IReadOnlyCollection<IRawShip> Ships { get; }
        public KnownCombinedFleet CombinedFleetType { get; }
    }
}
