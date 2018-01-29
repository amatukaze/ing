namespace Sakuno.KanColle.Amatsukaze.Data.Json
{
    public interface IJsonObjectNode : IJsonNode
    {
        IBindableCollection<IJsonProperty> Properties { get; }
    }
}
