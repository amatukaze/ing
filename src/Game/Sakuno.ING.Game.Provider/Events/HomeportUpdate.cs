using System.Collections.Generic;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events
{
    public readonly struct HomeportUpdate
    {
        public HomeportUpdate(IReadOnlyCollection<IRawShip> ships, CombinedFleetType combinedFleetType)
        {
            Ships = ships;
            CombinedFleetType = combinedFleetType;
        }

        public IReadOnlyCollection<IRawShip> Ships { get; }
        public CombinedFleetType CombinedFleetType { get; }
    }
}
