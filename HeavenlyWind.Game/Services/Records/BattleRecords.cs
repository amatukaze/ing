using Sakuno.KanColle.Amatsukaze.Game.Models.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Records
{
    public class BattleRecords : RecordsGroup
    {
        public override string GroupName => "battle";

        public override int Version => 4;

        internal BattleRecords(SQLiteConnection rpConnection) : base(rpConnection)
        {
            var rBattleResultApis = new[]
            {
                "api_req_sortie/battleresult",
                "api_req_combined_battle/battleresult",
            };
            DisposableObjects.Add(ApiService.Subscribe(rBattleResultApis, r => ProcessResult((RawBattleResult)r.Data)));
        }

        protected override void UpgradeFromOldVersionPreprocessStep(int rpOldVersion)
        {
            if (rpOldVersion < 3)
                using (var rCommand = Connection.CreateCommand())
                {
                    rCommand.CommandText = "DROP TABLE IF EXISTS battle_count;";

                    rCommand.ExecuteNonQuery();
                }

            if (rpOldVersion < 4)
                using (var rCommand = Connection.CreateCommand())
                {
                    rCommand.CommandText =
                        "ALTER TABLE battle ADD COLUMN mvp INTEGER; " +
                        "ALTER TABLE battle ADD COLUMN mvp_escort INTEGER;";

                    rCommand.ExecuteNonQuery();
                }
        }
        protected override void CreateTable()
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText =
                    "CREATE TABLE IF NOT EXISTS battle(" +
                        "id INTEGER PRIMARY KEY, " +
                        "rank INTEGER, " +
                        "dropped_ship INTEGER, " +
                        "mvp INTEGER, " +
                        "mvp_escort INTEGER);" +

                    "CREATE TABLE IF NOT EXISTS battle_dropped_item(" +
                        "id INTEGER PRIMARY KEY NOT NULL REFERENCES battle(id), " +
                        "item INTEGER NOT NULL);" +

                    "CREATE TABLE IF NOT EXISTS battle_participant(" +
                        "battle INTEGER NOT NULL REFERENCES battle(id), " +
                        "ship INTEGER NOT NULL, " +
                        "level INTEGER NOT NULL, " +
                        "PRIMARY KEY(battle, ship)) WITHOUT ROWID;";

                rCommand.ExecuteNonQuery();
            }
        }

        void ProcessResult(RawBattleResult rpData)
        {
            var rBattle = BattleInfo.Current;

            var rCommand = Connection.CreateCommand();

            rCommand.CommandText = "INSERT INTO battle(id, rank, dropped_ship) VALUES(@id, @rank, @dropped_ship);";
            rCommand.Parameters.AddWithValue("@id", rBattle.ID);
            rCommand.Parameters.AddWithValue("@rank", (int)rpData.Rank);
            rCommand.Parameters.AddWithValue("@dropped_ship", rpData.DroppedShip?.ID);

            if (rpData.DroppedItem != null)
            {
                rCommand.CommandText += " INSERT INTO battle_dropped_item(id, item) VALUES(@id, @item);";
                rCommand.Parameters.AddWithValue("@item", rpData.DroppedItem.ID);
            }

            if (rpData.MvpShipIndex != -1)
            {
                rCommand.CommandText += " UPDATE battle SET mvp = @mvp WHERE id = @id;";
                rCommand.Parameters.AddWithValue("@mvp", rBattle.Participants.FriendMain[rpData.MvpShipIndex - 1].Info.ID);
            }

            if (rpData.EscortFleetMvpShipIndex.HasValue && rpData.EscortFleetMvpShipIndex.Value != -1)
            {
                rCommand.CommandText += " UPDATE battle SET mvp_escort = @mvp_escort WHERE id = @id;";
                rCommand.Parameters.AddWithValue("@mvp_escort", rBattle.Participants.FriendEscort[rpData.EscortFleetMvpShipIndex.Value - 1].Info.ID);
            }

            rCommand.PostToTransactionQueue();
        }
    }
}
