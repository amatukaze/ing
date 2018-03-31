using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sakuno.KanColle.Amatsukaze.Data;
using Windows.Storage;
using Windows.Storage.AccessCache;

namespace Sakuno.KanColle.Amatsukaze.UWP.Data
{
    internal class DataService : IDataService
    {
        StorageFolder dataRoot;
        private StorageFile dbFile;
        internal void Initialize()
        {
            if (dbFile != null) return;
            InitializeAsync().Wait();
        }

        private async Task InitializeAsync()
        {
            StorageItemAccessList futureList = StorageApplicationPermissions.FutureAccessList;

            foreach (var entry in futureList.Entries)
                if (entry.Metadata == "data")
                {
                    dataRoot = await futureList.GetFolderAsync(entry.Token);
                    break;
                }
            if (dataRoot == null)
                dataRoot = ApplicationData.Current.RoamingFolder;
            dbFile = await dataRoot.CreateFileAsync("ing.db", CreationCollisionOption.OpenIfExists);
        }

        public void ConfigureDbContext(DbContextOptionsBuilder builder)
        {
            if (dbFile == null)
                throw new InvalidOperationException("Data service not initialized.");

            builder.UseSqlite("Data Source=" + dbFile.Path);
        }

        public Task<Stream> ReadFile(string filename) => dataRoot.OpenStreamForReadAsync(filename);
        public Task<Stream> WriteFile(string filename) => dataRoot.OpenStreamForWriteAsync(filename, CreationCollisionOption.ReplaceExisting);
    }
}
