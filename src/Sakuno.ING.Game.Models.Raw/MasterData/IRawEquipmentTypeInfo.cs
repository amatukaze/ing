namespace Sakuno.ING.Game.Models.MasterData
{
    public interface IRawEquipmentTypeInfo : IIdentifiable
    {
        string Name { get; }

        bool AvailableInExtraSlot { get; }
    }
}
