using Sakuno.SystemInterop;
using Sakuno.UserInterface;
using System.Windows;
using System.Windows.Input;

namespace Sakuno.KanColle.Amatsukaze.Views.Preferences
{
    /// <summary>
    /// Other.xaml の相互作用ロジック
    /// </summary>
    partial class Other
    {
        Key r_OldKey;
        ModifierKeys r_OldModifierKeys;

        public Other()
        {
            InitializeComponent();

            Loaded += Other_Loaded;
        }

        void Other_Loaded(object sender, RoutedEventArgs e)
        {
            EnablePortCustomization.Checked += EnablePortCustomization_Checked;
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

        void PanicKey_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            var rKey = e.Key;
            switch (rKey)
            {
                case Key.LWin:
                case Key.RWin:
                case Key.NumLock:
                case Key.LeftShift:
                case Key.RightShift:
                case Key.LeftCtrl:
                case Key.RightCtrl:
                case Key.LeftAlt:
                case Key.RightAlt:
                    return;
            }

            if (r_OldKey == rKey && r_OldModifierKeys == Keyboard.Modifiers)
                return;

            r_OldKey = rKey;
            r_OldModifierKeys = Keyboard.Modifiers;

            Preference.Instance.Other.PanicKey.UpdateKey((int)Keyboard.Modifiers, KeyInterop.VirtualKeyFromKey(rKey));
        }
    }
}
