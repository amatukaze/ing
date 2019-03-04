using System;
using System.Collections.Generic;
using System.Linq;
using Sakuno.ING.Game.Json;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Combat
{
    public sealed class RawBattle
    {
        public static readonly DateTimeOffset EnemyIdChangeTime = new DateTimeOffset(2017, 4, 5, 3, 0, 0, TimeSpan.FromHours(9));
        public Engagement Engagement { get; }

        private readonly RawSide ally, enemy;
        public ref readonly RawSide Ally => ref ally;
        public ref readonly RawSide Enemy => ref enemy;
        public IReadOnlyList<RawShipInBattle> NpcFleet { get; }
        public bool HasNextPart { get; }

        public RawShellingPhase OpeningAswPhase { get; }
        public RawShellingPhase ShellingPhase1 { get; }
        public RawShellingPhase ShellingPhase2 { get; }
        public RawShellingPhase ShellingPhase3 { get; }
        public RawNightPhase NightPhase { get; }
        public RawNightPhase NightPhase1 { get; }
        public RawNightPhase NightPhase2 { get; }

        public RawTorpedoPhase OpeningTorpedoPhase { get; }
        public RawTorpedoPhase ClosingTorpedoPhase { get; }

        public RawAerialPhase AerialPhase { get; }
        public RawAerialPhase AerialPhase2 { get; }
        public RawAerialPhase JetPhase { get; }
        public RawAerialPhase LandBaseJetPhase { get; }
        public IReadOnlyList<RawLandBaseAerialPhase> LandBasePhases { get; }

        public SupportFireType SupportFireType { get; }
        public IReadOnlyList<ShipId> SupportFleet { get; }
        public RawAerialPhase AerialSupportPhase { get; }
        public RawSupportPhase SupportPhase { get; }
        public RawNightPhase NpcPhase { get; }

        public RawBattle(BattleDetailJson api, bool fixEnemyId = false)
        {
            // Fleets

            ally.Formation = (Formation)api.api_formation.At(0);
            enemy.Formation = (Formation)api.api_formation.At(1);
            Engagement = (Engagement)api.api_formation.At(2);
            ally.Detection = api.api_search.At(0);
            enemy.Detection = api.api_search.At(1);
            HasNextPart = api.api_midnight_flag;

            if (api.api_maxhps != null) // Deprecated from 17Q4
            {
                ally.Fleet = SelectFleet(api.api_maxhps.AsSpan(1, 6), api.api_nowhps.AsSpan(1, 6), api.api_fParam);
                enemy.Fleet = SelectFleet(api.api_maxhps.AsSpan(7, 6), api.api_nowhps.AsSpan(7, 6), api.api_eParam, api.api_ship_ke.AsSpan(1), api.api_ship_lv.AsSpan(1), api.api_eSlot, api.api_eKyouka, fixEnemyId);
            }
            if (api.api_maxhps_combined != null)
                ally.Fleet2 = SelectFleet(api.api_maxhps_combined.AsSpan(1), api.api_nowhps_combined.AsSpan(1), api.api_fParam_combined);

            if (api.api_f_maxhps != null)
                ally.Fleet = SelectFleet(api.api_f_maxhps, api.api_f_nowhps, api.api_fParam);
            if (api.api_f_maxhps_combined != null)
                ally.Fleet2 = SelectFleet(api.api_f_maxhps_combined, api.api_f_nowhps_combined, api.api_fParam_combined);
            if (api.api_e_maxhps != null)
                enemy.Fleet = SelectFleet(api.api_e_maxhps, api.api_e_nowhps, api.api_eParam, api.api_ship_ke, api.api_ship_lv, api.api_eSlot, fixEnemyId: fixEnemyId);
            if (api.api_e_maxhps_combined != null)
                enemy.Fleet2 = SelectFleet(api.api_e_maxhps_combined, api.api_e_nowhps_combined, api.api_eParam_combined, api.api_ship_ke_combined, api.api_ship_lv_combined, api.api_eSlot_combined, fixEnemyId: fixEnemyId);

            if (api.api_friendly_info != null)
                NpcFleet = SelectFleet(api.api_friendly_info.api_maxhps, api.api_friendly_info.api_nowhps, api.api_friendly_info.api_Param, api.api_friendly_info.api_ship_id, api.api_friendly_info.api_ship_lv, api.api_friendly_info.api_Slot);

            SetEscaped(ally.Fleet, api.api_escape_idx);
            SetEscaped(ally.Fleet2, api.api_escape_idx_combined);

            // Phases

            if (api.api_hougeki1 != null)
                ShellingPhase1 = new RawShellingPhase(api.api_hougeki1);
            if (api.api_hougeki2 != null)
                ShellingPhase2 = new RawShellingPhase(api.api_hougeki2);
            if (api.api_hougeki3 != null)
                ShellingPhase3 = new RawShellingPhase(api.api_hougeki3);
            if (api.api_opening_taisen != null)
                OpeningAswPhase = new RawShellingPhase(api.api_opening_taisen);

            if (api.api_hougeki != null)
                NightPhase = new RawNightPhase(api.api_hougeki, api.api_touch_plane, api.api_flare_pos, api.api_active_deck);
            if (api.api_n_hougeki1 != null)
                NightPhase1 = new RawNightPhase(api.api_n_hougeki1, api.api_touch_plane, api.api_flare_pos, api.api_active_deck);
            if (api.api_n_hougeki2 != null)
                NightPhase2 = new RawNightPhase(api.api_n_hougeki2, api.api_touch_plane, api.api_flare_pos, api.api_active_deck);

            if (api.api_opening_atack != null)
                OpeningTorpedoPhase = new RawTorpedoPhase(api.api_opening_atack);
            if (api.api_raigeki != null)
                ClosingTorpedoPhase = new RawTorpedoPhase(api.api_raigeki);

            if (api.api_kouku != null)
                AerialPhase = new RawAerialPhase(api.api_kouku);
            if (api.api_kouku2 != null)
                AerialPhase2 = new RawAerialPhase(api.api_kouku2);
            if (api.api_injection_kouku != null)
                JetPhase = new RawAerialPhase(api.api_injection_kouku);
            if (api.api_air_base_injection != null)
                LandBaseJetPhase = new RawAerialPhase(api.api_air_base_injection);
            LandBasePhases = api.api_air_base_attack?.Select(x => new RawLandBaseAerialPhase(x)).ToArray() ?? Array.Empty<RawLandBaseAerialPhase>();

            // Support

            SupportFireType = (api.api_support_flag == SupportFireType.None) ? api.api_n_support_flag : api.api_support_flag;
            var support = api.api_support_info ?? api.api_n_support_info;
            if (support != null)
            {
                SupportFleet = (support.api_support_airatack?.api_ship_id ??
                    support.api_support_hourai?.api_ship_id)
                    .Select(x => (ShipId)x).ToArray();
                if (support.api_support_airatack != null)
                    AerialSupportPhase = new RawAerialPhase(support.api_support_airatack);
                if (support.api_support_hourai != null)
                    SupportPhase = new RawSupportPhase(support.api_support_hourai);
            }

            if (api.api_friendly_battle != null)
            {
                if (api.api_friendly_battle.api_hougeki != null)
                    NpcPhase = new RawNightPhase(api.api_friendly_battle.api_hougeki, null, api.api_friendly_battle.api_flare_pos, null);
            }
        }

        internal static int? SelectPositive(int[] array, int index)
        {
            if (array == null) return null;
            if (array.Length <= index) return null;
            int i = array[index];
            if (i <= 0) return null;
            return i;
        }

        private static IReadOnlyList<RawShipInBattle> SelectFleet(ReadOnlySpan<int> maxhp, ReadOnlySpan<int> nowhp, int[][] param, ReadOnlySpan<int> id = default, ReadOnlySpan<int> lv = default, int[][] slot = null, int[][] upgrade = null, bool fixEnemyId = false)
        {
            var result = new List<RawShipInBattle>(maxhp.Length);
            for (int i = 0; i < maxhp.Length; i++)
            {
                if (maxhp[i] <= 0) continue;
                var p = param.At(i);
                var u = upgrade.At(i);
                var s = new RawShipInBattle
                {
                    HP = (nowhp[i], maxhp[i]),
                    Firepower = p.At(0) + u.At(0),
                    Torpedo = p.At(1) + u.At(1),
                    AntiAir = p.At(2) + u.At(2),
                    Armor = p.At(3) + u.At(3),
                    Equipment = slot.At(i)?.Where(x => x > 0).Select(x => (EquipmentInfoId)x).ToArray()
                };
                if (id != default)
                {
                    int sid = id[i];
                    if (fixEnemyId && sid > 500)
                        sid += 1000;
                    s.Id = (ShipInfoId)sid;
                }
                if (lv != default) s.Level = lv[i];
            }
            return result;
        }

        private static void SetEscaped(IReadOnlyList<RawShipInBattle> fleet, int[] idx)
        {
            if (fleet is null || idx is null) return;
            foreach (int i in idx)
                if (i > 0)
                    fleet[i - 1].IsEscaped = true;
        }
    }
}
