using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Services;
using Sakuno.KanColle.Amatsukaze.Views.History;
using Sakuno.KanColle.Amatsukaze.Views.Preferences;
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

        bool r_IsGameStarted;
        public bool IsGameStarted
        {
            get { return r_IsGameStarted; }
            private set
            {
                if (r_IsGameStarted != value)
                {
                    r_IsGameStarted = value;
                    OnPropertyChanged(nameof(IsGameStarted));
                }
            }
        }

        public UpdateService UpdateService => UpdateService.Instance;

        bool r_IsMenuExpanded;
        public bool IsMenuExpanded
        {
            get { return r_IsMenuExpanded; }
            private set
            {
                if (r_IsMenuExpanded != value)
                {
                    r_IsMenuExpanded = value;
                    OnPropertyChanged(nameof(IsMenuExpanded));
                }
            }
        }

        public ICommand ShowPreferencesWindowCommand { get; } = new DelegatedCommand(() => new PreferencesWindow().ShowDialog());

        public ICommand ExpandMenuCommand { get; }

        public ICommand ShowConstructionHistoryCommand { get; }
        public ICommand ShowDevelopmentHistoryCommand { get; }
        public ICommand ShowSortieHistoryCommand { get; }
        public ICommand ShowExpeditionHistoryCommand { get; }

        internal MainWindowViewModel()
        {
            var rGamePCEL = PropertyChangedEventListener.FromSource(KanColleGame.Current);
            rGamePCEL.Add(nameof(KanColleGame.Current.IsStarted), delegate
            {
                Content = new GameInformationViewModel();
                IsGameStarted = true;
            });

            ExpandMenuCommand = new DelegatedCommand(() => IsMenuExpanded = true);

            ShowConstructionHistoryCommand = new DelegatedCommand(delegate
            {
                IsMenuExpanded = false;
                new ConstructionHistoryWindow().Show();
            });
            ShowDevelopmentHistoryCommand = new DelegatedCommand(delegate
            {
                IsMenuExpanded = false;
                new DevelopmentHistoryWindow().Show();
            });
            ShowSortieHistoryCommand = new DelegatedCommand(delegate
            {
                IsMenuExpanded = false;
                new SortieHistoryWindow().Show();
            });
            ShowExpeditionHistoryCommand = new DelegatedCommand(delegate
            {
                IsMenuExpanded = false;
                new ExpeditionHistoryWindow().Show();
            });
        }
    }
}
