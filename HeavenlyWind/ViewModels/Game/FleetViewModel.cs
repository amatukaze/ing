using Sakuno.KanColle.Amatsukaze.Game.Models;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    public class FleetViewModel : ModelBase
    {
        public Fleet Source { get; }

        public FleetExpeditionStatusViewModel ExpeditionStatus { get; }
        public FleetConditionRegenerationViewModel ConditionRegeneration { get; }

        internal FleetViewModel(Fleet rpFleet)
        {
            Source = rpFleet;
            ExpeditionStatus = new FleetExpeditionStatusViewModel(rpFleet.ExpeditionStatus);
            ConditionRegeneration = new FleetConditionRegenerationViewModel(rpFleet.ConditionRegeneration);
        }
    }
}
