using Sakuno.KanColle.Amatsukaze.ViewModels.Tools;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace Sakuno.KanColle.Amatsukaze.Views.Tools
{
    /// <summary>
    /// SessionToolWindow.xaml の相互作用ロジック
    /// </summary>
    partial class SessionToolWindow
    {
        public SessionToolWindow()
        {
            InitializeComponent();
        }

        void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            var rListView = (ListView)sender;
            rListView.ItemContainerGenerator.ItemsChanged += (_, rpArgs) =>
            {
                if (DataContext == null)
                    return;

                var rViewModel = (SessionToolViewModel)DataContext;
                if (rViewModel.AutoScroll && rpArgs.Action == NotifyCollectionChangedAction.Add)
                {
                    var rItem = rListView.Items[rListView.Items.Count - 1];
                    if (rItem == null)
                        return;

                    rListView.ScrollIntoView(rItem);
                }
            };
        }
    }
}
