namespace Sakuno.ING.Game.Logger
{
    public interface ILogMigrator : IIdentifiable<string>
    {
        string Title { get; }
        bool RequireFolder { get; }
    }
}
