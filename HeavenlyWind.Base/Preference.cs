using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Internal;
using Sakuno.KanColle.Amatsukaze.Models.Preferences;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Sakuno.KanColle.Amatsukaze
{
    public partial class Preference
    {
        public static Preference Instance { get; } = new Preference();
        public static Preference Current => Instance;

        internal SQLiteConnection Connection { get; private set; }

        static Preference()
        {
            var rDirectory = new DirectoryInfo(@"Roaming\Preferences");
            if (!rDirectory.Exists)
                rDirectory.Create();
        }

        public void Initialize()
        {
            using (var rConnection = new SQLiteConnection(@"Data Source=Roaming\Preferences\Main.db; Page Size=8192").OpenAndReturn())
            using (var rTransaction = rConnection.BeginTransaction())
            {
                using (var rCommand = rConnection.CreateCommand())
                {
                    rCommand.CommandText =
                        "CREATE TABLE IF NOT EXISTS metadata(key TEXT PRIMARY KEY NOT NULL, value) WITHOUT ROWID; " +
                        "INSERT OR IGNORE INTO metadata(key, value) VALUES('version', 1); " +

                        "CREATE TABLE IF NOT EXISTS preference(key TEXT PRIMARY KEY NOT NULL, value) WITHOUT ROWID; " +
                        "INSERT OR REPLACE INTO preference(key, value) VALUES('main.version', @version);";
                    rCommand.Parameters.AddWithValue("@version", ProductInfo.AssemblyVersionString);

                    rCommand.ExecuteNonQuery();
                }

                rTransaction.Commit();
            }

            Connection = CoreDatabase.Connection;
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "ATTACH @filename AS preference;";
                rCommand.Parameters.AddWithValue("@filename", new FileInfo(@"Roaming\Preferences\Main.db").FullName);

                rCommand.ExecuteNonQuery();
            }
        }

        public void Reload()
        {
            using (var rTransaction = Connection.BeginTransaction())
            {
                MigrateFromPreviousVersion();

                var rReloadedProperties = new List<Property>(Property.Instances.Count);

                using (var rCommand = Connection.CreateCommand())
                {
                    rCommand.CommandText = "SELECT key, value FROM preference.preference;";

                    using (var rReader = rCommand.ExecuteReader())
                        while (rReader.Read())
                        {
                            Property rProperty;
                            if (Property.Instances.TryGetValue((string)rReader["key"], out rProperty))
                            {
                                rProperty.Reload(rReader["value"]);

                                rReloadedProperties.Add(rProperty);
                            }
                        }
                }

                var r = Property.Instances.Values.Except(rReloadedProperties).ToArray();
                foreach (var rProperty in Property.Instances.Values.Except(rReloadedProperties))
                    rProperty.Save();

                rTransaction.Commit();
            }
        }

        void MigrateFromPreviousVersion()
        {
            var rFile = new FileInfo(@"Preferences\Preference.json");
            if (!rFile.Exists)
                return;

            using (var rReader = new JsonTextReader(rFile.OpenText()))
            {
                var rSerializer = new JsonSerializer();
                var rOldPreference = rSerializer.Deserialize<OldPreference>(rReader);

                LoadFromOldPreference(rOldPreference);
            }

            rFile.Directory.Delete(true);
        }
        void LoadFromOldPreference(object rpObject)
        {
            foreach (var rOldProperty in rpObject.GetType().GetTypeInfo().DeclaredProperties)
            {
                if (!rOldProperty.IsDefined(typeof(OldPreferenceMappingAttribute)) && rOldProperty.PropertyType != typeof(JToken))
                {
                    LoadFromOldPreference(rOldProperty.GetValue(rpObject));

                    continue;
                }

                var rMapping = rOldProperty.GetCustomAttribute<OldPreferenceMappingAttribute>();

                Property rProperty;
                if (!Property.Instances.TryGetValue(rMapping.Key, out rProperty))
                    continue;

                if (rProperty.Key != "main.windows")
                    rProperty.SetValue(rOldProperty.GetValue(rpObject));
                else
                    rProperty.SetValue(((JToken)rOldProperty.GetValue(rpObject)).ToObject<WindowPreference[]>());
            }
        }
    }
}
