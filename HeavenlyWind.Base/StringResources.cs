using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using Sakuno.KanColle.Amatsukaze.Models;

namespace Sakuno.KanColle.Amatsukaze
{
    public class StringResources : ModelBase
    {
        public static StringResources Instance { get; } = new StringResources();

        public bool IsLoaded { get; private set; }

        static string StringResourceDirectory;

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
            StringResourceDirectory = Path.Combine(rRootDirectory, "Resources", "Strings");
        }

        public void Load() => Load(Preference.Current.Language);
        public static string GetDefaultLanguage()
        {
            var rCurrentCulture = CultureInfo.CurrentCulture;
            switch (rCurrentCulture.Name)
            {
                case "ja-JP":
                default: return "Japanese";

                case "zh-CN": return "SimplifiedChinese";
            }
        }

        public void Load(string rpLanguageName)
        {
            var rMainResourceFile = Path.Combine(StringResourceDirectory, rpLanguageName, "Main.xml");
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
