using Sakuno.KanColle.Amatsukaze.Models.Records;
using System.Data.SQLite;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.History
{
    class ExpeditionHistoryViewModel : HistoryViewModelBase<ExpeditionRecord>
    {
        protected override string LoadCommandText => "SELECT * FROM expedition ORDER BY time DESC;";

        protected override ExpeditionRecord CreateRecordFromReader(SQLiteDataReader rpReader) => new ExpeditionRecord(rpReader);

        protected override bool TableFilter(string rpTable) => rpTable == "main.expedition";

        protected override void PrepareCommandOnRecordInsert(SQLiteCommand rpCommand, string rpTable, long rpRowID)
        {
            rpCommand.CommandText = "SELECT * FROM expedition WHERE time = @time LIMIT 1;";
            rpCommand.Parameters.AddWithValue("@time", rpRowID);
        }
    }
}
