using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    public class SortieViewModel : TabItemViewModel
    {
        public override string Name
        {
            get { return StringResources.Instance.Main.Tab_Sortie; }
            protected set { throw new NotImplementedException(); }
        }

        public SortieInfo Model => KanColleGame.Current.Sortie;

        internal SortieViewModel()
        {
            var rGame = KanColleGame.Current;

            var rPropertyChangedSource = Observable.FromEventPattern<PropertyChangedEventArgs>(rGame, nameof(rGame.PropertyChanged))
                .Select(r => r.EventArgs.PropertyName);
            rPropertyChangedSource.Where(r => r == nameof(rGame.Sortie))
                .Subscribe(_ => OnPropertyChanged(nameof(Model)));
        }
    }
}
