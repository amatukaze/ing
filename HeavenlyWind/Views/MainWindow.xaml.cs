using Sakuno.KanColle.Amatsukaze.Services;
using Sakuno.KanColle.Amatsukaze.ViewModels;
using Sakuno.SystemInterop;
using Sakuno.UserInterface.Controls;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace Sakuno.KanColle.Amatsukaze.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
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

            var rHandle = new WindowInteropHelper(this).Handle;

            PowerManager.RegisterMonitor(this);

            PanicKeyService.Instance.Initialize(rHandle);
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            ((InitializationPageViewModel)App.Root.Page).Start();
        }

        protected override void OnClosing(CancelEventArgs e)
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
            if (rDialog.Show().ClickedCommonButton == TaskDialogCommonButton.No)
            {
                e.Cancel = true;
                return;
            }

            base.OnClosing(e);
        }
    }
}
