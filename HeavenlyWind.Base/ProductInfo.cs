namespace Sakuno.KanColle.Amatsukaze
{
    public static class ProductInfo
    {
        public const string AppName = "いんてりじぇんと連装砲くん";
        public const string ProductName = "Intelligent Naval Gun";

        public const string AssemblyVersionString = "0.1.7.2";

        public static string Version => AssemblyVersionString;
        public static string ReleaseCodeName => "Moonbria";

        public const string UserAgent = "ING/" + AssemblyVersionString;
    }
}
