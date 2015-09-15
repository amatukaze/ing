using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    public class AdmiralViewModel : ModelBase
    {
        public Admiral Source => KanColleGame.Current.Port.Admiral;

        public AdmiralViewModel()
        {
            var rPort = KanColleGame.Current.Port;

            var rPropertyChangedSource = Observable.FromEventPattern<PropertyChangedEventArgs>(rPort, nameof(rPort.PropertyChanged))
                .Select(r => r.EventArgs.PropertyName);
            rPropertyChangedSource.Where(r => r == nameof(rPort.Admiral))
                .Subscribe(_ => OnPropertyChanged(nameof(Source)));
        }
    }
}
