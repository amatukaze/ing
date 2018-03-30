using System.Collections.Generic;
using System.Xml;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Layout
{
    public class TabLayout : LayoutBase
    {
        private protected override string TypeName => "Tab";
        public IList<LayoutBase> Children { get; } = new List<LayoutBase>();

        internal override void FromXml(XmlElement xml)
        {
            base.FromXml(xml);
            foreach (XmlElement element in xml.ChildNodes)
                Children.Add(Resolve(element));
        }
        internal override XmlElement ToXml(XmlDocument document)
        {
            var element = base.ToXml(document);
            foreach (var child in Children)
                element.AppendChild(child.ToXml(document));
            return element;
        }
    }
}
