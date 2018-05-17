using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    [Flags]
    public enum CombinedFleetType
    {
        None = 0,
        CarrierTaskForce = 1,
        SurfaceTaskForce = 1 << 1,
        TransportEscort = 1 << 2,
    }
}
