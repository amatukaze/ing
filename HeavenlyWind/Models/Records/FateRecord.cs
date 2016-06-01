using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using System;
using System.Data.SQLite;

namespace Sakuno.KanColle.Amatsukaze.Models.Records
{
    class FateRecord : ModelBase
    {
        public string Time { get; }

        public int ID { get; }

        public bool IsEquipment { get; }
        public ShipInfo Ship { get; }
        public EquipmentInfo Equipment { get; }

        public int Level { get; }
        public int Proficiency { get; }

        public Fate Fate { get; }

        internal FateRecord(SQLiteDataReader rpReader)
        {
            Time = DateTimeUtil.FromUnixTime(Convert.ToUInt64(rpReader["time"])).LocalDateTime.ToString();

            var rMasterInfo = KanColleGame.Current.MasterInfo;
            var rMasterID = Convert.ToInt32(rpReader["master_id"]);
            IsEquipment = Convert.ToBoolean(rpReader["is_equipment"]);
            if (!IsEquipment)
                Ship = rMasterInfo.Ships[rMasterID];
            else
                Equipment = rMasterInfo.Equipment[rMasterID];

            Level = Convert.ToInt32(rpReader["level"]);
            Proficiency = Convert.ToInt32(rpReader["proficiency"]);

            Fate = (Fate)Convert.ToInt32(rpReader["fate"]);
        }
    }
}
