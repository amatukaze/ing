using Sakuno.KanColle.Amatsukaze.ViewModels.Game;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Contents
{
    public class GameInformationViewModel : ModelBase
    {
        public FleetsViewModel Fleets { get; } = new FleetsViewModel();
        public QuestsViewModel Quests { get; } = new QuestsViewModel();
    }
}
