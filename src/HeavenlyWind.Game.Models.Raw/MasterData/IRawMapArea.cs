namespace Sakuno.KanColle.Amatsukaze.Game.Models.MasterData
{
    public interface IRawMapArea : IIdentifiable
    {
        string Name { get; }
        bool IsEvent { get; }
    }
}
