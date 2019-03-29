using Sakuno.ING.Shell;
using Windows.UI.Xaml.Controls;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Views.UWP.Homeport
{
    [ExportView("ActiveQuests")]
    public sealed partial class ActiveQuestsView : UserControl
    {
        private readonly QuestManager Manager;
        public ActiveQuestsView(NavalBase navalBase)
        {
            Manager = navalBase.Quests;
            this.InitializeComponent();
        }
    }
}
