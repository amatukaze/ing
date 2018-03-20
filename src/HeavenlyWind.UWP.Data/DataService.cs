using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sakuno.KanColle.Amatsukaze.Data;
using Windows.Storage;
using Windows.Storage.AccessCache;

namespace Sakuno.KanColle.Amatsukaze.UWP.Data
{
    internal class DataService : IDataService
    {
        private StorageFile dataFile;
        internal void Initialize()
        {
            if (dataFile != null) return;
            InitializeAsync().Wait();
        }

        private async Task InitializeAsync()
        {
            StorageItemAccessList futureList = StorageApplicationPermissions.FutureAccessList;
            StorageFolder dataRoot = null;

            foreach (var entry in futureList.Entries)
                if (entry.Metadata == "data")
                {
                    dataRoot = await futureList.GetFolderAsync(entry.Token);
                    break;
                }
            if (dataRoot == null)
                dataRoot = ApplicationData.Current.RoamingFolder;
            dataFile = await dataRoot.CreateFileAsync("ing.db", CreationCollisionOption.OpenIfExists);
        }

        public void Configure(DbContextOptionsBuilder builder)
        {
            if (dataFile == null)
                throw new InvalidOperationException("Data service not initialized.");

            builder.UseSqlite("Data Source=" + dataFile.Path);
        }
    }
}
