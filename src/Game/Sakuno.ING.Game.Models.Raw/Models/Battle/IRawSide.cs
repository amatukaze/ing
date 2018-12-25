using System.Collections.Generic;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Battle
{
    public interface IRawSide
    {
        Formation Formation { get; }
        IReadOnlyList<IRawShipInBattle> Fleet { get; }
        IReadOnlyList<IRawShipInBattle> Fleet2 { get; }
        Detection? Detection { get; }
        EquipmentInfoId? NightTouchingId { get; }
        int? FlareIndex { get; }
        int? ActiveFleetId { get; }
    }
}
