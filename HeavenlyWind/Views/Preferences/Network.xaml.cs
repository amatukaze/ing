using Sakuno.SystemInterop;
using Sakuno.UserInterface;
using System.Windows;

namespace Sakuno.KanColle.Amatsukaze.Views.Preferences
{
    /// <summary>
    /// Network.xaml の相互作用ロジック
    /// </summary>
    partial class Network
    {
        public Network()
        {
            InitializeComponent();

            Loaded += Network_Loaded;
        }

        void Network_Loaded(object sender, RoutedEventArgs e)
        {
            EnablePortCustomization.Checked += EnablePortCustomization_Checked;
            HttpOnly.Checked += HttpOnly_Checked;
        }

        void EnablePortCustomization_Checked(object sender, RoutedEventArgs e)
        {
            if (!EnablePortCustomization.IsChecked.Value)
                return;

            var rDialog = new TaskDialog()
            {
                Caption = StringResources.Instance.Main.Product_Name,
                Instruction = StringResources.Instance.Main.PreferenceWindow_Network_Port_Customization_Dialog_Instruction,
                Icon = TaskDialogIcon.Information,
                Content = StringResources.Instance.Main.PreferenceWindow_Network_Port_Customization_Dialog_Content,
                Buttons =
                {
                    new TaskDialogButton(TaskDialogCommonButton.Yes, StringResources.Instance.Main.PreferenceWindow_Network_Port_Customization_Dialog_Button_Yes),
                    new TaskDialogButton(TaskDialogCommonButton.No, StringResources.Instance.Main.PreferenceWindow_Network_Port_Customization_Dialog_Button_No),
                },
                DefaultCommonButton = TaskDialogCommonButton.No,

                OwnerWindow = WindowUtil.GetTopWindow(),
                ShowAtTheCenterOfOwner = true,
            };

            if (rDialog.Show().ClickedCommonButton == TaskDialogCommonButton.No)
                EnablePortCustomization.IsChecked = false;
        }

        void HttpOnly_Checked(object sender, RoutedEventArgs e)
        {
            if (!HttpOnly.IsChecked.Value)
                return;

            var rDialog = new TaskDialog()
            {
                Caption = StringResources.Instance.Main.Product_Name,
                Instruction = StringResources.Instance.Main.PreferenceWindow_Network_UseUpstreamProxy_HttpOnly_Dialog_Instruction,
                Icon = TaskDialogIcon.Information,
                Content = StringResources.Instance.Main.PreferenceWindow_Network_UseUpstreamProxy_HttpOnly_Dialog_Content,
                Buttons =
                {
                    new TaskDialogButton(TaskDialogCommonButton.Yes, StringResources.Instance.Main.PreferenceWindow_Network_UseUpstreamProxy_HttpOnly_Dialog_Button_Yes),
                    new TaskDialogButton(TaskDialogCommonButton.No, StringResources.Instance.Main.PreferenceWindow_Network_UseUpstreamProxy_HttpOnly_Dialog_Button_No),
                },
                DefaultCommonButton = TaskDialogCommonButton.No,

                OwnerWindow = WindowUtil.GetTopWindow(),
                ShowAtTheCenterOfOwner = true,
            };
            if (rDialog.ShowAndDispose().ClickedCommonButton == TaskDialogCommonButton.No)
                HttpOnly.IsChecked = false;
        }
    }
}
