using System.Collections.Generic;
using System.Xml.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Layout
{
    public class TabLayout : LayoutBase
    {
        private protected override string TypeName => "Tab";
        public IList<LayoutBase> Children { get; } = new List<LayoutBase>();

        internal override void FromXml(XElement xml)
        {
            base.FromXml(xml);
            foreach (var element in xml.Elements())
                Children.Add(Resolve(element));
        }
        internal override XElement ToXml()
        {
            var element = base.ToXml();
            foreach (var child in Children)
                element.Add(child.ToXml());
            return element;
        }
    }
}
