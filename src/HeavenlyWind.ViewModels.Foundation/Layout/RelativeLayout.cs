using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Layout
{
    public sealed class RelativeLayout : ILayoutElement
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public ICollection<ILayoutElement> Children { get; } = new List<ILayoutElement>();
        public ICollection<ILayoutElement> Relations { get; } = new List<ILayoutElement>();
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
        Bottom,
        AlignLeft,
        AlignRight,
        AlignTop,
        AlignBottom,
        AlignHorizontal,
        AlignVertical
    }
}
