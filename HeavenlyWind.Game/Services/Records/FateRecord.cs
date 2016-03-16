using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Records
{
    public class FateRecord : RecordBase
    {
        public override string GroupName => "fate";

        internal FateRecord(SQLiteConnection rpConnection) : base(rpConnection)
        {
            var rBattleResultApis = new[]
            {
                "api_req_sortie/battleresult",
                "api_req_combined_battle/battleresult",
            };
            DisposableObjects.Add(SessionService.Instance.Subscribe(rBattleResultApis, ProcessBattleResult));
        }

        protected override void CreateTable()
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText =
                    "CREATE TABLE IF NOT EXISTS ship_fate(" +
                        "id INTEGER PRIMARY KEY NOT NULL, " +
                        "ship INTEGER NOT NULL, " +
                        "level INTEGER NOT NULL, " +
                        "time INTEGER NOT NULL, " +
                        "fate INTEGER NOT NULL);" +

                    "CREATE TABLE IF NOT EXISTS equipment_fate(" +
                        "id INTEGER PRIMARY KEY NOT NULL, " +
                        "equipment INTEGER NOT NULL, " +
                        "level INTEGER NOT NULL, " +
                        "proficiency INTEGER NOT NULL, " +
                        "time INTEGER NOT NULL, " +
                        "fate INTEGER NOT NULL);";

                rCommand.ExecuteNonQuery();
            }
        }

        internal void AddShipFate(Ship rpShip, Fate rpFate)
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "INSERT OR IGNORE INTO ship_fate(id, ship, level, time, fate) VALUES(@id, @ship, @level, strftime('%s', 'now'), @fate);";
                rCommand.Parameters.AddWithValue("@id", rpShip.ID);
                rCommand.Parameters.AddWithValue("@ship", rpShip.Info.ID);
                rCommand.Parameters.AddWithValue("@level", rpShip.Level);
                rCommand.Parameters.AddWithValue("@fate", (int)rpFate);

                rCommand.ExecuteNonQuery();
            }
        }
        internal void AddShipFate(IEnumerable<Ship> rpShips, Fate rpFate)
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "INSERT OR IGNORE INTO ship_fate(id, ship, level, time, fate) VALUES" + rpShips.Select(r => $"({r.ID}, {r.Info.ID}, {r.Level}, strftime('%s', 'now'), @fate)").Join(", ") + ";";
                rCommand.Parameters.AddWithValue("@fate", (int)rpFate);

                rCommand.ExecuteNonQuery();
            }
        }

        internal void AddEquipmentFate(Equipment rpEquipment, Fate rpFate)
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "INSERT OR IGNORE INTO equipment_fate(id, equipment, level, proficiency, time, fate) VALUES(@id, @equipment, @level, @proficiency, strftime('%s', 'now'), @fate);";
                rCommand.Parameters.AddWithValue("@id", rpEquipment.ID);
                rCommand.Parameters.AddWithValue("@equipment", rpEquipment.Info.ID);
                rCommand.Parameters.AddWithValue("@level", rpEquipment.Level == 0);
                rCommand.Parameters.AddWithValue("@proficiency", rpEquipment.Proficiency == 0);
                rCommand.Parameters.AddWithValue("@fate", (int)rpFate);

                rCommand.ExecuteNonQuery();
            }
        }
        internal void AddEquipmentFate(IEnumerable<Equipment> rpEquipment, Fate rpFate)
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "INSERT OR IGNORE INTO equipment_fate(id, equipment, level, proficiency, time, fate) VALUES" + rpEquipment.Select(r => $"({r.ID}, {r.Info.ID}, {r.Level}, {r.Proficiency}, strftime('%s', 'now'), @fate)").Join(", ") + ";";
                rCommand.Parameters.AddWithValue("@fate", (int)rpFate);

                rCommand.ExecuteNonQuery();
            }
        }

        void ProcessBattleResult(ApiData rpData)
        {
            var rBattle = BattleInfo.Current;
            var rCurrentStage = rBattle.CurrentStage;

            IEnumerable<BattleParticipantSnapshot> rParticipants = rCurrentStage.FriendMain;
            if (rCurrentStage.FriendEscort != null)
                rParticipants = rParticipants.Concat(rCurrentStage.FriendEscort);

            var rSunkShips = rParticipants.Where(r => r.State == BattleParticipantState.Damaged).Select(r => ((FriendShip)r.Participant).Ship).ToArray();
            if (rSunkShips.Length == 0)
                return;

            var rSunkEquipment = rSunkShips.SelectMany(r => r.EquipedEquipment);

            AddShipFate(rSunkShips, Fate.Sunk);
            AddEquipmentFate(rSunkEquipment, Fate.Sunk);
        }

        public async Task<List<RecordItem>> LoadRecordsAsync()
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = @"SELECT id, ship AS master_id, level, 0 AS proficiency, time, fate, 0 AS is_equipment FROM ship_fate
UNION
SELECT id, equipment AS master_id, level, proficiency, time, fate, 1 AS is_equipment FROM equipment_fate
ORDER BY time DESC;";

                using (var rReader = await rCommand.ExecuteReaderAsync())
                {
                    var rResult = new List<RecordItem>(rReader.VisibleFieldCount);

                    while (rReader.Read())
                        rResult.Add(new RecordItem(rReader));

                    return rResult;
                }
            }
        }

        public class RecordItem
        {
            public string Time { get; }

            public int ID { get; }

            public bool IsEquipment { get; }
            public ShipInfo Ship { get; }
            public EquipmentInfo Equipment { get; }

            public int Level { get; }
            public int Proficiency { get; }

            public Fate Fate { get; }

            internal RecordItem(DbDataReader rpReader)
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
}
