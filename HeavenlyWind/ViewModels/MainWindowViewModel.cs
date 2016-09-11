using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Services;
using Sakuno.KanColle.Amatsukaze.Views.History;
using Sakuno.KanColle.Amatsukaze.Views.Preferences;
using Sakuno.UserInterface;
using System.Windows.Input;

namespace Sakuno.KanColle.Amatsukaze.ViewModels
{
    public class MainWindowViewModel : WindowViewModel
    {
        ModelBase r_Page;
        public ModelBase Page
        {
            get { return r_Page; }
            internal set
            {
                if (r_Page != value)
                {
                    r_Page = value;
                    OnPropertyChanged(nameof(Page));
                }
            }
        }

        public GameInformationViewModel GameInformation { get; }
        public bool IsGameStarted { get; private set; }

        public UpdateService UpdateService => UpdateService.Instance;

        public ICommand ShowPreferencesWindowCommand { get; } = new DelegatedCommand(() => WindowService.Instance.Show<PreferencesWindow>(rpClearDataContextOnWindowClosed: false));

        public ICommand ExpandMenuCommand { get; }

        public ICommand ShowConstructionHistoryCommand { get; }
        public ICommand ShowDevelopmentHistoryCommand { get; }
        public ICommand ShowSortieHistoryCommand { get; }
        public ICommand ShowExpeditionHistoryCommand { get; }
        public ICommand ShowScrappingHistoryCommand { get; }
        public ICommand ShowResourceHistoryCommand { get; }

        internal MainWindowViewModel()
        {
            GameInformation = new GameInformationViewModel(this);
            r_Page = GameInformation;

            ApiService.SubscribeOnce("api_start2", delegate
            {
                IsGameStarted = true;
                OnPropertyChanged(nameof(IsGameStarted));
            });

            ApiService.Subscribe("api_req_map/start", _ => ThemeManager.Instance.ChangeAccent(Accent.Brown));
            KanColleGame.Current.ReturnedFromSortie += _ => ThemeManager.Instance.ChangeAccent(Accent.Blue);

            ShowConstructionHistoryCommand = new DelegatedCommand(() => WindowService.Instance.Show<ConstructionHistoryWindow>());
            ShowDevelopmentHistoryCommand = new DelegatedCommand(() => WindowService.Instance.Show<DevelopmentHistoryWindow>());
            ShowSortieHistoryCommand = new DelegatedCommand(() => WindowService.Instance.Show<SortieHistoryWindow>());
            ShowExpeditionHistoryCommand = new DelegatedCommand(() => WindowService.Instance.Show<ExpeditionHistoryWindow>());
            ShowScrappingHistoryCommand = new DelegatedCommand(() => WindowService.Instance.Show<ScrappingHistoryWindow>());
            ShowResourceHistoryCommand = new DelegatedCommand(() => WindowService.Instance.Show<ResourceHistoryWindow>());
        }
    }
}
