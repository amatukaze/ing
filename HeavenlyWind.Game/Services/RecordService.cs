using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
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

        public bool IsReadOnlyMode { get; set; }

        public ResourcesRecords Resources { get; private set; }
        public ShipsRecords Ships { get; private set; }
        public ExperienceRecords Experience { get; private set; }
        public ExpeditionRecords Expedition { get; private set; }
        public ConstructionRecords Construction { get; private set; }
        public DevelopmentRecords Development { get; private set; }
        public SortieRecords Sortie { get; private set; }
        public BattleRecords Battle { get; private set; }
        public RankingPointBonusRecords RankingPointBonus { get; private set; }
        public FateRecords Fate { get; private set; }

        public QuestProgressRecords QuestProgress { get; private set; }
        public BattleDetailRecords BattleDetail { get; private set; }

        public bool IsConnected { get; private set; }

        int r_UserID;
        SQLiteConnection r_Connection;

        RecordService() { }

        public void Initialize()
        {
            if (!Directory.Exists("Records"))
                Directory.CreateDirectory("Records");

            SessionService.Instance.Subscribe("api_get_member/require_info", r => Connect(((RawRequiredInfo)r.Data).Admiral.ID));
        }

        void Connect(int rpUserID)
        {
            if (r_UserID == rpUserID)
                return;

            Resources?.Dispose();
            Ships?.Dispose();
            Experience?.Dispose();
            Expedition?.Dispose();
            Construction?.Dispose();
            Development?.Dispose();
            Sortie?.Dispose();
            Battle?.Dispose();
            RankingPointBonus?.Dispose();
            Fate?.Dispose();
            QuestProgress?.Dispose();
            BattleDetail?.Dispose();
            r_Connection?.Dispose();

            IsConnected = false;

            r_UserID = rpUserID;

            r_Connection = new SQLiteConnection($@"Data Source=Records\{r_UserID}.db;Page Size=8192").OpenAndReturn();

            if (IsReadOnlyMode)
                using (var rSourceConnection = r_Connection)
                {
                    r_Connection = new SQLiteConnection("Data Source=:memory:;Page Size=8192").OpenAndReturn();
                    rSourceConnection.BackupDatabase(r_Connection, "main", "main", -1, null, 0);
                }

            using (var rTransaction = r_Connection.BeginTransaction())
            {
                CheckVersion();

                Resources = new ResourcesRecords(r_Connection).ConnectAndReturn();
                Ships = new ShipsRecords(r_Connection).ConnectAndReturn();
                Experience = new ExperienceRecords(r_Connection).ConnectAndReturn();
                Expedition = new ExpeditionRecords(r_Connection).ConnectAndReturn();
                Construction = new ConstructionRecords(r_Connection).ConnectAndReturn();
                Development = new DevelopmentRecords(r_Connection).ConnectAndReturn();
                Sortie = new SortieRecords(r_Connection).ConnectAndReturn();
                Battle = new BattleRecords(r_Connection).ConnectAndReturn();
                RankingPointBonus = new RankingPointBonusRecords(r_Connection).ConnectAndReturn();
                Fate = new FateRecords(r_Connection).ConnectAndReturn();

                QuestProgress = new QuestProgressRecords(r_Connection).ConnectAndReturn();

                rTransaction.Commit();
            }

            BattleDetail = new BattleDetailRecords(r_Connection, r_UserID).ConnectAndReturn();

            IsConnected = true;
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

        public SQLiteCommand CreateCommand() => r_Connection.CreateCommand();
    }
}
