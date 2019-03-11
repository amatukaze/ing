using System.Collections.Generic;

namespace Sakuno.ING.Game.Logger.Entities.Combat
{
    internal class BattleDetailEntity
    {
        public IReadOnlyList<ShipInBattleEntity> SortieFleetState { get; set; }
        public IReadOnlyList<ShipInBattleEntity> SortieFleet2State { get; set; }
        public IReadOnlyList<ShipInBattleEntity> SupportFleetState { get; set; }
        public IReadOnlyCollection<AirForceInBattle> LbasState { get; set; }
        public string LandBaseDefence { get; set; }
        public string FirstBattleDetail { get; set; }
        public string SecondBattleDetail { get; set; }
    }
}
