using System;
using System.Collections.Generic;
using System.Linq;
using Sakuno.ING.Game.Json.Combat;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Combat
{
    public sealed class RawBattle
    {
        public Engagement Engagement { get; }

        private readonly RawSide ally, enemy;
        public ref readonly RawSide Ally => ref ally;
        public ref readonly RawSide Enemy => ref enemy;
        public IReadOnlyList<RawShipInBattle> NpcFleet { get; }

        public RawShellingPhase OpeningAswPhase { get; }
        public RawShellingPhase ShellingPhase1 { get; }
        public RawShellingPhase ShellingPhase2 { get; }
        public RawShellingPhase ShellingPhase3 { get; }
        public RawShellingPhase NightPhase { get; }
        public RawShellingPhase NightPhase1 { get; }
        public RawShellingPhase NightPhase2 { get; }

        public RawTorpedoPhase OpeningTorpedorPhase { get; }
        public RawTorpedoPhase ClosingTorpedorPhase { get; }

        public RawAerialPhase AerialPhase { get; }
        public RawAerialPhase AerialPhase2 { get; }
        public RawAerialPhase JetPhase { get; }
        public RawAerialPhase LandBaseJetPhase { get; }
        public IReadOnlyList<RawLandBaseAerialPhase> LandBasePhases { get; }

        public SupportFireType? SupportFireType { get; }
        public IReadOnlyList<ShipId> SupportFleet { get; }
        public RawAerialPhase AerialSupportPhase { get; }
        public RawSupportPhase SupportPhase { get; }
        public RawShellingPhase NpcPhase { get; }

        public RawBattle(BattleApi api)
        {
            // Fleets

            ally.Formation = (Formation)api.api_formation.ElementAtOrDefault(0);
            enemy.Formation = (Formation)api.api_formation.ElementAtOrDefault(1);
            Engagement = (Engagement)api.api_formation.ElementAtOrDefault(2);
            ally.NightTouchingId = (EquipmentInfoId?)SelectPositive(api.api_touch_plane, 0);
            enemy.NightTouchingId = (EquipmentInfoId?)SelectPositive(api.api_touch_plane, 1);
            ally.FlareIndex = SelectPositive(api.api_flare_pos, 0);
            enemy.FlareIndex = SelectPositive(api.api_flare_pos, 1);
            ally.ActiveFleetId = SelectPositive(api.api_active_deck, 0);
            enemy.ActiveFleetId = SelectPositive(api.api_active_deck, 1);

            if (api.api_maxhps != null) // Deprecated from 17Q4
            {
                ally.Fleet = SelectFleet(api.api_maxhps.AsSpan(1, 6), api.api_nowhps.AsSpan(1, 6), api.api_fParam);
                enemy.Fleet = SelectFleet(api.api_maxhps.AsSpan(7, 6), api.api_nowhps.AsSpan(7, 6), api.api_eParam, api.api_ship_ke.AsSpan(1), api.api_ship_lv.AsSpan(1), api.api_eSlot, api.api_eKyouka);
            }
            if (api.api_maxhps_combined != null)
                ally.Fleet2 = SelectFleet(api.api_maxhps_combined.AsSpan(1), api.api_nowhps_combined.AsSpan(1), api.api_fParam_combined);

            if (api.api_f_maxhps != null)
                ally.Fleet = SelectFleet(api.api_f_maxhps, api.api_f_nowhps, api.api_fParam);
            if (api.api_f_maxhps_combined != null)
                ally.Fleet2 = SelectFleet(api.api_f_maxhps_combined, api.api_f_nowhps_combined, api.api_fParam_combined);
            if (api.api_e_maxhps != null)
                enemy.Fleet = SelectFleet(api.api_e_maxhps, api.api_e_nowhps, api.api_eParam, api.api_ship_ke, api.api_ship_lv, api.api_eSlot);
            if (api.api_e_maxhps_combined != null)
                enemy.Fleet2 = SelectFleet(api.api_e_maxhps_combined, api.api_e_nowhps_combined, api.api_eParam_combined, api.api_ship_ke_combined, api.api_ship_lv_combined, api.api_eSlot_combined);

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
                NightPhase = new RawShellingPhase(api.api_hougeki);
            if (api.api_n_hougeki1 != null)
                NightPhase1 = new RawShellingPhase(api.api_n_hougeki1);
            if (api.api_n_hougeki2 != null)
                NightPhase2 = new RawShellingPhase(api.api_n_hougeki2);

            if (api.api_opening_atack != null)
                OpeningTorpedorPhase = new RawTorpedoPhase(api.api_opening_atack);
            if (api.api_raigeki != null)
                ClosingTorpedorPhase = new RawTorpedoPhase(api.api_raigeki);

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

            SupportFireType = (SupportFireType?)(api.api_support_flag ?? api.api_n_support_flag);
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
                    NpcPhase = new RawShellingPhase(api.api_friendly_battle.api_hougeki);
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

        private static IReadOnlyList<RawShipInBattle> SelectFleet(ReadOnlySpan<int> maxhp, ReadOnlySpan<int> nowhp, int[][] param, ReadOnlySpan<int> id = default, ReadOnlySpan<int> lv = default, int[][] slot = null, int[][] upgrade = null)
        {
            var result = new List<RawShipInBattle>(maxhp.Length);
            for (int i = 0; i < maxhp.Length; i++)
            {
                if (maxhp[i] <= 0) continue;
                var p = param.ElementAtOrDefault(i);
                var u = upgrade.ElementAtOrDefault(i);
                var s = new RawShipInBattle
                {
                    HP = (nowhp[i], maxhp[i]),
                    Firepower = p.ElementAtOrDefault(0) + u.ElementAtOrDefault(0),
                    Torpedo = p.ElementAtOrDefault(1) + u.ElementAtOrDefault(1),
                    AntiAir = p.ElementAtOrDefault(2) + u.ElementAtOrDefault(2),
                    Armor = p.ElementAtOrDefault(3) + u.ElementAtOrDefault(3),
                    Equipment = slot.ElementAtOrDefault(i)?.Where(x => x > 0).Select(x => (EquipmentInfoId)x).ToArray()
                };
                if (id != default) s.Id = (ShipInfoId)id[i];
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
