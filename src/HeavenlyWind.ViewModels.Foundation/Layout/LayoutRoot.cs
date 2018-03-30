using System.Collections.Generic;
using System.Xml;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Layout
{
    public sealed class LayoutRoot
    {
        public IList<LayoutBase> Entries { get; } = new List<LayoutBase>();

        public XmlDocument ToXml()
        {
            var document = new XmlDocument();
            document.AppendChild(document.CreateElement("LayoutRoot"));

            foreach (var entry in Entries)
                document.DocumentElement.AppendChild(entry.ToXml(document));

            return document;
        }

        public LayoutRoot FromXml(XmlDocument document)
        {
            foreach (XmlElement element in document.DocumentElement.ChildNodes)
                Entries.Add(LayoutBase.Resolve(element));
            return this;
        }
    }
}
