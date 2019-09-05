using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.Quests;
using Sakuno.ING.Shell;
using Windows.UI;
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

        public static Color SelectColor(QuestCategory category)
            => category switch
            {
                QuestCategory.Composition => Color.FromArgb(255, 55, 156, 90),
                QuestCategory.Sortie => Color.FromArgb(255, 210, 80, 80),
                QuestCategory.Exercise => Color.FromArgb(255, 105, 172, 75),
                QuestCategory.Expedition => Color.FromArgb(255, 60, 170, 165),
                QuestCategory.Supply => Color.FromArgb(255, 203, 170, 81),
                QuestCategory.Arsenal => Color.FromArgb(255, 116, 77, 64),
                QuestCategory.Mordenization => Color.FromArgb(255, 179, 144, 197),
                _ => Color.FromArgb(255, 135, 135, 135)
            };

        public static bool StateEquals(QuestState left, QuestState right)
            => left == right;

        public static bool ProgressEquals(QuestProgress left, QuestProgress right)
            => left == right;

        public static Color SelectProgressColor(ClampedValue value)
            => value switch
            {
                { IsMaximum: true } => Colors.MediumTurquoise,
                var x when x.Percentage >= 0.8 => Colors.LimeGreen,
                var x when x.Percentage >= 0.5 => Colors.LawnGreen,
                _ => Colors.Orange
            };
    }
}
