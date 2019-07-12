using Sakuno.ING.Game.Models;
using Sakuno.ING.Shell;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.Views.UWP.Homeport
{
    [ExportView("ActiveQuests")]
    public sealed partial class ActiveQuestsView : UserControl
    {
        private readonly QuestManager Manager;
        public ActiveQuestsView(NavalBase navalBase)
        {
            Manager = navalBase.Quests;
            InitializeComponent();
        }
    }
}
