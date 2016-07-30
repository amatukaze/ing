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
