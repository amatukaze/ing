using System.Collections.Generic;
using System.Text.Json;

namespace Sakuno.ING.Game.Logger.Entities.Combat
{
    public class UnstoredBattleDetail
    {
        public IReadOnlyList<ShipInBattleEntity> SortieFleetState { get; set; }
        public IReadOnlyList<ShipInBattleEntity> SortieFleet2State { get; set; }
        public IReadOnlyList<ShipInBattleEntity> SupportFleet2State { get; set; }
        public IReadOnlyList<AirForceInBattle> LbasState { get; set; }
        internal JsonElement LandBaseDefence { get; set; }
        internal JsonElement FirstBattleDetail { get; set; }
        internal JsonElement SecondBattleDetail { get; set; }
    }
}
