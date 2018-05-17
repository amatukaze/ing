using System.Data.SQLite;
using System.Diagnostics;

namespace Sakuno.KanColle.Amatsukaze
{
    static class CoreDatabase
    {
        public static SQLiteConnection Connection { get; private set; }

        public static void Initialize()
        {
            Connection = new SQLiteConnection("Data Source=:memory:").OpenAndReturn();

            LogDatabaseUpdate();
        }
        [Conditional("DEBUG")]
        static void LogDatabaseUpdate() => Connection.Update += (s, e) => Debug.WriteLine($"Core: {e.Event} - {e.Table} - {e.RowId}");
    }
}
