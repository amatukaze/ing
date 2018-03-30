using System;
using System.Collections.Generic;
using System.Xml;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Layout
{
    public sealed class RelativeLayout : LayoutBase
    {
        private protected override string TypeName => "Relative";
        public IList<LayoutBase> Children { get; } = new List<LayoutBase>();
        public IList<RelativeRelation> Relations { get; } = new List<RelativeRelation>();

        internal override void FromXml(XmlElement xml)
        {
            base.FromXml(xml);
            foreach (XmlElement element in xml.ChildNodes)
                switch (element.Name)
                {
                    case "Children":
                        foreach (XmlElement child in element.ChildNodes)
                            Children.Add(Resolve(child));
                        break;
                    case "Relations":
                        foreach (XmlElement node in element.ChildNodes)
                        {
                            if (node.Name != "Relation")
                                throw new ArgumentException("Unknown element type.");
                            var relation = new RelativeRelation
                            {
                                Item = node.GetAttribute("Item"),
                                Type = (RelativeRelationType)Enum.Parse(typeof(RelativeRelationType), node.GetAttribute("Type"))
                            };
                            if (node.HasAttribute("Base"))
                                relation.Base = node.GetAttribute("Base");
                            Relations.Add(relation);
                        }
                        break;
                }
        }

        internal override XmlElement ToXml(XmlDocument document)
        {
            var xml = base.ToXml(document);

            var children = document.CreateElement("Children");
            xml.AppendChild(children);
            foreach (var child in Children)
                children.AppendChild(child.ToXml(document));

            var relations = document.CreateElement("Relations");
            xml.AppendChild(relations);
            foreach (var relation in Relations)
            {
                var node = document.CreateElement("Relation");
                relations.AppendChild(node);
                node.SetAttribute("Item", relation.Item);
                if (relation.Base != null)
                    node.SetAttribute("Base", relation.Base);
                node.SetAttribute("Type", relation.Type.ToString());
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
