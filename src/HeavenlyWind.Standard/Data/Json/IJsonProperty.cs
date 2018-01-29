namespace Sakuno.KanColle.Amatsukaze.Data.Json
{
    public interface IJsonProperty
    {
        string Name { get; }
        IJsonNode Value { get; }
    }
}
