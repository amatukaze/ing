using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Sakuno.KanColle.Amatsukaze
{
    public class StringResources : ModelBase
    {
        public static StringResources Instance { get; } = new StringResources();

        static Dictionary<string, LanguageInfo> r_InstalledLanguages;
        public static IList<LanguageInfo> InstalledLanguages { get; private set; }

        public bool IsLoaded { get; private set; }

        static DirectoryInfo StringResourceDirectory;

        StringResourcesItems r_Main;
        public StringResourcesItems Main
        {
            get { return r_Main; }
            private set
            {
                if (r_Main != value)
                {
                    r_Main = value;
                    OnPropertyChanged(nameof(Main));
                }
            }
        }

        StringResources()
        {
            var rRootDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            StringResourceDirectory = new DirectoryInfo(Path.Combine(rRootDirectory, "Resources", "Strings"));

            r_InstalledLanguages = new Dictionary<string, LanguageInfo>(StringComparer.InvariantCultureIgnoreCase);
            if (StringResourceDirectory.Exists)
                foreach (var rLanguageDirectory in StringResourceDirectory.EnumerateDirectories())
                {
                    var rResourceFile = Path.Combine(rLanguageDirectory.FullName, "Main.xml");
                    if (!File.Exists(rResourceFile))
                        continue;

                    var rRoot = XDocument.Load(rResourceFile).Root;
                    var rCultureName = rRoot.Attribute("CultureName").Value;
                    var rDisplayName = rRoot.Attribute("Name").Value;

                    r_InstalledLanguages.Add(rCultureName, new LanguageInfo(rLanguageDirectory.Name, rCultureName, rDisplayName));
                }

            InstalledLanguages = r_InstalledLanguages.Values.ToList().AsReadOnly();
        }

        public void Load()
        {
            if (!InstalledLanguages.Any(r => r.Directory == Preference.Current.Language))
                Preference.Current.Language = GetDefaultLanguage().Directory;

            Load(Preference.Current.Language);
        }
        public static LanguageInfo GetDefaultLanguage()
        {
            var rNames = GetAncestorsAndSelfCultureNames(CultureInfo.CurrentCulture).ToList();

            foreach (var rLanguage in InstalledLanguages)
                if (rNames.Contains(rLanguage.CultureName))
                    return rLanguage;

            return r_InstalledLanguages["en"];
        }
        static IEnumerable<string> GetAncestorsAndSelfCultureNames(CultureInfo rpCultureInfo)
        {
            do
            {
                yield return rpCultureInfo.Name;
                rpCultureInfo = rpCultureInfo.Parent;
            } while (rpCultureInfo != CultureInfo.InvariantCulture);
        }

        void Load(string rpLanguageName)
        {
            var rMainResourceFile = Path.Combine(StringResourceDirectory.FullName, rpLanguageName, "Main.xml");
            if (!File.Exists(rMainResourceFile))
                throw new Exception();

            Main = new StringResourcesItems(XDocument.Load(rMainResourceFile).Root.Descendants("String").ToDictionary(r => r.Attribute("Key").Value, r => r.Value));
            IsLoaded = true;
        }
    }

    public partial class StringResourcesItems
    {
        Dictionary<string, string> r_Items;

        internal StringResourcesItems(Dictionary<string, string> rpItems)
        {
            r_Items = rpItems;
        }

        public string GetString(string rpKey)
        {
            string rResult;
            if (!r_Items.TryGetValue(rpKey, out rResult))
                rResult = rpKey;

            return rResult;
        }
    }
}
