using System.Collections.Generic;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Knowledge;

namespace Sakuno.KanColle.Amatsukaze.Game.Events
{
    public class HomeportUpdate
    {
        public IReadOnlyCollection<IRawFleet> Fleets { get; internal set; }
        public IReadOnlyCollection<IRawShip> Ships { get; internal set; }
        public KnownCombinedFleet CombinedFleetType { get; internal set; }
    }
}
