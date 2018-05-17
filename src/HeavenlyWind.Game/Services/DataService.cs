using System;
using System.IO;

namespace Sakuno.KanColle.Amatsukaze.Game.Services
{
    public class DataService
    {
        public static DataService Instance { get; } = new DataService();

        DataService() { }

        public string GetDataDirectory() => Path.Combine(Path.GetDirectoryName(typeof(DataService).Assembly.Location), "Data");

        public void EnsureDirectory()
        {
            var rDirectory = new DirectoryInfo(GetDataDirectory());
            if (!rDirectory.Exists)
                rDirectory.Create();
        }
    }
}
