using System.Collections.Generic;
using System.Xml.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Layout
{
    public sealed class LayoutRoot
    {
        public IList<LayoutBase> Entries { get; } = new List<LayoutBase>();

        public XDocument ToXml()
        {
            var document = new XDocument();
            var root = new XElement("LayoutRoot");
            document.Add(root);

            foreach (var entry in Entries)
                root.Add(entry.ToXml());

            return document;
        }

        public LayoutRoot FromXml(XDocument document)
        {
            foreach (var element in document.Element("LayoutRoot").Elements())
                Entries.Add(LayoutBase.Resolve(element));
            return this;
        }
    }
}
