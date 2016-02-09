using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Events;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using System;
using System.Data.SQLite;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Records
{
    public class BattleDetailRecord : RecordBase
    {
        enum ParticipantFleetType { Main, Escort, SupportFire }

        string r_Filename;

        long? r_CurrentBattleID;

        internal BattleDetailRecord(SQLiteConnection rpConnection, int rpUserID) : base(rpConnection)
        {
            r_Filename = new FileInfo($"Records\\{rpUserID}_Battle.db").FullName;

            var rSortieFirstStageApis = new[]
            {
                "api_req_sortie/battle",
                "api_req_battle_midnight/sp_midnight",
                "api_req_sortie/airbattle",
                "api_req_combined_battle/airbattle",
                "api_req_combined_battle/battle",
                "api_req_combined_battle/battle_water",
                "api_req_combined_battle/sp_midnight",
            };
            DisposableObjects.Add(SessionService.Instance.Subscribe(rSortieFirstStageApis, ProcessSortieFirstStage));

            var rSecondStageApis = new[]
            {
                "api_req_practice/midnight_battle",
                "api_req_battle_midnight/battle",
                "api_req_combined_battle/midnight_battle",
                "api_req_practice/midnight_battle",
            };
            DisposableObjects.Add(SessionService.Instance.Subscribe(rSecondStageApis, ProcessSecondStage));

            var rBattleResultApis = new[]
            {
                "api_req_sortie/battleresult",
                "api_req_combined_battle/battleresult",
                "api_req_practice/battle_result",
            };
            DisposableObjects.Add(SessionService.Instance.Subscribe(rBattleResultApis, ProcessResult));
        }

        public override string GroupName => "battle_detail";

        protected override void CreateTable()
        {
            using (var rConnection = new SQLiteConnection($@"Data Source={r_Filename};Page Size=8192").OpenAndReturn())
            using (var rCommand = rConnection.CreateCommand())
            {
                rCommand.CommandText = "CREATE TABLE IF NOT EXISTS battle(" +
                    "id INTEGER PRIMARY KEY NOT NULL, " +
                    "first BLOB NOT NULL, " +
                    "second BLOB, " +
                    "result BLOB);" +

                "CREATE TABLE IF NOT EXISTS participant_fleet_name(" +
                    "id INTEGER PRIMARY KEY, " +
                    "name TEXT NOT NULL);" +
                "CREATE TABLE IF NOT EXISTS participant_fleet(" +
                    "battle INTEGER NOT NULL REFERENCES battle(id), " +
                    "id INTEGER NOT NULL, " +
                    "name INTEGER NOT NULL REFERENCES participant_fleet_name(id), " +
                    "PRIMARY KEY(battle, id)) WITHOUT ROWID;" +

                "CREATE TABLE IF NOT EXISTS participant(" +
                    "battle INTEGER NOT NULL REFERENCES battle(id), " +
                    "id INTEGER NOT NULL, " +
                    "ship INTEGER NOT NULL, " +
                    "level INTEGER NOT NULL, " +
                    "condition INTEGER NOT NULL, " +
                    "firepower INTEGER NOT NULL, " +
                    "torpedo INTEGER NOT NULL, " +
                    "aa INTEGER NOT NULL, " +
                    "armor INTEGER NOT NULL, " +
                    "evasion INTEGER NOT NULL, " +
                    "asw INTEGER NOT NULL, " +
                    "los INTEGER NOT NULL, " +
                    "luck INTEGER NOT NULL, " +
                    "range INTEGER NOT NULL, " +
                    "PRIMARY KEY(battle, id)) WITHOUT ROWID;" +
                "CREATE TABLE IF NOT EXISTS participant_slot(" +
                    "battle INTEGER NOT NULL REFERENCES battle(id), " +
                    "participant INTEGER NOT NULL, " +
                    "id INTEGER NOT NULL, " +
                    "equipment INTEGER NOT NULL, " +
                    "level INTEGER NOT NULL, " +
                    "plane_count INTEGER NOT NULL, " +
                    "PRIMARY KEY(battle, participant, id), " +
                    "FOREIGN KEY(battle, participant) REFERENCES participant(battle, id)) WITHOUT ROWID;";

                rCommand.ExecuteNonQuery();
            }

            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "ATTACH @battle_detail_db AS battle_detail";
                rCommand.Parameters.AddWithValue("@battle_detail_db", r_Filename);
                rCommand.ExecuteNonQuery();
            }
        }

        byte[] CompressJson(JToken rpJsonToken)
        {
            using (var rMemoryStream = new MemoryStream())
            using (var rCompressStream = new DeflateStream(rMemoryStream, CompressionMode.Compress))
            using (var rStreamWriter = new StreamWriter(rCompressStream))
            using (var rJsonTextWriter = new JsonTextWriter(rStreamWriter))
            {
                rpJsonToken.WriteTo(rJsonTextWriter);

                return rMemoryStream.ToArray();
            }
        }

        void ProcessSortieFirstStage(ApiData rpData)
        {
            var rSortie = KanColleGame.Current.Sortie;
            var rData = (RawDay)rpData.Data;
            r_CurrentBattleID = ((BattleEvent)rSortie.Node.Event).Battle.ID;

            using (var rTransaction = Connection.BeginTransaction())
            using (var rCommand = Connection.CreateCommand())
            {
                var rCommandTextBuilder = new StringBuilder(1024);
                rCommandTextBuilder.Append("INSERT INTO battle_detail.battle(id, first) VALUES(@battle_id, @first);");
                rCommand.Parameters.AddWithValue("@battle_id", r_CurrentBattleID.Value);
                rCommand.Parameters.AddWithValue("@first", CompressJson(rpData.Json["api_data"]));

                ProcessParticipantFleet(rCommandTextBuilder, rSortie.Fleet, ParticipantFleetType.Main);
                if (rSortie.EscortFleet != null)
                    ProcessParticipantFleet(rCommandTextBuilder, rSortie.EscortFleet, ParticipantFleetType.Escort);
                if (rData.SupportingFireType != 0)
                {
                    var rSupportFire = rData.SupportingFire;
                    var rFleetID = (rSupportFire.Shelling?.FleetID ?? rSupportFire.AerialCombat?.FleetID).Value;
                    ProcessParticipantFleet(rCommandTextBuilder, KanColleGame.Current.Port.Fleets[rFleetID], ParticipantFleetType.SupportFire);
                }

                rCommand.CommandText = rCommandTextBuilder.ToString();
                rCommand.ExecuteNonQuery();

                rTransaction.Commit();
            }
        }
        void ProcessSecondStage(ApiData rpData)
        {
            if (!r_CurrentBattleID.HasValue)
                return;

            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "UPDATE battle_detail.battle SET second = @second WHERE id = @id;";
                rCommand.Parameters.AddWithValue("@id", r_CurrentBattleID.Value);
                rCommand.Parameters.AddWithValue("@second", CompressJson(rpData.Json["api_data"]));

                rCommand.ExecuteNonQuery();
            }
        }
        void ProcessResult(ApiData rpData)
        {
            if (!r_CurrentBattleID.HasValue)
                return;

            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "UPDATE battle_detail.battle SET result = @result WHERE id = @id;";
                rCommand.Parameters.AddWithValue("@id", r_CurrentBattleID.Value);
                rCommand.Parameters.AddWithValue("@result", CompressJson(rpData.Json["api_data"]));

                rCommand.ExecuteNonQuery();
            }

            r_CurrentBattleID = null;
        }

        void ProcessParticipantFleet(StringBuilder rpCommandTextBuilder, Fleet rpFleet, ParticipantFleetType rpType)
        {
            var rFleetID = (int)rpType;

            rpCommandTextBuilder.Append($"INSERT OR IGNORE INTO battle_detail.participant_fleet_name(id, name) VALUES({rpFleet.RawData.NameID}, '{rpFleet.Name}');");
            rpCommandTextBuilder.Append($"INSERT INTO battle_detail.participant_fleet(battle, id, name) VALUES(@battle_id, {rFleetID}, {rpFleet.RawData.NameID});");

            for (var i = 0; i < rpFleet.Ships.Count; i++)
            {
                var rShip = rpFleet.Ships[i];
                var rID = rFleetID * 6 + i;

                rpCommandTextBuilder.Append("INSERT INTO battle_detail.participant(battle, id, ship, level, condition, firepower, torpedo, aa, armor, evasion, asw, los, luck, range) ");
                rpCommandTextBuilder.Append($"VALUES(@battle_id, {rID}, {rShip.Info.ID}, {rShip.Level}, {rShip.Condition}, {rShip.Status.FirepowerBase.Current}, {rShip.Status.TorpedoBase.Current}, {rShip.Status.AABase.Current}, {rShip.Status.ArmorBase.Current}, {rShip.Status.Evasion}, {rShip.Status.ASW}, {rShip.Status.LoS}, {rShip.Status.Luck}, {rShip.RawData.Range});");

                for (var j = 0; j < rShip.Slots.Count; j++)
                {
                    var rSlot = rShip.Slots[j];
                    if (!rSlot.HasEquipment)
                        break;

                    var rLevel = Math.Max(rSlot.Equipment.Level, rSlot.Equipment.Proficiency);
                    rpCommandTextBuilder.Append($"INSERT INTO battle_detail.participant_slot(battle, participant, id, equipment, level, plane_count) VALUES(@battle_id, {rID}, {j}, {rSlot.Equipment.Info.ID}, {rLevel}, {rSlot.PlaneCount});");
                }

                if (rShip.ExtraSlot != null)
                    rpCommandTextBuilder.Append($"INSERT INTO battle_detail.participant_slot(battle, participant, id, equipment, level, plane_count) VALUES(@battle_id, {rID}, -1, {rShip.ExtraSlot.Equipment.Info.ID}, 0, 0);");
            }
        }
    }
}
