using Sakuno.KanColle.Amatsukaze.ViewModels.Game;

namespace Sakuno.KanColle.Amatsukaze.ViewModels
{
    public class GameInformationViewModel : ModelBase
    {
        public OverviewViewModel Overview { get; } = new OverviewViewModel();
        public FleetsViewModel Fleets { get; } = new FleetsViewModel();
        public QuestsViewModel Quests { get; } = new QuestsViewModel();
    }
}
