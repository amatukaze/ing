using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Sakuno.KanColle.Amatsukaze.Plugin.ImprovementArsenal.Internal
{
    class InternalStringResources : ModelBase
    {
        public static InternalStringResources Instance { get; } = new InternalStringResources();

        public Dictionary<string, string> Strings { get; private set; }

        InternalStringResources()
        {
            PropertyChangedEventListener.FromSource(StringResources.Instance).Add(nameof(StringResources.Instance.Main), (s, e) => LoadStringResources());
            LoadStringResources();
        }
        void LoadStringResources()
        {
            var rAssembly = Assembly.GetExecutingAssembly();
            using (var rStream = rAssembly.GetManifestResourceStream($"Sakuno.KanColle.Amatsukaze.Plugin.ImprovementArsenal.Resources.Strings.{Preference.Instance.Language}.xml"))
            using (var rReader = new StreamReader(rStream))
            {
                Strings = XDocument.Load(rReader).Root.Descendants("String").ToDictionary(r => r.Attribute("Key").Value, r => r.Value);
                OnPropertyChanged(nameof(Strings));
            }
        }
    }
}
