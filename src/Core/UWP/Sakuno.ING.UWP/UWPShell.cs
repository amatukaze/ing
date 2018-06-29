using System;
using System.Collections.Concurrent;
using System.Linq;
using Sakuno.ING.Composition;
using Sakuno.ING.Localization;
using Sakuno.ING.Services;
using Sakuno.ING.Settings;
using Sakuno.ING.Shell;
using Sakuno.ING.Shell.Layout;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace Sakuno.ING.UWP
{
    [Export(typeof(IShell))]
    internal class UWPShell : FlexibleShell<FrameworkElement>
    {
        private readonly LayoutSetting layoutSetting;
        private readonly ITextStreamProvider gameProvider;
        private readonly ILocalizationService localizationService;
        private Func<LayoutRoot> layoutFactory;
        private string[] viewIds;
        private readonly ConcurrentDictionary<string, int> applicationViewIds = new ConcurrentDictionary<string, int>();

        public UWPShell(LayoutSetting layoutSetting, ITextStreamProvider gameProvider, LocaleSetting localeSetting, ILocalizationService localizationService)
            : base(localizationService)
        {
            this.layoutSetting = layoutSetting;
            this.gameProvider = gameProvider;
            this.localizationService = localizationService;

            Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = localeSetting.Language.Value;
        }

        public override void Run()
        {
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

            InitWindow();
            new UISettings().ColorValuesChanged += async (sender, _) =>
            {
                foreach (var view in CoreApplication.Views)
                    await view.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                        ApplicationView.GetForCurrentView().TitleBar.ButtonForegroundColor = sender.GetColorValue(UIColorType.Foreground));
            };

            Window.Current.Content = main;
            gameProvider.Enabled = true;
        }

        private void InitWindow()
        {
            var coreView = CoreApplication.GetCurrentView();
            coreView.TitleBar.ExtendViewIntoTitleBar = true;
            var titlebar = ApplicationView.GetForCurrentView().TitleBar;
            titlebar.ButtonBackgroundColor = Colors.Transparent;
            titlebar.ButtonForegroundColor = new UISettings().GetColorValue(UIColorType.Foreground);
        }

        public async override void SwitchWindow(string windowId)
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
                    var view = ApplicationView.GetForCurrentView();
                    applicationViewIds[windowId] = view.Id;

                    InitWindow();
                    if (windowId == "Settings")
                        Window.Current.Content = new SettingsView(CreateSettingViews(), localizationService);
                    else
                        Window.Current.Content = new SubView { ActualContent = layoutFactory()[windowId].LoadContent() };

                    view.Consolidated += (s, e) =>
                    {
                        applicationViewIds.TryRemove(windowId, out _);
                        CoreApplication.GetCurrentView().CoreWindow.Close();
                    };

                    Window.Current.Activate();
                });
                await ApplicationViewSwitcher.TryShowAsStandaloneAsync(applicationViewIds[windowId]);
            }
        }
    }
}
