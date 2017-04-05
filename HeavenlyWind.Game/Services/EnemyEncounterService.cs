using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Sakuno.KanColle.Amatsukaze.Game.Services
{
    public class EnemyEncounterService
    {
        public static EnemyEncounterService Instance { get; } = new EnemyEncounterService();

        SQLiteConnection r_Connection;

        EnemyEncounterService() { }

        public void Initialize()
        {
            FuckTanaka();

            ApiService.SubscribeOnce("api_get_member/require_info", delegate
            {
                using (var rConnection = new SQLiteConnection(@"Data Source=Data\AbyssalFleets.db; Page Size=8192").OpenAndReturn())
                using (var rCommand = rConnection.CreateCommand())
                {
                    rCommand.CommandText =
                        "CREATE TABLE IF NOT EXISTS composition(" +
                            "id INTEGER NOT NULL, " +
                            "position INTEGER NOT NULL, " +
                            "ship INTEGER NOT NULL, " +
                            "PRIMARY KEY(id, position)) WITHOUT ROWID;" +

                        "CREATE TABLE IF NOT EXISTS fleet(" +
                            "map INTEGER NOT NULL, " +
                            "node INTEGER NOT NULL, " +
                            "difficulty INTEGER NOT NULL, " +
                            "formation INTEGER NOT NULL, " +
                            "composition INTEGER NOT NULL REFERENCES composition(id), " +
                            "synced INTEGER NOT NULL, " +
                            "PRIMARY KEY(map, node, difficulty, formation, composition)) WITHOUT ROWID;";

                    rCommand.ExecuteNonQuery();
                }

                r_Connection = CoreDatabase.Connection;
                using (var rCommand = r_Connection.CreateCommand())
                {
                    rCommand.CommandText = "ATTACH @filename AS abyssal;";
                    rCommand.Parameters.AddWithValue("@filename", new FileInfo(@"Data\AbyssalFleets.db").FullName);

                    rCommand.ExecuteNonQuery();
                }
            });

            var rBattleApis = new[]
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
                "api_req_combined_battle/ec_battle",
                "api_req_combined_battle/each_battle",
                "api_req_combined_battle/each_battle_water",
            };
            ApiService.Subscribe(rBattleApis, ProcessAbyssalFleet);
        }

        void FuckTanaka()
        {
            if (!File.Exists(@"Data\AbyssalFleets.db"))
                return;

            using (var rConnection = new SQLiteConnection(@"Data Source=Data\AbyssalFleets.db; Page Size=8192").OpenAndReturn())
            using (var rTransaction = rConnection.BeginTransaction())
            using (var rCommand = rConnection.CreateCommand())
            using (var rCommand2 = rConnection.CreateCommand())
            using (var rCommand3 = rConnection.CreateCommand())
            {
                rCommand.CommandText = "SELECT id FROM composition WHERE position = 0;";
                rCommand2.CommandText = "INSERT OR IGNORE INTO composition(id, position, ship) VALUES(@id, @position, @ship);";
                rCommand3.CommandText =
                    "UPDATE fleet SET composition = @new WHERE composition = @old; " +
                    "DELETE FROM composition WHERE id = @old;";

                var rOldCompositionIds = new List<long>();

                using (var rReader = rCommand.ExecuteReader())
                    while (rReader.Read())
                        rOldCompositionIds.Add(rReader.GetInt64(0));

                rCommand.CommandText = "SELECT ship + 1000 FROM composition WHERE id = @id ORDER BY position;";

                var rShipIds = new List<int>(12);

                foreach (var rOldCompositionId in rOldCompositionIds)
                {
                    rCommand.Parameters.AddWithValue("@id", rOldCompositionId);

                    rShipIds.Clear();
                    using (var rReader = rCommand.ExecuteReader())
                        while (rReader.Read())
                            rShipIds.Add(rReader.GetInt32(0));

                    var rEnemies = rShipIds.ToArray();
                    var rBytes = new byte[rEnemies.Length * sizeof(int)];
                    Buffer.BlockCopy(rEnemies, 0, rBytes, 0, rBytes.Length);

                    long rNewCompositionId;
                    using (var rSHA1 = SHA1.Create())
                    {
                        var rHash = rSHA1.ComputeHash(rBytes);
                        rNewCompositionId = BitConverter.ToInt64(rHash, 0);
                    }

                    rCommand2.Parameters.AddWithValue("@id", rNewCompositionId);
                    for (var i = 0; i < rEnemies.Length; i++)
                    {
                        rCommand2.Parameters.AddWithValue("@position", i);
                        rCommand2.Parameters.AddWithValue("@ship", rShipIds[i]);

                        rCommand2.ExecuteNonQuery();
                    }

                    rCommand3.Parameters.AddWithValue("@old", rOldCompositionId);
                    rCommand3.Parameters.AddWithValue("@new", rNewCompositionId);
                    rCommand3.ExecuteNonQuery();
                }

                rTransaction.Commit();
            }
        }

        void ProcessAbyssalFleet(ApiInfo rpInfo)
        {
            var rData = rpInfo.Data as RawBattleBase;
            var rFormation = rpInfo.Data as IRawFormationAndEngagementForm;
            if (rData == null || rFormation == null)
                return;

            var rEnemies = rData.EnemyShipTypeIDs.Where(r => r != -1).ToArray();

            var rEnemyCombinedFleet = rData as IRawEnemyCombinedFleet;
            if (rEnemyCombinedFleet != null)
                rEnemies = rData.EnemyShipTypeIDs.Skip(1).Concat(rEnemyCombinedFleet.EnemyEscortShipTypeIDs.Where(r => r != -1)).ToArray();

            var rBytes = new byte[rEnemies.Length * sizeof(int)];
            Buffer.BlockCopy(rEnemies, 0, rBytes, 0, rBytes.Length);

            long rCompositionID;
            using (var rSHA1 = SHA1.Create())
            {
                var rHash = rSHA1.ComputeHash(rBytes);
                rCompositionID = BitConverter.ToInt64(rHash, 0);
            }

            int rNodeID;
            var rSortie = SortieInfo.Current;
            if (rSortie.Node.WikiID.IsNullOrEmpty())
                rNodeID = rSortie.Node.ID << 16;
            else
                rNodeID = rSortie.Node.WikiID[0] - 'A';

            using (var rTransaction = r_Connection.BeginTransaction())
            {
                using (var rCommand = r_Connection.CreateCommand())
                {
                    var rBuilder = new StringBuilder(256);

                    rBuilder.Append("INSERT OR IGNORE INTO abyssal.composition(id, position, ship) VALUES");
                    for (var i = 0; i < rEnemies.Length; i++)
                    {
                        if (rEnemies[i] == -1)
                            continue;

                        rBuilder.Append($"({rCompositionID}, {i}, {rEnemies[i]})");
                        if (i < rEnemies.Length - 1)
                            rBuilder.Append(',');
                    }
                    rBuilder.Append(';');

                    rBuilder.Append("INSERT OR IGNORE INTO abyssal.fleet(map, node, difficulty, formation, composition, synced) VALUES(@map, @node, @difficulty, @formation, @composition, 0);");

                    rCommand.CommandText = rBuilder.ToString();
                    rCommand.Parameters.AddWithValue("@map", rSortie.Map.ID);
                    rCommand.Parameters.AddWithValue("@node", rNodeID);
                    rCommand.Parameters.AddWithValue("@difficulty", (int?)rSortie.Map.Difficulty ?? 0);
                    rCommand.Parameters.AddWithValue("@formation", rFormation.FormationAndEngagementForm[1]);
                    rCommand.Parameters.AddWithValue("@composition", rCompositionID);

                    rCommand.ExecuteNonQuery();
                }

                rTransaction.Commit();
            }
        }

        public IList<EnemyFleet> GetEncounters(int rpMap, int rpNode, EventMapDifficulty? rpDifficulty)
        {
            var rFleets = new SortedList<long, EnemyFleet>();

            using (var rCommand = r_Connection.CreateCommand())
            {
                rCommand.CommandText = "SELECT composition.id AS id, group_concat(ship) AS ships, formation FROM abyssal.fleet JOIN abyssal.composition ON fleet.composition = composition.id WHERE map = @map AND node = @node AND difficulty = @difficulty GROUP BY id, formation;";
                rCommand.Parameters.AddWithValue("@map", rpMap);
                rCommand.Parameters.AddWithValue("@node", rpNode);
                rCommand.Parameters.AddWithValue("@difficulty", (int?)rpDifficulty ?? 0);

                using (var rReader = rCommand.ExecuteReader())
                    while (rReader.Read())
                    {
                        var rID = Convert.ToInt64(rReader["id"]);

                        EnemyFleet rFleet;
                        if (!rFleets.TryGetValue(rID, out rFleet))
                        {
                            var rShips = (string)rReader["ships"];
                            var rShipIDs = rShips.Split(',');

                            rFleets.Add(rID, rFleet = new EnemyFleet(rShipIDs));
                        }

                        var rFormation = (Formation)Convert.ToInt32(rReader["formation"]);
                        rFleet.Formations.Add(rFormation);
                    }
            }

            return rFleets.Values.ToArray();
        }
    }
}
