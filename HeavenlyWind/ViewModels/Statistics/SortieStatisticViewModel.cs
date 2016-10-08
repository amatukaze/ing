using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Models.Statistics;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Statistics
{
    class SortieStatisticViewModel : DisposableModelBase
    {
        public IList<SortieStatisticTimeSpanGroupViewModel> TimeSpans { get; private set; }

        SortieStatisticTimeSpanGroupViewModel r_SelectedTimeSpan;
        public SortieStatisticTimeSpanGroupViewModel SelectedTimeSpan
        {
            get { return r_SelectedTimeSpan; }
            set
            {
                if (r_SelectedTimeSpan != value)
                {
                    r_SelectedTimeSpan = value;
                    OnPropertyChanged(nameof(SelectedTimeSpan));
                }
            }
        }

        public SortieStatisticViewModel()
        {
            TimeSpans = Enumerable.Range(0, 6).Select(r => new SortieStatisticTimeSpanGroupViewModel((SortieStatisticTimeSpanType)r)).ToArray();
            r_SelectedTimeSpan = TimeSpans[0];
        }

        public void Load() => Task.Run((Action)LoadCore);
        void LoadCore()
        {
            using (var rCommand = CreateCommand())
            {
                rCommand.CommandText = @"SELECT type, sortie_map.id AS map, sortie_map.difficulty,
    count(*) AS count,
    sum(statistic.fuel) AS fuel_consumption,
    sum(statistic.bullet) AS bullet_consumption,
    sum(statistic.steel) AS steel_consumption,
    sum(statistic.bauxite) AS bauxite_consumption,
    sum(statistic.bucket) AS bucket_consumption,
    sum(statistic.ranking_point) AS ranking_point,
    sum(statistic.s_rank) AS s_rank_count,
    sum(statistic.a_rank) AS a_rank_count,
    sum(statistic.b_rank) AS b_rank_count,
    sum(statistic.failure_rank) AS failure_rank_count
FROM (
    SELECT 0 AS type, strftime('%s', 'now', 'localtime', 'start of day', 'utc') AS start, strftime('%s', 'now') AS end
    UNION ALL
    SELECT 1, strftime('%s', 'now', 'localtime', 'weekday 0', '-6 days', 'start of day', 'utc'), strftime('%s', 'now')
    UNION ALL
    SELECT 2, strftime('%s', 'now', 'localtime', 'start of month', 'utc'), strftime('%s', 'now')
    UNION ALL
    SELECT 3 AS type, strftime('%s', 'now', 'localtime', 'start of day', '-1 days', 'utc') AS start, strftime('%s', 'now', 'localtime', 'start of day', 'utc') AS end
    UNION ALL
    SELECT 4, strftime('%s', 'now', 'localtime', 'weekday 0', '-13 days', 'start of day', 'utc'), strftime('%s', 'now', 'localtime', 'weekday 0', '-6 days', 'start of day', 'utc')
    UNION ALL
    SELECT 5, strftime('%s', 'now', 'localtime', 'start of month', '-1 month', 'utc'), strftime('%s', 'now', 'localtime', 'start of month', 'utc')
) timespan
CROSS JOIN (SELECT DISTINCT map AS id, difficulty FROM sortie ORDER BY id, difficulty) sortie_map
JOIN (
    SELECT id, map, difficulty,
        ifnull(sum(sortie_consumption_detail.fuel), 0) - ifnull(sortie_reward.fuel, 0) AS fuel,
        ifnull(sum(sortie_consumption_detail.bullet), 0) - ifnull(sortie_reward.bullet, 0) AS bullet,
        ifnull(sum(sortie_consumption_detail.steel), 0) - ifnull(sortie_reward.steel, 0) AS steel,
        ifnull(sum(sortie_consumption_detail.bauxite), 0) - ifnull(sortie_reward.bauxite, 0) AS bauxite,
        ifnull(sum(sortie_consumption_detail.bucket), 0) AS bucket,
        ((SELECT experience FROM admiral_experience WHERE time = (SELECT min(time) FROM admiral_experience WHERE time >= sortie.return_time)) - (SELECT experience FROM admiral_experience WHERE time = (SELECT max(time) FROM admiral_experience WHERE time <= sortie.id))) * 7.0 / 10000 + ifnull((SELECT point FROM ranking_point_bonus WHERE sortie = sortie.id), 0) AS ranking_point,
        s_rank, a_rank, b_rank, failure_rank
    FROM sortie_consumption
    JOIN sortie USING(id)
    JOIN (
        SELECT sortie_detail.id,
            COUNT(CASE WHEN rank = 5 THEN sortie_detail.id END) AS s_rank,
            COUNT(CASE WHEN rank = 4 THEN sortie_detail.id END) AS a_rank,
            COUNT(CASE WHEN rank = 3 THEN sortie_detail.id END) AS b_rank,
            COUNT(CASE WHEN rank NOT IN (3, 4, 5) THEN sortie_detail.id END) AS failure_rank
        FROM sortie_detail
        JOIN battle ON battle.id = sortie_detail.extra_info
        GROUP BY sortie_detail.id
    ) USING(id)
    LEFT JOIN sortie_consumption_detail USING(id)
    LEFT JOIN sortie_reward USING(id)
    GROUP BY id
    ORDER BY id DESC
) statistic ON map = sortie_map.id AND statistic.difficulty IS sortie_map.difficulty AND statistic.id >= timespan.start AND statistic.id < timespan.end
GROUP BY type, sortie_map.id, sortie_map.difficulty;";

                using (var rReader = rCommand.ExecuteReader())
                {
                    var rData = new List<SortieStatisticData>(rReader.VisibleFieldCount);

                    while (rReader.Read())
                        rData.Add(new SortieStatisticData(rReader));

                    foreach (var rGroup in rData.GroupBy(r => r.Type))
                        TimeSpans[(int)rGroup.Key].Update(rGroup);
                }
            }
        }

        SQLiteCommand CreateCommand() => RecordService.Instance.CreateCommand();

        protected override void DisposeManagedResources()
        {
            SelectedTimeSpan = null;
            TimeSpans = null;
        }
    }
}
