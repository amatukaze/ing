using Sakuno.Collections;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using Sakuno.KanColle.Amatsukaze.Game.Proxy;
using Sakuno.KanColle.Amatsukaze.Game.Services.Records;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

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
        public FateRecords Fate { get; private set; }

        public QuestProgressRecords QuestProgress { get; private set; }
        public BattleDetailRecords BattleDetail { get; private set; }

        RankingPointsRecords r_RankingPoints;
        SortieConsumptionRecords r_SortieConsumption;

        HashSet<IRecordsGroupProvider> r_CustomRecordsGroupProviders = new HashSet<IRecordsGroupProvider>();
        HybridDictionary<string, RecordsGroup> r_CustomRecordsGroups = new HybridDictionary<string, RecordsGroup>();

        public bool IsConnected { get; private set; }

        int r_UserID;
        SQLiteConnection r_Connection;

        internal Queue<string> HistoryCommandTexts { get; } = new Queue<string>(6);

        public event Action<UpdateEventArgs> Update = delegate { };

        RecordService() { }

        public void Initialize()
        {
            if (!Directory.Exists("Records"))
                Directory.CreateDirectory("Records");

            ApiService.Subscribe("api_get_member/require_info", r => Connect(((RawRequiredInfo)r.Data).Admiral.ID));

            SQLiteConnection.Changed += (rpConnection, e) =>
            {
                if (rpConnection != r_Connection)
                    return;

                switch (e.EventType)
                {
                    case SQLiteConnectionEventType.NewDataReader:
                        HistoryCommandTexts.Enqueue(e.Command.CommandText);

                        if (HistoryCommandTexts.Count > 5)
                            HistoryCommandTexts.Dequeue();

                        break;
                }
            };
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
            Fate?.Dispose();
            r_RankingPoints?.Dispose();
            r_SortieConsumption?.Dispose();
            QuestProgress?.Dispose();
            BattleDetail?.Dispose();

            if (r_Connection != null)
            {
                r_Connection.Update -= OnDatabaseUpdate;

                r_Connection.Dispose();
            }

            foreach (var rCustomGroup in r_CustomRecordsGroups.Values)
                rCustomGroup.Dispose();

            IsConnected = false;

            r_UserID = rpUserID;

            r_Connection = new SQLiteConnection($@"Data Source=Records\{r_UserID}.db;Page Size=8192").OpenAndReturn();

            if (IsReadOnlyMode)
                using (var rSourceConnection = r_Connection)
                {
                    r_Connection = new SQLiteConnection("Data Source=:memory:;Page Size=8192").OpenAndReturn();
                    rSourceConnection.BackupDatabase(r_Connection, "main", "main", -1, null, 0);
                }

            using (var rCommand = r_Connection.CreateCommand())
            {
                rCommand.CommandText = "PRAGMA foreign_keys = ON;";

                rCommand.ExecuteNonQuery();
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
                Fate = new FateRecords(r_Connection).ConnectAndReturn();
                r_RankingPoints = new RankingPointsRecords(r_Connection).ConnectAndReturn();
                r_SortieConsumption = new SortieConsumptionRecords(r_Connection).ConnectAndReturn();

                QuestProgress = new QuestProgressRecords(r_Connection).ConnectAndReturn();

                foreach (var rProvider in r_CustomRecordsGroupProviders)
                {
                    var rGroup = rProvider.Create(r_Connection).ConnectAndReturn();
                    r_CustomRecordsGroups[rGroup.GroupName] = rGroup;
                }

                rTransaction.Commit();
            }

            BattleDetail = new BattleDetailRecords(r_Connection, r_UserID).ConnectAndReturn();

            r_Connection.Update += OnDatabaseUpdate;

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
        public SQLiteTransaction BeginTransaction() => r_Connection.BeginTransaction();

        public void RegisterRecordsGroupProvider(IRecordsGroupProvider rpProvider) => r_CustomRecordsGroupProviders.Add(rpProvider);

        public RecordsGroup GetCustomRecordsGroup(string rpName)
        {
            RecordsGroup rResult;
            r_CustomRecordsGroups.TryGetValue(rpName, out rResult);
            return rResult;
        }

        internal void HandleException(ApiSession rpSession, Exception rException)
        {
            try
            {
                using (var rStreamWriter = new StreamWriter(Logger.GetNewExceptionLogFilename(), false, new UTF8Encoding(true)))
                {
                    rStreamWriter.WriteLine("Exception:");
                    rStreamWriter.WriteLine(rException.ToString());
                    rStreamWriter.WriteLine();

                    var rCommandTexts = HistoryCommandTexts.ToArray();
                    rStreamWriter.WriteLine("SQL:");
                    for (var i = 0; i < rCommandTexts.Length; i++)
                    {
                        rStreamWriter.Write(i + 1);
                        rStreamWriter.WriteLine(':');
                        rStreamWriter.WriteLine(rCommandTexts[i]);
                    }
                    rStreamWriter.WriteLine();

                    rStreamWriter.WriteLine(ApiParserManager.TokenRegex.Replace(rpSession.FullUrl, "***************************"));
                    rStreamWriter.WriteLine("Request Data:");
                    rStreamWriter.WriteLine(ApiParserManager.TokenRegex.Replace(rpSession.RequestBodyString, "***************************"));
                    rStreamWriter.WriteLine();
                    rStreamWriter.WriteLine("Response Data:");
                    rStreamWriter.WriteLine(Regex.Unescape(rpSession.ResponseBodyString));
                }
            }
            catch { }
        }

        void OnDatabaseUpdate(object sender, UpdateEventArgs e)
        {
            Debug.WriteLine($"RecordService: {e.Event} - {e.Database}.{e.Table} - {e.RowId}");

            Update(e);
        }
    }
}
