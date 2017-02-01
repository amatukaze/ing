using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle
{
    public class AntiAirCutIn : ModelBase
    {
        public int TypeID { get; }

        public ShipInfo Triggerer { get; }
        public EquipmentInfo[] UsedEquipment { get; }

        internal AntiAirCutIn(BattleStage rpStage, RawAntiAirCutIn rpData)
        {
            TypeID = rpData.TypeID;

            Triggerer = rpStage.Friend[rpData.TriggererIndex].Participant.Info;
            UsedEquipment = rpData.EquipmentIDs.Select(r => KanColleGame.Current.MasterInfo.Equipment[r]).ToArray();
        }
    }
}
