namespace Sakuno.ING.Game.Models
{
    public interface IRawAirForceSquadron : IIdentifiable
    {
        EquipmentId? EquipmentId { get; }
        ClampedValue AircraftCount { get; }
        SquadronMorale Morale { get; }
    }

    public enum SquadronMorale
    {
        Normal = 1,
        Yellow = 2,
        Red = 3
    }
}
