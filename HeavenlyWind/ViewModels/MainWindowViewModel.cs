using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Services;
using Sakuno.KanColle.Amatsukaze.ViewModels.Game;
using Sakuno.KanColle.Amatsukaze.ViewModels.Tools;
using Sakuno.KanColle.Amatsukaze.Views.History;
using Sakuno.KanColle.Amatsukaze.Views.Preferences;
using Sakuno.KanColle.Amatsukaze.Views.Tools;
using Sakuno.UserInterface;
using System.Collections.Generic;
using System.Linq;
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

        SessionToolViewModel r_SessionTool = new SessionToolViewModel();

        public ICommand ShowPreferencesWindowCommand { get; } = new DelegatedCommand(() => WindowService.Instance.Show<PreferencesWindow>());

        public ICommand ExpandMenuCommand { get; }

        public ICommand ShowSessionToolCommand { get; }
        public ICommand ShowExpeditionOverviewCommand { get; }

        public ICommand ShowConstructionHistoryCommand { get; }
        public ICommand ShowDevelopmentHistoryCommand { get; }
        public ICommand ShowSortieHistoryCommand { get; }
        public ICommand ShowExpeditionHistoryCommand { get; }
        public ICommand ShowScrappingHistoryCommand { get; }
        public ICommand ShowResourceHistoryCommand { get; }

        ICommand r_OpenToolPaneCommand;
        public IList<ToolViewModel> ToolPanes { get; }

        internal MainWindowViewModel()
        {
            r_Page = new InitializationPageViewModel(this);
            GameInformation = new GameInformationViewModel(this);

            SessionService.Instance.SubscribeOnce("api_start2", delegate
            {
                IsGameStarted = true;
                OnPropertyChanged(nameof(IsGameStarted));
            });

            SessionService.Instance.Subscribe("api_req_map/start", _ => ThemeManager.Instance.ChangeAccent(Accent.Brown));
            KanColleGame.Current.ReturnedFromSortie += _ => ThemeManager.Instance.ChangeAccent(Accent.Blue);

            ShowSessionToolCommand = new DelegatedCommand(() => WindowService.Instance.Show<SessionToolWindow>(r_SessionTool));
            ShowExpeditionOverviewCommand = new DelegatedCommand(() =>
            {
                var rExpeditionOverview = GameInformation.TabItems.OfType<ExpeditionOverviewViewModel>().SingleOrDefault() ?? new ExpeditionOverviewViewModel();
                GameInformation.AddTabItem(rExpeditionOverview);
            });

            ShowConstructionHistoryCommand = new DelegatedCommand(() => WindowService.Instance.Show<ConstructionHistoryWindow>());
            ShowDevelopmentHistoryCommand = new DelegatedCommand(() => WindowService.Instance.Show<DevelopmentHistoryWindow>());
            ShowSortieHistoryCommand = new DelegatedCommand(() => WindowService.Instance.Show<SortieHistoryWindow>());
            ShowExpeditionHistoryCommand = new DelegatedCommand(() => WindowService.Instance.Show<ExpeditionHistoryWindow>());
            ShowScrappingHistoryCommand = new DelegatedCommand(() => WindowService.Instance.Show<ScrappingHistoryWindow>());
            ShowResourceHistoryCommand = new DelegatedCommand(() => WindowService.Instance.Show<ResourceHistoryWindow>());

            r_OpenToolPaneCommand = new DelegatedCommand<ToolViewModel>(GameInformation.AddTabItem);
            ToolPanes = PluginService.Instance.ToolPanes?.Select(r => new ToolViewModel(r, r_OpenToolPaneCommand)).ToList().AsReadOnly();
        }
    }
}
