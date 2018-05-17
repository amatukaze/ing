using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;

namespace Sakuno.KanColle.Amatsukaze.Models.Rewards
{
    class QuestItemReward : QuestReward
    {
        public ItemInfo Item { get; }

        public QuestItemReward(int rpItemID, int rpCount) : base(rpCount)
        {
            Item = KanColleGame.Current.MasterInfo.Items[rpItemID];
            Quantity = rpCount;
        }
    }
}
