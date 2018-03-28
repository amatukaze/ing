namespace Sakuno.KanColle.Amatsukaze.ViewModels.Layout
{
    public sealed class StandaloneLayout : ILayoutElement
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public ILayoutElement Content { get; set; }
    }
}
