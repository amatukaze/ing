using Sakuno.KanColle.Amatsukaze.Game.Models;

namespace Sakuno.KanColle.Amatsukaze.Models.Rewards
{
    class QuestMaterialReward : QuestReward
    {
        public MaterialType Type { get; }

        public QuestMaterialReward(MaterialType rpType, int rpAmount) : base(rpAmount)
        {
            Type = rpType;
            Quantity = rpAmount;
        }
    }
}
