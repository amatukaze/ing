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

            SessionService.Instance.Subscribe("api_req_map/start", _ => ThemeManager.Instance.ChangeAccent(Accent.Brown));
            KanColleGame.Current.ReturnedFromSortie += _ => ThemeManager.Instance.ChangeAccent(Accent.Blue);

            ShowSessionToolCommand = new DelegatedCommand(() => WindowService.Instance.Show<SessionToolWindow>(r_SessionTool));
            ShowExpeditionOverviewCommand = new DelegatedCommand(() =>
            {
                var rGameInfo = Content as GameInformationViewModel;
                if (rGameInfo == null)
                    return;

                var rExpeditionOverview = rGameInfo.TabItems.OfType<ExpeditionOverviewViewModel>().SingleOrDefault() ?? new ExpeditionOverviewViewModel();
                rGameInfo.AddTabItem(rExpeditionOverview);
            });

            ShowConstructionHistoryCommand = new DelegatedCommand(() => WindowService.Instance.Show<ConstructionHistoryWindow>());
            ShowDevelopmentHistoryCommand = new DelegatedCommand(() => WindowService.Instance.Show<DevelopmentHistoryWindow>());
            ShowSortieHistoryCommand = new DelegatedCommand(() => WindowService.Instance.Show<SortieHistoryWindow>());
            ShowExpeditionHistoryCommand = new DelegatedCommand(() => WindowService.Instance.Show<ExpeditionHistoryWindow>());
            ShowScrappingHistoryCommand = new DelegatedCommand(() => WindowService.Instance.Show<ScrappingHistoryWindow>());
            ShowResourceHistoryCommand = new DelegatedCommand(() => WindowService.Instance.Show<ResourceHistoryWindow>());

            r_OpenToolPaneCommand = new DelegatedCommand<ToolViewModel>(r =>
            {
                var rGameInfo = Content as GameInformationViewModel;
                if (rGameInfo == null)
                    return;

                rGameInfo.AddTabItem(r);
            });
            ToolPanes = PluginService.Instance.ToolPanes?.Select(r => new ToolViewModel(r, r_OpenToolPaneCommand)).ToList().AsReadOnly();
        }
    }
}
