using Sakuno.KanColle.Amatsukaze.ViewModels.Game;

namespace Sakuno.KanColle.Amatsukaze.ViewModels
{
    public class GameInformationViewModel : ModelBase
    {
        public OverviewViewModel Overview { get; } = new OverviewViewModel();
        public FleetsViewModel Fleets { get; }
        public QuestsViewModel Quests { get; } = new QuestsViewModel();

        internal GameInformationViewModel()
        {
            Fleets = new FleetsViewModel(this);
        }
    }
}
