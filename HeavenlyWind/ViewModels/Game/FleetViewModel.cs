using Sakuno.KanColle.Amatsukaze.Game.Models;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    public class FleetViewModel : ModelBase
    {
        public Fleet Source { get; }

        internal FleetViewModel(Fleet rpFleet)
        {
            Source = rpFleet;
        }
    }
}
