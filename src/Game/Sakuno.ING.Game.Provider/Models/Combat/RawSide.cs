using System.Collections.Generic;

namespace Sakuno.ING.Game.Models.Combat
{
    public struct RawSide
    {
        public Formation Formation { get; set; }
        public IReadOnlyList<RawShipInBattle> Fleet { get; set; }
        public IReadOnlyList<RawShipInBattle> Fleet2 { get; set; }
        public Detection? Detection { get; set; }
    }
}
