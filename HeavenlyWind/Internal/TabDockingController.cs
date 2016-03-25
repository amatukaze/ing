using Sakuno.UserInterface.Controls;
using Sakuno.UserInterface.Controls.Docking;

namespace Sakuno.KanColle.Amatsukaze.Internal
{
    class TabDockingController : ITabDockingController
    {
        public AdvancedTabControl CreateHost(AdvancedTabControl rpSourceTabControl, string rpSourcePartition)
        {
            return new AdvancedTabControl()
            {
                DataContext = rpSourceTabControl.DataContext,
                ItemTemplate = rpSourceTabControl.ItemTemplate,
                TabController = new TabController() { TearOffController = rpSourceTabControl.TabController.TearOffController, Partition = rpSourceTabControl.TabController.Partition },
            };
        }
    }
}
