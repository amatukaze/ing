using System.Collections.Generic;

namespace Sakuno.ING.Game.Models.Combat
{
    public struct RawSide
    {
        public Formation Formation { readonly get; set; }
        public IReadOnlyList<RawShipInBattle> Fleet { readonly get; set; }
        public IReadOnlyList<RawShipInBattle> Fleet2 { readonly get; set; }
        public Detection? Detection { readonly get; set; }
    }
}
