using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Views.Game;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    [ViewInfo(typeof(FleetDetail))]
    public class FleetViewModel : ModelBase
    {
        public Fleet Source { get; }
        public string Name => Source.Name;

        public FleetExpeditionStatusViewModel ExpeditionStatus { get; }
        public FleetConditionRegenerationViewModel ConditionRegeneration { get; }

        internal FleetViewModel(Fleet rpFleet)
        {
            Source = rpFleet;
            ExpeditionStatus = new FleetExpeditionStatusViewModel(rpFleet);
            ConditionRegeneration = new FleetConditionRegenerationViewModel(rpFleet.ConditionRegeneration);
        }
    }
}
