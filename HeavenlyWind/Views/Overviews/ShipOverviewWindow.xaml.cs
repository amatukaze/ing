using Sakuno.KanColle.Amatsukaze.ViewModels.Overviews;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Sakuno.KanColle.Amatsukaze.Views.Overviews
{
    /// <summary>
    /// ShipOverViewWindow.xaml の相互作用ロジック
    /// </summary>
    partial class ShipOverviewWindow
    {
        ShipOverviewViewModel r_ViewModel;

        public ShipOverviewWindow()
        {
            InitializeComponent();

            DataContext = r_ViewModel = new ShipOverviewViewModel();

            r_ListView.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler((s, e)=>
            {
                var rColumnHeader = e.OriginalSource as GridViewColumnHeader;
                if (rColumnHeader == null)
                    return;

                r_ViewModel.Sort(rColumnHeader);
            }));
        }

        protected override void OnClosed(EventArgs e)
        {
            r_ViewModel?.Dispose();

            base.OnClosed(e);
        }
    }
}
