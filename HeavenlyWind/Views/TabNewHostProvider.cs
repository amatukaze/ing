using Sakuno.UserInterface.Controls;

namespace Sakuno.KanColle.Amatsukaze.Views
{
    class TabNewHostProvider : IDragableTabNewHostProvider
    {
        public DragableTabNewHost CreateNewHost(DragableTabControl rpTabControl)
        {
            var rWindow = new TabWindow() { Owner = App.Current.MainWindow };
            rWindow.TabControl.DataContext = rpTabControl.DataContext;

            return new DragableTabNewHost(rWindow, rWindow.TabControl);
        }
    }
}
