namespace Sakuno.ING.Game
{
    public sealed class TextTranslationDescriptor
    {
        public TextTranslationDescriptor(int id, string category, string origin, bool preferOrigin = true)
        {
            Id = id;
            Category = category;
            Origin = origin;
            PreferOrigin = preferOrigin;
        }

        public int Id { get; }
        public string Category { get; }
        public string Origin { get; }
        public bool PreferOrigin { get; }
    }
}
