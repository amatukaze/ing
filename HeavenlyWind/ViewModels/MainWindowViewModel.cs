using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Models;
using Sakuno.KanColle.Amatsukaze.Services;
using Sakuno.KanColle.Amatsukaze.Views.History;
using Sakuno.KanColle.Amatsukaze.Views.Preferences;
using Sakuno.KanColle.Amatsukaze.Views.Statistics;
using Sakuno.UserInterface;
using System;
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

        Accent r_BlinkingBrownAccent;

        public IList<UIZoomInfo> UIZoomFactors { get; private set; }

        public ICommand ShowPreferencesWindowCommand { get; } = new DelegatedCommand(() => WindowService.Instance.Show<PreferencesWindow>(rpClearDataContextOnWindowClosed: false));

        public ICommand ExpandMenuCommand { get; }

        public ICommand UISetZoomCommand { get; private set; }
        public ICommand UIZoomInCommand { get; private set; }
        public ICommand UIZoomOutCommand { get; private set; }

        public ICommand ShowConstructionHistoryCommand { get; }
        public ICommand ShowDevelopmentHistoryCommand { get; }
        public ICommand ShowSortieHistoryCommand { get; }
        public ICommand ShowExpeditionHistoryCommand { get; }
        public ICommand ShowScrappingHistoryCommand { get; }
        public ICommand ShowResourceHistoryCommand { get; }
        public ICommand ShowSortieConsumptionHistoryCommand { get; }
        public ICommand ShowSortieStatisticCommand { get; }

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

            r_BlinkingBrownAccent = new Accent("BlinkingBrown", new Uri("pack://application:,,,/HeavenlyWind;component/Themes/Accents/BlinkingBrown.xaml"));

            PropertyChangedEventListener.FromSource(NotificationService.Instance)
                .Add(nameof(NotificationService.Instance.IsBlinking), delegate
                {
                    if (NotificationService.Instance.IsBlinking)
                        ThemeManager.Instance.ChangeAccent(r_BlinkingBrownAccent);
                });

            UISetZoomCommand = new DelegatedCommand<double>(SetZoom);
            UIZoomInCommand = new DelegatedCommand(() => SetZoom(Preference.Instance.UI.Zoom.Value + .05));
            UIZoomOutCommand = new DelegatedCommand(() => SetZoom(Preference.Instance.UI.Zoom.Value - .05));

            UIZoomFactors = new[] { .25, .5, .75, 1.0, 1.25, 1.5, 1.75, 2.0, 3.0, 4.0 }.Select(r => new UIZoomInfo(r, UISetZoomCommand)).ToArray();

            ShowConstructionHistoryCommand = new DelegatedCommand(() => WindowService.Instance.Show<ConstructionHistoryWindow>());
            ShowDevelopmentHistoryCommand = new DelegatedCommand(() => WindowService.Instance.Show<DevelopmentHistoryWindow>());
            ShowSortieHistoryCommand = new DelegatedCommand(() => WindowService.Instance.Show<SortieHistoryWindow>());
            ShowExpeditionHistoryCommand = new DelegatedCommand(() => WindowService.Instance.Show<ExpeditionHistoryWindow>());
            ShowScrappingHistoryCommand = new DelegatedCommand(() => WindowService.Instance.Show<ScrappingHistoryWindow>());
            ShowResourceHistoryCommand = new DelegatedCommand(() => WindowService.Instance.Show<ResourceHistoryWindow>());
            ShowSortieConsumptionHistoryCommand = new DelegatedCommand(() => WindowService.Instance.Show<SortieConsumptionHistoryWindow>());
            ShowSortieStatisticCommand = new DelegatedCommand(() => WindowService.Instance.Show<SortieStatisticWindow>());
        }

        void SetZoom(double rpZoom)
        {
            rpZoom = Math.Round(rpZoom, 2);

            if (rpZoom < .25)
                return;

            foreach (var rInfo in UIZoomFactors)
                rInfo.IsSelected = rInfo.Zoom == rpZoom;

            Preference.Instance.UI.Zoom.Value = rpZoom;
        }
    }
}
