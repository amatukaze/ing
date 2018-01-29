namespace Sakuno.KanColle.Amatsukaze.Data.Json
{
    public interface IJsonArrayNode : IJsonNode
    {
        IBindableCollection<IJsonNode> Children { get; }
    }
}
