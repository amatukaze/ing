using System.Collections.Generic;
using System.Linq;
using Sakuno.ING.Game.Json;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Combat
{
    public class RawAerialPhase : RawBattlePhase
    {
        public AirFightingResult? FightingResult { get; }
        private readonly RawAerialSide ally, enemy;
        public ref readonly RawAerialSide Ally => ref ally;
        public ref readonly RawAerialSide Enemy => ref enemy;
        public RawAntiAirFire AntiAirFire { get; }
        public RawAerialPhase(BattleDetailJson.Aerial api)
            : base(SelectStage3(api.api_stage3, 0).Concat(SelectStage3(api.api_stage3_combined, 6)))
        {
            if (api.api_stage1 != null)
            {
                FightingResult = (AirFightingResult?)api.api_stage1.api_disp_seiku;
                ally.FightedPlanes = (api.api_stage1.api_f_lostcount, api.api_stage1.api_f_count);
                enemy.FightedPlanes = (api.api_stage1.api_e_lostcount, api.api_stage1.api_e_count);
                ally.PlanesFrom = api.api_plane_from.At(0);
                enemy.PlanesFrom = api.api_plane_from.At(1);
                ally.TouchingPlane = (EquipmentInfoId?)RawBattle.SelectPositive(api.api_stage1.api_touch_plane, 0);
                enemy.TouchingPlane = (EquipmentInfoId?)RawBattle.SelectPositive(api.api_stage1.api_touch_plane, 1);
            }
            if (api.api_stage2 != null)
            {
                ally.ShootedPlanes = (api.api_stage2.api_f_lostcount, api.api_stage2.api_f_count);
                enemy.ShootedPlanes = (api.api_stage2.api_e_lostcount, api.api_stage2.api_e_count);
                if (api.api_stage2.api_air_fire != null)
                    AntiAirFire = new RawAntiAirFire(api.api_stage2.api_air_fire.api_idx, api.api_stage2.api_air_fire.api_kind,
                        api.api_stage2.api_air_fire.api_use_items.Select(x => (EquipmentInfoId)x).ToArray());
            }
        }

        private static IEnumerable<RawAttack> SelectStage3(BattleDetailJson.Aerial.Stage3 stage3, int indexBase)
            => stage3 is null ?
                Enumerable.Empty<RawAttack>() :
                SelectAerial(stage3.api_frai_flag, stage3.api_fbak_flag, stage3.api_fdam, stage3.api_fcl_flag, true, indexBase)
                    .Concat(SelectAerial(stage3.api_erai_flag, stage3.api_ebak_flag, stage3.api_edam, stage3.api_ecl_flag, false, indexBase));

        private static IEnumerable<RawAttack> SelectAerial(bool[] torpedoed, bool[] bombed, decimal[] damages, int[] criticals, bool enemyAttacks, int indexBase)
        {
            if (torpedoed is null || bombed is null || damages is null || criticals is null)
                yield break;
            int offset = damages[0] < 0 ? -1 : 0;
            for (int i = -offset; i < damages.Length; i++)
            {
                int typeNumber = (torpedoed.At(i) ? TorpedoFlag : 0) | (bombed.At(i) ? BombFlag : 0);
                if (typeNumber == 0 && damages.At(i) == 0)
                    continue;
                yield return new SingleAttack(null, enemyAttacks, typeNumber, null, new RawHit(i + offset + indexBase, damages.At(i), criticals.At(i)));
            }
        }

        public const int BombFlag = 1;
        public const int TorpedoFlag = 2;
    }
}
