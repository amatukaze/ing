using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Models.Records;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.History
{
    class SortieHistoryViewModel : ModelBase
    {
        public IList<SortieRecord> Records { get; private set; }

        public async void LoadRecords()
        {
            using (var rCommand = RecordService.Instance.CreateCommand())
            {
                rCommand.CommandText = @"SELECT sortie.id AS id, sortie.map AS map, CASE is_event_map WHEN 0 THEN 0 ELSE sortie_detail.node - (sortie_detail.node + 2) / 3 * 3 + 3 END AS difficulty, step, CASE is_event_map WHEN 0 THEN node ELSE (node + 2) / 3 END AS node, type, subtype, extra_info, rank, dropped_ship, battle_dropped_item.item as dropped_item, battle_detail.first IS NOT NULL AS battle_detail, participant_hd.ships AS heavily_damaged FROM sortie
JOIN sortie_map ON sortie.map = sortie_map.id
JOIN sortie_detail ON sortie.id = sortie_detail.id
JOIN sortie_node ON sortie.map = sortie_node.map AND CASE sortie_map.is_event_map WHEN 0 THEN sortie_detail.node ELSE (sortie_detail.node + 2) / 3 END = sortie_node.id
JOIN battle ON extra_info = battle.id
LEFT JOIN battle_dropped_item ON battle.id = battle_dropped_item.id
LEFT JOIN battle_detail.battle battle_detail ON extra_info = battle_detail.id
LEFT JOIN (SELECT participant_hd.battle, group_concat(ship) AS ships FROM battle_detail.participant_heavily_damaged participant_hd
    JOIN battle_detail.participant participant ON participant_hd.battle = participant.battle AND participant_hd.id = participant.id
    GROUP BY participant_hd.battle) participant_hd ON extra_info = participant_hd.battle
ORDER BY id DESC, step DESC;";

                using (var rReader = await rCommand.ExecuteReaderAsync())
                {
                    var rRecords = new List<SortieRecord>(rReader.VisibleFieldCount);

                    while (rReader.Read())
                        rRecords.Add(new SortieRecord(rReader));

                    Records = rRecords;
                }
            }

            OnPropertyChanged(nameof(Records));
        }
    }
}
