namespace Sakuno.KanColle.Amatsukaze.Extensibility
{
    public interface IPreference
    {
        string Name { get; }

        object View { get; }
    }
}
