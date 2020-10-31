using Sakuno.ING.Game.Json.Converters;
using Sakuno.ING.Game.Models.MasterData;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Sakuno.ING.Game.Models
{
#nullable disable
    public sealed class RawShip
    {
        [JsonPropertyName("api_id")]
        public ShipId Id { get; set; }
        [JsonPropertyName("api_ship_id")]
        public ShipInfoId ShipInfoId { get; set; }

        public int api_lv { get; set; }
        public int[] api_exp { get; set; }

        public int api_nowhp { get; set; }
        public int api_maxhp { get; set; }

        [JsonPropertyName("api_soku")]
        public ShipSpeed Speed { get; set; }
        [JsonPropertyName("api_leng")]
        public FireRange FireRange { get; set; }

        [JsonPropertyName("api_slot")]
        public IReadOnlyList<SlotItemId?> SlotItemIds { get; set; }
        [JsonPropertyName("api_onslot")]
        public IReadOnlyList<int> PlaneCapacities { get; set; }

        public int api_slot_ex { get; set; }
        public bool IsExtraSlotAvailable => api_slot_ex != 0;
        public SlotItemId? ExtraSlotSlotItemId => api_slot_ex > 0 ? (SlotItemId?)api_slot_ex : null;

        [JsonPropertyName("api_fuel")]
        public int Fuel { get; set; }
        [JsonPropertyName("api_bull")]
        public int Bullet { get; set; }

        [JsonPropertyName("api_ndock_time")]
        [JsonConverter(typeof(TimeSpanInMinuteConverter))]
        public TimeSpan RepairTime { get; set; }
        public int[] api_ndock_item { get; set; }
        public Materials RepairConsumption => new Materials()
        {
            Fuel = api_ndock_item[0],
            Steel = api_ndock_item[1],
        };

        [JsonPropertyName("api_cond")]
        public int Morale { get; set; }

        public int[] api_kyouka { get; set; }

        public int[] api_karyoku { get; set; }
        public ShipModernizationStatus Firepower =>
            new ShipModernizationStatus
            (
                max: api_karyoku[1],
                improved: api_kyouka[0],
                displaying: api_karyoku[0]
            );

        public int[] api_raisou { get; set; }
        public ShipModernizationStatus Torpedo =>
            new ShipModernizationStatus
            (
                improved: api_kyouka[1],
                displaying: api_raisou[0],
                max: api_raisou[1]
            );

        public int[] api_taiku { get; set; }
        public ShipModernizationStatus AntiAir =>
            new ShipModernizationStatus
            (
                improved: api_kyouka[2],
                displaying: api_taiku[0],
                max: api_taiku[1]
            );

        public int[] api_soukou { get; set; }
        public ShipModernizationStatus Armor =>
            new ShipModernizationStatus
            (
                improved: api_kyouka[3],
                displaying: api_soukou[0],
                max: api_soukou[1]
            );

        public int[] api_kaihi { get; set; }
        public ShipModernizationStatus Evasion =>
            new ShipModernizationStatus
            (
                displaying: api_kaihi[0],
                max: api_kaihi[1]
            );

        public int[] api_taisen { get; set; }
        public ShipModernizationStatus AntiSubmarine =>
            new ShipModernizationStatus
            (
                improved: api_kyouka[6],
                displaying: api_taisen[0],
                max: api_taisen[1]
            );

        public int[] api_sakuteki { get; set; }
        public ShipModernizationStatus LineOfSight =>
            new ShipModernizationStatus
            (
                displaying: api_sakuteki[0],
                max: api_sakuteki[1]
            );

        public int[] api_lucky { get; set; }
        public ShipModernizationStatus Luck =>
            new ShipModernizationStatus
            (
                improved: api_kyouka[4],
                displaying: api_lucky[0],
                max: api_lucky[1]
            );

        [JsonPropertyName("api_locked")]
        [JsonConverter(typeof(IntToBooleanConverter))]
        public bool IsLocked { get; set; }

        [JsonPropertyName("api_sally_area")]
        public int? ShipLockingTagId { get; set; }
    }
#nullable enable
}
