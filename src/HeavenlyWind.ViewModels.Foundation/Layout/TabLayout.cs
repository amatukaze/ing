using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Layout
{
    public class TabLayout : ILayoutElement
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public ICollection<ILayoutElement> Children { get; } = new List<ILayoutElement>();
    }
}
