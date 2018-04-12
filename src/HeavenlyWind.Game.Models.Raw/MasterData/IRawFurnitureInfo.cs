namespace Sakuno.KanColle.Amatsukaze.Game.Models.MasterData
{
    public interface IRawFurnitureInfo
    {
        int Id { get; }
        int Type { get; }
        int CategoryNo { get; }
        string Name { get; }
        string Description { get; }

        int Rarity { get; }
        int Price { get; }
        bool IsSale { get; }
    }
}
