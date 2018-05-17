using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using System.Data.SQLite;

namespace Sakuno.KanColle.Amatsukaze.Models.Records
{
    class FateRecord : ModelBase, IRecordID
    {
        public long ID { get; set; }
        public string Time => DateTimeUtil.FromUnixTime(ID).LocalDateTime.ToString();

        public bool IsEquipment { get; }
        public ShipInfo Ship { get; }
        public EquipmentInfo Equipment { get; }

        public int Level { get; }
        public int Proficiency { get; }

        public Fate Fate { get; }

        internal FateRecord(SQLiteDataReader rpReader)
        {
            ID = rpReader.GetInt64("time");

            var rMasterInfo = KanColleGame.Current.MasterInfo;
            var rMasterID = rpReader.GetInt32("master_id");
            IsEquipment = rpReader.GetBoolean("is_equipment");
            if (!IsEquipment)
                Ship = rMasterInfo.Ships[rMasterID];
            else
                Equipment = rMasterInfo.Equipment[rMasterID];

            Level = rpReader.GetInt32("level");
            Proficiency = rpReader.GetInt32("proficiency");

            Fate = (Fate)rpReader.GetInt32("fate");
        }
    }
}
