using Sakuno.KanColle.Amatsukaze.Game.Services.Records;
using System;
using System.Data.SQLite;
using System.IO;

namespace Sakuno.KanColle.Amatsukaze.Game.Services
{
    public class RecordService
    {
        public const int Version = 1;

        public static RecordService Instance { get; } = new RecordService();

        public ResourcesRecord Resources { get; private set; }
        public ShipsRecord Ships { get; private set; }
        public ExperienceRecord Experience { get; private set; }

        public bool IsConnected { get; private set; }

        int r_UserID;
        SQLiteConnection r_Connection;

        RecordService() { }

        public void Initialize()
        {
            if (!Directory.Exists("Records"))
                Directory.CreateDirectory("Records");

            SessionService.Instance.Subscribe("api_get_member/basic", _ => Connect(KanColleGame.Current.Port.Admiral.ID));
        }

        public void Connect(int rpUserID)
        {
            if (r_UserID == rpUserID)
                return;

            Resources?.Dispose();
            Ships?.Dispose();
            Experience?.Dispose();
            r_Connection?.Dispose();

            r_UserID = rpUserID;
            r_Connection = new SQLiteConnection($@"Data Source=Records\{r_UserID}.db").OpenAndReturn();

            using (var rTransaction = r_Connection.BeginTransaction())
            {
                CheckVersion();

                Resources = new ResourcesRecord(r_Connection).ConnectAndReturn();
                Ships = new ShipsRecord(r_Connection).ConnectAndReturn();
                Experience = new ExperienceRecord(r_Connection).ConnectAndReturn();

                rTransaction.Commit();
            }
        }

        void CheckVersion()
        {
            int rVersion;
            using (var rCommand = r_Connection.CreateCommand())
            {
                rCommand.CommandText = "CREATE TABLE IF NOT EXISTS metadata(key TEXT PRIMARY KEY NOT NULL, value TEXT) WITHOUT ROWID;" +
                    "SELECT value FROM metadata WHERE key = 'version';";

                rVersion = Convert.ToInt32(rCommand.ExecuteScalar());
            }

            var rLastestVersion = true;
            if (rVersion != 0)
                rLastestVersion = rVersion == Version;
            else
                InitializeDatabase();

            if (!rLastestVersion)
            {

            }
        }
        void InitializeDatabase()
        {
            using (var rCommand = r_Connection.CreateCommand())
            {
                rCommand.CommandText = "CREATE TABLE IF NOT EXISTS versions(key TEXT PRIMARY KEY NOT NULL, value TEXT) WITHOUT ROWID;" +
                    "INSERT INTO metadata(key, value) VALUES('version', @version);";
                rCommand.Parameters.AddWithValue("@version", Version.ToString());

                rCommand.ExecuteNonQuery();
            }
        }
    }
}
