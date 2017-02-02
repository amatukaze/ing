using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle
{
    public class AntiAirCutIn : ModelBase
    {
        public int TypeID { get; }

        public EquipmentInfo[] UsedEquipment { get; }

        internal AntiAirCutIn(RawAntiAirCutIn rpData)
        {
            TypeID = rpData.TypeID;

            UsedEquipment = rpData.EquipmentIDs.Select(r => KanColleGame.Current.MasterInfo.Equipment[r]).ToArray();
        }
    }
}
