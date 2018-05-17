using System.Windows;

namespace Sakuno.KanColle.Amatsukaze.Views.Game
{
    /// <summary>
    /// Materials.xaml の相互作用ロジック
    /// </summary>
    partial class Materials
    {
        public static readonly DependencyProperty RegenerationLimitProperty = DependencyProperty.Register(nameof(RegenerationLimit), typeof(int), typeof(Materials),
                new PropertyMetadata(Int32Util.Zero));
        public int RegenerationLimit
        {
            get { return (int)GetValue(RegenerationLimitProperty); }
            set { SetValue(RegenerationLimitProperty, value); }
        }

        public Materials()
        {
            InitializeComponent();
        }
    }
}
