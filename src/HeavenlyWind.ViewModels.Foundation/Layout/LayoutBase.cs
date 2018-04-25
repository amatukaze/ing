using System;
using System.Xml.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Layout
{
    public abstract class LayoutBase
    {
        public string Id { get; set; }
        public string Title { get; set; }
        private protected abstract string TypeName { get; }

        internal virtual XElement ToXml()
        {
            var element = new XElement(TypeName);
            element.SetAttributeValue(nameof(Id), Id);
            element.SetAttributeValue(nameof(Title), Title);
            return element;
        }

        internal virtual void FromXml(XElement xml)
        {
            Id = xml.Attribute(nameof(Id))?.Value;
            Title = xml.Attribute(nameof(Title))?.Value;
        }

        internal static LayoutBase Resolve(XElement xml)
        {
            LayoutBase item;
            switch (xml.Name.LocalName)
            {
                case "Item":
                    item = new LayoutItem();
                    break;
                case "Relative":
                    item = new RelativeLayout();
                    break;
                case "Tab":
                    item = new TabLayout();
                    break;
                default:
                    throw new ArgumentException("Unknown element type.");
            }
            item.FromXml(xml);
            return item;
        }
    }
}
