using Sakuno.KanColle.Amatsukaze.Game.Models;
using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    public class FleetViewModel : ModelBase
    {
        public Fleet Source { get; }

        public FleetExpeditionStatusViewModel ExpeditionStatus { get; }

        internal FleetViewModel(Fleet rpFleet)
        {
            Source = rpFleet;
            ExpeditionStatus = new FleetExpeditionStatusViewModel(rpFleet.ExpeditionStatus);
        }
    }
}
