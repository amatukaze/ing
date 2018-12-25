using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.Battle;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Json.Battle
{
    internal partial class BattleJson : IRawBattle
    {
        public int[] api_formation;
        public Engagement Engagement => (Engagement)api_formation.ElementAtOrDefault(2);

#pragma warning disable IDE1006 // Naming Styles
        public int[] api_f_nowhps { set => ally.fleet1.Value.NowHP = value; }
        public int[] api_f_maxhps { set => ally.fleet1.Value.MaxHP = value; }
        public int[][] api_fParam { set => ally.fleet1.Value.Param = value; }

        public int[] api_f_nowhps_combined { set => ally.fleet2.Value.NowHP = value; }
        public int[] api_f_maxhps_combined { set => ally.fleet2.Value.MaxHP = value; }
        public int[][] api_fParam_combined { set => ally.fleet2.Value.Param = value; }

        public ShipInfoId[] api_ship_ke { set => enemy.fleet1.Value.Id = value; }
        public int[] api_ship_lv { set => enemy.fleet1.Value.Level = value; }
        public int[] api_e_nowhps { set => enemy.fleet1.Value.NowHP = value; }
        public int[] api_e_maxhps { set => enemy.fleet1.Value.MaxHP = value; }
        public EquipmentInfoId[][] api_eSlot { set => enemy.fleet1.Value.Equipment = value; }
        public int[][] api_eParam { set => enemy.fleet1.Value.Param = value; }

        public ShipInfoId[] api_ship_ke_combined { set => enemy.fleet2.Value.Id = value; }
        public int[] api_ship_lv_combined { set => enemy.fleet2.Value.Level = value; }
        public int[] api_e_nowhps_combined { set => enemy.fleet2.Value.NowHP = value; }
        public int[] api_e_maxhps_combined { set => enemy.fleet2.Value.MaxHP = value; }
        public EquipmentInfoId[][] api_eSlot_combined { set => enemy.fleet2.Value.Equipment = value; }
        public int[][] api_eParam_combined { set => enemy.fleet2.Value.Param = value; }
#pragma warning restore IDE1006 // Naming Styles

        public int[] api_escape_idx;
        public IReadOnlyList<int> EscapedIndices => api_escape_idx.Select(x => x - 1).ToArray();

        public class Ship : IRawShipInBattle
        {
            public ShipInfoId Id { get; set; }
            public int Level { get; set; }
            public int nowhp, maxhp;
            public ClampedValue HP => (nowhp, maxhp);
            public IReadOnlyList<EquipmentInfoId> Equipment { get; set; }
            public int[] param;
            public int Firepower => param.ElementAtOrDefault(0);
            public int Torpedo => param.ElementAtOrDefault(1);
            public int AntiAir => param.ElementAtOrDefault(2);
            public int Armor => param.ElementAtOrDefault(3);
        }

        internal class ShipOwner
        {
            [JsonProperty("api_nowhps")]
            public IList<int> NowHP { set => value.AlignSet(Ships, (s, v) => s.nowhp = v); }
            [JsonProperty("api_maxhps")]
            public IList<int> MaxHP { set => value.AlignSet(Ships, (s, v) => s.maxhp = v); }
            [JsonProperty("api_ship_id")]
            public IList<ShipInfoId> Id { set => value.AlignSet(Ships, (s, v) => s.Id = v); }
            [JsonProperty("api_ship_lv")]
            public IList<int> Level { set => value.AlignSet(Ships, (s, v) => s.Level = v); }
            [JsonProperty("api_Slot")]
            public IList<EquipmentInfoId[]> Equipment { set => value.AlignSet(Ships, (s, v) => s.Equipment = v); }
            [JsonProperty("api_Param")]
            public IList<int[]> Param { set => value.AlignSet(Ships, (s, v) => s.param = v); }

            public List<Ship> Ships { get; } = new List<Ship>();
        }

        internal class Side : IRawSide
        {
            private readonly BattleJson owner;
            private readonly int index;
            public Side(BattleJson owner, int index)
            {
                this.owner = owner;
                this.index = index;
            }

            public Formation Formation => (Formation)owner.api_formation.ElementAtOrDefault(index);
            public Lazy<ShipOwner> fleet1, fleet2;
            public IReadOnlyList<IRawShipInBattle> Fleet => fleet1.IsValueCreated ? fleet1.Value.Ships : null;
            public IReadOnlyList<IRawShipInBattle> Fleet2 => fleet2.IsValueCreated ? fleet2.Value.Ships : null;
            public Detection? Detection => owner.api_search.ElementAtOrDefault(index);
            public EquipmentInfoId? NightTouchingId => owner.api_touch_plane.ElementAtOrDefault(index);
            public int? FlareIndex
            {
                get
                {
                    var i = owner.api_flare_pos.ElementAtOrDefault(index);
                    return i > 0 ? i - 1 : (int?)null;
                }
            }
        }

        internal ShipOwner api_friendly_info;
        public IReadOnlyList<IRawShipInBattle> NpcFleet => api_friendly_info?.Ships;

        public Detection[] api_search;
        public EquipmentInfoId?[] api_touch_plane;
        public int[] api_flare_pos;

        private readonly Side ally;
        public IRawSide Ally => ally;

        private readonly Side enemy;
        public IRawSide Enemy => enemy;

        public BattleJson()
        {
            ally = new Side(this, 0);
            enemy = new Side(this, 1);
        }
    }
}
