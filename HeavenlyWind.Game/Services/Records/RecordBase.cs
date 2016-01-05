using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Records
{
    public abstract class RecordBase : IDisposable
    {
        protected SQLiteConnection Connection { get; }

        public abstract string GroupName { get; }
        public virtual int Version => 1;

        List<IDisposable> r_DisposableObjects;
        protected IList<IDisposable> DisposableObjects
        {
            get
            {
                if (r_DisposableObjects == null)
                    r_DisposableObjects = new List<IDisposable>();
                return r_DisposableObjects;
            }
        }

        protected RecordBase(SQLiteConnection rpConnection)
        {
            Connection = rpConnection;
        }

        internal void Connect()
        {
            CheckVersion();
            Load();
        }
        void CheckVersion()
        {
            int rVersion;
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "SELECT value FROM versions WHERE key = @group;";
                rCommand.Parameters.AddWithValue("@group", GroupName);

                rVersion = Convert.ToInt32(rCommand.ExecuteScalar());
            }

            if (rVersion == 0)
                InitializeTableVersion();
            else if (rVersion != Version)
            {
                UpgradeFromOldVersion(rVersion);
                UpdateVersion(rVersion);
            }

            CreateTable();
        }
        void InitializeTableVersion()
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "INSERT INTO versions(key, value) VALUES(@group, @version);";
                rCommand.Parameters.AddWithValue("@group", GroupName);
                rCommand.Parameters.AddWithValue("@version", Version.ToString());

                rCommand.ExecuteNonQuery();
            }
        }
        protected abstract void CreateTable();

        protected virtual void UpgradeFromOldVersion(int rpOldVersion) { }
        void UpdateVersion(int rpVersion)
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "UPDATE versions SET value = @version WHERE key = @group;";
                rCommand.Parameters.AddWithValue("@group", GroupName);
                rCommand.Parameters.AddWithValue("@version", Version.ToString());

                rCommand.ExecuteNonQuery();
            }
        }

        protected virtual void Load() { }

        public void Dispose()
        {
            if (r_DisposableObjects != null)
            {
                foreach (var rObject in r_DisposableObjects)
                    rObject.Dispose();

                r_DisposableObjects = null;
            }
        }
    }
}
