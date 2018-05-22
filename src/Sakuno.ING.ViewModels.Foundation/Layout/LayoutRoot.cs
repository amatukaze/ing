using System.Collections.Generic;
using System.Xml.Linq;

namespace Sakuno.ING.ViewModels.Layout
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

        public static LayoutRoot FromXml(XDocument document)
        {
            var item = new LayoutRoot();
            foreach (var element in document.Element("LayoutRoot").Elements())
                item.Entries.Add(LayoutBase.Resolve(element));
            return item;
        }
    }
}
