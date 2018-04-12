namespace Sakuno.KanColle.Amatsukaze.Game.Models.MasterData
{
    public interface IRawEquipmentTypeInfo
    {
        int Id { get; }
        string Name { get; }

        bool AvailableInExtraSlot { get; }
    }
}
