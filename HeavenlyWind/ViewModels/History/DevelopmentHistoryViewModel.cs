using Sakuno.KanColle.Amatsukaze.Models.Records;
using System.Data.SQLite;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.History
{
    class DevelopmentHistoryViewModel : HistoryViewModelBase<DevelopmentRecord>
    {
        protected override string LoadCommandText => "SELECT * FROM development ORDER BY time DESC;";

        protected override DevelopmentRecord CreateRecordFromReader(SQLiteDataReader rpReader) => new DevelopmentRecord(rpReader);

        protected override bool TableFilter(string rpTable) => rpTable == "main.development";

        protected override void PrepareCommandOnRecordInsert(SQLiteCommand rpCommand, string rpTable, long rpRowID)
        {
            rpCommand.CommandText = "SELECT * FROM development WHERE time = @time LIMIT 1;";
            rpCommand.Parameters.AddWithValue("@time", rpRowID);
        }
    }
}
