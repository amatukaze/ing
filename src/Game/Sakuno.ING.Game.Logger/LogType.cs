using System;

namespace Sakuno.ING.Game.Logger
{
    [Flags]
    public enum LogType
    {
        ShipCreation = 1,
        EquipmentCreation = 2,
        ExpeditionCompletion = 4,
    }
}
