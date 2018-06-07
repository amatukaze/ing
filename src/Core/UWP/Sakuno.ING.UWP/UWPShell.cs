using System;
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
    internal class UWPShell : FlexibleShell
    {
        private readonly LayoutSetting layoutSetting;
        private readonly ITextStreamProvider gameProvider;

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
                main = BuildXaml((LayoutRoot)XamlReader.Load(layoutSetting.XamlString.Value));
            }
            catch
            {
                var layout = new LayoutRoot();
                Application.LoadComponent(layout, new Uri("ms-appx:///Layout/Default.xaml"));
                main = BuildXaml(layout);
            }

            SetupTransparencity();
            new UISettings().ColorValuesChanged += async (sender, _) =>
            {
                foreach (var view in CoreApplication.Views)
                    await view.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                        ApplicationView.GetForCurrentView().TitleBar.ButtonForegroundColor = sender.GetColorValue(UIColorType.Foreground));
            };

            gameProvider.Enabled = true;
        }

        private UIElement BuildXaml(LayoutRoot root)
        {
            var mainView = new MainView();

            return mainView;
        }

        private void SetupTransparencity()
        {
            var coreView = CoreApplication.GetCurrentView();
            coreView.TitleBar.ExtendViewIntoTitleBar = true;
            var titlebar = ApplicationView.GetForCurrentView().TitleBar;
            titlebar.ButtonBackgroundColor = Colors.Transparent;
            titlebar.ButtonForegroundColor = new UISettings().GetColorValue(UIColorType.Foreground);
        }
    }
}
