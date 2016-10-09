using Sakuno.KanColle.Amatsukaze.Extensibility;
using Sakuno.KanColle.Amatsukaze.Extensibility.Services;
using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Models;
using Sakuno.KanColle.Amatsukaze.Services;
using Sakuno.SystemInterop;
using Sakuno.UserInterface;
using Sakuno.UserInterface.Controls;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace Sakuno.KanColle.Amatsukaze.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : MetroWindow, IMainWindowService
    {
        IntPtr r_Handle;
        IntPtr IMainWindowService.Handle => r_Handle;

        static MainWindow()
        {
            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(int.MaxValue));
        }
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            r_Handle = new WindowInteropHelper(this).Handle;

            PowerManager.RegisterMonitor(this);

            PanicKeyService.Instance.Initialize(r_Handle);

            ServiceManager.Register<IMainWindowService>(this);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            var rMode = Preference.Instance.Browser.RefreshConfirmationMode.Value;
            if (rMode == ConfirmationMode.Always || (rMode == ConfirmationMode.DuringSortie && KanColleGame.Current.Sortie is SortieInfo && !(KanColleGame.Current.Sortie is PracticeInfo)))
            {
                var rDialog = new TaskDialog()
                {
                    Caption = StringResources.Instance.Main.Product_Name,
                    Instruction = StringResources.Instance.Main.Window_ClosingConfirmation_Instruction,
                    Icon = TaskDialogIcon.Information,
                    Buttons =
                    {
                        new TaskDialogCommandLink(TaskDialogCommonButton.Yes, StringResources.Instance.Main.Window_ClosingConfirmation_Button_Yes),
                        new TaskDialogCommandLink(TaskDialogCommonButton.No, StringResources.Instance.Main.Window_ClosingConfirmation_Button_No),
                    },
                    DefaultCommonButton = TaskDialogCommonButton.No,

                    OwnerWindow = this,
                    ShowAtTheCenterOfOwner = true,
                };
                if (rDialog.ShowAndDispose().ClickedCommonButton == TaskDialogCommonButton.No)
                {
                    e.Cancel = true;
                    return;
                }
            }

            base.OnClosing(e);
        }

        internal void SubscribeBrowserPreferenceChanged()
        {
            if (!BrowserService.Instance.NoInstalledLayoutEngines)
            {
                BrowserService.Instance.Resized += (s, e) => Dispatcher.Invoke(UpdateSize);

                Preference.Instance.UI.LandscapeDock.Subscribe(OnDockChanged);
                Preference.Instance.UI.PortraitDock.Subscribe(OnDockChanged);
            }
        }

        void UpdateSize()
        {
            var rZoom = DpiUtil.ScaleX + Preference.Instance.Browser.Zoom - 1.0;
            var rBrowserWidth = GameConstants.GameWidth * rZoom / DpiUtil.ScaleX / DpiUtil.ScaleX;
            var rBrowserHeight = GameConstants.GameHeight * rZoom / DpiUtil.ScaleX / DpiUtil.ScaleX + Browser.Instance.GetBarSize().Height;

            MinWidth = rBrowserWidth;
            MinHeight = rBrowserHeight + r_CaptionBar.ActualHeight + r_StatusBar.ActualHeight;
        }

        void OnDockChanged(Dock rpDock)
        {
            Dispatcher.InvokeAsync(() =>
            {
                var rZoom = DpiUtil.ScaleX + Preference.Instance.Browser.Zoom - 1.0;
                var rWidth = GameConstants.GameWidth * rZoom / DpiUtil.ScaleX / DpiUtil.ScaleX;
                var rHeight = GameConstants.GameHeight * rZoom / DpiUtil.ScaleX / DpiUtil.ScaleX + Browser.Instance.GetBarSize().Height;

                switch (rpDock)
                {
                    case Dock.Left:
                    case Dock.Right:
                        rWidth += 400;
                        break;

                    case Dock.Top:
                    case Dock.Bottom:
                        rHeight += 400;
                        break;
                }

                Width = rWidth;
                Height = rHeight;
            });
        }

        void IMainWindowService.Flash(int rpCount, int rpTimeout)
        {
            var rInfo = new NativeStructs.FLASHWINFO()
            {
                cbSize = Marshal.SizeOf(typeof(NativeStructs.FLASHWINFO)),
                hwnd = r_Handle,
                dwFlags = NativeEnums.FLASHW.FLASHW_TRAY | NativeEnums.FLASHW.FLASHW_TIMERNOFG,
                uCount = (uint)rpCount,
                dwTimeout = (uint)rpTimeout,
            };
            NativeMethods.User32.FlashWindowEx(ref rInfo);
        }
    }
}
