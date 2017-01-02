using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System.Data.SQLite;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Records
{
    class RankingPointsRecords : RecordsGroup
    {
        public override string GroupName => "ranking_point";
        public override int Version => 2;

        internal RankingPointsRecords(SQLiteConnection rpConnection) : base(rpConnection)
        {
            DisposableObjects.Add(ApiService.Subscribe("api_req_map/next", r => ProcessEscortSuccess((RawMapExploration)r.Data)));

            var rBattleResultApis = new[]
            {
                "api_req_sortie/battleresult",
                "api_req_combined_battle/battleresult",
            };
            DisposableObjects.Add(ApiService.Subscribe(rBattleResultApis, r => ProcessBattleResult((RawBattleResult)r.Data)));
        }

        protected override void CreateTable()
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText =
                    "CREATE TABLE IF NOT EXISTS ranking_point_bonus(" +
                        "sortie INTEGER NOT NULL PRIMARY KEY REFERENCES sortie(id), " +
                        "step INTEGER NOT NULL, " +
                        "time INTEGER NOT NULL, " +
                        "point INTEGER NOT NULL);";

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
