using Sakuno.KanColle.Amatsukaze.Models.Records;
using System.Data.SQLite;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.History
{
    class ConstructionHistoryViewModel : HistoryViewModelBase<ConstructionRecord>
    {
        protected override string LoadCommandText => "SELECT * FROM construction ORDER BY time DESC;";

        protected override ConstructionRecord CreateRecordFromReader(SQLiteDataReader rpReader) => new ConstructionRecord(rpReader);

        protected override bool TableFilter(string rpTable) => rpTable == "main.construction";

        protected override void PrepareCommandOnRecordInsert(SQLiteCommand rpCommand, string rpTable, long rpRowID)
        {
            rpCommand.CommandText = "SELECT * FROM construction WHERE time = @time LIMIT 1;";
            rpCommand.Parameters.AddWithValue("@time", rpRowID);
        }
    }
}
