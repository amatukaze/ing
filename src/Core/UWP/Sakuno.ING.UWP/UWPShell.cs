using System;
using System.Collections.Concurrent;
using System.Linq;
using Sakuno.ING.Composition;
using Sakuno.ING.Localization;
using Sakuno.ING.Settings;
using Sakuno.ING.Shell;
using Sakuno.ING.Shell.Layout;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace Sakuno.ING.UWP
{
    [Export(typeof(IShell))]
    internal class UWPShell : FlexibleShell<FrameworkElement>, IShell
    {
        private readonly LayoutSetting layoutSetting;
        private readonly LocaleSetting localeSetting;
        private readonly ILocalizationService localization;
        private Func<LayoutRoot> layoutFactory;
        private string[] viewIds;
        private readonly ConcurrentDictionary<string, int> applicationViewIds = new ConcurrentDictionary<string, int>();

        public UWPShell(LayoutSetting layoutSetting, LocaleSetting localeSetting, ILocalizationService localization)
            : base(localization)
        {
            this.layoutSetting = layoutSetting;
            this.localeSetting = localeSetting;
            this.localization = localization;

            Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = localeSetting.Language.Value;
        }

        event Action IShell.Exited
        {
            add { }
            remove { }
        }

        public void Run()
        {
            InitWindow();

            UIElement main;
            try
            {
                string layoutString = layoutSetting.XamlString.Value;
                layoutFactory = () => (LayoutRoot)XamlReader.Load(layoutString);
                var layout = layoutFactory();
                main = new MainView { MainContent = layout.MainWindow.LoadContent() };
                viewIds = layout.SubWindows.Select(x => x.Id).Append("Settings").ToArray();
            }
            catch
            {
                layoutFactory = () =>
                {
                    var l = new LayoutRoot();
                    Application.LoadComponent(l, new Uri("ms-appx:///Layout/Default.xaml"));
                    return l;
                };
                var layout = layoutFactory();
                main = new MainView { MainContent = layout.MainWindow.LoadContent() };
                viewIds = layout.SubWindows.Select(x => x.Id).Append("Settings").ToArray();
            }

            new UISettings().ColorValuesChanged += async (sender, _) =>
            {
                foreach (var view in CoreApplication.Views)
                    await view.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                        ApplicationView.GetForCurrentView().TitleBar.ButtonForegroundColor = sender.GetColorValue(UIColorType.Foreground));
            };

            Window.Current.Content = main;
        }

        private void InitWindow()
        {
            var translatableTextStyle = new Style
            {
                TargetType = typeof(TextBlock)
            };
            translatableTextStyle.Setters.Add(new Setter
            {
                Property = FrameworkElement.VerticalAlignmentProperty,
                Value = VerticalAlignment.Center
            });
            if (!localeSetting.TranslateContent.InitialValue)
                translatableTextStyle.Setters.Add(new Setter
                {
                    Property = FrameworkElement.LanguageProperty,
                    Value = "ja-jp"
                });
            translatableTextStyle.Seal();
            Application.Current.Resources["TranslatableTextStyle"] = translatableTextStyle;
            Application.Current.Resources["ShipNameTextStyle"] = translatableTextStyle;

            var coreView = CoreApplication.GetCurrentView();
            coreView.TitleBar.ExtendViewIntoTitleBar = true;
            var titlebar = ApplicationView.GetForCurrentView().TitleBar;
            titlebar.ButtonBackgroundColor = Colors.Transparent;
            titlebar.ButtonForegroundColor = new UISettings().GetColorValue(UIColorType.Foreground);
        }

        public async void SwitchWindow(string windowId)
        {
            if (applicationViewIds.TryGetValue(windowId, out int id))
            {
                await ApplicationViewSwitcher.SwitchAsync(id);
                return;
            }

            if (viewIds.Contains(windowId))
            {
                var coreView = CoreApplication.CreateNewView();
                await coreView.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
                    CoreApplication.GetCurrentView().Properties["Id"] = windowId;
                    var appView = ApplicationView.GetForCurrentView();
                    applicationViewIds[windowId] = appView.Id;

                    InitWindow();
                    if (windowId == "Settings")
                        Window.Current.Content = new SettingsView(CreateSettingViews(), localization);
                    else
                        Window.Current.Content = new SubView
                        {
                            ActualContent = layoutFactory()[windowId].LoadContent(),
                            ActualTitle = localization.GetLocalized("ViewTitle", windowId) ?? windowId
                        };

                    appView.Consolidated += (s, e) =>
                    {
                        applicationViewIds.TryRemove(windowId, out _);
                        CoreApplication.GetCurrentView().CoreWindow.Close();
                    };

                    Window.Current.Activate();
                });
                await ApplicationViewSwitcher.TryShowAsStandaloneAsync(applicationViewIds[windowId]);
            }
        }

        public async void ShowViewWithParameter<T>(string viewId, T parameter)
        {
            var coreView = CoreApplication.CreateNewView();
            int appViewId = 0;
            await coreView.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                var appView = ApplicationView.GetForCurrentView();
                appViewId = appView.Id;

                InitWindow();
                Window.Current.Content = new SubView
                {
                    ActualContent = Compositor.Default.ResolveNamedWithParameter<FrameworkElement, T>(viewId, parameter),
                    ActualTitle = localization.GetLocalized("ViewTitle", viewId) ?? viewId
                };

                appView.Consolidated += (s, e) =>
                {
                    CoreApplication.GetCurrentView().CoreWindow.Close();
                };

                Window.Current.Activate();
            });
            await ApplicationViewSwitcher.TryShowAsStandaloneAsync(appViewId);
        }
    }
}
