using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Records
{
    public abstract class RecordsGroup : IDisposable
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

        protected RecordsGroup(SQLiteConnection connection)
        {
            Connection = connection;
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
                rCommand.CommandText = "SELECT coalesce(value, '0') FROM versions WHERE key = @group;";
                rCommand.Parameters.AddWithValue("@group", GroupName);

                rVersion = Convert.ToInt32(rCommand.ExecuteScalar());
            }

            if (rVersion != 0 && rVersion != Version)
                UpgradeFromOldVersionPreprocessStep(rVersion);

            CreateTable();

            if (rVersion != 0 && rVersion != Version)
                UpgradeFromOldVersionPostprocessStep(rVersion);

            UpdateVersion(rVersion);
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

        protected virtual void UpgradeFromOldVersionPreprocessStep(int oldVersion) { }
        protected virtual void UpgradeFromOldVersionPostprocessStep(int oldVersion) { }

        void UpdateVersion(int rpVersion)
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "INSERT OR REPLACE INTO versions(key, value) VALUES(@group, @version);";
                rCommand.Parameters.AddWithValue("@group", GroupName);
                rCommand.Parameters.AddWithValue("@version", Version.ToString());

                rCommand.ExecuteNonQuery();
            }
        }

        protected virtual void Load() { }

        public virtual void Dispose()
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
