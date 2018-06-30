namespace Sakuno.ING.Game.Logger
{
    public interface ILogMigrator : IIdentifiable<string>
    {
        bool RequireFolder { get; }
    }
}
