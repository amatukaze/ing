using System;
using System.Windows;
using Sakuno.UserInterface.Controls;
using Sakuno.UserInterface.Controls.Docking;
using Sakuno.KanColle.Amatsukaze.Views;

namespace Sakuno.KanColle.Amatsukaze.Internal
{
    class TabTearOffController : ITabTearOffController
    {
        public Tuple<AdvancedTabControl, Window> CreateHost(AdvancedTabControl rpSourceTabControl, string rpSourcePartition)
        {
            var rWindow = new TabWindow() { Owner = App.Current.MainWindow };
            rWindow.TabControl.DataContext = rpSourceTabControl.DataContext;
            rWindow.TabControl.TabController = rpSourceTabControl.TabController;
            rWindow.DockableZone.DockingController = new TabDockingController();

            return Tuple.Create(rWindow.TabControl, (Window)rWindow);
        }

        public TabEmptiedAction OnTabEmptied(AdvancedTabControl rpTabControl, Window rpWindow)
            => rpWindow == App.Current.MainWindow ? TabEmptiedAction.DoNothing : TabEmptiedAction.CloseWindow;
    }
}
