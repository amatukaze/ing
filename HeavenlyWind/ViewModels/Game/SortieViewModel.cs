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

        public SortieInfo Sortie { get; private set; }
        public bool IsSortieVisible { get; private set; }

        public PracticeInfo Practice { get; private set; }
        public bool IsPracticeVisible { get; private set; }

        internal SortieViewModel()
        {
            var rGame = KanColleGame.Current;

            var rPropertyChangedSource = Observable.FromEventPattern<PropertyChangedEventArgs>(rGame, nameof(rGame.PropertyChanged))
                .Select(r => r.EventArgs.PropertyName);
            rPropertyChangedSource.Where(r => r == nameof(rGame.Sortie))
                .Subscribe(_ =>
                {
                    var rInfo = KanColleGame.Current.Sortie;

                    Sortie = rInfo as SortieInfo;
                    Practice = rInfo as PracticeInfo;

                    IsPracticeVisible = Practice != null;
                    IsSortieVisible = Sortie != null && !IsPracticeVisible;

                    OnPropertyChanged(nameof(Sortie));
                    OnPropertyChanged(nameof(Practice));
                    OnPropertyChanged(nameof(IsSortieVisible));
                    OnPropertyChanged(nameof(IsPracticeVisible));
                });
        }
    }
}
