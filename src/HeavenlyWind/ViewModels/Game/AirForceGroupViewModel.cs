using Sakuno.KanColle.Amatsukaze.Game.Models;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    class AirForceGroupViewModel : ModelBase
    {
        public AirForceGroup Source { get; }

        internal AirForceGroupViewModel(AirForceGroup rpGroup)
        {
            Source = rpGroup;
        }
    }
}
