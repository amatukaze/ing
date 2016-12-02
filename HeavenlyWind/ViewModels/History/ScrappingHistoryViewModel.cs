using Sakuno.KanColle.Amatsukaze.Models.Records;
using System.Data.SQLite;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.History
{
    class ScrappingHistoryViewModel : HistoryViewModelBase<FateRecord>
    {
        protected override string LoadCommandText => @"SELECT id, ship AS master_id, level, 0 AS proficiency, time, fate, 0 AS is_equipment FROM ship_fate WHERE ship != -1
UNION
SELECT id, equipment AS master_id, level, proficiency, time, fate, 1 AS is_equipment FROM equipment_fate WHERE equipment != -1;";

        protected override FateRecord CreateRecordFromReader(SQLiteDataReader rpReader) => new FateRecord(rpReader);

        protected override bool TableFilter(string rpTable) => rpTable == "main.ship_fate" || rpTable == "main.equipment_fate";

        protected override void PrepareCommandOnRecordInsert(SQLiteCommand rpCommand, string rpTable, long rpRowID)
        {
            string rCommandText;
            switch (rpTable)
            {
                case "main.ship_fate":
                    rCommandText = "SELECT id, ship AS master_id, level, 0 AS proficiency, time, fate, 0 AS is_equipment FROM ship_fate WHERE id = @id AND ship != -1 LIMIT 1;";
                    break;

                case "main.equipment_fate":
                    rCommandText = "SELECT id, equipment AS master_id, level, proficiency, time, fate, 1 AS is_equipment FROM equipment_fate WHERE id = @id AND equipment != -1 LIMIT 1;";
                    break;

                default: return;
            }

            rpCommand.CommandText = rCommandText;
            rpCommand.Parameters.AddWithValue("@id", rpRowID);
        }
    }
}
