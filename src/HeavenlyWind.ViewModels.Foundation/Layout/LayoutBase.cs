using System;
using System.Xml;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Layout
{
    public abstract class LayoutBase
    {
        public string Id { get; set; }
        public string Title { get; set; }
        private protected abstract string TypeName { get; }

        internal virtual void FromXml(XmlElement xml)
        {
            if (xml.HasAttribute("Id"))
                Id = xml.GetAttribute("Id");
            if (xml.HasAttribute("Title"))
                Title = xml.GetAttribute("Title");
        }

        internal virtual XmlElement ToXml(XmlDocument document)
        {
            var element = document.CreateElement(TypeName);
            if (Id != null)
                element.SetAttribute("Id", Id);
            if (Title != null)
                element.SetAttribute("Title", Title);
            return element;
        }

        internal static LayoutBase Resolve(XmlElement xml)
        {
            LayoutBase item;
            switch (xml.Name)
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
