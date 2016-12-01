using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Models.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Statistics
{
    abstract class SortieStatisticTimeSpanGroupViewModel : ModelBase
    {
        const string CommandTextBase = @"SELECT sortie_map.id AS map, sortie_map.difficulty,
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
FROM (SELECT DISTINCT map AS id, difficulty FROM sortie WHERE map / 10 IN ({0}) ORDER BY id, difficulty) sortie_map
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
            count(CASE WHEN rank = 5 THEN sortie_detail.id END) AS s_rank,
            count(CASE WHEN rank = 4 THEN sortie_detail.id END) AS a_rank,
            count(CASE WHEN rank = 3 THEN sortie_detail.id END) AS b_rank,
            count(CASE WHEN rank NOT IN (3, 4, 5) THEN sortie_detail.id END) AS failure_rank
        FROM sortie_detail
        JOIN battle ON battle.id = sortie_detail.extra_info
        GROUP BY sortie_detail.id
    ) USING(id)
    LEFT JOIN sortie_consumption_detail USING(id)
    LEFT JOIN sortie_reward USING(id)
    GROUP BY id
    ORDER BY id DESC
) statistic ON map = sortie_map.id AND statistic.difficulty IS sortie_map.difficulty AND statistic.id >= {1} AND statistic.id < {2}
GROUP BY sortie_map.id, sortie_map.difficulty;";

        SortieStatisticViewModel r_Owner;

        public SortieStatisticTimeSpanType Type { get; }

        protected string TimeSpanStart { get; set; }
        protected string TimeSpanEnd { get; set; }

        IList<ModelBase> r_Map;
        public IList<ModelBase> Maps
        {
            get { return r_Map; }
            internal set
            {
                if (r_Map != value)
                {
                    r_Map = value;
                    OnPropertyChanged(nameof(Maps));
                }
            }
        }

        protected SortieStatisticTimeSpanGroupViewModel(SortieStatisticViewModel rpOwner, SortieStatisticTimeSpanType rpType)
        {
            r_Owner = rpOwner;

            Type = rpType;
        }

        public void Reload() => Task.Run((Action)ReloadCore);
        void ReloadCore()
        {
            if (TimeSpanStart.IsNullOrEmpty() || TimeSpanEnd.IsNullOrEmpty())
                return;

            using (var rCommand = RecordService.Instance.CreateCommand())
            {
                var rBuilder = new StringBuilder(256);
                foreach (var rArea in r_Owner.Areas)
                {
                    if (!rArea.IsSelected)
                        continue;

                    if (rArea.Area == null && r_Owner.PastAreas == null)
                        continue;

                    if (rBuilder.Length > 0)
                        rBuilder.Append(", ");

                    if (rArea.Area != null)
                        rBuilder.Append(rArea.Area.ID);
                    else
                        rBuilder.Append(r_Owner.PastAreas);
                }

                if (rBuilder.Length == 0)
                {
                    Maps = new[] { new SortieStatisticTotalItem(Enumerable.Empty<SortieStatisticData>()) };
                    OnPropertyChanged(string.Empty);

                    return;
                }

                rCommand.CommandText = string.Format(CommandTextBase, rBuilder, TimeSpanStart, TimeSpanEnd);

                using (var rReader = rCommand.ExecuteReader())
                {
                    var rData = new List<SortieStatisticData>(rReader.VisibleFieldCount);

                    while (rReader.Read())
                        rData.Add(new SortieStatisticData(rReader));

                    Maps = rData.Select(r => new SortieStatisticMapItem(r)).ToList<ModelBase>();
                    Maps.Insert(0, new SortieStatisticTotalItem(rData));
                    OnPropertyChanged(string.Empty);
                }
            }
        }
    }
}
