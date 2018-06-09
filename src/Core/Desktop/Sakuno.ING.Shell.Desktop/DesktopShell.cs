using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using Sakuno.ING.Localization;
using Sakuno.ING.Services;
using Sakuno.ING.Settings;
using Sakuno.ING.Shell.Layout;

namespace Sakuno.ING.Shell.Desktop
{
    internal class DesktopShell : FlexibleShell
    {
        private readonly LayoutSetting layoutSetting;
        private readonly ITextStreamProvider provider;
        private readonly string localeName;
        private readonly FontFamily userFont;
        private readonly List<Window> layoutWindows = new List<Window>();

        public DesktopShell(LayoutSetting layoutSetting, ILocalizationService localization, LocaleSetting locale, ITextStreamProvider provider)
            : base(localization)
        {
            this.layoutSetting = layoutSetting;
            this.provider = provider;
            localeName = locale.Language.Value;
            var userFontName = locale.UserLanguageFont.Value;
            if (!string.IsNullOrEmpty(userFontName))
                userFont = new FontFamily(userFontName);
        }

        public override void Run()
        {
            base.Run();

            Window window;
            try
            {
                window = BuildXaml(XamlReader.Parse(layoutSetting.XamlString.Value));
            }
            catch
            {
                window = BuildXaml(Application.LoadComponent(new Uri("/Sakuno.ING.Shell.Desktop;component/Layout/Default.xaml", UriKind.Relative)));
            }

            var app = new Application
            {
                ShutdownMode = ShutdownMode.OnMainWindowClose
            };

            var style = new Style
            {
                TargetType = typeof(ViewPresenter),
            };
            style.Setters.Add(new Setter(ViewPresenter.ViewSourceProperty, Views));
            app.Resources[typeof(ViewPresenter)] = style;

            app.Startup += (s, e) => provider.Enabled = true;
            app.Run(window);
        }

        private Window BuildXaml(object xamlObject)
        {
            var root = (LayoutRoot)xamlObject;
            var mainWindow = new MainWindow();
            InitWindow(mainWindow);
            mainWindow.MainContent.Content = root.MainWindow.Content.LoadContent();

            return mainWindow;
        }

        private void InitWindow(Window window)
        {
            if (!string.IsNullOrEmpty(localeName))
                window.Language = XmlLanguage.GetLanguage(localeName);
            if (userFont != null)
                window.FontFamily = userFont;
        }
    }
}
