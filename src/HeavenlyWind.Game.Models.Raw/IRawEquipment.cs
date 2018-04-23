namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public interface IRawEquipment : IIdentifiable
    {
        int EquipmentInfoId { get; }
        bool IsLocked { get; }
        int ImprovementLevel { get; }
        int AirProficiency { get; }
    }
}
