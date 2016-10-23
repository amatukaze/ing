using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Internal;
using Sakuno.KanColle.Amatsukaze.Models.Records;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.History
{
    class SortieHistoryViewModel : SortieHistoryViewModelBase<SortieRecord>
    {
        protected override string LoadCommandText => @"SELECT sortie.id AS id, sortie.map AS map, difficulty, step, node, type, subtype, extra_info, rank, dropped_ship, battle_dropped_item.item as dropped_item, battle_detail.first IS NOT NULL AS battle_detail, participant_hd.ships AS heavily_damaged, mvp, mvp_escort FROM sortie
JOIN sortie_map ON sortie.map = sortie_map.id
JOIN sortie_detail ON sortie.id = sortie_detail.id
JOIN sortie_node ON sortie.map = sortie_node.map AND sortie_detail.node = sortie_node.id
JOIN battle ON extra_info = battle.id
LEFT JOIN battle_dropped_item ON battle.id = battle_dropped_item.id
LEFT JOIN battle_detail.battle battle_detail ON extra_info = battle_detail.id
LEFT JOIN battle_detail.participant_hd_view participant_hd ON extra_info = participant_hd.battle
ORDER BY id DESC, step DESC;";

        protected override string LoadMapsCommandText => "SELECT DISTINCT map, difficulty FROM sortie ORDER BY map, difficulty;";

        public FilterKeyCollection<string> Nodes { get; } = new FilterKeyCollection<string>(string.Empty, string.Compare);

        string r_Node = string.Empty;
        public string SelectedNode
        {
            get { return r_Node; }
            set
            {
                if (r_Node != value)
                {
                    r_Node = value;
                    OnPropertyChanged(nameof(SelectedNode));

                    Refresh();
                }
            }
        }

        bool r_SRank = true;
        public bool SRank
        {
            get { return r_SRank; }
            set
            {
                if (r_SRank != value)
                {
                    r_SRank = value;
                    OnPropertyChanged(nameof(SRank));

                    Refresh();
                }
            }
        }

        bool r_ARank = true;
        public bool ARank
        {
            get { return r_ARank; }
            set
            {
                if (r_ARank != value)
                {
                    r_ARank = value;
                    OnPropertyChanged(nameof(ARank));

                    Refresh();
                }
            }
        }

        bool r_BRank = true;
        public bool BRank
        {
            get { return r_BRank; }
            set
            {
                if (r_BRank != value)
                {
                    r_BRank = value;
                    OnPropertyChanged(nameof(BRank));

                    Refresh();
                }
            }
        }

        bool r_OtherRank = true;
        public bool OtherRank
        {
            get { return r_OtherRank; }
            set
            {
                if (r_OtherRank != value)
                {
                    r_OtherRank = value;
                    OnPropertyChanged(nameof(OtherRank));

                    Refresh();
                }
            }
        }

        public override bool Filter(SortieRecord rpItem)
        {
            var rResult = r_Map == SortieMapFilterKey.All ||
                (rpItem.Map == r_Map.Map && (!r_Map.IsEventMap || rpItem.EventMapDifficulty == r_Map.EventMapDifficulty) && (r_Node == string.Empty || rpItem.NodeWikiID == r_Node));

            switch (rpItem.BattleRank)
            {
                case BattleRank.SS:
                case BattleRank.S:
                    rResult &= r_SRank;
                    break;

                case BattleRank.A:
                    rResult &= r_ARank;
                    break;

                case BattleRank.B:
                    rResult &= r_BRank;
                    break;

                default:
                    rResult &= r_OtherRank;
                    break;
            }

            return rResult;
        }

        protected override void OnSelectedMapChanged()
        {
            if (r_Map != SortieMapFilterKey.All)
                UpdateNodes();

            r_Node = string.Empty;
            OnPropertyChanged(nameof(SelectedNode));
        }
        void UpdateNodes()
        {
            var rMap = r_Map.Map;

            using (var rCommand = CreateCommand())
            {
                rCommand.CommandText = @"SELECT DISTINCT node FROM sortie
JOIN sortie_map ON sortie.map = sortie_map.id
JOIN sortie_detail ON sortie.id = sortie_detail.id
JOIN sortie_node ON sortie.map = sortie_node.map AND sortie_detail.node = sortie_node.id
WHERE sortie_map.id = @map AND (difficulty IS NULL OR difficulty = @difficulty) AND type IN (4, 5);";
                rCommand.Parameters.AddWithValue("@map", rMap.ID);
                rCommand.Parameters.AddWithValue("@difficulty", r_Map.EventMapDifficulty);

                using (var rReader = rCommand.ExecuteReader())
                {
                    var rNodes = new List<string>(rReader.VisibleFieldCount);

                    while (rReader.Read())
                    {
                        var rNode = rReader.GetInt32(0);
                        rNodes.Add(MapService.Instance.GetNodeWikiID(rMap.ID, rNode) ?? rNode.ToString());
                    }

                    Nodes.Update(rNodes.Distinct());
                }
            }
        }

        protected override SortieRecord CreateRecordFromReader(SQLiteDataReader rpReader) => new SortieRecord(rpReader);

        protected override bool TableFilter(string rpTable) => rpTable == "main.battle";

        protected override void PrepareCommandOnRecordInsert(SQLiteCommand rpCommand, string rpTable, long rpRowID)
        {
            rpCommand.CommandText = @"SELECT sortie.id AS id, sortie.map AS map, difficulty, step, node, type, subtype, extra_info, rank, dropped_ship, battle_dropped_item.item as dropped_item, battle_detail.first IS NOT NULL AS battle_detail, participant_hd.ships AS heavily_damaged FROM sortie
JOIN sortie_map ON sortie.map = sortie_map.id
JOIN sortie_detail ON sortie.id = sortie_detail.id
JOIN sortie_node ON sortie.map = sortie_node.map AND sortie_detail.node = sortie_node.id
JOIN battle ON extra_info = battle.id
LEFT JOIN battle_dropped_item ON battle.id = battle_dropped_item.id
LEFT JOIN battle_detail.battle battle_detail ON extra_info = battle_detail.id
LEFT JOIN battle_detail.participant_hd_view participant_hd ON extra_info = participant_hd.battle
WHERE battle.id = @id";
            rpCommand.Parameters.AddWithValue("@id", rpRowID);
        }

        protected override void OnRecordUpdate(string rpTable, long rpRowID)
        {
            if (LastInsertedRecord == null || LastInsertedRecord.ID != rpRowID)
                return;

            using (var rCommand = CreateCommand())
            {
                rCommand.CommandText = @"SELECT rank, dropped_ship, battle_dropped_item.item as dropped_item, battle_detail.first IS NOT NULL AS battle_detail, participant_hd.ships AS heavily_damaged FROM battle
LEFT JOIN battle_dropped_item ON battle.id = battle_dropped_item.id
LEFT JOIN battle_detail.battle battle_detail ON battle.id = battle_detail.id
LEFT JOIN battle_detail.participant_hd_view participant_hd ON battle.id = participant_hd.battle
WHERE battle.id = @id";
                rCommand.Parameters.AddWithValue("@id", rpRowID);

                using (var rReader = rCommand.ExecuteReader())
                    if (rReader.Read())
                        LastInsertedRecord.Update(rReader);
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
            rpWriter.Write(rSR.SortieHistory_DroppedShip);
            rpWriter.Write(',');
            rpWriter.WriteLine(rSR.SortieHistory_HeavilyDamagedShip);

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
                rpWriter.Write(rRecord.DroppedShip?.Name);
                rpWriter.Write(',');
                rpWriter.WriteLine(rRecord.HeavilyDamagedShips == null || rRecord.HeavilyDamagedShips.Count == 0 ? string.Empty : rRecord.HeavilyDamagedShips.Select(r => r.TranslatedName).Join(", "));
            }
        }
    }
}
