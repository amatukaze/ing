using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    public class MaterialsViewModel : ModelBase
    {
        public Materials Source => KanColleGame.Current.Port.Materials;

        public MaterialsViewModel()
        {
            var rPort = KanColleGame.Current.Port;

            var rPropertyChangedSource = Observable.FromEventPattern<PropertyChangedEventArgs>(rPort, nameof(rPort.PropertyChanged))
                .Select(r => r.EventArgs.PropertyName);
            rPropertyChangedSource.Where(r => r == nameof(rPort.Materials))
                .Subscribe(_ => OnPropertyChanged(nameof(Source)));
        }
    }
}
