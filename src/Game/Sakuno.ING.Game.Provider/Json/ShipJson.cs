using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.Knowledge;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Json
{
    internal class ShipJson : IRawShip
    {
        [JsonProperty("api_id")]
        public ShipId Id { get; set; }
        [JsonProperty("api_ship_id")]
        public ShipInfoId ShipInfoId { get; set; }

        public int api_lv;
        public int[] api_exp;
        public Leveling Leveling => new Leveling(api_lv,
            api_exp.ElementAtOrDefault(0),
            KnownLeveling.GetShipExp(api_lv),
            api_exp.ElementAtOrDefault(1),
            api_lv >= KnownLeveling.MaxShipLevel);

        public int api_nowhp;
        public int api_maxhp;
        public ClampedValue HP => (api_nowhp, api_maxhp);

        [JsonProperty("api_soku")]
        public ShipSpeed Speed { get; set; }
        [JsonProperty("api_leng")]
        public FireRange FireRange { get; set; }
        [JsonProperty("api_slot")]
        public IReadOnlyList<EquipmentId?> EquipmentIds { get; set; }
        [JsonProperty("api_onslot")]
        public IReadOnlyList<int> SlotAircraft { get; set; }

        public int api_slot_ex;
        public bool ExtraSlotOpened => api_slot_ex != 0;
        public EquipmentId? ExtraSlotEquipId => api_slot_ex > 0 ? (EquipmentId?)api_slot_ex : null;

        [JsonProperty("api_fuel")]
        public int CurrentFuel { get; set; }
        [JsonProperty("api_bull")]
        public int CurrentBullet { get; set; }
        public int api_ndock_time;
        public TimeSpan RepairingTime => TimeSpan.FromMilliseconds(api_ndock_time);
        public int[] api_ndock_item;
        public Materials RepairingCost
            => new Materials
            {
                Fuel = api_ndock_item.ElementAtOrDefault(0),
                Steel = api_ndock_item.ElementAtOrDefault(1)
            };

        [JsonProperty("api_cond")]
        public int Morale { get; set; }

        public int[] api_kyouka;

        public int[] api_karyoku;
        public ShipMordenizationStatus Firepower =>
            new ShipMordenizationStatus
            {
                Improved = api_kyouka.ElementAtOrDefault(0),
                Displaying = api_karyoku.ElementAtOrDefault(0),
                Max = api_karyoku.ElementAtOrDefault(1)
            };

        public int[] api_raisou;
        public ShipMordenizationStatus Torpedo =>
            new ShipMordenizationStatus
            {
                Improved = api_kyouka.ElementAtOrDefault(1),
                Displaying = api_raisou.ElementAtOrDefault(0),
                Max = api_raisou.ElementAtOrDefault(1)
            };

        public int[] api_taiku;
        public ShipMordenizationStatus AntiAir =>
            new ShipMordenizationStatus
            {
                Improved = api_kyouka.ElementAtOrDefault(2),
                Displaying = api_taiku.ElementAtOrDefault(0),
                Max = api_taiku.ElementAtOrDefault(1)
            };

        public int[] api_soukou;
        public ShipMordenizationStatus Armor =>
            new ShipMordenizationStatus
            {
                Improved = api_kyouka.ElementAtOrDefault(3),
                Displaying = api_soukou.ElementAtOrDefault(0),
                Max = api_soukou.ElementAtOrDefault(1)
            };

        public int[] api_kaihi;
        public ShipMordenizationStatus Evasion =>
            new ShipMordenizationStatus
            {
                Displaying = api_kaihi.ElementAtOrDefault(0),
                Max = api_kaihi.ElementAtOrDefault(1)
            };

        public int[] api_taisen;
        public ShipMordenizationStatus AntiSubmarine =>
            new ShipMordenizationStatus
            {
                Improved = api_kyouka.ElementAtOrDefault(6),
                Displaying = api_taisen.ElementAtOrDefault(0),
                Max = api_taisen.ElementAtOrDefault(1)
            };

        public int[] api_sakuteki;
        public ShipMordenizationStatus LightOfSight =>
            new ShipMordenizationStatus
            {
                Displaying = api_sakuteki.ElementAtOrDefault(0),
                Max = api_sakuteki.ElementAtOrDefault(1)
            };

        public int[] api_lucky;
        public ShipMordenizationStatus Luck =>
            new ShipMordenizationStatus
            {
                Improved = api_kyouka.ElementAtOrDefault(4),
                Displaying = api_lucky.ElementAtOrDefault(0),
                Max = api_lucky.ElementAtOrDefault(1)
            };

        [JsonProperty("api_locked")]
        public bool IsLocked { get; set; }
        [JsonProperty("api_sally_area")]
        public int? ShipLockingTag { get; set; }
    }
}
