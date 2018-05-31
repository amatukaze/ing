using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml.Linq;
using Sakuno.ING.Localization;
using Sakuno.ING.Services;
using Sakuno.ING.Settings;
using Sakuno.ING.ViewModels;
using Sakuno.ING.ViewModels.Layout;

namespace Sakuno.ING.Shell.Desktop
{
    class Shell : IShell
    {
        MainWindowVM _mainWindowVM;
        private readonly LayoutSetting layoutSetting;
        private readonly ILocalizationService localization;
        private readonly ITextStreamProvider provider;
        private readonly string localeName;
        private readonly FontFamily userFont;

        public Shell(LayoutSetting layoutSetting, ILocalizationService localization, LocaleSetting locale, ITextStreamProvider provider)
        {
            _mainWindowVM = new MainWindowVM()
            {
                Title = "Intelligent Naval Gun",
            };
            this.layoutSetting = layoutSetting;
            this.localization = localization;
            this.provider = provider;
            localeName = locale.Language.Value;
            var userFontName = locale.UserLanguageFont.Value;
            if (!string.IsNullOrEmpty(userFontName))
                userFont = new FontFamily(userFontName);
        }

        private readonly Dictionary<string, (Type ViewType, bool IsFixedSize, bool SingleWindowRecommended)> views = new Dictionary<string, (Type, bool, bool)>();
        private readonly List<(Type ViewType, SettingCategory Category)> settingViews = new List<(Type, SettingCategory)>();
        private bool started;

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

        private LayoutRoot layout;
        private MainWindow main;
        private SettingsWindow settings;

        public void Run()
        {
            started = true;

            XDocument layoutDocument;
            try
            {
                layoutDocument = XDocument.Parse(layoutSetting.XmlString.Value);
            }
            catch
            {
                string layoutString;
                using (var file = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(Shell), "Layout.Default.xml")))
                    layoutString = file.ReadToEnd();
                layoutSetting.XmlString.Value = layoutString;
                layoutDocument = XDocument.Parse(layoutString);
            }

            layout = LayoutRoot.FromXml(layoutDocument);

            var app = new Application
            {
                ShutdownMode = ShutdownMode.OnMainWindowClose
            };

            app.Startup += (sender, args) =>
            {
                main = new MainWindow() { DataContext = _mainWindowVM };
                InitWindow(main);
                main.Settings.Click += (_, __) =>
                {
                    if (settings == null)
                    {
                        settings = new SettingsWindow
                        {
                            DataContext = settingViews
                                .GroupBy(e => e.Category)
                                .OrderBy(e => e.Key)
                                .Select(e => new
                                {
                                    Title = localization.GetLocalized("SettingCategory", e.Key.ToString()) ?? e.Key.ToString(),
                                    Content = e.Select(t => Activator.CreateInstance(t.ViewType)).ToArray()
                                }).ToArray()
                        };
                        InitWindow(settings);
                        settings.Closed += (___, ____) => settings = null;
                    }
                    settings.Show();
                    settings.Activate();
                };

                Rearrange();

                layoutSetting.XmlString.ValueChanged += s =>
                {
                    try
                    {
                        layout = LayoutRoot.FromXml(XDocument.Parse(s));
                        Rearrange();
                    }
                    catch
                    {
                        // log:fail to reload layout
                    }
                };

                provider.Enabled = true;

                (sender as Application).MainWindow = main;
                main.Show();
            };

            app.Run();
        }

        private void InitWindow(Window window)
        {
            if (!string.IsNullOrEmpty(localeName))
                window.Language = XmlLanguage.GetLanguage(localeName);
            if (userFont != null)
                window.FontFamily = userFont;
        }

        private string GetTitle(LayoutBase item)
            => item.Title
            ?? localization.GetLocalized("ViewTitle", item.Id)
            ?? item.Id;

        private void Rearrange()
        {
            foreach (var entry in layout.Entries)
                if (main.MainContent.Content == null)
                    main.MainContent.Content = BuildLayout(entry);
                else
                {
                    var button = new Button { Content = GetTitle(entry), Tag = entry };
                    button.Click += (sender, e) =>
                    {
                        var le = (sender as Button).Tag as LayoutBase;
                        var window = new Window { Content = BuildLayout(le) };
                        InitWindow(window);
                        window.Show();
                        window.Activate();
                    };
                    main.Switcher.Children.Add(button);
                }
        }

        private FrameworkElement BuildLayout(LayoutBase layout)
        {
            switch (layout)
            {
                case TabLayout tab:
                    var tabcontrol = new TabControl { Name = tab.Id };
                    foreach (var child in tab.Children)
                        tabcontrol.Items.Add(new TabItem
                        {
                            Header = GetTitle(child),
                            Content = BuildLayout(child)
                        });
                    return tabcontrol;

                case LayoutItem item:
                    if (!views.TryGetValue(item.Id, out var descriptor))
                        return null;
                    return (FrameworkElement)Activator.CreateInstance(descriptor.ViewType);
                default:
                    return null;
            }
        }
    }
}
