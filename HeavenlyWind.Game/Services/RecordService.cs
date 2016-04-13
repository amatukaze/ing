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

        public ResourcesRecord Resources { get; private set; }
        public ShipsRecord Ships { get; private set; }
        public ExperienceRecord Experience { get; private set; }
        public ExpeditionRecord Expedition { get; private set; }
        public ConstructionRecord Construction { get; private set; }
        public DevelopmentRecord Development { get; private set; }
        public SortieRecord Sortie { get; private set; }
        public BattleRecord Battle { get; private set; }
        public RankingPointBonusRecord RankingPointBonus { get; private set; }
        public FateRecord Fate { get; private set; }

        public QuestProgressRecord QuestProgress { get; private set; }
        public BattleDetailRecord BattleDetail { get; private set; }

        public bool IsConnected { get; private set; }

        int r_UserID;
        SQLiteConnection r_Connection;

        RecordService() { }

        public void Initialize()
        {
            if (!Directory.Exists("Records"))
                Directory.CreateDirectory("Records");

            SessionService.Instance.Subscribe("api_get_member/require_info", _ => Connect(KanColleGame.Current.Port.Admiral.ID));
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

                Resources = new ResourcesRecord(r_Connection).ConnectAndReturn();
                Ships = new ShipsRecord(r_Connection).ConnectAndReturn();
                Experience = new ExperienceRecord(r_Connection).ConnectAndReturn();
                Expedition = new ExpeditionRecord(r_Connection).ConnectAndReturn();
                Construction = new ConstructionRecord(r_Connection).ConnectAndReturn();
                Development = new DevelopmentRecord(r_Connection).ConnectAndReturn();
                Sortie = new SortieRecord(r_Connection).ConnectAndReturn();
                Battle = new BattleRecord(r_Connection).ConnectAndReturn();
                RankingPointBonus = new RankingPointBonusRecord(r_Connection).ConnectAndReturn();
                Fate = new FateRecord(r_Connection).ConnectAndReturn();

                QuestProgress = new QuestProgressRecord(r_Connection).ConnectAndReturn();

                rTransaction.Commit();
            }

            BattleDetail = new BattleDetailRecord(r_Connection, r_UserID).ConnectAndReturn();

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
