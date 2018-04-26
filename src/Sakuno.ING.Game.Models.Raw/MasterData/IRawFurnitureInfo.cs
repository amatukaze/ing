namespace Sakuno.ING.Game.Models.MasterData
{
    public interface IRawFurnitureInfo : IIdentifiable
    {
        int Type { get; }
        int CategoryNo { get; }
        string Name { get; }
        string Description { get; }

        int Rarity { get; }
        int Price { get; }
        bool IsSale { get; }
    }
}
