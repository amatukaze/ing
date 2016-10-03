using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using System;
using System.Data.SQLite;

namespace Sakuno.KanColle.Amatsukaze.Models.Records
{
    class ExpeditionRecord : ModelBase
    {
        public string Time { get; }

        public ExpeditionInfo Expedition { get; }

        public ExpeditionResult Result { get; }

        public int? Fuel { get; }
        public int? Bullet { get; }
        public int? Steel { get; }
        public int? Bauxite { get; }

        public ItemInfo Item1 { get; }
        public int? Item1Count { get; }
        public ItemInfo Item2 { get; }
        public int? Item2Count { get; }

        internal ExpeditionRecord(SQLiteDataReader rpReader)
        {
            Time = DateTimeUtil.FromUnixTime(rpReader.GetInt64("time")).LocalDateTime.ToString();

            ExpeditionInfo rExpedition;
            if (KanColleGame.Current.MasterInfo.Expeditions.TryGetValue(rpReader.GetInt32("expedition"), out rExpedition))
                Expedition = rExpedition;
            else
                Expedition = ExpeditionInfo.Dummy;

            Result = (ExpeditionResult)rpReader.GetInt32("result");

            if (Result == ExpeditionResult.Failure)
                return;

            Fuel = rpReader.GetInt32("fuel");
            Bullet = rpReader.GetInt32("bullet");
            Steel = rpReader.GetInt32("steel");
            Bauxite = rpReader.GetInt32("bauxite");

            var rItem1 = rpReader.GetInt32Optional("item1");
            if (rItem1.HasValue)
            {
                var rItem1ID = rItem1.Value;
                if (rItem1ID != -1)
                    Item1 = KanColleGame.Current.MasterInfo.Items[rItem1ID];
                Item1Count = rpReader.GetInt32("item1_count");
            }
            var rItem2 = rpReader.GetInt32Optional("item2");
            if (rItem2.HasValue)
            {
                var rItem2ID = rItem2.Value;
                if (rItem2ID != -1)
                    Item2 = KanColleGame.Current.MasterInfo.Items[rItem2ID];
                Item2Count = rpReader.GetInt32("item2_count");
            }
        }
    }
}
