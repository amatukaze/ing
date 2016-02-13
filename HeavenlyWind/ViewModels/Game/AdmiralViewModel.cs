using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    public class AdmiralViewModel : ModelBase
    {
        public Admiral Source => KanColleGame.Current.Port.Admiral;

        public AdmiralViewModel()
        {
            var rPort = KanColleGame.Current.Port;

            PropertyChangedEventListener.FromSource(rPort).Add(nameof(rPort.Admiral), (s, e) => OnPropertyChanged(nameof(Source)));
        }
    }
}
