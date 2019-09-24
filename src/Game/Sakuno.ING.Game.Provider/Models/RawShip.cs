using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sakuno.ING.Game.Models.Knowledge;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models
{
    [Identifier]
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
        public override int GetHashCode() => value;
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
            api_exp.At(0),
            KnownLeveling.GetShipExp(api_lv),
            api_exp.At(0) + api_exp.At(1),
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
                Fuel = api_ndock_item.At(0),
                Steel = api_ndock_item.At(1)
            };

        [JsonProperty("api_cond")]
        public int Morale { get; internal set; }

        internal int[] api_kyouka;

        internal int[] api_karyoku;
        public ShipMordenizationStatus Firepower =>
            new ShipMordenizationStatus
            (
                max: api_karyoku.At(1),
                improved: api_kyouka.At(0),
                displaying: api_karyoku.At(0)
            );

        internal int[] api_raisou;
        public ShipMordenizationStatus Torpedo =>
            new ShipMordenizationStatus
            (
                improved: api_kyouka.At(1),
                displaying: api_raisou.At(0),
                max: api_raisou.At(1)
            );

        internal int[] api_taiku;
        public ShipMordenizationStatus AntiAir =>
            new ShipMordenizationStatus
            (
                improved: api_kyouka.At(2),
                displaying: api_taiku.At(0),
                max: api_taiku.At(1)
            );

        internal int[] api_soukou;
        public ShipMordenizationStatus Armor =>
            new ShipMordenizationStatus
            (
                improved: api_kyouka.At(3),
                displaying: api_soukou.At(0),
                max: api_soukou.At(1)
            );

        internal int[] api_kaihi;
        public ShipMordenizationStatus Evasion =>
            new ShipMordenizationStatus
            (
                displaying: api_kaihi.At(0),
                max: api_kaihi.At(1)
            );

        internal int[] api_taisen;
        public ShipMordenizationStatus AntiSubmarine =>
            new ShipMordenizationStatus
            (
                improved: api_kyouka.At(6),
                displaying: api_taisen.At(0),
                max: api_taisen.At(1)
            );

        internal int[] api_sakuteki;
        public ShipMordenizationStatus LineOfSight =>
            new ShipMordenizationStatus
            (
                displaying: api_sakuteki.At(0),
                max: api_sakuteki.At(1)
            );

        internal int[] api_lucky;
        public ShipMordenizationStatus Luck =>
            new ShipMordenizationStatus
            (
                improved: api_kyouka.At(4),
                displaying: api_lucky.At(0),
                max: api_lucky.At(1)
            );

        [JsonProperty("api_locked")]
        public bool IsLocked { get; internal set; }
        [JsonProperty("api_sally_area")]
        public int? ShipLockingTag { get; internal set; }
    }
}
