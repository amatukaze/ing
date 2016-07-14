using System;

namespace Sakuno.KanColle.Amatsukaze.Views.Preferences
{
    /// <summary>
    /// PreferencesWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class PreferencesWindow
    {
        public static PreferencesWindow Instance { get; private set; }

        public PreferencesWindow()
        {
            Instance = this;

            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e)
        {
            Instance = null;

            base.OnClosed(e);
        }
    }
}
