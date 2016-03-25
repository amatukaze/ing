using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Services;
using Sakuno.KanColle.Amatsukaze.ViewModels.Game;
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

        public bool IsBrowserAvailable { get; private set; } = true;

        public ICommand ShowPreferencesWindowCommand { get; } = new DelegatedCommand(() => new PreferencesWindow().ShowDialog());

        public ICommand ExpandMenuCommand { get; }

        public ICommand ShowExpeditionOverviewCommand { get; }

        public ICommand ShowConstructionHistoryCommand { get; }
        public ICommand ShowDevelopmentHistoryCommand { get; }
        public ICommand ShowSortieHistoryCommand { get; }
        public ICommand ShowExpeditionHistoryCommand { get; }
        public ICommand ShowScrappingHistoryCommand { get; }

        internal MainWindowViewModel()
        {
            var rBrowserServicePCEL = PropertyChangedEventListener.FromSource(BrowserService.Instance);
            rBrowserServicePCEL.Add(nameof(BrowserService.Instance.NoInstalledLayoutEngines), delegate
            {
                IsBrowserAvailable = false;
                OnPropertyChanged(nameof(IsBrowserAvailable));
            });

            var rGamePCEL = PropertyChangedEventListener.FromSource(KanColleGame.Current);
            rGamePCEL.Add(nameof(KanColleGame.Current.IsStarted), delegate
            {
                Content = new GameInformationViewModel();
                IsGameStarted = true;
            });

            ShowExpeditionOverviewCommand = new DelegatedCommand(() =>
            {
                var rGameInfo = Content as GameInformationViewModel;
                if (rGameInfo == null)
                    return;

                var rExpeditionOverview = new ExpeditionOverviewViewModel();
                rGameInfo.TabItems.Add(rExpeditionOverview);
                rGameInfo.SelectedItem = rExpeditionOverview;
            });

            ShowConstructionHistoryCommand = new DelegatedCommand(() => new ConstructionHistoryWindow().Show());
            ShowDevelopmentHistoryCommand = new DelegatedCommand(() => new DevelopmentHistoryWindow().Show());
            ShowSortieHistoryCommand = new DelegatedCommand(() => new SortieHistoryWindow().Show());
            ShowExpeditionHistoryCommand = new DelegatedCommand(() => new ExpeditionHistoryWindow().Show());
            ShowScrappingHistoryCommand = new DelegatedCommand(() => new ScrappingHistoryWindow().Show());
        }
    }
}
