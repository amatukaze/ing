using System;

namespace Sakuno.ING.Game.Models.MasterData
{
    [Flags]
    public enum AbyssalShipClass
    {
        None = 0,
        Elite = 1 << 0,
        Flagship = 1 << 1,
        /// <summary>改</summary>
        Remodel = 1 << 2,
        /// <summary>後期型</summary>
        LateModel = 1 << 3,
    }
}
