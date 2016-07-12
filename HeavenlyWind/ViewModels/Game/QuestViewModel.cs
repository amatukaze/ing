using Sakuno.KanColle.Amatsukaze.Game.Models;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    public class QuestViewModel : RawDataWrapper<Quest>, IID
    {
        public Quest Source => RawData;

        public int ID => RawData.ID;

        internal QuestViewModel(Quest rpQuest) : base(rpQuest) { }
    }
}
