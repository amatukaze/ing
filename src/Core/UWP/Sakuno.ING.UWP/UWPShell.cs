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
    internal class UWPShell : FlexibleShell
    {
        private readonly LayoutSetting layoutSetting;
        private readonly ITextStreamProvider gameProvider;
        private Func<LayoutRoot> layoutFactory;
        private string[] viewIds;
        private readonly ConcurrentDictionary<string, int> applicationViewIds = new ConcurrentDictionary<string, int>();

        public UWPShell(LayoutSetting layoutSetting, ITextStreamProvider gameProvider, LocaleSetting localeSetting, ILocalizationService localizationService)
            : base(localizationService)
        {
            this.layoutSetting = layoutSetting;
            this.gameProvider = gameProvider;

            Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = localeSetting.Language.Value;
        }

        public override void Run()
        {
            base.Run();

            UIElement main;
            try
            {
                string layoutString = layoutSetting.XamlString.Value;
                layoutFactory = () => (LayoutRoot)XamlReader.Load(layoutString);
                var layout = layoutFactory();
                main = new MainView { MainContent = layout.MainWindow.Content.LoadContent() };
                viewIds = layout.SubWindows.Select(x => x.Id).ToArray();
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
                main = new MainView { MainContent = layout.MainWindow.Content.LoadContent() };
                viewIds = layout.SubWindows.Select(x => x.Id).ToArray();
            }

            InitWindow();
            new UISettings().ColorValuesChanged += async (sender, _) =>
            {
                foreach (var view in CoreApplication.Views)
                    await view.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                        ApplicationView.GetForCurrentView().TitleBar.ButtonForegroundColor = sender.GetColorValue(UIColorType.Foreground));
            };

            var style = new Style
            {
                TargetType = typeof(ViewPresenter),
            };
            style.Setters.Add(new Setter(ViewPresenter.ViewSourceProperty, Views));
            Application.Current.Resources[typeof(ViewPresenter)] = style;
            Application.Current.Resources[ViewSwitcher.SwitchActionKey] = new Action<string>(async viewId =>
            {
                if (applicationViewIds.TryGetValue(viewId, out int id))
                {
                    await ApplicationViewSwitcher.SwitchAsync(id);
                    return;
                }

                if (viewIds.Contains(viewId))
                {
                    var coreView = CoreApplication.CreateNewView();
                    await coreView.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                    {
                        CoreApplication.GetCurrentView().Properties["Id"] = viewId;
                        var view = ApplicationView.GetForCurrentView();
                        applicationViewIds[viewId] = view.Id;

                        InitWindow();
                        Window.Current.Content = new SubView { ActualContent = layoutFactory()[viewId].Content.LoadContent() };

                        view.Consolidated += (s, e) =>
                        {
                            Window.Current.Content = null;
                            applicationViewIds.TryRemove(viewId, out _);
                        };

                        Window.Current.Activate();
                    });
                    await ApplicationViewSwitcher.TryShowAsStandaloneAsync(applicationViewIds[viewId]);
                }
            });

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
    }
}
