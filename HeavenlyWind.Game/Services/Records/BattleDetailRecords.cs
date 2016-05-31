using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Models.Events;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Records
{
    public class BattleDetailRecords : RecordsGroup
    {
        enum ParticipantFleetType { Main, Escort, SupportFire }

        public override string GroupName => "battle_detail";
        public override int Version => 3;

        string r_Filename;
        SQLiteConnection r_Connection;

        long? r_CurrentBattleID;

        internal BattleDetailRecords(SQLiteConnection rpConnection, int rpUserID) : base(rpConnection)
        {
            r_Filename = new FileInfo($"Records\\{rpUserID}_Battle.db").FullName;
            r_Connection = new SQLiteConnection($@"Data Source={r_Filename};Page Size=8192").OpenAndReturn();

            var rSortieFirstStageApis = new[]
            {
                "api_req_sortie/battle",
                "api_req_battle_midnight/sp_midnight",
                "api_req_sortie/airbattle",
                "api_req_sortie/ld_airbattle",
                "api_req_combined_battle/airbattle",
                "api_req_combined_battle/battle",
                "api_req_combined_battle/battle_water",
                "api_req_combined_battle/sp_midnight",
                "api_req_combined_battle/ld_airbattle",
            };
            DisposableObjects.Add(SessionService.Instance.Subscribe(rSortieFirstStageApis, ProcessSortieFirstStage));
            DisposableObjects.Add(SessionService.Instance.Subscribe("api_req_practice/battle", ProcessPracticeFirstStage));

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

        protected override void CreateTable()
        {
            using (var rCommand = r_Connection.CreateCommand())
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
                    "fuel INTEGER NOT NULL, " +
                    "bullet INTEGER NOT NULL, " +
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
                    "FOREIGN KEY(battle, participant) REFERENCES participant(battle, id)) WITHOUT ROWID;" +

                "CREATE TABLE IF NOT EXISTS participant_heavily_damaged(" +
                    "battle INTEGER NOT NULL REFERENCES battle(id), " +
                    "id INTEGER NOT NULL, " +
                    "PRIMARY KEY(battle, id), " +
                    "FOREIGN KEY(battle, id) REFERENCES participant(battle, id)) WITHOUT ROWID;" +

                "CREATE TABLE IF NOT EXISTS practice_opponent(" +
                    "id INTEGER PRIMARY KEY NOT NULL, " +
                    "name TEXT NOT NULL);" +
                "CREATE TABLE IF NOT EXISTS practice_opponent_comment(" +
                    "id INTEGER PRIMARY KEY NOT NULL, " +
                    "comment TEXT NOT NULL);" +
                "CREATE TABLE IF NOT EXISTS practice_opponent_fleet(" +
                    "id INTEGER PRIMARY KEY NOT NULL, " +
                    "name TEXT NOT NULL);" +
                "CREATE TABLE IF NOT EXISTS practice(" +
                    "id INTEGER PRIMARY KEY NOT NULL, " +
                    "opponent INTEGER NOT NULL, " +
                    "opponent_level INTEGER NOT NULL, " +
                    "opponent_experience INTEGER NOT NULL, " +
                    "opponent_rank INTEGER NOT NULL, " +
                    "opponent_comment INTEGER NOT NULL REFERENCES practice_opponent_comment(id), " +
                    "opponent_fleet INTEGER NOT NULL REFERENCES practice_opponent_fleet(id), " +
                    "rank INTEGER); " +

                "CREATE VIEW IF NOT EXISTS participant_hd_view AS " +
                    "SELECT participant_heavily_damaged.battle AS battle, group_concat(participant.ship) AS ships FROM participant_heavily_damaged " +
                    "JOIN participant participant ON participant_heavily_damaged.battle = participant.battle AND participant_heavily_damaged.id = participant.id " +
                    "GROUP BY participant_heavily_damaged.battle;";

                rCommand.ExecuteNonQuery();
            }

            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "ATTACH @battle_detail_db AS battle_detail";
                rCommand.Parameters.AddWithValue("@battle_detail_db", r_Filename);
                rCommand.ExecuteNonQuery();
            }

            if (r_Connection != null)
            {
                r_Connection.Dispose();
                r_Connection = null;
            }
        }

        protected override void UpgradeFromOldVersion(int rpOldVersion)
        {
            if (rpOldVersion == 1)
            {
                using (var rTransaction = r_Connection.BeginTransaction())
                using (var rCommand = r_Connection.CreateCommand())
                {
                    rCommand.CommandText =
                        "ALTER TABLE participant RENAME TO participant_old;" +

                        "CREATE TABLE IF NOT EXISTS participant(battle INTEGER NOT NULL REFERENCES battle(id), id INTEGER NOT NULL, ship INTEGER NOT NULL, level INTEGER NOT NULL, condition INTEGER NOT NULL, fuel INTEGER NOT NULL, bullet INTEGER NOT NULL, firepower INTEGER NOT NULL, torpedo INTEGER NOT NULL, aa INTEGER NOT NULL, armor INTEGER NOT NULL, evasion INTEGER NOT NULL, asw INTEGER NOT NULL, los INTEGER NOT NULL, luck INTEGER NOT NULL, range INTEGER NOT NULL, PRIMARY KEY(battle, id)) WITHOUT ROWID;" +

                        "INSERT INTO participant(battle, id, ship, level, condition, fuel, bullet, firepower, torpedo, aa, armor, evasion, asw, los, luck, range) SELECT battle, id, ship, level, condition, -1 AS fuel, -1 AS bullet, firepower, torpedo, aa, armor, evasion, asw, los, luck, range FROM participant_old;" +

                        "DROP TABLE participant_old;";

                    rCommand.ExecuteNonQuery();

                    rTransaction.Commit();
                }
            }
        }

        byte[] CompressJson(JToken rpJsonToken)
        {
            using (var rMemoryStream = new MemoryStream())
            {
                using (var rCompressStream = new DeflateStream(rMemoryStream, CompressionMode.Compress))
                using (var rStreamWriter = new StreamWriter(rCompressStream))
                using (var rJsonTextWriter = new JsonTextWriter(rStreamWriter))
                    rpJsonToken.WriteTo(rJsonTextWriter);

                return rMemoryStream.ToArray();
            }
        }

        void ProcessSortieFirstStage(ApiData rpData)
        {
            var rSortie = SortieInfo.Current;
            r_CurrentBattleID = BattleInfo.Current.ID;

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

                var rData = rpData.Data as RawDay;
                if (rData != null && rData.SupportingFireType != 0)
                {
                    var rSupportFire = rData.SupportingFire;
                    var rFleetID = (rSupportFire.SupportShelling?.FleetID ?? rSupportFire.AerialSupport?.FleetID).Value;
                    ProcessParticipantFleet(rCommandTextBuilder, KanColleGame.Current.Port.Fleets[rFleetID], ParticipantFleetType.SupportFire);
                }

                rCommand.CommandText = rCommandTextBuilder.ToString();
                rCommand.ExecuteNonQuery();

                rTransaction.Commit();
            }
        }
        void ProcessPracticeFirstStage(ApiData rpData)
        {
            var rParticipantFleet = KanColleGame.Current.Port.Fleets[int.Parse(rpData.Parameters["api_deck_id"])];
            var rPractice = (PracticeInfo)KanColleGame.Current.Sortie;
            var rOpponent = rPractice.Opponent;
            r_CurrentBattleID = rPractice.Battle.ID;

            using (var rTransaction = Connection.BeginTransaction())
            using (var rCommand = Connection.CreateCommand())
            {
                var rCommandTextBuilder = new StringBuilder(1024);
                rCommandTextBuilder.Append("INSERT OR IGNORE INTO practice_opponent(id, name) VALUES(@opponent_id, @opponent_name);" +
                    "INSERT OR IGNORE INTO practice_opponent_comment(id, comment) VALUES(@opponent_comment_id, @opponent_coment);" +
                    "INSERT OR IGNORE INTO practice_opponent_fleet(id, name) VALUES(@opponent_fleet_name_id, @opponent_fleet_name);" +
                    "INSERT INTO practice(id, opponent, opponent_level, opponent_experience, opponent_rank, opponent_comment, opponent_fleet) VALUES(@battle_id, @opponent_id, @opponent_level, @opponent_experience, @opponent_rank, @opponent_comment_id, @opponent_fleet_name_id);" +
                    "INSERT INTO battle_detail.battle(id, first) VALUES(@battle_id, @first);");
                rCommand.Parameters.AddWithValue("@opponent_id", rOpponent.RawData.ID);
                rCommand.Parameters.AddWithValue("@opponent_name", rOpponent.Name);
                rCommand.Parameters.AddWithValue("@opponent_comment_id", rOpponent.RawData.CommentID ?? -1);
                rCommand.Parameters.AddWithValue("@opponent_coment", rOpponent.Comment);
                rCommand.Parameters.AddWithValue("@opponent_fleet_name_id", rOpponent.RawData.FleetNameID ?? -1);
                rCommand.Parameters.AddWithValue("@opponent_fleet_name", rOpponent.FleetName);
                rCommand.Parameters.AddWithValue("@opponent_level", rOpponent.Level);
                rCommand.Parameters.AddWithValue("@opponent_experience", rOpponent.RawData.Experience[0]);
                rCommand.Parameters.AddWithValue("@opponent_rank", (int)rOpponent.Rank);
                rCommand.Parameters.AddWithValue("@battle_id", r_CurrentBattleID.Value);
                rCommand.Parameters.AddWithValue("@first", CompressJson(rpData.Json["api_data"]));

                ProcessParticipantFleet(rCommandTextBuilder, rParticipantFleet, ParticipantFleetType.Main);

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

            using (var rTransaction = Connection.BeginTransaction())
            {
                using (var rCommand = Connection.CreateCommand())
                {
                    rCommand.CommandText = "UPDATE battle_detail.battle SET result = @result WHERE id = @id;";
                    rCommand.Parameters.AddWithValue("@id", r_CurrentBattleID.Value);
                    rCommand.Parameters.AddWithValue("@result", CompressJson(rpData.Json["api_data"]));

                    if (rpData.Api == "api_req_practice/battle_result")
                    {
                        rCommand.CommandText += "UPDATE battle_detail.practice SET rank = @rank WHERE id = @id;";
                        rCommand.Parameters.AddWithValue("@rank", (int)((RawBattleResult)rpData.Data).Rank);
                    }

                    rCommand.ExecuteNonQuery();
                }

                var rStage = BattleInfo.Current.CurrentStage;
                ProcessHeavilyDamagedShip(rStage.FriendMain, ParticipantFleetType.Main);
                if (rStage.FriendEscort != null)
                    ProcessHeavilyDamagedShip(rStage.FriendEscort, ParticipantFleetType.Escort);

                rTransaction.Commit();
            }

            r_CurrentBattleID = null;
        }
        void ProcessHeavilyDamagedShip(IList<BattleParticipantSnapshot> rpParticipants, ParticipantFleetType rpType)
        {
            for (var i = 0; i < rpParticipants.Count; i++)
            {
                var rID = (int)rpType * 6 + i;
                var rState = rpParticipants[i].State;
                if (rState == BattleParticipantState.HeavilyDamaged || rState == BattleParticipantState.Sunk)
                    using (var rCommand = Connection.CreateCommand())
                    {
                        rCommand.CommandText = "INSERT INTO battle_detail.participant_heavily_damaged(battle, id) VALUES(@battle_id, @id);";
                        rCommand.Parameters.AddWithValue("@battle_id", r_CurrentBattleID.Value);
                        rCommand.Parameters.AddWithValue("@id", rID);

                        rCommand.ExecuteNonQuery();
                    }
            }
        }

        void ProcessParticipantFleet(StringBuilder rpCommandTextBuilder, Fleet rpFleet, ParticipantFleetType rpType)
        {
            var rFleetID = (int)rpType;

            rpCommandTextBuilder.Append($"INSERT OR IGNORE INTO battle_detail.participant_fleet_name(id, name) VALUES({rpFleet.RawData.NameID ?? -rpFleet.ID}, '{rpFleet.Name}');");
            rpCommandTextBuilder.Append($"INSERT INTO battle_detail.participant_fleet(battle, id, name) VALUES(@battle_id, {rFleetID}, {rpFleet.RawData.NameID});");

            for (var i = 0; i < rpFleet.Ships.Count; i++)
            {
                var rShip = rpFleet.Ships[i];
                var rID = rFleetID * 6 + i;

                rpCommandTextBuilder.Append("INSERT INTO battle_detail.participant(battle, id, ship, level, condition, fuel, bullet, firepower, torpedo, aa, armor, evasion, asw, los, luck, range) ");
                rpCommandTextBuilder.Append($"VALUES(@battle_id, {rID}, {rShip.Info.ID}, {rShip.Level}, {rShip.Condition}, {rShip.Fuel.Current}, {rShip.Bullet.Current}, {rShip.Status.FirepowerBase.Current}, {rShip.Status.TorpedoBase.Current}, {rShip.Status.AABase.Current}, {rShip.Status.ArmorBase.Current}, {rShip.Status.Evasion}, {rShip.Status.ASW}, {rShip.Status.LoS}, {rShip.Status.Luck}, {rShip.RawData.Range});");

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
