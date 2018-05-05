using System.Collections.Generic;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.Knowledge;

namespace Sakuno.ING.Game.Events
{
    public interface IHomeportUpdate
    {
        IReadOnlyCollection<IRawShip> Ships { get; }
        KnownCombinedFleet CombinedFleetType { get; }
    }
}
