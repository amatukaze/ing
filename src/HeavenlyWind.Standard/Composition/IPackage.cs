namespace Sakuno.KanColle.Amatsukaze.Composition
{
    public interface IPackage
    {
        string Id { get; }
        string Version { get; }
        bool IsLoaded { get; }
        bool IsEnabledAfterRestart { get; set; }
        PackageMetadata Metadata { get; }
    }
}
