using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Views.Preferences;
using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Sakuno.KanColle.Amatsukaze.ViewModels
{
    public class MainWindowViewModel : WindowViewModel
    {
        ModelBase r_Content;
        public ModelBase Content
        {
            get { return r_Content; }
            internal set
            {
                if (r_Content != value)
                {
                    r_Content = value;
                    OnPropertyChanged(nameof(Content));
                }
            }
        }

        public ICommand ShowPreferencesWindowCommand { get; } = new DelegatedCommand(() => new PreferencesWindow().ShowDialog());

        internal MainWindowViewModel()
        {
            Title = "Heavenly Wind";

            var rPropertyChangedSource = Observable.FromEventPattern<PropertyChangedEventArgs>(KanColleGame.Current, nameof(KanColleGame.Current.PropertyChanged))
                .Select(r => r.EventArgs.PropertyName);
            rPropertyChangedSource.Where(r => r == nameof(KanColleGame.Current.IsStarted)).Subscribe(_ => Content = new GameInformationViewModel());
        }
    }
}
