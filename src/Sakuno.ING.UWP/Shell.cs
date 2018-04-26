using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Sakuno.ING.Data;
using Sakuno.ING.Services;
using Sakuno.ING.Settings;
using Sakuno.ING.Shell;
using Sakuno.ING.ViewModels;
using Sakuno.ING.ViewModels.Layout;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Sakuno.ING.UWP
{
    internal class Shell : IShell
    {
        private readonly IDataService dataService;
        private readonly ITextStreamProvider gameProvider;
        private readonly ILocalizationService localizationService;

        public Shell(IDataService dataService, ITextStreamProvider gameProvider, LocaleSetting localeSetting, ILocalizationService localizationService)
        {
            this.dataService = dataService;
            this.gameProvider = gameProvider;
            this.localizationService = localizationService;

            Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = localeSetting.Language.Value;
        }

        public async void Run()
        {
            started = true;

            XDocument layoutDocument = null;
            try
            {
                using (var file = await dataService.ReadFile("layout.xml"))
                    layoutDocument = XDocument.Load(file);
            }
            catch (FileNotFoundException)
            {
                // load default
            }

            if (layoutDocument != null)
            {
                layout = new LayoutRoot().FromXml(layoutDocument);
            }
            else
            {
                layout = new LayoutRoot();
                layout.Entries.Add(new LayoutItem { Id = "ApiDebug" });
                layout.Entries.Add(new LayoutItem { Id = "MasterData" });
                // load browser only
            }

            SetupTransparencity();
            new UISettings().ColorValuesChanged += async (sender, _) =>
            {
                foreach (var view in CoreApplication.Views)
                    await view.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                        ApplicationView.GetForCurrentView().TitleBar.ButtonForegroundColor = sender.GetColorValue(UIColorType.Foreground));
            };
            Window.Current.Content = main = new MainView(new MainWindowVM(), this);
            Rearrange();
            gameProvider.Enabled = true;
        }

        private readonly Dictionary<string, (Type ViewType, bool IsFixedSize, bool SingleWindowRecommended)> views = new Dictionary<string, (Type, bool, bool)>();
        private readonly List<(Type ViewType, SettingCategory Category)> settingViews = new List<(Type, SettingCategory)>();
        private LayoutRoot layout;
        private bool started;
        private MainView main;

        public void RegisterView(Type viewType, string id, bool isFixedSize = true, bool singleWindowRecommended = false)
        {
            if (started) throw new InvalidOperationException("Shell already started.");
            views.Add(id, (viewType, isFixedSize, singleWindowRecommended));
        }

        public void RegisterSettingView(Type viewType, SettingCategory category = SettingCategory.Misc)
        {
            if (started) throw new InvalidOperationException("Shell already started.");
            settingViews.Add((viewType, category));
        }

        private void SetupTransparencity()
        {
            var coreView = CoreApplication.GetCurrentView();
            coreView.TitleBar.ExtendViewIntoTitleBar = true;
            var titlebar = ApplicationView.GetForCurrentView().TitleBar;
            titlebar.ButtonBackgroundColor = Colors.Transparent;
            titlebar.ButtonForegroundColor = new UISettings().GetColorValue(UIColorType.Foreground);
        }

        private int settingsViewId = 0;
        internal async void ShowSettingsView()
        {
            if (settingsViewId == 0)
            {
                var coreView = CoreApplication.CreateNewView();
                await coreView.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
                    var view = ApplicationView.GetForCurrentView();
                    settingsViewId = view.Id;

                    SetupTransparencity();
                    Window.Current.Content = new SettingsView(settingViews);

                    view.Consolidated += (_, __) =>
                    {
                        settingsViewId = 0;
                        Window.Current.Content = null;
                    };

                    Window.Current.Activate();
                });
            }
            await ApplicationViewSwitcher.TryShowAsStandaloneAsync(settingsViewId);
        }

        internal void Rearrange()
        {
            foreach (var view in CoreApplication.Views)
                if (!view.IsMain)
                    view.CoreWindow.Close();

            main.MainContent.Content = null;
            main.ViewSwitcher.Children.Clear();
            foreach (var entry in layout.Entries)
            {
                if (main.MainContent.Content == null)
                    main.MainContent.Content = BuildLayout(entry);
                else
                {
                    var button = new Button { Content = GetTitle(entry), Tag = entry };
                    button.Tapped += async (sender, e) =>
                    {
                        var le = (sender as Button).Tag as LayoutBase;
                        var coreView = CoreApplication.CreateNewView();
                        int coreViewId = 0;

                        await coreView.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                        {
                            var view = ApplicationView.GetForCurrentView();
                            coreViewId = view.Id;

                            SetupTransparencity();
                            Window.Current.Content = new SubView
                            {
                                Content = BuildLayout(le)
                            };
                            view.Consolidated += (_, __) => Window.Current.Content = null;

                            Window.Current.Activate();
                        });
                        await ApplicationViewSwitcher.TryShowAsStandaloneAsync(coreViewId);
                    };
                    main.ViewSwitcher.Children.Add(button);
                }
            }
        }

        private string GetTitle(LayoutBase item)
            => item.Title
            ?? localizationService.GetLocalized("ViewTitle", item.Id)
            ?? item.Id;

        private UIElement BuildLayout(LayoutBase layout)
        {
            switch (layout)
            {
                case TabLayout tab:
                    var pivot = new Pivot { Name = tab.Id };
                    foreach (var child in tab.Children)
                        pivot.Items.Add(new PivotItem
                        {
                            Header = GetTitle(child),
                            Content = BuildLayout(child)
                        });
                    return pivot;
                case RelativeLayout relative:
                    var panel = new RelativePanel();
                    foreach (var child in relative.Children)
                        panel.Children.Add(BuildLayout(child));
                    foreach (var relation in relative.Relations)
                    {
                        var item = panel.FindName(relation.Item) as UIElement;
                        switch (relation.Type)
                        {
                            case RelativeRelationType.LeftOf:
                                RelativePanel.SetLeftOf(item, relation.Base);
                                break;
                            case RelativeRelationType.RightOf:
                                RelativePanel.SetRightOf(item, relation.Base);
                                break;
                            case RelativeRelationType.Above:
                                RelativePanel.SetAbove(item, relation.Base);
                                break;
                            case RelativeRelationType.Below:
                                RelativePanel.SetBelow(item, relation.Base);
                                break;
                            case RelativeRelationType.AlignLeft:
                                if (relation.Base != null)
                                    RelativePanel.SetAlignLeftWith(item, relation.Base);
                                else
                                    RelativePanel.SetAlignLeftWithPanel(item, true);
                                break;
                            case RelativeRelationType.AlignRight:
                                if (relation.Base != null)
                                    RelativePanel.SetAlignRightWith(item, relation.Base);
                                else
                                    RelativePanel.SetAlignRightWithPanel(item, true);
                                break;
                            case RelativeRelationType.AlignTop:
                                if (relation.Base != null)
                                    RelativePanel.SetAlignTopWith(item, relation.Base);
                                else
                                    RelativePanel.SetAlignTopWithPanel(item, true);
                                break;
                            case RelativeRelationType.AlignBottom:
                                if (relation.Base != null)
                                    RelativePanel.SetAlignBottomWith(item, relation.Base);
                                else
                                    RelativePanel.SetAlignBottomWithPanel(item, true);
                                break;
                            case RelativeRelationType.AlignHorizontal:
                                if (relation.Base != null)
                                    RelativePanel.SetAlignHorizontalCenterWith(item, relation.Base);
                                else
                                    RelativePanel.SetAlignHorizontalCenterWithPanel(item, true);
                                break;
                            case RelativeRelationType.AlignVertical:
                                if (relation.Base != null)
                                    RelativePanel.SetAlignVerticalCenterWith(item, relation.Base);
                                else
                                    RelativePanel.SetAlignVerticalCenterWithPanel(item, true);
                                break;
                        }
                    }
                    return panel;
                case LayoutItem item:
                    if (!views.TryGetValue(item.Id, out var descriptor))
                        return null;
                    var view = (FrameworkElement)Activator.CreateInstance(descriptor.ViewType);
                    return new Border
                    {
                        Margin = new Thickness(2),
                        Padding = new Thickness(2),
                        BorderThickness = new Thickness(2),
                        BorderBrush = new SolidColorBrush(Colors.Gray),
                        HorizontalAlignment = view.HorizontalAlignment,
                        VerticalAlignment = view.VerticalAlignment,
                        Child = view
                    };
                default:
                    return null;
            }
        }
    }
}
