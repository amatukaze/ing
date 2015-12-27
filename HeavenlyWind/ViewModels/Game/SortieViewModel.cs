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
        public enum DisplayType { MapGauge, Sortie, Practice }

        public override string Name
        {
            get { return StringResources.Instance.Main.Tab_Sortie; }
            protected set { throw new NotImplementedException(); }
        }

        DisplayType r_Type;
        public DisplayType Type
        {
            get { return r_Type; }
            private set
            {
                r_Type = value;
                OnPropertyChanged(nameof(Type));
            }
        }

        public MapGaugesViewModel MapGauges { get; } = new MapGaugesViewModel();

        public SortieInfo Info { get; private set; }

        internal SortieViewModel()
        {
            var rGame = KanColleGame.Current;

            var rPropertyChangedSource = Observable.FromEventPattern<PropertyChangedEventArgs>(rGame, nameof(rGame.PropertyChanged))
                .Select(r => r.EventArgs.PropertyName);
            rPropertyChangedSource.Where(r => r == nameof(rGame.Sortie))
                .Subscribe(_ =>
                {
                    var rInfo = KanColleGame.Current.Sortie;
                    if (rInfo == null)
                        Type = DisplayType.MapGauge;
                    else
                    {
                        Info = rInfo;
                        Type = rInfo is PracticeInfo ? DisplayType.Practice : DisplayType.Sortie;
                        OnPropertyChanged(nameof(Info));
                    }
                });
        }
    }
}
