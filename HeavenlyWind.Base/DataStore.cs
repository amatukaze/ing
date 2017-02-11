using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace Sakuno.KanColle.Amatsukaze
{
    public static class DataStore
    {
        static SQLiteConnection r_Connection;

        internal static void Initialize()
        {
            var rDirectory = new DirectoryInfo(Path.Combine(ProductInfo.RootDirectory, "Local"));
            if (!rDirectory.Exists)
                rDirectory.Create();

            var rDataStorePath = Path.Combine(rDirectory.FullName, "DataStore.db");

            using (var rConnection = new SQLiteConnection($@"Data Source={rDataStorePath}; Page Size=8192").OpenAndReturn())
            using (var rTransaction = rConnection.BeginTransaction())
            using (var rCommand = rConnection.CreateCommand())
            {
                rCommand.CommandText =
                    "CREATE TABLE IF NOT EXISTS metadata(key TEXT PRIMARY KEY NOT NULL, value) WITHOUT ROWID; " +
                    "INSERT OR IGNORE INTO metadata(key, value) VALUES('version', 1); " +

                    "CREATE TABLE IF NOT EXISTS item(name TEXT PRIMARY KEY NOT NULL, content BLOB NOT NULL, timestamp INTEGER) WITHOUT ROWID;";

                rCommand.ExecuteNonQuery();
                rTransaction.Commit();
            }

            r_Connection = CoreDatabase.Connection;

            using (var rCommand = r_Connection.CreateCommand())
            {
                rCommand.CommandText = "ATTACH @filename AS datastore;";
                rCommand.Parameters.AddWithValue("@filename", rDataStorePath);

                rCommand.ExecuteNonQuery();
            }
        }

        public static bool TryGet(string name, out byte[] bytes)
        {
            if (name == null)
                throw new ArgumentNullException();

            using (var rCommand = r_Connection.CreateCommand())
            {
                rCommand.CommandText = "SELECT content FROM datastore.item WHERE name = @name;";
                rCommand.Parameters.AddWithValue("@name", name);

                bytes = rCommand.ExecuteScalar() as byte[];

                return bytes != null;
            }
        }
        public static bool TryGet(string name, out DataStoreItem item) => TryGet(name, DataStoreRetrieveOption.IncludeContent, out item);
        public static bool TryGet(string name, DataStoreRetrieveOption option, out DataStoreItem item)
        {
            if (name == null)
                throw new ArgumentNullException();

            using (var rCommand = r_Connection.CreateCommand())
            {
                rCommand.CommandText = option == DataStoreRetrieveOption.IncludeContent
                    ? "SELECT content, timestamp FROM datastore.item WHERE name = @name;"
                    : "SELECT NULL, timestamp FROM datastore.item WHERE name = @name;";
                rCommand.Parameters.AddWithValue("@name", name);

                using (var rReader = rCommand.ExecuteReader())
                {
                    if (!rReader.Read())
                    {
                        item = default(DataStoreItem);
                        return false;
                    }

                    item = new DataStoreItem(name, rReader[0] as byte[], rReader.GetInt64Optional(1));
                    return true;
                }
            }
        }

        public static void Set(string name, byte[] bytes, long? timestamp = null)
        {
            if (name == null || bytes == null)
                throw new ArgumentNullException();

            using (var rCommand = r_Connection.CreateCommand())
            {
                rCommand.CommandText = "INSERT OR REPLACE INTO datastore.item(name, content, timestamp) VALUES(@name, @content, @timestamp);";
                rCommand.Parameters.AddWithValue("@name", name);
                rCommand.Parameters.AddWithValue("@content", bytes);
                rCommand.Parameters.AddWithValue("@timestamp", timestamp);

                rCommand.ExecuteNonQuery();
            }
        }
        public static void Set(string name, string text, long? timestamp = null)
        {
            if (name == null || text == null)
                throw new ArgumentNullException();

            using (var rCommand = r_Connection.CreateCommand())
            {
                rCommand.CommandText = "INSERT OR REPLACE INTO datastore.item(name, content, timestamp) VALUES(@name, CAST(@content AS BLOB), @timestamp);";
                rCommand.Parameters.AddWithValue("@name", name);
                rCommand.Parameters.AddWithValue("@content", text);
                rCommand.Parameters.AddWithValue("@timestamp", timestamp);

                rCommand.ExecuteNonQuery();
            }
        }

        public static void Delete(string name)
        {
            if (name == null)
                throw new ArgumentNullException();

            using (var rCommand = r_Connection.CreateCommand())
            {
                rCommand.CommandText = "DELETE FROM datastore.item WHERE name = @name;";
                rCommand.Parameters.AddWithValue("@name", name);

                rCommand.ExecuteNonQuery();
            }
        }

        public static IEnumerable<DataStoreItem> EnumerateItems(DataStoreRetrieveOption option)
        {
            using (var rCommand = r_Connection.CreateCommand())
            {
                rCommand.CommandText = option == DataStoreRetrieveOption.IncludeContent
                    ? "SELECT name, content, timestamp FROM datastore.item;"
                    : "SELECT name, NULL, timestamp FROM datastore.item;";

                using (var rReader = rCommand.ExecuteReader())
                    while (rReader.Read())
                        yield return new DataStoreItem(rReader.GetString(0), rReader[1] as byte[], rReader.GetInt64Optional(2));
            }
        }
    }
}
