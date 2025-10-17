using System.IO;

namespace Sakuno.KanColle.Amatsukaze
{
    public static class ProductInfo
    {
        public const string AppName = "いんてりじぇんと連装砲くん";
        public const string ProductName = "Intelligent Naval Gun";

        public const string AssemblyVersionString = "1.17.0";

        public static string Version => AssemblyVersionString;
        public static string ReleaseCodeName => "Braindrive";
        public static string ReleaseDate => "2025.10.18";
        public static string ReleaseType => "Beta";

        public const string UserAgent = "ING/" + AssemblyVersionString;

        public static string RootDirectory { get; } = Path.GetDirectoryName(typeof(ProductInfo).Assembly.Location);
    }
}
