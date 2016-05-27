using System.Windows.Media;
using System.Windows.Markup;

namespace Sakuno.KanColle.Amatsukaze.Models
{
    public class SystemFont : ModelBase
    {
        public FontFamily FontFamily { get; }
        public string Name { get; }

        internal SystemFont(FontFamily rpFontFamily)
        {
            FontFamily = rpFontFamily;

            var rNames = rpFontFamily.FamilyNames;
            string rName;
            if (rNames.TryGetValue(XmlLanguage.GetLanguage("ja-jp"), out rName) ||
                rNames.TryGetValue(XmlLanguage.GetLanguage("zh-cn"), out rName) ||
                rNames.TryGetValue(XmlLanguage.GetLanguage("zh-tw"), out rName) ||
                rNames.TryGetValue(XmlLanguage.GetLanguage("zh-hk"), out rName) ||
                rNames.TryGetValue(XmlLanguage.GetLanguage("en-us"), out rName))
                Name = rName;
            else
                Name = rpFontFamily.ToString();
        }
        internal SystemFont(string rpFontFamily)
        {
            Name = rpFontFamily;
            FontFamily = new FontFamily(rpFontFamily);
        }
    }
}
