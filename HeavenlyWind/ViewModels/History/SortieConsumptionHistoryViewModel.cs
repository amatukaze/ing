using Sakuno.KanColle.Amatsukaze.Models.Records;
using System.Data.SQLite;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.History
{
    class SortieConsumptionHistoryViewModel : HistoryViewModelBase<SortieConsumptionRecord>
    {
        protected override string LoadCommandText => @"SELECT sortie.id, map, difficulty,
    ifnull(sum(sortie_consumption_detail.fuel), 0) - ifnull(sortie_reward.fuel, 0) AS fuel,
    ifnull(sum(sortie_consumption_detail.bullet), 0) - ifnull(sortie_reward.bullet, 0) AS bullet,
    ifnull(sum(sortie_consumption_detail.steel), 0) - ifnull(sortie_reward.steel, 0) AS steel,
    ifnull(sum(sortie_consumption_detail.bauxite), 0) - ifnull(sortie_reward.bauxite, 0) AS bauxite,
    ifnull(sum(sortie_consumption_detail.bucket), 0) AS bucket,
    ((SELECT experience FROM admiral_experience WHERE time = (SELECT min(time) FROM admiral_experience WHERE time >= sortie.return_time)) - (SELECT experience FROM admiral_experience WHERE time = (SELECT max(time) FROM admiral_experience WHERE time <= sortie.id))) * 7.0 / 10000 AS ranking_point
FROM sortie_consumption
JOIN sortie ON sortie.id = sortie_consumption.id AND sortie.return_time IS NOT NULL
LEFT JOIN sortie_consumption_detail USING(id)
LEFT JOIN sortie_reward USING(id)
GROUP BY sortie.id
ORDER BY sortie.id DESC;";

        protected override SortieConsumptionRecord CreateRecordFromReader(SQLiteDataReader rpReader) => new SortieConsumptionRecord(rpReader);

        protected override void PrepareCommandOnRecordInsert(SQLiteCommand rpCommand, string rpTable, long rpRowID)
        {
        }

        protected override bool TableFilter(string rpTable) => false;
    }
}
