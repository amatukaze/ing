using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;

namespace Sakuno.KanColle.Amatsukaze.Models.Rewards
{
    class QuestEquipmentReward : QuestReward
    {
        public EquipmentInfo Equipment { get; }

        public QuestEquipmentReward(int rpEquipmentID, int rpCount) : base(rpCount)
        {
            Equipment = KanColleGame.Current.MasterInfo.Equipment[rpEquipmentID];
            Quantity = rpCount;
        }
    }
}
