namespace Sakuno.KanColle.Amatsukaze.Extensibility
{
    public interface IToolPane
    {
        string Name { get; }

        object View { get; }
    }
}
