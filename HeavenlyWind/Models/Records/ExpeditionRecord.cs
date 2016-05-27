using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using System;
using System.Data.Common;

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

        internal ExpeditionRecord(DbDataReader rpReader)
        {
            Time = DateTimeUtil.FromUnixTime(Convert.ToUInt64(rpReader["time"])).LocalDateTime.ToString();

            ExpeditionInfo rExpedition;
            if (KanColleGame.Current.MasterInfo.Expeditions.TryGetValue(Convert.ToInt32(rpReader["expedition"]), out rExpedition))
                Expedition = rExpedition;
            else
                Expedition = ExpeditionInfo.Dummy;

            Result = (ExpeditionResult)Convert.ToInt32(rpReader["result"]);

            if (Result == ExpeditionResult.Failure)
                return;

            Fuel = Convert.ToInt32(rpReader["fuel"]);
            Bullet = Convert.ToInt32(rpReader["bullet"]);
            Steel = Convert.ToInt32(rpReader["steel"]);
            Bauxite = Convert.ToInt32(rpReader["bauxite"]);

            var rItem1 = rpReader["item1"];
            if (rItem1 != DBNull.Value)
            {
                var rItem1ID = Convert.ToInt32(rItem1);
                if (rItem1ID != -1)
                    Item1 = KanColleGame.Current.MasterInfo.Items[rItem1ID];
                Item1Count = Convert.ToInt32(rpReader["item1_count"]);
            }
            var rItem2 = rpReader["item2"];
            if (rItem2 != DBNull.Value)
            {
                var rItem2ID = Convert.ToInt32(rItem2);
                if (rItem2ID != -1)
                    Item2 = KanColleGame.Current.MasterInfo.Items[rItem2ID];
                Item2Count = Convert.ToInt32(rpReader["item2_count"]);
            }
        }
    }
}
