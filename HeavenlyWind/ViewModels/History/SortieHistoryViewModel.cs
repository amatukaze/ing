using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Models.Records;
using System.Data.SQLite;
using System.IO;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.History
{
    class SortieHistoryViewModel : HistoryViewModelBase<SortieRecord>
    {
        protected override string LoadCommandText => @"SELECT sortie.id AS id, sortie.map AS map, CASE is_event_map WHEN 0 THEN 0 ELSE sortie_detail.node - (sortie_detail.node + 2) / 3 * 3 + 3 END AS difficulty, step, CASE is_event_map WHEN 0 THEN node ELSE (node + 2) / 3 END AS node, type, subtype, extra_info, rank, dropped_ship, battle_dropped_item.item as dropped_item, battle_detail.first IS NOT NULL AS battle_detail, participant_hd.ships AS heavily_damaged FROM sortie
JOIN sortie_map ON sortie.map = sortie_map.id
JOIN sortie_detail ON sortie.id = sortie_detail.id
JOIN sortie_node ON sortie.map = sortie_node.map AND CASE sortie_map.is_event_map WHEN 0 THEN sortie_detail.node ELSE (sortie_detail.node + 2) / 3 END = sortie_node.id
JOIN battle ON extra_info = battle.id
LEFT JOIN battle_dropped_item ON battle.id = battle_dropped_item.id
LEFT JOIN battle_detail.battle battle_detail ON extra_info = battle_detail.id
LEFT JOIN battle_detail.participant_hd_view participant_hd ON extra_info = participant_hd.battle
ORDER BY id DESC, step DESC;";

        protected override SortieRecord CreateRecordFromReader(SQLiteDataReader rpReader) => new SortieRecord(rpReader);

        protected override bool TableFilter(string rpTable) => rpTable == "main.battle";

        protected override void PrepareCommandOnRecordInsert(SQLiteCommand rpCommand, string rpTable, long rpRowID)
        {
            rpCommand.CommandText = @"SELECT sortie.id AS id, sortie.map AS map, CASE is_event_map WHEN 0 THEN 0 ELSE sortie_detail.node - (sortie_detail.node + 2) / 3 * 3 + 3 END AS difficulty, step, CASE is_event_map WHEN 0 THEN node ELSE (node + 2) / 3 END AS node, type, subtype, extra_info, rank, dropped_ship, battle_dropped_item.item as dropped_item, battle_detail.first IS NOT NULL AS battle_detail, participant_hd.ships AS heavily_damaged FROM sortie
JOIN sortie_map ON sortie.map = sortie_map.id
JOIN sortie_detail ON sortie.id = sortie_detail.id
JOIN sortie_node ON sortie.map = sortie_node.map AND CASE sortie_map.is_event_map WHEN 0 THEN sortie_detail.node ELSE (sortie_detail.node + 2) / 3 END = sortie_node.id
JOIN battle ON extra_info = battle.id
LEFT JOIN battle_dropped_item ON battle.id = battle_dropped_item.id
LEFT JOIN battle_detail.battle battle_detail ON extra_info = battle_detail.id
LEFT JOIN battle_detail.participant_hd_view participant_hd ON extra_info = participant_hd.battle
WHERE battle.id = @id";
            rpCommand.Parameters.AddWithValue("@id", rpRowID);
        }

        protected override void OnRecordUpdate(string rpTable, long rpRowID)
        {
            if (LastInsertRecord == null || LastInsertRecord.ID != rpRowID)
                return;

            using (var rCommand = RecordService.Instance.CreateCommand())
            {
                rCommand.CommandText = @"SELECT rank, dropped_ship, battle_dropped_item.item as dropped_item, battle_detail.first IS NOT NULL AS battle_detail, participant_hd.ships AS heavily_damaged FROM battle
LEFT JOIN battle_dropped_item ON battle.id = battle_dropped_item.id
LEFT JOIN battle_detail.battle battle_detail ON battle.id = battle_detail.id
LEFT JOIN battle_detail.participant_hd_view participant_hd ON battle.id = participant_hd.battle
WHERE battle.id = @id";
                rCommand.Parameters.AddWithValue("@id", rpRowID);

                using (var rReader = rCommand.ExecuteReader())
                    if (rReader.Read())
                        LastInsertRecord.Update(rReader);
            }
        }

        protected override void ExportAsCsvFileCore(StreamWriter rpWriter)
        {
            var rSR = StringResources.Instance.Main;

            rpWriter.Write(rSR.Record_Time);
            rpWriter.Write(',');
            rpWriter.Write(rSR.SortieHistory_Area);
            rpWriter.Write(',');
            rpWriter.Write(rSR.SortieHistory_Node);
            rpWriter.Write(',');
            rpWriter.Write(rSR.SortieHistory_Node);
            rpWriter.Write("_Wiki,");
            rpWriter.Write(rSR.SortieHistory_Type);
            rpWriter.Write(',');
            rpWriter.Write(rSR.SortieHistory_BattleRank);
            rpWriter.Write(',');
            rpWriter.WriteLine(rSR.SortieHistory_DroppedShip);

            foreach (var rRecord in Records)
            {
                rpWriter.Write(rRecord.Time);
                rpWriter.Write(',');
                rpWriter.Write(rRecord.Map.ID);
                rpWriter.Write(',');
                rpWriter.Write(rRecord.Node);
                rpWriter.Write(',');
                rpWriter.Write(rRecord.NodeWikiID);
                rpWriter.Write(',');

                if (rRecord.EventType == SortieEventType.NormalBattle)
                    rpWriter.Write(StringResources.Instance.Main.GetString("Sortie_BattleType_" + rRecord.BattleType));
                else
                    rpWriter.Write(StringResources.Instance.Main.GetString("Sortie_Event_" + rRecord.EventType));

                rpWriter.Write(',');
                rpWriter.Write(rRecord.BattleRank);
                rpWriter.Write(',');
                rpWriter.WriteLine(rRecord.DroppedShip?.Name);
            }
        }
    }
}
