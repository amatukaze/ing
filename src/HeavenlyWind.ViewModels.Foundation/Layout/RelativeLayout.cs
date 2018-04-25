using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Layout
{
    public sealed class RelativeLayout : LayoutBase
    {
        private protected override string TypeName => "Relative";
        public IList<LayoutBase> Children { get; } = new List<LayoutBase>();
        public IList<RelativeRelation> Relations { get; } = new List<RelativeRelation>();

        internal override void FromXml(XElement xml)
        {
            base.FromXml(xml);

            var children = xml.Element(nameof(Children));
            if (children != null)
                foreach (var child in children.Elements())
                    Children.Add(Resolve(child));

            var relations = xml.Element(nameof(Relations));
            if (relations != null)
                foreach (var relation in relations.Elements())
                {
                    if (relation.Name.LocalName != "Relation")
                        throw new ArgumentException("Unknown element type.");
                    Relations.Add(new RelativeRelation
                    {
                        Item = relation.Attribute(nameof(RelativeRelation.Item)).Value,
                        Type = (RelativeRelationType)Enum.Parse(typeof(RelativeRelationType), relation.Attribute(nameof(RelativeRelation.Type)).Value),
                        Base = relation.Attribute(nameof(RelativeRelation.Base))?.Value
                    });
                }
        }

        internal override XElement ToXml()
        {
            var xml = base.ToXml();

            var children = new XElement(nameof(Children));
            xml.Add(children);
            foreach (var child in Children)
                children.Add(child.ToXml());

            var relations = new XElement(nameof(Relations));
            xml.Add(relations);
            foreach (var relation in Relations)
            {
                var node = new XElement("Relation");
                relations.Add(node);
                node.SetAttributeValue(nameof(RelativeRelation.Item), relation.Item);
                node.SetAttributeValue(nameof(RelativeRelation.Base), relation.Base);
                node.SetAttributeValue(nameof(RelativeRelation.Type), relation.Type.ToString());
            }

            return xml;
        }
    }

    public sealed class RelativeRelation
    {
        public string Item { get; set; }
        public string Base { get; set; }
        public RelativeRelationType Type { get; set; }
    }

    public enum RelativeRelationType
    {
        LeftOf,
        RightOf,
        Above,
        Below,
        AlignLeft,
        AlignRight,
        AlignTop,
        AlignBottom,
        AlignHorizontal,
        AlignVertical
    }
}
