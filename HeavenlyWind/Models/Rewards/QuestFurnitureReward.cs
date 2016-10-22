using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;

namespace Sakuno.KanColle.Amatsukaze.Models.Rewards
{
    class QuestFurnitureReward : QuestReward
    {
        public FurnitureInfo Furniture { get; }

        public QuestFurnitureReward(int rpFurnitureID, int rpCount) : base(rpCount)
        {
            Furniture = KanColleGame.Current.MasterInfo.Furnitures[rpFurnitureID];
            Quantity = rpCount;
        }
    }
}
