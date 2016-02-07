using Sakuno.KanColle.Amatsukaze.Services;
using Sakuno.SystemInterop;
using Sakuno.UserInterface;
using Sakuno.UserInterface.Controls;
using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Sakuno.KanColle.Amatsukaze.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        static readonly Size r_GameInformationDefaultSize = new Size(500, 400);

        Dock CurrentDock => ScreenOrientation == ScreenOrientation.Landscape ? Preference.Current.Layout.LandscapeDock : Preference.Current.Layout.PortraitDock;

        public MainWindow()
        {
            InitializeComponent();

            if (!BrowserService.Instance.NoInstalledLayoutEngines)
            {
                var rLayoutPreferencePropertyChangedSource = Observable.FromEventPattern<PropertyChangedEventArgs>(Preference.Current.Layout, nameof(Preference.Current.Layout.PropertyChanged))
                    .Select(r => r.EventArgs.PropertyName);
                rLayoutPreferencePropertyChangedSource.Where(r => r == nameof(Preference.Current.Layout.LandscapeDock)).Subscribe(_ => UpdateSize(Preference.Current.Layout.LandscapeDock));
                rLayoutPreferencePropertyChangedSource.Where(r => r == nameof(Preference.Current.Layout.PortraitDock)).Subscribe(_ => UpdateSize(Preference.Current.Layout.PortraitDock));

                r_Browser.SizeChanged += Browser_SizeChanged;
            }
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            PowerMonitor.RegisterMonitor(this);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (MessageBox.Show(this, StringResources.Instance.Main.Window_ClosingConfirmation, ProductInfo.FullAppName, MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.No) == MessageBoxResult.No)
            {
                e.Cancel = true;
                return;
            }

            base.OnClosing(e);
        }

        void Browser_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!(r_Browser.Host?.IsExtracted).GetValueOrDefault())
                return;

            var rDock = CurrentDock;
            var rSize = r_Browser.DesiredSize;

            MinWidth = rSize.Width;
            MinHeight = r_CaptionBar.ActualHeight + rSize.Height + r_StatusBar.ActualHeight;

            if (Width < MinWidth)
                Width = MinWidth;
            if (Height < MinHeight)
                Height = MinHeight;

            UpdateSize();
        }

        void UpdateSize() => UpdateSize(CurrentDock);
        void UpdateSize(Dock rpDock)
        {
            var rBrowserDesiredSize = r_Browser.DesiredSize;
            var rWidth = rBrowserDesiredSize.Width;
            var rHeight = r_CaptionBar.ActualHeight + rBrowserDesiredSize.Height + r_StatusBar.ActualHeight;

            if (rpDock == Dock.Left || rpDock == Dock.Right)
                rWidth += Math.Max(r_Content.ActualWidth, r_GameInformationDefaultSize.Width);
            else
                rHeight += Math.Max(r_Content.ActualHeight, r_GameInformationDefaultSize.Height);

            Width = rWidth;
            Height = rHeight;
        }
    }
}
