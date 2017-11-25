using System.IO;

namespace Sakuno.KanColle.Amatsukaze
{
    public static class ProductInfo
    {
        public const string AppName = "いんてりじぇんと連装砲くん";
        public const string ProductName = "Intelligent Naval Gun";

        public const string AssemblyVersionString = "0.1.15.6";

        public static string Version => AssemblyVersionString;
        public static string ReleaseCodeName => "Braindrive";
        public static string ReleaseDate => "2017.11.25";

        public const string UserAgent = "ING/" + AssemblyVersionString;

        public static string RootDirectory { get; } = Path.GetDirectoryName(typeof(ProductInfo).Assembly.Location);
    }
}
