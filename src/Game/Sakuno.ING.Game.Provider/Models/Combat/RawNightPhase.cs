using Sakuno.ING.Game.Json;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Combat
{
    public struct RawNightEffects
    {
        public int? FlareIndex { get; set; }
        public EquipmentInfoId? TouchingPlane { get; set; }
        public int? ActiveFleet { get; set; }
    }

    public class RawNightPhase : RawShellingPhase
    {
        private RawNightEffects ally, enemy;
        public ref readonly RawNightEffects Ally => ref ally;
        public ref readonly RawNightEffects Enemy => ref enemy;
        public RawNightPhase(BattleDetailJson.Shelling api, int[] touch, int[] flare, int[] active) : base(api)
        {
            ally.TouchingPlane = (EquipmentInfoId?)RawBattle.SelectPositive(touch, 0);
            enemy.TouchingPlane = (EquipmentInfoId?)RawBattle.SelectPositive(touch, 1);
            ally.FlareIndex = RawBattle.SelectPositive(flare, 0);
            enemy.FlareIndex = RawBattle.SelectPositive(flare, 1);
            ally.ActiveFleet = RawBattle.SelectPositive(active, 0);
            enemy.ActiveFleet = RawBattle.SelectPositive(active, 1);
        }
    }
}
