using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Layout
{
    public sealed class LayoutRoot
    {
        public ICollection<StandaloneLayout> Entries { get; } = new List<StandaloneLayout>();
    }
}
