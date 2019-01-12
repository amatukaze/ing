﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sakuno.ING.Game.Models.Knowledge;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models
{
    [Identifier(typeof(int))]
    public readonly struct ShipId : IEquatable<ShipId>, IComparable<ShipId>
    {
        private readonly int value;
        public ShipId(int value) => this.value = value;

        public int CompareTo(ShipId other) => value - other.value;
        public bool Equals(ShipId other) => value == other.value;

        public static implicit operator int(ShipId id) => id.value;
        public static explicit operator ShipId(int value) => new ShipId(value);

        public static bool operator ==(ShipId left, ShipId right) => left.value == right.value;
        public static bool operator !=(ShipId left, ShipId right) => left.value != right.value;
        public override bool Equals(object obj) => (ShipId)obj == this;
        public override int GetHashCode() => value.GetHashCode();
        public override string ToString() => value.ToString();
    }

    public sealed class RawShip : IIdentifiable<ShipId>
    {
        internal RawShip() { }

        [JsonProperty("api_id")]
        public ShipId Id { get; internal set; }
        [JsonProperty("api_ship_id")]
        public ShipInfoId ShipInfoId { get; internal set; }

        internal int api_lv;
        internal int[] api_exp;
        public Leveling Leveling => new Leveling(api_lv,
            api_exp.ElementAtOrDefault(0),
            KnownLeveling.GetShipExp(api_lv),
            api_exp.ElementAtOrDefault(0) + api_exp.ElementAtOrDefault(1),
            api_lv >= KnownLeveling.MaxShipLevel);

        internal int api_nowhp;
        internal int api_maxhp;
        public ShipHP HP => (api_nowhp, api_maxhp);

        [JsonProperty("api_soku")]
        public ShipSpeed Speed { get; internal set; }
        [JsonProperty("api_leng")]
        public FireRange FireRange { get; internal set; }
        [JsonProperty("api_slot")]
        public IReadOnlyList<EquipmentId?> EquipmentIds { get; internal set; }
        [JsonProperty("api_onslot")]
        public IReadOnlyList<int> SlotAircraft { get; internal set; }

        internal int api_slot_ex;
        public bool ExtraSlotOpened => api_slot_ex != 0;
        public EquipmentId? ExtraSlotEquipId => api_slot_ex > 0 ? (EquipmentId?)api_slot_ex : null;

        [JsonProperty("api_fuel")]
        public int CurrentFuel { get; internal set; }
        [JsonProperty("api_bull")]
        public int CurrentBullet { get; internal set; }
        internal int api_ndock_time;
        public TimeSpan RepairingTime => TimeSpan.FromMilliseconds(api_ndock_time);
        internal int[] api_ndock_item;
        public Materials RepairingCost
            => new Materials
            {
                Fuel = api_ndock_item.ElementAtOrDefault(0),
                Steel = api_ndock_item.ElementAtOrDefault(1)
            };

        [JsonProperty("api_cond")]
        public int Morale { get; internal set; }

        internal int[] api_kyouka;

        internal int[] api_karyoku;
        public ShipMordenizationStatus Firepower =>
            new ShipMordenizationStatus
            {
                Improved = api_kyouka.ElementAtOrDefault(0),
                Displaying = api_karyoku.ElementAtOrDefault(0),
                Max = api_karyoku.ElementAtOrDefault(1)
            };

        internal int[] api_raisou;
        public ShipMordenizationStatus Torpedo =>
            new ShipMordenizationStatus
            {
                Improved = api_kyouka.ElementAtOrDefault(1),
                Displaying = api_raisou.ElementAtOrDefault(0),
                Max = api_raisou.ElementAtOrDefault(1)
            };

        internal int[] api_taiku;
        public ShipMordenizationStatus AntiAir =>
            new ShipMordenizationStatus
            {
                Improved = api_kyouka.ElementAtOrDefault(2),
                Displaying = api_taiku.ElementAtOrDefault(0),
                Max = api_taiku.ElementAtOrDefault(1)
            };

        internal int[] api_soukou;
        public ShipMordenizationStatus Armor =>
            new ShipMordenizationStatus
            {
                Improved = api_kyouka.ElementAtOrDefault(3),
                Displaying = api_soukou.ElementAtOrDefault(0),
                Max = api_soukou.ElementAtOrDefault(1)
            };

        internal int[] api_kaihi;
        public ShipMordenizationStatus Evasion =>
            new ShipMordenizationStatus
            {
                Displaying = api_kaihi.ElementAtOrDefault(0),
                Max = api_kaihi.ElementAtOrDefault(1)
            };

        internal int[] api_taisen;
        public ShipMordenizationStatus AntiSubmarine =>
            new ShipMordenizationStatus
            {
                Improved = api_kyouka.ElementAtOrDefault(6),
                Displaying = api_taisen.ElementAtOrDefault(0),
                Max = api_taisen.ElementAtOrDefault(1)
            };

        internal int[] api_sakuteki;
        public ShipMordenizationStatus LineOfSight =>
            new ShipMordenizationStatus
            {
                Displaying = api_sakuteki.ElementAtOrDefault(0),
                Max = api_sakuteki.ElementAtOrDefault(1)
            };

        internal int[] api_lucky;
        public ShipMordenizationStatus Luck =>
            new ShipMordenizationStatus
            {
                Improved = api_kyouka.ElementAtOrDefault(4),
                Displaying = api_lucky.ElementAtOrDefault(0),
                Max = api_lucky.ElementAtOrDefault(1)
            };

        [JsonProperty("api_locked")]
        public bool IsLocked { get; internal set; }
        [JsonProperty("api_sally_area")]
        public int? ShipLockingTag { get; internal set; }
    }
}