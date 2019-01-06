using System.Collections.Generic;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events
{
    public readonly struct HomeportUpdate
    {
        public HomeportUpdate(IReadOnlyCollection<RawShip> ships, CombinedFleetType combinedFleetType)
        {
            Ships = ships;
            CombinedFleetType = combinedFleetType;
        }

        public IReadOnlyCollection<RawShip> Ships { get; }
        public CombinedFleetType CombinedFleetType { get; }
    }
}
