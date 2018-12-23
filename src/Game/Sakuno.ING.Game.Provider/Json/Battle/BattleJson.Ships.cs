using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.Battle;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Json.Battle
{
    internal partial class BattleJson : IRawBattle
    {
        public int[] api_formation;
        public Engagement Engagement => (Engagement)api_formation.ElementAtOrDefault(2);

        public int[] api_f_nowhps;
        public int[] api_f_maxhps;
        public int[][] api_fParam;

        public int[] api_f_nowhps_combined;
        public int[] api_f_maxhps_combined;
        public int[][] api_fParam_combined;

        public ShipInfoId[] api_ship_ke;
        public int[] api_ship_lv;
        public int[] api_e_nowhps;
        public int[] api_e_maxhps;
        public EquipmentInfoId[][] api_eSlot;
        public int[][] api_eParam;

        public ShipInfoId[] api_ship_ke_combined;
        public int[] api_ship_lv_combined;
        public int[] api_e_nowhps_combined;
        public int[] api_e_maxhps_combined;
        public EquipmentInfoId[][] api_eSlot_combined;
        public int[][] api_eParam_combined;

        public int[] api_escape_idx;
        public IReadOnlyList<int> EscapedIndices => api_escape_idx.Select(x => x - 1).ToArray();

        private class Ship : IRawShipInBattle
        {
            public ShipInfoId Id { get; set; }
            public int Level { get; set; }
            public ClampedValue HP { get; set; }
            public IReadOnlyList<EquipmentInfoId> Equipments { get; set; }
            public int Firepower { get; set; }
            public int Torpedo { get; set; }
            public int AntiAir { get; set; }
            public int Armor { get; set; }
        }

        internal class ShipCollection : IReadOnlyList<IRawShipInBattle>
        {
            private readonly int[] nowhp;
            private readonly int[] maxhp;
            private readonly int[][] param;
            private readonly ShipInfoId[] id;
            private readonly int[] lv;
            private readonly EquipmentInfoId[][] slot;

            public ShipCollection(int[] nowhp, int[] maxhp, int[][] param, ShipInfoId[] id, int[] lv, EquipmentInfoId[][] slot)
            {
                this.nowhp = nowhp;
                this.maxhp = maxhp;
                this.param = param;
                this.id = id;
                this.lv = lv;
                this.slot = slot;
            }

            public IRawShipInBattle this[int index] => new Ship
            {
                Id = id.ElementAtOrDefault(index),
                Level = lv.ElementAtOrDefault(index),
                HP = (nowhp.ElementAtOrDefault(index), maxhp.ElementAtOrDefault(index)),
                Equipments = slot.ElementAtOrDefault(index)?.Where(x => x >= 0).ToArray(),
                Firepower = param.ElementAtOrDefault(index).ElementAtOrDefault(0),
                Torpedo = param.ElementAtOrDefault(index).ElementAtOrDefault(1),
                AntiAir = param.ElementAtOrDefault(index).ElementAtOrDefault(2),
                Armor = param.ElementAtOrDefault(index).ElementAtOrDefault(3),
            };

            public int Count { get; }

            public IEnumerator<IRawShipInBattle> GetEnumerator()
            {
                for (int i = 0; i < Count; i++)
                    yield return this[i];
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public class Friendly
        {
            public ShipInfoId[] api_ship_id;
            public int[] api_ship_lv;
            public int[] api_nowhps;
            public int[] api_maxhps;
            public EquipmentInfoId[][] api_Slot;
            public int[][] api_Param;
            internal ShipCollection ToFleet() => new ShipCollection
            (
                api_nowhps,
                api_maxhps,
                api_Param,
                api_ship_id,
                api_ship_lv,
                api_Slot
            );
        }
        public Friendly api_friendly_info;
        public IReadOnlyList<IRawShipInBattle> NpcFleet
            => api_friendly_info?.ToFleet();

        public Detection[] api_search;
        public int[] api_touch_plane;
        public int[] api_flare_pos;
        private int? GetFromArray(int[] array, int index)
        {
            int result = array.ElementAtOrDefault(index);
            if (result <= 0) return null;
            else return result;
        }

        public RawSide Ally => new RawSide
        (
            (Formation)api_formation.ElementAtOrDefault(0),
            new ShipCollection(api_f_nowhps, api_f_maxhps, api_fParam, null, null, null),
            new ShipCollection(api_f_nowhps_combined, api_f_maxhps_combined, api_fParam_combined, null, null, null),
            api_search?.ElementAtOrDefault(0),
            (EquipmentInfoId?)GetFromArray(api_touch_plane, 0),
            GetFromArray(api_flare_pos, 0) - 1
        );

        public RawSide Enemy => new RawSide
        (
            (Formation)api_formation.ElementAtOrDefault(1),
            new ShipCollection(api_e_nowhps, api_e_maxhps, api_eParam, api_ship_ke, api_ship_lv, api_eSlot),
            new ShipCollection(api_e_nowhps_combined, api_e_maxhps_combined, api_eParam_combined, api_ship_ke_combined, api_ship_lv_combined, api_eSlot_combined),
            api_search?.ElementAtOrDefault(1),
            (EquipmentInfoId?)GetFromArray(api_touch_plane, 1),
            GetFromArray(api_flare_pos, 1) - 1
        );
    }
}
