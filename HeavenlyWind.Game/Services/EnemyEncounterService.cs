using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
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
            SessionService.Instance.SubscribeOnce("api_get_member/basic", delegate
            {
                r_Connection = new SQLiteConnection(@"Data Source=Data\AbyssalFleets.db; Page Size=8192").OpenAndReturn();

                using (var rCommand = r_Connection.CreateCommand())
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
            };
            SessionService.Instance.Subscribe(rBattleApis, ProcessAbyssalFleet);
        }

        void ProcessAbyssalFleet(ApiData rpData)
        {
            var rData = rpData.Data as RawBattleBase;
            var rFormation = rpData.Data as IRawFormationAndEngagementForm;
            if (rData == null || rFormation == null)
                return;

            var rEnemies = rData.EnemyShipTypeIDs.Where(r => r != -1).ToArray();
            var rBytes = new byte[rEnemies.Length * sizeof(int)];
            Buffer.BlockCopy(rEnemies, 0, rBytes, 0, rBytes.Length);

            long rCompositionID;
            using (var rSHA1 = SHA1.Create())
            {
                var rHash = rSHA1.ComputeHash(rBytes);
                rCompositionID = BitConverter.ToInt64(rHash, 0);
            }

            int rNodeID;
            var rSortie = KanColleGame.Current.Sortie;
            if (rSortie.Node.WikiID.IsNullOrEmpty())
                rNodeID = rSortie.Node.ID << 16;
            else
                rNodeID = rSortie.Node.WikiID[0] - 'A';

            using (var rTransaction = r_Connection.BeginTransaction())
            {
                using (var rCommand = r_Connection.CreateCommand())
                {
                    var rBuilder = new StringBuilder(256);

                    rBuilder.Append("INSERT OR IGNORE INTO composition(id, position, ship) VALUES");
                    for (var i = 0; i < rEnemies.Length; i++)
                    {
                        rBuilder.Append($"({rCompositionID}, {i}, {rEnemies[i]})");
                        if (i < rEnemies.Length - 1)
                            rBuilder.Append(',');
                    }
                    rBuilder.Append(';');

                    rBuilder.Append("INSERT OR IGNORE INTO fleet(map, node, difficulty, formation, composition, synced) VALUES(@map, @node, @difficulty, @formation, @composition, 0);");

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
            var rResult = new List<EnemyFleet>();

            using (var rCommand = r_Connection.CreateCommand())
            {
                rCommand.CommandText = "SELECT group_concat(ship) AS ships, formation FROM fleet JOIN composition ON fleet.composition = composition.id WHERE map = @map AND node = @node AND difficulty = @difficulty GROUP BY id, formation;";
                rCommand.Parameters.AddWithValue("@map", rpMap);
                rCommand.Parameters.AddWithValue("@node", rpNode);
                rCommand.Parameters.AddWithValue("@difficulty", (int?)rpDifficulty ?? 0);

                using (var rReader = rCommand.ExecuteReader())
                    while (rReader.Read())
                    {
                        var rShips = (string)rReader["ships"];
                        var rShipIDs = rShips.Split(',').Select(int.Parse);
                        var rFormation = (Formation)Convert.ToInt32(rReader["formation"]);

                        rResult.Add(new EnemyFleet(rShipIDs, rFormation));
                    }
            }

            return rResult;
        }
    }
}
