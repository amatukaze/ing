using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Models;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    public class EventMapOverviewViewModel : ModelBase
    {
        public IList<EventMap> EventMaps { get; }

        public IList<FleetLocking> FleetLocking { get; }

        internal EventMapOverviewViewModel(MapMasterInfo[] rpEventMaps)
        {
            EventMaps = rpEventMaps.Select(r => new EventMap(r)).ToList();

            FleetLocking = ShipLockingService.Instance.ShipLocking.Select(r => new FleetLocking(r)).ToList();
        }
    }
}
