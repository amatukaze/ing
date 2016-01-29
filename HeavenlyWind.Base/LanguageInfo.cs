namespace Sakuno.KanColle.Amatsukaze
{
    public class LanguageInfo
    {
        public string Directory { get; }

        public string CultureName { get; }
        public string DisplayName { get; }

        internal LanguageInfo(string rpDirectory, string rpCultureName, string rpDisplayName)
        {
            Directory = rpDirectory;

            CultureName = rpCultureName;
            DisplayName = rpDisplayName;
        }
    }
}
