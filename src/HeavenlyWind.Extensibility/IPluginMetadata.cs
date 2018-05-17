namespace Sakuno.KanColle.Amatsukaze.Extensibility
{
    public interface IPluginMetadata
    {
        string Guid { get; }

        string Name { get; }
        string Author { get; }
        string Version { get; }
    }
}
