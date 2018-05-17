namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class CachePreference : ModelBase
    {
        public Property<CacheMode> Mode { get; } = new Property<CacheMode>("cache.mode");

        public Property<string> Path { get; } = new Property<string>("cache.path", "Cache");
    }
}
