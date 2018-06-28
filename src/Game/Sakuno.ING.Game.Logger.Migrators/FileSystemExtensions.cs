using System.IO;

namespace Sakuno.ING.Game.Logger.Migrators
{
    internal static class FileSystemExtensions
    {
        public static bool TryGetFile(this DirectoryInfo folder, string name, out FileInfo file)
        {
            var files = folder.GetFiles(name);
            if (files.Length != 1)
            {
                file = null;
                return false;
            }
            else
            {
                file = files[0];
                return true;
            }
        }
    }
}
