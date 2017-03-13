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
        const string CommandTextBase = @"SELECT * FROM (
    SELECT sortie.map, sortie.difficulty,
        ifnull(sum(consumption.fuel), 0) - ifnull(sum(reward.fuel), 0) AS fuel,
        ifnull(sum(consumption.bullet), 0) - ifnull(sum(reward.bullet), 0) AS bullet,
        ifnull(sum(consumption.steel), 0) - ifnull(sum(reward.steel), 0) AS steel,
        ifnull(sum(consumption.bauxite), 0) - ifnull(sum(reward.bauxite), 0) AS bauxite,
        ifnull(sum(consumption.bucket), 0) - ifnull(sum(reward.bucket), 0) AS bucket,
        sum(b.experience - a.experience) * 7.0 / 10000 AS ranking_point
    FROM sortie
    LEFT JOIN sortie_consumption_detail consumption USING (id)
    LEFT JOIN sortie_reward reward ON reward.id = sortie.id AND consumption.type = (SELECT min(type) FROM sortie_consumption_detail WHERE id = sortie.id)
    LEFT JOIN admiral_experience a ON a.time = (SELECT max(time) FROM admiral_experience WHERE time <= sortie.id) AND consumption.type = (SELECT min(type) FROM sortie_consumption_detail WHERE id = sortie.id)
    LEFT JOIN admiral_experience b ON b.time = (SELECT min(time) FROM admiral_experience WHERE time >= sortie.return_time) AND sortie.return_time AND consumption.type = (SELECT min(type) FROM sortie_consumption_detail WHERE id = sortie.id)
    WHERE sortie.id >= {0} AND sortie.id < {1} AND sortie.map / 10 IN ({2})
    GROUP BY sortie.map, sortie.difficulty
) consumption
JOIN (
    SELECT sortie.map, sortie.difficulty,
        count(DISTINCT sortie.id) AS count,
        count(sortie_detail.extra_info) AS battle_count,
        count(CASE WHEN sortie_node.type = 5 THEN sortie_detail.extra_info END) AS battle_boss_count,
        count(battle_s.id) AS S,
        count(battle_a.id) AS A,
        count(battle_b.id) AS B,
        count(sortie_detail.extra_info) - count(battle_s.id) - count(battle_a.id) - count(battle_b.id) AS F,
        count(battle_boss_s.id) AS S_boss,
        count(battle_boss_a.id) AS A_boss,
        count(battle_boss_b.id) AS B_boss,
        count(CASE WHEN sortie_node.type = 5 THEN sortie_detail.extra_info END) - count(battle_boss_s.id) - count(battle_boss_a.id) - count(battle_boss_b.id) AS F_boss
    FROM sortie
    JOIN sortie_detail USING (id)
    JOIN sortie_node ON sortie_node.map = sortie.map AND sortie_node.id = sortie_detail.node
    LEFT JOIN battle battle_s ON battle_s.id = sortie_detail.extra_info AND battle_s.rank = 5
    LEFT JOIN battle battle_a ON battle_a.id = sortie_detail.extra_info AND battle_a.rank = 4
    LEFT JOIN battle battle_b ON battle_b.id = sortie_detail.extra_info AND battle_b.rank = 3
    LEFT JOIN battle battle_boss ON battle_boss.id = sortie_detail.extra_info AND sortie_node.type = 5
    LEFT JOIN battle battle_boss_s ON battle_boss_s.id = sortie_detail.extra_info AND sortie_node.type = 5 AND battle_boss_s.rank = 5
    LEFT JOIN battle battle_boss_a ON battle_boss_a.id = sortie_detail.extra_info AND sortie_node.type = 5 AND battle_boss_a.rank = 4
    LEFT JOIN battle battle_boss_b ON battle_boss_b.id = sortie_detail.extra_info AND sortie_node.type = 5 AND battle_boss_b.rank = 3
    WHERE sortie.id >= {0} AND sortie.id < {1} AND sortie.map / 10 IN ({2})
    GROUP BY sortie.map, sortie.difficulty
) battle ON battle.map = consumption.map AND battle.difficulty IS consumption.difficulty;";

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

                rCommand.CommandText = string.Format(CommandTextBase, TimeSpanStart, TimeSpanEnd, rBuilder);

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
