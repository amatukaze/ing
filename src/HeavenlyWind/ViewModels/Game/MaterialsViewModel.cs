using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    class MaterialsViewModel : ModelBase
    {
        public Materials Source => KanColleGame.Current.Port.Materials;

        public MaterialsViewModel()
        {
            var rPort = KanColleGame.Current.Port;

            PropertyChangedEventListener.FromSource(rPort).Add(nameof(rPort.Materials), (s, e) => OnPropertyChanged(nameof(Source)));
        }
    }
}
