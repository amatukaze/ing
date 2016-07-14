using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System.Data.SQLite;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Records
{
    public class RankingPointsRecords : RecordsGroup
    {
        public override string GroupName => "ranking_point";
        public override int Version => 2;

        internal RankingPointsRecords(SQLiteConnection rpConnection) : base(rpConnection)
        {
            DisposableObjects.Add(SessionService.Instance.Subscribe("api_req_ranking/getlist", rpData =>
            {
                var rMyData = rpData.Json["api_data"]["api_list"].SingleOrDefault(r => (int)r["api_member_id"] == KanColleGame.Current.Port.Admiral.ID);
                if (rMyData == null)
                    return;

                var rPosition = (int)rMyData["api_no"];
                var rScore = (int)rMyData["api_rate"];

                InsertRankingPoints(rPosition, rScore);
            }));

            DisposableObjects.Add(SessionService.Instance.Subscribe("api_req_map/next", r => ProcessEscortSuccess((RawMapExploration)r.Data)));

            var rBattleResultApis = new[]
            {
                "api_req_sortie/battleresult",
                "api_req_combined_battle/battleresult",
            };
            DisposableObjects.Add(SessionService.Instance.Subscribe(rBattleResultApis, r => ProcessBattleResult((RawBattleResult)r.Data)));
        }

        protected override void CreateTable()
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText =
                    "CREATE TABLE IF NOT EXISTS ranking_point(" +
                        "time INTEGER NOT NULL PRIMARY KEY, " +
                        "position INTEGER NOT NULL, " +
                        "score INTEGER NOT NULL); " +

                    "CREATE TABLE IF NOT EXISTS ranking_point_bonus(" +
                        "sortie INTEGER NOT NULL PRIMARY KEY REFERENCES sortie(id), " +
                        "step INTEGER NOT NULL, " +
                        "time INTEGER NOT NULL, " +
                        "point INTEGER NOT NULL);";

                rCommand.ExecuteNonQuery();
            }
        }

        void InsertRankingPoints(int rpPosition, int rpScore)
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "INSERT OR IGNORE INTO ranking_point(time, position, score) " +
                    "VALUES((CASE (strftime('%H', 'now') + 7) / 12 WHEN 1 THEN strftime('%s', 'now', 'start of day', '+5 hour') ELSE strftime('%s', 'now', 'start of day', '-7 hour') END), " +
                        "@position, @score);";
                rCommand.Parameters.AddWithValue("@position", rpPosition);
                rCommand.Parameters.AddWithValue("@score", rpScore);

                rCommand.ExecuteNonQuery();
            }
        }

        void ProcessEscortSuccess(RawMapExploration rpData)
        {
            if (rpData.RankingPointBonus == 0)
                return;

            InsertRankingPointsBonusRecord(rpData.RankingPointBonus);
        }

        void ProcessBattleResult(RawBattleResult rpData)
        {
            if (rpData.ExtraOperationRankingPointBonus == 0)
                return;

            InsertRankingPointsBonusRecord(rpData.ExtraOperationRankingPointBonus);
        }

        void InsertRankingPointsBonusRecord(int rpPoint)
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "INSERT INTO ranking_point_bonus(sortie, step, time, point) " +
                    "VALUES((SELECT MAX(id) FROM sortie), " +
                        "(SELECT max(step) FROM sortie_detail WHERE id = (SELECT MAX(id) FROM sortie)), " +
                        "strftime('%s', 'now'), @point);";
                rCommand.Parameters.AddWithValue("@point", rpPoint);

                rCommand.ExecuteNonQuery();
            }
        }
    }
}
